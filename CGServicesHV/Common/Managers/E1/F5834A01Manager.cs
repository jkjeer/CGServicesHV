using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.E1
{
    public class F5834A01Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F5834A01Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F5834A01Manager.GetSchedule", GetSchedule()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Extract
         ********************************************************************************/
        public iDB2Command GetSchedule()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT
                    S1MCU,
                    S1Z58SCN,
                    S1Z58SCSTS,
                    S1Z58SCD,
                    S1Z58SCTYP,
                    S1Z58SFS,
                    S1Z58SCDDJ,
                    S1UPMJ,
                    S1TDAY
                FROM
                    F5834A01                 
                WHERE
                    S1MCU = ? AND
                    S1Z58SCN = ? AND
                    S1Z58SCD = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN"].Precision = 5;
            cmd.Parameters["SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCD"].Precision = 6;
            cmd.Parameters["SCD"].Scale = 0;
            return cmd;
        }

        public F5834A01 GetSchedule(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1)
        {
            var f5834A01 = new F5834A01();
            try
            {
                var cmd = e1Session.DbCommands["F5834A01Manager.GetSchedule"];
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["SCN"].Value = scheduleNum;
                cmd.Parameters["SCD"].Value = scheduleDateE1;
                cmd.Connection = e1Session.DbConnection;
                cmd.CommandTimeout = 0;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        f5834A01.S1MCU = CastDbNull.To(reader["S1MCU"], string.Empty).Trim();
                        f5834A01.S1Z58SCN = CastDbNull.To<decimal>(reader["S1Z58SCN"], 0);
                        f5834A01.S1Z58SCSTS = CastDbNull.To(reader["S1Z58SCSTS"], string.Empty).Trim();
                        f5834A01.S1Z58SCD = CastDbNull.To<decimal>(reader["S1Z58SCD"], 0);
                        f5834A01.S1Z58SCTYP = CastDbNull.To(reader["S1Z58SCTYP"], string.Empty).Trim();
                        f5834A01.S1Z58SFS = CastDbNull.To(reader["S1Z58SFS"], string.Empty).Trim();
                        f5834A01.S1Z58SCDDJ = CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0);
                        f5834A01.S1UPMJ = CastDbNull.To<decimal>(reader["S1UPMJ"], 0);
                        f5834A01.S1TDAY = CastDbNull.To<decimal>(reader["S1TDAY"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return f5834A01;
        }
    }
}
