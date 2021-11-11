using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using CGServices.Common.Managers.E1;
using NLog;

namespace CGServices.Common.Managers.Enterprise
{
    public class UdcTypeManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /*********************************************************************************
         * Get UDC Types Dynamic
         *******************************************************************************/
        private static iDB2Command GetUdcTypesDynamic(IReadOnlyCollection<string> filters)
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
                CommandText = $@"
	                SELECT 
                        DTSY,
                        DTRT,
                        DTDL01,
                        DTCDL,
                        DTCNUM,
                        DTUPMJ,
                        DTUPMT
                    FROM
                        {ConfigurationManager.AppSettings["ControlLib"]}.F0004
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static UdcTypes GetUdcTypes(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string[] productCodes)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var udcTypes = new UdcTypes();
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
                    filters.Add("LEFT(DTSY,2) IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }
                // Process Run Span
                if ((bool)!processRunSpan?.IsEmpty)
                {
                    if (processRunSpan != null) filters.Add(E1TimeManager.GetE1DateTimeQueryString("DTUPMJ", "DTUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetUdcTypesDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetUdcTypesDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var udcType = new UdcType
                    {
                        ProductCode = CastDbNull.To(reader["DTSY"], string.Empty).Trim(),
                        UserCode = CastDbNull.To(reader["DTRT"], string.Empty).Trim(),
                        Description = CastDbNull.To(reader["DTDL01"], string.Empty).Trim(),
                        CodeLength = CastDbNull.To<decimal>(reader["DTCDL"], 0),
                        Numeric = CastDbNull.To(reader["DTCNUM"], string.Empty).Trim()
                    };
                    var updateDateId = CastDbNull.To<decimal>(reader["DTUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["DTUPMT"], 0);
                    udcType.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                    udcTypes.Add(udcType);
                }
                reader.Close();
                Logger.Trace("ROWS: " + udcTypes.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return udcTypes;
        }
    }
}
