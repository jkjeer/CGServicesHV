using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public static class ItemAttributeDownloadManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum)
        {
            var response = new ProcessResponse("Item Attribute Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));

            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var stepSpan = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ItemAttribute") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                stepSpan.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                stepSpan.Errors.Add(new Error(e));
            }
            finally
            {
                stepSpan.Done();
                response.AddStep(stepSpan);
            }

            List<decimal> items = new List<decimal>();
            /********************************************************/
            var stepList = new ProcessStep("Get Item List");
            try
            {
                var reader = GetItems(session, plant, processRunSpan, itemNum);
                while (reader.Read())
                {
                    items.Add(reader.Get<decimal>("IBITM"));
                }
                reader.Close();
                stepList.RowsAffected = items.Count;
            }
            catch (Exception e)
            {
                stepList.Errors.Add(new Error(e));
            }
            stepList.Done();
            response.ProcessSteps.Add(stepList);

            /********************************************************/
            var table = new ItemAttributeTable();
            var stepTable = new ProcessStep("Build Table");
            try
            {
                foreach (var item in items)
                {
                    var attributes = DiscreteItemAttributeManager.GetItemAttributes(session, item);
                    table.Add(attributes, plant);
                }
                stepTable.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                stepTable.Errors.Add(new Error(e));
            }
            stepTable.Done();
            response.ProcessSteps.Add(stepTable);

            /********************************************************/
            if (table.Rows.Count > 0)
            {
                var stepBulk = new ProcessStep("Bulk Copy");
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
                    stepBulk.Errors.Add(new Error(e));
                }
                finally
                {
                    stepBulk.RowsAffected = (int)bulkResponse.RowsCopied;
                    stepBulk.SqlBulkCopyResponses.Add(bulkResponse);
                    stepBulk.Done();
                    response.RowsAffected = stepBulk.RowsAffected;
                    response.AddStep(stepBulk);
                }
            }

            /********************************************************/
            if (useHighWaterMark && !response.HasErrors())
            {
                var step4 = new ProcessStep("Update High Water Mark");
                try
                {
                    if (table.Rows.Count > 0)
                    {
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"],
                            plant, highWaterMark, table.MaxDate));
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
            response.Close();
            return response;
        }

        /*********************************************************************************
        * Spacer Items
        ********************************************************************************/
        public static iDB2Command GetItems(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT
                IBITM
            FROM 
	            F4102
            WHERE 
                EXISTS (SELECT 1 FROM F580002 WHERE IAITM = IBITM AND {filters}) 
            FOR READ ONLY";
            return cmd;
        }

        public static IDataReader GetItems(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan, decimal itemNum)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add(new Sql.SqlFilter($"IBMCU = '{plant.PlantNum.PadLeft(12)}'", "AND"));

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add(new Sql.SqlFilter($"IBITM = ?", "AND"));
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                }

                // Dates
                if (!processRunSpan.IsEmpty)
                {
                    var dtList = new List<string[]>
                    {
                        new string[] { "IAUPMJ", "IATDAY" }
                    };
                    filters.Add(new Sql.SqlFilter($"{E1TimeManager.GetE1DateFilterString(dtList, processRunSpan.StartDateTime, processRunSpan.EndDateTime)}", "AND"));
                }
                // Command
                var cmd = GetItems(filters);
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
