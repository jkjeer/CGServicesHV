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
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class LaborDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public EmployeeDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"UdcItemDownloadManager.GetUdcItem", GetUdcItem()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, decimal employeeNum = 0)
        {
            var response = new ProcessResponse("Labor Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new LaborTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "Labor") : null;
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
                    var reader = GetReader(sqlCns["CORP.WAREHOUSE"], processRunSpan, plant);
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
        private static SqlCommand GetReader(IReadOnlyCollection<string> filters)
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
	                PL.PLANT_NO AS Plant,
	                YEAR(PL.EVENT_DATE) AS ProductionYear,
                    MONTH(PL.EVENT_DATE) AS ProductionMonth,
	                PL.WG1_NAME AS WorkGroup1,
	                PL.WG2_NAME AS WorkGroup2,
	                PL.WG3_NAME AS WorkGroup3,
	                PL.PAYLINE_TYPE_CD AS PaylineTypeCode,
	                PL.PAY_TYPE_DESC AS PayTypeDesc,
	                SUM(Hours * Rate) AS Amount
                FROM
	                PROD.AOD_PAYLINE			 PL
	                INNER JOIN PROD.AOD_EMPLOYEE EP
	                ON EP.EMPLOYEE_ID = PL.EMPLOYEE_ID
                WHERE
                    {filterString}
                GROUP BY
	                PL.PLANT_NO,
	                YEAR(PL.EVENT_DATE),
                    MONTH(PL.EVENT_DATE),
	                PL.WG1_NAME,
	                PL.WG2_NAME,
	                PL.WG3_NAME,
	                PL.PAYLINE_TYPE_CD,
	                PL.PAY_TYPE_DESC
                ORDER BY 1,2,3"
            };
            return cmd;
        }

        public static IDataReader GetReader(SqlConnection cn, ProcessRunSpan processRunSpan, Plant plant)
        {
            var filters = new List<string>();
            var parameters = new List<SqlParameter>();
            try
            {

                filters.Add("PL.PLANT_NO = @PlantNum AND \r\n");
                parameters.Add(new SqlParameter("@PlantNum", SqlDbType.VarChar, 12));
                parameters.Last().Value = plant.PlantNum;

                filters.Add("PL.EVENT_DATE BETWEEN @StartDate AND @EndDate \r\n");
                parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
                parameters.Last().Value = new DateTime(processRunSpan.StartDateTime.Year, processRunSpan.StartDateTime.Month, 1);
                parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));
                parameters.Last().Value = new DateTime(processRunSpan.EndDateTime.AddMonths(1).Year, processRunSpan.EndDateTime.AddMonths(1).Month, 1);

                // Command
                var cmd = GetReader(filters);
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
