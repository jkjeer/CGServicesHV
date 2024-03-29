using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
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
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant)
        {
            var response = new ProcessResponse("Inventory Location Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var table = new InventoryLocationTable();
            /********************************************************/
            var step2 = new ProcessStep("Get Source Data");
            try
            {
                var reader = GetReader(session, plant);
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
                            LMLOCN,
                            LMAISL,
                            LMBIN,
                            LMLA03,
                            LMLA04,
                            LMLLDL,
                            LMSTAG,
                            LMPZON,
                            LMUPMJ,
                            LMTDAY,
                            ROW_NUMBER() OVER ( PARTITION BY LMMCU, LMLOCN ORDER BY LMUPMJ desc, LMTDAY desc ) AS RowNumber
                        FROM
                            F4100
                        WHERE
                            {filterString}
                    ) x WHERE RowNumber = 1
                    FOR READ ONLY
                "
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession session, Plant plant)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                filters.Add("TRIM(LMLOCN) <> '' AND \r\n");
                filters.Add("(LMMCU = '          32' OR (TRIM(LMLOCN) IN ('MZRAW','MZFIN','MZSHP') AND LMMCU IN ('          05', '          04', '          08'))) \r\n");
                var cmd = GetReader(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
