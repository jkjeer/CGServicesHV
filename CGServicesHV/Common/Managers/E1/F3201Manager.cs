using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.E1
{
    public class F3201Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F3201Manager()
        {
        }

        public F3201Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F3201Manager.GetF3201ByWorkorder", GetF3201ByWorkorder()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/
        public iDB2Command GetF3201ByWorkorder()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        CMCFGID,
                        CMKCOO,
                        CMDOCO,
                        CMDCTO,
                        CMSFXO,
                        CMLNID,
                        CMCFGCID,
                        CMSY,
                        CMAN8,
                        CMSHAN,
                        CMEMCU,
                        CMCO,
                        CMCRRM,
                        CMCRCD,
                        CMCRDC,
                        CMURCD,
                        CMURDT,
                        CMURAT,
                        CMURAB,
                        CMURRF,
                        CMUSER,
                        CMPID,
                        CMJOBN,
                        CMUPMJ,
                        CMTDAY
                    FROM
                        F3201
                    WHERE
                        CMDOCO >= ? AND
                        CMKCOO = '*' AND
                        CMDCTO = 'WO' AND
                        CMPID != 'P580150' AND 
                        CMUPMJ > 115000
                    ORDER BY CMUPMJ, CMTDAY"
            };
            cmd.Parameters.Add(new iDB2Parameter("CMDOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CMDOCO"].Precision = 8;
            cmd.Parameters["CMDOCO"].Scale = 0;
            return cmd;
        }

        public F3201 GetF3201(BusinessSystemSession e1Session, decimal workOrderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var f3201 = new F3201();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("F3201Manager.GetF3201ByWorkorder")) ? e1Session.DbCommands["F3201Manager.GetF3201ByWorkorder"] : GetF3201ByWorkorder();
                cmd.Parameters["CMDOCO"].Value = workOrderNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var i = 0;
                    while (reader.Read())
                    {
                        var f = new F3201
                        {
                            CMCFGID = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0),
                            CMKCOO = Data.CastDbNull.To(reader["CMKCOO"], string.Empty).Trim(),
                            CMDOCO = Data.CastDbNull.To<decimal>(reader["CMDOCO"], 0),
                            CMDCTO = Data.CastDbNull.To(reader["CMDCTO"], string.Empty).Trim(),
                            CMSFXO = Data.CastDbNull.To(reader["CMSFXO"], string.Empty).Trim(),
                            CMLNID = Data.CastDbNull.To<decimal>(reader["CMLNID"], 0),
                            CMCFGCID = Data.CastDbNull.To<decimal>(reader["CMCFGCID"], 0),
                            CMSY = Data.CastDbNull.To(reader["CMSY"], string.Empty).Trim(),
                            CMAN8 = Data.CastDbNull.To<decimal>(reader["CMAN8"], 0),
                            CMSHAN = Data.CastDbNull.To<decimal>(reader["CMSHAN"], 0),
                            CMEMCU = Data.CastDbNull.To(reader["CMEMCU"], string.Empty).Trim(),
                            CMCO = Data.CastDbNull.To(reader["CMCO"], string.Empty).Trim(),
                            CMCRRM = Data.CastDbNull.To(reader["CMCRRM"], string.Empty).Trim(),
                            CMCRCD = Data.CastDbNull.To(reader["CMCRCD"], string.Empty).Trim(),
                            CMCRDC = Data.CastDbNull.To(reader["CMCRDC"], string.Empty).Trim(),
                            CMURCD = Data.CastDbNull.To(reader["CMURCD"], string.Empty).Trim(),
                            CMURDT = Data.CastDbNull.To<decimal>(reader["CMURDT"], 0),
                            CMURAT = Data.CastDbNull.To<decimal>(reader["CMURAT"], 0),
                            CMURAB = Data.CastDbNull.To<decimal>(reader["CMURAB"], 0),
                            CMURRF = Data.CastDbNull.To(reader["CMURRF"], string.Empty).Trim(),
                            CMUSER = Data.CastDbNull.To(reader["CMUSER"], string.Empty).Trim(),
                            CMPID = Data.CastDbNull.To(reader["CMPID"], string.Empty).Trim(),
                            CMJOBN = Data.CastDbNull.To(reader["CMJOBN"], string.Empty).Trim(),
                            CMUPMJ = Data.CastDbNull.To<decimal>(reader["CMUPMJ"], 0),
                            CMTDAY = Data.CastDbNull.To<decimal>(reader["CMTDAY"], 0)
                        };
                        f3201 = f;
                        i++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + i);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return f3201;
        }

        public decimal GetCMCFGID(BusinessSystemSession e1Session, decimal workOrderNum)
        {
            var f3201 = GetF3201(e1Session, workOrderNum);
            return f3201?.CMCFGID ?? 0;
        }
    }
}
