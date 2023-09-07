using NLog;
using System;
using System.Data;

namespace CGServicesHV.Common.Utilities
{
    public static class ReaderExtension
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static T Get<T>(this IDataReader r, string columnName, T defaultValue = default(T), bool trim = true)
        {
            var obj = r[columnName];
            var i = r.GetOrdinal(columnName);
            try
            {
                if (r.IsDBNull(r.GetOrdinal(columnName)))
                {
                    return defaultValue;
                }
                else
                {
                    switch (typeof(T).Name.ToLower())
                    {
                        case "string":
                            obj = (trim) ? r.GetString(i).Trim() : r.GetString(i);
                            break;
                        case "int16":
                            obj = r.GetInt16(i);
                            break;
                        case "int":
                            obj = r.GetInt32(i);
                            break;
                        case "int32":
                            obj = r.GetInt32(i);
                            break;
                        case "int64":
                            obj = r.GetInt64(i);
                            break;
                        case "decimal":
                            obj = r.GetDecimal(i);
                            break;
                        case "double":
                            obj = r.GetDouble(i);
                            break;
                        case "float":
                            obj = r.GetFloat(i);
                            break;
                        case "datetime":
                            obj = r.GetDateTime(i);
                            break;
                        case "bool":
                            obj = r.GetBoolean(i);
                            break;
                        case "byte":
                            obj = r.GetByte(i);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(columnName);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception($"{e.Message}:  {columnName}");
            }
            return (T)obj;
        }
    }
}
