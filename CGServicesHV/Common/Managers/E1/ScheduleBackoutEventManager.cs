using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.E1
{
    public static class ScheduleBackoutEventManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /*****************************************************************************
         * Get Status Events
         *****************************************************************************/
        public static SqlCommand GetEvents(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
                    ScheduleBackoutTransferLogID,
                    ScheduleId,
                    ScheduleNum,
                    ScheduleDate
                FROM
                    EBS.ScheduleBackoutTransferLog
                WHERE 
                    {filterString}
                    1=1";
            return cmd;
        }

        public static ScheduleEvents GetEvents(SqlConnection cn, Plant plant, string status, decimal scheduleNum)
        {
            var evts = new ScheduleEvents();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            SqlCommand cmd;
            try
            {

                filters.Add("ISNULL(TransferStatus, 'N') = @TransferStatus AND\r\n");
                parameters.Add(new SqlParameter("@TransferStatus", SqlDbType.VarChar, 10));
                parameters.Last().Value = "N";

                if (scheduleNum > 0)
                {
                    filters.Add("ScheduleNum = @ScheduleNum AND\r\n");
                    parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.Int));
                    parameters.Last().Value = scheduleNum;
                }

                cmd = GetEvents(filters);
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = cn;

                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var evt = new ScheduleEvent();
                    evt.Plant = plant;
                    evt.ScheduleId = Data.CastDbNull.To<int>(reader["ScheduleId"], 0);
                    evt.ScheduleNum = Data.CastDbNull.To<int>(reader["ScheduleNum"], 0);
                    evt.ScheduleDate = Data.CastDbNull.To<DateTime>(reader["ScheduleDate"], DateTime.MinValue);
                    evt.ScheduleDateE1 = E1TimeManager.CastToE1Date(Data.CastDbNull.To<DateTime>(reader["ScheduleDate"], DateTime.MinValue));
                    evt.TransferLogId = Data.CastDbNull.To<Int32>(reader["ScheduleBackoutTransferLogID"], 0);
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
         * Get Schedule Event
         *****************************************************************************/
        public static SqlCommand GetScheduleEvent()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                SELECT DISTINCT
                    E1Schedule,
                    QueueDateTime,
                    DueDate,
                    S7MCU
                FROM 
                    dbo.HVQue
                WHERE
                    S7MCU = @SchedulePlantNum AND
                    ScheduleNum = @ScheduleNum
             ";
            cmd.Parameters.Add(new SqlParameter("@SchedulePlantNum", SqlDbType.VarChar));
            cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.Int));
            return cmd;
        }

        public static ScheduleEvents GetEvent(SqlConnection cn, Plant plant, string schedulePlantNum, decimal scheduleNum)
        {
            var evts = new ScheduleEvents();
            try
            {
                var cmd = GetScheduleEvent();
                cmd.Parameters["@SchedulePlantNum"].Value = schedulePlantNum;
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var evt = new ScheduleEvent();
                    evt.Plant = plant;
                    evt.PlantNum = Data.CastDbNull.To<string>(reader["S7MCU"], "");
                    evt.ScheduleNum = Data.CastDbNull.To<int>(reader["E1Schedule"], 0);
                    evt.ScheduleDate = Data.CastDbNull.To<DateTime>(reader["DueDate"], DateTime.MinValue);
                    evt.ScheduleDateE1 = E1TimeManager.CastToE1Date(Data.CastDbNull.To<DateTime>(reader["DueDate"], DateTime.MinValue));
                    evt.TransferLogId = 0;
                    evts.Add(evt);
                }
                reader.Close();
                Logger.Trace($@"Rows: {evts.Count}");
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
        public static SqlDmlResponse ReserveStatus(SqlConnection cn, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = UpdateStatus(cn, evt, "R");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse UpdateBeforeStatus(SqlConnection cn, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = UpdateStatus(cn, evt, "U");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse UpdateAfterFail(SqlConnection cn, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = UpdateStatus(cn, evt, "E");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse UpdateAfterSuccess(SqlConnection cn, ScheduleEvent evt)
        {
            SqlDmlResponse sqlResponse;
            try
            {
                sqlResponse = UpdateStatus(cn, evt, "Y");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        /*****************************************************************************
        *   UPDATE STATUS
        ******************************************************************************/
        private static SqlCommand UpdateStatus()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                UPDATE 
                    EBS.ScheduleBackoutTransferLog
                SET 
                    TransferStatus = @StatusCode
                WHERE 
                    ScheduleBackoutTransferLogId = @ScheduleBackoutTransferLogId"
            };
            cmd.Parameters.Add(new SqlParameter("@StatusCode", SqlDbType.VarChar, 10));
            cmd.Parameters.Add(new SqlParameter("@ScheduleBackoutTransferLogId", SqlDbType.Int));
            return cmd;
        }

        public static SqlDmlResponse UpdateStatus(SqlConnection cn, ScheduleEvent evt, string statusCode)
        {
            var sqlResponse = new SqlDmlResponse();
            try
            {
                var cmd = UpdateStatus();
                cmd.Connection = cn;
                cmd.Parameters["@StatusCode"].Value = statusCode;
                cmd.Parameters["@ScheduleBackoutTransferLogId"].Value = evt.TransferLogId;
                Logger.Trace(Logging.DbCmdString(cmd));
                sqlResponse.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                sqlResponse.Done(rows);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }


        /*****************************************************************************
        *   Transfer Status
        ******************************************************************************/
        public static SqlDmlResponse SaveTransfer(SqlConnection cn, ScheduleEvent evt)
        {
            SqlDmlResponse response;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                    UPDATE
                        EBS.ScheduleBackoutTransferLog
                    SET     
                        TransferTS = GETDATE(),
                        TransferID = SUSER_NAME(),
                        TransferWS = HOST_NAME(),
                        TransferBatchNum = '{evt.TransferBatchNum}',
                        TransferTransactionId = {evt.TransferTransactionId},
                        TransferMessage = '{evt.TransferMessage}',
                        TransferStatus = '{evt.TransferStatusCode}'
                    WHERE
                        ScheduleBackoutTransferLogId = {evt.TransferLogId}"
                };
                response = new SqlDmlResponse(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                response.RowsAffected = rowCount;
                response.Done();
            }
            catch (Exception e)
            {
                throw new Exception($"{cn.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            return response;
        }

    }
}
