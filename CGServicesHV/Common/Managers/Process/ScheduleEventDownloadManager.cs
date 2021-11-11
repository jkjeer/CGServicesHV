using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.ShopFloor;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public static class ScheduleEventDownloadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Logger ProcessLogger = LogManager.GetLogger("ProcessLog");

        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Schedule Event Download");
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                foreach (var sqlCn in sqlCns)
                {
                    response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
                }

                var dt = new DataTable("Schedule");
                dt.Columns.Add("ScheduleNum", typeof(int));
                dt.Columns.Add("ScheduleDate", typeof(DateTime));
                dt.Columns.Add("BoxedDate", typeof(DateTime));
                dt.Columns.Add("RackedDate", typeof(DateTime));
                dt.Columns.Add("Boxed", typeof(int));
                dt.Columns.Add("Racked", typeof(int));
                dt.Columns.Add("EBSRacked", typeof(int));
                dt.Columns.Add("EBSTransactionDate", typeof(DateTime));

                var dtEvent = new DataTable("ScheduleEvent");
                dtEvent.Columns.Add("ScheduleNum", typeof(int));
                dtEvent.Columns.Add("ScheduleDate", typeof(DateTime));
                dtEvent.Columns.Add("EventCode", typeof(string));
                dtEvent.Columns.Add("EventDate", typeof(DateTime));
                dtEvent.Columns.Add("EventMessage", typeof(string));

                // Get Boxed Schedules
                var step1 = new ProcessStep("Get Boxed Schedules");
                try
                {
                    GetSchedules(sqlCns["PLANT.SHOPFLOOR"], dt);
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
                            DateTime rackedDate = (DateTime)schDt.Rows[0]["RackedDate"];
                            row["RackedDate"] = rackedDate.AddHours(plant.TimeOffset);
                            row["Boxed"] = (int)schDt.Rows[0]["Boxed"];
                            row["Racked"] = (int)schDt.Rows[0]["Racked"];
                            if ((int)row["Boxed"] == (int)row["Racked"])
                            {
                                var dr = dtEvent.NewRow();
                                dr["ScheduleNum"] = Convert.ToDecimal(row["ScheduleNum"]);
                                dr["ScheduleDate"] = Convert.ToDateTime(row["ScheduleDate"]);
                                dr["EventCode"] = "RACKED";
                                dr["EventDate"] = rackedDate.AddHours(plant.TimeOffset);
                                dr["EventMessage"] = "";
                                dtEvent.Rows.Add(dr);

                                //var dmlResponse = ScheduleStatusEventsManager.Save(sqlCns["PLANT.SHOPFLOOR"], plant.PlantNum,
                                //    Convert.ToDecimal(row["ScheduleNum"]),
                                //    (DateTime)row["ScheduleDate"], "RACKED", (DateTime)row["RackedDate"], true);
                                //step2.SqlDmlResponses.Add(dmlResponse);
                            }
                        }
                    }
                    step2.RowsAffected = step2.SqlDmlResponses.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                // Check EBS Racking
                var step3 = new ProcessStep("Check EBS Racking");
                try
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var ebsDt = GetEbsRacking(sqlCns["PLANT.SHOPFLOOR"], plant, Convert.ToDecimal(row["ScheduleNum"]), Convert.ToDateTime(row["ScheduleDate"]));
                        if ((int)ebsDt.Rows[0]["Units"] > 0)
                        {
                            if ((int)row["Boxed"] == (int)ebsDt.Rows[0]["Units"])
                            {

                                var dr = dtEvent.NewRow();
                                dr["ScheduleNum"] = Convert.ToDecimal(row["ScheduleNum"]);
                                dr["ScheduleDate"] = Convert.ToDateTime(row["ScheduleDate"]);
                                dr["EventCode"] = "EBS-RACKED";
                                dr["EventDate"] = (DateTime) ebsDt.Rows[0]["TransactionDate"];
                                dr["EventMessage"] = "";
                                dtEvent.Rows.Add(dr);

                                //var dmlResponse = ScheduleStatusEventsManager.Save(sqlCns["PLANT.SHOPFLOOR"], plant.PlantNum,
                                //    Convert.ToDecimal(row["ScheduleNum"]),
                                //    (DateTime)row["ScheduleDate"], "EBS-RACKED", (DateTime)ebsDt.Rows[0]["TransactionDate"], true);
                                //step3.SqlDmlResponses.Add(dmlResponse);
                            }
                        }
                    }
                    step3.RowsAffected = step3.SqlDmlResponses.Count;
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }
                step3.Done();
                response.AddStep(step3);

                /********************************************************/
                if (dtEvent.Rows.Count > 0)
                {
                    var step4 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (dtEvent.Rows.Count > 0)
                        {
                            bulkResponse = BulkCopyManager.BulkTable("EBS.ScheduleEventLedger", dtEvent, sqlCns["PLANT.INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.RowsAffected = (int)bulkResponse.RowsCopied;
                        step4.SqlBulkCopyResponses.Add(bulkResponse);
                        step4.Done();
                        response.RowsAffected = step4.RowsAffected;
                        response.AddStep(step4);
                    }
                }

            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static SqlCommand GetSchedules()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format(@"
	        SELECT 
                ScheduleNum,
                ScheduleDate,
                StatusDate
	        FROM 
                dbo.Schedule
	        WHERE 
                StatusDate > GETDATE() - 7 AND
                IsCurrentVersion = 1 AND
                StatusCode IN ('BOXED','RACKED')");
            return cmd;
        }

        public static DataTable GetSchedules(SqlConnection cn, DataTable dt)
        {
            try
            {
                var cmd = GetSchedules();
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var dr = dt.NewRow();
                        dr["ScheduleNum"] = CastDbNull.To<int>(reader["ScheduleNum"], 0);
                        dr["ScheduleDate"] = CastDbNull.To<DateTime>(reader["ScheduleDate"], new DateTime(1900, 1, 1));
                        dr["BoxedDate"] = CastDbNull.To<DateTime>(reader["StatusDate"], new DateTime(1900, 1, 1));
                        dr["RackedDate"] = new DateTime(1900, 1, 1);
                        dr["Boxed"] = 0;
                        dr["Racked"] = 0;
                        dt.Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message} {e.StackTrace}");
            }

            return dt;
        }

        /*****************************************************************************
        *   SHOP FLOOR RACKING
        ******************************************************************************/
        public static SqlCommand GetEbsRacking()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                SELECT
                    COUNT(*) AS Units,
                    MAX(ri.WrittenDate) AS TransactionDate
                FROM
                    EBS.vw_FuseOrders ord
	                INNER JOIN EBS.RackItem ri
	                ON ri.DocumentNum = ord.WOrderNum AND
                    ri.DocumentType = 'WO'
                WHERE
                    ord.ScheduleNum = @ScheduleNum AND 
	                ord.ScheduleDate = @ScheduleDate AND
                    ord.IsCurrentVersion = 1 ";
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public static DataTable GetEbsRacking(SqlConnection cn, Plant plant, decimal scheduleNum, DateTime scheduleDate)
        {
            var dt = new DataTable("EBSRacking");
            dt.Columns.Add("Units", typeof(int));
            dt.Columns.Add("TransactionDate", typeof(DateTime));
            try
            {
                var cmd = GetEbsRacking();
                cmd.Parameters[0].Value = (int) scheduleNum;
                cmd.Parameters[1].Value = scheduleDate;
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        var dr = dt.NewRow();
                        dr["Units"] = CastDbNull.To<int>(reader["Units"], 0);
                        dr["TransactionDate"] = CastDbNull.To<DateTime>(reader["TransactionDate"], new DateTime(1900, 1, 1));
                        dt.Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message} {e.StackTrace}");
            }

            return dt;
        }

    }


}
