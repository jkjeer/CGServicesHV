using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;
using CGServicesHV.Common.Types.ShopFloor;
using NLog;

namespace CGServicesHV.Common.Utilities
{
    public class TriggerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Triggers GetTriggers(SqlConnection cnSFInt, string tableName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var triggers = new Triggers();
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT  t.name AS 'TableName'
                               ,tr.name AS 'TriggerName'
                               ,tr.[create_date] AS 'TriggerCreateDate'
                               ,tr.[modify_date] AS 'TriggerMmodifyDate'
                               ,tr.[is_disabled]
	                           ,te.[type_desc]
                               ,t.[create_date] AS 'TableCreateDate'
                               ,t.[modify_date] AS 'TableModifyDate'
                        FROM    sys.triggers tr
		                        INNER JOIN sys.trigger_events te ON te.object_id = tr.object_id
                                INNER JOIN sys.tables t ON t.[object_id] = tr.[parent_id]
                        WHERE   t.name = '{tableName}'",
                    Connection = cnSFInt
                };
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var trigger = new Trigger
                        {
                            TableName = tableName,
                            TriggerName = Data.CastDbNull.To(reader["TriggerName"], string.Empty).Trim(),
                            InitialState = Data.CastDbNull.To(reader["is_disabled"], false)
                        };
                        triggers.Add(trigger);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + triggers.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e.Message + ": " + e.StackTrace);
            }

            Logger.Trace("CLOSE: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return triggers;
        }

        public bool SetTriggers(SqlConnection cnSFInt, string tableName, bool isEnable)
        {
            var startTime = DateTime.Now.ToLocalTime();
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = isEnable ? $@"ENABLE TRIGGER ALL ON {tableName}" : $@"DISABLE TRIGGER ALL ON {tableName}",
                    Connection = cnSFInt
                };
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: 1");
            }
            catch (Exception e)
            {
                Logger.Trace("ERROR: " + e.Message);
                throw new ApplicationException(cnSFInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            Logger.Trace("CLOSE: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return true;
        }

        public bool ResetTriggers(SqlConnection cnSFInt, Triggers triggers)
        {
            var startTime = DateTime.Now.ToLocalTime();
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnSFInt,
                    CommandType = CommandType.Text
                };
                var cmdStringBldr = new StringBuilder();
                var rowCount = 0;
                foreach (var trigger in triggers)
                {
                    rowCount += 1;
                    var cmdStatement = trigger.InitialState ? $@"ENABLE TRIGGER {trigger.TriggerName} ON {trigger.TableName};" : $@"DISABLE TRIGGER {trigger.TriggerName} ON {trigger.TableName};";
                    cmdStringBldr.Append(cmdStatement);
                }
                cmd.CommandText = cmdStringBldr.ToString();
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                if (rowCount > 0)
                {
                    cmd.CommandText = cmdStringBldr.ToString();
                    cmd.ExecuteNonQuery();
                }
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Trace("ERROR: " + e.Message);
                throw new ApplicationException(cnSFInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            Logger.Trace("CLOSE: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return true;
        }
    }
}
