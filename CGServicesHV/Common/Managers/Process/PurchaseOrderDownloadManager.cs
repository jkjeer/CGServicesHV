using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

namespace CGServicesHV.Common.Managers.Process
{
    public class PurchaseOrderDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public PurchaseOrderDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"PurchaseOrderDownloadManager.GetReceivedDate", GetReceivedDate()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal orderNum)
        {
            var response = new ProcessResponse("Purchase Order Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new PurchaseOrderTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "PurchaseOrder") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            if (!response.HasErrors())
            {
                if (processRunSpan.HighWaterMark.Exists)
                {
                    /********************************************************/
                    var step2a = new ProcessStep("Get Source Data");
                    try
                    {
                        //var reader = GetPoLedger(session, processRunSpan, plant.PlantNum, orderNum);
                        var reader = GetPoCurrent(session, processRunSpan, plant.PlantNum, orderNum);
                        table.Add(reader, plant);
                        step2a.RowsAffected = table.Rows.Count;
                    }
                    catch (Exception e)
                    {
                        step2a.Errors.Add(new Error(e));
                    }
                    step2a.Done();
                    response.AddStep(step2a);
                }
                else
                {
                    /********************************************************/
                    var step3a = new ProcessStep("Get Source Data");
                    try
                    {
                        var reader = GetPoCurrent(session, processRunSpan, plant.PlantNum, orderNum);
                        table.Add(reader, plant);
                        step3a.RowsAffected = table.Rows.Count;
                    }
                    catch (Exception e)
                    {
                        step3a.Errors.Add(new Error(e));
                    }
                    step3a.Done();
                    response.AddStep(step3a);
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        response.RowsAffected = table.Rows.Count;
                        bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.RowsAffected = (int)bulkResponse.RowsCopied;
                        step3.SqlBulkCopyResponses.Add(bulkResponse);
                        step3.Done();
                        response.RowsAffected = step3.RowsAffected;
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                if (useHighWaterMark && table.Rows.Count > 0)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime));
                        step4.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }
            }
            response.Close();
            return response;
        }

        /********************************************************************************
        * RACK LEDGER
        ********************************************************************************/
        private static iDB2Command GetPoLedger(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
	                    OLUKID,
	                    OLMCU,
	                    OLDOCO,
	                    OLDCTO,
	                    OLLNID,
	                    OLKCOO,
	                    OLAN8,
	                    OLSHAN,
	                    OLTRDJ,
	                    OLDRQJ,
	                    OLCNDJ,
                        OLADDJ,
	                    OLITM,
                        OLLOTN,
	                    OLLNTY,
	                    OLNXTR,
	                    OLLTTR,
	                    OLUORG,
	                    OLUREC,
	                    OLPRRC,
	                    OLAEXP,
	                    OLGLC,
	                    OLUPMJ,
	                    OLTDAY,
                        CMCFGID
                    FROM 
	                    F43199
                        LEFT OUTER JOIN F3201
                        ON  CMKCOO = OLKCOO AND
                            CMDOCO = OLDOCO AND
                            CMDCTO = OLDCTO AND
                            CMLNID = OLLNID
	                WHERE 
                        {filterString}
	                    OLGLC IN ('GLAS','GLAF','GLAX','GLAZ','GLAC','COMA')"
                //ORDER BY OLUKID
                //FETCH FIRST 50000 ROWS ONLY"
            };
            return cmd;
        }

        public static IDataReader GetPoLedger(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal orderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("OLMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // OrderNum
                if (orderNum > 0)
                {
                    filters.Add("OLDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("OLUPMJ", "OLTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                var cmd = GetPoLedger(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /********************************************************************************
        * RACK CURRENT
        ********************************************************************************/
        private static iDB2Command GetPoCurrent(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT * FROM (
                        SELECT 
	                        CAST(0 AS DECIMAL(8,0)) AS OLUKID,
	                        PDMCU AS OLMCU,
	                        PDDOCO AS OLDOCO,
	                        PDDCTO AS OLDCTO,
	                        PDLNID AS OLLNID,
	                        PDKCOO AS OLKCOO,
	                        PDAN8 AS OLAN8,
	                        PDSHAN AS OLSHAN,
	                        PDTRDJ AS OLTRDJ,
	                        PDDRQJ AS OLDRQJ,
	                        PDCNDJ AS OLCNDJ,
                            PDADDJ AS OLADDJ,
	                        PDITM AS OLITM,
                            PDLOTN AS OLLOTN,
	                        PDLNTY AS OLLNTY,
	                        PDNXTR AS OLNXTR,
	                        PDLTTR AS OLLTTR,
	                        PDUORG AS OLUORG,
	                        PDUREC AS OLUREC,
	                        PDPRRC AS OLPRRC,
	                        PDAEXP AS OLAEXP,
	                        PDGLC AS OLGLC,
	                        PDUPMJ AS OLUPMJ,
	                        PDTDAY AS OLTDAY,
                            CMCFGID,
                            ROW_NUMBER() OVER (PARTITION BY PDKCOO, PDDOCO, PDDCTO, PDLNID ORDER BY PDUPMJ, PDTDAY) AS RN
                        FROM 
	                        F4311                   
                            LEFT OUTER JOIN F3201
                            ON  CMKCOO = PDKCOO AND
                                CMDOCO = PDDOCO AND
                                CMDCTO = PDDCTO AND
                                CMLNID = PDLNID
	                    WHERE 
                            {filterString}
	                        PDGLC IN ('GLAS','GLAF','GLAX','GLAZ','GLAC','COMA')
                    ) A
                    WHERE A.RN = 1"
            };
            return cmd;
        }

        public static IDataReader GetPoCurrent(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal orderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("PDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Order Num
                if (orderNum > 0)
                {
                    filters.Add("PDDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("PDUPMJ", "PDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                var cmd = GetPoCurrent(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
