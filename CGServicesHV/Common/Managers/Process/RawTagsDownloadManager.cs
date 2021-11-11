using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace CGServicesHV.Common.Managers.Process
{
    public class RawTagsDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RawTagsDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RawTagsDownloadManager.GetReceivedQty", GetReceivedQty()}
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
            var response = new ProcessResponse("Raw Tags Download Manager");
            var table = new RawTagsTable();
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
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "RawTag") : null;
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

                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    Logger.Info($@"GetRawTags PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                    var reader = GetRawTags(session, processRunSpan, plant, rackId, statusCodes);
                    table.Add(reader, plant);
                    Logger.Info($"Rows: {table.Rows.Count}");
                    Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
                        }
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
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime));
                        step4.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
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
        private static iDB2Command GetRawTags(IReadOnlyCollection<string> filters)
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
                            REMCU,
                            RELITM,
                            REZ58SN,
                            REZ58RID,
                            RESEQNO,
                            REVEND,
                            RELOCN,
                            REEDSP,
                            REZ58DST,
                            REZ58L1TK,
                            REZ58L11C,
                            REZ58L1CL,
                            REZ58L1HT,
                            REZ58L12C,
                            REZ58L1PX,
                            REZ58L1ALT,
                            REZ58L1LPT,
                            REZ58L1PQL,
                            REZ58SHAPE,
                            REZ58L1BDM,
                            REZ58L1BNM,
                            REZ58L1BDN,
                            REZ58L1LDM,
                            REZ58L1LNM,
                            REZ58L1LDN,
                            REZ58L1RDM,
                            REZ58L1RNM,
                            REZ58L1RDN,
                            REZ58L1TDM,
                            REZ58L1TNM,
                            REZ58L1TDN,
                            REZ58L11DM,
                            REZ58L11NM,
                            REZ58L11DN,
                            REZ58L12DM,
                            REZ58L12NM,
                            REZ58L12DN,
                            REZ581BMIM,
                            REZ581LMIM,
                            REZ58MDT,
                            REPQOH,
                            REZ58BSF,
                            REZ58USF,
                            REUSER,
                            REPID,
                            REJOBN,
                            REUPMJ,
                            RETDAY,
                            REZ58RT,
                            REZ58LPR,
                            REGLPT,
                            REZ58RCD,
                            REZ58AID
                        FROM 
	                        F580003A
	                    WHERE 
                            {filterString}
                    FOR READ ONLY
                "
            };
            return cmd;
        }

        public static IDataReader GetRawTags(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant, decimal rackId, List<string> statusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {

                // Cannot run wide Open
                if (!statusCodes.Any() && processRunSpan.IsEmpty && rackId == 0)
                {
                    filters.Add("0 = 1 AND \r\n");
                }

                filters.Add("REMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("REZ58RID = ? AND \r\n");
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
                    filters.Add("REZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("REUPMJ", "RETDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                var cmd = GetRawTags(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

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
                DLIOPR = 'Insert'";
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
                var cmd = session.DbCommands["RackItemDownloadManager.GetReceivedQty"];
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
                            var dldoc2 = CastDbNull.To<decimal>(reader["DLDOC2"], 0);
                            if (dldoc2 > 0)
                            {
                                dr["DocumentNum"] = CastDbNull.To<decimal>(reader["DLDOC2"], 0);
                                dr["DocumentType"] = CastDbNull.To(reader["DLXCTO"], String.Empty).Trim();
                                dr["DocumentLineNum"] = CastDbNull.To<decimal>(reader["DLLNIC"], 0) / 1000;
                            }
                            else
                            {
                                dr["DocumentNum"] = CastDbNull.To<decimal>(reader["DLDOC1"], 0);
                                dr["DocumentType"] = CastDbNull.To(reader["DLXDCTO"], String.Empty).Trim();
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
