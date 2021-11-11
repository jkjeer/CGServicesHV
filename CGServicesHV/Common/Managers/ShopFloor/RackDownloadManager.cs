using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CGServices.Common.Managers.E1;
using CGServices.Common.Managers.Enterprise;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Types.Response;
using CGServices.Common.Types.ShopFloor;
using CGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServices.Common.Managers.ShopFloor
{
    public class RackDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public RackDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"WorkOrdManager.GetRelatedOrders", GetRelatedOrders()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal rackId)
        {
            var response = new ProcessResponse("Rack Download");
            var table = new RackTable();
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("SF", cn.ConnectionString));

                var step1 = new ProcessStep("E1 Extract F580559");
                try
                {
                    step1.Messages.Add(new Message("Span", span.Serialize()));
                    var reader = this.GetReader(session, cn, plant, span, rackId);
                    table.Add(reader);
                    step1.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);


                if (table.Rows.Count > 0)
                {

                    var step2 = new ProcessStep("SF Download");
                    try
                    {
                        var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                        if (bulkResponse.HasErrors())
                        {
                            step2.Errors.AddRange(bulkResponse.Errors);
                        }
                        response.SqlBulkCopyResponses.Add(bulkResponse);
                        step2.RowsAffected = (int)response.SqlBulkCopyResponses.Last().RowsCopied;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }
                    step2.Done();
                    response.AddStep(step2);

                    if (span.HighWaterMark.Exists)
                    {
                        var step3 = new ProcessStep("SF High Water Mark");
                        try
                        {
                            span.HighWaterMark.EnterpriseDateTime = table.MaxDate;
                            span.HighWaterMark.UniqueID = (long)table.MaxId;
                            response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, span.HighWaterMark));
                            step3.RowsAffected = response.SqlDmlResponses.Last().RowsAffected;
                        }
                        catch (Exception e)
                        {
                            step3.Errors.Add(new Error(e));
                        }
                        step3.Done();
                        response.AddStep(step3);
                    }
                }
                response.Done();
                Logging.InsertProcessLog(response);
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(e.Message);
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
                HLUKID,
                HLZ58RID,
                HLZ58RT,
                HLZ58HST,
                HLLOCN,
                HLURCD,
                HLUPMJ,
                HLTDAY
            FROM 
	            F580559
	        WHERE 
                {0} 1=1
            ORDER BY
                HLUKID", filterString.ToString());
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal rackId)
        {
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {

                // Plant - Required
                filters.Add("HLMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("HLZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("HLUPMJ", "HLTDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND\r\n");
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
         * High Water Mark
         ******************************************************************/
        public HighWaterMark GetHighWaterMark(BusinessSystemSession session, SqlConnection cn, Plant plant)
        {
            var mark = new HighWaterMark();
            try
            {
                mark = HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F580559");
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return mark;
        }
    }
}
