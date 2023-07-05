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
using System.Data.SqlClient;
using System.Linq;

namespace CGServicesHV.Common.Managers.Process
{
    public class ShipmentPlanningDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public ShipmentPlanningDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"ShipmentPlanningDownloadManager.GetShipmentPlanningLedgerPrepared", GetShipmentPlanningLedgerPrepared()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Shipment Planning Download Manager");
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
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ShipmentPlanning") : null;
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
                var table = new ShipmentPlanningTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (useHighWaterMark)
                    {
                        Logger.Debug($@"GetShipmentPlanningLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetShipmentPlanningLedgerPrepared(session, processRunSpan, plant);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        Logger.Debug($@"GetShipmentPlanningLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetShipmentPlanningLedgerPrepared(session, processRunSpan, plant);
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
                        highWaterMark.UniqueID = (long)table.MaxId;
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime, highWaterMark.UniqueID));
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

        public static IDataReader GetShipmentPlanningLedgerPrepared(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, Plant plant)
        {
            try
            {
                var cmd = e1Session.DbCommands["ShipmentPlanningDownloadManager.GetShipmentPlanningLedgerPrepared"];
                cmd.Parameters["UKID"].Value = processRunSpan.HighWaterMark.UniqueID;
                cmd.Parameters["MCU"].Value = plant.PlantNum.PadLeft(12);
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetShipmentPlanningLedgerPrepared()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        PLZ58SPID,
                        PLSEQN,
                        PLEDSP,
                        PLZ58SPRT,
                        PLZ58LOZN,
                        PLZ58ORTY,
                        PLMCU,
                        PLSHAN,
                        PLCTY1,
                        PLADDS,
                        PLAN8,
                        PLZ58RKS,
                        PLDRQJ,
                        PLZ58GWTT,
                        PLZ58MTRW,
                        PLZ58LPR,
                        PLZ58RID,
                        PLZ58HST,
                        PLZ58AID,
                        PLZ58RT,
                        PLZ58RQSG,
                        PLDOCO,
                        PLDCTO,
                        PLKCOO,
                        PLLNID,
                        PLVR01,
                        PLITM,
                        PLDSC2,
                        PLZ58BOX,
                        PLZ58SCN,
                        PLCARS,
                        PLZ58SHLN,
                        PLZ58SHRT,
                        PLZ58TN,
                        PLFRTH,
                        PLZ58PT,
                        PLPSN,
                        PLZ58SHID,
                        PLURCD,
                        PLURRF,
                        PLJOBS,
                        PLUSER,
                        PLPID,
                        PLJOBN,
                        PLUPMJ,
                        PLUPMT,
                        PLUKID,
                        PLMUSE,
                        PLJBNM,
                        PLJOB,
                        PLIOPR,
                        PLMUPM,
                        PLMUPT,
						RHLOCN
                    FROM 
                        F5803669
						LEFT OUTER JOIN F580055
						    ON RHZ58RID = PLZ58RID
                    WHERE 
                        PLUKID > ? 
                        AND PLMCU = ? 
                    FOR READ ONLY
                "
            };
            cmd.Parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID"].Precision = 15;
            cmd.Parameters["UKID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

    }
}
