using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.Response;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Utilities
{
    public class Logging
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static string DbCmdString(SqlCommand cmd)
        {
            var sb = new StringBuilder();
            try
            {
                sb.Append(Environment.NewLine);
                sb.Append("************SQL SERVER STATEMENT*********");
                sb.Append(Environment.NewLine);
                sb.Append("[Connection String]");
                sb.Append(Environment.NewLine);
                sb.Append(cmd.Connection.ConnectionString);
                sb.Append(Environment.NewLine);
                sb.Append("[Command Text]");
                sb.Append(Environment.NewLine);
                sb.Append(cmd.CommandText);
                sb.Append(Environment.NewLine);
                sb.Append("[Command Parameters]");
                sb.Append(Environment.NewLine);
                sb.Append(string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                sb.Append(Environment.NewLine);
                sb.Append("*****************************************");
                sb.Append(Environment.NewLine);
            }
            catch (ApplicationException e)
            {
                throw new Exception($"{e.Message} {e.StackTrace}");
            }
            return sb.ToString();
        }

        public static string DbCmdString(iDB2Command cmd)
        {
            var sb = new StringBuilder();
            try
            {
                sb.Append(Environment.NewLine);
                sb.Append("**********DB2 STATEMENT******************");
                sb.Append(Environment.NewLine);
                sb.Append("[Connection String]");
                sb.Append(Environment.NewLine);
                sb.Append(cmd.Connection.ConnectionString);
                sb.Append(Environment.NewLine);
                sb.Append("[Command Text]");
                sb.Append(Environment.NewLine);
                sb.Append(cmd.CommandText);
                sb.Append(Environment.NewLine);
                sb.Append("[Command Parameters]");
                sb.Append(Environment.NewLine);
                sb.Append(string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                sb.Append(Environment.NewLine);
                sb.Append("*****************************************");
                sb.Append(Environment.NewLine);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message} {e.StackTrace}");
            }
            return sb.ToString();
        }

        public static void InsertProcessLog(ProcessResponse response, bool saveZero = false)
        {
            Logger ProcessLogger = LogManager.GetLogger("ProcessLog");
            try
            {
                if (response.RowsAffected > 0 || saveZero || response.HasErrors())
                {
                    if (response.ProcessSteps.Any())
                    {
                        LogEventInfo logEvent = new LogEventInfo();
                        logEvent.Level = LogLevel.Info;
                        logEvent.Properties.Add("EVENT_DATE", DateTime.Now);
                        logEvent.Properties.Add("PLANT", response.Plant?.PlantCode);
                        logEvent.Properties.Add("PROCESS", response.ProcessName);
                        logEvent.Properties.Add("STATUS", (response.HasErrors()) ? "ERROR" : "SUCCESS");
                        logEvent.Properties.Add("START_DATE", response.ProcessSteps.Min(x => x.StartDate));
                        logEvent.Properties.Add("END_DATE", response.ProcessSteps.Max(x => x.EndDate));
                        logEvent.Properties.Add("ROWS_AFFECTED", response.RowsAffected);
                        logEvent.Properties.Add("CONNECTION_STRINGS", XmlSerializationUtility.SerializeObject(Encoding.UTF8, response.ConnectionStrings, true));
                        logEvent.Properties.Add("STEPS", XmlSerializationUtility.SerializeObject(Encoding.UTF8, response, true));
                        ProcessLogger.Log(logEvent);
                    }
                    else
                    {
                        LogEventInfo logEvent = new LogEventInfo();
                        logEvent.Level = LogLevel.Info;
                        logEvent.Properties.Add("EVENT_DATE", DateTime.Now);
                        logEvent.Properties.Add("PLANT", response.Plant?.PlantCode);
                        logEvent.Properties.Add("PROCESS", response.ProcessName);
                        logEvent.Properties.Add("STATUS", (response.HasErrors()) ? "ERROR" : "SUCCESS");
                        logEvent.Properties.Add("START_DATE", response.StartDate);
                        logEvent.Properties.Add("END_DATE", response.EndDate);
                        logEvent.Properties.Add("ROWS_AFFECTED", response.RowsAffected);
                        logEvent.Properties.Add("CONNECTION_STRINGS", XmlSerializationUtility.SerializeObject(Encoding.UTF8, response.ConnectionStrings, true));
                        logEvent.Properties.Add("STEPS", XmlSerializationUtility.SerializeObject(Encoding.UTF8, response, true));
                        ProcessLogger.Log(logEvent);
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error($"{e.Message} {e.StackTrace}"));
                Logger.Error($@"ERROR: {e.Message} {e.StackTrace}");
                throw new Exception($"{e.Message} {e.StackTrace}");
            }
        }
    }
}