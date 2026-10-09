using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
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
    public class GlassItemMetricDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns,
            Plant plant, TimeSpan timeSpan, bool useHighWaterMark, List<string> metrics, decimal itemNum)
        {
            var response = new ProcessResponse("Glass Item Metric Download Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));

            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new GlassItemMetricTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var stepMetricsList = new ProcessStep("Get Metrics List");
            try
            {
                stepMetricsList.Messages.Add(new Message("Metrics", String.Join(", ", metrics.ToArray())));
            }
            catch (Exception e)
            {
                stepMetricsList.Errors.Add(new Error(e));
            }
            finally
            {
                stepMetricsList.Done();
                response.AddStep(stepMetricsList);
            }

            /********************************************************/
            if (metrics.Contains("CONSUMED"))
            {
                var stepGetConsumption = new ProcessStep("Get Consumption Data");
                try
                {
                    var reader = GetConsumption(session, plant);
                    table.Add(reader, plant);
                    stepGetConsumption.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    stepGetConsumption.Errors.Add(new Error(e));
                }
                finally
                {
                    stepGetConsumption.Done();
                    response.AddStep(stepGetConsumption);
                }
            }

            /********************************************************/
            if (metrics.Contains("SOLD"))
            {
                var stepGetSales = new ProcessStep("Get Sales Data");
                try
                {
                    var reader = GetSales(session, plant);
                    table.Add(reader, plant);
                    stepGetSales.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    stepGetSales.Errors.Add(new Error(e));
                }
                finally
                {
                    stepGetSales.Done();
                    response.AddStep(stepGetSales);
                }
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
        public static IDataReader GetConsumption(BusinessSystemSession session, Plant plant)
        {
            try
            {
                var currentDateId = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                var yearDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddYears(-1).Date);
                var quarterDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddMonths(-3).Date);
                var monthDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddMonths(-1).Date);
                var cmd = new iDB2Command
                {
                    CommandType = CommandType.Text,
                    CommandText = $@"
                select 
	                ILITM,  
                    'CONSUMED' AS METRICNAME,
	                SUM(CASE WHEN ILDGL BETWEEN {monthDateId} and {currentDateId} THEN ILTRQT/1000 ELSE 0 END) AS MONTHAMT,
	                SUM(CASE WHEN ILDGL BETWEEN {quarterDateId}  and {currentDateId} THEN ILTRQT/1000 ELSE 0 END) AS QUARTERAMT,
					SUM(ILTRQT/1000) AS YEARAMT
                from 
	                F4111 
                    --INNER JOIN F4101
                    --ON IMITM = ILITM
                where
                    ILMCU = '          {plant.NumericValue}' AND
	                ILDCT IN ('IM', '2I') AND
                    ILDGL BETWEEN {yearDateId} AND {currentDateId} AND
                    ILLOTN = '' AND
                    --IMSTKT <> 'C' AND
                    ILGLPT IN ('GLAS','GLAF','GLAC','GLAX','GLAZ') 
                group by 
	                ILITM
                FOR READ ONLY"
                };
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        public static IDataReader GetSales(BusinessSystemSession session, Plant plant)
        {
            try
            {
                var currentDateId = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                var yearDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddYears(-1).Date);
                var quarterDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddMonths(-3).Date);
                var monthDateId = E1TimeManager.CastToE1Date(DateTime.Now.AddMonths(-1).Date);
                var cmd = new iDB2Command
                {
                    CommandType = CommandType.Text,
                    CommandText = $@"
                select 
	                ILITM,  
                    'SOLD' AS METRICNAME,
	                SUM(CASE WHEN ILDGL BETWEEN {monthDateId} and {currentDateId} THEN ILTRQT/1000 ELSE 0 END) AS MONTHAMT,
	                SUM(CASE WHEN ILDGL BETWEEN {quarterDateId}  and {currentDateId} THEN ILTRQT/1000 ELSE 0 END) AS QUARTERAMT,
					SUM(ILTRQT/1000) AS YEARAMT
                from 
	                F4111 
                    --INNER JOIN F4101
                    --ON IMITM = ILITM
                where
                    ILMCU = '          {plant.NumericValue}' AND
	                ILDCT IN ('RI') AND
                    ILDGL BETWEEN {yearDateId} AND {currentDateId} AND
                    ILLOTN = '' AND
                    --IMSTKT = 'S' AND
                    ILGLPT IN ('GLAS','GLAF','GLAC','GLAX','GLAZ') 
                group by 
	                ILITM
                FOR READ ONLY"
                };
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

    }
}


