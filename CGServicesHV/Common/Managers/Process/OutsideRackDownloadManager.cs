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
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class OutsideRackDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public OutsideRackDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"OutsideRackDownloadManager.GetRackLedgerPrepared", GetRackLedgerPrepared()},
                {"OutsideRackDownloadManager.GetRackLedgerPreparedNew", GetRackLedgerPreparedNew()},
                {"OutsideRackDownloadManager.GetRackDetailLedgerPrepared", GetRackDetailLedgerPrepared()}
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
            var response = new ProcessResponse("Outside Rack Download Manager");
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
            HighWaterMark highWaterMarkDetail = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "OutsideRack") : null;
                highWaterMarkDetail = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "OutsideRackDetail") : null;
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
                var table = new RackTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (useHighWaterMark)
                    {
                        Logger.Info($@"GetRackLedgerPreparedNew PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackLedgerPreparedNew(session, processRunSpan);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        Logger.Info($@"GetRackDynamicNew PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackDynamicNew(session, processRunSpan, rackId, statusCodes);
                        table.Add(reader, plant);
                    }
                    Logger.Info($"Rows: {table.Rows.Count}");
                    Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
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
                    var startTime = DateTime.Now.ToLocalTime();
                    var step2a = new ProcessStep("Get Rack Detail");

                    List<decimal> rackList = new List<decimal>();
                    var detailTable = new RackDetailTable();
                    foreach (DataRow row in table.Rows)
                    {
                        var thisRackId = Convert.ToInt32(row["RackId"]);
                        if (!rackList.Contains(thisRackId))
                        {
                            rackList.Add(thisRackId);
                            Logger.Info($@"RackID: {thisRackId}");
                        }
                    }
                    foreach (var rack in rackList)
                    {
                        Logger.Info($@"GetRackDetailLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} RackID: {rack} BEGIN: {startTime}");
                        var reader = GetRackDetailLedgerPrepared(session, plant, rack);
                        detailTable.Add(reader, plant);
                    }
                    if (detailTable.Rows.Count > 0)
                    {
                        var step3a = new ProcessStep("Bulk Copy Detail");
                        var bulkResponseDetail = new SqlBulkCopyResponse();
                        try
                        {
                            bulkResponseDetail = BulkCopyManager.BulkTable(detailTable.ExtendedProperties["Target"].ToString(), detailTable, sqlCns["PLANT.INTEGRATION"]);
                        }
                        catch (Exception e)
                        {
                            step3a.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3a.RowsAffected = (int)bulkResponseDetail.RowsCopied;
                            step3a.SqlBulkCopyResponses.Add(bulkResponseDetail);
                            step3a.Done();
                            response.RowsAffected = step3a.RowsAffected;
                            response.AddStep(step3a);
                        }
                    }

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
                        highWaterMark.UniqueID = (long)table.MaxId;
                        highWaterMarkDetail.EnterpriseDateTime = highWaterMark.EnterpriseDateTime;
                        highWaterMarkDetail.UniqueID = highWaterMark.UniqueID;
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime, highWaterMark.UniqueID));
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, highWaterMarkDetail, highWaterMarkDetail.EnterpriseDateTime, highWaterMarkDetail.UniqueID));
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

        /********************************************************************************
        * RACK
        ********************************************************************************/
        private static iDB2Command GetRackDynamicNew(IReadOnlyCollection<string> filters)
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
                        RHZ58RID AS HLZ58RID,
                        RHMCU AS HLMCU,
                        RHZ58RT AS HLZ58RT,
                        RHZ58HST AS HLZ58HST,
                        RHLOCN AS HLLOCN,
                        RHURCD AS HLURCD,
                        RHZ58BOX AS HLZ58BOX,
                        RHZ58SCN AS HLZ58SCN,
                        RHZ58RTWT AS HLZ58RTWT,
                        RHZ58GWTT AS HLZ58GWTT,
                        RHURRF AS HLURRF,
                        RHZ58RCD AS HLZ58RCD,
                        RHPSN AS HLPSN,
                        RHAN8 AS HLAN8,
                        RHSHAN AS HLSHAN,
                        RHUPMJ AS HLMUPM,
                        RHTDAY AS HLUPMT,   
                        RHUSER AS USERID,
                        RHPID AS PID,
		                CAST(0 AS DECIMAL(15,0)) AS HLUKID,
		                CAST('Init' AS VARCHAR(10)) AS HLIOPR,
                        HLMUPM AS HLMUPM_ORIG,
                        HLUPMT AS HLUPMT_ORIG,
                        RHZ58NA02 AS HLZ58NA02
                    FROM 
	                    F580055
                        INNER JOIN F580559
                        ON  HLZ58RID = RHZ58RID 
	                WHERE 
                        {filterString}
                    FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetRackDynamicNew(BusinessSystemSession session, ProcessRunSpan processRunSpan, decimal rackId, List<string> statusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("RHMCU IN ('          05', '          04', '          08') AND \r\n");
                //filters.Add("RHZ58RID IN (SELECT DISTINCT HLZ58RID FROM F580559 WHERE HLMCU IN ('          05', '          04', '          08') AND HLLOCN LIKE 'MZ%') AND \r\n");
                filters.Add("EXISTS(SELECT 1 FROM F580559 WHERE HLZ58RID = RHZ58RID AND HLMCU IN ('          05', '          04', '          08') AND HLLOCN LIKE 'MZ%') AND \r\n");

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("RHZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 9;
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
                    filters.Add("RHZ58HST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("RHUPMJ", "RHTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                var cmd = GetRackDynamicNew(filters);
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

        public static IDataReader GetRackLedgerPreparedNew(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan)
        {
            try
            {
                var cmd = e1Session.DbCommands["OutsideRackDownloadManager.GetRackLedgerPreparedNew"];
                cmd.Parameters["UKID"].Value = processRunSpan.HighWaterMark.UniqueID;
                cmd.Connection = e1Session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetRackLedgerPreparedNew()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        HLUKID,
                        HLZ58RID,
                        HLMCU,
                        HLZ58RT,
                        HLZ58HST,
                        HLLOCN,
                        HLURCD,
                        HLZ58BOX,
                        HLZ58SCN,
                        HLZ58RTWT,
                        HLZ58GWTT,
                        HLURRF,
                        HLZ58RCD,
                        HLPSN,
                        HLAN8,
                        HLSHAN,
                        HLMUPM,
                        HLUPMT,
                        HLUSER AS USERID,
                        HLPID AS PID,
                        HLIOPR,
                        CAST(0 AS DECIMAL(6,0)) AS HLMUPM_ORIG,
                        CAST(0 AS DECIMAL(6,0)) AS HLUPMT_ORIG,
                        HLZ58NA02 AS HLZ58NA02
                    FROM 
                        F580559
                    WHERE 
                        HLUKID >= ? AND 
                        HLMCU IN ('          05', '          04', '          08') AND 
                        HLLOCN LIKE 'MZ%'
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID"].Precision = 15;
            cmd.Parameters["UKID"].Scale = 0;
            return cmd;
        }

        /********************************************************************************
        * Rack Detail
        ********************************************************************************/
        public static IDataReader GetRackDetailLedgerPrepared(BusinessSystemSession e1Session, Plant plant, decimal rackId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Debug($@"GetRackDetailLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            try
            {
                var cmd = e1Session.DbCommands["OutsideRackDownloadManager.GetRackDetailLedgerPrepared"];
                cmd.Parameters["RackID"].Value = rackId;

                cmd.Connection = e1Session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
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
                        DLUKID as UKID
	                FROM
		                F580569	
	                WHERE
                        DLZ58RID = ?
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("RackID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackID"].Precision = 15;
            cmd.Parameters["RackID"].Scale = 0;

            return cmd;
        }

    }
}
