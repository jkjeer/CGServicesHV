using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CGServicesHV.Common.Types.Response;
using NLog;

namespace CGServicesHV.Common.Utilities
{
    public class Sql
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static SqlDmlResponses SqlBatchExecution(List<string> statements, short batchSize, SqlConnection cn, SqlTransaction txn = null)
        {
            var responses = new SqlDmlResponses();
            var cmd = new SqlCommand();
            cmd.Connection = cn;
            if (txn != null)
            {
                cmd.Transaction = txn;
            }

            var sb = new StringBuilder();
            int i = 0;
            try
            {
                foreach (var statement in statements)
                {
                    i++;
                    sb.Append(statement);
                    if (i % batchSize == 0 || i == statements.Count)
                    {
                        var dmlResponse = new SqlDmlResponse();
                        cmd.CommandText = sb.ToString();
                        dmlResponse.Init(cmd);
                        Logger.Trace(Logging.DbCmdString(cmd));
                        int rows = cmd.ExecuteNonQuery();
                        dmlResponse.RowsAffected = rows;
                        responses.Add(dmlResponse);
                        sb.Clear();
                    }
                }
                txn?.Commit();
            }
            catch (Exception e)
            {
                txn?.Rollback();
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return responses;
        }
    }
}
