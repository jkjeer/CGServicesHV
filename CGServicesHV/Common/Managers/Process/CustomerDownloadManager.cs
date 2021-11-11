using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class CustomerDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************        
        * CONSTRUCTOR
        ********************************************************************************/
        public CustomerDownloadManager(BusinessSystemSession e1Session)
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

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, decimal customerNum = 0)
        {
            var response = new ProcessResponse("Customer Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new CustomerTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "Customer") : null;
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
                    var reader = GetCustomers(session, processRunSpan, plant.PlantNum, customerNum);
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
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
                        if (plant.PlantCode == "SGCG")
                        {
                            var mzcgCn = ConnectionManager.GetSqlConnectionFromKey("MZCG.INTEGRATION");
                            mzcgCn.Open();
                            var bulkResponse1 = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, mzcgCn);
                            mzcgCn.Close();
                            step3.SqlBulkCopyResponses.Add(bulkResponse1);
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
        private static iDB2Command GetCustomers(IReadOnlyCollection<string> filters)
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
                    SELECT * FROM (
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
                            ABUPMT,
                            ROW_NUMBER() OVER ( PARTITION BY F0116.ALAN8 ORDER BY F0116.ALEFTB ) AS RowNumber
                        FROM 
                            F0101
	                        LEFT OUTER JOIN F0116 
                            ON  ALAN8 = ABAN8 AND
				                ALEFTB = ABEFTB
	                        LEFT OUTER JOIN F0115 
                            ON  WPAN8 = ABAN8 AND
				                WPIDLN = 0 AND
				                WPRCK7 = 1 AND
				                WPCNLN = 0
	                        LEFT OUTER JOIN F0150 
                            ON  MAAN8 = ABAN8
                            LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 
                            ON  DRSY = '01' AND
                                DRRT = 'ST' AND
                                TRIM(DRKY) = ABAT1
                        WHERE
                            {filterString}
                            EXISTS (SELECT 1 FROM F580053 WHERE CPSHAN = ABAN8 AND CPMCU = ?)
                    ) x WHERE RowNumber = 1"
            };
            return cmd;
        }

        public static IDataReader GetCustomers(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal customerNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Customer Num
                if (customerNum > 0)
                {
                    filters.Add("ABAN8 = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = customerNum;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                // Command
                var cmd = GetCustomers(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
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
