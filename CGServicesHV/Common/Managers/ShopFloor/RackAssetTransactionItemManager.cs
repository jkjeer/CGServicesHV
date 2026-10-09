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
    public class RackAssetTransactionItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackAssetTransactionItemManager(BusinessSystemSession e1Session)
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
        * Get Wo Mgmt Records
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
            cmd.CommandText = @"
	        SELECT 
                RackAssetTransactionId,
                RackId,
                AssetId,
                TransactionDate,
                TransactionType,
                UserId,
                WorkstationId,
                DoNotTransfer,
                TransferId,
                BatchNum,
                TransferStatus,
                TransferDate,
                WrittenDate
	        FROM 
                EBS.RackAssetTransaction
	        WHERE 
                ISNULL(TransferStatus, 'N') = 'N' AND 
                DoNotTransfer = 0";
            return cmd;
        }

        public RackAssetTransactionItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant)
        {
            var records = new RackAssetTransactionItems();
            var cmd = new SqlCommand();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                //// Transfer Status
                //if (!String.IsNullOrEmpty(status))
                //{
                //    filters.Add("Transferred = @Upload AND\r\n");
                //    parameters.Add(new SqlParameter("@Upload", SqlDbType.Bit));
                //    parameters.Last().Value = (status == "Y") ? 1 : 0;
                //}

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
                        var rec = new RackAssetTransactionItem();
                        rec.RackAssetTransactionId = Data.CastDbNull.To<int>(reader["RackAssetTransactionId"], 0);
                        rec.RackId = Data.CastDbNull.To<decimal>(reader["RackId"], 0);
                        rec.AssetId = Data.CastDbNull.To<string>(reader["AssetId"], String.Empty).Trim();
                        rec.TransactionDate = Data.CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.MinValue);
                        rec.TransactionType = Data.CastDbNull.To<string>(reader["TransactionType"], String.Empty).Trim();
                        rec.UserId = Data.CastDbNull.To<string>(reader["UserId"], String.Empty).Trim();
                        rec.WorkstationId = Data.CastDbNull.To<string>(reader["WorkstationId"], String.Empty).Trim();
                        rec.DoNotTransfer = Data.CastDbNull.To<bool>(reader["DoNotTransfer"], false);
                        rec.TransferId = Data.CastDbNull.To<decimal>(reader["TransferId"], 0);
                        rec.BatchNum = Data.CastDbNull.To<string>(reader["BatchNum"], String.Empty).Trim();
                        rec.TransferStatus = Data.CastDbNull.To<string>(reader["TransferStatus"], String.Empty).Trim();
                        rec.TransferDate = Data.CastDbNull.To<DateTime?>(reader["TransferDate"], null);
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
        * Save Records to Shop Floor
        **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(RackAssetTransactionItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    var cmdStatement = $@"UPDATE EBS.RackAssetTransaction SET TransferId = {record.TransferId}, BatchNum = '{record.BatchNum}', TransferStatus = '{record.TransferStatus}', TransferMessage = '{record.TransferMessage}', TransferDate = GETDATE() WHERE RackAssetTransactionId = {record.RackAssetTransactionId};";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sFUpdateBatchSize)
                    {
                        var dmlResponse = new SqlDmlResponse();
                        cmd.CommandText = cmdStringBldr.ToString();
                        dmlResponse.Init(cmd);
                        Logger.Trace(Logging.DbCmdString(cmd));
                        int rows = cmd.ExecuteNonQuery();
                        dmlResponse.RowsAffected = rows;
                        responses.Add(dmlResponse);
                        // reset
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
                cmd.CommandText = $"UPDATE EBS.RackAssetTransaction SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
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
        public F580030s Transform(RackAssetTransactionItems records, Plant plant)
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
                    f580030.SFEDBT = rec.BatchNum;
                    f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580030.SFEDLN = 1000;
                    f580030.SFEDUS = "SOASVC";
                    f580030.SFEDCT = rec.TransactionType;
                    f580030.SFTYTN = "CGHV";
                    f580030.SFEDFT = "CGHV";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "ShopFloorAsset";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = plant.PlantNum.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFZ58AID = rec.AssetId;
                    f580030.SFZ58RID = rec.RackId;
                    f580030.SFZ58UID = rec.RackAssetTransactionId.ToString();
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

