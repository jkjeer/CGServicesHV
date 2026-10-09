using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using IBM.Data.DB2.iSeries;
using NLog;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Managers.Process
{
    public class ConfiguredOrderDownloadManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns,
            Plant plant, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark)
        {
            /********************************************************/
            var response = new ProcessResponse("Configured Order Download Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));

            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark)
                    ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ConfiguredOrder")
                    : null;
                processRunSpan =
                    ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, timeSpanEnd);
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
            var table = new ConfiguredOrderTable();
            var tableName = table.ExtendedProperties["Target"].ToString();
            var batchSize = int.Parse((string)table.ExtendedProperties["BatchSize"]);
            var maxDate = new DateTime(1900, 1, 1);

            var stepGetCurrent = new ProcessStep("Get Source Data");
            try
            {
                var reader = GetTransactions(session, processRunSpan, plant);
                var i = 0;
                while (reader.Read())
                {
                    i++;
                    table.Add((IDataRecord)reader, plant);
                    if (i % batchSize == 0)
                    {
                        logger.Debug($"Start Bulk {plant.PlantCode} i={i} batchSize={batchSize}");
                        stepGetCurrent.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                        logger.Debug($"End Bulk {plant.PlantCode} i={i} batchSize={batchSize}");
                        maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                        table.Rows.Clear();
                        logger.Debug($"End Clear {plant.PlantCode} i={i} batchSize={batchSize}");
                    }
                }
                reader.Close();
                if (table.Rows.Count > 0)
                {
                    logger.Debug($"Last Start Bulk {plant.PlantCode} i={i} batchSize={batchSize}");
                    stepGetCurrent.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                    logger.Debug($"Last End Bulk {plant.PlantCode} i={i} batchSize={batchSize}");
                    maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                    table.Rows.Clear();
                    logger.Debug($"Last End Clear {plant.PlantCode} i={i} batchSize={batchSize}");
                }
                stepGetCurrent.RowsAffected = i;
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
            if (useHighWaterMark && stepGetCurrent.RowsAffected > 0)
            {
                var stepMark = new ProcessStep("Update High Water Mark");
                try
                {
                    stepMark.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, maxDate.AddMinutes(-plant.BusinessSystemOffset)));
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
        public static IDataReader GetTransactions(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add(new Sql.SqlFilter("CMEMCU = ?", "AND"));
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Dates
                if (!processRunSpan.IsEmpty)
                {
                    var dtList = new List<string[]>
                    {
                        new string[] { "CMUPMJ", "CMTDAY" }
                    };
                    filters.Add(new Sql.SqlFilter($"{E1TimeManager.GetE1DateFilterString(dtList, processRunSpan.StartDateTime, processRunSpan.EndDateTime)}", "AND"));
                }

                // Command
                var cmd = GetTransactions(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
                cmd.Connection = session.DbConnection;
                logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        private static iDB2Command GetTransactions(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        CMCFGID,
                        CMDOCO,
                        CMDCTO,
                        CMLNID,
                        CMKCOO,
                        CMAN8,
                        CMSHAN,
                        CMUSER,
                        CMPID,
                        CMUPMJ,
                        CMTDAY,
                        CCITM
                    FROM
                        F3201
                        LEFT OUTER JOIN F3215
                        ON  CMCFGID = CCCFGID AND
                            CCCFGCID = 1
                    WHERE
                        {filters} 
                    ORDER BY
                        CMUPMJ,
                        CMTDAY
                    FOR READ ONLY
                "
            };
            return cmd;
        }
    }
}
