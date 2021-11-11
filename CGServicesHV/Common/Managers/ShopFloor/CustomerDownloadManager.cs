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
    public class CustomerDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public CustomerDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"AddressBookDownloadManager.GetRelatedOrders", GetRelatedOrders()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, decimal addressNum = 0)
        {
            var response = new ProcessResponse("Customer Download");
            var customerTable = new CustomerTable();
            var highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(e1Session, cnSFInt, plant, response.ProcessName) : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(e1Session));
            //var responseTime1 = new ResponseTime(response.ProcessName, "Get", e1Session.DbConnection.ConnectionString);
            var reader = GetReader(e1Session, processRunSpan, plant.PlantNum);
            //response.ResponseTimes.Add(responseTime1.Done());
            customerTable.Add(reader);
            if (customerTable.Rows.Count > 0)
            {
                //var responseTime2 = new ResponseTime(response.ProcessName, "Download", cnSFInt.ConnectionString);
                var bulkResponse = BulkCopyManager.BulkTable(customerTable.ExtendedProperties["Target"].ToString(), customerTable, cnSFInt);
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

        private static iDB2Command GetCustomersDynamic(IReadOnlyCollection<string> filters)
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
                CommandText = $@"
	                SELECT 
	                    ABAN8,
                        ABAT1,
                        DRDL01,
                        ABALPH,
                        ALADD1,
                        ALADD2,
                        ALADD3,
                        ALADD4,
                        ALADDZ,
                        ALCTY1,
                        ALADDS,
                        ALCTR,
                        WPAR1,
                        WPPH1,
                        COALESCE(MAPA8,ABAN8) AS MAPA8,
                        ABAC05,
                        ABAC29,
                        ABUPMJ,
                        ABUPMT
                    FROM F0101
	                    LEFT OUTER JOIN F0116 ON ALAN8 = ABAN8 AND
				                                 ALEFTB = ABEFTB
	                    LEFT OUTER JOIN F0115 ON WPAN8 = ABAN8 AND
				                                 WPIDLN = 0 AND
				                                 WPRCK7 = 1 AND
				                                 WPCNLN = 0
	                    LEFT OUTER JOIN F0150 ON MAAN8 = ABAN8
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 ON DRSY = '01' AND
                                                                                                  DRRT = 'ST' AND
                                                                                                  TRIM(DRKY) = ABAT1
                {filterString}"
            };
            return cmd;
        }

        private static iDB2Command GetCustomersDynamicPlant(IReadOnlyCollection<string> filters)
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
                CommandText = $@"
	                SELECT 
	                    ABAN8,
                        ABAT1,
                        DRDL01,
                        ABALPH,
                        ALADD1,
                        ALADD2,
                        ALADD3,
                        ALADD4,
                        ALADDZ,
                        ALCTY1,
                        ALADDS,
                        ALCTR,
                        WPAR1,
                        WPPH1,
                        COALESCE(MAPA8,ABAN8) AS MAPA8,
                        ABAC05,
                        ABAC29,
                        ABUPMJ,
                        ABUPMT
                    FROM F0101
	                    LEFT OUTER JOIN F0116 ON ALAN8 = ABAN8 AND
				                                 ALEFTB = ABEFTB
	                    LEFT OUTER JOIN F0115 ON WPAN8 = ABAN8 AND
				                                 WPIDLN = 0 AND
				                                 WPRCK7 = 1 AND
				                                 WPCNLN = 0
	                    LEFT OUTER JOIN F0150 ON MAAN8 = ABAN8
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 ON DRSY = '01' AND
                                                                                                  DRRT = 'ST' AND
                                                                                                  TRIM(DRKY) = ABAT1
                        INNER JOIN (SELECT DISTINCT CPSHAN
                                    FROM F580053
                                    WHERE CPSHAN > 0 AND
                                          CPMCU = ?) x ON CPSHAN = ABAN8
                {filterString}"
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string plantNum = null, string[] searchTypes = null, decimal addressNum = 0)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Search Types
                if (searchTypes != null && searchTypes.Any())
                {
                    var strInClause = "";
                    foreach (var productCode in searchTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("SearchTypes", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = productCode;
                    }
                    filters.Add("ABAT1 IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }
                // Address Num
                if (addressNum > 0)
                {
                    filters.Add("ABAN8 = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = addressNum;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        cmd = GetCustomersDynamicPlant(filters);
                        cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                    }
                    else
                    {
                        cmd = GetCustomersDynamic(filters);
                    }
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        cmd = GetCustomersDynamicPlant(filters);
                        cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                    }
                    else
                    {
                        cmd = GetCustomersDynamic(filters);
                    }
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
