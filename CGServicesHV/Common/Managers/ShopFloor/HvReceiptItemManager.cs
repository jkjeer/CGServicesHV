using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
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
    public class HvReceiptItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HvReceiptItemManager(BusinessSystemSession e1Session)
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
                    HVReceiptsID,
                    PlantID,
                    WOrderNum,
                    WOrderType,
                    E1Schedule,
                    FirmSchedule,
                    TransactionDate,
                    ProductionDate,
                    SerialNumber,
                    Location,
                    ProductionLineCode,
                    LineOperationCode,
                    E1Workcenter,
                    E1Operation,
                    Workstation,
                    LineID,
                    Shift,
                    GlassHandler1,
                    GlassHandler2,
                    WIP,
                    Quantity,
                    SqftActual,
                    SqftBillable,
                    Transferred,
                    TransferredDate,
                    Upload,
                    ProdType,
                    Crew,
                    Lite,
                    ShiftOverride,
                    IPAddress,
                    Comment,
                    RackType,
                    ReportingOnly,
                    WIP_PAR,
                    PartialQty,
                    PartialSN,
                    RackStatus,
                    RackReasonStatus,
                    WIP_TYPE,
                    ProducedQuantity,
                    HVTransactionHistoryID,
                    FreeLiteQuantity,
                    AssetID
                FROM 
                    dbo.HVReceipts
                WHERE 
                    ISNULL(ProducedQuantity, 0) != 0 AND
                    ISNULL(Upload, 0) = 0";
            return cmd;
        }

        public HvReceiptItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new HvReceiptItems();
            try
            {
                // Command
                var cmd = Extract();
                cmd.Connection = cnShopFloor;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rec = new HvReceiptItem();
                        rec.HVReceiptsID = Data.CastDbNull.To(reader["HVReceiptsID"], 0);
                        rec.PlantID = Data.CastDbNull.To<string>(reader["PlantID"], String.Empty).Trim();
                        rec.WOrderNum = Data.CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                        rec.WOrderType = Data.CastDbNull.To(reader["WOrderType"], String.Empty).Trim();
                        rec.E1Schedule = Data.CastDbNull.To<decimal>(reader["E1Schedule"], 0);
                        rec.FirmSchedule = Data.CastDbNull.To<decimal>(reader["FirmSchedule"], 0);
                        rec.TransactionDate = Data.CastDbNull.To(reader["TransactionDate"], DateTime.MinValue);
                        rec.ProductionDate = Data.CastDbNull.To(reader["ProductionDate"], DateTime.MinValue);
                        rec.SerialNumber = Data.CastDbNull.To(reader["SerialNumber"], String.Empty).Trim();
                        rec.Location = Data.CastDbNull.To(reader["Location"], String.Empty).Trim();
                        rec.ProductionLineCode = Data.CastDbNull.To(reader["ProductionLineCode"], String.Empty).Trim();
                        rec.LineOperationCode = Data.CastDbNull.To(reader["LineOperationCode"], String.Empty).Trim();
                        rec.E1Workcenter = Data.CastDbNull.To(reader["E1Workcenter"], String.Empty).Trim();
                        rec.E1Operation = Data.CastDbNull.To(reader["E1Operation"], String.Empty).Trim();
                        rec.Workstation = Data.CastDbNull.To(reader["Workstation"], String.Empty).Trim();
                        rec.LineID = Data.CastDbNull.To(reader["LineID"], String.Empty).Trim();
                        rec.Shift = Data.CastDbNull.To(reader["Shift"], String.Empty).Trim();
                        rec.GlassHandler1 = Data.CastDbNull.To<decimal>(reader["GlassHandler1"], 0);
                        rec.GlassHandler2 = Data.CastDbNull.To<decimal>(reader["GlassHandler2"], 0);
                        rec.WIP = Data.CastDbNull.To(reader["WIP"], false);
                        rec.Quantity = Data.CastDbNull.To<decimal>(reader["Quantity"], 0);
                        rec.SqftActual = Data.CastDbNull.To<decimal>(reader["SqftActual"], 0);
                        rec.SqftBillable = Data.CastDbNull.To<decimal>(reader["SqftBillable"], 0);
                        rec.Transferred = Data.CastDbNull.To(reader["Transferred"], false);
                        rec.TransferredDate = Data.CastDbNull.To(reader["TransferredDate"], DateTime.MinValue);
                        rec.Upload = Data.CastDbNull.To(reader["Upload"], false);
                        rec.ProdType = Data.CastDbNull.To(reader["ProdType"], String.Empty).Trim();
                        rec.Crew = Data.CastDbNull.To(reader["Crew"], String.Empty).Trim();
                        rec.Lite = Data.CastDbNull.To(reader["Lite"], String.Empty).Trim();
                        rec.ShiftOverride = Data.CastDbNull.To(reader["ShiftOverride"], false);
                        rec.IPAddress = Data.CastDbNull.To(reader["IPAddress"], String.Empty).Trim();
                        rec.Comment = Data.CastDbNull.To(reader["Comment"], String.Empty).Trim();
                        rec.RackType = Data.CastDbNull.To(reader["RackType"], String.Empty).Trim();
                        rec.ReportingOnly = Data.CastDbNull.To(reader["ReportingOnly"], false);
                        rec.WIP_PAR = Data.CastDbNull.To(reader["WIP_PAR"], false);
                        rec.PartialQty = Data.CastDbNull.To<decimal>(reader["PartialQty"], 0);
                        rec.PartialSN = Data.CastDbNull.To(reader["PartialSN"], String.Empty).Trim();
                        rec.RackStatus = Data.CastDbNull.To(reader["RackStatus"], String.Empty).Trim();
                        rec.RackReasonStatus = Data.CastDbNull.To(reader["RackReasonStatus"], String.Empty).Trim();
                        rec.WIP_TYPE = Data.CastDbNull.To(reader["WIP_TYPE"], 0);
                        rec.ProducedQuantity = Data.CastDbNull.To<decimal>(reader["ProducedQuantity"], 0);
                        rec.HVTransactionHistoryID = Data.CastDbNull.To(reader["HVTransactionHistoryID"], 0);
                        rec.FreeLiteQuantity = Data.CastDbNull.To<decimal>(reader["FreeLiteQuantity"], 0);
                        rec.AssetID = Data.CastDbNull.To(reader["AssetID"], String.Empty).Trim();
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
        public SqlDmlResponses SaveTransferProperties(HvReceiptItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    var cmdStatement = $@"UPDATE dbo.HVReceipts SET Upload = 1, TransferredDate = GETDATE() WHERE HVReceiptsID = {record.HVReceiptsID};";
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
                cmd.CommandText = $"UPDATE dbo.HVReceipts SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
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
        public F580030s Transform(HvReceiptItems records)
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
                    f580030.SFEDBT = rec.TransferBatchId.ToString(CultureInfo.InvariantCulture);
                    f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580030.SFEDLN = 1000;
                    f580030.SFEDUS = "SOASVC";
                    f580030.SFEDCT = "HR";
                    f580030.SFTYTN = (rec.ProdType == "MTO") ? "CGMTO" : "CGHV";
                    f580030.SFEDFT = "CGHVR";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = (rec.ReportingOnly) ? "B" : "";
                    f580030.SFPNID = "HVReceipts";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.ProductionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLOTN = rec.SerialNumber;
                    f580030.SFZ58SN = rec.PartialSN;
                    f580030.SFDOCO = rec.WOrderNum;
                    f580030.SFDCTO = (rec.WOrderNum > 0) ? "WO" : "";
                    f580030.SFMCU = rec.E1Workcenter.PadLeft(12);
                    f580030.SFZ58SCN = rec.E1Schedule;
                    f580030.SFZ58SCSN = rec.FirmSchedule;
                    f580030.SFSFT = rec.Shift;
                    f580030.SFLOCN = rec.Location;
                    f580030.SFUORG = rec.ProducedQuantity * 1000;
                    f580030.SFSOCN = rec.PartialQty * 1000;
                    f580030.SFZ58IGLN = rec.LineID.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58UID = rec.HVReceiptsID.ToString();
                    f580030.SFZ58PT = rec.ProdType;
                    f580030.SFZ58L1CLN = (rec.Lite == "EXT") ? 1 : (rec.Lite == "INT") ? 2 : 0;
                    f580030.SFZ58BSF = rec.SqftBillable * 100;
                    f580030.SFZ5850000 = rec.SqftActual * 100;
                    f580030.SFFLAG = (rec.WIP) ? "1" : "";
                    f580030.SFZ58IP01 = rec.IPAddress;
                    f580030.SFZ58PCR = rec.Crew.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58LWSTN = rec.E1Operation;
                    f580030.SFZ58LNID = rec.HVTransactionHistoryID;
                    f580030.SFZ58RT = rec.RackType;
                    f580030.SFZ58HST = rec.RackStatus;
                    f580030.SFZ58RCD = rec.RackReasonStatus;
                    f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
                    f580030.SFZ58AID = rec.AssetID;
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
