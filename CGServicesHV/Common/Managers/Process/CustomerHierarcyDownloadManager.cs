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
    public class CustomerHierarcyDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public ConfiguratorRuleDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"UdcItemDownloadManager.GetUdcItem", GetUdcItem()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, string tableName = "")
        {
            var response = new ProcessResponse("Customer Hierarchy Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new CustomerHierarchyTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "CustomerHierarchy") : null;
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
                try
                {
                    var reader = GetReader(session, plant.PlantNum, processRunSpan, tableName);
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
                            bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
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
                if (useHighWaterMark)
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
        * EVENTS
        ********************************************************************************/
        private static iDB2Command GetReader(IReadOnlyCollection<string> filters)
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
	                ABAN8 AS CHILD_NO,
	                ABAN8 AS PARENT_NO,
	                0 AS LEVEL_NO,
	                --
	                ABALPH AS CHILD_NAME,
	                ABAT1 AS CHILD_TYPE_CD,
	                ABALPH AS PARENT_NAME,
	                ABAT1 AS PARENT_TYPE_CD,
                    CAST(ABAN8 AS VARCHAR(8)) AS SORT_KEY,
	                --
	                ABUPMJ AS E1_DATE_ID,
	                ABUPMT AS E1_TIME_ID
                FROM 
	                F0101
                WHERE
	                ABAT1 IN ('CP','C','CST','F')
                UNION ALL
                SELECT 
	                ABC.ABAN8 AS CHILD_NO,
	                CH1.MAPA8 AS PARENT_NO,
	                1 AS LEVEL_NO,
	                --
	                ABC.ABALPH AS CHILD_NAME,
	                ABC.ABAT1 AS CHILD_TYPE_CD,
	                ABP.ABALPH AS PARENT_NAME,
	                ABP.ABAT1 AS PARENT_TYPE_CD,
                    CAST(CH1.MAPA8 AS VARCHAR(8)) || '.' || CAST(ABC.ABAN8 AS VARCHAR(8)) AS SORT_KEY,
	                --
	                CH1.MAUPMJ,
	                CH1.MAUPMT
                FROM 
	                F0101 ABC
	                INNER JOIN F0150 CH1
	                ON	CH1.MAAN8 = ABC.ABAN8
	                INNER JOIN F0101 ABP
	                ON	ABP.ABAN8 = CH1.MAPA8
                WHERE
	                ABC.ABAT1 IN ('CP','C','CST','F') 
                UNION ALL
                SELECT 
	                ABC.ABAN8 AS CHILD_NO,
	                CH2.MAPA8 AS PARENT_NO,
	                2 AS LEVEL_NO,
	                --
	                ABC.ABALPH AS CHILD_NAME,
	                ABC.ABAT1 AS CHILD_TYPE_CD,
	                ABP.ABALPH AS PARENT_NAME,
	                ABP.ABAT1 AS PARENT_TYPE_CD,
                    CAST(CH2.MAPA8 AS VARCHAR(8)) || '.' || CAST(CH1.MAPA8 AS VARCHAR(8)) || '.' || CAST(ABC.ABAN8 AS VARCHAR(8)) AS SORT_KEY,
	                --
	                CH2.MAUPMJ,
	                CH2.MAUPMT
                FROM 
	                F0101 ABC
	                INNER JOIN F0150 CH1
	                ON	CH1.MAAN8 = ABC.ABAN8
	                INNER JOIN F0150 CH2
	                ON	CH2.MAAN8 = CH1.MAPA8
	                INNER JOIN F0101 ABP
	                ON	ABP.ABAN8 = CH2.MAPA8
                WHERE
	                ABC.ABAT1 IN ('CP','C','CST','F') 
                UNION ALL
                SELECT 
	                ABC.ABAN8 AS CHILD_NO,
	                CH3.MAPA8 AS PARENT_NO,
	                3 AS LEVEL_NO,
	                --
	                ABC.ABALPH AS CHILD_NAME,
	                ABC.ABAT1 AS CHILD_TYPE_CD,
	                ABP.ABALPH AS PARENT_NAME,
	                ABP.ABAT1 AS PARENT_TYPE_CD,
                    CAST(CH3.MAPA8 AS VARCHAR(8)) || '.' || CAST(CH2.MAPA8 AS VARCHAR(8)) || '.' || CAST(CH1.MAPA8 AS VARCHAR(8)) || '.' || CAST(ABC.ABAN8 AS VARCHAR(8)) AS SORT_KEY,
	                --
	                CH3.MAUPMJ,
	                CH3.MAUPMT
                FROM 
	                F0101 ABC
	                INNER JOIN F0150 CH1
	                ON	CH1.MAAN8 = ABC.ABAN8
	                INNER JOIN F0150 CH2
	                ON	CH2.MAAN8 = CH1.MAPA8
	                INNER JOIN F0150 CH3
	                ON	CH3.MAAN8 = CH2.MAPA8
	                INNER JOIN F0101 ABP
	                ON	ABP.ABAN8 = CH3.MAPA8
                WHERE
	                ABC.ABAT1 IN ('CP','C','CST','F') 
                UNION ALL
                SELECT 
	                ABC.ABAN8 AS CHILD_NO,
	                CH4.MAPA8 AS PARENT_NO,
	                4 AS LEVEL_NO,
	                --
	                ABC.ABALPH AS CHILD_NAME,
	                ABC.ABAT1 AS CHILD_TYPE_CD,
	                ABP.ABALPH AS PARENT_NAME,
	                ABP.ABAT1 AS PARENT_TYPE_CD,
                    CAST(CH4.MAPA8 AS VARCHAR(8)) || '.' || CAST(CH3.MAPA8 AS VARCHAR(8)) || '.' || CAST(CH2.MAPA8 AS VARCHAR(8)) || '.' || CAST(CH1.MAPA8 AS VARCHAR(8)) || '.' || CAST(ABC.ABAN8 AS VARCHAR(8)) AS SORT_KEY,
	                --
	                CH4.MAUPMJ,
	                CH4.MAUPMT
                FROM 
	                F0101 ABC
	                INNER JOIN F0150 CH1
	                ON	CH1.MAAN8 = ABC.ABAN8
	                INNER JOIN F0150 CH2
	                ON	CH2.MAAN8 = CH1.MAPA8
	                INNER JOIN F0150 CH3
	                ON	CH3.MAAN8 = CH2.MAPA8
	                INNER JOIN F0150 CH4
	                ON	CH4.MAAN8 = CH3.MAPA8
	                INNER JOIN F0101 ABP
	                ON	ABP.ABAN8 = CH4.MAPA8
                WHERE
	                ABC.ABAT1 IN ('CP','C','CST','F')"
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession session, string plantNum, ProcessRunSpan processRunSpan, string tableName)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                var cmd = GetReader(filters);
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
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }
}
