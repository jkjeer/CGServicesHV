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
    public class RackItemDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackItemDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackItemDownloadManager.GetReceivedQty", GetReceivedQty()},
                {"RackItemDownloadManager.GetMaxHlUkid", GetMaxHlUkid()},
                {"RackItemDownloadManager.GetMaxDlUkid", GetMaxDlUkid()}

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
            var response = new ProcessResponse("Rack Item Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Spans");

            // Rack 
            ProcessRunSpan processRunSpanRack = new ProcessRunSpan();
            HighWaterMark highWaterMarkRack = new HighWaterMark();

            // Rack Item Inserts
            ProcessRunSpan processRunSpanRackItem = new ProcessRunSpan();
            HighWaterMark highWaterMarkRackItem = new HighWaterMark();

            try
            {
                highWaterMarkRack = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "Rack") : null;
                //if (useHighWaterMark)
                //{
                //    highWaterMarkRack.MaxUniqueID = (long)GetMaxHlUkid(session, plant, highWaterMarkRack.UniqueID);
                //}
                processRunSpanRack = ProcessRunSpanManager.GetRunSpan(highWaterMarkRack, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpanRack", processRunSpanRack.Serialize()));

                highWaterMarkRackItem = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "RackItem") : null;
                //if (useHighWaterMark)
                //{
                //    highWaterMarkRackItem.MaxUniqueID = (long)GetMaxDlUkid(session, plant, highWaterMarkRackItem.UniqueID);
                //}
                processRunSpanRackItem = ProcessRunSpanManager.GetRunSpan(highWaterMarkRackItem, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpanRackItem", processRunSpanRackItem.Serialize()));
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
                var table = new RackItemTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (useHighWaterMark)
                    {
                        Logger.Debug($@"GetRackItemLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackItemLedgerPrepared(session, processRunSpanRackItem, plant);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        Logger.Debug($@"GetRackItemDynamic PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackItemDynamic(session, processRunSpanRackItem, plant, rackId, statusCodes);
                        table.Add(reader, plant);
                    }
                    Logger.Trace($"Rows: {table.Rows.Count}");
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
                        bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),table, sqlCns["PLANT.INTEGRATION"]);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
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
                var statusTable = new RackItemStatusTable();
                if (table.Rows.Count > 0)
                //if (!processRunSpanRack.IsEmpty)
                {
                    var step3a = new ProcessStep("Get Status Data");
                    try
                    {
                        var startTime = DateTime.Now.ToLocalTime();
                        Logger.Debug($@"GetRackItemStatus PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetRackItemStatus(session, processRunSpanRack, processRunSpanRackItem, plant, rackId, statusCodes);
                        statusTable.Add(reader, plant);
                        Logger.Trace($"Rows: {table.Rows.Count}");
                        Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                        step3a.RowsAffected = statusTable.Rows.Count;
                        response.RowsAffected = statusTable.Rows.Count;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        step3a.Errors.Add(new Error(e));
                    }
                    step3a.Done();
                    response.AddStep(step3a);

                    var step3b = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (statusTable.Rows.Count > 0)
                        {
                            bulkResponse = BulkCopyManager.BulkTable(statusTable.ExtendedProperties["Target"].ToString(), statusTable, sqlCns["PLANT.INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        step3b.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3b.RowsAffected = (int)bulkResponse.RowsCopied;
                        step3b.SqlBulkCopyResponses.Add(bulkResponse);
                        step3b.Done();
                        response.RowsAffected = step3b.RowsAffected;
                        response.AddStep(step3b);
                    }

                }

                /********************************************************/
                if (useHighWaterMark && !response.HasErrors() && table.Rows.Count > 0)
                {
                    var step4 = new ProcessStep("Update High Water Marks");
                    try
                    {
                        highWaterMarkRackItem.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                        highWaterMarkRackItem.UniqueID = (long)table.MaxId;
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, highWaterMarkRackItem, highWaterMarkRackItem.EnterpriseDateTime, highWaterMarkRackItem.UniqueID));
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
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        /********************************************************************************
        * Get Rack Items 
        ********************************************************************************/
        public static IDataReader GetRackItemLedgerPrepared(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant)
        {
            try
            {
                var cmd = GetRackItemLedgerPrepared();
                cmd.Parameters["UKID1"].Value = processRunSpan.HighWaterMark.UniqueID;
                cmd.Parameters["MCU1"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU2"].Value = (plant.PlantCode == "MZCG") ? "05".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU3"].Value = (plant.PlantCode == "MZCG") ? "04".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU3a"].Value = (plant.PlantCode == "MZCG") ? "08".PadLeft(12) : plant.PlantNum.PadLeft(12);

                cmd.Parameters["UKID2"].Value = processRunSpan.HighWaterMark.UniqueID;
                cmd.Parameters["MCU4"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU5"].Value = (plant.PlantCode == "MZCG") ? "05".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU6"].Value = (plant.PlantCode == "MZCG") ? "04".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU6a"].Value = (plant.PlantCode == "MZCG") ? "08".PadLeft(12) : plant.PlantNum.PadLeft(12);

                cmd.Parameters["UKID3"].Value = processRunSpan.HighWaterMark.UniqueID;
                cmd.Parameters["MCU7"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU8"].Value = (plant.PlantCode == "MZCG") ? "05".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU9"].Value = (plant.PlantCode == "MZCG") ? "04".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU9a"].Value = (plant.PlantCode == "MZCG") ? "08".PadLeft(12) : plant.PlantNum.PadLeft(12);

                cmd.Parameters["UKID4"].Value = processRunSpan.HighWaterMark.UniqueID;
                cmd.Parameters["MCU10"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU11"].Value = (plant.PlantCode == "MZCG") ? "05".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU12"].Value = (plant.PlantCode == "MZCG") ? "04".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU12a"].Value = (plant.PlantCode == "MZCG") ? "08".PadLeft(12) : plant.PlantNum.PadLeft(12);

                cmd.Parameters["UKID5"].Value = processRunSpan.HighWaterMark.UniqueID;
                cmd.Parameters["MCU13"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU14"].Value = (plant.PlantCode == "MZCG") ? "05".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU15"].Value = (plant.PlantCode == "MZCG") ? "04".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU15a"].Value = (plant.PlantCode == "MZCG") ? "08".PadLeft(12) : plant.PlantNum.PadLeft(12);

                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                Logger.Trace(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetRackItemLedgerPrepared()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
	                DLLOTN AS LOTN,
	                DLZ58SN AS Z58SN,
	                DLITM AS ITM,
	                DLZ58DST AS Z58DST,
	                DLZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                DLPQOH AS PQOH,
	                DLZ58PS AS Z58PS,
	                DLZ58SL AS Z58SL,
	                DLZ58STK AS Z58STK,
	                DLZ58ROW AS Z58ROW,
	                DLZ58SD AS Z58SD,
                    DLZ58BXRO AS Z58BXRO,
	                DLDOC2 AS DOCO,
	                DLXCTO AS DCTO,
	                DLLNIC AS LNID,
	                CMCFGID AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                DLZ58MDT AS Z58MDT,
	                DLZ58PDT AS Z58PDT,
	                PDAN8 AS AN8,
	                ABALPH AS ALPH,
                    DLMUPM AS UPMJ,
                    DLUPMT AS TDAY,
	                DLPQOH AS RQOH,
	                DLMUPM AS MUPM,
	                DLUPMT AS UPMT,
                    DLUKID AS UKID,
                    DLIOPR AS IOPR,
	                'PURCFG' AS PROC
                  FROM
                    F580569
                    INNER JOIN F580055
                    ON RHZ58RID = DLZ58RID
                    INNER JOIN F4102
                    ON	IBMCU = DLMCU AND
                        IBITM = DLITM
	                INNER JOIN F3201
	                ON	CMKCOO = DLPKCO AND
		                CMDOCO = DLDOC2 AND
		                CMDCTO = DLXCTO AND
		                CMLNID = DLLNIC 
	                INNER JOIN F4311
	                ON	PDKCOO = DLPKCO AND
		                PDDOCO = DLDOC2 AND
		                PDDCTO = DLXCTO AND
		                PDLNID = DLLNIC
	                INNER JOIN F0101 
	                ON	ABAN8 = PDAN8
                  WHERE
                     DLUKID > ? AND DLMCU IN (?,?,?,?) AND
                     DLIOPR = 'Insert' AND
                        IBSTKT IN ( 'C' ) AND
                        RHZ58RT != 'EXCPT' AND
	                    DLDOC2 > 0 AND
	                    DLMCU = PDMCU AND
	                    (CMPID != 'P580150' OR MOD(DLDOC2, 1000) != 0)

                UNION ALL

                SELECT
	                DLLOTN AS LOTN,
	                DLZ58SN AS Z58SN,
	                DLITM AS ITM,
	                DLZ58DST AS Z58DST,
	                DLZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                DLPQOH AS PQOH,
	                DLZ58PS AS Z58PS,
	                DLZ58SL AS Z58SL,
	                DLZ58STK AS Z58STK,
	                DLZ58ROW AS Z58ROW,
	                DLZ58SD AS Z58SD,
                    DLZ58BXRO AS Z58BXRO,
	                DLDOC1 AS DOCO,
	                DLXDCTO AS DCTO,
	                0 AS LNID,
	                CMCFGID AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                DLZ58MDT AS Z58MDT,
	                DLZ58PDT AS Z58PDT,
	                WAAN8 AS AN8,
	                ABALPH AS ALPH,
                    DLMUPM AS UPMJ,
                    DLUPMT AS TDAY,
	                DLPQOH AS RQOH,
	                DLMUPM AS MUPM,
	                DLUPMT AS UPMT,
                    DLUKID AS UKID,
                    DLIOPR AS IOPR,
	                'PRDCFG' AS PROC
                  FROM
                    F580569
                    INNER JOIN F580055
                    ON	RHZ58RID = DLZ58RID
                    INNER JOIN F4102
                    ON	IBMCU = DLMCU AND
                        IBITM = DLITM
	                INNER JOIN F3201
	                ON	CMKCOO = '*' AND
		                CMDOCO = DLDOC1 AND
		                CMDCTO = DLXDCTO 	
	                INNER JOIN F4801 
	                ON	WADOCO = DLDOC1 AND
		                WAMCU = DLMCU
	                INNER JOIN F0101 
	                ON	ABAN8 = WAAN8
                  WHERE
                     DLUKID > ? AND DLMCU IN (?,?,?,?) AND
                    DLIOPR = 'Insert' AND
                    IBSTKT IN ( 'C' ) AND
                    RHZ58RT != 'EXCPT' AND
	                DLDOC1 > 0 AND
	                DLMCU = WAMCU AND
	                CMPID != 'P580150' AND
	                NOT EXISTS (
		                SELECT 
			                1 
		                FROM 
			                F4311
		                WHERE	
			                PDKCOO = DLPKCO AND
			                PDMCU = DLMCU AND
			                PDDOCO = DLDOC2 AND
			                PDDCTO = DLXCTO AND
			                PDLNID = DLLNIC)

                UNION ALL

                SELECT
	                DLLOTN AS LOTN,
	                DLZ58SN AS Z58SN,
	                DLITM AS ITM,
	                DLZ58DST AS Z58DST,
	                DLZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                DLPQOH AS PQOH,
	                DLZ58PS AS Z58PS,
	                DLZ58SL AS Z58SL,
	                DLZ58STK AS Z58STK,
	                DLZ58ROW AS Z58ROW,
	                DLZ58SD AS Z58SD,
                    DLZ58BXRO AS Z58BXRO,
	                DLDOC2 AS DOCO,
	                DLXCTO AS DCTO,
	                DLLNIC AS LNID,
	                0 AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                DLZ58MDT AS Z58MDT,
	                DLZ58PDT AS Z58PDT,
	                PDAN8 AS AN8,
	                ABALPH AS ALPH,
                    DLUPMJ AS UPMJ,
                    DLTDAY AS TDAY,
	                DLPQOH AS RQOH,
	                DLMUPM AS MUPM,
	                DLUPMT AS UPMT,
                    DLUKID AS UKID,
                    DLIOPR AS IOPR,
	                'PURSTK' AS PROC
                  FROM
                    F580569
                    INNER JOIN F580055
                    ON	RHZ58RID = DLZ58RID
                    INNER JOIN F4102
                    ON	IBMCU = DLMCU AND
                        IBITM = DLITM
	                INNER JOIN F4311 
	                ON	PDKCOO = DLPKCO AND
		                PDDOCO = DLDOC2 AND
		                PDDCTO = DLXCTO AND
		                PDLNID = DLLNIC
                    INNER JOIN F0101 
                    ON  ABAN8 = PDAN8
                  WHERE
                     DLUKID > ? AND DLMCU IN (?,?,?,?) AND
                    DLIOPR = 'Insert' AND
                    IBSTKT NOT IN ( 'C' ) AND
                    RHZ58RT != 'EXCPT' AND
	                DLDOC2 > 0 AND
	                PDMCU = DLMCU

                UNION ALL

                SELECT
	                DLLOTN AS LOTN,
	                DLZ58SN AS Z58SN,
	                DLITM AS ITM,
	                DLZ58DST AS Z58DST,
	                DLZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                DLPQOH AS PQOH,
	                DLZ58PS AS Z58PS,
	                DLZ58SL AS Z58SL,
	                DLZ58STK AS Z58STK,
	                DLZ58ROW AS Z58ROW,
	                DLZ58SD AS Z58SD,
                    DLZ58BXRO AS Z58BXRO,
	                DLDOC1 AS DOCO,
	                DLXDCTO AS DCTO,
	                0 AS LNID,
	                0 AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                DLZ58MDT AS Z58MDT,
	                DLZ58PDT AS Z58PDT,
	                ABAN8 AS AN8,
	                ABALPH AS ALPH,
	                DLMUPM AS UPMJ,
                    DLUPMT AS TDAY,
	                0 AS RQOH,
	                DLMUPM AS MUPM,
	                DLUPMT AS UPMT,
                    DLUKID AS UKID,
                    DLIOPR AS IOPR,
	                'PRDSTK' AS PROC
                  FROM
                    F580569
                    INNER JOIN F580055
                    ON	RHZ58RID = DLZ58RID
                    INNER JOIN F4102
                    ON	IBMCU = DLMCU AND
                         IBITM = DLITM
	                INNER JOIN F4801
	                ON	WADOCO = DLDOC1 AND
		                WAMCU = DLMCU
	                LEFT OUTER JOIN F580002
	                ON	IAITM = DLITM AND 
		                IAZ58ATNM = 'AN8DR'
                    LEFT OUTER JOIN F0101
                    ON	CAST(ABAN8 AS VARCHAR(10)) = IADVAL
                  WHERE
                     DLUKID > ? AND DLMCU IN (?,?,?,?) AND
                    DLIOPR = 'Insert' AND
                    IBSTKT NOT IN ( 'C' ) AND
                    RHZ58RT != 'EXCPT' AND
	                DLDOC1 > 0 AND
	                DLMCU = WAMCU AND
	                NOT EXISTS (
		                SELECT 
			                1 
		                FROM 
			                F4311
		                WHERE	
			                PDKCOO = DLPKCO AND
			                PDMCU = DLMCU AND
			                PDDOCO = DLDOC2 AND
			                PDDCTO = DLXCTO AND
			                PDLNID = DLLNIC)

                UNION ALL

                SELECT 
	                DLLOTN AS LOTN,
	                DLZ58SN AS Z58SN,
	                DLITM AS ITM,
	                DLZ58DST AS Z58DST,
	                DLZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                DLPQOH AS PQOH,
	                DLZ58PS AS Z58PS,
	                DLZ58SL AS Z58SL,
	                DLZ58STK AS Z58STK,
	                DLZ58ROW AS Z58ROW,
	                DLZ58SD AS Z58SD,
                    DLZ58BXRO AS Z58BXRO,
	                0 AS DOCO,
	                '' AS DCTO,
	                0 AS LNID,
	                0 AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                DLZ58MDT AS Z58MDT,
	                DLZ58PDT AS Z58PDT,
	                ABAN8 AS AN8,
	                ABALPH AS ALPH,
                    DLUPMJ AS UPMJ,
                    DLTDAY AS TDAY,
	                0 AS RQOH,
	                DLMUPM AS MUPM,
	                DLUPMT AS UPMT,
                    DLUKID AS UKID,
                    DLIOPR AS IOPR,
	                'MISC' AS PROC
                 from 
	                F580569
	                INNER JOIN F580055 
	                ON	RHZ58RID = DLZ58RID 
                    INNER JOIN F4102
                    ON	IBMCU = DLMCU AND
                        IBITM = DLITM
	                LEFT OUTER JOIN F580002
	                ON	IAITM = DLITM AND 
		                IAZ58ATNM = 'AN8DR'
                    LEFT OUTER JOIN F0101
                    ON	CAST(ABAN8 AS VARCHAR(8)) = IADVAL
                  where   
                     DLUKID > ? AND DLMCU IN (?,?,?,?) AND
                    DLIOPR = 'Insert' AND
	                NOT EXISTS (
		                SELECT 
			                1 
		                FROM 
			                F4311 
		                WHERE 
			                PDKCOO = DLPKCO AND
			                PDDOCO = DLDOC2 AND
			                PDDCTO = DLXCTO AND
			                PDLNID = DLLNIC) AND
	                NOT EXISTS (
		                SELECT 
			                1 
		                FROM 
			                F4801 
		                WHERE 
			                WADOCO = DLDOC1) 
                  ORDER BY 1,2,3
	            FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("UKID1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID1"].Precision = 15;
            cmd.Parameters["UKID1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU2", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU3", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU3a", iDB2DbType.iDB2Graphic, 12));

            cmd.Parameters.Add(new iDB2Parameter("UKID2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID2"].Precision = 15;
            cmd.Parameters["UKID2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU4", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU5", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU6", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU6a", iDB2DbType.iDB2Graphic, 12));

            cmd.Parameters.Add(new iDB2Parameter("UKID3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID3"].Precision = 15;
            cmd.Parameters["UKID3"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU7", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU8", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU9", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU9a", iDB2DbType.iDB2Graphic, 12));

            cmd.Parameters.Add(new iDB2Parameter("UKID4", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID4"].Precision = 15;
            cmd.Parameters["UKID4"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU10", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU11", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU12", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU12a", iDB2DbType.iDB2Graphic, 12));

            cmd.Parameters.Add(new iDB2Parameter("UKID5", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID5"].Precision = 15;
            cmd.Parameters["UKID5"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU13", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU14", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU15", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU15a", iDB2DbType.iDB2Graphic, 12));

            return cmd;
        }

        /********************************************************************************
        * Rack Item
        ********************************************************************************/
        public static IDataReader GetRackItemDynamic(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant, decimal rackId, List<string> statusCodes)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                if (plant.PlantCode == "MZCG")
                {
                    filters.Add("((RDMCU = '          32') OR (RHLOCN LIKE 'MZ%' AND RDMCU IN ( '          04', '          05', '          08'))) AND \r\n");
                }
                else
                {
                    filters.Add("RDMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("RDZ58RID = ? AND \r\n");
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
                    filters.Add("RDZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Process Run Span
                //if (!processRunSpan.IsEmpty)
                //{
                //    filters.Add(E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                //}
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add("(" + E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", processRunSpan.E1TimeIdSpan) + " OR " + E1TimeManager.GetE1DateTimeQueryString("RHUPMJ", "RHTDAY", processRunSpan.E1TimeIdSpan) + ") AND \r\n");
                }

                // Command
                var cmd = GetRackItemDynamic(filters);
                for (int i = 0; i < 5; i++)
                {
                    foreach (iDB2Parameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
        }

        private static iDB2Command GetRackItemDynamic(IReadOnlyCollection<string> filters)
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
	                RDLOTN AS LOTN,
	                RDZ58SN AS Z58SN,
	                RDITM AS ITM,
	                RDZ58DST AS Z58DST,
	                RDZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                RDPQOH AS PQOH,
	                RDZ58PS AS Z58PS,
	                RDZ58SL AS Z58SL,
	                RDZ58STK AS Z58STK,
	                RDZ58ROW AS Z58ROW,
	                RDZ58SD AS Z58SD,
                    RDZ58BXRO AS Z58BXRO,
	                RDDOC2 AS DOCO,
	                RDXCTO AS DCTO,
	                RDLNIC AS LNID,
	                CMCFGID AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                RDZ58MDT AS Z58MDT,
	                RDZ58PDT AS Z58PDT,
	                PDAN8 AS AN8,
	                ABALPH AS ALPH,
                    RDUPMJ AS UPMJ,
                    RDTDAY AS TDAY,
	                RDPQOH AS RQOH,
	                DLMUPM AS MUPM,
	                DLUPMT AS UPMT,
                    CAST(0 AS DECIMAL(15,0)) AS UKID,
                    '' AS IOPR,
	                'PURCFG' AS PROC
                  FROM
                    F580056
                    INNER JOIN F580055
                    ON RHZ58RID = RDZ58RID
                    INNER JOIN F4102
                    ON	IBMCU = RDMCU AND
                        IBITM = RDITM
	                INNER JOIN F3201
	                ON	CMKCOO = RDPKCO AND
		                CMDOCO = RDDOC2 AND
		                CMDCTO = RDXCTO AND
		                CMLNID = RDLNIC 
	                INNER JOIN F4311
	                ON	PDKCOO = RDPKCO AND
		                PDDOCO = RDDOC2 AND
		                PDDCTO = RDXCTO AND
		                PDLNID = RDLNIC
	                INNER JOIN F0101 
	                ON	ABAN8 = PDAN8
	                LEFT OUTER JOIN F580569
	                ON	DLMCU = RDMCU AND 
		                DLZ58RID = RDZ58RID AND 
		                DLZ58SN = RDZ58SN AND 
		                DLLOTN = RDLOTN AND 
		                DLIOPR = 'Insert'
                  WHERE
                    {filterString}
                    IBSTKT IN ( 'C' ) AND
                    RHZ58RT != 'EXCPT' AND
	                RDDOC2 > 0 AND
	                RDMCU = PDMCU AND
	                (CMPID != 'P580150' OR MOD(RDDOC2, 1000) != 0)

                UNION ALL

                SELECT
	                RDLOTN AS LOTN,
	                RDZ58SN AS Z58SN,
	                RDITM AS ITM,
	                RDZ58DST AS Z58DST,
	                RDZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                RDPQOH AS PQOH,
	                RDZ58PS AS Z58PS,
	                RDZ58SL AS Z58SL,
	                RDZ58STK AS Z58STK,
	                RDZ58ROW AS Z58ROW,
	                RDZ58SD AS Z58SD,
                    RDZ58BXRO AS Z58BXRO,
	                RDDOC1 AS DOCO,
	                RDXDCTO AS DCTO,
	                0 AS LNID,
	                CMCFGID AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                RDZ58MDT AS Z58MDT,
	                RDZ58PDT AS Z58PDT,
	                WAAN8 AS AN8,
	                ABALPH AS ALPH,
                    RDUPMJ AS UPMJ,
                    RDTDAY AS TDAY,
	                0 AS RQOH,
	                0 AS MUPM,
	                0 AS UPMT,
                    0 AS UKID,
                    '' AS IOPR,
	                'PRDCFG' AS PROC
                  FROM
                    F580056
                    INNER JOIN F580055
                    ON	RHZ58RID = RDZ58RID
                    INNER JOIN F4102
                    ON	IBMCU = RDMCU AND
                        IBITM = RDITM
	                INNER JOIN F3201
	                ON	CMKCOO = '*' AND
		                CMDOCO = RDDOC1 AND
		                CMDCTO = RDXDCTO 	
	                INNER JOIN F4801 
	                ON	WADOCO = RDDOC1 AND
		                WAMCU = RDMCU
	                INNER JOIN F0101 
	                ON	ABAN8 = WAAN8
                  WHERE
                    {filterString}
                    IBSTKT IN ( 'C' ) AND
                    RHZ58RT != 'EXCPT' AND
	                RDDOC1 > 0 AND
	                RDMCU = WAMCU AND
	                CMPID != 'P580150' AND
	                NOT EXISTS (
		                SELECT 
			                1 
		                FROM 
			                F4311
		                WHERE	
			                PDKCOO = RDPKCO AND
			                PDMCU = RDMCU AND
			                PDDOCO = RDDOC2 AND
			                PDDCTO = RDXCTO AND
			                PDLNID = RDLNIC)

                UNION ALL

                SELECT
	                RDLOTN AS LOTN,
	                RDZ58SN AS Z58SN,
	                RDITM AS ITM,
	                RDZ58DST AS Z58DST,
	                RDZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                RDPQOH AS PQOH,
	                RDZ58PS AS Z58PS,
	                RDZ58SL AS Z58SL,
	                RDZ58STK AS Z58STK,
	                RDZ58ROW AS Z58ROW,
	                RDZ58SD AS Z58SD,
                    RDZ58BXRO AS Z58BXRO,
	                RDDOC2 AS DOCO,
	                RDXCTO AS DCTO,
	                RDLNIC AS LNID,
	                0 AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                RDZ58MDT AS Z58MDT,
	                RDZ58PDT AS Z58PDT,
	                PDAN8 AS AN8,
	                ABALPH AS ALPH,
                    RDUPMJ AS UPMJ,
                    RDTDAY AS TDAY,
	                RDPQOH AS RQOH,
	                DLMUPM AS MUPM,
	                DLUPMT AS UPMT,
                    0 AS UKID,
                    '' AS IOPR,
	                'PURSTK' AS PROC
                  FROM
                    F580056
                    INNER JOIN F580055
                    ON	RHZ58RID = RDZ58RID
                    INNER JOIN F4102
                    ON	IBMCU = RDMCU AND
                        IBITM = RDITM
	                INNER JOIN F4311 INNER JOIN F0101 ON ABAN8 = PDAN8
	                ON	PDKCOO = RDPKCO AND
		                PDDOCO = RDDOC2 AND
		                PDDCTO = RDXCTO AND
		                PDLNID = RDLNIC
	                LEFT OUTER JOIN F580569
	                ON	DLMCU = RDMCU AND 
		                DLZ58RID = RDZ58RID AND 
		                DLZ58SN = RDZ58SN AND 
		                DLLOTN = RDLOTN AND 
		                DLIOPR = 'Insert'
                  WHERE
                    {filterString}
                    IBSTKT NOT IN ( 'C' ) AND
                    RHZ58RT != 'EXCPT' AND
	                RDDOC2 > 0 AND
	                PDMCU = RDMCU

                UNION ALL

                SELECT
	                RDLOTN AS LOTN,
	                RDZ58SN AS Z58SN,
	                RDITM AS ITM,
	                RDZ58DST AS Z58DST,
	                RDZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                RDPQOH AS PQOH,
	                RDZ58PS AS Z58PS,
	                RDZ58SL AS Z58SL,
	                RDZ58STK AS Z58STK,
	                RDZ58ROW AS Z58ROW,
	                RDZ58SD AS Z58SD,
                    RDZ58BXRO AS Z58BXRO,
	                RDDOC1 AS DOCO,
	                RDXDCTO AS DCTO,
	                0 AS LNID,
	                0 AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                RDZ58MDT AS Z58MDT,
	                RDZ58PDT AS Z58PDT,
	                ABAN8 AS AN8,
	                ABALPH AS ALPH,
	                RDUPMJ AS UPMJ,
                    RDTDAY AS TDAY,
	                0 AS RQOH,
	                0 AS MUPM,
	                0 AS UPMT,
                    0 AS UKID,
                    '' AS IOPR,
	                'PRDSTK' AS PROC
                  FROM
                    F580056
                    INNER JOIN F580055
                    ON	RHZ58RID = RDZ58RID
                    LEFT OUTER JOIN F4102
                    ON	IBMCU = RDMCU AND
                         IBITM = RDITM
	                LEFT OUTER JOIN F4801
	                ON	WADOCO = RDDOC1 AND
		                WAMCU = RDMCU
	                LEFT OUTER JOIN F580002
	                ON	IAITM = RDITM AND 
		                IAZ58ATNM = 'AN8DR'
                    LEFT OUTER JOIN F0101
                    ON	CAST(ABAN8 AS VARCHAR(10)) = IADVAL
                  WHERE
                    {filterString}
                    IBSTKT NOT IN ( 'C' ) AND
                    RHZ58RT != 'EXCPT' AND
	                RDDOC1 > 0 AND
	                RDMCU = WAMCU AND
	                NOT EXISTS (
		                SELECT 
			                1 
		                FROM 
			                F4311
		                WHERE	
			                PDKCOO = RDPKCO AND
			                PDMCU = RDMCU AND
			                PDDOCO = RDDOC2 AND
			                PDDCTO = RDXCTO AND
			                PDLNID = RDLNIC)

                UNION ALL

                SELECT 
	                RDLOTN AS LOTN,
	                RDZ58SN AS Z58SN,
	                RDITM AS ITM,
	                RDZ58DST AS Z58DST,
	                RDZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
	                RHZ58RT AS Z58RT,
	                RHLOCN AS LOCN,
	                RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
	                RDPQOH AS PQOH,
	                RDZ58PS AS Z58PS,
	                RDZ58SL AS Z58SL,
	                RDZ58STK AS Z58STK,
	                RDZ58ROW AS Z58ROW,
	                RDZ58SD AS Z58SD,
                    RDZ58BXRO AS Z58BXRO,
	                0 AS DOCO,
	                '' AS DCTO,
	                0 AS LNID,
	                0 AS CFGID,
	                IBSTKT AS STKT,
	                IBGLPT AS GLPT,
	                RDZ58MDT AS Z58MDT,
	                RDZ58PDT AS Z58PDT,
	                ABAN8 AS AN8,
	                ABALPH AS ALPH,
                    RDUPMJ AS UPMJ,
                    RDTDAY AS TDAY,
	                0 AS RQOH,
	                0 AS MUPM,
	                0 AS UPMT,
                    0 AS UKID,
                    '' AS IOPR,
	                'MISC' AS PROC
                 from 
	                F580056 
	                INNER JOIN F580055 
	                ON	RHZ58RID = RDZ58RID 
                    LEFT OUTER JOIN F4102
                    ON	IBMCU = RDMCU AND
                        IBITM = RDITM
	                LEFT OUTER JOIN F580002
	                ON	IAITM = RDITM AND 
		                IAZ58ATNM = 'AN8DR'
                    LEFT OUTER JOIN F0101
                    ON	CAST(ABAN8 AS VARCHAR(8)) = IADVAL
                  where   
                    {filterString}
	                NOT EXISTS (
		                SELECT 
			                1 
		                FROM 
			                F4311 
		                WHERE 
			                PDKCOO = RDPKCO AND
			                PDDOCO = RDDOC2 AND
			                PDDCTO = RDXCTO AND
			                PDLNID = RDLNIC) AND
	                NOT EXISTS (
		                SELECT 
			                1 
		                FROM 
			                F4801 
		                WHERE 
			                WADOCO = RDDOC1) 
                  ORDER BY  
                    2 ,
                    3,4
	            FOR READ ONLY"
            };
            return cmd;
        }

        /********************************************************************************
        * Rack Item Status
        ********************************************************************************/
        public static IDataReader GetRackItemStatus(BusinessSystemSession session, ProcessRunSpan processRunSpanRack, ProcessRunSpan processRunSpanRackItem, Plant plant, decimal rackId, List<string> statusCodes)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var parameters1 = new List<iDB2Parameter>();
            var parameters2 = new List<iDB2Parameter>();
            var filters1 = new List<string>();
            var filters2 = new List<string>();

            try
            {
                // Plant - Required
                filters1.Add("RDMCU = ? AND \r\n");
                parameters1.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters1.Last().Value = plant.PlantNum.PadLeft(12);

                filters2.Add("RDMCU = ? AND \r\n");
                parameters2.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters2.Last().Value = plant.PlantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters1.Add("RDZ58RID = ? AND \r\n");
                    parameters1.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters1.Last().Precision = 15;
                    parameters1.Last().Scale = 0;
                    parameters1.Last().Value = rackId;

                    filters2.Add("RDZ58RID = ? AND \r\n");
                    parameters2.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters2.Last().Precision = 15;
                    parameters2.Last().Scale = 0;
                    parameters2.Last().Value = rackId;
                }

                // Status Codes
                if (statusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in statusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters1.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
                        parameters1.Last().Value = statusCode;
                        parameters2.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
                        parameters2.Last().Value = statusCode;
                    }
                    filters1.Add("RDZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                    filters2.Add("RDZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                if (processRunSpanRackItem.HighWaterMark.Exists)
                {
                    filters1.Add($"EXISTS (SELECT 1 FROM F580569 WHERE DLUKID > {processRunSpanRackItem.HighWaterMark.UniqueID} AND DLUKID <= {processRunSpanRackItem.HighWaterMark.MaxUniqueID} and DLMCU = '{plant.PlantNum.PadLeft(12)}' AND DLMCU = RDMCU AND DLLOTN = RDLOTN AND DLZ58SN = RDZ58SN) AND \r\n");
                    filters2.Add($"EXISTS (SELECT 1 FROM F580559 WHERE HLUKID > {processRunSpanRack.HighWaterMark.UniqueID} AND HLUKID <= {processRunSpanRack.HighWaterMark.MaxUniqueID} and HLMCU = '{plant.PlantNum.PadLeft(12)}' AND HLMCU = RDMCU AND HLZ58RID = RHZ58RID) AND \r\n");
                }
                else if (!processRunSpanRackItem.IsEmpty)
                {
                    filters1.Add("(" + E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", processRunSpanRack.E1TimeIdSpan) + ") AND \r\n");
                    filters2.Add("(" + E1TimeManager.GetE1DateTimeQueryString("RHUPMJ", "RHTDAY", processRunSpanRack.E1TimeIdSpan) + ") AND \r\n");
                }

                // Command
                filters1[filters1.Count - 1] = filters1.Last().ToUpper().Replace(" AND \r\n", "");
                filters2[filters2.Count - 1] = filters2.Last().ToUpper().Replace(" AND \r\n", "");
                var cmd = GetRackItemStatus(filters1, filters2);
                foreach (iDB2Parameter parameter in parameters1)
                {
                    cmd.Parameters.Add(parameter);
                }
                foreach (iDB2Parameter parameter in parameters2)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
        }

        private static iDB2Command GetRackItemStatus(IReadOnlyCollection<string> filters1, IReadOnlyCollection<string> filters2)
        {
            var filterString1 = new StringBuilder();
            var filterString2 = new StringBuilder();
            foreach (var filter in filters1)
            {
                filterString1.Append(filter);
            }
            foreach (var filter in filters2)
            {
                filterString2.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
	                RDMCU AS MCU,
                    RDLOTN AS LOTN,
                    RDZ58SN AS Z58SN,
                    RDITM AS ITM,
                    RDZ58DST AS Z58DST,
                    RDZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
                    RHZ58RT AS Z58RT,
                    RHLOCN AS LOCN,
                    RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
                    RDPQOH AS PQOH,
                    RDZ58PS AS Z58PS,
                    RDZ58SL AS Z58SL,
                    RDZ58STK AS Z58STK,
                    RDZ58ROW AS Z58ROW,
                    RDZ58SD AS Z58SD,
                    RDZ58BXRO AS Z58BXRO,
                    RDZ58MDT AS Z58MDT,
                    RDZ58PDT AS Z58PDT,
		            RDDOCO AS DOCO,
		            RDDCTO AS DCTO,
		            RDLNID AS LNID,
		            RDDOC1 AS DOC1,
		            RDXDCTO AS XDCTO,
                    RDDOC2 AS DOC2,
                    RDXCTO AS XCTO,
                    RDLNIC AS LNIC,
                    IBSTKT AS STKT,
                    IBGLPT AS GLPT,
                    RDUPMJ AS UPMJ,
                    RDTDAY AS TDAY,
                    CAST(0 AS DECIMAL(15,0)) AS UKID,
                    '' AS IOPR,
                    '' AS VERS
                FROM
                    F580056
                    INNER JOIN F580055
                        ON  RHZ58RID = RDZ58RID
                    LEFT OUTER JOIN F4102
                        ON  IBMCU = RDMCU AND
                            IBITM = RDITM
                WHERE
                    {filterString1} 
            UNION
                SELECT
	                RDMCU AS MCU,
                    RDLOTN AS LOTN,
                    RDZ58SN AS Z58SN,
                    RDITM AS ITM,
                    RDZ58DST AS Z58DST,
                    RDZ58RID AS Z58RID,
                    RHZ58HST AS Z58HST,
                    RHZ58RT AS Z58RT,
                    RHLOCN AS LOCN,
                    RHURRF AS URRF,
                    RHZ58SCN AS Z58SCN,
                    RHZ58BOX AS Z58BOX,
                    RDPQOH AS PQOH,
                    RDZ58PS AS Z58PS,
                    RDZ58SL AS Z58SL,
                    RDZ58STK AS Z58STK,
                    RDZ58ROW AS Z58ROW,
                    RDZ58SD AS Z58SD,
                    RDZ58BXRO AS Z58BXRO,
                    RDZ58MDT AS Z58MDT,
                    RDZ58PDT AS Z58PDT,
		            RDDOCO AS DOCO,
		            RDDCTO AS DCTO,
		            RDLNID AS LNID,
		            RDDOC1 AS DOC1,
		            RDXDCTO AS XDCTO,
                    RDDOC2 AS DOC2,
                    RDXCTO AS XCTO,
                    RDLNIC AS LNIC,
                    IBSTKT AS STKT,
                    IBGLPT AS GLPT,
                    RDUPMJ AS UPMJ,
                    RDTDAY AS TDAY,
                    CAST(0 AS DECIMAL(15,0)) AS UKID,
                    '' AS IOPR,
                    '' AS VERS
                FROM
                    F580056
                    INNER JOIN F580055
                        ON  RHZ58RID = RDZ58RID
                    LEFT OUTER JOIN F4102
                        ON  IBMCU = RDMCU AND
                            IBITM = RDITM
                WHERE
                    {filterString2} 
	            ORDER BY 1,2,3,4
	            FOR READ ONLY"
            };
            return cmd;
        }

        /******************************************************************
         * Received Qty
         ******************************************************************/
        public iDB2Command GetReceivedQty()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                DLPQOH,
                DLDOC1,
                DLXDCTO,
                DLDOC2,
                DLXCTO,
                DLLNIC
            FROM
                F580569
            WHERE
                DLMCU = ? AND
                DLZ58RID = ? AND
                DLZ58SN = ? AND
                DLIOPR = 'Insert'
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RID"].Precision = 15;
            cmd.Parameters["RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SN", iDB2DbType.iDB2Graphic, 20));
            return cmd;
        }

        /******************************************************************
         * Max UKID
         ******************************************************************/
        public iDB2Command GetMaxDlUkid()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                MAX(DLUKID) AS UKID
            FROM
                F580569
            WHERE
                DLMCU = ? AND
                DLUKID >= ?
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID"].Precision = 15;
            cmd.Parameters["UKID"].Scale = 0;
            return cmd;
        }

        public long GetMaxDlUkid(BusinessSystemSession session, Plant plant, decimal minUkid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            long ukid = 0;
            try
            {
                var cmd = session.DbCommands["RackItemDownloadManager.GetMaxDlUkid"];
                cmd.Connection = session.DbConnection;
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = minUkid;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        ukid = (long)Data.CastDbNull.To<decimal>(reader["UKID"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return ukid;
        }

        public iDB2Command GetMaxHlUkid()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                MAX(HLUKID) AS UKID
            FROM
                F580559
            WHERE
                HLMCU = ? AND
                HLUKID >= ?
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID"].Precision = 15;
            cmd.Parameters["UKID"].Scale = 0;
            return cmd;
        }

        public long GetMaxHlUkid(BusinessSystemSession session, Plant plant, decimal minUkid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            long ukid = 0;
            try
            {
                //var cmd = (session.DbCommands.ContainsKey("RackItemDownloadManager.GetMaxHlUkid")) ? session.DbCommands["RackItemDownloadManager.GetMaxHlUkid"] : GetMaxHlUkid();
                var cmd = session.DbCommands["RackItemDownloadManager.GetMaxHlUkid"];
                cmd.Connection = session.DbConnection;
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = minUkid;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        ukid = (long)Data.CastDbNull.To<decimal>(reader["UKID"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return ukid;
        }
    }
}
