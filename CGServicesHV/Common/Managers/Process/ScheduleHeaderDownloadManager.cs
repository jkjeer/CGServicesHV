using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CGServicesHV.Common.Managers.Process
{
    public class ScheduleHeaderDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public ScheduleHeaderDownloadManager(BusinessSystemSession e1Session)
        {
            //var hash = new Dictionary<string, iDB2Command>
            //{
            //    {"LoadHeaderDownloadManager.GetLoadHeaderLedgerPrepared", GetLoadHeaderLedgerPrepared()}
            //};
            //foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            //{
            //    e1Session.DbCommands.Add(keyName, hash[keyName]);
            //}
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Schedule Header Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ScheduleHeader") : null;
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
                var table = new ScheduleHeaderTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (useHighWaterMark)
                    {
                        Logger.Debug($@"GetScheduleHeaderLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetScheduleHeaderLedgerPrepared(session, processRunSpan, plant);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        Logger.Debug($@"GetScheduleHeaderLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetScheduleHeaderLedgerPrepared(session, processRunSpan, plant);
                        table.Add(reader, plant);
                    }
                    Logger.Trace($"Rows: {table.Rows.Count}");
                    Logger.Trace($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
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
                        Logger.Error(e.Message);
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
                        Logger.Error(e.Message);
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }
            }
            response.Done();
            return response;
        }

        public static IDataReader GetScheduleHeaderLedgerPrepared(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, Plant plant)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                filters.Add("S6MCU = ? \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add($@"AND {E1TimeManager.GetE1DateTimeQueryString("S6UPMJ", "S6UPMT", processRunSpan.E1TimeIdSpan)}");
                }
                var cmd = GetScheduleHeaderLedgerPrepared(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetScheduleHeaderLedgerPrepared(IReadOnlyCollection<string> filters)
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
	                    S6EDBT,
	                    S6MCU,
	                    S6Z58SCN,
	                    S6Z58SCD,
	                    S6Z58SFS,
	                    S6TSC,
	                    S6EDSP,
	                    S6MATH06,
	                    S6WOPST,
	                    S6DS50,
	                    S6URAB,
	                    S6URCD,
	                    S6URDT,
	                    S6USER,
	                    S6PID,
	                    S6JOBN,
	                    S6UPMJ,
	                    S6UPMT,
						CASE WHEN (SELECT MAX(BIEDDT) FROM F580200 WHERE BIMCU = S6MCU AND BIZ58SCN = S6Z58SCN AND BIEDDT >= S6Z58SCD) IS NULL THEN 'N' ELSE 'Y' END AS E1BOXED,
                	    CASE WHEN (SELECT MAX(RHZ58RSDT) FROM F580055 WHERE RHMCU = S6MCU AND RHZ58SCN = S6Z58SCN AND RHZ58RSDT >= S6Z58SCD) IS NULL THEN 'N' ELSE 'Y' END AS E1RACKED
                    FROM
                        F580020
                    WHERE
                        {filterString}
                    FOR READ ONLY
                "
            };
            return cmd;
        }
    }
}
