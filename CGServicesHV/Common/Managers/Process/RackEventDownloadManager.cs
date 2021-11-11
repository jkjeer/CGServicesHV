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
    public class RackEventDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackEventDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackEventDownloadManager.GetReceivedDate", GetReceivedDate()}
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
            var response = new ProcessResponse("Rack Event Download Manager");
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
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "RackEvent") : null;
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
                var table = new RackEventTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var reader = GetRackEvent(session, processRunSpan, plant.PlantNum, rackId, statusCodes);
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

                /********************************************************/
                //var step2a = new ProcessStep("Get Received Date");
                //try
                //{
                //    GetReceivedDate(session, plant, table);
                //    step2a.RowsAffected = table.Rows.Count;
                //}
                //catch (Exception e)
                //{
                //    step2a.Errors.Add(new Error(e));
                //}
                //finally
                //{
                //    step2a.Done();
                //    response.ProcessSteps.Add(step2a);
                //}

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
        * RACK LEDGER
        ********************************************************************************/
        private static iDB2Command GetRackEvent(IReadOnlyCollection<string> filters, string fetchClause)
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
                    HLUKID,
                    HLZ58RID,
                    HLZ58RT,
                    HLZ58HST,
                    HLLOCN,
                    HLURCD,
                    HLZ58BOX,
                    HLZ58SCN,
                    HLZ58RTWT,
                    HLZ58GWTT,
                    HLURRF,
                    HLMUPM,
                    HLUPMT,
                    HLIOPR
                FROM 
	                F580559
	            WHERE 
                    {filterString}
                    HLVERS != 'DELETED'
                ORDER BY
                    HLUKID
                {fetchClause}
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetRackEvent(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal rackId, List<string> statusCodes)
        {
            var filters = new List<string>();
            string fetchClause = "";
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("HLMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("HLZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
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
                    filters.Add("HLZ58HST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    if (processRunSpan.HighWaterMark.Exists)
                    {
                        filters.Add("HLUKID > ? AND \r\n");
                        parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 15;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = processRunSpan.HighWaterMark.UniqueID;
                        fetchClause = "FETCH FIRST 10000 ROWS ONLY";
                    }
                    else
                    {
                        filters.Add(E1TimeManager.GetE1DateTimeQueryString("HLMUPM", "HLUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                    }
                }

                // Command
                var cmd = GetRackEvent(filters, fetchClause);
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
         * ReceivedDate
         ******************************************************************/
        public static iDB2Command GetReceivedDate()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                HLMUPM,
                HLUPMT
            FROM
                F580559
            WHERE
                HLMCU = ? AND
                HLZ58RID = ? AND
                HLIOPR = 'Insert'
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RID"].Precision = 15;
            cmd.Parameters["RID"].Scale = 0;
            return cmd;
        }

        public void GetReceivedDate(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["RackEventDownloadManager.GetReceivedDate"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal) dr["RackId"];
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            var dateId = Data.CastDbNull.To<decimal>(reader["HLMUPM"], 0);
                            var timeId = Data.CastDbNull.To<decimal>(reader["HLUPMT"], 0);
                            dr["ReceivedDate"] = E1TimeManager.CastToDateTime(dateId, timeId);
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
