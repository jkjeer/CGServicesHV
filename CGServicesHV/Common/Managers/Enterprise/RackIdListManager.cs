using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class RackIdListManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackIdListManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackIdListManager.GetRackIdListByStatus", GetRackIdListByStatus()},
                {"RackIdListManager.GetRackIdListBySchedule", GetRackIdListBySchedule()},
                {"RackIdListManager.GetScheduleDateId", GetScheduleDateId()},
                {"RackIdListManager.GetCurrentRackIdList", GetCurrentRackIdList()},
                {"RackIdListManager.GetArchiveRackIdList", GetArchiveRackIdList()},
                {"RackIdListManager.GetLockedRacks", GetLockedRacks()}
            };
            foreach (var keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /****************************************************************
         * List By Status
         ***************************************************************/
        public iDB2Command GetRackIdListByStatus()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        RHZ58RID
                    FROM
                        F580055 
                         INNER JOIN ( SELECT
                                        S1MCU ,
                                        S1Z58SCN ,
                                        MAX(S1Z58SCD) AS S1Z58SCD
                                       FROM
                                        F5834A01
                                       GROUP BY
                                        S1MCU ,
                                        S1Z58SCN
                                     ) 
                          ON S1MCU = S5MCU AND
                             S1Z58SCN = S5Z58SCN AND
                             S1Z58SCD = S5Z58SCD
                    WHERE
                        RHMCU = ? AND
                        RHZ58HST = ? AND
                        RHZ58RSDT >= ? AND
                        RHZ58SCN < 1000
                    ORDER BY
                        RHZ58RID"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("RackStatus", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("RackStatusDateId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackStatusDateId"].Precision = 6;
            cmd.Parameters["RackStatusDateId"].Scale = 0;
            return cmd;
        }

        public List<decimal> GetRackIdListByStatus(BusinessSystemSession e1Session, Plant plant, string statusCode, TimeSpan timeSpan)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var plantNum = plant.PlantNum;
            var ets = new E1TimeSpan();
            if (timeSpan.TotalDays > 0)
            {
                ets.SetTimeSpan(timeSpan);
            }
            var rackIds = new List<decimal>();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("RackIdListManager.GetRackIdListByStatus")) ? e1Session.DbCommands["RackIdListManager.GetRackIdListByStatus"] : GetRackIdListByStatus();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = statusCode;
                cmd.Parameters[2].Value = ets.DateComponent1;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        rackIds.Add(Data.CastDbNull.To<decimal>(reader["RHZ58RID"], 0));
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rackIds.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return rackIds;
        }

        /****************************************************************
         * Locked Racks
         ***************************************************************/
        public iDB2Command GetLockedRacks()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        RHZ58RID
                    FROM
                        F580055 
                    WHERE
                        RHMCU = ? AND
                        RHZ58HST IN ('5','7','8') AND
                        RHZ58SCN < 1000 AND
                        (
                            (RHUPMJ = ? AND RHTDAY >= ?) OR 
                            (RHUPMJ >= ? AND RHUPMJ <= ?)
                        )
                    ORDER BY
                        RHZ58RID"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Date1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date1"].Precision = 6;
            cmd.Parameters["Date1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Time1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Time1"].Precision = 6;
            cmd.Parameters["Time1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date2"].Precision = 6;
            cmd.Parameters["Date2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date3"].Precision = 6;
            cmd.Parameters["Date3"].Scale = 0;
            return cmd;
        }

        public List<decimal> GetLockedRacks(BusinessSystemSession e1Session, string plantCode, decimal minutes)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            //var plantNum = e1Session.PlantMap.GetAttributesByCode(plantCode).NumericCode;
            var dtParms = new E1DateParms();
            dtParms.SetParms(DateTime.Now.AddMinutes((double)minutes * -1));
            var rackIds = new List<decimal>();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("RackIdListManager.GetRackIdListByStatus")) ? e1Session.DbCommands["RackIdListManager.GetRackIdListByStatus"] : GetRackIdListByStatus();
                //var cmd = GetRackIdListByStatus();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = dtParms.DateComponent1;
                cmd.Parameters[2].Value = dtParms.TimeComponent1;
                cmd.Parameters[3].Value = dtParms.DateComponent2;
                cmd.Parameters[4].Value = dtParms.DateComponent3;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        rackIds.Add(Data.CastDbNull.To<decimal>(reader["RHZ58RID"], 0));
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rackIds.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return rackIds;
        }

        /****************************************************************
         * List By Schedule
         ***************************************************************/
        public iDB2Command GetRackIdListBySchedule()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    RHZ58RID 
                    FROM  
	                    F580055  
	                    INNER JOIN F580056
	                    ON	RHZ58RID = RDZ58RID  
	                    INNER JOIN F5834A11 
	                    ON	RDDOC1 = S5DOCO AND
							RHZ58SCN = S5Z58SCN
	                    INNER JOIN ( SELECT
				                    S1MCU ,
				                    S1Z58SCN ,
				                    MAX(S1Z58SCD) AS S1Z58SCD
				                    FROM
				                    F5834A01
				                    GROUP BY
				                    S1MCU ,
				                    S1Z58SCN
                                ) S1
	                    ON	S1.S1MCU = S5MCU AND
		                    S1.S1Z58SCN = S5Z58SCN AND
		                    S1.S1Z58SCD = S5Z58SCD
                    WHERE
                        RHMCU = ? AND
                        RHZ58SCN = ? AND
                        RDURRF = ?
                    GROUP BY
                        RHZ58RID"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN"].Precision = 5;
            cmd.Parameters["SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("URRF", iDB2DbType.iDB2Graphic, 15));
            return cmd;
        }

        public List<decimal> GetRackIdListBySchedule(BusinessSystemSession e1Session, string plantCode, decimal scheduleNum)
        {
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            var rackIds = new List<decimal>();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("RackIdListManager.GetRackIdListBySchedule")) ? e1Session.DbCommands["RackIdListManager.GetRackIdListBySchedule"] : GetRackIdListBySchedule();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleNum.ToString(CultureInfo.InvariantCulture);
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        rackIds.Add(Data.CastDbNull.To<decimal>(reader["RHZ58RID"], 0));
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rackIds.Count);
                    Logger.Trace($"Rack Ids [{string.Join(",", rackIds.ToArray())}]");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return rackIds;
        }

        /****************************************************************
         * Rack List By Schedule - Scheduling
         ***************************************************************/
        public List<decimal> GetRackIdList(BusinessSystemSession e1Session, string plantCode, decimal scheduleNum)
        {
            List<decimal> rackIds;
            try
            {
                var scheduleDateId = GetScheduleDateId(e1Session, plantCode, scheduleNum);

                var currentRackIds = GetCurrentRackIdList(e1Session, plantCode, scheduleNum, scheduleDateId);
                if (currentRackIds.Count > 0)
                {
                    rackIds = currentRackIds;
                } else
                {
                    rackIds = GetArchiveRackIdList(e1Session, plantCode, scheduleNum, scheduleDateId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return rackIds;
        }



        public decimal GetScheduleDateId(BusinessSystemSession e1Session, string plantCode, decimal scheduleNum)
        {
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            decimal scheduleDateId = 0;
            try
            {
                var cmd = e1Session.DbCommands["RackIdListManager.GetScheduleDateId"];
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        scheduleDateId = Data.CastDbNull.To<decimal>(reader["S1Z58SCD"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return scheduleDateId;
        }

        public iDB2Command GetScheduleDateId()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	                MAX(S1Z58SCD) AS S1Z58SCD
                FROM  
	                F5834A01
                WHERE
	                S1MCU = ? AND
	                S1Z58SCN = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN"].Precision = 5;
            cmd.Parameters["SCN"].Scale = 0;
            return cmd;
        }

        public List<decimal> GetCurrentRackIdList(BusinessSystemSession e1Session, string plantCode, decimal scheduleNum, decimal scheduleDateId)
        {
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            var rackIds = new List<decimal>();
            try
            {
                var cmd = e1Session.DbCommands["RackIdListManager.GetCurrentRackIdList"];
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleNum.ToString(CultureInfo.InvariantCulture);
                cmd.Parameters[3].Value = scheduleDateId;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        rackIds.Add(Data.CastDbNull.To<decimal>(reader["RHZ58RID"], 0));
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rackIds.Count);
                    Logger.Trace($"Rack Ids [{string.Join(",", rackIds.ToArray())}]");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return rackIds;
        }

        public iDB2Command GetCurrentRackIdList()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	                RHZ58RID 
                FROM  
	                F580055  
	                INNER JOIN F580056
	                ON	RHZ58RID = RDZ58RID  
	                INNER JOIN F5834A11 
	                ON	RDDOC1 = S5DOCO AND
		                RHZ58SCN = S5Z58SCN
	                INNER JOIN F5834A01
	                ON	S1MCU = S5MCU AND
		                S1Z58SCN = S5Z58SCN AND
		                S1Z58SCD = S5Z58SCD
                WHERE
                    RHMCU = ? AND
                    RHZ58SCN = ? AND
	                RDURRF = ? AND
	                S1Z58SCD = ?
                GROUP BY
                    RHZ58RID"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN"].Precision = 5;
            cmd.Parameters["SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("URRF", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCD"].Precision = 6;
            cmd.Parameters["SCD"].Scale = 0;
            return cmd;
        }

        public List<decimal> GetArchiveRackIdList(BusinessSystemSession e1Session, string plantCode, decimal scheduleNum, decimal scheduleDateId)
        {
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            var rackIds = new List<decimal>();
            try
            {
                var cmd = e1Session.DbCommands["RackIdListManager.GetArchiveRackIdList"];
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleNum.ToString(CultureInfo.InvariantCulture);
                cmd.Parameters[3].Value = scheduleDateId;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        rackIds.Add(Data.CastDbNull.To<decimal>(reader["HLZ58RID"], 0));
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rackIds.Count);
                    Logger.Trace($"Rack Ids [{string.Join(",", rackIds.ToArray())}]");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return rackIds;
        }

        public iDB2Command GetArchiveRackIdList()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	                HLZ58RID 
                FROM  
	                F580559 
	                INNER JOIN F580569
	                ON	HLZ58RID = DLZ58RID  AND
		                HLZ58HST = DLZ58DST AND
		                HLMCU = DLMCU
	                INNER JOIN F5834A11 
	                ON	DLDOC1 = S5DOCO AND
		                HLZ58SCN = S5Z58SCN
	                INNER JOIN F5834A01
	                ON	S1MCU = S5MCU AND
		                S1Z58SCN = S5Z58SCN AND
		                S1Z58SCD = S5Z58SCD
                WHERE
                    HLMCU = ? AND
                    HLZ58SCN = ? AND
	                HLZ58HST = '8' AND
	                DLURRF = ? AND
	                S1Z58SCD = ?
                GROUP BY
                    HLZ58RID
                FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN"].Precision = 5;
            cmd.Parameters["SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("URRF", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCD"].Precision = 6;
            cmd.Parameters["SCD"].Scale = 0;
            return cmd;
        }

    }
}
