using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServices.Common.Managers.E1;
using CGServices.Common.Managers.Enterprise;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Types.Response;
using CGServices.Common.Types.ShopFloor;
using CGServices.Common.Utilities;
using NLog;

namespace CGServices.Common.Managers.ShopFloor
{
    public class UdcItemDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UdcItemDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"UdcItemDownloadManager.GetUdcItem", GetUdcItem()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, string[] productCodes = null, string userCode = null)
        {
            var response = new ProcessResponse("UdcItem Download");
            var udcItemTable = new UdcItemTable();
            var highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(e1Session, cnSFInt, plant, response.ProcessName) : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(e1Session));
            //var responseTime1 = new ResponseTime(response.ProcessName, "Get", e1Session.DbConnection.ConnectionString);
            var reader = GetReader(e1Session, processRunSpan, productCodes, userCode);
            //response.ResponseTimes.Add(responseTime1.Done());
            udcItemTable.Add(reader);
            if (udcItemTable.Rows.Count > 0)
            {
                //var responseTime2 = new ResponseTime(response.ProcessName, "Download", cnSFInt.ConnectionString);
                var bulkResponse = BulkCopyManager.BulkTable(udcItemTable.ExtendedProperties["Target"].ToString(), udcItemTable, cnSFInt);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                if (useHighWaterMark)
                {
                    highWaterMark.EnterpriseDateTime = highWaterMark.StartDateTimeE1;
                    response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cnSFInt, highWaterMark));
                }
                //response.ResponseTimes.Add(responseTime2.Done());
            }
            return response;
        }

        private static iDB2Command GetUdcItem()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
                        DRSY,
                        DRRT,
                        DTDL01,
                        DRKY,
                        DRDL01,
                        DRDL02,
                        DRSPHD,
                        DRUPMJ,
                        DRUPMT
                    FROM
                        {ConfigurationManager.AppSettings["ControlLib"]}.F0005 
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0004
                        ON DRSY = DTSY AND
                           DRRT = DTRT
                    WHERE 
                        DRSY = ? AND
                        DRRT = ? AND
                        TRIM(DRKY) = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("DRSY", iDB2DbType.iDB2Graphic, 4));
            cmd.Parameters.Add(new iDB2Parameter("DRRT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("DRKY", iDB2DbType.iDB2Graphic, 10));
            return cmd;
        }

        private static iDB2Command GetUdcItemsDynamic(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = string.Format(@"
	                SELECT 
                        DRSY,
                        DRRT,
                        DTDL01,
                        DRKY,
                        DRDL01,
                        DRDL02,
                        DRSPHD,
                        DRUPMJ,
                        DRUPMT
                    FROM
                        {0}.F0005 
                        LEFT OUTER JOIN {0}.F0004
                        ON DRSY = DTSY AND
                           DRRT = DTRT
                    {1}",
                    ConfigurationManager.AppSettings["ControlLib"],
                    filterString)
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string[] productCodes, string userCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Product Codes
                if (productCodes != null && productCodes.Any())
                {
                    var strInClause = "";
                    foreach (var productCode in productCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("ProductCodes", iDB2DbType.iDB2Graphic, 4));
                        parameters.Last().Value = productCode;
                    }
                    filters.Add("LEFT(DRSY,2) IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }
                // User Code
                if (!string.IsNullOrEmpty(userCode))
                {
                    filters.Add("DRRT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("UserCode", iDB2DbType.iDB2Graphic, 2));
                    parameters.Last().Value = userCode;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("DTUPMJ", "DTUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + ")" + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetUdcItemsDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetUdcItemsDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
