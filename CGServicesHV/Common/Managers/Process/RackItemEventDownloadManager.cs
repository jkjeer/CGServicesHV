using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Process
{
    public class RackItemEventDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackItemEventDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackItemEventDownloadManager.GetReceivedQty", GetReceivedQty()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal rackId, List<string> statusCodes)
        {
            var response = new ProcessResponse("Rack Item Event Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "RackItemEvent") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            if (!response.HasErrors())
            {
                var table = new RackItemEventTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var reader = GetRackItemEvent(session, processRunSpan, plant.PlantNum, rackId, statusCodes);
                    table.Add(reader, plant);
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                var step2a = new ProcessStep("Get Received Qty");
                try
                {
                    GetReceivedQty(session, plant, table);
                    step2a.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2a.Errors.Add(new Error(e));
                }
                finally
                {
                    step2a.Done();
                    response.ProcessSteps.Add(step2a);
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.RowsAffected = (int)bulkResponse.RowsCopied;
                        step3.SqlBulkCopyResponses.Add(bulkResponse);
                        step3.Done();
                        response.RowsAffected = step3.RowsAffected;
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                if (useHighWaterMark && !response.HasErrors() && table.Rows.Count > 0)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                        highWaterMark.UniqueID = (long)table.MaxId;
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime, highWaterMark.UniqueID));
                        step4.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }
            }
            response.Close();
            return response;
        }

        /********************************************************************************
        * RACK ITEM LEDGER
        ********************************************************************************/
        private static iDB2Command GetRackItemEvent(IReadOnlyCollection<string> filters, string fetchClause)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
                    DLUKID,
                    DLZ58RID,
                    DLLOTN,
                    DLZ58SN,
                    DLITM,
                    DLZ58DST,
                    DLPQOH,
                    DLZ58SD,
                    DLZ58PS,
                    DLZ58STK,
                    DLZ58ROW,
                    DLZ58SL,
                    DLZ58BXRO,
                    DLDOC1,
                    DLXDCTO,
                    DLDOC2,
                    DLXCTO,
                    DLLNIC,
                    DLZ58MDT,
                    DLMUPM,
                    DLUPMT,
                    DLIOPR
                FROM 
	                F580569
	            WHERE 
                    {filterString} 1=1
                ORDER BY
                    DLUKID
                {fetchClause}
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetRackItemEvent(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal rackId, List<string> statusCodes)
        {
            var filters = new List<string>();
            string fetchClause = "";
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("DLMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("DLZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }

                // Status Codes
                if (statusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in statusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("DLZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    if (processRunSpan.HighWaterMark.Exists)
                    {
                        filters.Add("DLUKID > ? AND \r\n");
                        parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 15;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = processRunSpan.HighWaterMark.UniqueID;
                        fetchClause = "FETCH FIRST 10000 ROWS ONLY";
                    }
                    else
                    {
                        filters.Add(E1TimeManager.GetE1DateTimeQueryString("DLMUPM", "DLUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                    }
                }

                // Command
                var cmd = GetRackItemEvent(filters, fetchClause);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
         * Received Qty
         ******************************************************************/
        public iDB2Command GetReceivedQty()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                DLPQOH,
                DLDOC1,
                DLXDCTO,
                DLDOC2,
                DLXCTO,
                DLLNIC
            FROM
                F580569
            WHERE
                DLMCU = ? AND
                DLZ58RID = ? AND
                DLZ58SN = ? AND
                DLIOPR = 'Insert'
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RID"].Precision = 15;
            cmd.Parameters["RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SN", iDB2DbType.iDB2Graphic, 20));
            return cmd;
        }

        public void GetReceivedQty(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["RackItemEventDownloadManager.GetReceivedQty"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal) dr["RackId"];
                    cmd.Parameters[2].Value = (string) dr["SerialNum"];
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["ReceivedQty"] = Data.CastDbNull.To<decimal>(reader["DLPQOH"], 0) / 1000;
                            var dldoc2 = Data.CastDbNull.To<decimal>(reader["DLDOC2"], 0);
                            if (dldoc2 > 0)
                            {
                                dr["DocumentNum"] = Data.CastDbNull.To<decimal>(reader["DLDOC2"], 0);
                                dr["DocumentType"] = Data.CastDbNull.To(reader["DLXCTO"], String.Empty).Trim();
                                dr["DocumentLineNum"] = Data.CastDbNull.To<decimal>(reader["DLLNIC"], 0) / 1000;
                            }
                            else
                            {
                                dr["DocumentNum"] = Data.CastDbNull.To<decimal>(reader["DLDOC1"], 0);
                                dr["DocumentType"] = Data.CastDbNull.To(reader["DLXDCTO"], String.Empty).Trim();
                                dr["DocumentLineNum"] = 0;
                            }
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }
}
