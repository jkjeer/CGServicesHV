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
    public class RackAssetDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackAssetDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"RackAssetDownloadManager.GetReceivedDate", GetReceivedDate()}
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
            var response = new ProcessResponse("Rack Asset Download Manager");
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
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "RackAsset") : null;
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
                var table = new RackAssetTable();
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (processRunSpan.IsEmpty)
                    {
                        Logger.Debug($@"GetRackAsset PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackAsset(session, processRunSpan, plant.PlantNum, rackId, statusCodes);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        Logger.Debug($@"GetRackAssetLedger PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackAssetLedger(session, processRunSpan, plant.PlantNum, rackId, statusCodes);
                        table.Add(reader, plant);
                    }
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
                        //step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime));
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
        * RACK ASSET
        ********************************************************************************/
        private static iDB2Command GetRackAsset(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any())
            {
                filterString.Append("WHERE ");
                foreach (var filter in filters)
                {
                    filterString.Append(filter);
                }
            }

            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        AMZ58AID AS ALZ58AID,
                        AMMCU AS ALMCU,
                        AMLOCN AS ALLOCN,
                        AMZ58AST AS ALZ58AST,
                        AMAN8 AS ALAN8,
                        AMSHAN AS ALSHAN,
                        AMZ58ATY AS ALZ58ATY,
                        AMZ58RT AS ALZ58RT,
                        AMZ58RID AS ALZ58RID,
                        AMZ58ACD AS ALZ58ACD,
                        AMZ58RDT AS ALZ58RDT,
                        AMSHPJ AS ALSHPJ,
                        AMRCDJ AS ALRCDJ,
                        AMUPMJ AS ALMUPM,
                        AMUPMT AS ALTDAY,
                        CAST(0 AS DECIMAL(15,0)) AS ALUKID,
                        '''' AS ALIOPR
                    FROM 
	                    F581201
                    {filterString}
                    FOR READ ONLY
                    "
            };
            return cmd;
        }

        public static IDataReader GetRackAsset(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal rackId, List<string> statusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {

                // Plant
                //filters.Add("AMMCU = ? AND \r\n");
                //parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                //parameters.Last().Value = plantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("AMZ58RID = ? AND \r\n");
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
                    filters.Add("AMZ58AST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("AMUPMJ", "AMUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                if (filters.Any()) filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                var cmd = GetRackAsset(filters);
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

        /********************************************************************************
        * RACK ASSET LEDGER
        ********************************************************************************/
        private static iDB2Command GetRackAssetLedger(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any())
            {
                filterString.Append("WHERE ");
                foreach (var filter in filters)
                {
                    filterString.Append(filter);
                }
            }

            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT * FROM (
                        SELECT 
                            ALZ58AID,
                            ALMCU,
                            ALLOCN,
                            ALZ58AST,
                            ALAN8,
                            ALSHAN,
                            ALZ58ATY,
                            ALZ58RT,
                            ALZ58RID,
                            ALZ58ACD,
                            ALZ58RDT,
                            ALSHPJ,
                            ALRCDJ,
                            ALMUPM,
                            ALTDAY,
                            ALUKID,
                            ALIOPR,
                            ROW_NUMBER() OVER (PARTITION BY ALZ58AID ORDER BY ALUKID DESC) AS RN
                        FROM 
	                        F5812019
                        {filterString}) A
                    WHERE A.RN = 1
                    FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetRackAssetLedger(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal rackId, List<string> statusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {

                // Plant
                //filters.Add("ALMCU = ? AND \r\n");
                //parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                //parameters.Last().Value = plantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("ALZ58AID = ? AND \r\n");
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
                    filters.Add("ALZ58AST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // High Water Mark
                if (processRunSpan.HighWaterMark.Exists)
                {
                    var ukid = processRunSpan.HighWaterMark.UniqueID;
                    filters.Add($"ALUKID >= {ukid} AND ALUKID < ( {ukid} + 1000000 ) AND \r\n");
                    //parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
                    //parameters.Last().Precision = 15;
                    //parameters.Last().Scale = 0;
                    //parameters.Last().Value = processRunSpan.HighWaterMark.UniqueID;
                    //filters.Add("ALUKID < ( ? + 1000000 ) AND \r\n");
                    //parameters.Add(new iDB2Parameter("UKID2", iDB2DbType.iDB2Decimal));
                    //parameters.Last().Precision = 15;
                    //parameters.Last().Scale = 0;
                    //parameters.Last().Value = processRunSpan.HighWaterMark.UniqueID;
                }
                else
                {
                    if (!processRunSpan.IsEmpty)
                    {
                        filters.Add(E1TimeManager.GetE1DateTimeQueryString("ALMUPM", "ALUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                    }
                }

                // Command
                filters[filters.Count - 1] = filters.Last().Replace(" AND \r\n", "");
                var cmd = GetRackAssetLedger(filters);
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
    }
}
