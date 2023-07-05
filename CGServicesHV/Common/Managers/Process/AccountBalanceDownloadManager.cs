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
    public class AccountBalanceDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, int century, int year)
        {
            var response = new ProcessResponse("Account Balance Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new AccountBalanceTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "AccountBalance") : null;
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
                    var reader = GetTransactionAggregates(session, processRunSpan, plant, century, year);
                    table.AddTransactionAggregate(reader, plant);
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
                if (useHighWaterMark)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"],
                                plant, highWaterMark, table.MaxDate.AddMinutes(-plant.BusinessSystemOffset)));
                        }
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
        * Balances
        ********************************************************************************/
        public static IDataReader GetBalances(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant, int century, int year)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add("GMMCU = ? AND\r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Century
                filters.Add("GBCTRY = ? AND\r\n");
                parameters.Add(new iDB2Parameter("CTRY", iDB2DbType.iDB2Decimal));
                parameters.Last().Precision = 2;
                parameters.Last().Scale = 0;
                parameters.Last().Value = century;

                // Year
                filters.Add("GBFY = ? AND\r\n");
                parameters.Add(new iDB2Parameter("FY", iDB2DbType.iDB2Decimal));
                parameters.Last().Precision = 2;
                parameters.Last().Scale = 0;
                parameters.Last().Value = year;

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("GBUPMJ", "GBUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                var cmd = GetBalances(filters);
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

        private static iDB2Command GetBalances(IReadOnlyCollection<string> filters)
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
                        GBFY,
                        GBCTRY,
                        GBAID,
                        GBSBL,
                        GBLT,
                        GBAPYC,
                        GBAN01,
                        GBAN02,
                        GBAN03,
                        GBAN04,
                        GBAN05,
                        GBAN06,
                        GBAN07,
                        GBAN08,
                        GBAN09,
                        GBAN10,
                        GBAN11,
                        GBAN12,
                        GBUPMJ,
                        GBUPMT,
                        GMMCU,
                        GMOBJ,
                        GMSUB,
                        GMDL01
                    FROM
                        F0902
                        INNER JOIN F0901
                        ON GMAID = GBAID
                    WHERE
                    {filterString} 
                    GBLT = 'AA' AND
                    GMOBJ IN ('5510','5520','5530','6020','6040','6050','6070','6260','6310','6320','6350','6370','6010','6100','6110','6120','6130','6200','6205','6210','6215','6220','6221','6230','6240','6250','6270','6300','6305','6340','6375','6400' )"
            };
            return cmd;
        }

        /********************************************************************************
        * Transaction Aggregates
        ********************************************************************************/
        public static IDataReader GetTransactionAggregates(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant, int century, int year)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add("GMMCU = ? AND\r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Century
                filters.Add("GLCTRY = ? AND\r\n");
                parameters.Add(new iDB2Parameter("CTRY", iDB2DbType.iDB2Decimal));
                parameters.Last().Precision = 2;
                parameters.Last().Scale = 0;
                parameters.Last().Value = century;

                // Century
                filters.Add("GLFY = ? AND\r\n");
                parameters.Add(new iDB2Parameter("FY", iDB2DbType.iDB2Decimal));
                parameters.Last().Precision = 2;
                parameters.Last().Scale = 0;
                parameters.Last().Value = year;

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("GLUPMJ", "GLUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                var cmd = GetTransactionAggregates(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
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

        private static iDB2Command GetTransactionAggregates(IReadOnlyCollection<string> filters)
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
                    GMMCU,
                    GMOBJ,
                    GMAID,
                    GMSUB,
                    GMDL01,
                    GLSBL,
                    GLLT,
                    GLFY,
                    GLCTRY,
	                GLDCT,
	                GLPN,
	                SUM(GLAA) AS GLAA
                FROM
                    F0911
                    INNER JOIN F0901
                    ON GMAID = GLAID
                WHERE
                    {filterString} 
	                GMOBJ IN ('5510','5520','5530','6020','6040','6050','6070','6260','6310','6320','6350','6370','6010','6100','6110','6120','6130','6200','6205','6210','6215','6220','6221','6230','6240','6250','6270','6300','6305','6340','6375','6400') AND
	                GLLT = 'AA'
                GROUP BY
                    GMMCU,
                    GMOBJ,
                    GMAID,
                    GMSUB,
                    GLSBL,
                    GMDL01,
                    GLLT,
                    GLFY,
                    GLCTRY,
	                GLDCT,
	                GLPN"
            };
            return cmd;
        }


    }
}
