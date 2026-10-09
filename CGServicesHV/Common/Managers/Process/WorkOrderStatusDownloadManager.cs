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
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class WorkOrderStatusDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public WorkOrderStatusDownloadManager(BusinessSystemSession e1Session)
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
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal workOrderNum)
        {
            var response = new ProcessResponse("Work Order Status Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns) { response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString)); }

            var table = new WorkOrderStatusTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "WorkOrderStatus") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e) { step1.Errors.Add(new Error(e)); }
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
                    var reader = GetWorkOrderStatus(session, processRunSpan, plant.PlantNum, workOrderNum);
                    table.Add(reader, plant);
                    step2.RowsAffected = table.Rows.Count;
                }
                catch (Exception e) { step2.Errors.Add(new Error(e)); }
                finally
                {
                    step2.Done();
                    response.AddStep(step2);
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try { if (table.Rows.Count > 0) { bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]); } }
                    catch (Exception e) { step3.Errors.Add(new Error(e)); }
                    finally
                    {
                        step3.RowsAffected = (int) bulkResponse.RowsCopied;
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
                        highWaterMark.UniqueID = (long)table.MaxId;
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime, highWaterMark.UniqueID));
                        step4.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
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
        private static iDB2Command GetWorkOrderStatusLedger(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters) { filterString.Append(filter); }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
                        WAUKID,
                        WADOCO,
                        WASRST,
                        WAMUPM,
                        WAUPMT
                    FROM
                        F584801
                    WHERE
                    {filterString}
                    1=1"
            };
            return cmd;
        }

        private static iDB2Command GetWorkOrderStatusCurrent(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters) { filterString.Append(filter); }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
                        CAST(0 AS DECIMAL(15,0)) AS WAUKID,
                        WADOCO,
                        WASRST,
                        WAUPMJ AS WAMUPM,
                        WATDAY AS WAUPMT
                    FROM
                        F4801
                    WHERE
                    {filterString}
                    1=1"
            };
            return cmd;
        }

        public static IDataReader GetWorkOrderStatus(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal workOrderNum)
        {
            try
            {
                if (processRunSpan.HighWaterMark.Exists) { return GetWorkOrderStatusLedger(session, processRunSpan, plantNum, workOrderNum); }
                return GetWorkOrderStatusCurrent(session, processRunSpan, plantNum, workOrderNum);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        public static IDataReader GetWorkOrderStatusLedger(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal workOrderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("WAMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add("WAUKID > ? AND \r\n");
                    parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = processRunSpan.HighWaterMark.UniqueID;
                }

                // WorkOrderNum
                if (workOrderNum > 0)
                {
                    filters.Add("WADOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = workOrderNum;
                }

                // Command
                var cmd = GetWorkOrderStatusLedger(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        public static IDataReader GetWorkOrderStatusCurrent(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal workOrderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("WAMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty) { filters.Add(E1TimeManager.GetE1DateTimeQueryString("WAUPMJ", "WATDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n"); }

                // WorkOrderNum
                if (workOrderNum > 0)
                {
                    filters.Add("WADOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = workOrderNum;
                }

                // Command
                var cmd = GetWorkOrderStatusCurrent(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
    }
}