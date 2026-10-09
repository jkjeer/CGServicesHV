using System;
using System.Data;
using Microsoft.Data.SqlClient;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.ShopFloor
{
    public class ScheduleEventLedgerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static int InsertScheduleEvent(SqlConnection cnSF, string plantNum, decimal scheduleNum, int prodScheduleId, DateTime scheduleDate, string scheduleStatus,
            decimal e1ScheduleNum, DateTime e1ScheduleDate, string e1ScheduleStatus, int workOrderQty,
            string eventCode, DateTime eventDate, string eventMessage, bool retry = false)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " SCHEDULE: " + scheduleNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            int rowCount = 0;
            try
            {
                var serviceContext = new ServiceContext();
                var cmd = new SqlCommand
                {
                    Connection = cnSF,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                    INSERT INTO INTEGRATION.EBS.ScheduleEventLedger (
                        [PlantNum]
                          ,[ScheduleId]
                          ,[ScheduleNum]
                          ,[ProdScheduleId]
                          ,[ScheduleDate]
                          ,[ScheduleStatus]
                          ,[E1ScheduleNum]
                          ,[E1ScheduleDate]
                          ,[E1ScheduleStatus]
                          ,[WorkOrderQty]
                          ,[EventCode]
                          ,[EventDate]
                          ,[EventMessage]
                          ,[WrittenDate]
                          ,[WrittenBy]
                          ,[WorkStation]
                        )
                    VALUES (
                        @PlantNum,
                        0,
                        @ScheduleNum,
                        @ProdScheduleId,
                        @ScheduleDate,
                        @ScheduleStatus,
                        @E1ScheduleNum,
                        @E1ScheduleDate,
                        @E1ScheduleStatus,
                        @WorkOrderQty,
                        @EventCode,
                        @EventDate,
                        @EventMessage,
                        GETDATE(),
                        '{serviceContext.UserName}',
                        '{serviceContext.HostName}'
                        )"
                };
                cmd.Parameters.Add(new SqlParameter("@PlantNum", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.Decimal));
                cmd.Parameters.Add(new SqlParameter("@ProdScheduleId", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
                cmd.Parameters.Add(new SqlParameter("@ScheduleStatus", SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@E1ScheduleNum", SqlDbType.Decimal));
                cmd.Parameters.Add(new SqlParameter("@E1ScheduleDate", SqlDbType.DateTime));
                cmd.Parameters.Add(new SqlParameter("@E1ScheduleStatus", SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@WorkOrderQty", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@EventCode", SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@EventDate", SqlDbType.DateTime));
                cmd.Parameters.Add(new SqlParameter("@EventMessage", SqlDbType.VarChar));
                cmd.Parameters["@PlantNum"].Value = Convert.ToInt32(plantNum);
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters["@ProdScheduleId"].Value = prodScheduleId;
                cmd.Parameters["@ScheduleDate"].Value = scheduleDate;
                cmd.Parameters["@ScheduleStatus"].Value = scheduleStatus;
                cmd.Parameters["@E1ScheduleNum"].Value = e1ScheduleNum;
                cmd.Parameters["@E1ScheduleDate"].Value = e1ScheduleDate;
                cmd.Parameters["@E1ScheduleStatus"].Value = e1ScheduleStatus;
                cmd.Parameters["@WorkOrderQty"].Value = workOrderQty;
                cmd.Parameters["@EventCode"].Value = eventCode;
                cmd.Parameters["@EventDate"].Value = eventDate;
                cmd.Parameters["@EventMessage"].Value = eventMessage;
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
