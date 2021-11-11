using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class UdcItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UdcItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"UdcItemManager.GetUdcItem", GetUdcItem()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        private static iDB2Command GetUdcItem()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
                        DRSY,
                        DRRT,
                        DTDL01,
                        DRKY,
                        DRDL01,
                        DRDL02,
                        DRSPHD,
                        DRUPMJ,
                        DRUPMT
                    FROM
                        {ConfigurationManager.AppSettings["ControlLib"]}.F0005 
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0004
                        ON DRSY = DTSY AND
                           DRRT = DTRT
                    WHERE 
                        DRSY = ? AND
                        DRRT = ? AND
                        TRIM(DRKY) = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("DRSY", iDB2DbType.iDB2Graphic, 4));
            cmd.Parameters.Add(new iDB2Parameter("DRRT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("DRKY", iDB2DbType.iDB2Graphic, 10));
            return cmd;
        }

        /*********************************************************************************
         * Get UDC Items Dynamic
         *******************************************************************************/
        private static iDB2Command GetUdcItemsDynamic(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = string.Format(@"
	                SELECT 
                        DRSY,
                        DRRT,
                        DTDL01,
                        DRKY,
                        DRDL01,
                        DRDL02,
                        DRSPHD,
                        DRUPMJ,
                        DRUPMT
                    FROM
                        {0}.F0005 
                        LEFT OUTER JOIN {0}.F0004
                        ON DRSY = DTSY AND
                           DRRT = DTRT
                    {1}",
                    ConfigurationManager.AppSettings["ControlLib"],
                    filterString)
            };
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static UdcItems GetUdcItems(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string[] productCodes, string userCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var udcItems = new UdcItems();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Product Codes
                if (productCodes != null && productCodes.Any())
                {
                    var strInClause = "";
                    foreach (var productCode in productCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("ProductCodes", iDB2DbType.iDB2Graphic, 4));
                        parameters.Last().Value = productCode;
                    }

                    filters.Add("LEFT(DRSY,2) IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // User Code
                if (!string.IsNullOrEmpty(userCode))
                {
                    filters.Add("DRRT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("UserCode", iDB2DbType.iDB2Graphic, 2));
                    parameters.Last().Value = userCode;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("DTUPMJ", "DTUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + ")" + " AND \r\n");
                }

                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetUdcItemsDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetUdcItemsDynamic(filters);
                }

                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var udcItem = new UdcItem
                        {
                            ProductCode = CastDbNull.To(reader["DRSY"], string.Empty).Trim(),
                            UserCode = CastDbNull.To(reader["DRRT"], string.Empty).Trim(),
                            UserCodeType = CastDbNull.To(reader["DTDL01"], string.Empty).Trim(),
                            UserValue = CastDbNull.To(reader["DRKY"], string.Empty).Trim(),
                            Description1 = CastDbNull.To(reader["DRDL01"], string.Empty).Trim(),
                            Description2 = CastDbNull.To(reader["DRDL02"], string.Empty).Trim(),
                            SpecialHandlingCode = CastDbNull.To(reader["DRSPHD"], string.Empty).Trim()
                        };
                        var updateDateId = CastDbNull.To<decimal>(reader["DRUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<decimal>(reader["DRUPMT"], 0);
                        udcItem.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                        udcItems.Add(udcItem);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + udcItems.Count);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return udcItems;
        }

        public UdcItem GetUdcItem(BusinessSystemSession e1Session, string productCode, string userCode, string userKey)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var item = new UdcItem();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("UdcItemManager.GetUdcItem")) ? e1Session.DbCommands["UdcItemManager.GetUdcItem"] : GetUdcItem();
                cmd.Parameters["DRSY"].Value = productCode;
                cmd.Parameters["DRRT"].Value = userCode;
                cmd.Parameters["DRKY"].Value = userKey;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        var udcItem = new UdcItem
                        {
                            ProductCode = CastDbNull.To(reader["DRSY"], string.Empty).Trim(),
                            UserCode = CastDbNull.To(reader["DRRT"], string.Empty).Trim(),
                            UserCodeType = CastDbNull.To(reader["DTDL01"], string.Empty).Trim(),
                            UserValue = CastDbNull.To(reader["DRKY"], string.Empty).Trim(),
                            Description1 = CastDbNull.To(reader["DRDL01"], string.Empty).Trim(),
                            Description2 = CastDbNull.To(reader["DRDL02"], string.Empty).Trim(),
                            SpecialHandlingCode = CastDbNull.To(reader["DRSPHD"], string.Empty).Trim()
                        };
                        var updateDateId = CastDbNull.To<decimal>(reader["DRUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<decimal>(reader["DRUPMT"], 0);
                        udcItem.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return item;
        }
    }
}
