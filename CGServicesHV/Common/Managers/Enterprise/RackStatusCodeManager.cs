using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class RackStatusCodeManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackStatusCodeManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackStatusCodeManager.GetRackStatusCodes", GetRackStatusCodes()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public iDB2Command GetRackStatusCodes()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT
                        DRKY,
                        DRDL01
                    FROM
                        {ConfigurationManager.AppSettings["ControlLib"]}.F0005 
                    WHERE
                        DRSY = '58' AND
                        DRRT = 'ST'"
            };
            return cmd;
        }

        public RackStatusCodes GetRackStatusCodes(BusinessSystemSession e1Session)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var rackStatusCodes = new RackStatusCodes();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("RackStatusCodeManager.GetRackStatusCodes")) ? e1Session.DbCommands["RackStatusCodeManager.GetRackStatusCodes"] : GetRackStatusCodes();
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var code = new RackStatusCode
                        {
                            UserCode = Data.CastDbNull.To(reader["DRKY"], string.Empty).Trim(),
                            Description1 = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim()
                        };
                        rackStatusCodes.Add(code);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rackStatusCodes.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return rackStatusCodes;
        }
    }
}
