using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class ScheduleManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


        public ScheduleManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            //hash.Add("RackManager.GetRacks", GetRacks());
            foreach (string keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }


        /******************************************************************
        * Racks - Dynamic
        ******************************************************************/
        public iDB2Command GetSchedules(List<string> filters)
        {
            var filterListString = String.Join(" ", filters);
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
	        SELECT 
                S1MCU,
                S1Z58SCD,
                S1Z58SCN
	        FROM
		        F5834A01
                INNER JOIN F5834A11
                ON  S5Z58SCN = S1Z58SCN AND
                    S5Z58SCD = S1Z58SCD AND
                    S5MCU = S1MCU
	        WHERE 
		        {0} 1=1
            GROUP BY
                S1MCU,
                S1Z58SCD,
                S1Z58SCN", filterListString);
            return cmd;
        }

        public Schedules GetMtoSchedules(BusinessSystemSession session, Plant plant, decimal workOrderNum, DateTime? scheduleDate = null)
        {
            var schedules = new Schedules();
            var cmd = new iDB2Command();
            List<String> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {

                if (workOrderNum > 0)
                {
                    filters.Add("S5DOCO = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = workOrderNum;
                }

                // Plant Code
                if (plant != null)
                {
                    filters.Add("S1MCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Plant Code
                if (scheduleDate != null)
                {
                    filters.Add("S1Z58SCD = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("Z58SCD", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 6;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = E1TimeManager.CastToE1DateId((DateTime) scheduleDate);
                }

                // Command
                cmd = GetSchedules(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                _logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {

                    // Reader
                    while (reader.Read())
                    {
                        var schedule = new Schedule();
                        schedule.ScheduleNum = Data.CastDbNull.To<Decimal>(reader["S1Z58SCN"], 0);
                        schedule.Plant = plant;
                        var scheduleDateId = Data.CastDbNull.To<Decimal>(reader["S1Z58SCD"], 0);
                        schedule.ScheduleDate = Utilities.Common.CastToDateTime(scheduleDateId, 0);
                        schedules.Add(schedule);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            return schedules;
        }

    }
}
