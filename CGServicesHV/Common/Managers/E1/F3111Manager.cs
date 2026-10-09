using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using NLog;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.E1
{
    public class F3111Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F3111Manager()
        {
        }

        public F3111Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F3111Manager.GetF3111", GetF3111()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
    * Prepared Commands
    ********************************************************************************/
        public iDB2Command GetF3111()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        WMDOCO,
                        WMFORQ,
                        WMCPIT,
                        WMCPIL,
                        WMCPIA,
                        WMCMCU,
                        WMDSC1,
                        WMDSC2,
                        WMUORG
                    FROM
                        F3111
                    WHERE
                        WMCMCU = ? AND
                        WMDOCO = ? 
                        ORDER BY WMFORQ DESC, WMOPSQ ASC
                        FETCH FIRST 1 ROWS ONLY
                    "
            };
            cmd.Parameters.Add(new iDB2Parameter("WMCMCU", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("WMDOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["WMDOCO"].Precision = 8;
            cmd.Parameters["WMDOCO"].Scale = 0;
            return cmd;
        }

        public string GetRawItem(BusinessSystemSession e1Session, string plantNum, decimal workOrderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var rawItem = "";
            try
            {
                var cmd = e1Session.DbCommands["F3111Manager.GetF3111"];
                cmd.Parameters["WMCMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["WMDOCO"].Value = workOrderNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var i = 0;
                    while (reader.Read())
                    {
                        //rawItem = Data.CastDbNull.To<decimal>(reader["WMCPIT"], 0);
                        rawItem = Data.CastDbNull.To(reader["WMCPIL"], string.Empty).Trim();
                        i++;
                        if (i > 1)
                        {
                            throw new ApplicationException($"Multiple F3111s found for Work Order {workOrderNum}");
                        }
                    }

                    reader.Close();
                    Logger.Trace($"ROWS: {i} WMCPIL={rawItem}");
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return rawItem;
        }

        public F3111 GetF3111(BusinessSystemSession e1Session, string plantNum, decimal workOrderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var f3111 = new F3111();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("F3111Manager.GetF3111")) ? e1Session.DbCommands["F3111Manager.GetF3111"] : GetF3111();
                cmd.Parameters["WMCMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["WMDOCO"].Value = workOrderNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var i = 0;
                    while (reader.Read())
                    {
                        var f = new F3111
                        {
                            WMDOCO = Data.CastDbNull.To<decimal>(reader["WMDOCO"], 0),
                            WMFORQ = Data.CastDbNull.To(reader["WMFORQ"], string.Empty).Trim(),
                            WMCPIT = Data.CastDbNull.To<decimal>(reader["WMCPIT"], 0),
                            WMCPIL = Data.CastDbNull.To(reader["WMCMCU"], string.Empty).Trim(),
                            WMCPIA = Data.CastDbNull.To(reader["WMCMCU"], string.Empty).Trim(),
                            WMCMCU = Data.CastDbNull.To(reader["WMCMCU"], string.Empty).Trim(),
                            WMDSC1 = Data.CastDbNull.To(reader["WMDSC1"], string.Empty).Trim(),
                            WMDSC2 = Data.CastDbNull.To(reader["WMDSC2"], string.Empty).Trim(),
                            WMUORG = Data.CastDbNull.To<decimal>(reader["WMUORG"], 0)
                        };
                        f3111 = f;
                        i++;
                        if (i > 1)
                        {
                            throw new ApplicationException($"Multiple F3111s found for Work Order {workOrderNum}");
                        }
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + i);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return f3111;
        }
    }
}
