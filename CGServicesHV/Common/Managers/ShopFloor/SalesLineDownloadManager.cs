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
    public class SalesLineDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public SalesLineDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"InventoryLocationDownloadManager.GetRelatedOrders", GetRelatedOrders()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns,
            Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal salesOrderNum, List<string> nextStatusCodes, List<string> orderTypes)
        {
            var response = new ProcessResponse("Sales Line Download Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new SalesLineTable();
            //var tableArch = new SalesLineTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark)
                    ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "SalesLine")
                    : null;
                processRunSpan =
                    ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
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

            /********************************************************/
            var stepGetArchive = new ProcessStep("Get Archive Data");
            try
            {
                var reader = GetSalesLineArchive(session, processRunSpan, plant.PlantNum, salesOrderNum,
                    nextStatusCodes, orderTypes);
                table.Add(reader, plant);
                stepGetArchive.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                stepGetArchive.Errors.Add(new Error(e));
            }
            finally
            {
                stepGetArchive.Done();
                response.AddStep(stepGetArchive);
            }

            /********************************************************/
            var stepGetCurrent = new ProcessStep("Get Source Data");
            try
            {
                var reader = GetSalesLine(session, processRunSpan, plant.PlantNum, salesOrderNum, nextStatusCodes, orderTypes);
                table.Add(reader, plant);
                stepGetCurrent.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                stepGetCurrent.Errors.Add(new Error(e));
            }
            finally
            {
                stepGetCurrent.Done();
                response.AddStep(stepGetCurrent);
            }

            /********************************************************/
            var stepBulkCurrent = new ProcessStep("Bulk Copy");
            var bulkResponse = new SqlBulkCopyResponse();
            try
            {
                if (table.Rows.Count > 0)
                {
                    bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),
                        table, sqlCns["PLANT.INTEGRATION"]);
                }
            }
            catch (Exception e)
            {
                stepBulkCurrent.Errors.Add(new Error(e));
            }
            finally
            {
                stepBulkCurrent.RowsAffected = (int)bulkResponse.RowsCopied;
                stepBulkCurrent.SqlBulkCopyResponses.Add(bulkResponse);
                stepBulkCurrent.Done();
                response.RowsAffected += stepBulkCurrent.RowsAffected;
                response.AddStep(stepBulkCurrent);
            }

            /********************************************************/
            if (useHighWaterMark)
            {
                var stepMark = new ProcessStep("Update High Water Mark");
                try
                {
                    if (table.Rows.Count > 0)
                    {
                        stepMark.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(
                            sqlCns["PLANT.INTEGRATION"],
                            plant, highWaterMark, table.MaxDate.AddMinutes(-plant.BusinessSystemOffset)));
                    }
                }
                catch (Exception e)
                {
                    stepMark.Errors.Add(new Error(e));
                }
                finally
                {
                    stepMark.Done();
                    response.AddStep(stepMark);
                }
            }
            response.Close();
            return response;
        }

        /********************************************************************************
        * EVENTS
        ********************************************************************************/
        public static IDataReader GetSalesLine(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal salesOrderNum, List<string> nextStatusCodes, List<string> orderTypes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("SDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("SDUPMJ", "SDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // SalesOrderNum
                if (salesOrderNum > 0)
                {
                    filters.Add("SDDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = salesOrderNum;
                }

                // Status Codes
                if (nextStatusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in nextStatusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("NXTR", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("SDNXTR IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Order Types
                if (orderTypes.Any())
                {
                    var strInClause = "";
                    foreach (var orderType in orderTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
                        parameters.Last().Value = orderType;
                    }
                    filters.Add("SDDCTO IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Command
                var cmd = GetSalesLine(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        private static iDB2Command GetSalesLine(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters) { filterString.Append(filter); }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT 
                    SDKCOO,
	                SDDOCO,
	                SDDCTO,
	                SDLNID,
	                SDRORN,
	                SDRCTO,
                    SDRLLN,
	                SDTRDJ,
	                SDDRQJ,
                    SDOPDJ,
	                SDCNDJ,
                    SDADDJ,
	                SDIVD,
	                SDITM,
	                SDLNTY,
	                SDNXTR,
	                SDLTTR,
	                SDAN8,
	                SDSHAN,
	                SDUORG,
	                SDUNCS,
	                SDECST,
	                SDUPRC,
                    SDLPRC,
	                SDAEXP,
	                SDDOC,
	                SDGLC,
	                SDRCD,
                    SDURCD,
	                SDVR01,
                    SDPSN,
	                SDUPMJ,
	                SDTDAY,
                    SDDSC1,
                    SDDSC2,
                    SDPROV,
                    SDPDDJ,
                    SDDRQT,
	                ATZ58PT,
	                ATZ58CSQNO,
	                ATZ58GSQNO,
	                ATZ58CP2,
	                ATZ58CP3,
	                ATZ58CP4,
	                ATZ5840460,
                    ATZ58USF,
                    ATZ58BSF,
                    ATZ58ZPO,
	                ATUPMJ,
	                ATTDAY,	  
                    ATZ58WODC,
                    COALESCE(CMCFGID,0) AS CMCFGID,
	                CMUPMJ,
	                CMTDAY,
                    BHZ58GDES,
                    BHZ58BDES
                FROM
                   F4211
                   LEFT OUTER JOIN F580032
                   ON ATKCOO = SDKCOO AND
	                ATDOCO = SDDOCO AND
	                ATDCTO = SDDCTO AND
	                ATLNID = SDLNID
                   LEFT OUTER JOIN F3201
                   ON CMKCOO = SDKCOO AND
	                CMDOCO = SDDOCO AND
	                CMDCTO = SDDCTO AND
	                CMLNID = SDLNID
                    LEFT OUTER JOIN F580232 
                    ON	SDKCOO = BDKCOO AND
                    SDDOCO = BDDOCO AND
                    SDDCTO = BDDCTO AND
                    SDLNID = BDLNID
					LEFT OUTER JOIN F580230 	
					ON	BHMCU = BDMCU AND
						BHZ58BNUM = BDZ58BNUM
	            WHERE 
                    {filterString}
                    1=1
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetSalesLineArchive(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal salesOrderNum, List<string> nextStatusCodes, List<string> orderTypes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("SDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    //var dts = new List<string> { "SDUPMJ", "ATUPMJ", "CMUPMJ" };
                    //var tms = new List<string> { "SDTDAY", "ATTDAY", "CMTDAY" };
                    //filters.Add(E1TimeManager.GetE1DateTimeQueryString(dts, tms, processRunSpan.StartDateTime, processRunSpan.EndDateTime) + " AND \r\n");
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("SDUPMJ", "SDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // SalesOrderNum
                if (salesOrderNum > 0)
                {
                    filters.Add("SDDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = salesOrderNum;
                }

                // Order Types
                if (orderTypes.Any())
                {
                    var strInClause = "";
                    foreach (var orderType in orderTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
                        parameters.Last().Value = orderType;
                    }
                    filters.Add("SDDCTO IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Status Codes
                if (nextStatusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in nextStatusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("NXTR", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("SDNXTR IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Command
                var cmd = GetSalesLineArchive(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        private static iDB2Command GetSalesLineArchive(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters) { filterString.Append(filter); }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT 
                    SDKCOO,
	                SDDOCO,
	                SDDCTO,
	                SDLNID,
	                SDRORN,
	                SDRCTO,
                    SDRLLN,
	                SDTRDJ,
	                SDDRQJ,
                    SDOPDJ,
	                SDCNDJ,
                    SDADDJ,
	                SDIVD,
	                SDITM,
	                SDLNTY,
	                SDNXTR,
	                SDLTTR,
	                SDAN8,
	                SDSHAN,
	                SDUORG,
	                SDUNCS,
	                SDECST,
	                SDUPRC,
                    SDLPRC,
	                SDAEXP,
	                SDDOC,
	                SDGLC,
	                SDRCD,
                    SDURCD,
	                SDVR01,
                    SDPSN,
	                SDUPMJ,
	                SDTDAY,
                    SDDSC1,
                    SDDSC2,
                    SDPROV,
                    SDPDDJ,
                    SDDRQT,
	                ATZ58PT,
	                ATZ58CSQNO,
	                ATZ58GSQNO,
	                ATZ58CP2,
	                ATZ58CP3,
	                ATZ58CP4,
	                ATZ5840460,                    
                    ATZ58USF,
                    ATZ58BSF,
                    ATZ58ZPO,
	                ATUPMJ,
	                ATTDAY,	    
                    ATZ58WODC,
                    COALESCE(CMCFGID,0) AS CMCFGID,
	                CMUPMJ,
	                CMTDAY,
                    BHZ58GDES,
                    BHZ58BDES
                FROM
                   F42119
                   LEFT OUTER JOIN F580032
                   ON ATKCOO = SDKCOO AND
	                ATDOCO = SDDOCO AND
	                ATDCTO = SDDCTO AND
	                ATLNID = SDLNID
                   LEFT OUTER JOIN F3201
                   ON CMKCOO = SDKCOO AND
	                CMDOCO = SDDOCO AND
	                CMDCTO = SDDCTO AND
	                CMLNID = SDLNID
                    LEFT OUTER JOIN F580232 
                    ON	SDKCOO = BDKCOO AND
                    SDDOCO = BDDOCO AND
                    SDDCTO = BDDCTO AND
                    SDLNID = BDLNID
					LEFT OUTER JOIN F580230 	
					ON	BHMCU = BDMCU AND
						BHZ58BNUM = BDZ58BNUM
	            WHERE 
                    {filterString}
                    1=1
                FOR READ ONLY"
            };
            return cmd;
        }
    }
}


