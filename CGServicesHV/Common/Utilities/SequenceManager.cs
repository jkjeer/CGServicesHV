using System;
using System.Data;
using Microsoft.Data.SqlClient;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Utilities
{
    public class SequenceManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static long GetRange(SqlConnection cn, string sequenceName, int values)
        {
            long firstUkid;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "sys.sp_sequence_get_range"
                };
                cmd.Parameters.AddWithValue("@sequence_name", sequenceName);
                cmd.Parameters.AddWithValue("@range_size", values);
                var firstValueInRange = new SqlParameter("@range_first_value", SqlDbType.Variant)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(firstValueInRange);
                Logger.Info(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
                firstUkid = Data.CastDbNull.To<long>(firstValueInRange.Value, 0);
                Logger.Info($"firstUkid = {firstUkid}");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return firstUkid;
        }

        public static long GetNextVal(SqlConnection cn, string sequenceName)
        {
            long nextVal = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = String.Format(@"SELECT NEXT VALUE FOR {0} AS VAL", sequenceName),
                    Connection = cn
                };
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        nextVal = Data.CastDbNull.To<long>(reader["VAL"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return nextVal;
        }
    }
}
