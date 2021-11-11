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
    public class InventoryLocationDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public InventoryLocationDownloadManager(BusinessSystemSession e1Session)
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
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, string locationCode = null)
        {
            var response = new ProcessResponse("Inventory Location Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new InventoryLocationTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {          
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "InventoryLocation") : null;
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
                    var reader = GetReader(session, processRunSpan, plant, locationCode);
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
                            LMMCU,
                            TRIM(LMLOCN) LMLOCN,
                            TRIM(LMAISL) LMAISL,
                            TRIM(LMBIN) LMBIN,
                            LMLA03,
                            LMLA04,
                            LMLLDL,
                            LMSTAG,
                            LMPZON,
                            LMUPMJ,
                            LMTDAY,
                            ROW_NUMBER() OVER ( PARTITION BY LMMCU, TRIM(LMLOCN) ORDER BY LMUPMJ desc, LMTDAY desc ) AS RowNumber
                        FROM
                            F4100
                        WHERE
                            {filterString}
                    ) x WHERE RowNumber = 1"
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant, string locationCode)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                filters.Add("TRIM(LMLOCN) <> '' AND \r\n");
                //filters.Add("TRIM(LMLLDL) <> '2' AND \r\n");
                //filters.Add("TRIM(LMLLDL) <> '9' AND \r\n"); //commented out 20190708 pre SGCG email non-glass locations
                // Plant Num
                filters.Add("(LMMCU = '          32' OR (LMLOCN IN ('MZRAW','MZFIN','MZSHP') AND LMMCU IN ('          05', '          04', '          08'))) AND \r\n");
                // Location Code
                if (!string.IsNullOrEmpty(locationCode))
                {
                    filters.Add("TRIM(LMLOCN) = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("LOCN", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = locationCode.Trim();
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("LMUPMJ", "LMTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                // Command
                if (filters.Any()) filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                var cmd = GetReader(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
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
