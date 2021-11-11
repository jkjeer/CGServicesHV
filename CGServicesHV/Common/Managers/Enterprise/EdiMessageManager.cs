using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class EdiMessageManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EdiMessageManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GetEdiMessages", GetEdiMessages()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         *******************************************************************************/
        public iDB2Command GetEdiMessages()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        ZDEDTY,
                        ZDEDSQ,
                        ZDTXLN
                    FROM
                        F4715
                    WHERE
                        ZDDOCO > 0 AND
                        ZDTXLN IS NOT NULL AND
                        ZDKCOO = ? AND
                        ZDDOCO = ? AND
                        ZDDCTO = ? AND
                        ZDLNID = ?
                    ORDER BY
                        ZDEDTY,
                        ZDEDSQ"
            };
            cmd.Parameters.Add(new iDB2Parameter("SalesOrderCompany", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SalesOrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SalesOrderNum"].Precision = 8;
            cmd.Parameters["SalesOrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SalesOrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SalesOrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SalesOrderLineNum"].Precision = 6;
            cmd.Parameters["SalesOrderLineNum"].Scale = 0;
            return cmd;
        }

        public EdiMessages GetEdiMessages(BusinessSystemSession e1Session, string companyNum, decimal orderNum, string orderType, decimal orderLineNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var messages = new EdiMessages();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetEdiMessages")) ? e1Session.DbCommands["GetEdiMessages"] : GetEdiMessages();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["SalesOrderCompany"].Value = companyNum;
                cmd.Parameters["SalesOrderNum"].Value = orderNum;
                cmd.Parameters["SalesOrderType"].Value = orderType;
                cmd.Parameters["SalesOrderLineNum"].Value = orderLineNum;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var message = new EdiMessage
                        {
                            EdiType = CastDbNull.To(reader["ZDEDTY"], string.Empty).Trim(),
                            EdiSeqNum = CastDbNull.To<decimal>(reader["ZDEDSQ"], 0),
                            Message = CastDbNull.To(reader["ZDTXLN"], string.Empty).Trim()
                        };
                        messages.Add(message);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + messages.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return messages;
        }
    }
}
