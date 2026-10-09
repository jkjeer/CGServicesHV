using System;
using System.Data;
using Microsoft.Data.SqlClient;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.ShopFloor
{
    public static class ScheduleStatusEventsManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static SqlCommand Insert()
        {

            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO ShopFloor.dbo.ScheduleStatusEvent (
                        [PlantNum],
                        [ScheduleId],
                        [ScheduleNum],
                        [ScheduleDate],
                        [EventCode],
                        [EventDate],
                        [EventMessage],
                        [WrittenDate],
                        [WrittenBy]
                        )
                    VALUES (
                        @PlantNum,
                        0,
                        @ScheduleNum,
                        @ScheduleDate,
                        @EventCode,
                        @TransactionDate,
                        '',
                        GETDATE(),
                        'SVCSOA'
                        )"
            };
            cmd.Parameters.Add(new SqlParameter("@PlantNum", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
            cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@EventCode", SqlDbType.VarChar));
            cmd.Parameters.Add(new SqlParameter("@TransactionDate", SqlDbType.DateTime));
            return cmd;
        }

        public static SqlDmlResponse Save(SqlConnection cn, string plantNum, decimal scheduleNum, DateTime scheduleDate, string eventCode, DateTime transactionDate, bool write)
        {
            var response = new SqlDmlResponse();
            try
            {
                var cmd = Insert();
                cmd.Parameters["@PlantNum"].Value = Convert.ToInt32(plantNum);
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters["@ScheduleDate"].Value = scheduleDate;
                cmd.Parameters["@EventCode"].Value = eventCode;
                cmd.Parameters["@TransactionDate"].Value = transactionDate;
                cmd.Connection = cn;
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return response;
        }

        public static int InsertScheduleStatusEvent(SqlConnection cnSF, string plantNum, decimal scheduleNum, DateTime scheduleDate, string eventCode, bool retry = false)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " SCHEDULE: " + scheduleNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            int rowCount = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnSF,
                    CommandType = CommandType.Text,
                    CommandText = @"
                        INSERT INTO [dbo].[ScheduleStatusEvent] (
                            [PlantNum]
                            ,[ScheduleId]
                            ,[ScheduleNum]
                            ,[ScheduleDate]
                            ,[EventCode]
                            ,[EventDate]
                            ,[EventMessage]
                            ,[WrittenDate]
                            ,[WrittenBy]
                            )
                        VALUES (
                            @PlantNum
                            ,0
                            ,@ScheduleNum
                            ,@ScheduleDate
                            ,@EventCode
                            ,GETDATE()
                            ,''
                            ,GETDATE()
                            ,'SVCSOA'
                        )"
                };
                cmd.Parameters.Add(new SqlParameter("@PlantNum", SqlDbType.Int));
                cmd.Parameters["@PlantNum"].Value = Convert.ToInt32(plantNum);
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
                cmd.Parameters["@ScheduleDate"].Value = scheduleDate;
                cmd.Parameters.Add(new SqlParameter("@EventCode", SqlDbType.VarChar));
                cmd.Parameters["@EventCode"].Value = eventCode;
                Logger.Trace(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                if (retry) throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }
    }
}
