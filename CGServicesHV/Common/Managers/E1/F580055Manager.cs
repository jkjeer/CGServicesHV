using System;
using System.Collections.Generic;
using System.Data;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.E1
{
    public class F580055Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static iDB2Command GetSerialNumsByWorkOrder()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT RHZ58RID
	                    , RHZ58SCN
	                    , RDITM
	                    , RDPQOH
	                    , RDZ58CQTY
	                    , RDDOC1
	                    , RDXDCTO
	                    , RDLOT1
	                    , RDZ58SN
	                    , RDURRF
                    FROM  F580055
	                    INNER JOIN F580056 ON RDZ58RID = RHZ58RID
                    WHERE  
                        RHMCU = ?
                        AND RDDOC1 = ?
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("RHMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("RDDOC1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RDDOC1"].Precision = 8;
            cmd.Parameters["RDDOC1"].Scale = 0;
            return cmd;
        }

        public static List<string> GetSerialNumsByWorkOrder(BusinessSystemSession e1Session, string plantNum, decimal workOrder)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            var sqlResponse = new SqlDmlResponse();
            var serialNums = new List<string>();
            try
            {
                var cmd = GetSerialNumsByWorkOrder();
                cmd.Parameters["RHMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["RDDOC1"].Value = workOrder;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                sqlResponse.Init(cmd);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var serialNum = Data.CastDbNull.To<string>(reader["RDZ58SN"], string.Empty).Trim();
                        serialNums.Add(serialNum);
                    }

                    reader.Close();
                    sqlResponse.Done(serialNums.Count);
                    Logger.Trace("ROWS: " + serialNums.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return serialNums;
        }

        private static iDB2Command GetSerialNumsByScheduleNum()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT RHZ58RID
	                    , RHZ58SCN
	                    , RDITM
	                    , RDPQOH
	                    , RDZ58CQTY
	                    , RDDOC1
	                    , RDXDCTO
	                    , RDLOT1
	                    , RDZ58SN
	                    , RDURRF
                    FROM  F580055
	                    INNER JOIN F580056 ON RDZ58RID = RHZ58RID
                    WHERE  
                        RHMCU = ?
                        AND RHZ58SCN = ?
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("RHMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("RHZ58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RHZ58SCN"].Precision = 5;
            cmd.Parameters["RHZ58SCN"].Scale = 0;
            return cmd;
        }

        public static List<string> GetSerialNumsBySchedule(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            var sqlResponse = new SqlDmlResponse();
            var serialNums = new List<string>();
            try
            {
                var cmd = GetSerialNumsByScheduleNum();
                cmd.Parameters["RHMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["RHZ58SCN"].Value = scheduleNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                sqlResponse.Init(cmd);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var serialNum = Data.CastDbNull.To<string>(reader["RDZ58SN"], string.Empty).Trim();
                        serialNums.Add(serialNum);
                    }

                    reader.Close();
                    sqlResponse.Done(serialNums.Count);
                    Logger.Trace("ROWS: " + serialNums.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return serialNums;
        }
    }
}
