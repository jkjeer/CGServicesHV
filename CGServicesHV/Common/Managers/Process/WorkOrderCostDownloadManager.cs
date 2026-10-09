using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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

namespace CGServicesHV.Common.Managers.Process
{
    public class WorkOrderCostDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public WorkOrderCostDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"WorkOrderCostDownloadManager.GetEventData", GetEventData()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal orderNum, string startDate, string endDate)
        {
            var response = new ProcessResponse("Work Order Cost Download Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new WorkOrderCostTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "WorkOrderCost") : null;
                if (!String.IsNullOrEmpty(startDate))
                {
                    processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, Convert.ToDateTime(endDate) - Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
                }
                else
                {
                    processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                }
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
                var stepGet = new ProcessStep("Get Source Data");
                try
                {
                    var reader = GetData(session, processRunSpan, plant, orderNum);
                    table.Add(reader, plant);
                    stepGet.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    stepGet.Errors.Add(new Error(e));
                }
                stepGet.Done();
                response.AddStep(stepGet);

                if (!useHighWaterMark)
                {
                    var stepGetArchive = new ProcessStep("Get Archived Data");
                    try
                    {
                        var reader = GetArchivedData(session, processRunSpan, plant, orderNum);
                        table.Add(reader, plant);
                        stepGetArchive.RowsAffected = table.Rows.Count - stepGet.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        stepGetArchive.Errors.Add(new Error(e));
                    }
                    stepGetArchive.Done();
                    response.AddStep(stepGetArchive);

                    //var stepGetArchiveBK = new ProcessStep("Get Archived BK Data");
                    //try
                    //{
                    //    var reader = GetArchivedBKData(session, processRunSpan, plant, orderNum);
                    //    table.Add(reader, plant);
                    //    stepGetArchiveBK.RowsAffected = table.Rows.Count - stepGet.RowsAffected - stepGetArchive.RowsAffected;
                    //}
                    //catch (Exception e)
                    //{
                    //    stepGetArchiveBK.Errors.Add(new Error(e));
                    //}
                    //stepGetArchiveBK.Done();
                    //response.AddStep(stepGetArchiveBK);
                }
            }

            /********************************************************/
            if (table.Rows.Count > 0)
            {
                var stepBulk = new ProcessStep("Bulk Copy");
                var bulkResponse = new SqlBulkCopyResponse();
                try
                {
                    if (table.Rows.Count > 0)
                    {
                        response.RowsAffected = table.Rows.Count;
                        bulkResponse =
                            BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),
                                table, sqlCns["PLANT.INTEGRATION"]);
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

            response.Close();
            return response;
        }

        /******************************************************************
         * Event Data
         ******************************************************************/
        private static iDB2Command GetData(IReadOnlyCollection<string> filters)
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
	                IGDOCO,
                    IGITM,
	                IGCOST,
                    IGPART,
	                IGUOM,
	                IGOPUN,
	                IGOPAT,
                    IGCLUN,
                    IGCLAT,
                    IGCCCU,
                    IGCCCA,
                    IGPLUN,
                    IGPLAT,
                    IGCPUN,
                    IGCPAT,
                    IGSLUN,
                    IGSLAT,
	                IGUPMJ,
	                IGTDAY
                FROM 
	                F3102
	                INNER JOIN F4801
	                ON IGDOCO = WADOCO
                WHERE 
                    {filterString.Remove(filterString.Length - 4, 3)}
                FOR READ ONLY"
            };
            return cmd;
        }



        public static IDataReader GetData(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant,
            decimal orderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("WAMCU = ? AND ");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // OrderNum
                if (orderNum > 0)
                {
                    filters.Add("IGDOCO = ? AND ");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("IGUPMJ", "IGTDAY", processRunSpan.E1TimeIdSpan) + " AND ");
                }

                // Command
                var cmd = GetData(filters);
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

        private static iDB2Command GetArchivedData(IReadOnlyCollection<string> filters)
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
	                IGDOCO,
                    IGITM,
	                IGCOST,
                    IGPART,
	                IGUOM,
	                IGOPUN,
	                IGOPAT,
                    IGCLUN,
                    IGCLAT,
                    IGCCCU,
                    IGCCCA,
                    IGPLUN,
                    IGPLAT,
                    IGCPUN,
                    IGCPAT,
                    IGSLUN,
                    IGSLAT,
	                IGUPMJ,
	                IGTDAY
                FROM 
	                E1PURGE21.F3102
	                INNER JOIN E1PURGE21.F4801
	                ON IGDOCO = WADOCO
                WHERE 
                    {filterString.Remove(filterString.Length - 4, 3)} 
                ORDER BY 
                    IGDOCO,
                    IGITM,
                    IGCOST
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetArchivedData(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant,
            decimal orderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("WAMCU = ? AND ");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // OrderNum
                if (orderNum > 0)
                {
                    filters.Add("IGDOCO = ? AND ");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("IGUPMJ", "IGTDAY", processRunSpan.E1TimeIdSpan) + " AND ");
                }

                // Command
                var cmd = GetArchivedData(filters);
                cmd.CommandTimeout = 0;
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

        private static iDB2Command GetArchivedBKData(IReadOnlyCollection<string> filters)
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
	                IGDOCO,
                    IGITM,
	                IGCOST,
                    IGPART,
	                IGUOM,
	                IGOPUN,
	                IGOPAT,
                    IGCLUN,
                    IGCLAT,
                    IGCCCU,
                    IGCCCA,
                    IGPLUN,
                    IGPLAT,
                    IGCPUN,
                    IGCPAT,
                    IGSLUN,
                    IGSLAT,
	                IGUPMJ,
	                IGTDAY
                FROM 
	                E1PURGE21.F3102BK
	                INNER JOIN E1PURGE21.F4801BK
	                ON IGDOCO = WADOCO
                WHERE 
                    {filterString.Remove(filterString.Length - 4, 3)} 
                ORDER BY 
                    IGDOCO,
                    IGITM,
                    IGCOST
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetArchivedBKData(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant,
            decimal orderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("WAMCU = ? AND ");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // OrderNum
                if (orderNum > 0)
                {
                    filters.Add("IGDOCO = ? AND ");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("IGUPMJ", "IGTDAY", processRunSpan.E1TimeIdSpan) + " AND ");
                }

                // Command
                var cmd = GetArchivedBKData(filters);
                cmd.CommandTimeout = 0;
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

        private static SqlCommand GetWarehouseData(IReadOnlyCollection<string> filters)
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
	                IGDOCO,
                    IGITM,
	                IGCOST,
                    IGPART,
	                IGUOM,
	                IGOPUN,
	                IGOPAT,
                    IGCLUN,
                    IGCLAT,
                    IGCCCU,
                    IGCCCA,
                    IGPLUN,
                    IGPLAT,
                    IGCPUN,
                    IGCPAT,
                    IGSLUN,
                    IGSLAT,
	                IGUPMJ,
	                IGTDAY
                FROM 
	                PROD.PS_JWS_F3102
	                INNER JOIN PROD.PS_JWS_F4801
	                ON IGDOCO = WADOCO
                WHERE 
                    {filterString.Remove(filterString.Length - 4, 3)}  
                ORDER BY IGDOCO, IGITM, IGCOST"
            };
            return cmd;
        }

        public static IDataReader GetWarehouseData(SqlConnection cn, ProcessRunSpan processRunSpan, Plant plant,
            decimal orderNum)
        {
            var filters = new List<string>();
            var parameters = new List<SqlParameter>();
            try
            {
                // Plant - Required
                filters.Add("WAMCU = @MCU AND ");
                parameters.Add(new SqlParameter("@MCU", SqlDbType.VarChar, 12));
                parameters.Last().Value = plant.PlantNum;

                // OrderNum
                if (orderNum > 0)
                {
                    filters.Add("IGDOCO = @DOCO AND ");
                    parameters.Add(new SqlParameter("@DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("IGUPMJ", "IGTDAY", processRunSpan.E1TimeIdSpan) + " AND ");
                }

                // Command
                var cmd = GetWarehouseData(filters);
                cmd.CommandTimeout = 0;
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
                throw new Exception(e.Message);
            }
        }

    }
}
