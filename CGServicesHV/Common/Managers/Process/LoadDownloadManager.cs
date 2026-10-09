using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class LoadDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public LoadDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal loadNum, List<string> statusCodes)
        {
            var response = new ProcessResponse("Load Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));

            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var stepSpan = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "Load") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                stepSpan.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                stepSpan.Errors.Add(new Error(e));
            }
            finally
            {
                stepSpan.Done();
                response.AddStep(stepSpan);
            }

            if (!response.HasErrors())
            {
                var table = new LoadTable();
                /********************************************************/
                var stepGet = new ProcessStep("Get Source Data");
                try
                {
                    if (useHighWaterMark)
                    {
                        var reader = GetLoadLedger(session, processRunSpan, plant.PlantNum, loadNum, statusCodes);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        var reader = GetLoad(session, processRunSpan, plant.PlantNum, loadNum, statusCodes);
                        table.Add(reader, plant);
                    }
                    stepGet.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    stepGet.Errors.Add(new Error(e));
                }
                stepGet.Done();
                response.AddStep(stepGet);

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var stepBulk = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse =
                                BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),
                                    table, sqlCns["PLANT.INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        stepBulk.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepBulk.RowsAffected = (int)bulkResponse.RowsCopied;
                        stepBulk.SqlBulkCopyResponses.Add(bulkResponse);
                        stepBulk.Done();
                        response.RowsAffected = stepBulk.RowsAffected;
                        response.AddStep(stepBulk);
                    }
                }

                /********************************************************/
                if (useHighWaterMark && !response.HasErrors())
                {
                    var stepHighWaterSave = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            stepHighWaterSave.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, table.MaxDate.AddMinutes(-plant.BusinessSystemOffset), (long)table.MaxId));
                        }
                    }
                    catch (Exception e)
                    {
                        stepHighWaterSave.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepHighWaterSave.Done();
                        response.AddStep(stepHighWaterSave);
                    }
                }
            }
            response.Close();
            return response;
        }

        /********************************************************************************
        * Load Ledger - high water mark
        ********************************************************************************/
        public static IDataReader GetLoadLedger(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal loadNum, List<string> statusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {

                // Cannot run wide Open
                if (!statusCodes.Any() && processRunSpan.IsEmpty && loadNum == 0)
                {
                    filters.Add("0 = 1 AND ");
                }

                // Plant - Required
                filters.Add("LYMCU = ? AND ");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Process Run Span
                if (processRunSpan.HighWaterMark.Exists)
                {
                    filters.Add("LYUKID > ? AND ");
                    parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = processRunSpan.HighWaterMark.UniqueID;
                }

                // Load Num
                if (loadNum > 0)
                {
                    filters.Add("LYLDNM = ? AND ");
                    parameters.Add(new iDB2Parameter("LDNM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = loadNum;
                }

                // Status Codes
                if (statusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in statusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("LDS", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("LYZ58LDS IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND ");
                }

                // Command
                var cmd = GetLoadLedger(filters);
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

        private static iDB2Command GetLoadLedger(IReadOnlyCollection<string> filters)
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
                    LYLDNM AS LDNM,
                    LYMCU AS MCU,
                    LYZ58LDS AS Z58LDS,
                    LYDRQJ AS DRQJ,
                    LYDRQT AS DRQT,
                    LYLOAD AS LOAD,
                    LYTMLS AS TMLS,
                    LYAN8 AS AN8,
                    LYSHAN AS SHAN,
                    LYZ58CMT1 AS Z58CMT1,
                    LYZ58CMT2 AS Z58CMT2,
                    LYZ58CMT3 AS Z58CMT3,
                    LYCARS AS CARS,
                    LYZ58TN AS Z58TN,
                    LYZ58TT AS Z58TT,
                    LYFRTH AS FRTH,
                    LYZ58RKN AS Z58RKN,
                    LYZ58RKL AS Z58RKL,
                    LYZ58FRTB AS Z58FRTB,
                    LYZ58FRTO AS Z58FRTO,
                    LYZ58EDTA AS Z58EDTA,
                    LYZ58ETMA AS Z58ETMA,
                    LYLDDT AS LDDT,
                    LYZ58TMLC AS Z58TMLC,
                    LYADDJ AS ADDJ,
                    LYADTM AS ADTM,
                    LYCNDJ AS CNDJ,
                    LYZ58TMCX AS Z58TMCX,
                    LYBOLN AS BOLN,
                    LYZ58MBOL AS Z58MBOL,
                    LYUWUM AS UWUM,
                    LYZ58PLNWT AS Z58PLNWT,
                    LYZ58GWTT AS Z58GWTT,
                    LYTOWT AS TOWT,
                    LYZ58TSF AS Z58TSF,
                    LYZ58SLOC AS Z58SLOC,
                    LYZ58VLOC AS Z58VLOC,
                    LYZ58EXPRT AS Z58EXPRT,
                    LYZ58BKNG# AS Z58BKNG#,
                    LYUSER AS USER,
                    LYPID AS PID,
                    LYUKID AS UKID,
                    LYIOPR AS IOPR,
                    LYMUPM AS UPMJ,
                    LYUPMT AS TDAY
                FROM 
	                F5801509
	            WHERE 
                    {filterString.Remove(filterString.Length - 4, 3)}
                FOR READ ONLY"
            };
            return cmd;
        }

        /********************************************************************************
        * Load
        ********************************************************************************/
        public static IDataReader GetLoad(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal loadNum, List<string> statusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {

                // Cannot run wide Open
                if (!statusCodes.Any() && processRunSpan.IsEmpty && loadNum == 0)
                {
                    filters.Add("0 = 1 AND ");
                }

                // Plant - Required
                filters.Add("LHMCU = ? AND ");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Rack Id
                if (loadNum > 0)
                {
                    filters.Add("LHLDNM = ? AND ");
                    parameters.Add(new iDB2Parameter("LDNM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = loadNum;
                }

                // Status Codes
                if (statusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in statusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("LDS", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("LHZ58LDS IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND ");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("LHUPMJ", "LHTDAY", processRunSpan.E1TimeIdSpan) + " AND ");
                }

                // Command
                var cmd = GetLoad(filters);
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

        private static iDB2Command GetLoad(IReadOnlyCollection<string> filters)
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
                    LHLDNM AS LDNM,
                    LHMCU AS MCU,
                    LHZ58LDS AS Z58LDS,
                    LHDRQJ AS DRQJ,
                    LHDRQT AS DRQT,
                    LHLOAD AS LOAD,
                    LHTMLS AS TMLS,
                    LHAN8 AS AN8,
                    LHSHAN AS SHAN,
                    LHZ58CMT1 AS Z58CMT1,
                    LHZ58CMT2 AS Z58CMT2,
                    LHZ58CMT3 AS Z58CMT3,
                    LHCARS AS CARS,
                    LHZ58TN AS Z58TN,
                    LHZ58TT AS Z58TT,
                    LHFRTH AS FRTH,
                    LHZ58RKN AS Z58RKN,
                    LHZ58RKL AS Z58RKL,
                    LHZ58FRTB AS Z58FRTB,
                    LHZ58FRTO AS Z58FRTO,
                    LHZ58EDTA AS Z58EDTA,
                    LHZ58ETMA AS Z58ETMA,
                    LHLDDT AS LDDT,
                    LHZ58TMLC AS Z58TMLC,
                    LHADDJ AS ADDJ,
                    LHADTM AS ADTM,
                    LHCNDJ AS CNDJ,
                    LHZ58TMCX AS Z58TMCX,
                    LHBOLN AS BOLN,
                    LHZ58MBOL AS Z58MBOL,
                    LHUWUM AS UWUM,
                    LHZ58PLNWT AS Z58PLNWT,
                    LHZ58GWTT AS Z58GWTT,
                    LHTOWT AS TOWT,
                    LHZ58TSF AS Z58TSF,
                    LHZ58SLOC AS Z58SLOC,
                    LHZ58VLOC AS Z58VLOC,
                    LHZ58EXPRT AS Z58EXPRT,
                    LHZ58BKNG# AS Z58BKNG#,
                    LHUSER AS USER,
                    LHPID AS PID,
                    CAST(0 AS DECIMAL(15,0)) AS UKID,
                    '' AS IOPR,
                    LHUPMJ AS UPMJ,
                    LHTDAY AS TDAY
                FROM 
	                F580150
	            WHERE 
                    {filterString.Remove(filterString.Length - 4, 3)}
                FOR READ ONLY"
            };
            return cmd;
        }

    }
}
