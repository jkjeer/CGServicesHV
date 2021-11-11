using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.ShopFloor
{
    public class ConsumedStoceItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ConsumedStoceItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"AddressBookDownloadManager.GetRelatedOrders", GetRelatedOrders()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /**********************************************************************
        * Get Cust Pack Records
        **********************************************************************/

        public SqlCommand Extract()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
	            SELECT 
                    GLStockID,
                    PlantID,
                    ProductionLineCode,
                    LineOperationCode,
                    LineTypeCode,
                    E1WorkCenter,
                    E1Operation,
                    RawThick,
                    RawCTCFCode,
                    Color,
                    HeatTreatment,
                    1stSrfCoating,
                    2ndSrfCoating,
                    Pattern,
                    RawGlassBaseL,
                    RawGlassBaseN,
                    RawGlassBaseD,
                    RawGlassLeftL,
                    RawGlassLeftN,
                    RawGlassLeftD,
                    SheetsConsumed,
                    SheetsRejected,
                    Shift,
                    CuttingTable,
                    RawCrateNumber,
                    WrittenDate,
                    Complete,
                    Transferred,
                    TransferDate,
                    Upload,
                    Crew,
                    RejectCode,
                    ProductionDate,
                    Workstation,
                    RawGlassRightL,
                    RawGlassRightN,
                    RawGlassRightD,
                    RawGlassTopL,
                    RawGlassTopN,
                    RawGlassTopD,
                    RawGlassM1L,
                    RawGlassM1N,
                    RawGlassM1D,
                    RawGlassM2L,
                    RawGlassM2N,
                    RawGlassM2D,
                    DoNotUpload,
                    ShiftOverride,
                    DefaultSN,
                    DefaultSNNotification,
                    DefaultSNNotificationTS,
                    Archive,
                    SNUpdateUID,
                    ConsumedStoceID,
                    RejectedStoceID,
                    ReportingOnly
	            FROM 
                    dbo.GlassStock
	            WHERE 
                    (Upload = 0 OR Upload IS NULL) AND Complete != 'Yes' AND ISNULL(DoNotUpload,0) = 0";
            return cmd;
        }

        public ConsumedStoceItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new ConsumedStoceItems();
            var cmd = new SqlCommand();
            try
            {
                // Command
                cmd = Extract();
                cmd.Connection = cnShopFloor;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rec = new ConsumedStoceItem();
                        rec.PlantID = plant.PlantNum;
                        rec.GLStockID = CastDbNull.To<int>(reader["GLStockID"], 0);
                        rec.ProductionLineCode = CastDbNull.To<String>(reader["ProductionLineCode"], String.Empty).Trim();
                        rec.Shift = CastDbNull.To<String>(reader["Shift"], String.Empty).Trim();
                        rec.Crew = CastDbNull.To<String>(reader["Crew"], String.Empty).Trim();
                        rec.CuttingTable = CastDbNull.To<String>(reader["CuttingTable"], String.Empty).Trim();
                        rec.E1WorkCenter = CastDbNull.To<String>(reader["E1WorkCenter"], String.Empty).Trim();
                        rec.E1Operation = CastDbNull.To<String>(reader["E1Operation"], String.Empty).Trim();
                        rec.RawCTCFCode = CastDbNull.To<String>(reader["RawCTCFCode"], String.Empty).Trim();
                        rec.SheetsConsumed = CastDbNull.To<int>(reader["SheetsConsumed"], 0);
                        rec.RawCrateNumber = CastDbNull.To<String>(reader["RawCrateNumber"], String.Empty).Trim();
                        rec.RawGlassBaseL = CastDbNull.To<Byte>(reader["RawGlassBaseL"], 0);
                        rec.RawGlassLeftL = CastDbNull.To<Byte>(reader["RawGlassLeftL"], 0);
                        rec.ReportingOnly = CastDbNull.To<bool>(reader["ReportingOnly"], false);
                        rec.ProductionDate = CastDbNull.To<DateTime>(reader["ProductionDate"], DateTime.MinValue);
                        rec.WrittenDate = CastDbNull.To<DateTime>(reader["WrittenDate"], DateTime.MinValue);
                        records.Add(rec);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }

            return records;
        }

        /**********************************************************************
            * Save Cust Pack Records to Shop Floor
            **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(ConsumedStoceItems items, short sFUpdateBatchSize, SqlConnection cn)
        {
            var responses = new SqlDmlResponses();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                var rowCount = 0;
                foreach (var record in items)
                {
                    rowCount += 1;
                    var cmdStatement = $@"UPDATE dbo.GlassStock SET Upload = 1, TransferDate = GETDATE() WHERE GLStockID = {record.GLStockID};";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sFUpdateBatchSize)
                    {
                        cmd.CommandText = cmdStringBldr.ToString();
                        Logger.Trace(Logging.DbCmdString(cmd));
                        int rows = cmd.ExecuteNonQuery();
                        cmdStringBldr = new StringBuilder();
                        rowCount = 0;
                    }
                }
                if (rowCount > 0)
                {
                    var dmlResponse = new SqlDmlResponse();
                    cmd.CommandText = cmdStringBldr.ToString();
                    dmlResponse.Init(cmd);
                    Logger.Trace(Logging.DbCmdString(cmd));
                    int rows = cmd.ExecuteNonQuery();
                    dmlResponse.RowsAffected = rows;
                    responses.Add(dmlResponse);
                }
                Logger.Info("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return responses;
        }

        /**********************************************************************
            * Set Transfer Status
            **********************************************************************/
        public SqlDmlResponse SetTransferStatus(SqlConnection cn, string preStatus, string postStatus)
        {
            var response = new SqlDmlResponse();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                cmd.CommandText = $"UPDATE dbo.GlassStock SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
                Logger.Trace(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return response;
        }

        /**********************************************************************
        * Save Cust Pack Records to E1
        **********************************************************************/
        public F580030s Transform(ConsumedStoceItems records)
        {
            var f580030s = new F580030s();
            try
            {
                int edtn = 0;
                foreach (var rec in records)
                {
                    var f580030 = new F580030();
                    edtn++;
                    f580030.SFUKID = rec.TransferId;
                    f580030.SFEDBT = rec.TransferBatchId.ToString();
                    f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580030.SFEDLN = 1000;
                    f580030.SFEDUS = "SVCSOA";
                    f580030.SFEDCT = "GS";
                    f580030.SFTYTN = "CGMTO";
                    f580030.SFEDFT = "CGGS";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = (rec.ReportingOnly) ? "B" : "";
                    f580030.SFPNID = "GlassStock";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.WrittenDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFZ58SN = rec.RawCrateNumber;
                    f580030.SFSFT = rec.Shift;
                    f580030.SFUORG = (rec.SheetsConsumed == 0) ? 1000 : rec.SheetsConsumed*1000;
                    f580030.SFZ58IGLN = rec.CuttingTable.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58UID = rec.GLStockID.ToString();
                    f580030.SFZ58PCR = rec.Crew.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFMCU = rec.E1WorkCenter.PadLeft(12);
                    f580030.SFZ58LWSTN = rec.E1Operation;
                    f580030.SFDTMS = $"{rec.ProductionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFZ58GT = rec.RawCTCFCode;
                    f580030.SFZ58BSF = rec.RawGlassBaseL*rec.RawGlassLeftL/144*100;
                    f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
                    f580030s.Add(f580030);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return f580030s;
        }
    }
}

