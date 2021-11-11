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
    public class CustPackItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CustPackItemManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"AddressBookDownloadManager.GetRelatedOrders", GetRelatedOrders()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
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
            cmd.CommandText = String.Format(@"
	        SELECT 
                ProdCustPackID,
                SerialNumber,
                Position,
                LiteSerialNumber, 
                ScheduleNumber,
                WorkOrderNum,
                WarehouseLocation,
                LineID,
                ProdType,
                Quantity,
                RackType,
                BoxNum,
                BoxSlot,
                BoxRackSlot,
                Workcenter,
                IsBackout,
                CreatedTS
	        FROM 
                ClearPath.ProdCustPack
	        WHERE 
                {0} 1=1", filterString.ToString());
            return cmd;
        }

        public CustPackItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new CustPackItems();
            var cmd = new SqlCommand();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                // Transfer Status
                //if (!String.IsNullOrEmpty(status))
                //{
                    filters.Add("TransferStatus = @TransferStatus AND\r\n");
                    parameters.Add(new SqlParameter("@TransferStatus", SqlDbType.VarChar, 10));
                    parameters.Last().Value = "Q";
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
                        var rec = new CustPackItem();
                        rec.PlantNum = plant.PlantNum;
                        rec.CustPackId = CastDbNull.To<int>(reader["ProdCustPackID"], 0);
                        rec.WorkOrderNum = CastDbNull.To<int>(reader["WorkOrderNum"], 0);
                        rec.ScheduleNum = CastDbNull.To<int>(reader["ScheduleNumber"], 0);
                        rec.SerialNum = CastDbNull.To<String>(reader["SerialNumber"], String.Empty).Trim();
                        rec.Location = CastDbNull.To<String>(reader["WarehouseLocation"], String.Empty).Trim();
                        rec.ProductionLineCode = CastDbNull.To<String>(reader["LineID"], String.Empty).Trim();
                        rec.ProductionType = CastDbNull.To<String>(reader["ProdType"], String.Empty).Trim();
                        rec.RackType = CastDbNull.To<String>(reader["RackType"], String.Empty).Trim();
                        rec.BoxSlot = CastDbNull.To<String>(reader["BoxRackSlot"], String.Empty).Trim();
                        rec.WorkCenterCode = CastDbNull.To<String>(reader["Workcenter"], String.Empty).Trim();
                        rec.Quantity = CastDbNull.To<int>(reader["Quantity"], 0);
                        rec.TransactionDate = CastDbNull.To<DateTime>(reader["CreatedTS"], DateTime.MinValue);
                        rec.Backout = CastDbNull.To<bool>(reader["IsBackout"], false);
                        records.Add(rec);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new FaultException($@"{cnShopFloor.DataSource}.{cnShopFloor.Database} {e.Message}");
            }
            return records;
        }

        /**********************************************************************
        * Save Cust Pack Records to Shop Floor
        **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(List<CustPackItem> items, short sFUpdateBatchSize, SqlConnection cn)
        {
            var statements = new List<string>();
            var responses = new SqlDmlResponses();
            try
            {
                foreach (var record in items)
                {
                    string statement = "";
                    if (record.TransferStatus == "E")
                    {
                        statement = $@"
                     UPDATE ClearPath.ProdCustPack
                    SET TransferStatus = '{record.TransferStatus}', 
                        TransferComment = '{record.TransferComment}'
                    WHERE ProdCustPackId = { record.CustPackId }; ";
                    }
                    else
                    {
                        statement = $@"
                    UPDATE ClearPath.ProdCustPack 
                    SET TransferStatus = 'Y', 
                        TransferId = '{record.TransferId}', 
                        TransferDate = GETDATE(), 
                        TransferComment = '{record.TransferComment}' 
                    WHERE ProdCustPackId = {record.CustPackId};";
                    }
                    statements.Add(statement);
                }
                responses = Sql.SqlBatchExecution(statements, sFUpdateBatchSize, cn);
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
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            var response = new SqlDmlResponse(cmd);
            var cmdStringBldr = new StringBuilder();
            try
            {
                cmd.CommandText =
                    $"UPDATE ClearPath.ProdCustPack SET TransferStatus = '{postStatus}' WHERE ISNULL(TransferStatus, 'N') = '{preStatus}'";
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                response.Done();
            }
            return response;
        }

        /**********************************************************************
        * Save Cust Pack Records to E1
        **********************************************************************/
        public F580030s Transform(CustPackItems records)
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
                    f580030.SFEDCT = "HR";
                    f580030.SFTYTN = "CGMTO";
                    f580030.SFEDFT = "CGHVR";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "HVReceipts";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantNum.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLOTN = rec.SerialNum;
                    f580030.SFDOCO = rec.WorkOrderNum;
                    f580030.SFDCTO = "WO";
                    f580030.SFMCU = rec.WorkCenterCode.PadLeft(12);
                    f580030.SFZ58SCN = rec.ScheduleNum;
                    //f580030.SFZ58SCSN = rec.FirmSchedule;
                    //f580030.SFSFT = rec.Shift;
                    f580030.SFLOCN = rec.Location;
                    f580030.SFUORG = (rec.Backout) ? rec.Quantity*-1000 : rec.Quantity*1000;
                    f580030.SFZ58IGLN = rec.LineID.ToString().PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58UID = rec.CustPackId.ToString();
                    //f580030.SFZ58BSF = rec.SqftBillable * 100;
                    //f580030.SFZ5850000 = rec.SqftActual * 100;
                    //f580030.SFFLAG = (rec.WIP) ? "1" : "";
                    //f580030.SFZ58PCR = rec.Crew.PadLeft(2).Substring(0, 2).Trim();
                    //f580030.SFZ58LWSTN = rec.E1Operation;
                    //f580030.SFZ58LNID = rec.HVTransactionHistoryID;
                    f580030.SFZ58RT = rec.RackType;
                    //f580030.SFZ58HST = rec.RackStatus;
                    //f580030.SFZ58RCD = rec.RackReasonStatus;
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

