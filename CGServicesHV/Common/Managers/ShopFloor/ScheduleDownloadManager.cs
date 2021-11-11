using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.ShopFloor
{
    public class ScheduleDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ScheduleDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"ScheduleDownloadManager.GetQuantity", GetQuantity()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant, TimeSpan span, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Schedule Download");
            var table = new ScheduleTable();
            var table1 = new ScheduleLineTable();

            // Process Run Span
            var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F5834A01") : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));

            //var rt1 = new ResponseTime("Get From E1");
            var reader = this.GetReader(session, cn, plant, processRunSpan);
            table.Add(reader);
            //response.ResponseTimes.Add(rt1.Done());

            //var rt2 = new ResponseTime("Enrich Quantity");
            GetQuantity(session, plant, table);
            //response.ResponseTimes.Add(rt2.Done());

            //var scheduleDataManager = new ScheduleDataManager(session);
            //foreach (DataRow dr in table.Rows)
            //{
            //    var scheduleData = scheduleDataManager.GetScheduleData(session, plant.PlantNum, (decimal)dr["SCHEDULE_NUM"],
            //        (DateTime)dr["SCHEDULE_DATE"], (string)dr["BATCH_NUM"]);
            //    table1.Add(scheduleData);
            //}

            if (table.Rows.Count > 0)
            {
                // Bulk Copy
                var bulkResponse1 = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse1);

                var bulkResponse2 = BulkCopyManager.BulkTable(table1.ExtendedProperties["Target"].ToString(), table1, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse2);

                // Set High Water Mark
                if (mark != null)
                {
                    mark.EnterpriseDateTime = table.MaxDate;
                    //response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, mark));
                    response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(cn, plant, mark, mark.EnterpriseDateTime));
                }
            }
            return response;
        }

        public iDB2Command GetReader(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = String.Format(@"
            SELECT
                *
            FROM (
                SELECT 
                    S1MCU,
                    S1Z58SCN,
                    S1Z58SCD,
                    S1Z58SCSTS,
                    S1Z58SCTYP,
                    S1Z58SFS,
                    DRDL01 AS S1Z58SFS_DESC,
                    S1Z58SCDDJ,
                    S1UPMJ,
                    S1TDAY,
                    S6EDBT,
                    ROW_NUMBER() OVER (PARTITION BY S1MCU, S1Z58SCN, S1Z58SCD ORDER BY S1UPMJ DESC, S1TDAY DESC) AS RN
                FROM 
	                F5834A01
                    INNER JOIN F580020
                    ON  S6MCU = S1MCU AND
                        S6Z58SCN = S1Z58SCN AND
                        S6Z58SCD = S1Z58SCD 
                    LEFT OUTER JOIN {1}.F0005
                    ON  DRSY = '58' AND
                        DRRT = 'SY' AND
                        TRIM(DRKY) = TRIM(S1Z58SFS)
	            WHERE 
                    {0} 1=1) A
            WHERE 
                A.RN = 1", filterString.ToString(), ConfigurationManager.AppSettings["ControlLib"]);
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span)
        {
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {

                // Plant - Required
                filters.Add("S1MCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("S1UPMJ", "S1TDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND \r\n");
                }

                cmd = GetReader(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));

                _logger.Trace("Start Get Orders");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
          * Quantity
          ******************************************************************/
        public static iDB2Command GetQuantity()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                COUNT(*) AS LINES,
                SUM(S5Z58QTY) AS UNITS
            FROM
                F5834A11
            WHERE
                S5MCU = ? AND
                S5Z58SCN = ? AND
                S5Z58SCD = ?";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN"].Precision = 5;
            cmd.Parameters["SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCD"].Precision = 6;
            cmd.Parameters["SCD"].Scale = 0;
            return cmd;
        }

        public void GetQuantity(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["ScheduleDownloadManager.GetQuantity"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal) dr["SCHEDULE_NUM"];
                    cmd.Parameters[2].Value = E1TimeManager.CastToE1Date((DateTime) dr["SCHEDULE_DATE"]);
                    _logger.Info(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["LINES"] = CastDbNull.To<int>(reader["LINES"], 0);
                            dr["QUANTITY"] = CastDbNull.To<decimal>(reader["UNITS"], 0);
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
          * Schedule Lines
          ******************************************************************/
        public static iDB2Command GetScheduleLines()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                COUNT(*) AS LINES,
                SUM(S5Z58QTY) AS UNITS
            FROM
                F580021
                LEFT OUTER JOIN F580024
                ON  S8EDBT = S7EDBT AND
                    S8MCU = S7MCU AND
                    S8EDLN = S7EDLN AND
                    S8Z58SCN = S7Z58SCN AND
                    S8Z58SCSN = S7Z58SCSN
            WHERE
                S7MCU = ? AND
                S7EDBT = ? AND
                S7Z58SCN = ?";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("EDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN"].Precision = 5;
            cmd.Parameters["SCN"].Scale = 0;
            return cmd;
        }

    }
}
