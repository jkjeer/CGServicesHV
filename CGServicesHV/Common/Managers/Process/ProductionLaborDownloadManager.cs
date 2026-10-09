using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class ProductionLaborDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Production Labor Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new ProductionLaborTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ProductionLabor") : null;
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
                var step2 = new ProcessStep("Get UKG Data");
                try
                {
                    var reader = GetUKG(sqlCns["CORP.WAREHOUSE"], processRunSpan, plant);
                    table.AddUKG(reader, plant);
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
        private static SqlCommand GetUKG(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
                      A.PLANT_NAME AS PlantName,
                    B.NumericCode AS PlantCode,
                    A.TRANSACTION_DT AS TransactionDate,
                    A.WORK_CENTER_NAME AS WorkCenterName,
                    A.PAY_CODE_NAME AS PayCode,
                    A.TOTAL_HOURS AS TotalHours,
                    A.TOTAL_AMOUNT AS TotalAmt,
                    A.BI_INSERT_DTTM AS SourceInsertDate,
                    A.BI_UPDATE_DTTM AS SourceUpdateDate,
                      A.IS_DELETED AS IsDeleted,
                    A.SHIFT_NAME AS ShiftName
                FROM
                      DW_SOURCE.UKG.PRODUCTION_LABOR              A WITH (NOLOCK)
                      INNER JOIN PSBI_WH_PROD.PROD.UKG_PLANT_MAP B WITH (NOLOCK)
                      ON B.PlantName = A.PLANT_NAME
                WHERE
                    {filterString}
                ORDER BY 1,2,3"
            };
            return cmd;
        }

        public static IDataReader GetUKG(SqlConnection cn, ProcessRunSpan processRunSpan, Plant plant)
        {
            var filters = new List<string>();
            var parameters = new List<SqlParameter>();
            try
            {

                filters.Add("B.NumericCode = @PlantNum AND \r\n");
                parameters.Add(new SqlParameter("@PlantNum", SqlDbType.VarChar, 12));
                parameters.Last().Value = plant.PlantNum;

                filters.Add("A.BI_INSERT_DTTM > @UpdateDate \r\n");
                parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                parameters.Last().Value = processRunSpan.StartDateTime;

                //filters.Add("A.TRANSACTION_DT BETWEEN @StartDate AND @EndDate \r\n");
                //parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
                //parameters.Last().Value = new DateTime(processRunSpan.StartDateTime.Year, processRunSpan.StartDateTime.Month, 1);
                //parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));
                //parameters.Last().Value = new DateTime(processRunSpan.EndDateTime.AddMonths(1).Year, processRunSpan.EndDateTime.AddMonths(1).Month, 1);

                // Command
                var cmd = GetUKG(filters);
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = cn;
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
