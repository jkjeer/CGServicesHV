using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.ShopFloor;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class RackedScheduleUpdateManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackedScheduleUpdateManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"GetF580200Single", GetF580200Single()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant)
        {
            var response = new ProcessResponse("Racked Schedule Update");
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                foreach (var sqlCn in sqlCns)
                {
                    response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
                }

                var dt = new DataTable("BoxedSchedules");
                dt.Columns.Add("ScheduleNum", typeof(int));
                dt.Columns.Add("ScheduleDate", typeof(DateTime));
                dt.Columns.Add("BoxedDate", typeof(DateTime));
                dt.Columns.Add("RackedDate", typeof(DateTime));
                dt.Columns.Add("Boxed", typeof(int));
                dt.Columns.Add("Racked", typeof(int));
                dt.Columns.Add("EBSRacked", typeof(int));

                // Get Boxed Schedules
                var step1 = new ProcessStep("Get Boxed Schedules");
                try
                {
                    GetBoxedSchedules(sqlCns["PLANT.SHOPFLOOR"], dt);
                    step1.RowsAffected = dt.Rows.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);

                // Check E1 Racking
                var step2 = new ProcessStep("Check E1 Racking");
                try
                {
                    var mgr = new RackManager(session);
                    foreach (DataRow row in dt.Rows)
                    {
                        var schDt = mgr.GetScheduleRackStatus(session, plant, Convert.ToDecimal(row["ScheduleNum"]));
                        if (schDt.Rows.Count > 0)
                        {
                            DateTime rackedDate = (DateTime) schDt.Rows[0]["RackedDate"];
                            row["RackedDate"] = rackedDate.AddHours(plant.TimeOffset);
                            row["Boxed"] = (int) schDt.Rows[0]["Boxed"];
                            row["Racked"] = (int) schDt.Rows[0]["Racked"];
                            if ((int) row["Boxed"] == (int) row["Racked"])
                            {
                                step2.RowsAffected = ScheduleEventLedgerManager.InsertScheduleEvent(sqlCns["PLANT.INTEGRATION"], plant.PlantNum,
                                    Convert.ToDecimal(row["ScheduleNum"]),
                                    0, (DateTime)row["ScheduleDate"], "RACKED",
                                    Convert.ToDecimal(row["ScheduleNum"]), (DateTime)row["ScheduleDate"], "RACKED",
                                    0, "RACKED", DateTime.Now, "Racked");
                                if (step2.RowsAffected == 0)
                                {
                                    step2.RowsAffected = ScheduleEventLedgerManager.InsertScheduleEvent(sqlCns["PLANT.INTEGRATION"], plant.PlantNum,
                                        Convert.ToDecimal(row["ScheduleNum"]),
                                        0, (DateTime)row["ScheduleDate"], "RACKED",
                                        Convert.ToDecimal(row["ScheduleNum"]), (DateTime)row["ScheduleDate"], "RACKED",
                                        0, "RACKED", DateTime.Now, "Racked");
                                }
                            }
                        }
                    }
                    step2.RowsAffected = step2.SqlDmlResponses.Count;
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public SqlCommand GetBoxedSchedules()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
	        SELECT 
                ScheduleNum,
                ScheduleDate,
                StatusDate
	        FROM 
                dbo.Schedule
	        WHERE 
                IsCurrentVersion = 1 AND
                StatusCode = 'BOXED'";
            return cmd;
        }

        public DataTable GetBoxedSchedules(SqlConnection cn, DataTable dt)
        {
            try
            {
                var cmd = GetBoxedSchedules();
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var dr = dt.NewRow();
                        dr["ScheduleNum"] = CastDbNull.To(reader["ScheduleNum"], 0);
                        dr["ScheduleDate"] = CastDbNull.To(reader["ScheduleDate"], new DateTime(1900, 1, 1));
                        dr["BoxedDate"] = CastDbNull.To(reader["StatusDate"], new DateTime(1900, 1, 1));
                        dr["RackedDate"] = new DateTime(1900, 1, 1);
                        dr["Boxed"] = 0;
                        dr["Racked"] = 0;
                        dt.Rows.Add(dr);
                    }

                    reader.Close();
                }
                Logger.Trace($"Rows: {dt.Rows.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception($"{e.Message} {e.StackTrace}");
            }

            return dt;
        }
    }
}
