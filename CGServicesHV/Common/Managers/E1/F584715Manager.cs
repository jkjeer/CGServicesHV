using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.E1
{
    public class F584715Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static iDB2Command GetF584715SDynamic(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
                        ZDEDBT,
                        ZDEDLN,
                        ZDMCU,
                        ZDZ58SCN,
                        ZDZ58SCSN,
                        ZDEDTY,
                        ZDKCOO,
                        ZDDOCO,
                        ZDDCTO,
                        ZDLNID,
                        ZDZ58TX01,
                        ZDZ58TX02,
                        ZDZ58TX03,
                        ZDZ58TX04,
                        ZDZ58TX05,
                        ZDUSER,
                        ZDPID,
                        ZDJOBN,
                        ZDUPMJ,
                        ZDTDAY
                    FROM
                        F584715
                    {filterString}
                    FOR READ ONLY"
            };
            return cmd;
        }

        public static F584715s GetF584715Ls(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var f584715Ls = new F584715s();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Cust Msg
                filters.Add("ZDEDTY = 'L' AND \r\n");
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("ZDMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }
                // MTOSchedule Num
                if (scheduleNum > 0)
                {
                    filters.Add("ZDZ58SCN = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 5;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleNum;
                }
                // Batch Num
                if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                {
                    filters.Add("ZDEDBT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                    parameters.Last().Value = batchNum;
                }
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = GetF584715SDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            var f584715 = new F584715
                            {
                                ZDEDBT = CastDbNull.To(reader["ZDEDBT"], string.Empty).Trim(),
                                ZDEDLN = CastDbNull.To<decimal>(reader["ZDEDLN"], 0),
                                ZDMCU = CastDbNull.To(reader["ZDMCU"], string.Empty).Trim(),
                                ZDZ58SCN = CastDbNull.To<decimal>(reader["ZDZ58SCN"], 0),
                                ZDZ58SCSN = CastDbNull.To<decimal>(reader["ZDZ58SCSN"], 0),
                                ZDEDTY = CastDbNull.To(reader["ZDEDTY"], string.Empty).Trim(),
                                ZDKCOO = CastDbNull.To(reader["ZDKCOO"], string.Empty).Trim(),
                                ZDDOCO = CastDbNull.To<decimal>(reader["ZDDOCO"], 0),
                                ZDDCTO = CastDbNull.To(reader["ZDDCTO"], string.Empty).Trim(),
                                ZDLNID = CastDbNull.To<decimal>(reader["ZDLNID"], 0),
                                ZDZ58TX01 = CastDbNull.To(reader["ZDZ58TX01"], string.Empty).Trim(),
                                ZDZ58TX02 = CastDbNull.To(reader["ZDZ58TX02"], string.Empty).Trim(),
                                ZDZ58TX03 = CastDbNull.To(reader["ZDZ58TX03"], string.Empty).Trim(),
                                ZDZ58TX04 = CastDbNull.To(reader["ZDZ58TX04"], string.Empty).Trim(),
                                ZDZ58TX05 = CastDbNull.To(reader["ZDZ58TX05"], string.Empty).Trim(),
                                ZDUSER = CastDbNull.To(reader["ZDUSER"], string.Empty).Trim(),
                                ZDPID = CastDbNull.To(reader["ZDPID"], string.Empty).Trim(),
                                ZDJOBN = CastDbNull.To(reader["ZDJOBN"], string.Empty).Trim(),
                                ZDUPMJ = CastDbNull.To<decimal>(reader["ZDUPMJ"], 0),
                                ZDTDAY = CastDbNull.To<decimal>(reader["ZDTDAY"], 0)
                            };
                            f584715Ls.Add(f584715);
                        }

                        reader.Close();
                        Logger.Trace($@"ROWS: {f584715Ls.Count}");
                    }
                } // if (filters.Any())
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
            return f584715Ls;
        }

        public static F584715s GetF584715Es(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var f584715Es = new F584715s();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Cust Msg
                filters.Add("ZDEDTY = 'E' AND \r\n");
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("ZDMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }
                // MTOSchedule Num
                if (scheduleNum > 0)
                {
                    filters.Add("ZDZ58SCN = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 5;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleNum;
                }
                // Batch Num
                if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                {
                    filters.Add("ZDEDBT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                    parameters.Last().Value = batchNum;
                }
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = GetF584715SDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            var f584715 = new F584715
                            {
                                ZDEDBT = CastDbNull.To(reader["ZDEDBT"], string.Empty).Trim(),
                                ZDEDLN = CastDbNull.To<decimal>(reader["ZDEDLN"], 0),
                                ZDMCU = CastDbNull.To(reader["ZDMCU"], string.Empty).Trim(),
                                ZDZ58SCN = CastDbNull.To<decimal>(reader["ZDZ58SCN"], 0),
                                ZDZ58SCSN = CastDbNull.To<decimal>(reader["ZDZ58SCSN"], 0),
                                ZDEDTY = CastDbNull.To(reader["ZDEDTY"], string.Empty).Trim(),
                                ZDKCOO = CastDbNull.To(reader["ZDKCOO"], string.Empty).Trim(),
                                ZDDOCO = CastDbNull.To<decimal>(reader["ZDDOCO"], 0),
                                ZDDCTO = CastDbNull.To(reader["ZDDCTO"], string.Empty).Trim(),
                                ZDLNID = CastDbNull.To<decimal>(reader["ZDLNID"], 0),
                                ZDZ58TX01 = CastDbNull.To(reader["ZDZ58TX01"], string.Empty).Trim(),
                                ZDZ58TX02 = CastDbNull.To(reader["ZDZ58TX02"], string.Empty).Trim(),
                                ZDZ58TX03 = CastDbNull.To(reader["ZDZ58TX03"], string.Empty).Trim(),
                                ZDZ58TX04 = CastDbNull.To(reader["ZDZ58TX04"], string.Empty).Trim(),
                                ZDZ58TX05 = CastDbNull.To(reader["ZDZ58TX05"], string.Empty).Trim(),
                                ZDUSER = CastDbNull.To(reader["ZDUSER"], string.Empty).Trim(),
                                ZDPID = CastDbNull.To(reader["ZDPID"], string.Empty).Trim(),
                                ZDJOBN = CastDbNull.To(reader["ZDJOBN"], string.Empty).Trim(),
                                ZDUPMJ = CastDbNull.To<decimal>(reader["ZDUPMJ"], 0),
                                ZDTDAY = CastDbNull.To<decimal>(reader["ZDTDAY"], 0)
                            };
                            f584715Es.Add(f584715);
                        }

                        reader.Close();
                        Logger.Trace($@"ROWS: {f584715Es.Count}");
                    }
                } // if (filters.Any())
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
            return f584715Es;
        }
    }
}