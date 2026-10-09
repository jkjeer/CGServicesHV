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
    public class WorkOrderPartDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public WorkOrderPartDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"WorkOrderPartDownloadManager.GetEventData", GetEventData()}
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
            var response = new ProcessResponse("Work Order Part Download Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new WorkOrderPartTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var stepSpan = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "WorkOrderPart") : null;
                if (!String.IsNullOrEmpty(startDate))
                {
                    processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, Convert.ToDateTime(endDate) - Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
                }
                else
                {
                    processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                }
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
                var step3 = new ProcessStep("Bulk Copy");
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

            response.Close();
            return response;
        }

        /******************************************************************
         * Event Data
         ******************************************************************/
        public iDB2Command GetEvents(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
            SELECT DISTINCT
	            WMDOCO
            FROM 
	            F3111
                INNER JOIN F4801
                ON WADOCO = WMDOCO AND
                    WAMCU = WMMCU
            WHERE 
                {filterString}
	            1=1
            FOR READ ONLY";
            return cmd;
        }

        public List<decimal> GetEvents(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var events = new List<decimal>();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {

                // Process Run Span
                filters.Add(E1TimeManager.GetE1DateTimeQueryString("WMUPMJ", "WMTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");

                // Plant - Required
                filters.Add("WMMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                cmd = GetEvents(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    events.Add(Data.CastDbNull.To<decimal>(reader["WMDOCO"], 0));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return events;
        }


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
                    WMMCU,
                    WMDOCO,
                    WMDCTO,
                    WMTBM,
                    WMOPSQ,
                    WMCPNB,
                    WMCPIT,
                    WMLOTN,
                    WMLOCN,
                    WMUORG,
                    WMTRQT,
                    WMUM,
                    WMUKID,
                    WMUPMJ,
                    WMTDAY
                FROM 
                    F3111 
                WHERE 
                    {filterString} 1=1
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
                filters.Add("WMMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // OrderNum
                if (orderNum > 0)
                {
                    filters.Add("WMDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("WMUPMJ", "WMTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
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
                    WMMCU,
                    WMDOCO,
                    WMDCTO,                    
                    WMTBM,
                    WMOPSQ,
                    WMCPNB,
                    WMCPIT,
                    WMLOTN,
                    WMLOCN,
                    WMUORG,
                    WMTRQT,
                    WMUM,
                    WMUKID,
                    WMUPMJ,
                    WMTDAY
                FROM 
                    E1PURGE21.F3111
                WHERE 
                    {filterString} 1=1
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
                filters.Add("WMMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // OrderNum
                if (orderNum > 0)
                {
                    filters.Add("WMDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("WMUPMJ", "WMTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                var cmd = GetArchivedData(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
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
