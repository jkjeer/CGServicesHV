using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServices.Common.Managers.E1;
using CGServices.Common.Managers.Enterprise;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Types.Response;
using CGServices.Common.Types.ShopFloor;
using NLog;

namespace CGServices.Common.Managers.ShopFloor
{
    public class ConfiguratorRuleDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public ConfiguratorRuleDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"ConfiguratorRuleDownloadManager.GetRelatedOrders", GetRelatedOrders()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, string tableName = null)
        {
            var response = new ProcessResponse("ConfiguratorRule Download");
            var configuratorRuleTable = new ConfiguratorRuleTable();
            var highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(e1Session, cnSFInt, plant, response.ProcessName) : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(e1Session));
            //var responseTime1 = new ResponseTime(response.ProcessName, "Get", e1Session.DbConnection.ConnectionString);
            var reader = GetReader(e1Session, processRunSpan, plant.PlantNum, tableName);
            //response.ResponseTimes.Add(responseTime1.Done());
            configuratorRuleTable.Add(reader);
            Logger.Trace("Rows: " + configuratorRuleTable.Rows.Count);
            if (configuratorRuleTable.Rows.Count > 0)
            {
                //var responseTime2 = new ResponseTime(response.ProcessName, "Download", cnSFInt.ConnectionString);
                var bulkResponse = BulkCopyManager.BulkTable(configuratorRuleTable.ExtendedProperties["Target"].ToString(), configuratorRuleTable, cnSFInt);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                if (useHighWaterMark)
                {
                    highWaterMark.EnterpriseDateTime = highWaterMark.StartDateTimeE1;
                    response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cnSFInt, highWaterMark));
                }
                //response.ResponseTimes.Add(responseTime2.Done());
            }
            return response;
        }

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

        public static IDataReader GetReader(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, string plantNum, string tableName = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
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
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND\r\n", "");
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
                //Logger.Debug(Logging.DbCmdString(cmd));
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));

                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
