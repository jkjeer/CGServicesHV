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
    public class HvCompletionItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HvCompletionItemManager(BusinessSystemSession e1Session)
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
                    HVCompletionsID,
                    PlantID,
                    WOrderNum,
                    WOrderType,
                    E1Schedule,
                    TransactionDate,
                    ProductionDate,
                    SerialNumber,
                    ProductionLineCode,
                    LineOperationCode,
                    E1Workcenter,
                    E1Operation,
                    WorkCenter,
                    LineID,
                    RoutingOperation,
                    Shift,
                    GlassHandler,
                    Quantity,
                    QuantityCanceled,
                    SqftActual,
                    SqftBillable,
                    RejectCode,
                    RawSerialNumber,
                    Transferred,
                    TransferredDate,
                    Lite,
                    ProdType,
                    Upload,
                    Crew,
                    FirmSchedule,
                    ShiftOverride,
                    RejectReason,
                    DimBase,
                    DimLeft,
                    Thickness,
                    Color,
                    HeatTreatment,
                    [1stSrfCoating],
                    [2ndSrfCoating],
                    Pattern,
                    RawCTCFCode,
                    DefaultSN,
                    DefaultSNNotification,
                    DefaultSNNotificationTS,
                    SNUpdateUID,
                    DoNotUpload,
                    RejectType,
                    ReportingOnly,
                    RackType,
                    SNUpdateUTS,
                    HVTransactionHistoryID,
                    CustPONum,
                    ISNULL(ClearPath.ufn_gSFgetBenchmarkRejectCode(ProductionLineCode,LineOperationCode,RejectCode), RejectCode) AS BenchmarkReasonCode,
                    CASE WHEN ISNULL(RTRIM(RejectReason),'') = '' THEN dbo.fnGetClearPathRejectDesc(RejectCode) ELSE RejectReason END AS RejectReasonDesc
	            FROM 
                    dbo.HVCompletions
	            WHERE 
                    ISNULL(Upload, 0) = 0 AND 
                    ISNULL(DoNotUpload,0) = 0 AND 
                    QuantityCanceled <> 0 AND 
                    ProductionLineCode NOT LIKE 'BKOUT%'";
            return cmd;
        }

        public HvCompletionItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new HvCompletionItems();
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
                        var rec = new HvCompletionItem();
                        //rec.PlantID = plant.PlantNum;
                        rec.HVCompletionsID = CastDbNull.To(reader["HVCompletionsID"], 0);
                        rec.PlantID = CastDbNull.To(reader["PlantID"], string.Empty).Trim();
                        rec.WOrderNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                        rec.WOrderType = CastDbNull.To(reader["WOrderType"], string.Empty).Trim();
                        rec.E1Schedule = CastDbNull.To<decimal>(reader["E1Schedule"], 0);
                        rec.TransactionDate = CastDbNull.To(reader["TransactionDate"], DateTime.MinValue);
                        rec.ProductionDate = CastDbNull.To(reader["ProductionDate"], DateTime.MinValue);
                        rec.SerialNumber = CastDbNull.To(reader["SerialNumber"], string.Empty).Trim();
                        rec.ProductionLineCode = CastDbNull.To(reader["ProductionLineCode"], string.Empty).Trim();
                        rec.LineOperationCode = CastDbNull.To(reader["LineOperationCode"], string.Empty).Trim();
                        rec.E1Workcenter = CastDbNull.To(reader["E1Workcenter"], string.Empty).Trim();
                        rec.E1Operation = CastDbNull.To(reader["E1Operation"], string.Empty).Trim();
                        rec.WorkCenter = CastDbNull.To(reader["WorkCenter"], string.Empty).Trim();
                        rec.LineID = CastDbNull.To(reader["LineID"], string.Empty).Trim();
                        rec.RoutingOperation = CastDbNull.To(reader["RoutingOperation"], string.Empty).Trim();
                        rec.Shift = CastDbNull.To(reader["Shift"], string.Empty).Trim();
                        rec.GlassHandler = CastDbNull.To<decimal>(reader["GlassHandler"], 0);
                        rec.Quantity = CastDbNull.To<decimal>(reader["Quantity"], 0);
                        rec.QuantityCanceled = CastDbNull.To<decimal>(reader["QuantityCanceled"], 0);
                        rec.SqftActual = CastDbNull.To<decimal>(reader["SqftActual"], 0);
                        rec.SqftBillable = CastDbNull.To<decimal>(reader["SqftBillable"], 0);
                        rec.RejectCode = CastDbNull.To(reader["RejectCode"], string.Empty).Trim();
                        rec.RawSerialNumber = CastDbNull.To(reader["RawSerialNumber"], string.Empty).Trim();
                        rec.Transferred = CastDbNull.To(reader["Transferred"], false);
                        rec.TransferredDate = CastDbNull.To(reader["TransferredDate"], DateTime.MinValue);
                        rec.Lite = CastDbNull.To(reader["Lite"], string.Empty).Trim();
                        rec.ProdType = CastDbNull.To(reader["ProdType"], string.Empty).Trim();
                        rec.Upload = CastDbNull.To(reader["Upload"], false);
                        rec.Crew = CastDbNull.To(reader["Crew"], string.Empty).Trim();
                        rec.FirmSchedule = CastDbNull.To<decimal>(reader["FirmSchedule"], 0);
                        rec.ShiftOverride = CastDbNull.To(reader["ShiftOverride"], false);
                        rec.RejectReason = CastDbNull.To(reader["RejectReason"], string.Empty).Trim();
                        rec.DimBase = CastDbNull.To<double>(reader["DimBase"], 0);
                        rec.DimLeft = CastDbNull.To<double>(reader["DimLeft"], 0);
                        rec.Thickness = CastDbNull.To<decimal>(reader["Thickness"], 0);
                        rec.Color = CastDbNull.To(reader["Color"], string.Empty).Trim();
                        rec.HeatTreatment = CastDbNull.To(reader["HeatTreatment"], string.Empty).Trim();
                        rec.FirstSrfCoating = CastDbNull.To(reader["1stSrfCoating"], string.Empty).Trim();
                        rec.SecondSrfCoating = CastDbNull.To(reader["2ndSrfCoating"], string.Empty).Trim();
                        rec.Pattern = CastDbNull.To(reader["Pattern"], string.Empty).Trim();
                        rec.RawCTCFCode = CastDbNull.To(reader["RawCTCFCode"], string.Empty).Trim();
                        rec.DefaultSN = CastDbNull.To(reader["DefaultSN"], false);
                        rec.DefaultSNNotification = CastDbNull.To(reader["DefaultSNNotification"], false);
                        rec.DefaultSNNotificationTS = CastDbNull.To(reader["DefaultSNNotificationTS"], DateTime.MinValue);
                        rec.SNUpdateUID = CastDbNull.To(reader["SNUpdateUID"], string.Empty).Trim();
                        rec.DoNotUpload = CastDbNull.To(reader["DoNotUpload"], false);
                        rec.RejectType = CastDbNull.To(reader["RejectType"], string.Empty).Trim();
                        rec.ReportingOnly = CastDbNull.To(reader["ReportingOnly"], false);
                        rec.RackType = CastDbNull.To(reader["RackType"], string.Empty).Trim();
                        rec.SNUpdateUTS = CastDbNull.To(reader["SNUpdateUTS"], DateTime.MinValue);
                        rec.HVTransactionHistoryID = CastDbNull.To(reader["HVTransactionHistoryID"], 0);
                        rec.CustPONum = CastDbNull.To(reader["CustPONum"], string.Empty).Trim();
                        rec.BenchmarkReasonCode = CastDbNull.To(reader["BenchmarkReasonCode"], string.Empty).Trim();
                        rec.RejectReasonDesc = CastDbNull.To(reader["RejectReasonDesc"], string.Empty).Trim();
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
        public SqlDmlResponses SaveTransferProperties(HvCompletionItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    var cmdStatement = $@"UPDATE dbo.HVCompletions SET Upload = 1, TransferredDate = GETDATE() WHERE HVCompletionsID = {record.HVCompletionsID};";
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
                cmd.CommandText = $"UPDATE dbo.HVCompletions SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
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
        public F580030s Transform(HvCompletionItems records)
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
                    f580030.SFEDUS = "SOASVC";
                    f580030.SFEDCT = "HC";
                    f580030.SFTYTN = (rec.ProdType == "MTO") ? "CGMTO" : "CGHV";
                    f580030.SFEDFT = "CGHVC";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = (rec.ReportingOnly) ? "B" : "";
                    f580030.SFPNID = "HVCompletions";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.ProductionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLOTN = rec.SerialNumber;
                    f580030.SFZ58SN = rec.RawSerialNumber;
                    f580030.SFDOCO = rec.WOrderNum;
                    f580030.SFDCTO = (rec.WOrderNum > 0) ? "WO" : "";
                    f580030.SFMCU = rec.E1Workcenter.PadLeft(12);
                    f580030.SFZ58SCN = rec.E1Schedule;
                    f580030.SFZ58SCSN = rec.FirmSchedule;
                    f580030.SFSFT = rec.Shift;
                    f580030.SFUORG = rec.Quantity * 1000;
                    f580030.SFSOCN = rec.QuantityCanceled * 1000;
                    f580030.SFZ58IGLN = rec.LineID.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58UID = rec.HVCompletionsID.ToString();
                    f580030.SFZ58PT = rec.ProdType;
                    f580030.SFZ58L1CLN = (rec.Lite == "EXT") ? 1 : (rec.Lite == "INT") ? 2 : 0;
                    f580030.SFZ58BSF = rec.SqftBillable*100;
                    f580030.SFZ5850000 = rec.SqftActual * 100;
                    f580030.SFZ58PCR = rec.Crew.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58LWSTN = rec.E1Operation;
                    f580030.SFZ58LNID = rec.HVTransactionHistoryID;
                    f580030.SFZ58RCP = rec.BenchmarkReasonCode;
                    f580030.SFZ58RCC = rec.RejectReasonDesc;
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

