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
    public class ConfiguratorRuleManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public ConfiguratorRuleManager(BusinessSystemSession e1Session)
        //{
        //}

        /*********************************************************************************
         * Get ConfiguratorRules Dynamic
         *******************************************************************************/
        private static iDB2Command GetConfiguratorRulesDynamic(IReadOnlyCollection<string> filters)
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
                        RDRTBT,
                        RDTBLC,
                        RDMCU,
                        RDCTKID,
                        RDTSV1,
                        RDTSV2,
                        RDTSV3,
                        RDTSV4,
                        RDTSV5,
                        RDTSV6,
                        RDTSV7,
                        RDTSV8,
                        RDTSV9,
                        RDTSV0,
                        RDTSV11,
                        RDTSV12,
                        RDTSV13,
                        RDTSV14,
                        RDTSV15,
                        RDTSV16,
                        RDTSV17,
                        RDTSV18,
                        RDTSV19,
                        RDTSV20,
                        RDBSEQ,
                        RDEFFF,
                        RDEFFT,
                        RDTVAL,
                        RDUPAX,
                        RDITM,
                        RDQNTY,
                        RDUM,
                        RDURCD,
                        RDURDT,
                        RDURAT,
                        RDURAB,
                        RDURRF,
                        RDUSER,
                        RDPID,
                        RDJOBN,
                        RDUPMJ,
                        RDTDAY
                    FROM
	                    F3283
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static ConfiguratorRules GetConfiguratorRules(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, string plantNum, string tableName = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var configuratorRules = new ConfiguratorRules();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("RDMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }
                // Table Name
                if (!string.IsNullOrEmpty(tableName))
                {
                    filters.Add("RDTBLC = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("TableName", iDB2DbType.iDB2Graphic, 10));
                    parameters.Last().Value = tableName;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetConfiguratorRulesDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetConfiguratorRulesDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var counter = 0;
                while (reader.Read())
                {
                    var rule = new ConfiguratorRule
                    {
                        RulesTableType = CastDbNull.To(reader["RDRTBT"], string.Empty).Trim(),
                        RulesTableName = CastDbNull.To(reader["RDTBLC"], string.Empty).Trim(),
                        CostCenter = CastDbNull.To(reader["RDMCU"], string.Empty).Trim(),
                        ConfiguredTableKeyID = CastDbNull.To(reader["RDCTKID"], string.Empty).Trim(),
                        TableSegmentValue1 = CastDbNull.To(reader["RDTSV1"], string.Empty).Trim(),
                        TableSegmentValue2 = CastDbNull.To(reader["RDTSV2"], string.Empty).Trim(),
                        TableSegmentValue3 = CastDbNull.To(reader["RDTSV3"], string.Empty).Trim(),
                        TableSegmentValue4 = CastDbNull.To(reader["RDTSV4"], string.Empty).Trim(),
                        TableSegmentValue5 = CastDbNull.To(reader["RDTSV5"], string.Empty).Trim(),
                        TableSegmentValue6 = CastDbNull.To(reader["RDTSV6"], string.Empty).Trim(),
                        TableSegmentValue7 = CastDbNull.To(reader["RDTSV7"], string.Empty).Trim(),
                        TableSegmentValue8 = CastDbNull.To(reader["RDTSV8"], string.Empty).Trim(),
                        TableSegmentValue9 = CastDbNull.To(reader["RDTSV9"], string.Empty).Trim(),
                        TableSegmentValue0 = CastDbNull.To(reader["RDTSV0"], string.Empty).Trim(),
                        TableSegmentValue11 = CastDbNull.To(reader["RDTSV11"], string.Empty).Trim(),
                        TableSegmentValue12 = CastDbNull.To(reader["RDTSV12"], string.Empty).Trim(),
                        TableSegmentValue13 = CastDbNull.To(reader["RDTSV13"], string.Empty).Trim(),
                        TableSegmentValue14 = CastDbNull.To(reader["RDTSV14"], string.Empty).Trim(),
                        TableSegmentValue15 = CastDbNull.To(reader["RDTSV15"], string.Empty).Trim(),
                        TableSegmentValue16 = CastDbNull.To(reader["RDTSV16"], string.Empty).Trim(),
                        TableSegmentValue17 = CastDbNull.To(reader["RDTSV17"], string.Empty).Trim(),
                        TableSegmentValue18 = CastDbNull.To(reader["RDTSV18"], string.Empty).Trim(),
                        TableSegmentValue19 = CastDbNull.To(reader["RDTSV19"], string.Empty).Trim(),
                        TableSegmentValue20 = CastDbNull.To(reader["RDTSV20"], string.Empty).Trim(),
                        BubbleSequence = CastDbNull.To<decimal>(reader["RDBSEQ"], 0),
                        EffectiveFromDate = CastDbNull.To<decimal>(reader["RDEFFF"], 0),
                        EffectiveThruDate = CastDbNull.To<decimal>(reader["RDEFFT"], 0),
                        RulesTableValue = CastDbNull.To(reader["RDTVAL"], string.Empty).Trim(),
                        UnitPriceEntered = CastDbNull.To<decimal>(reader["RDUPAX"], 0),
                        IdentifierShortItem = CastDbNull.To<decimal>(reader["RDITM"], 0),
                        QtyRequiredStandard = CastDbNull.To<decimal>(reader["RDQNTY"], 0),
                        UnitOfMeasure = CastDbNull.To(reader["RDUM"], string.Empty).Trim(),
                        UserReservedCode = CastDbNull.To(reader["RDURCD"], string.Empty).Trim(),
                        UserReservedDate = CastDbNull.To<decimal>(reader["RDURDT"], 0),
                        UserReservedAmount = CastDbNull.To<decimal>(reader["RDURAT"], 0),
                        UserReservedNumber = CastDbNull.To<decimal>(reader["RDURAB"], 0),
                        UserReservedReference = CastDbNull.To(reader["RDURRF"], string.Empty).Trim(),
                        UserId = CastDbNull.To(reader["RDUSER"], string.Empty).Trim(),
                        ProgramId = CastDbNull.To(reader["RDPID"], string.Empty).Trim(),
                        WorkStationId = CastDbNull.To(reader["RDJOBN"], string.Empty).Trim()
                    };
                    var updateDateId = CastDbNull.To<decimal>(reader["RDUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["RDTDAY"], 0);
                    rule.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                    configuratorRules.Add(rule);
                    counter++;
                }
                reader.Close();
                Logger.Trace("ROWS: " + counter);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            Logger.Trace("CLOSE: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return configuratorRules;
        }
    }
}
