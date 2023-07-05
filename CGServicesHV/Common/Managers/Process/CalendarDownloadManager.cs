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
    public static class CalendarDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        ///********************************************************************************
        //* CONSTRUCTOR
        //********************************************************************************/
        //public CalendarDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"CalendarDownloadManager.GetReceivedDate", GetReceivedDate()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Calendar Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "Calendar") : null;
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

                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                var table = new CalendarTable();
                try
                {
                    var reader = GetCalendar(session, processRunSpan, plant);
                    table.Add(reader, plant);
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
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
                if (useHighWaterMark && !response.HasErrors())
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"],
                                plant, highWaterMark, table.MaxDate.AddMinutes(-plant.BusinessSystemOffset)));
                        }
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
        * RACK ASSET
        ********************************************************************************/
        private static iDB2Command GetCalendar(IReadOnlyCollection<string> filters)
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
	                CZMMCU,
	                CZCTRY,
	                CZYR,
	                CZMT,
	                NAME,
	                VAL,
	                CZWDCT,
				    CZUPMJ,
				    CZTDAY
                FROM 
                    F0007
                    CROSS JOIN TABLE (VALUES
                    (1, CAST(CZTD01 AS GRAPHIC(1))) ,
                    (2, CAST(CZTD02 AS GRAPHIC(1))) ,
                    (3, CAST(CZTD03 AS GRAPHIC(1))) ,
                    (4, CAST(CZTD04 AS GRAPHIC(1))) ,
                    (5, CAST(CZTD05 AS GRAPHIC(1))) ,
                    (6, CAST(CZTD06 AS GRAPHIC(1))) ,
                    (7, CAST(CZTD07 AS GRAPHIC(1))) ,
                    (8, CAST(CZTD08 AS GRAPHIC(1))) ,
                    (9, CAST(CZTD09 AS GRAPHIC(1))) ,
                    (10, CAST(CZTD10 AS GRAPHIC(1))) ,
                    (11, CAST(CZTD11 AS GRAPHIC(1))) ,
                    (12, CAST(CZTD12 AS GRAPHIC(1))) ,
                    (13, CAST(CZTD13 AS GRAPHIC(1))) ,
                    (14, CAST(CZTD14 AS GRAPHIC(1))) ,
                    (15, CAST(CZTD15 AS GRAPHIC(1))) ,
                    (16, CAST(CZTD16 AS GRAPHIC(1))) ,
                    (17, CAST(CZTD17 AS GRAPHIC(1))) ,
                    (18, CAST(CZTD18 AS GRAPHIC(1))) ,
                    (19, CAST(CZTD19 AS GRAPHIC(1))) ,
                    (20, CAST(CZTD20 AS GRAPHIC(1))) ,
                    (21, CAST(CZTD21 AS GRAPHIC(1))) ,
                    (22, CAST(CZTD22 AS GRAPHIC(1))) ,
                    (23, CAST(CZTD23 AS GRAPHIC(1))) ,
                    (24, CAST(CZTD24 AS GRAPHIC(1))) ,
                    (25, CAST(CZTD25 AS GRAPHIC(1))) ,
                    (26, CAST(CZTD26 AS GRAPHIC(1))) ,
                    (27, CAST(CZTD27 AS GRAPHIC(1))) ,
                    (28, CAST(CZTD28 AS GRAPHIC(1))) ,
                    (29, CAST(CZTD29 AS GRAPHIC(1))) ,
                    (30, CAST(CZTD30 AS GRAPHIC(1))) ,
                    (31, CAST(CZTD31 AS GRAPHIC(1))) 
                        ) AS T (NAME, VAL)
                WHERE
                    {filterString}
				    CZSHFT = '' AND 
				    VAL != ''
                ORDER BY
                   1,2,3,4
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetCalendar(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("CZMMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("CZUPMJ", "CZTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                var cmd = GetCalendar(filters);
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
