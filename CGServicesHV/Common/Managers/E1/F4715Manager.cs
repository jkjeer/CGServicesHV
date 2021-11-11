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
    public class F4715Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F4715Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F4715Manager.GetF4715", GetF4715()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/

        public iDB2Command GetF4715()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        ZDKCOO,
                        ZDDOCO,
                        ZDDCTO,
                        ZDLNID,
                        ZDEDTY,
                        ZDEDSQ,
                        ZDLINS,
                        ZDTXLN,
                        ZDUPMJ,
                        ZDTDAY
                    FROM
                        F4715                 
                    WHERE
                        ZDDOCO > 0 AND
                        ZDKCOO = ? AND
                        ZDDOCO = ? AND
                        ZDDCTO = ? AND
                        ZDLNID = ?
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("KCOO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("LNID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LNID"].Precision = 6;
            cmd.Parameters["LNID"].Scale = 0;
            return cmd;
        }

        public F4715s GetF4715s(BusinessSystemSession e1Session, string kcoo, decimal doco, string dcto, decimal lnid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var f4715S = new F4715s();
            try
            {
                var cmd = e1Session.DbCommands["F4715Manager.GetF4715"];
                if (doco > 0)
                {
                    cmd.Parameters["KCOO"].Value = kcoo;
                    cmd.Parameters["DOCO"].Value = doco;
                    cmd.Parameters["DCTO"].Value = dcto;
                    cmd.Parameters["LNID"].Value = lnid;
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            var f4715 = new F4715
                            {
                                ZDEDTY = CastDbNull.To(reader["ZDEDTY"], string.Empty).Trim(),
                                ZDEDSQ = CastDbNull.To<decimal>(reader["ZDEDSQ"], 0),
                                ZDKCOO = CastDbNull.To(reader["ZDKCOO"], string.Empty).Trim(),
                                ZDDOCO = CastDbNull.To<decimal>(reader["ZDDOCO"], 0),
                                ZDDCTO = CastDbNull.To(reader["ZDDCTO"], string.Empty).Trim(),
                                ZDLNID = CastDbNull.To<decimal>(reader["ZDLNID"], 0),
                                ZDLINS = CastDbNull.To<decimal>(reader["ZDLINS"], 0),
                                ZDTXLN = CastDbNull.To(reader["ZDTXLN"], string.Empty).Trim(),
                                ZDUPMJ = CastDbNull.To<decimal>(reader["ZDUPMJ"], 0),
                                ZDTDAY = CastDbNull.To<decimal>(reader["ZDTDAY"], 0)
                            };
                            f4715S.Add(f4715);
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return f4715S;
        }

        public static F4715Pivots PivotF4715s(F4715s f4715S)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var f4715Pivots = new F4715Pivots();
            try
            {
                foreach (var f4715 in f4715S)
                {
                    if (!f4715Pivots.Exists(o => o.ZDKCOO == f4715.ZDKCOO && o.ZDDOCO == f4715.ZDDOCO && o.ZDDCTO == f4715.ZDDCTO && o.ZDLNID == f4715.ZDLNID))
                    {
                        var tmpF4715Pivot = new F4715Pivot
                        {
                            ZDKCOO = f4715.ZDKCOO,
                            ZDDOCO = f4715.ZDDOCO,
                            ZDDCTO = f4715.ZDDCTO,
                            ZDLNID = f4715.ZDLNID
                        };
                        f4715Pivots.Add(tmpF4715Pivot);
                    }
                    var f4715PivotOld = f4715Pivots.Find(o => o.ZDKCOO == f4715.ZDKCOO && o.ZDDOCO == f4715.ZDDOCO && o.ZDDCTO == f4715.ZDDCTO && o.ZDLNID == f4715.ZDLNID);
                    if (f4715PivotOld != null)
                    {
                        if (f4715.ZDEDTY.Equals("L"))
                        {
                            switch (Convert.ToInt16(f4715.ZDEDSQ))
                            {
                                case 1:
                                    f4715PivotOld.L1 = f4715.ZDTXLN;
                                    break;
                                case 2:
                                    f4715PivotOld.L2 = f4715.ZDTXLN;
                                    break;
                                case 3:
                                    f4715PivotOld.L3 = f4715.ZDTXLN;
                                    break;
                                case 4:
                                    f4715PivotOld.L4 = f4715.ZDTXLN;
                                    break;
                                case 5:
                                    f4715PivotOld.L5 = f4715.ZDTXLN;
                                    break;
                                case 6:
                                    f4715PivotOld.L6 = f4715.ZDTXLN;
                                    break;
                                case 7:
                                    f4715PivotOld.L7 = f4715.ZDTXLN;
                                    break;
                                case 8:
                                    f4715PivotOld.L8 = f4715.ZDTXLN;
                                    break;
                                case 9:
                                    f4715PivotOld.L9 = f4715.ZDTXLN;
                                    break;
                                case 10:
                                    f4715PivotOld.L10 = f4715.ZDTXLN;
                                    break;
                                case 11:
                                    f4715PivotOld.L11 = f4715.ZDTXLN;
                                    break;
                                case 12:
                                    f4715PivotOld.L12 = f4715.ZDTXLN;
                                    break;
                                case 13:
                                    f4715PivotOld.L13 = f4715.ZDTXLN;
                                    break;
                                case 14:
                                    f4715PivotOld.L14 = f4715.ZDTXLN;
                                    break;
                                case 15:
                                    f4715PivotOld.L15 = f4715.ZDTXLN;
                                    break;
                                case 16:
                                    f4715PivotOld.L16 = f4715.ZDTXLN;
                                    break;
                                case 17:
                                    f4715PivotOld.L17 = f4715.ZDTXLN;
                                    break;
                                case 18:
                                    f4715PivotOld.L18 = f4715.ZDTXLN;
                                    break;
                                case 19:
                                    f4715PivotOld.L19 = f4715.ZDTXLN;
                                    break;
                                case 20:
                                    f4715PivotOld.L20 = f4715.ZDTXLN;
                                    break;
                                case 21:
                                    f4715PivotOld.L21 = f4715.ZDTXLN;
                                    break;
                                case 22:
                                    f4715PivotOld.L22 = f4715.ZDTXLN;
                                    break;
                                case 23:
                                    f4715PivotOld.L23 = f4715.ZDTXLN;
                                    break;
                                case 24:
                                    f4715PivotOld.L24 = f4715.ZDTXLN;
                                    break;
                                case 25:
                                    f4715PivotOld.L25 = f4715.ZDTXLN;
                                    break;
                                case 26:
                                    f4715PivotOld.L26 = f4715.ZDTXLN;
                                    break;
                                case 27:
                                    f4715PivotOld.L27 = f4715.ZDTXLN;
                                    break;
                                case 28:
                                    f4715PivotOld.L28 = f4715.ZDTXLN;
                                    break;
                                case 29:
                                    f4715PivotOld.L29 = f4715.ZDTXLN;
                                    break;
                                case 30:
                                    f4715PivotOld.L30 = f4715.ZDTXLN;
                                    break;
                            }
                        }
                        else if (f4715.ZDEDTY.Equals("E"))
                        {
                            switch (Convert.ToInt16(f4715.ZDEDSQ))
                            {
                                case 1:
                                    f4715PivotOld.E1 = f4715.ZDTXLN;
                                    break;
                                case 2:
                                    f4715PivotOld.E2 = f4715.ZDTXLN;
                                    break;
                                case 3:
                                    f4715PivotOld.E3 = f4715.ZDTXLN;
                                    break;
                                case 4:
                                    f4715PivotOld.E4 = f4715.ZDTXLN;
                                    break;
                                case 5:
                                    f4715PivotOld.E5 = f4715.ZDTXLN;
                                    break;
                                case 6:
                                    f4715PivotOld.E6 = f4715.ZDTXLN;
                                    break;
                                case 7:
                                    f4715PivotOld.E7 = f4715.ZDTXLN;
                                    break;
                                case 8:
                                    f4715PivotOld.E8 = f4715.ZDTXLN;
                                    break;
                                case 9:
                                    f4715PivotOld.E9 = f4715.ZDTXLN;
                                    break;
                                case 10:
                                    f4715PivotOld.E10 = f4715.ZDTXLN;
                                    break;
                                case 11:
                                    f4715PivotOld.E11 = f4715.ZDTXLN;
                                    break;
                                case 12:
                                    f4715PivotOld.E12 = f4715.ZDTXLN;
                                    break;
                                case 13:
                                    f4715PivotOld.E13 = f4715.ZDTXLN;
                                    break;
                                case 14:
                                    f4715PivotOld.E14 = f4715.ZDTXLN;
                                    break;
                                case 15:
                                    f4715PivotOld.E15 = f4715.ZDTXLN;
                                    break;
                                case 16:
                                    f4715PivotOld.E16 = f4715.ZDTXLN;
                                    break;
                                case 17:
                                    f4715PivotOld.E17 = f4715.ZDTXLN;
                                    break;
                                case 18:
                                    f4715PivotOld.E18 = f4715.ZDTXLN;
                                    break;
                                case 19:
                                    f4715PivotOld.E19 = f4715.ZDTXLN;
                                    break;
                                case 20:
                                    f4715PivotOld.E20 = f4715.ZDTXLN;
                                    break;
                                case 21:
                                    f4715PivotOld.E21 = f4715.ZDTXLN;
                                    break;
                                case 22:
                                    f4715PivotOld.E22 = f4715.ZDTXLN;
                                    break;
                                case 23:
                                    f4715PivotOld.E23 = f4715.ZDTXLN;
                                    break;
                                case 24:
                                    f4715PivotOld.E24 = f4715.ZDTXLN;
                                    break;
                                case 25:
                                    f4715PivotOld.E25 = f4715.ZDTXLN;
                                    break;
                                case 26:
                                    f4715PivotOld.E26 = f4715.ZDTXLN;
                                    break;
                                case 27:
                                    f4715PivotOld.E27 = f4715.ZDTXLN;
                                    break;
                                case 28:
                                    f4715PivotOld.E28 = f4715.ZDTXLN;
                                    break;
                                case 29:
                                    f4715PivotOld.E29 = f4715.ZDTXLN;
                                    break;
                                case 30:
                                    f4715PivotOld.E30 = f4715.ZDTXLN;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return f4715Pivots;
        }
    }
}