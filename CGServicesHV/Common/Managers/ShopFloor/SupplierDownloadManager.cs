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
    public class SupplierDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public SupplierDownloadManager(BusinessSystemSession e1Session)
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
            var response = new ProcessResponse("Supplier Download");
            var supplierTable = new SupplierTable();
            var highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(e1Session, cnSFInt, plant, response.ProcessName) : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(e1Session));
            //var responseTime1 = new ResponseTime(response.ProcessName, "Get", e1Session.DbConnection.ConnectionString);
            var reader = GetReader(e1Session, processRunSpan, addressNum);
            //response.ResponseTimes.Add(responseTime1.Done());
            supplierTable.Add(reader);
            if (supplierTable.Rows.Count > 0)
            {
                //var responseTime2 = new ResponseTime(response.ProcessName, "Download", cnSFInt.ConnectionString);
                var bulkResponse = BulkCopyManager.BulkTable(supplierTable.ExtendedProperties["Target"].ToString(), supplierTable, cnSFInt);
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

        private static iDB2Command GetSuppliersDynamic(IReadOnlyCollection<string> filters)
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
                     SELECT  F0101.ABAN8 
                            ,F0101.ABALKY 
                            ,F0101.ABTAX 
                            ,F0101.ABALPH 
                            ,F0101.ABDC 
                            ,F0101.ABMCU 
                            ,F0101.ABAT1 
                            ,F0005.DRDL01
                            ,F0101.ABTAXC 
                            ,F0101.ABAT2 
                            ,F0101.ABAT3 
                            ,F0101.ABAT4 
                            ,F0101.ABAT5 
                            ,F0101.ABATP 
                            ,F0101.ABATR 
                            ,F0101.ABATPR 
                            ,F0101.ABATE 
                            ,F0101.ABAN81 
                            ,F0101.ABAC05 
                            ,F0101.ABAC06 
                            ,F0101.ABAC08 
                            ,F0101.ABAC10 
                            ,F0101.ABAC13 
                            ,F0101.ABAC16 
                            ,F0101.ABAC18 
                            ,F0101.ABAC26 
                            ,F0101.ABAC29 
                            ,F0101.ABAC30 
                            ,F0116.ALADD1 
                            ,F0116.ALADD2 
                            ,F0116.ALADD3 
                            ,F0116.ALADD4 
                            ,F0116.ALADDZ 
                            ,F0116.ALCTY1 
                            ,F0116.ALCOUN 
                            ,F0116.ALADDS 
                            ,F0116.ALCTR
                            ,F0116.ALEFTB
                            ,F0401.A6UPMJ 
                            ,F0401.A6UPMT 
                            ,F0101.ABUPMJ 
                            ,F0101.ABUPMT 
                            ,F0116.ALUPMJ 
                            ,F0116.ALUPMT 
                            ,F0115.WPAR1
                            ,F0115.WPPH1
                            ,COALESCE(MAPA8,ABAN8) AS MAPA8
                            ,ROW_NUMBER() OVER ( PARTITION BY F0116.ALAN8 ORDER BY F0116.ALEFTB ) AS RowNumber
                    FROM    F0401 F0401
                            INNER JOIN F0101 F0101 ON F0101.ABAN8 = F0401.A6AN8 
                            LEFT OUTER JOIN ( SELECT * 
                                                FROM (
		                                                SELECT   ALAN8 
                                                                ,ALADD1 
                                                                ,ALADD2 
                                                                ,ALADD3 
                                                                ,ALADD4 
                                                                ,ALADDZ 
                                                                ,ALCTY1 
                                                                ,ALCOUN 
                                                                ,ALADDS 
                                                                ,ALCTR
                                                                ,ALEFTB
                                                                ,ALUPMJ 
                                                                ,ALUPMT 
		                                                        ,ROW_NUMBER() OVER ( PARTITION BY ALAN8 ORDER BY ALEFTB ) AS RowNumber
		                                                FROM F0116 
	                                                ) x WHERE RowNumber = 1 	
                                            ) F0116 ON F0116.ALAN8 = F0101.ABAN8 
	                        LEFT OUTER JOIN F0115 F0115 ON WPAN8 = ABAN8 AND
				                                     WPIDLN = 0 AND
				                                     WPRCK7 = 1 AND
				                                     WPCNLN = 0
	                        LEFT OUTER JOIN F0150 ON MAAN8 = ABAN8
                            LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 F0005 ON DRSY = '01' AND
                                                                                                      DRRT = 'ST' AND
                                                                                                      TRIM(DRKY) = ABAT1
                    {filterString}"
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            decimal addressNum = 0)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var filters = new List<string>();
            try
            {
                filters.Add("RowNumber = 1 AND \r\n");
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("ALUPMJ", "ALUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("A6UPMJ", "A6UPMT", processRunSpan.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString6 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + " OR " + queryString6 + ")" + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetSuppliersDynamic(filters);
                }
                else
                {
                    cmd = GetSuppliersDynamic(filters);
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
