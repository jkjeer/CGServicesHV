using System;
using System.Collections.Generic;
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
    public class EmployeeManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EmployeeManager(BusinessSystemSession e1Session)
        {
        }

        /*********************************************************************************
         * Get Employees Dynamic
         *******************************************************************************/
        private static iDB2Command GetEmployeesDynamic(IReadOnlyCollection<string> filters)
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
                        YAHMCO,
                        MCDL01, 
                        YAUN, 
                        YAHMCU,
                        YAMCU, 
                        YAAN8, 
                        YAALPH, 
                        YAPAST,
                        YAUPMJ,
                        YADTSP
                    FROM
                        F060116
                        LEFT OUTER JOIN F0006
                        ON MCMCU = YAHMCU
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static Employees GetEmployees(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string plantNum, decimal addressNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var employees = new Employees();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Employment Status
                filters.Add("YAPAST = '0' AND \r\n");
                // Plant Num
                if (plantNum != null && !plantNum.Equals("0"))
                {
                    filters.Add("YAUN = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 6));
                    parameters.Last().Value = plantNum;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("YAUPMJ", "YADTSP", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetEmployeesDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetEmployeesDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var employee = new Employee
                    {
                        AddressNum = CastDbNull.To<decimal>(reader["YAAN8"], 0),
                        Name = CastDbNull.To(reader["YAALPH"], string.Empty).Trim(),
                        HomeCompany = CastDbNull.To(reader["YAHMCO"], string.Empty).Trim(),
                        HomeCompanyDesc = CastDbNull.To(reader["MCDL01"], string.Empty).Trim(),
                        Company = "",
                        HomeCostCenter = CastDbNull.To(reader["YAHMCU"], string.Empty).Trim(),
                        CostCenter = CastDbNull.To(reader["YAMCU"], string.Empty).Trim(),
                        PayStatus = CastDbNull.To(reader["YAPAST"], string.Empty).Trim(),
                        UnionCode = CastDbNull.To(reader["YAUN"], string.Empty).Trim()
                    };
                    // CastDbNull.To<String>(reader["YACO"], String.Empty).Trim();
                    var updateDateId = CastDbNull.To<decimal>(reader["YAUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["YADTSP"], 0);
                    employee.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                    employees.Add(employee);
                }
                reader.Close();
                Logger.Trace("ROWS: " + employees.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return employees;
        }
    }
}
