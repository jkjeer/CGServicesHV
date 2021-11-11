using System;
using System.Collections.Generic;
using System.Configuration;
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
using CGServices.Common.Utilities;
using NLog;

namespace CGServices.Common.Managers.ShopFloor
{
    public class RejectCodeDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public RejectCodeDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"RejectCodeDownloadManager.GetRelatedOrders", GetRelatedOrders()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("RejectCode Download");
            var rejectCodeTable = new RejectCodeTable();
            var highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(e1Session, cnSFInt, plant, response.ProcessName) : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(e1Session));
            //var responseTime1 = new ResponseTime(response.ProcessName, "Get", e1Session.DbConnection.ConnectionString);
            var reader = GetReader(e1Session, processRunSpan, plant.PlantNum);
            //response.ResponseTimes.Add(responseTime1.Done());
            rejectCodeTable.Add(reader);
            if (rejectCodeTable.Rows.Count > 0)
            {
                //var responseTime2 = new ResponseTime(response.ProcessName, "Download", cnSFInt.ConnectionString);
                var bulkResponse = BulkCopyManager.BulkTable(rejectCodeTable.ExtendedProperties["Target"].ToString(), rejectCodeTable, cnSFInt);
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

        public static IDataReader GetReader(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string plantNum = null)
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
                Logger.Debug(Logging.DbCmdString(cmd));
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