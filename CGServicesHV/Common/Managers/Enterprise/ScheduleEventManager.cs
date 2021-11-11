using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.E1
{
    public static class ScheduleEventManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /*****************************************************************************
         * Get Events
         *****************************************************************************/
        public static iDB2Command GetEvents(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT 
                S6EDBT,
                S6MCU,
                S6Z58SCN,
                S6Z58SCD,
                S6Z58SFS,
                S6TSC,
                S6EDSP,
                S6MATH06,
                S6WOPST,
                S6DS50
            FROM (
                SELECT 
                    S6EDBT,
                    S6MCU,
                    S6Z58SCN,
                    S6Z58SCD,
                    S6Z58SFS,
                    S6TSC,
                    S6EDSP,
                    S6MATH06,
                    S6WOPST,
                    S6DS50,
	                ROW_NUMBER() OVER (PARTITION BY S6Z58SCN ORDER BY S6Z58SCD DESC, S6EDBT DESC) AS RN
                FROM
                    F580020
                WHERE 
                    {filterString}
                    1=1
            ) A
            WHERE
	            A.RN = 1";
            return cmd;
        }

        public static ScheduleEvents GetEvents(BusinessSystemSession session, Plant plant, string status, decimal scheduleNum, List<string> systemCodes)
        {
            var evts = new ScheduleEvents();
            List<String> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            iDB2Command cmd;
            try
            {

                filters.Add("S6MCU = ? AND\r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                if (scheduleNum > 0)
                {
                    filters.Add("S6Z58SCN = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 5;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleNum;
                }
                else
                {
                    filters.Add("S6TSC = 'Y' AND\r\n");
                    if (!String.IsNullOrEmpty(status))
                    {
                        filters.Add("S6EDSP = ? AND\r\n");
                        parameters.Add(new iDB2Parameter("EDSP", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = status;
                    }
                    else
                    {
                        filters.Add("S6EDSP IN ('N') AND\r\n");
                    }
                }

                // Status Codes
                if (systemCodes.Any())
                {
                    var strInClause = "";
                    foreach (var systemCode in systemCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = systemCode;
                    }
                    filters.Add("S6Z58SFS IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                cmd = GetEvents(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;

                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var evt = new ScheduleEvent();
                    evt.Plant = plant;
                    evt.BatchNum = Data.CastDbNull.To<String>(reader["S6EDBT"], String.Empty).Trim();
                    evt.CostCenter = Data.CastDbNull.To<String>(reader["S6MCU"], String.Empty).Trim();
                    evt.PlantNum = Data.CastDbNull.To<String>(reader["S6MCU"], String.Empty).Trim();
                    evt.ScheduleNum = Data.CastDbNull.To<Decimal>(reader["S6Z58SCN"], 0);
                    evt.ScheduleDateE1 = Data.CastDbNull.To<Decimal>(reader["S6Z58SCD"], 0);
                    evt.ScheduleDate = Utilities.Common.CastToDateTime(evt.ScheduleDateE1, 0);
                    evt.ShopFloorSystem = Data.CastDbNull.To<String>(reader["S6Z58SFS"], String.Empty).Trim();
                    evt.StatusCode = Data.CastDbNull.To<String>(reader["S6TSC"], String.Empty).Trim();
                    evt.ProcessFlag = Data.CastDbNull.To<String>(reader["S6EDSP"], String.Empty).Trim();
                    evt.MathNumeric06 = Data.CastDbNull.To<Decimal>(reader["S6MATH06"], 0);
                    evt.OperationStatus = Data.CastDbNull.To<String>(reader["S6WOPST"], String.Empty).Trim();
                    evt.Description = Data.CastDbNull.To<String>(reader["S6DS50"], String.Empty).Trim();
                    evts.Add(evt);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return evts;
        }

        /*****************************************************************************
         * Set Event Status
         *****************************************************************************/
        public static SqlDmlResponse ReserveStatus(BusinessSystemSession session, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = F580020Manager.UpdateStatus(session, evt, "R", "Reserved");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse UpdateBeforeStatus(BusinessSystemSession session, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = F580020Manager.UpdateStatus(session, evt, "U", "In Use");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse UpdateAfterFail(BusinessSystemSession session, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = F580020Manager.UpdateStatus(session, evt, "E", "Error");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse UpdateAfterSuccess(BusinessSystemSession session, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = F580020Manager.UpdateStatus(session, evt, "Y", "Complete");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse Cancel(BusinessSystemSession session, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = F580020Manager.UpdateStatus(session, evt, "E", "Cancelled");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse Reset(BusinessSystemSession session, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = F580020Manager.UpdateStatus(session, evt, "N", "Reset");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse Success(BusinessSystemSession session, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = F580020Manager.UpdateStatus(session, evt, "Y", "Complete");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse Complete(BusinessSystemSession session, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = F580020Manager.UpdateStatus(session, evt, "C", "Complete");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        /*****************************************************************************
         * Set Event Status
         *****************************************************************************/
        public static iDB2Command GetRackedStatus()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
	            S1MCU,
	            S1Z58SCN,
	            S1Z58SCD,
	            S1Z58TQSC,
	            (SELECT COUNT(*) from F5834A11 INNER JOIN F580200 ON BIDOCO = S5DOCO WHERE S5Z58SCN = S1Z58SCN AND S5Z58SCD = S1Z58SCD AND S5MCU = S1MCU) AS BOXED,
	            (SELECT COUNT(*) from F5834A11 INNER JOIN F580056 ON RDDOC1 = S5DOCO WHERE S5Z58SCN = S1Z58SCN AND S5Z58SCD = S1Z58SCD AND S5MCU = S1MCU) AS RACKED
            FROM 
	            F5834A01 
            WHERE 
	            S1Z58SCN = ? AND 
	            S1Z58SCD = ? AND
	            S1MCU = ?";
            cmd.Parameters.Add(new iDB2Parameter("Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58SCN"].Precision = 5;
            cmd.Parameters["Z58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Z58SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58SCD"].Precision = 6;
            cmd.Parameters["Z58SCD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public static bool GetRackedStatus(BusinessSystemSession session, Plant plant, ScheduleEvent evt)
        {
            iDB2Command cmd;
            bool ret = false;
            try
            {
                cmd = GetRackedStatus();
                cmd.Parameters["Z58SCN"].Value = evt.ScheduleNum;
                cmd.Parameters["Z58SCD"].Value = evt.ScheduleDateE1;
                cmd.Parameters["MCU"].Value = evt.PlantNum.PadLeft(12);
                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    var boxed = Data.CastDbNull.To<int>(reader["BOXED"], 0);
                    var racked = Data.CastDbNull.To<int>(reader["RACKED"], 0);
                    ret = (boxed == racked);
                    Logger.Trace($"{evt.PlantNum} {evt.ScheduleNum} {evt.ScheduleDate} BOXED {boxed} RACKED {racked}");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return ret;
        }

    }
}
