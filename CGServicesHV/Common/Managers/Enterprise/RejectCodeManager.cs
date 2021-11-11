using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using IBM.Data.DB2.iSeries;
using System.Text;
using CGServices.Common.Managers.E1;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Utilities;
using NLog;

namespace CGServices.Common.Managers.Enterprise
{
    public class RejectCodeManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public RejectCodeManager(BusinessSystemSession e1Session)
        //{
        //}

        /*********************************************************************************
         * Get Reject Codes Dynamic
         *******************************************************************************/
        private static iDB2Command GetRejectCodesDynamic(IReadOnlyCollection<string> filters)
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
                        RCSY,
                        RCRT,
                        RCKY,
                        RCDL01,
                        RCMCU,
                        MCRP01,
                        DRDL01,
                        DRUPMJ,
                        DRUPMT
                    FROM 
                        F580027
	                    INNER JOIN F0006 
                        ON  MCMCU = RCMCU
	                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 
                        ON  RCSY = DRSY AND
				            RCRT = DRRT AND
				            TRIM(RCKY) = TRIM(DRKY)
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static RejectCodes GetRejectCodes(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string plantNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var rejectCodes = new RejectCodes();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("MCRP01 = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 3));
                    parameters.Last().Value = plantNum;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetRejectCodesDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetRejectCodesDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rejectCode = new RejectCode
                    {
                        Reject = CastDbNull.To(reader["RCKY"], string.Empty).Trim(),
                        RejectType = CastDbNull.To(reader["RCDL01"], string.Empty).Trim(),
                        RejectDesc = CastDbNull.To(reader["DRDL01"], string.Empty).Trim(),
                        ProductCode = CastDbNull.To(reader["RCSY"], string.Empty).Trim(),
                        UserCode = CastDbNull.To(reader["RCRT"], string.Empty).Trim(),
                        Workcenter = CastDbNull.To(reader["RCMCU"], string.Empty).Trim(),
                        PlantNum = CastDbNull.To(reader["MCRP01"], string.Empty).Trim(),
                        UpdateDate = DateTime.Now.ToLocalTime()
                    };
                    rejectCodes.Add(rejectCode);
                }
                reader.Close();
                Logger.Trace("ROWS: " + rejectCodes.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return rejectCodes;
        }
    }
}
