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
    public class F580220Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /*********************************************************************************
         * Get F580220s Dynamic
         *******************************************************************************/

        private static iDB2Command GetF580220sDynamic(IReadOnlyCollection<string> filters)
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
                        CRZ58CRID,
                        CRZ58CRTY,
                        CRDOCO,
                        CRDCTO,
                        CRKCOO,
                        CRLNID,
                        CRUORG,
                        CRZ58CRQ,
                        CRZ58CRMSG,
                        CRDOC1,
                        CRRCTO,
                        CRRKCO,
                        CRRLLN,
                        CREV01,
                        CREV02,
                        CRUSER,
                        CRPID,
                        CRJOBN,
                        CRUPMJ,
                        CRTDAY
                    FROM
                        F580220 
                        INNER JOIN F580021 
                        ON S7RORN = CRDOCO AND 
                           S7RCTO = CRDCTO AND 
                           S7LNID = CRLNID
                    {filterString}
                    FOR READ ONLY"
            };
            return cmd;
        }

        /*********************************************************************************
         * Get F580220s FirstRacks Dynamic
         *******************************************************************************/

        private static iDB2Command GetF580220sFirstRacksDynamic(IReadOnlyCollection<string> filters)
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
                    SELECT * FROM (
	                    SELECT 
                            A.CRZ58CRID,
                            A.CRZ58CRTY,
                            A.CRDOCO,
                            A.CRDCTO,
                            A.CRKCOO,
                            A.CRLNID,
                            A.CRUORG,
                            A.CRZ58CRQ,
                            A.CRZ58CRMSG,
                            A.CRDOC1,
                            A.CRRCTO,
                            A.CRRKCO,
                            A.CRRLLN,
                            A.CREV01,
                            A.CREV02,
                            A.CRUSER,
                            A.CRPID,
                            A.CRJOBN,
                            A.CRUPMJ,
                            A.CRTDAY,
                            ROW_NUMBER() OVER (PARTITION BY CRDOCO, CRDCTO, CRLNID ORDER BY CRZ58CRID ASC) AS rownum
                        FROM
                            F580220 A
                            INNER JOIN F580021 
                            ON S7RORN = A.CRDOCO AND 
                               S7RCTO = A.CRDCTO AND 
                               S7LNID = A.CRLNID
                        {filterString}
                    ) X 
                    WHERE X.rownum = 1
                    FOR READ ONLY"
            };
            return cmd;
        }

        /*********************************************************************************
        * Methods
        ********************************************************************************/
        public F580220s GetF580220s(BusinessSystemSession e1Session, string plantNum = null, decimal scheduleNum = 0, decimal scheduleDateE1 = 0, string batchNum = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            var f580220s = new F580220s();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("S7MCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }

                // MTOSchedule Num
                if (scheduleNum > 0)
                {
                    filters.Add("S7Z58SCN = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 5;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleNum;
                }

                // Batch Num
                if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                {
                    filters.Add("S7EDBT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                    parameters.Last().Value = batchNum;
                }

                // MTOSchedule Date
                if (scheduleDateE1 > 0)
                {
                    filters.Add("S7Z58SCD = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 6;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleDateE1;
                }

                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND", "");
                    cmd = GetF580220sDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetF580220sDynamic(filters);
                }

                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var f580220 = new F580220
                        {
                            CRZ58CRID = Data.CastDbNull.To<decimal>(reader["CRZ58CRID"], 0),
                            CRZ58CRTY = Data.CastDbNull.To(reader["CRZ58CRTY"], string.Empty).Trim(),
                            CRDOCO = Data.CastDbNull.To<decimal>(reader["CRDOCO"], 0),
                            CRDCTO = Data.CastDbNull.To(reader["CRDCTO"], string.Empty).Trim(),
                            CRKCOO = Data.CastDbNull.To(reader["CRKCOO"], string.Empty).Trim(),
                            CRLNID = Data.CastDbNull.To<decimal>(reader["CRLNID"], 0),
                            CRUORG = Data.CastDbNull.To<decimal>(reader["CRUORG"], 0),
                            CRZ58CRQ = Data.CastDbNull.To<decimal>(reader["CRZ58CRQ"], 0),
                            CRZ58CRMSG = Data.CastDbNull.To(reader["CRZ58CRMSG"], string.Empty).Trim(),
                            CRDOC1 = Data.CastDbNull.To<decimal>(reader["CRDOC1"], 0),
                            CRRCTO = Data.CastDbNull.To(reader["CRRCTO"], string.Empty).Trim(),
                            CRRKCO = Data.CastDbNull.To(reader["CRRKCO"], string.Empty).Trim(),
                            CRRLLN = Data.CastDbNull.To<decimal>(reader["CRRLLN"], 0),
                            CREV01 = Data.CastDbNull.To(reader["CREV01"], string.Empty).Trim(),
                            CREV02 = Data.CastDbNull.To(reader["CREV02"], string.Empty).Trim(),
                            CRUSER = Data.CastDbNull.To(reader["CRUSER"], string.Empty).Trim(),
                            CRPID = Data.CastDbNull.To(reader["CRPID"], string.Empty).Trim(),
                            CRJOBN = Data.CastDbNull.To(reader["CRJOBN"], string.Empty).Trim(),
                            CRUPMJ = Data.CastDbNull.To<decimal>(reader["CRUPMJ"], 0),
                            CRTDAY = Data.CastDbNull.To<decimal>(reader["CRTDAY"], 0)
                        };
                        f580220s.Add(f580220);
                    }

                    reader.Close();
                    Logger.Trace($@"ROWS: {f580220s.Count}");
                }
            }
            catch (Exception e)
            {
                Logger.Trace("ERROR: " + e.Message);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Trace("CLOSE: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return f580220s;
        }

        public static F580220s GetF580220FirstRacks(BusinessSystemSession e1Session, string plantNum = null, decimal scheduleNum = 0, decimal scheduleDateE1 = 0, string batchNum = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var f580220s = new F580220s();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("S7MCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }
                // MTOSchedule Num
                if (scheduleNum > 0)
                {
                    filters.Add("S7Z58SCN = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 5;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleNum;
                }
                // MTOSchedule Date
                if (scheduleDateE1 > 0)
                {
                    filters.Add("S7Z58SCD = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 6;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleDateE1;
                }
                // Batch Num
                if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                {
                    filters.Add("S7EDBT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                    parameters.Last().Value = batchNum;
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND", "");
                    cmd = GetF580220sFirstRacksDynamic(filters);
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
                            var f580220 = new F580220
                            {
                                CRZ58CRID = Data.CastDbNull.To<decimal>(reader["CRZ58CRID"], 0),
                                CRZ58CRTY = Data.CastDbNull.To(reader["CRZ58CRTY"], string.Empty).Trim(),
                                CRDOCO = Data.CastDbNull.To<decimal>(reader["CRDOCO"], 0),
                                CRDCTO = Data.CastDbNull.To(reader["CRDCTO"], string.Empty).Trim(),
                                CRKCOO = Data.CastDbNull.To(reader["CRKCOO"], string.Empty).Trim(),
                                CRLNID = Data.CastDbNull.To<decimal>(reader["CRLNID"], 0),
                                CRUORG = Data.CastDbNull.To<decimal>(reader["CRUORG"], 0),
                                CRZ58CRQ = Data.CastDbNull.To<decimal>(reader["CRZ58CRQ"], 0),
                                CRZ58CRMSG = Data.CastDbNull.To(reader["CRZ58CRMSG"], string.Empty).Trim(),
                                CRDOC1 = Data.CastDbNull.To<decimal>(reader["CRDOC1"], 0),
                                CRRCTO = Data.CastDbNull.To(reader["CRRCTO"], string.Empty).Trim(),
                                CRRKCO = Data.CastDbNull.To(reader["CRRKCO"], string.Empty).Trim(),
                                CRRLLN = Data.CastDbNull.To<decimal>(reader["CRRLLN"], 0),
                                CREV01 = Data.CastDbNull.To(reader["CREV01"], string.Empty).Trim(),
                                CREV02 = Data.CastDbNull.To(reader["CREV02"], string.Empty).Trim(),
                                CRUSER = Data.CastDbNull.To(reader["CRUSER"], string.Empty).Trim(),
                                CRPID = Data.CastDbNull.To(reader["CRPID"], string.Empty).Trim(),
                                CRJOBN = Data.CastDbNull.To(reader["CRJOBN"], string.Empty).Trim(),
                                CRUPMJ = Data.CastDbNull.To<decimal>(reader["CRUPMJ"], 0),
                                CRTDAY = Data.CastDbNull.To<decimal>(reader["CRTDAY"], 0)
                            };
                            f580220s.Add(f580220);
                        }

                        reader.Close();
                        Logger.Trace($@"ROWS: {f580220s.Count}");
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
            return f580220s;
        }
    }
}