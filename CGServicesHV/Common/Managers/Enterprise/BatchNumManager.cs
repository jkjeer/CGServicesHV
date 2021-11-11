using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class BatchNumManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BatchNumManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"BatchNumManager.SetNextBatch", SetNextBatch()},
                {"BatchNumManager.GetBatch", GetBatch()},
                {"BatchNumManager.SetNextBatchAR", SetNextBatchAR()}
            };
            foreach (var keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /******************************************************************
        * Batch Num
        ******************************************************************/
        public iDB2Command SetNextBatch(string numberField)
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format(
                    @"UPDATE
                        {0}.F0002
                    SET
                        {1} = {1} + 1
                    WHERE
                        NNSY = ?", ConfigurationManager.AppSettings["ControlLib"], numberField);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("ProductCode", iDB2DbType.iDB2Graphic, 4));
            return cmd;
        }

        public iDB2Command GetBatch()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                    SELECT
                        NNSY,
                        NNN001,
                        NNN002,
                        NNN003,
                        NNN004,
                        NNN005,
                        NNN006,
                        NNN007,
                        NNN008,
                        NNN009,
                        NNN010
                    FROM
                        {ConfigurationManager.AppSettings["ControlLib"]}.F0002
                    WHERE
                        NNSY = ?";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("ProductCode", iDB2DbType.iDB2Graphic, 4));
            return cmd;
        }

        public decimal GetNextBatch(BusinessSystemSession e1Session, string productCode, string numberField)
        {
            decimal batchNum = 0;
            var cn = e1Session.DbConnection;
            var txn = cn.BeginTransaction();
            try
            {
                var cmd1 = SetNextBatch(numberField);
                cmd1.Parameters[0].Value = productCode;
                cmd1.Connection = cn;
                cmd1.Transaction = txn;
                var rows = cmd1.ExecuteNonQuery();

                var cmd2 = GetBatch();
                cmd2.Parameters[0].Value = productCode;
                cmd2.Connection = cn;
                cmd2.Transaction = txn;
                using (IDataReader reader = cmd2.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        batchNum = CastDbNull.To<decimal>(reader[numberField], 0);
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }

                txn.Commit();
            }
            catch (Exception e)
            {
                txn.Rollback();
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return batchNum;
        }

        /******************************************************************
        * Batch Num A/R
        ******************************************************************/
        public iDB2Command SetNextBatchAR()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                    UPDATE
                        {ConfigurationManager.AppSettings["ControlLib"]}.F0002
                    SET
                        NNN001 = NNN001 + 1
                    WHERE
                        NNSY = '58'";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public iDB2Command GetBatchAR()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                    SELECT
                        NNSY,
                        NNN001,
                        NNN002,
                        NNN003,
                        NNN004,
                        NNN005,
                        NNN006,
                        NNN007,
                        NNN008,
                        NNN009,
                        NNN010
                    FROM
                        {ConfigurationManager.AppSettings["ControlLib"]}.F0002
                    WHERE
                        NNSY = '58'";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public decimal GetNextBatchAR(BusinessSystemSession e1Session)
        {
            decimal batchNum = 0;
            var cn = e1Session.DbConnection;
            var txn = cn.BeginTransaction();
            try
            {
                var cmd1 = (e1Session.DbCommands.ContainsKey("BatchNumManager.SetNextBatchAR")) ? e1Session.DbCommands["BatchNumManager.SetNextBatchAR"] : SetNextBatchAR();
                //var cmd1 = SetNextBatchAR();
                cmd1.Connection = cn;
                cmd1.Transaction = txn;
                var rows = cmd1.ExecuteNonQuery();

                var cmd2 = (e1Session.DbCommands.ContainsKey("BatchNumManager.GetBatchAR")) ? e1Session.DbCommands["BatchNumManager.GetBatchAR"] : GetBatchAR();
                //var cmd2 = GetBatchAR();
                cmd2.Connection = cn;
                cmd2.Transaction = txn;
                using (IDataReader reader = cmd2.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        batchNum = CastDbNull.To<decimal>(reader["NNN001"], 0);
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }

                txn.Commit();
            }
            catch (Exception e)
            {
                txn.Rollback();
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return batchNum;
        }

        private static SqlCommand GetNextQAUKID()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT NEXT VALUE FOR [E1].[QAUKID] AS 'QAUKID'";
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return cmd;
        }

        private static SqlCommand GetNextBIEDBT()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT NEXT VALUE FOR [E1].[BIEDBT] AS 'BIEDBT'";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public static string GetNextBIEDBT(SqlConnection cnBIEDBT)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var sBatchNum = string.Empty;
            try
            {
                var cmd = GetNextBIEDBT();
                cmd.Connection = cnBIEDBT;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var rowCount = 0;
                    while (reader.Read())
                    {
                        var batchNum = CastDbNull.To<long>(reader["BIEDBT"], 0);
                        sBatchNum = Convert.ToString(batchNum);
                        rowCount++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rowCount);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnBIEDBT.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnBIEDBT.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return sBatchNum;
        }

        public static decimal GetNextQAUKID(SqlConnection cnQAUKID)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            long uniqueId = 0;
            decimal dUniqueId = 0;
            try
            {
                var cmd = GetNextQAUKID();
                cmd.Connection = cnQAUKID;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var rowCount = 0;
                    while (reader.Read())
                    {
                        uniqueId = CastDbNull.To<long>(reader["QAUKID"], 0);
                        rowCount++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rowCount);
                    dUniqueId = Convert.ToDecimal(uniqueId);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnQAUKID.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnQAUKID.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return dUniqueId;
        }

        public static decimal GetFirstBIUKIDRange(SqlConnection cnBIUKID, int numberOfBIUKIDS)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            decimal dFirstBIUKID = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnBIUKID,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "sys.sp_sequence_get_range"
                };
                cmd.Parameters.AddWithValue("@sequence_name", "E1.BIUKID");
                cmd.Parameters.AddWithValue("@range_size", numberOfBIUKIDS);
                var firstValueInRange = new SqlParameter("@range_first_value", SqlDbType.Variant)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(firstValueInRange);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
                var firstBIUKID = CastDbNull.To<long>(firstValueInRange.Value, 0);
                dFirstBIUKID = Convert.ToDecimal(firstBIUKID);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnBIUKID.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnBIUKID.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return dFirstBIUKID;
        }
    }
}
