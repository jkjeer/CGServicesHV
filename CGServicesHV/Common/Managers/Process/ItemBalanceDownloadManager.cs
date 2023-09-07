using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    public static class ItemBalanceDownloadManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Item Balance Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new ItemBalanceTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ItemBalance") : null;
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
                    var reader = GetBalances(session, plant, processRunSpan);
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
        * SQL
        ********************************************************************************/
        public static iDB2Command GetBalances(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT
	            LIMCU,
	            LIITM,
	            LILOCN,
	            LILOTN,
	            LIPBIN,
	            LIGLPT,
	            LILOTS,
	            LILRCJ,
	            LIPQOH,
	            LIUSER,
	            LIPID,
	            LIUPMJ,
	            LITDAY
            FROM
	            F41021
	            INNER JOIN F4101
	            ON	IMITM = LIITM
            WHERE 
                {filters}";
            return cmd;
        }

        public static IDataReader GetBalances(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add(new Sql.SqlFilter($"LIMCU = '{plant.PlantNum.PadLeft(12)}'", "AND"));

                // Discrete Items Only
                filters.Add(new Sql.SqlFilter($"IMSTKT NOT IN ('C')", "AND"));

                // Dates
                if (!processRunSpan.IsEmpty)
                {
                    var dtList = new List<string[]>
                    {
                        new string[] { "LIUPMJ", "LITDAY" }
                    };
                    filters.Add(new Sql.SqlFilter($"{E1TimeManager.GetE1DateFilterString(dtList, processRunSpan.StartDateTime, processRunSpan.EndDateTime)}", "AND"));
                }
                // Command
                var cmd = GetBalances(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }


    }
}
