using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
    public class WoMgmtItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public WoMgmtItemManager(BusinessSystemSession e1Session)
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
        public SqlCommand ExtractByWO(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                select wo.WOManagementID
                     , wo.WOStatus
                     , wo.ActionDateTime
                     , wo.WONumber
                     , wo.WODocType
                     , wo.E1BatchID
                     , wo.UserID
                     , wo.Workstation
                     , wo.DoNotTransfer
                     , wo.Transferred
                     , wo.TransferredDate
					 , wo.PlantID
					 , COALESCE(wo.PlantID, (SELECT TOP (1) S7MCU FROM dbo.HVQue hq WHERE hq.WOrderNum = wo.WONumber ORDER BY hq.HVQueID DESC), '32') AS S7MCU
                from dbo.WOManagement             wo
                where wo.WODocType not in ( 'RE', 'BO' ) AND
                    isnull(wo.Transferred,0) = 0 AND 
                    wo.DoNotTransfer = 0
            ";
            return cmd;
        }

        public SqlCommand ExtractBySched(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                select min(wo.WOManagementID) as WOManagementID
                     , wo.WOStatus
                     , wo.ActionDateTime
                     , wo.WODocType                     
                     , wo.E1BatchID
                     , wo.UserID
                     , wo.Workstation
                     , wo.DoNotTransfer
                     , ord.E1Schedule as ScheduleNum
	                 , max(ord.DueDate) as ScheduleDate
                     , wo.Transferred
                     , min(wo.TransferredDate) as TransferredDate
					 , wo.PlantID
					 , COALESCE(wo.PlantID, ord.S7MCU, '32') AS S7MCU
                from dbo.WOManagement             wo
                    left outer join dbo.HVQue ord
                        on ord.WOrderNum = wo.WONumber
                where wo.WODocType in ( 'RE', 'BO' ) AND
                    isnull(wo.Transferred,0) = 0 AND 
                    wo.DoNotTransfer = 0
                group by ord.E1Schedule
                       , wo.WOStatus
                       , wo.ActionDateTime
                       , wo.WODocType 
                       , wo.E1BatchID
                       , wo.UserID
                       , wo.Workstation
                       , wo.DoNotTransfer
                       , wo.Transferred
					    , wo.PlantID
        			    , S7MCU
           ";
            return cmd;
        }

        public WoMgmtItems ExtractByWO(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant)
        {
            var records = new WoMgmtItems();
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
                cmd = ExtractByWO(filters);
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
                        var rec = new WoMgmtItem();
                        rec.PlantID = CastDbNull.To<string>(reader["S7MCU"], plant.PlantNum); 
                        rec.WOManagementID = CastDbNull.To<int>(reader["WOManagementID"], 0);
                        rec.WOStatus = CastDbNull.To<string>(reader["WOStatus"], String.Empty).Trim();
                        rec.ActionDateTime = CastDbNull.To<DateTime>(reader["ActionDateTime"], DateTime.MinValue);
                        rec.WONumber = CastDbNull.To<decimal>(reader["WONumber"], 0);
                        rec.WODocType = CastDbNull.To<string>(reader["WODocType"], String.Empty).Trim();
                        //rec.ScheduleNum = CastDbNull.To<decimal>(reader["ScheduleNum"], 0);
                        //rec.ScheduleDate = CastDbNull.To<DateTime>(reader["ScheduleDate"], DateTime.MinValue);
                        //rec.Process = CastDbNull.To<string>(reader["Process"], String.Empty).Trim();
                        rec.E1BatchID = CastDbNull.To<string>(reader["E1BatchID"], String.Empty).Trim();
                        rec.UserID = CastDbNull.To<string>(reader["UserID"], String.Empty).Trim();
                        rec.Workstation = CastDbNull.To<string>(reader["Workstation"], String.Empty).Trim();
                        rec.DoNotTransfer = CastDbNull.To<bool>(reader["DoNotTransfer"], false);
                        rec.Transferred = CastDbNull.To<bool>(reader["Transferred"], false);
                        rec.TransferredDate = CastDbNull.To<DateTime>(reader["TransferredDate"], DateTime.MinValue);
                        records.Add(rec);
                    }

                    reader.Close();
                    Logger.Info($"Rows: {records.Count}");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return records;
        }

        public WoMgmtItems ExtractBySched(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant)
        {
            var records = new WoMgmtItems();
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
                cmd = ExtractBySched(filters);
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
                        var rec = new WoMgmtItem();
                        rec.PlantID = CastDbNull.To<string>(reader["S7MCU"], plant.PlantNum);
                        rec.WOManagementID = CastDbNull.To<int>(reader["WOManagementID"], 0);
                        rec.WOStatus = CastDbNull.To<string>(reader["WOStatus"], String.Empty).Trim();
                        rec.ActionDateTime = CastDbNull.To<DateTime>(reader["ActionDateTime"], DateTime.MinValue);
                        //rec.WONumber = CastDbNull.To<decimal>(reader["WONumber"], 0);
                        rec.WODocType = CastDbNull.To<string>(reader["WODocType"], String.Empty).Trim();
                        rec.ScheduleNum = CastDbNull.To<decimal>(reader["ScheduleNum"], 0);
                        rec.ScheduleDate = CastDbNull.To<DateTime>(reader["ScheduleDate"], DateTime.MinValue);
                        //rec.Process = CastDbNull.To<string>(reader["Process"], String.Empty).Trim();
                        rec.E1BatchID = CastDbNull.To<string>(reader["E1BatchID"], String.Empty).Trim();
                        rec.UserID = CastDbNull.To<string>(reader["UserID"], String.Empty).Trim();
                        rec.Workstation = CastDbNull.To<string>(reader["Workstation"], String.Empty).Trim();
                        rec.DoNotTransfer = CastDbNull.To<bool>(reader["DoNotTransfer"], false);
                        rec.Transferred = CastDbNull.To<bool>(reader["Transferred"], false);
                        rec.TransferredDate = CastDbNull.To<DateTime>(reader["TransferredDate"], DateTime.MinValue);
                        records.Add(rec);
                    }

                    reader.Close();
                    Logger.Info($"Rows: {records.Count}");
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
        public SqlDmlResponses SaveTransferPropertiesByWO(WoMgmtItems items, short sFUpdateBatchSize, SqlConnection cn)
        {
            var responses = new SqlDmlResponses();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            var cmdStatement = "";
            try
            {
                var rowCount = 0;
                foreach (var record in items)
                {
                    rowCount += 1;
                    cmdStatement = $@"
                                UPDATE dbo.WoManagement 
                                SET Transferred = 1
                                    , TransferredDate = GETDATE() 
                                    , E1BatchID = {record.E1BatchID}
                                WHERE WOManagementID = {record.WOManagementID}
                            ";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sFUpdateBatchSize)
                    {
                        var dmlResponse = new SqlDmlResponse();
                        cmd.CommandText = cmdStringBldr.ToString();
                        dmlResponse.Init(cmd);
                        Logger.Info(Logging.DbCmdString(cmd));
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
                    Logger.Info(Logging.DbCmdString(cmd));
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

        public SqlDmlResponses SaveTransferPropertiesBySched(WoMgmtItems items, short sFUpdateBatchSize, SqlConnection cn)
        {
            var responses = new SqlDmlResponses();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            var cmdStatement = "";
            try
            {
                var rowCount = 0;
                foreach (var record in items)
                {
                    rowCount += 1;
                    cmdStatement = $@"
                        UPDATE wo
                        set Transferred = 1, TransferredDate = GETDATE() 
                        from dbo.WoManagement wo
                        left outer join EBS.FuseOrder ord
                            on ord.WOrderNum = wo.WONumber
                               and ord.IsCurrentVersion = 1
                        where ord.ScheduleNum = {record.ScheduleNum}
                            and isnull(wo.Transferred,0) = 0
                    ";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sFUpdateBatchSize)
                    {
                        var dmlResponse = new SqlDmlResponse();
                        cmd.CommandText = cmdStringBldr.ToString();
                        dmlResponse.Init(cmd);
                        Logger.Info(Logging.DbCmdString(cmd));
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
                    Logger.Info(Logging.DbCmdString(cmd));
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

        public SqlDmlResponses SaveTransferPropertiesBySched(WoMgmtItem item, short sFUpdateBatchSize, SqlConnection cn)
        {
            var responses = new SqlDmlResponses();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            try
            {
                var dmlResponse = new SqlDmlResponse();
                cmd.CommandText = $@"
                    UPDATE wo
                    set Transferred = 1, TransferredDate = GETDATE(), E1BatchID =  {item.E1BatchID}
                    from dbo.WoManagement wo
                    left outer join EBS.FuseOrder ord
                        on ord.WOrderNum = wo.WONumber
                           and ord.IsCurrentVersion = 1
                    where ord.ScheduleNum = {item.ScheduleNum}
                        and isnull(wo.Transferred,0) = 0
                ";
                dmlResponse.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                int rows = cmd.ExecuteNonQuery();
                dmlResponse.RowsAffected = rows;
                responses.Add(dmlResponse);
                Logger.Info($"ROWS: {rows}");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return responses;
        }

        /**********************************************************************
        * Save WO Mgmt Records to E1
        **********************************************************************/
        public F580030s TransformByWO(WoMgmtItems records)
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
                    f580030.SFEDCT = rec.WODocType;
                    f580030.SFTYTN = "WOMGMT";
                    f580030.SFEDFT = "WOMGMT";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "ShopFloorWO";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.ActionDateTime:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.ActionDateTime:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDOCO = rec.WONumber;
                    f580030.SFDCTO = "WO";
                    f580030.SFZ58UID = rec.WOManagementID.ToString();
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

        public F580030 TransformBySched(WoMgmtItem rec)
        {
            var f580030 = new F580030();
            try
            {
                int edtn = 1;
                var docType = "";
                switch (rec.WODocType)
                {
                    case "RE":
                        docType = "BX";
                        break;
                    case "BO":
                        docType = "BD";
                        break;
                    default:
                        docType = rec.WODocType;
                        break;
                }

                f580030.SFUKID = rec.TransferId;
                f580030.SFEDBT = rec.TransferBatchId.ToString();
                f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                f580030.SFEDLN = 1000;
                f580030.SFEDUS = "SVCSOA";
                f580030.SFEDCT = docType;
                f580030.SFTYTN = "WOMGMT";
                f580030.SFEDFT = "WOMGMT";
                f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                f580030.SFDRIN = "1";
                f580030.SFEDDL = 0;
                f580030.SFEDSP = "";
                f580030.SFPNID = "ShopFloorWO";
                f580030.SFTNAC = "02";
                f580030.SFMMCU = rec.PlantID.PadLeft(12);
                f580030.SFACDT = E1TimeManager.CastToE1Date(rec.ScheduleDate);
                f580030.SFSTTM = $"{rec.ActionDateTime:yyyy-MM-dd HH:mm:ss}";
                f580030.SFDTMS = $"{rec.ActionDateTime:yyyy-MM-dd HH:mm:ss}";
                f580030.SFZ58SCN = rec.ScheduleNum;
                // f580030.SFDOCO = 0; // These transactions are processed at the Schedule level in E1 so don't send a WorkOrder
                // f580030.SFDCTO = "";
                f580030.SFUSER = "SVCSOA";
                f580030.SFZ58UID = rec.WOManagementID.ToString();
                f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return f580030;
        }
    }
}

