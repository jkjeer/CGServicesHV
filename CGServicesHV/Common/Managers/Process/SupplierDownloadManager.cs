using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace CGServicesHV.Common.Managers.Process
{
    public class SupplierDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public SupplierDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"UdcItemDownloadManager.GetUdcItem", GetUdcItem()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, decimal supplierNum = 0)
        {
            var response = new ProcessResponse("Supplier Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new SupplierTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "Supplier") : null;
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
                    var reader = GetReader(session, plant.PlantNum, processRunSpan, supplierNum);
                    table.Add(reader, plant);
                    Logger.Info($"Rows: {table.Rows.Count}  Plant: {plant.PlantCode}");
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
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime));
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
        * EVENTS
        ********************************************************************************/
        private static iDB2Command GetReader(IReadOnlyCollection<string> filters)
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
                            ,DRUPMJ
                            ,DRUPMT
                            ,WPUPMJ
                            ,WPUPMT
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

        public static IDataReader GetReader(BusinessSystemSession session, string plantNum, ProcessRunSpan processRunSpan, decimal supplierNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Supplier Num
                if (supplierNum > 0)
                {
                    filters.Add("ABAN8 = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = supplierNum;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("ALUPMJ", "ALUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan);
                    //var queryString3 = E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("A6UPMJ", "A6UPMT", processRunSpan.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    //filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + ")" + " AND \r\n");
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString4 + " OR " + queryString5 + ")" + " AND \r\n");
                }
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                }
                // Command
                var cmd = GetReader(filters);
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
                throw new Exception(e.Message);
            }
        }
    }
}
