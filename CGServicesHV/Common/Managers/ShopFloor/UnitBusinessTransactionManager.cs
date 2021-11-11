using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.ShopFloor
{
    public class UnitBusinessTransactionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UnitBusinessTransactionManager(BusinessSystemSession e1Session)
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
        * Get Completion Records
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
            cmd.CommandText = String.Format(@"
	        SELECT 
                UnitBusinessTransactionId,                
                TransactionType,
                TransactionId,
                TransactionDate,
                ProdType,
                ProductionDate,
                WOrderNum,
                SerialNum,
                RackId,
                ItemNum,
                LocationCode,
                ScheduleNum,
                BoxNum,
                LineTypeCode,
                ProductionLineCode,
                E1WorkCenterCode,
                LineOperationCode,
                E1OperationCode,
                LineTransactionCode,
                IsFinalStation,
                LineCurrentStatusCode,
                BusinessRackStationCode,
                BusinessRackStatusCode,
                Quantity,
                RejectReason,
                RejectCode,
                Comments,
                TransferStatus,
                TransferBatchNum,
                TransferId,
                TransferDate,
                TransferMessage,
                WrittenDate,
                RoutingCode
            FROM 
                EBS.UnitBusinessTransaction
            WHERE 
                TransferStatus = 'R'
            ORDER BY
                TransactionDate", filterString);
            return cmd;
        }

        public UnitBusinessTransactions Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new UnitBusinessTransactions();
            var cmd = new SqlCommand();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                //// Transfer Status
                //if (!String.IsNullOrEmpty(status))
                //{
                //    filters.Add("Upload = @Upload AND\r\n");
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
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rec = new UnitBusinessTransaction();
                    rec.PlantID = plant.PlantNum;
                    rec.UnitBusinessTransactionId = CastDbNull.To<int>(reader["UnitBusinessTransactionId"], 0);
                    rec.TransactionId = CastDbNull.To<int>(reader["TransactionId"], 0);
                    rec.TransactionType = CastDbNull.To<string>(reader["TransactionType"], String.Empty).Trim();
                    rec.ProdType = CastDbNull.To<string>(reader["ProdType"], String.Empty).Trim();
                    rec.WOrderNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                    rec.LotNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0).ToString();
                    rec.ItemNum = CastDbNull.To<decimal>(reader["ItemNum"], 0);
                    rec.LocationCode = CastDbNull.To<string>(reader["LocationCode"], String.Empty).Trim();
                    rec.ScheduleNum = CastDbNull.To<decimal>(reader["ScheduleNum"], 0);
                    rec.BoxNum = CastDbNull.To<decimal>(reader["BoxNum"], 0);
                    rec.TransactionDate = CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.MinValue);
                    rec.TransactionDateChar = CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.MinValue)
                        .ToString("MM/dd/yyyy hh:mm:ss tt");
                    rec.ProductionDate = CastDbNull.To<DateTime>(reader["ProductionDate"], DateTime.MinValue);
                    rec.SerialNum = CastDbNull.To<string>(reader["SerialNum"], String.Empty).Trim();
                    rec.RackId = CastDbNull.To<decimal>(reader["RackId"], 0);
                    rec.LineTypeCode = CastDbNull.To<string>(reader["LineTypeCode"], String.Empty).Trim();
                    rec.ProductionLineCode = CastDbNull.To<string>(reader["ProductionLineCode"], String.Empty).Trim();
                    rec.LineOperationCode = CastDbNull.To<string>(reader["LineOperationCode"], String.Empty).Trim();
                    rec.LineTransactionCode = CastDbNull.To<string>(reader["LineTransactionCode"], String.Empty).Trim();
                    rec.LineCurrentStatusCode = CastDbNull.To<int>(reader["LineCurrentStatusCode"], 0);
                    rec.IsFinalStation = CastDbNull.To<bool>(reader["IsFinalStation"], false);
                    rec.BusinessRackStatusCode = CastDbNull.To<string>(reader["BusinessRackStatusCode"], String.Empty).Trim();
                    rec.BusinessRackStationCode = CastDbNull.To<string>(reader["BusinessRackStationCode"], String.Empty).Trim();
                    rec.E1WorkcenterCode = CastDbNull.To<string>(reader["E1WorkCenterCode"], String.Empty).Trim();
                    rec.E1OperationCode = CastDbNull.To<string>(reader["E1OperationCode"], String.Empty).Trim();
                    rec.RejectReason = CastDbNull.To<string>(reader["RejectReason"], String.Empty).Trim();
                    rec.RejectCode = CastDbNull.To<string>(reader["RejectCode"], String.Empty).Trim();
                    rec.Quantity = CastDbNull.To<int>(reader["Quantity"], 0);
                    rec.LineTransactionDesc =
                        $"{rec.LineTransactionCode}{(!String.IsNullOrEmpty(rec.RejectReason) ? " [" + rec.RejectReason + "]" : "")}";
                    rec.RoutingCode = CastDbNull.To<string>(reader["RoutingCode"], String.Empty).Trim();
                    rec.ProcessingStatus.TransferStatus = ProcessingStatus.TransferStatusCode.Reserved;
                    records.Add(rec);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return records;
        }

        /**********************************************************************
        * Save Transfer Properties to Shop Floor
        **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(UnitBusinessTransactions items, short sFUpdateBatchSize, SqlConnection cn)
        {
            var responses = new SqlDmlResponses();
            try
            {
                var statements = new List<string>();
                foreach (var record in items)
                {
                    statements.Add(
                        $@"UPDATE EBS.UnitBusinessTransaction SET TransferStatus = '{((char)record.ProcessingStatus.TransferStatus).ToString()}', TransferDate = '{record.ProcessingStatus.TransferredDate}', TransferId = {record.ProcessingStatus.TransferId}, TransferBatchNum = '{record.ProcessingStatus.TransferBatchId}', TransferMessage = '{record.ProcessingStatus.TransferMessage}' WHERE UnitBusinessTransactionId = {record.UnitBusinessTransactionId};");
                }
                responses.AddRange(Sql.SqlBatchExecution(statements, sFUpdateBatchSize, cn));
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
            try
            {
                cmd.CommandText = $"UPDATE EBS.UnitBusinessTransaction SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
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
        * Set Transfer Status
        **********************************************************************/
        public SqlDmlResponse InsertBusinessTransactionsOld(SqlConnection cn)
        {
            var response = new SqlDmlResponse();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            try
            {
                cmd.CommandText = "EXEC [EBS].[usp_Insert_UnitBusinessTransaction_MTO]";
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
                response.Done();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return response;
        }

        public SqlDmlResponse InsertBusinessTransactions(SqlConnection cn)
        {
            var response = new SqlDmlResponse();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.StoredProcedure
            };
            try
            {
                cmd.CommandText = "[EBS].[usp_Insert_UnitBusinessTransaction]";
                SqlParameter parmMessage = cmd.Parameters.Add("@Message", SqlDbType.VarChar, -1);
                parmMessage.Direction = ParameterDirection.Output;
                SqlParameter parmRows = cmd.Parameters.Add("@Rows", SqlDbType.Int);
                parmRows.Direction = ParameterDirection.Output;
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.Message = parmMessage.Value.ToString();
                response.RowsAffected = (int)parmRows.Value;
                response.Done();
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
        public F580030s Transform(List<UnitBusinessTransaction> records)
        {
            var f580030s = new F580030s();
            try
            {
                int edtn = 0;
                foreach (var rec in records)
                {
                    var f580030 = new F580030();
                    edtn++;
                    f580030.SFUKID = rec.ProcessingStatus.TransferId;
                    f580030.SFEDBT = rec.ProcessingStatus.TransferBatchId.ToString(CultureInfo.InvariantCulture);
                    f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580030.SFEDLN = 1000;
                    f580030.SFEDUS = "SOASVC";
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFTYTN = $"LG{rec.ProdType}";
                    f580030.SFEDFT = $"LG{rec.ProdType}";
                    f580030.SFPNID = "LGUnitTran";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFEDCT = rec.TransactionType;
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.ProductionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDOCO = rec.WOrderNum;
                    f580030.SFDCTO = (rec.WOrderNum > 0) ? "WO" : "";
                    f580030.SFZ58SCN = rec.ScheduleNum;
                    f580030.SFMCU = rec.E1WorkcenterCode.PadLeft(12);
                    f580030.SFZ58LWSTN = rec.E1OperationCode;
                    f580030.SFZ58UID = rec.UnitBusinessTransactionId.ToString();
                    f580030.SFZ58HST = rec.BusinessRackStatusCode;
                    f580030.SFZ58SFSTS = rec.BusinessRackStationCode;
                    f580030.SFZ58RCC = rec.RejectReason;
                    f580030.SFZ58RCP = String.IsNullOrEmpty(rec.RejectCode) ? String.Empty : "13";
                    f580030.SFLOCN = rec.LocationCode;
                    f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
                    // conditional MTO/MTS
                    f580030.SFLOTN = (rec.ProdType == "MTO") ? rec.WOrderNum.ToString() : "";
                    f580030.SFZ58SN = rec.SerialNum;
                    f580030.SFZ58RID = rec.RackId;
                    f580030.SFUORG = (rec.TransactionType != "HC") ? rec.Quantity * 1000 : 0;
                    f580030.SFSOCN = (rec.TransactionType == "HC") ? rec.Quantity * 1000 : 0;
                    f580030.SFZ58LDOCO = Utilities.Data.CastStringToDecimal(rec.RoutingCode);
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

