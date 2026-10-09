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
    public class HvTrimItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HvTrimItemManager(BusinessSystemSession e1Session)
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
                    HVTrimID,
                    PlantID,
                    E1Schedule,
                    FirmSchedule,
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
                    Crew,
                    TrimSide,
                    RejectCode,
                    SqftTrim,
                    Transferred,
                    TransferredDate,
                    RackID,
                    ShiftOverride
	            FROM 
                    dbo.HVTrim
	            WHERE 
                    Transferred = 0";
            return cmd;
        }

        public HvTrimItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new HvTrimItems();
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
                        var rec = new HvTrimItem();
                        rec.PlantID = plant.PlantNum;
                        rec.HVTrimID = Data.CastDbNull.To<int>(reader["HVTrimID"], 0);
                        //rec.PlantID = Data.CastDbNull.To<string>(reader["PlantID"], String.Empty).Trim();
                        rec.E1Schedule = Data.CastDbNull.To<decimal>(reader["E1Schedule"], 0);
                        rec.FirmSchedule = Data.CastDbNull.To<decimal>(reader["FirmSchedule"], 0);
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
                        rec.Crew = Data.CastDbNull.To<string>(reader["Crew"], String.Empty).Trim();
                        rec.TrimSide = Data.CastDbNull.To<string>(reader["TrimSide"], String.Empty).Trim();
                        rec.RejectCode = Data.CastDbNull.To<string>(reader["RejectCode"], String.Empty).Trim();
                        rec.SqftTrim = Data.CastDbNull.To<decimal>(reader["SqftTrim"], 0);
                        rec.Transferred = Data.CastDbNull.To<bool>(reader["Transferred"], false);
                        rec.TransferredDate = Data.CastDbNull.To<DateTime>(reader["TransferredDate"], DateTime.MinValue);
                        rec.RackID = Data.CastDbNull.To<decimal>(reader["RackID"], 0);
                        rec.ShiftOverride = Data.CastDbNull.To<bool>(reader["ShiftOverride"], false);
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
        public SqlDmlResponses SaveTransferProperties(HvTrimItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    var cmdStatement = $@"UPDATE dbo.HVTrim SET Transferred = 1, TransferredDate = GETDATE() WHERE HVTrimID = {record.HVTrimID};";
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
                cmd.CommandText = $"UPDATE dbo.HVTrim SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
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
        public F580030s Transform(HvTrimItems records)
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
                    f580030.SFEDCT = "HT";
                    f580030.SFTYTN = "CGHV";
                    f580030.SFEDFT = "CGHVT";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "HVTrim";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.ProductionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFZ58SN = rec.RawSerialNumber;
                    f580030.SFMCU = rec.E1Workcenter.PadLeft(12);
                    f580030.SFZ58SCN = rec.E1Schedule;
                    f580030.SFZ58SCSN = rec.FirmSchedule;
                    f580030.SFSFT = rec.Shift;
                    f580030.SFZ58RCP = (rec.TrimSide == "") ? ""
                        : (rec.TrimSide == "Trailing") ? "2N"
                        : (rec.TrimSide == "Leading") ? "2O"
                        : (rec.TrimSide == "Top") ? "2P" 
                        : (rec.TrimSide == "Bottom") ? "2Q" 
                        : "13";
                    f580030.SFZ58IGLN = rec.LineID.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58UID = rec.HVTrimID.ToString();
                    f580030.SFZ5850000 = rec.SqftTrim * 1000;
                    f580030.SFZ58PCR = rec.Crew.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFLITM = rec.ItemNum;
                    f580030.SFZ58RID = rec.RackID;
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

