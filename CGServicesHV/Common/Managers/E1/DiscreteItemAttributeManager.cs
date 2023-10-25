using System;
using System.Configuration;
using System.Data;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.E1
{
    public static class DiscreteItemAttributeManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * Commands
        *******************************************************************************/
        public static ItemAttributes GetItemAttributes(BusinessSystemSession session, decimal itemNum)
        {
            var attributes = new ItemAttributes();
            try
            {
                var cmd = GetItemAttributes();
                cmd.Parameters[0].Value = itemNum;
                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var attribute = new ItemAttribute
                    {
                        ItemNum = itemNum,
                        Name = reader.Get<string>("ATNM"),
                        Value = reader.Get<string>("SGVL"),
                        Numeric = reader.Get<string>("CNUM"),
                        Udc1 = reader.Get<string>("RT"),
                        Udc2 = reader.Get<string>("SY"),
                        Desc1 = reader.Get<string>("DL01"),
                        Desc2 = reader.Get<string>("Dl02"),
                        Desc3 = reader.Get<string>("SPHD"),
                        TransactionDate = E1TimeManager.CastToDateTime(reader.Get<decimal>("UPMJ"), reader.Get<decimal>("TDAY"))
                    };
                    if (!attributes.Contains(attribute.Name))
                    {
                        attributes.Add(attribute);
                    }
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return attributes;
        }

        public static iDB2Command GetItemAttributes()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
	                IAZ58ATNM AS ATNM,
	                IADVAL AS SGVL,
                    IAUPMJ AS UPMJ,
                    IATDAY AS TDAY,
                    IACNUM AS CNUM,
                    IASY AS SY,
                    IART AS RT,
                    A.DRDL01 AS DL01,
                    A.DRDL02 AS DL02,
                    A.DRSPHD AS SPHD                      
                FROM
                    F580002
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 A
                    ON A.DRSY = IASY AND
                        A.DRRT = IART AND
                        TRIM(A.DRKY) = IADVAL  
                WHERE
	                IAITM = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }



    }
}
