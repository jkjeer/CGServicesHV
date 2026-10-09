using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
    public class HvWoIssueItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HvWoIssueItemManager(BusinessSystemSession e1Session)
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
                    HVWOIssuesID,
                    PlantID,
                    WOrderNum,
                    WOrderType,
                    TransactionDate,
                    ProductionDate,
                    ItemNum,
                    RawSerialNumber,
                    ProductionLineCode,
                    LineOperationCode,
                    E1Workcenter,
                    E1Operation,
                    Workstation,
                    LineID,
                    Shift,
                    Quantity,
                    QuantityCanceled,
                    SqftActual,
                    SqftBillable,
                    RejectCode,
                    Transferred,
                    TransferredDate,
                    Upload,
                    Crew,
                    LegacyWorkStation,
                    ProdType,
                    ShiftOverride,
                    RejectReason,
                    DimBase,
                    DimLeft,
                    Thickness,
                    Color,
                    HeatTreatment,
                    [1stSrfCoating] AS FirstSrfCoating,
                    [2ndSrfCoating] AS SecondSrfCoating,
                    Pattern,
                    RawCTCFCode,
                    DefaultSN,
                    DefaultSNNotification,
                    DefaultSNNotificationTS,
                    SNUpdateUID,
                    DoNotUpload,
                    LiteNum,
                    FGSerialNumber,
                    LoaderID,
                    ReportingOnly,
                    SNUpdateUTS,
                    HVTransactionHistoryID,
                    ISNULL(ClearPath.ufn_gSFgetBenchmarkRejectCode(ProductionLineCode,LineOperationCode,RejectCode), RejectCode) AS BenchmarkReasonCode,
                    CASE WHEN ISNULL(RTRIM(RejectReason),'') = '' THEN dbo.fnGetClearPathRejectDesc(RejectCode) ELSE RejectReason END AS RejectReasonDesc
                FROM 
                    dbo.HVWOIssues
                WHERE 
                    ISNULL(Upload, 0) = 0 AND ISNULL(DoNotUpload,0) = 0";
            return cmd;
        }

        public HvWoIssueItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new HvWoIssueItems();
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
                        var rec = new HvWoIssueItem();
                        //rec.PlantID = plant.PlantNum;
                        rec.HVWOIssuesID = Data.CastDbNull.To<int>(reader["HVWOIssuesID"], 0);
                        rec.PlantID = Data.CastDbNull.To<string>(reader["PlantID"], String.Empty).Trim();
                        rec.WOrderNum = Data.CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                        rec.WOrderType = Data.CastDbNull.To<string>(reader["WOrderType"], String.Empty).Trim();
                        rec.TransactionDate = Data.CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.MinValue);
                        rec.ProductionDate = Data.CastDbNull.To<DateTime>(reader["ProductionDate"], DateTime.MinValue);
                        rec.ItemNum = Data.CastDbNull.To<string>(reader["ItemNum"], String.Empty).Trim();
                        rec.RawSerialNumber = Data.CastDbNull.To<string>(reader["RawSerialNumber"], String.Empty).Trim();
                        rec.ProductionLineCode = Data.CastDbNull.To<string>(reader["ProductionLineCode"], String.Empty).Trim();
                        rec.LineOperationCode = Data.CastDbNull.To<string>(reader["LineOperationCode"], String.Empty).Trim();
                        rec.E1Workcenter = Data.CastDbNull.To<string>(reader["E1Workcenter"], String.Empty).Trim();
                        rec.E1Operation = Data.CastDbNull.To<string>(reader["E1Operation"], String.Empty).Trim();
                        rec.Workstation = Data.CastDbNull.To<string>(reader["Workstation"], String.Empty).Trim();
                        rec.LineID = Data.CastDbNull.To<string>(reader["LineID"], String.Empty).Trim();
                        rec.Shift = Data.CastDbNull.To<string>(reader["Shift"], String.Empty).Trim();
                        rec.Quantity = Data.CastDbNull.To<decimal>(reader["Quantity"], 0);
                        rec.QuantityCanceled = Data.CastDbNull.To<decimal>(reader["QuantityCanceled"], 0);
                        rec.SqftActual = Data.CastDbNull.To<decimal>(reader["SqftActual"], 0);
                        rec.SqftBillable = Data.CastDbNull.To<decimal>(reader["SqftBillable"], 0);
                        rec.RejectCode = Data.CastDbNull.To<string>(reader["RejectCode"], String.Empty).Trim();
                        rec.Transferred = Data.CastDbNull.To<bool>(reader["Transferred"], false);
                        rec.TransferredDate = Data.CastDbNull.To<DateTime>(reader["TransferredDate"], DateTime.MinValue);
                        rec.Upload = Data.CastDbNull.To<bool>(reader["Upload"], false);
                        rec.Crew = Data.CastDbNull.To<string>(reader["Crew"], String.Empty).Trim();
                        rec.LegacyWorkStation = Data.CastDbNull.To<string>(reader["LegacyWorkStation"], String.Empty).Trim();
                        rec.ProdType = Data.CastDbNull.To<string>(reader["ProdType"], String.Empty).Trim();
                        rec.ShiftOverride = Data.CastDbNull.To<bool>(reader["ShiftOverride"], false);
                        rec.RejectReason = Data.CastDbNull.To<string>(reader["RejectReason"], String.Empty).Trim();
                        rec.DimBase = Data.CastDbNull.To<double>(reader["DimBase"], 0);
                        rec.DimLeft = Data.CastDbNull.To<double>(reader["DimLeft"], 0);
                        rec.Thickness = Data.CastDbNull.To<decimal>(reader["Thickness"], 0);
                        rec.Color = Data.CastDbNull.To<string>(reader["Color"], String.Empty).Trim();
                        rec.HeatTreatment = Data.CastDbNull.To<string>(reader["HeatTreatment"], String.Empty).Trim();
                        rec.FirstSrfCoating = Data.CastDbNull.To<string>(reader["FirstSrfCoating"], String.Empty).Trim();
                        rec.SecondSrfCoating = Data.CastDbNull.To<string>(reader["SecondSrfCoating"], String.Empty).Trim();
                        rec.Pattern = Data.CastDbNull.To<string>(reader["Pattern"], String.Empty).Trim();
                        rec.RawCTCFCode = Data.CastDbNull.To<string>(reader["RawCTCFCode"], String.Empty).Trim();
                        rec.DefaultSN = Data.CastDbNull.To<bool>(reader["DefaultSN"], false);
                        rec.DefaultSNNotification = Data.CastDbNull.To<bool>(reader["DefaultSNNotification"], false);
                        rec.DefaultSNNotificationTS = Data.CastDbNull.To<DateTime>(reader["DefaultSNNotificationTS"], DateTime.MinValue);
                        rec.SNUpdateUID = Data.CastDbNull.To<string>(reader["SNUpdateUID"], String.Empty).Trim();
                        rec.DoNotUpload = Data.CastDbNull.To<bool>(reader["DoNotUpload"], false);
                        rec.LiteNum = Data.CastDbNull.To<int>(reader["LiteNum"], 0);
                        rec.FGSerialNumber = Data.CastDbNull.To<string>(reader["FGSerialNumber"], String.Empty).Trim();
                        rec.LoaderID = Data.CastDbNull.To<string>(reader["LoaderID"], String.Empty).Trim();
                        rec.ReportingOnly = Data.CastDbNull.To<bool>(reader["ReportingOnly"], false);
                        rec.SNUpdateUTS = Data.CastDbNull.To<DateTime>(reader["SNUpdateUTS"], DateTime.MinValue);
                        rec.HVTransactionHistoryID = Data.CastDbNull.To<int>(reader["HVTransactionHistoryID"], 0);
                        rec.BenchmarkReasonCode = Data.CastDbNull.To(reader["BenchmarkReasonCode"], string.Empty).Trim();
                        rec.RejectReasonDesc = Data.CastDbNull.To(reader["RejectReasonDesc"], string.Empty).Trim();
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
        public SqlDmlResponses SaveTransferProperties(HvWoIssueItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    var cmdStatement = $@"UPDATE dbo.HVWOIssues SET Upload = 1, TransferredDate = GETDATE() WHERE HVWOIssuesID = {record.HVWOIssuesID};";
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
                cmd.CommandText = $"UPDATE dbo.HVWOIssues SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
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
        public F580030s Transform(HvWoIssueItems records)
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
                    f580030.SFEDCT = "HW";
                    f580030.SFTYTN = "CGHV";
                    f580030.SFEDFT = "CGHVW";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = (rec.ReportingOnly) ? "B" : "";
                    f580030.SFPNID = "HVWOIssues";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.ProductionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLITM = rec.ItemNum;
                    f580030.SFZ58SN = rec.RawSerialNumber;
                    f580030.SFDOCO = rec.WOrderNum;
                    f580030.SFDCTO = (rec.WOrderNum > 0) ? "WO" : "";
                    f580030.SFMCU = rec.E1Workcenter.PadLeft(12);
                    f580030.SFSFT = rec.Shift;
                    f580030.SFUORG = rec.Quantity * 1000;
                    f580030.SFSOCN = rec.QuantityCanceled * 1000;
                    f580030.SFZ58IGLN = rec.LineID.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58UID = rec.HVWOIssuesID.ToString();
                    f580030.SFZ58BSF = rec.SqftBillable * 100;
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
