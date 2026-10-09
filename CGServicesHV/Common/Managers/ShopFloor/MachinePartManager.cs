using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
    public class MachinePartManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MachinePartManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"MachinePartManager.GetItemLocation", GetItemLocation()},
                {"MachinePartManager.GetItemProperties", GetItemProperties()}
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
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
	        SELECT 
                MachinePartConsumedId,
                ItemNum,
                Quantity,
                UnitMeasure,
                Location,
                TransactionId,
                TransactionDate,
                WrittenDate,
                WrittenBy,
                TransferStatus,
                TransferDate,
                TransferMessage,
                TransferBatchNum,
                TransferId
	        FROM 
                EBS.MachinePartConsumed
	        WHERE 
                {filterString} 1=1";
            return cmd;
        }

        public MachinePartConsumedItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var items = new MachinePartConsumedItems();
            var filters = new List<string>();
            var parameters = new List<SqlParameter>();
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
                var cmd = Extract(filters);
                foreach (var parameter in parameters)
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
                        var rec = new MachinePartConsumedItem();
                        rec.PlantNum = plant.PlantNum;
                        rec.ConsumedId = Data.CastDbNull.To<long>(reader["MachinePartConsumedId"], 0);
                        rec.ItemNum = Data.CastDbNull.To<decimal>(reader["ItemNum"], 0);
                        rec.Quantity = Data.CastDbNull.To(reader["Quantity"], 0);
                        rec.UnitMeasure = Data.CastDbNull.To(reader["UnitMeasure"], string.Empty).Trim();
                        rec.Location = Data.CastDbNull.To(reader["Location"], string.Empty);
                        rec.TransactionId = Data.CastDbNull.To<long>(reader["TransactionId"], 0);
                        rec.TransactionDate = Data.CastDbNull.To(reader["TransactionDate"], DateTime.MinValue);
                        rec.WrittenDate = Data.CastDbNull.To(reader["WrittenDate"], DateTime.MinValue);
                        //rec.E1Location = GetItemLocation(session, rec.PlantNum, rec.Location);
                        items.Add(rec);
                    }

                    reader.Close();
                    GetItemLocation(session, items);
                    GetItemProperties(session, items);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return items;
        }

        /**********************************************************************
        * Item Location
        **********************************************************************/
        public iDB2Command GetItemLocation()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	                LMLOCN        
                FROM 
	                F4100
                WHERE 
                    LMMCU = ? AND
                    LMLOCN LIKE ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("LOCN", iDB2DbType.iDB2Graphic, 20));
            return cmd;
        }

        public void GetItemLocation(BusinessSystemSession session, MachinePartConsumedItems items)
        {
            try
            {
                var cmd = session.DbCommands["MachinePartManager.GetItemLocation"];
                cmd.Connection = session.DbConnection;
                foreach (var item in items)
                {
                    cmd.Parameters[0].Value = item.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = "%" + item.Location + "%";
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            item.E1Location = Data.CastDbNull.To(reader["LMLOCN"], string.Empty);
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        /**********************************************************************
        * Item Location
        **********************************************************************/
        public iDB2Command GetItemProperties()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	                IMDSC1        
                FROM 
	                F4101
                WHERE 
                    IMITM = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public string GetItemProperties(BusinessSystemSession session, MachinePartConsumedItems items)
        {
            var ret = "";
            try
            {
                var cmd = session.DbCommands["MachinePartManager.GetItemProperties"];
                cmd.Connection = session.DbConnection;
                foreach (var item in items)
                {
                    cmd.Parameters[0].Value = item.ItemNum;
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            item.ItemDesc = Data.CastDbNull.To(reader["IMDSC1"], string.Empty);
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return ret;
        }

        /**********************************************************************
        * Save Cust Pack Records to Shop Floor
        **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(List<MachinePartConsumedItem> items, short sFUpdateBatchSize, SqlConnection cn)
        {
            var statements = new List<string>();
            SqlDmlResponses responses;
            try
            {
                foreach (var record in items)
                {
                       var statement = $@"
                    UPDATE EBS.MachinePartConsumed 
                    SET TransferStatus = 'Y', 
                        TransferBatchNum = '{record.TransferBatchNum}', 
                        TransferId = '{record.TransferId}', 
                        TransferDate = GETDATE(), 
                        TransferMessage = '{record.TransferMessage}' 
                    WHERE MachinePartConsumedId = {record.ConsumedId};";               
                    statements.Add(statement);
                }
                responses = Sql.SqlBatchExecution(statements, sFUpdateBatchSize, cn);
            }
            catch (Exception e)
            {
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
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
                    $"UPDATE EBS.MachinePartConsumed SET TransferStatus = '{postStatus}' WHERE ISNULL(TransferStatus, 'N') = '{preStatus}'";
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
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
        public F580030s Transform(List<MachinePartConsumedItem> records)
        {
            var f580030s = new F580030s();
            try
            {
                var edtn = 0;
                foreach (var rec in records)
                {
                    var f580030 = new F580030();
                    edtn++;
                    f580030.SFUKID = rec.TransferId;
                    f580030.SFEDBT = rec.TransferBatchNum.ToString();
                    f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580030.SFEDLN = 1000;
                    f580030.SFEDUS = "SVCSOA";
                    f580030.SFEDCT = "HM";
                    f580030.SFTYTN = "CGMTO";
                    f580030.SFEDFT = "CGHM";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "CGPart";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantNum.PadLeft(12);
                    f580030.SFLITM = rec.ItemNum.ToString(CultureInfo.InvariantCulture);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.WrittenDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLOCN = rec.Location;
                    f580030.SFUORG = rec.Quantity * 1000;
                    f580030.SFZ58UID = rec.ConsumedId.ToString();
                    f580030.SFJFLG = rec.UnitMeasure;
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

