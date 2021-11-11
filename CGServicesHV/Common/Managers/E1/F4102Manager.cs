using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.E1
{
    public class F4102Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F4102Manager()
        {
        }

        public F4102Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F4102Manager.GetCustomerNum", GetPlannerNum()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/
        public iDB2Command GetPlannerNum()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        IBANPL,
                        IBPRP4
                    FROM
                        F4102
                    WHERE
                        IBMCU = ? AND
                        IBITM = ? 
                "
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public decimal GetPlannerNum(BusinessSystemSession e1Session, string plantNum, decimal itemNum)
        {
            decimal customerNum = 0;
            try
            {
                var cmd = e1Session.DbCommands["F4102Manager.GetCustomerNum"];
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["ITM"].Value = itemNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var family = CastDbNull.To(reader["IBPRP4"], string.Empty).Trim();
                        var planner = CastDbNull.To<decimal>(reader["IBANPL"], 0);
                        if (family == "PLN")
                        {
                            customerNum = planner;
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return customerNum;
        }

    }
}
