using System;
using System.Collections.Generic;
using System.Data;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using CGServicesHV.WOStatusSvc;
using IBM.Data.DB2.iSeries;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class WorkOrderStatusManager
    {
        public WorkOrderStatusManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("WorkOrderStatusManager.GetPendingUpdatesByPlant", GetPendingUpdatesByPlant());
            hash.Add("WorkOrderStatusManager.SetCompletedUpdatesByPlant", SetCompletedUpdatesByPlant());

            foreach (string keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }


        public iDB2Command GetPendingUpdatesByPlant()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"SELECT 
                WADCTO,
                WADOCO,
                WAMCU,
                WASRST,
                WAPOST,
                WADATE,
                WATIME             
            FROM 
                F4801TRG
            WHERE 
                WAMCU = ? AND
                WAPOST = 'X'
            ORDER BY
                WADATE, 
                WATIME";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public WorkOrderStatuses GetPendingUpdatesByPlant(BusinessSystemSession session, string plantNum)
        {
            var records = new WorkOrderStatuses();
            try
            {
                var cmd = (session.DbCommands.ContainsKey("WorkOrderStatusManager.GetPendingUpdatesByPlant")) ? session.DbCommands["WorkOrderStatusManager.GetPendingUpdatesByPlant"] : GetPendingUpdatesByPlant();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var record = new WorkOrderStatus();
                        record.PlantNum = Data.CastDbNull.To<String>(reader["WAMCU"], String.Empty).Trim();
                        record.OrderNum = Data.CastDbNull.To<Decimal>(reader["WADOCO"], 0);
                        record.OrderType = Data.CastDbNull.To<String>(reader["WADCTO"], String.Empty).Trim();
                        record.PostedCode = Data.CastDbNull.To<String>(reader["WAPOST"], String.Empty).Trim();
                        record.StatusCode = Data.CastDbNull.To<String>(reader["WASRST"], String.Empty).Trim();
                        record.SystemDate = Data.CastDbNull.To<Decimal>(reader["WADATE"], 0);
                        record.SystemTime = Data.CastDbNull.To<Decimal>(reader["WATIME"], 0);
                        records.Add(record);
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return records;
        }

        public iDB2Command GetPlantsWithPendingUpdates()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"SELECT 
                WAMCU,
                COUNT(*) AS UPDATES          
            FROM 
                F4801TRG
            WHERE 
                WAPOST = 'X'
            GROUP BY
                WAMCU
            ORDER BY WAMCU";
            return cmd;
        }

        public PendingUpdatesResults GetPlantsWithPendingUpdates(BusinessSystemSession session)
        {
            var pendingUpdatesResults = new PendingUpdatesResults();
            try
            {
                var cmd = GetPlantsWithPendingUpdates();
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var pendingUpdatesResult = new PendingUpdatesResult();
                        pendingUpdatesResult.PlantNum = Data.CastDbNull.To<String>(reader["WAMCU"], String.Empty).Trim();
                        pendingUpdatesResult.Updates = Data.CastDbNull.To<int>(reader["UPDATES"], 0);
                        pendingUpdatesResults.Add(pendingUpdatesResult);

                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return pendingUpdatesResults;
        }

        public iDB2Command SetPendingUpdates()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"UPDATE 
                F4801TRG
            SET 
                WAPOST = 'X',
                WADATE = ?,
                WATIME = ?            
            WHERE 
                WAPOST = ''";
            cmd.Parameters.Add(new iDB2Parameter("DateInt", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DateInt"].Precision = 6;
            cmd.Parameters["DateInt"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("TimeInt", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TimeInt"].Precision = 6;
            cmd.Parameters["TimeInt"].Scale = 0;
            return cmd;
        }

        public SqlDmlResponse SetPendingUpdates(BusinessSystemSession session)
        {
            var response = new SqlDmlResponse();
            response.StartTime = DateTime.Now;
            response.ConnectionString = session.DbConnection.ConnectionString;
            var dtArray = GetDecimalDateTime();
            try
            {
                var cmd = SetPendingUpdates();
                response.Statement = cmd.CommandText;
                cmd.Parameters[0].Value = Convert.ToDecimal(dtArray[0]);
                cmd.Parameters[1].Value = Convert.ToDecimal(dtArray[1]);
                cmd.Connection = session.DbConnection;
                response.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception err)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(err));
                response.Message = err.Message;
                //throw new ApplicationException(err.ToString());
            }
            response.EndTime = DateTime.Now;
            return response;
        }

        public iDB2Command SetCompletedUpdatesByPlant()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"UPDATE 
                F4801TRG
            SET 
                WAPOST = 'Y'            
            WHERE 
                WAMCU = ? AND
                WAPOST = 'X'";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public SqlDmlResponse SetCompletedUpdatesByPlant(BusinessSystemSession session, string plantNum)
        {
            var response = new SqlDmlResponse();
            response.StartTime = DateTime.Now;
            try
            {
                var cmd = (session.DbCommands.ContainsKey("WorkOrderStatusManager.SetCompletedUpdatesByPlant")) ? session.DbCommands["WorkOrderStatusManager.SetCompletedUpdatesByPlant"] : SetCompletedUpdatesByPlant();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Connection = session.DbConnection;
                response.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception err)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(err));
                response.Message = err.Message;
                //throw new ApplicationException(err.ToString());
            }
            response.EndTime = DateTime.Now;
            return response;
        }

        private int[] GetDecimalDateTime()
        {
            var dtArray = new int[2];
            var dtNow = DateTime.Now;
            dtArray[0] = (dtNow.Month*10000) + (dtNow.Day*100) + (dtNow.Year - 2000);
            dtArray[1] = (dtNow.Hour * 10000) + (dtNow.Minute * 100) + dtNow.Second;
            return dtArray;
        }

    }
}
