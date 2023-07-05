using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using IBM.Data.DB2.iSeries;
using NLog;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Managers.Process
{
    public class SalesLineMessageDownloadManager
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
            Plant plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Sales Line Message Download Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new SalesLineMessageTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark)
                    ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "SalesLineMessage")
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
                var reader = GetSalesLineMessageArchive(session, plant, processRunSpan);
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
                var reader = GetSalesLineMessage(session, plant, processRunSpan);
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
        public static IDataReader GetSalesLineMessage(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add(new Sql.SqlFilter("SDMCU = ?", "AND"));
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(new Sql.SqlFilter("SDUPMJ >= ?", "AND"));
                    parameters.Add(new iDB2Parameter("UPMJ", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 6;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = processRunSpan.E1TimeIdSpan.StartDateId;

                    var parmSet = E1TimeManager.GetBusinessDateFilter("ZDUPMJ", "ZDTDAY", processRunSpan.E1TimeIdSpan);
                    filters.Add(new Sql.SqlFilter(parmSet.Item1, "AND"));
                    parameters.AddRange(parmSet.Item2);
                }

                // Command
                var cmd = GetSalesLineMessage(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        private static iDB2Command GetSalesLineMessage(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                select
	                SDDOCO,
	                SDDCTO,
	                SDLNID,
	                ZDEDSQ,
	                ZDEDTY,
	                ZDLINS,
	                ZDTXLN,
	                ZDUPMJ,
	                ZDTDAY
                from 
	                F4715
	                INNER JOIN F4211
	                ON	SDKCOO = ZDKCOO AND
		                SDDOCO = ZDDOCO AND
		                SDDCTO = ZDDCTO AND
		                SDLNID = ZDLNID
	            WHERE 
                    {filters}
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetSalesLineMessageArchive(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add(new Sql.SqlFilter("SDMCU = ?", "AND"));
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(new Sql.SqlFilter("SDUPMJ >= ?", "AND"));
                    parameters.Add(new iDB2Parameter("UPMJ", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 6;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = processRunSpan.E1TimeIdSpan.StartDateId;

                    var parmSet = E1TimeManager.GetBusinessDateFilter("ZDUPMJ", "ZDTDAY", processRunSpan.E1TimeIdSpan);
                    filters.Add(new Sql.SqlFilter(parmSet.Item1, "AND"));
                    parameters.AddRange(parmSet.Item2);
                }

                // Command
                var cmd = GetSalesLineMessageArchive(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
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

        private static iDB2Command GetSalesLineMessageArchive(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                select
	                SDDOCO,
	                SDDCTO,
	                SDLNID,
	                ZDEDSQ,
	                ZDEDTY,
	                ZDLINS,
	                ZDTXLN,
	                ZDUPMJ,
	                ZDTDAY
                from 
	                F4715
	                INNER JOIN F42119
	                ON	SDKCOO = ZDKCOO AND
		                SDDOCO = ZDDOCO AND
		                SDDCTO = ZDDCTO AND
		                SDLNID = ZDLNID
	            WHERE 
                    {filters}
                FOR READ ONLY"
            };
            return cmd;
        }
    }
}


