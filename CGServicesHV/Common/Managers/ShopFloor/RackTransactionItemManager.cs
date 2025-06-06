using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
    public class RackTransactionItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackTransactionItemManager(BusinessSystemSession e1Session)
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
        public SqlCommand Extract(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT
                    rt.PlantID as rtPlantID,
                    tr.PlantID,
	                tr.E1RackingTransactionId,
	                tr.DocumentType,
	                tr.SerialNumber,
	                tr.RackId,
	                ISNULL(tr.Location, '') AS Location,
	                tr.QtyOnHand,
	                tr.RackStatus,
	                tr.CreateDate,
	                tr.Transferred,
	                tr.TransferredDate,
	                tr.PCName,
	                tr.DestinationSerialNumber,
	                tr.[ReasonCode],
                    tr.[RackType],
	                tr.[E1WorkCenter],
                    tr.[DefaultSN],
	                rt.ItemNum
                FROM	
	                dbo.E1RackingTransactions tr
	                LEFT OUTER JOIN Rawtags rt
	                ON	rt.SerialNum = tr.SerialNumber
	            WHERE 
                    ISNULL(Transferred,0) = 0
            ";
            return cmd;
        }

        public RackTransactionItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor)
        {
            var records = new RackTransactionItems();
            var cmd = new SqlCommand();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                // Command
                cmd = Extract(filters);
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = cnShopFloor;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rec = new RackTransactionItem();
                        var plantId = CastDbNull.To<string>(reader["PlantID"], String.Empty).Trim();
                        var rtPlantId = CastDbNull.To<string>(reader["rtPlantID"], String.Empty).Trim();
                        rec.PlantID = String.IsNullOrEmpty(plantId) ? rtPlantId : plantId;
                        rec.E1RackingTransactionId = CastDbNull.To<int>(reader["E1RackingTransactionId"], 0);
                        rec.DocumentType = CastDbNull.To<string>(reader["DocumentType"], String.Empty).Trim();
                        rec.SerialNumber = CastDbNull.To<string>(reader["SerialNumber"], String.Empty).Trim();
                        rec.RackId = CastDbNull.To<decimal>(reader["RackId"], 0);
                        rec.Location = CastDbNull.To<string>(reader["Location"], String.Empty).Trim();
                        rec.QtyOnHand = CastDbNull.To<decimal>(reader["QtyOnHand"], 0);
                        rec.RackStatus = CastDbNull.To<string>(reader["RackStatus"], String.Empty).Trim();
                        rec.CreateDate = CastDbNull.To<DateTime>(reader["CreateDate"], DateTime.MinValue);
                        rec.Transferred = CastDbNull.To<bool>(reader["Transferred"], false);
                        rec.TransferredDate = CastDbNull.To<DateTime>(reader["TransferredDate"], DateTime.MinValue);
                        rec.PCName = CastDbNull.To<string>(reader["PCName"], String.Empty).Trim();
                        rec.ItemNum = CastDbNull.To<string>(reader["ItemNum"], String.Empty).Trim();
                        rec.DestinationSerialNumber = CastDbNull.To<string>(reader["DestinationSerialNumber"], String.Empty).Trim();
                        rec.ReasonCode = CastDbNull.To<string>(reader["ReasonCode"], String.Empty).Trim();
                        rec.RackType = CastDbNull.To<string>(reader["RackType"], String.Empty).Trim();
                        rec.E1WorkCenter = CastDbNull.To<string>(reader["E1WorkCenter"], String.Empty).Trim();
                        rec.DefaultSN = CastDbNull.To<bool>(reader["DefaultSN"], false);
                        records.Add(rec);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return records;
        }

        /**********************************************************************
            * Save Cust Pack Records to Shop Floor
            **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(RackTransactionItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    var cmdStatement = $@"UPDATE dbo.E1RackingTransactions SET Transferred = 1, TransferredDate = GETDATE() WHERE E1RackingTransactionId = {record.E1RackingTransactionId};";
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
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
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
                cmd.CommandText = $"UPDATE dbo.E1RackingTransactions SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
                Logger.Trace(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return response;
        }

        /**********************************************************************
        * Save Cust Pack Records to E1
        **********************************************************************/
        public F580030s Transform(RackTransactionItems records)
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
                    f580030.SFEDCT = rec.DocumentType;
                    f580030.SFTYTN = "CGHV";
                    f580030.SFEDFT = "CGHV";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "ShopFloorRC";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.CreateDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.CreateDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLITM = rec.ItemNum;
                    f580030.SFLOTN = rec.DestinationSerialNumber;
                    f580030.SFZ58SN = rec.SerialNumber;
                    f580030.SFUORG = rec.QtyOnHand * 1000;
                    f580030.SFZ58UID = rec.E1RackingTransactionId.ToString();
                    f580030.SFLOCN = rec.Location;
                    f580030.SFZ58RID = rec.RackId;
                    f580030.SFZ58RT = rec.RackType;
                    f580030.SFZ58HST = rec.RackStatus;
                    f580030.SFZ58RCD = rec.ReasonCode;
                    f580030.SFMCU = rec.E1WorkCenter.PadLeft(12);
                    f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
                    f580030s.Add(f580030);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return f580030s;
        }
    }
}

