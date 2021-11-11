using System;
using System.Collections.Generic;
using System.Data;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class CustomerNameManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CustomerNameManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"CustomerNameManager.GetName", GetName()}
            };
            foreach (var keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        public string GetName(BusinessSystemSession session, decimal customerNum)
        {
            string name = "";
            try
            {
                var cmd = session.DbCommands["CustomerNameManager.GetName"];
                cmd.Parameters["AN8"].Value = customerNum;
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        name = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }

            return name;
        }

        public iDB2Command GetName()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    ABALPH            
                    FROM 
	                    F0101
                    WHERE 
                        ABAN8 = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AN8"].Precision = 8;
            cmd.Parameters["AN8"].Scale = 0;
            return cmd;
        }
    }
}
