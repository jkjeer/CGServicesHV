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

namespace CGServicesHV.Common.Managers.Process
{
    public class ShipmentSummaryDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public ShipmentSummaryDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"ShipmentSummaryDownloadManager.GetShipmentSummaryLedgerPrepared", GetShipmentSummaryLedgerPrepared()}
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
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ShipmentSummary") : null;
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
                var table = new ShipmentSummaryTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (useHighWaterMark)
                    {
                        Logger.Debug($@"GetShipmentSummaryLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetShipmentSummaryLedgerPrepared(session, processRunSpan, plant);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        Logger.Debug($@"GetShipmentSummaryLedgerPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        var reader = GetShipmentSummaryLedgerPrepared(session, processRunSpan, plant);
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

        public static IDataReader GetShipmentSummaryLedgerPrepared(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, Plant plant)
        {
            try
            {
                var cmd = e1Session.DbCommands["ShipmentSummaryDownloadManager.GetShipmentSummaryLedgerPrepared"];
                cmd.Parameters["UKID"].Value = processRunSpan.HighWaterMark.UniqueID;
                cmd.Parameters["MCU"].Value = plant.PlantNum.PadLeft(12);
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetShipmentSummaryLedgerPrepared()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        SLMCU,
                        SLZ58SHID,
                        SLAN8,
                        SLSHAN,
                        SLCARS,
                        SLZ58TN,
                        SLZ58TT,
                        SLFRTH,
                        SLZ58LOZN,
                        SLNSTP,
                        SLZ58RKN,
                        SLZ58MTRW,
                        SLDRQJ,
                        SLPSN,
                        SLLOAD,
                        SLDRQT,
                        SLZ58CMT1,
                        SLZ58CMT2,
                        SLZ58CMT3,
                        SLEDSP,
                        SLURCD,
                        SLURRF,
                        SLUSER,
                        SLPID,
                        SLJOBN,
                        SLUPMJ,
                        SLUPMT,
                        SLUKID,
                        SLMUSE,
                        SLJBNM,
                        SLJOB,
                        SLIOPR,
                        SLMUPM,
                        SLMUPT
                    FROM 
                        F5803679
                    WHERE 
                        SLUKID > ? 
                        AND SLMCU = ? 
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
