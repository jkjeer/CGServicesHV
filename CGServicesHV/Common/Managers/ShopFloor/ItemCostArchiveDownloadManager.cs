using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using NLog;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using static CGServicesHV.Common.Utilities.Sql;

namespace CGServicesHV.Common.Managers.Process
{
    public static class ItemCostArchiveDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum = 0)
        {
            var response = new ProcessResponse("Item Cost Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new ItemCostArchiveTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ItemCostArchive") : null;
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
                    var reader = GetData(session, processRunSpan, plant, itemNum);
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
        * EVENTS
        ********************************************************************************/
        private static iDB2Command GetData(SqlFilters filters)
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
                        CLITM,
                        CLMCU,
                        CLLOCN,
                        CLLOTN,
                        CLLOTG,
                        CLLEDG,
                        CLUNCS,
                        CLCSPO,
                        CLCSIN,
                        CLURCD,
                        CLURDT,
                        CLURAT,
                        CLURAB,
                        CLURRF,
                        CLUSER,
                        CLPID,
                        CLJOBN,
                        CLUPMJ,
                        CLTDAY,
                        CLCCFL,
                        CLCRCS,
                        CLOSTC,
                        CLSTOC,
                        CLUKID,
                        CLMUPM,
                        CLUPMT
                    FROM 
                        F584105
                        INNER JOIN F4101
                        ON IMITM = CLITM
                    WHERE
                        IMSTKT <> 'C' AND
                        {filters}
                   	FOR READ ONLY
                "
            };
            return cmd;
        }

        public static IDataReader GetData(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant, decimal customerNum)
        {
            var filters = new SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add(new SqlFilter("CLMCU = ?", "AND"));
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(new SqlFilter(E1TimeManager.GetE1DateTimeQueryString("CLUPMJ", "CLTDAY", processRunSpan.E1TimeIdSpan), "AND"));
                }

                // Command
                var cmd = GetData(filters);
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
    }
}
