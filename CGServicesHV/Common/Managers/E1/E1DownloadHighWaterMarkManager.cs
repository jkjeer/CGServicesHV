using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CGServices.Common.Managers.Enterprise;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Types.Response;
using CGServices.Common.Types.ShopFloor;
using CGServices.Common.Utilities;
using NLog;

namespace CGServices.Common.Managers.E1
{
    public class E1DownloadHighWaterMarkManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static SqlCommand GetHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT [MarkId]
                            ,[MarkName]
                            ,[EnterpriseDateTime]
                            ,[LocalDateTime]
                            ,[WrittenDate]
                            ,[UniqueID]
                            ,[HighWaterMarkLookbackSeconds]
                            ,GETDATE() AS 'StartDateTimeSF'
                        FROM [INTEGRATION].[dbo].[HighWaterMarks]
                        WHERE [MarkName] = @MarkName "
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            return cmd;
        }

        public HighWaterMark GetHighWaterMark(BusinessSystemSession e1Session, SqlConnection cnSFInt, string plantNum, string markName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("HighwaterMark: " + markName + " BEGIN: " + DateTime.Now.ToLocalTime());
            var mark = new HighWaterMark
            {
                PlantNum = plantNum
            };
            try
            {
                var cmd = GetHighWaterMark();
                cmd.Parameters["@MarkName"].Value = markName;
                mark.HighWaterMarkName = markName;
                cmd.Connection = cnSFInt;
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                var counter = 0;
                while (reader.Read())
                {
                    mark.HighWaterMarkID = CastDbNull.To(reader["MarkId"], 0);
                    mark.HighWaterMarkName = CastDbNull.To(reader["MarkName"], string.Empty).Trim();
                    mark.EnterpriseDateTime = CastDbNull.To(reader["EnterpriseDateTime"], DateTime.MinValue);
                    mark.LocalDateTime = CastDbNull.To(reader["LocalDateTime"], DateTime.MinValue);
                    mark.LookbackSeconds = CastDbNull.To(reader["HighWaterMarkLookbackSeconds"], 0);
                    mark.UniqueID = CastDbNull.To<long>(reader["UniqueID"], 0);
                    mark.TimeSpan = (DateTime.Now - (mark.EnterpriseDateTime.AddSeconds(-CastDbNull.To(reader["HighWaterMarkLookbackSeconds"], 0))));
                    mark.StartDateTimeSF = CastDbNull.To(reader["StartDateTimeSF"], DateTime.MinValue);
                    mark.StartDateTimeE1 = E1TimeManager.GetE1TimeStamp(e1Session);
                    counter++;
                }
                reader.Close();
                Logger.Trace("ROWS: " + counter);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSFInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSFInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            Logger.Info("HighwaterMark: " + markName + " END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return mark;
        }

        private static SqlCommand UpdateHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    UPDATE 
                        [INTEGRATION].[dbo].[HighWaterMarks]
                    SET
                        [EnterpriseDateTime] = @EnterpriseDateTime,
                        [LocalDateTime] = @LocalDateTime,
                        [WrittenDate] = GETDATE(),
                        [UniqueID] = @UniqueID                        
                    WHERE 
                        [MarkName] = @MarkName"
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            cmd.Parameters.Add(new SqlParameter("@EnterpriseDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LocalDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@UniqueID", SqlDbType.Int));
            return cmd;
        }

        private static SqlCommand InsertHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT  INTO [INTEGRATION].[dbo].[HighWaterMarks]
					    ( [MarkName]
					    ,[EnterpriseDateTime]
					    ,[LocalDateTime]
					    ,[HighWaterMarkLookbackSeconds]
                        ,[WrittenDate]
                        ,[UniqueID] ) 
			        VALUES ( @MarkName
					    ,@EnterpriseDateTime
					    ,@LocalDateTime
					    ,120 
					    ,GETDATE()
                        ,@UniqueID)"
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            cmd.Parameters.Add(new SqlParameter("@EnterpriseDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LocalDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@UniqueID", SqlDbType.Int));
            return cmd;
        }

        public SqlDmlResponse SaveHighWaterMark(SqlConnection cnSFInt, HighWaterMark mark)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("HighwaterMark: " + mark + " BEGIN: " + DateTime.Now.ToLocalTime());
            var response = new SqlDmlResponse();
            try
            {
                var plant = PlantManager.GetPlant(mark.PlantNum);
                mark.LocalDateTime = mark.EnterpriseDateTime.AddHours(plant.TimeOffset);
                var cmd = (mark.HighWaterMarkID > 0) ? UpdateHighWaterMark() : InsertHighWaterMark();
                cmd.Parameters["@MarkName"].Value = mark.HighWaterMarkName;
                cmd.Parameters["@EnterpriseDateTime"].Value = mark.StartDateTimeE1;
                cmd.Parameters["@LocalDateTime"].Value = mark.StartDateTimeSF;
                cmd.Parameters["@UniqueID"].Value = mark.UniqueID;
                cmd.Connection = cnSFInt;
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                response.Init(cmd);
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
                Logger.Trace("ROWS: " + rows);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSFInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSFInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            Logger.Info("HighwaterMark: " + mark + " END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return response;
        }

        private static SqlCommand GetHighWaterMarks(List<string> filters)
        {
            var filterListString = string.Join(" ", filters);
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        MarkId,
                        MarkName,   
                        EnterpriseDateTime,
                        LocalDateTime,
                        WrittenDate,
                        UniqueID                                  
                    FROM 
	                    HighWaterMarks
                    WHERE 
                        {filterListString}
                        1=1"
            };
            return cmd;
        }

        public HighWaterMarks GetHighWaterMarks(SqlConnection cnSFInt, Plant plant, string markName = null, TimeSpan? timeSpan = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("HighwaterMark: " + markName + " BEGIN: " + DateTime.Now.ToLocalTime());
            var marks = new HighWaterMarks();
            var filters = new List<string>();
            var parameters = new List<SqlParameter>();
            try
            {
                // Mark Name
                if (!string.IsNullOrEmpty(markName))
                {
                    filters.Add("MarkName = @MarkName AND\r\n");
                    parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
                    parameters.Last().Value = markName;
                }
                // Time Span
                if (timeSpan != null && timeSpan.Value.TotalSeconds > 0)
                {
                    filters.Add("WrittenDate >= @WrittenDate AND\r\n");
                    parameters.Add(new SqlParameter("@WrittenDate", SqlDbType.DateTime));
                    var dt = DateTime.Now.Subtract((TimeSpan)timeSpan);
                    parameters.Last().Value = dt;
                }
                // Command
                var cmd = GetHighWaterMarks(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = cnSFInt;
                cmd.CommandTimeout = 0;
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var mark = new HighWaterMark
                    {
                        PlantNum = plant.PlantNum,
                        HighWaterMarkID = CastDbNull.To(reader["MarkId"], 0),
                        HighWaterMarkName = CastDbNull.To(reader["MarkName"], string.Empty).Trim(),
                        EnterpriseDateTime = CastDbNull.To(reader["EnterpriseDateTime"], DateTime.MinValue),
                        LocalDateTime = CastDbNull.To(reader["LocalDateTime"], DateTime.MinValue),
                        WrittenDate = CastDbNull.To(reader["WrittenDate"], DateTime.MinValue),
                        UniqueID = CastDbNull.To<long>(reader["UniqueID"], 0)
                    };
                    mark.TimeSpan = DateTime.Now - mark.EnterpriseDateTime;
                    marks.Add(mark);
                }
                reader.Close();
                Logger.Trace("ROWS: " + marks.Count);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSFInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSFInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            Logger.Info("HighwaterMark: " + markName + " END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return marks;
        }
    }
}
