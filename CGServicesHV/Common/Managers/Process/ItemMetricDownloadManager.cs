using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
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
    public class ItemMetricDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns,
            Plant plant, TimeSpan timeSpan, bool useHighWaterMark, List<string> documentTypes)
        {
            var response = new ProcessResponse("Item Metric Download Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));

            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new ItemMetricTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var stepGetMetrics = new ProcessStep("Get Metrics Data");
            try
            {
                var reader = GetMetrics(session, plant, processRunSpan, documentTypes);
                table.Add(reader, plant);
                stepGetMetrics.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                stepGetMetrics.Errors.Add(new Error(e));
            }
            finally
            {
                stepGetMetrics.Done();
                response.AddStep(stepGetMetrics);
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

            response.Close();
            return response;
        }

        /********************************************************************************
        * METRICS
        ********************************************************************************/
        public static IDataReader GetMetrics(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan, List<string> documentTypes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {

                // Cannot run wide Open
                //if (!documentTypes.Any())
                //{
                //    filters.Add("0 = 1 AND ");
                //}

                // Plant - Required
                filters.Add("ILMCU = ? AND ");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Status Codes
                //if (documentTypes.Any())
                //{
                //    var strInClause = "";
                //    foreach (var documentType in documentTypes)
                //    {
                //        strInClause = strInClause + "?,";
                //        parameters.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 2));
                //        parameters.Last().Value = documentType;
                //    }
                //    filters.Add("ILDCT IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND ");
                //}
                filters.Add("ILDCT IN ('RI','1I','2I','IM','1P')");

                // Command
                var cmd = GetMetrics(filters);
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

        private static iDB2Command GetMetrics(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var currentDateId = E1TimeManager.CastToE1Date(DateTime.Now.Date);
            var yearDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddYears(-1).Date);
            var quarterDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddMonths(-3).Date);
            var monthDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddMonths(-1).Date);
            var cmd = new iDB2Command
            {
                CommandTimeout = 0,
                CommandType = CommandType.Text,
                CommandText = $@"
              select 
	                ILITM,  
					ILGLPT,
					ILDCT,
					ILTRUM,
					IMUOM1,
	                SUM(CASE WHEN ILDGL BETWEEN {monthDateId} and {currentDateId} THEN ILTRQT/1000 ELSE 0 END) AS ILTRQT1,
	                SUM(CASE WHEN ILDGL BETWEEN {quarterDateId} and {currentDateId} THEN ILTRQT/1000 ELSE 0 END) AS ILTRQT2,
					SUM(ILTRQT/1000) AS ILTRQT3
                from 
	                F4111 
                    INNER JOIN F4101
                    ON IMITM = ILITM
                where
                    ILDGL BETWEEN {yearDateId} AND {currentDateId} AND
                    {filterString}
                group by 
	                ILITM,
					ILGLPT,
					ILDCT,
					ILTRUM,
					IMUOM1
                FOR READ ONLY"
            };
            return cmd;
        }

    }
}


