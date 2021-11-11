using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class HighWaterMarkManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static SqlCommand GetHighWaterMarks(string filterString)
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
                    MarkId,
                    MarkName ,
                    EnterpriseDateTime ,
                    LocalDateTime ,
                    LookbackSeconds ,
                    UniqueID ,
                    WrittenDate,
                    InitStatement
                FROM 
                    EBS.HighWaterMark {filterString}"
            };
            return cmd;
        }

        public static HighWaterMarks GetHighWaterMarks(SqlConnection cn, Plant plant, List<string> markNames)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var marks = new HighWaterMarks();
            try
            {
                var inString = new StringBuilder();
                if (markNames.Count > 0)
                {
                    inString.Append(" WHERE MarkName IN (");
                    markNames[0] = markNames[0].Replace("MarkNames=", "");
                    foreach (var markName in markNames)
                    {
                        inString.Append($"'{markName}',");
                    }
                    if (inString.Length > 0) inString.Length -= 1; //Strip off trailing comma.
                    inString.Append(")");
                }
                var cmd = GetHighWaterMarks(inString.ToString());
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var mark = new HighWaterMark();
                        mark.PlantCode = plant.PlantCode;
                        mark.HighWaterMarkID = CastDbNull.To(reader["MarkId"], 0);
                        mark.HighWaterMarkName = CastDbNull.To(reader["MarkName"], string.Empty).Trim();
                        mark.EnterpriseDateTime = CastDbNull.To(reader["EnterpriseDateTime"], DateTime.MinValue);
                        mark.LocalDateTime = CastDbNull.To(reader["LocalDateTime"], DateTime.MinValue);
                        mark.LookbackSeconds = CastDbNull.To(reader["LookbackSeconds"], 0);
                        mark.UniqueID = CastDbNull.To<long>(reader["UniqueID"], 0);
                        mark.EnterpriseTimeSpan = (DateTime.Now - (mark.EnterpriseDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                        mark.LocalTimeSpan = (DateTime.Now - (mark.LocalDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                        mark.InitStatement = CastDbNull.To(reader["InitStatement"], string.Empty).Trim();
                        mark.Exists = true;
                        marks.Add(mark);
                    }

                    reader.Close();
                }
                Logger.Trace($@"ROWS: {marks.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cn.ConnectionString}  {e.Message}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return marks;
        }

        private static SqlCommand GetHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT
                    MarkId,
                    MarkName ,
                    EnterpriseDateTime ,
                    LocalDateTime ,
                    LookbackSeconds ,
                    UniqueID ,
                    WrittenDate
                FROM 
                    EBS.HighWaterMark
                WHERE 
                    MarkName = @MarkName "
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            return cmd;
        }

        public static HighWaterMark GetHighWaterMark(SqlConnection cn, Plant plant, string markName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var mark = new HighWaterMark(markName, plant.PlantCode);
            try
            {
                var cmd = GetHighWaterMark();
                cmd.Parameters["@MarkName"].Value = markName;
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        mark.HighWaterMarkID = CastDbNull.To(reader["MarkId"], 0);
                        mark.HighWaterMarkName = CastDbNull.To(reader["MarkName"], string.Empty).Trim();
                        mark.EnterpriseDateTime = CastDbNull.To(reader["EnterpriseDateTime"], DateTime.MinValue);
                        mark.LocalDateTime = CastDbNull.To(reader["LocalDateTime"], DateTime.MinValue);
                        mark.LookbackSeconds = CastDbNull.To(reader["LookbackSeconds"], 0);
                        mark.UniqueID = CastDbNull.To<long>(reader["UniqueID"], 0);
                        mark.EnterpriseTimeSpan = (DateTime.Now - (mark.EnterpriseDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                        mark.LocalTimeSpan = (DateTime.Now - (mark.LocalDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                        mark.Exists = true;
                    }

                    reader.Close();
                }
                Logger.Trace($@"ROWS: 1");

                if (!mark.Exists)
                {
                    throw new Exception($"High Water Mark [{markName}] does NOT exist at Plant {plant.PlantNum}-{plant.PlantCode}");
                }

                if (mark.UniqueID == 0)
                {
                    throw new Exception($"High Water Mark [{markName}] UniqueId = 0 at Plant {plant.PlantNum}-{plant.PlantCode}");
                }

            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cn.ConnectionString}  {e.Message}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return mark;
        }

        private static SqlCommand SaveHighWaterMarkDate()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                UPDATE 
                    EBS.HighWaterMark
                SET
                    EnterpriseDateTime = @EnterpriseDateTime,
                    LocalDateTime = @LocalDateTime,
                    WrittenDate = GETDATE()                     
                WHERE 
                    MarkName = @MarkName"
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            cmd.Parameters.Add(new SqlParameter("@EnterpriseDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LocalDateTime", SqlDbType.DateTime));
            return cmd;
        }

        public static SqlDmlResponse SaveHighWaterMarkDate(SqlConnection cn, Plant plant, HighWaterMark mark, DateTime enterpriseDateTime)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new SqlDmlResponse();
            try
            {

                if (mark.EnterpriseDateTime > enterpriseDateTime)
                {
                    Logger.Error($@"ERROR: High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Date to Older... Current {mark.EnterpriseDateTime} New {enterpriseDateTime}");
                    throw new Exception(
                        $"High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Date to Older... Current {mark.EnterpriseDateTime} New {enterpriseDateTime}");
                }

                //if (enterpriseDateTime > DateTime.Now.AddMinutes(1))
                //{
                //    Logger.Error($@"ERROR: High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Date to Future... Current {DateTime.Now} New {enterpriseDateTime}");
                //    throw new Exception(
                //        $"High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Date to Future... Current {DateTime.Now} New {enterpriseDateTime}");
                //}

                if (enterpriseDateTime > DateTime.Now) enterpriseDateTime = DateTime.Now;
                var localDateTime = enterpriseDateTime.AddHours(plant.TimeOffset);
                var cmd = SaveHighWaterMarkDate();
                cmd.Parameters["@MarkName"].Value = mark.HighWaterMarkName;
                cmd.Parameters["@EnterpriseDateTime"].Value = enterpriseDateTime;
                cmd.Parameters["@LocalDateTime"].Value = localDateTime;
                cmd.Connection = cn;
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
                Logger.Trace($@"ROWS: {rows}");
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error($"{cn.ConnectionString}  {e.Message}"));
                Logger.Error($@"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cn.ConnectionString}  {e.Message}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }

        private static SqlCommand SaveHighWaterMarkId()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                UPDATE 
                    EBS.HighWaterMark
                SET
                    EnterpriseDateTime = @EnterpriseDateTime,
                    LocalDateTime = @LocalDateTime,
                    WrittenDate = GETDATE(),
                    UniqueID = @UniqueID                        
                WHERE 
                    MarkName = @MarkName"
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            cmd.Parameters.Add(new SqlParameter("@EnterpriseDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LocalDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@UniqueID", SqlDbType.BigInt));
            return cmd;
        }

        public static SqlDmlResponse SaveHighWaterMarkId(SqlConnection cn, Plant plant, HighWaterMark mark, DateTime enterpriseDateTime, long uniqueId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new SqlDmlResponse();
            try
            {

                if (mark.EnterpriseDateTime > enterpriseDateTime)
                {
                    Logger.Error($@"ERROR: High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Date to Older... Current {mark.EnterpriseDateTime} New {enterpriseDateTime}");
                    throw new Exception(
                        $"High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Date to Older... Current {mark.EnterpriseDateTime} New {enterpriseDateTime}");
                }

                //if (enterpriseDateTime > DateTime.Now)
                //{
                //    Logger.Error($@"ERROR: High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Date to Future... Current {DateTime.Now} New {enterpriseDateTime}");
                //    throw new Exception(
                //        $"High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Date to Future... Current {DateTime.Now} New {enterpriseDateTime}");
                //}

                if (mark.UniqueID > uniqueId)
                {
                    Logger.Error($@"ERROR: High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Id ... Current {mark.UniqueID} New {uniqueId}");
                    throw new Exception(
                        $"High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Id ... Current {mark.UniqueID} New {uniqueId}");
                }

                if (uniqueId == 0)
                {
                    Logger.Error($@"ERROR: High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Id to 0");
                    throw new Exception(
                        $"High Water Mark Save Error ... {plant.PlantCode} {mark.HighWaterMarkName} Attempt to Reset Id to 0");
                }

                if (enterpriseDateTime > DateTime.Now) enterpriseDateTime = DateTime.Now;
                var localDateTime = enterpriseDateTime.AddHours(plant.TimeOffset);
                var cmd = SaveHighWaterMarkId();
                cmd.Parameters["@MarkName"].Value = mark.HighWaterMarkName;
                cmd.Parameters["@EnterpriseDateTime"].Value = enterpriseDateTime;
                cmd.Parameters["@LocalDateTime"].Value = localDateTime;
                cmd.Parameters["@UniqueID"].Value = uniqueId;
                cmd.Connection = cn;
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error($"{cn.ConnectionString}  {e.Message}"));
                Logger.Error($@"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cn.ConnectionString}  {e.Message}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }

    }
}
