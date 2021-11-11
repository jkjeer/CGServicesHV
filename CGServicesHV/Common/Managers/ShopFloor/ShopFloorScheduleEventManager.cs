using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.ShopFloor
{
    public static class ShopFloorScheduleEventManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static SqlDmlResponse Save(SqlConnection cn, ScheduleEvent evt, string eventCode, string message)
        {
            var response = new SqlDmlResponse();
            try
            {
                var cmd = Insert();
                cmd.Parameters["@ScheduleId"].Value = evt.ScheduleId;
                cmd.Parameters["@ScheduleNum"].Value = evt.ScheduleNum;
                cmd.Parameters["@ScheduleDate"].Value = evt.ScheduleDate;
                cmd.Parameters["@E1ScheduleDate"].Value = evt.ScheduleDate;
                cmd.Parameters["@EventCode"].Value = eventCode;
                cmd.Parameters["@EventDate"].Value = DateTime.Now;
                cmd.Parameters["@EventMessage"].Value = message;
                cmd.Connection = cn;
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
                Logger.Error(e.Message);
                throw new Exception($"{cn.ConnectionString}  {e.Message}: {e.StackTrace} Values: {Utilities.Common.GetAllProperties(evt)}" );
            }
            return response;
        }

        public static SqlCommand Insert()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO EBS.ScheduleEventLedger (
                        [ScheduleId],
                        [ScheduleNum],
                        [ScheduleDate],
                        [E1ScheduleDate],
                        [EventCode],
                        [EventDate],
                        [EventMessage],
                        [WrittenDate]
                        )
                    VALUES (
                        @ScheduleId,
                        @ScheduleNum,
                        @ScheduleDate,
                        @E1ScheduleDate,
                        @EventCode,
                        @EventDate,
                        @EventMessage,
                        GETDATE()
                        )"
            };
            cmd.Parameters.Add(new SqlParameter("@ScheduleId", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
            cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@E1ScheduleDate", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@EventCode", SqlDbType.VarChar, 12));
            cmd.Parameters.Add(new SqlParameter("@EventDate", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@EventMessage", SqlDbType.VarChar, -1));
            return cmd;
        }

    }
}
