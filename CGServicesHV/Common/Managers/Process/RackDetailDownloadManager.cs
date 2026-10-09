using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
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
    public class RackDetailDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackDetailDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackDetailDownloadManager.GetRackDetailLedgerPrepared", GetRackDetailLedgerPrepared()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal rackId, List<string> statusCodes)
        {
            var response = new ProcessResponse("Rack Detail Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Spans");

            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "RackDetail") : null;
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
                var table = new RackDetailTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (useHighWaterMark)
                    {
                        Logger.Info($@"GetRackDetailLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackDetailLedgerPrepared(session, processRunSpan, plant);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        Logger.Info($@"GetRackDetailDynamic PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackDetailDynamic(session, processRunSpan, plant, rackId, statusCodes);
                        table.Add(reader, plant);
                    }
                    Logger.Info($"Rows: {table.Rows.Count}");
                    Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
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
                    var step4 = new ProcessStep("Update High Water Marks");
                    try
                    {
                        highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                        highWaterMark.UniqueID = (long)table.MaxId;
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime, highWaterMark.UniqueID));
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
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        /********************************************************************************
        * Rack Detail
        ********************************************************************************/
        public static IDataReader GetRackDetailLedgerPrepared(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, Plant plant)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Debug($@"GetRackDetailLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            try
            {
                var cmd = e1Session.DbCommands["RackDetailDownloadManager.GetRackDetailLedgerPrepared"];
                cmd.Parameters["MCU"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["UKID"].Value = processRunSpan.HighWaterMark.UniqueID;

                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetRackDetailLedgerPrepared()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = @"
	                SELECT
		                DLLOTN AS LOTN,
		                DLZ58SN AS Z58SN,
		                DLITM AS ITM,
		                DLMCU AS MCU,
		                DLZ58DST AS Z58DST,
		                DLZ58RID AS Z58RID,
		                DLZ58MDT AS Z58MDT,
		                DLZ58PDT AS Z58PDT,
		                DLPQOH AS PQOH,
                        DLZ58CQTY AS RQOH,
		                DLZ58PS AS Z58PS,
		                DLZ58SL AS Z58SL,
		                DLZ58STK AS Z58STK,
		                DLZ58ROW AS Z58ROW,
		                DLZ58SD AS Z58SD,
		                DLZ58BXRO AS Z58BXRO,
		                DLDOC1 AS DOC1,
                        DLXDCTO AS XDCTO,
		                DLPKCO AS PKCO,
		                DLDOC2 AS DOC2,
		                DLXCTO AS XCTO,
		                DLLNIC AS LNIC,
		                DLKCOO AS KCOO,
		                DLDOCO AS DOCO,
		                DLDCTO AS DCTO,
		                DLLNID AS LNID,
		                DLMUPM AS UPMJ,
		                DLUPMT AS TDAY,
                        DLUSER AS USERID,
                        DLPID AS PID,
		                DLIOPR AS IOPR,
		                DLMUPM AS MUPM,
		                DLUPMT AS UPMT,
		                DLDOC1 AS DOC1INIT,
                        DLXDCTO AS XDCTOINIT,
		                DLPKCO AS PKCOINIT,
		                DLDOC2 AS DOC2INIT,
		                DLXCTO AS XCTOINIT,
		                DLLNIC AS LNICINIT,
		                DLKCOO AS KCOOINIT,
		                DLDOCO AS DOCOINIT,
		                DLDCTO AS DCTOINIT,
		                DLLNID AS LNIDINIT,
		                DLUKID AS UKID
	                FROM
		                F580569	
	                WHERE
                        DLMCU = ? AND DLUKID > ?
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID"].Precision = 15;
            cmd.Parameters["UKID"].Scale = 0;

            return cmd;
        }

        /********************************************************************************
         * Rack Detail 
         ********************************************************************************/
        public static IDataReader GetRackDetailDynamic(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant, decimal rackId, List<string> statusCodes)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Debug($@"GetRackLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Cannot run wide Open
                //if (!statusCodes.Any() && processRunSpan.IsEmpty && rackId == 0)
                //{
                //    filters.Add("0 = 1 AND \r\n");
                //}

                // Plant - Required
                filters.Add("RDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("RDZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }

                // Status Codes
                if (statusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in statusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("RDZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + ")" + " AND \r\n");
                }

                // Command
                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                var cmd = GetRackDetailDynamic(filters);
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
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetRackDetailDynamic(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = $@"
	                    SELECT
		                    RDLOTN AS LOTN,
		                    RDZ58SN AS Z58SN,
		                    RDITM AS ITM,
		                    RDMCU AS MCU,
		                    RDZ58DST AS Z58DST,
		                    RDZ58RID AS Z58RID,
		                    RDZ58MDT AS Z58MDT,
		                    RDZ58PDT AS Z58PDT,
		                    RDPQOH AS PQOH,
                            RDZ58CQTY AS RQOH,
		                    RDZ58PS AS Z58PS,
		                    RDZ58SL AS Z58SL,
		                    RDZ58STK AS Z58STK,
		                    RDZ58ROW AS Z58ROW,
		                    RDZ58SD AS Z58SD,
		                    RDZ58BXRO AS Z58BXRO,
                            RDDOC1 AS DOC1,
                            RDXDCTO AS XDCTO,
                            RDPKCO AS PKCO,
                            RDDOC2 AS DOC2,
                            RDXCTO AS XCTO,
                            RDLNIC AS LNIC,
		                    RDKCOO AS KCOO,
		                    RDDOCO AS DOCO,
		                    RDDCTO AS DCTO,
		                    RDLNID AS LNID,
		                    RDUPMJ AS UPMJ,
		                    RDTDAY AS TDAY,
                            RDUSER AS USERID,
                            RDPID AS PID,
		                    CAST('Init' AS VARCHAR(10)) AS IOPR,
                            DLMUPM AS MUPM,
                            DLUPMT AS UPMT,
		                    DLDOC1 AS DOC1INIT,
                            DLXDCTO AS XDCTOINIT,
		                    DLPKCO AS PKCOINIT,
		                    DLDOC2 AS DOC2INIT,
		                    DLXCTO AS XCTOINIT,
		                    DLLNIC AS LNICINIT,
		                    DLKCOO AS KCOOINIT,
		                    DLDOCO AS DOCOINIT,
		                    DLDCTO AS DCTOINIT,
		                    DLLNID AS LNIDINIT,
                            DLUKID AS UKID
	                    FROM
		                    F580056
                            LEFT OUTER JOIN F580569
                            ON  DLMCU = RDMCU AND
                                DLLOTN = RDLOTN AND
                                DLZ58SN = RDZ58SN AND
                                DLIOPR = 'Insert'
	                    WHERE
                            {filterString}
                    FOR READ ONLY"
            };
            return cmd;
        }
    }
}
