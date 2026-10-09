using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using Microsoft.Data.SqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Managers.Process
{
    public static class ConfiguredUnitAttributeDownloadManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark, decimal configurationId, List<string> rackStatusCodes, bool archived, bool missing)
        {
            var response = new ProcessResponse("Configured Unit Attribute Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new ConfiguredUnitAttributeTable();
            var tableName = table.ExtendedProperties["Target"].ToString();
            var batchSize = int.Parse((string)table.ExtendedProperties["BatchSize"]);
            var maxDate = new DateTime(1900, 1, 1);

            /********************************************************/
            var stepSpan = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ConfiguredUnitAttribute") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, timeSpanEnd);
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

            var currentIds = new List<decimal>();
            var currentIdDict = new Dictionary<decimal, DateTime>();
            /********************************************************/
            if (!missing)
            {
                var stepBusinessIds = new ProcessStep("Get Business Ids");
                try
                {
                    var reader = GetBusinessIds(session, plant, processRunSpan, configurationId, rackStatusCodes);
                    while (reader.Read())
                    {
                        currentIds.Add(reader.Get<decimal>("CMCFGID"));
                        currentIdDict.Add(reader.Get<decimal>("CMCFGID"), E1TimeManager.CastToDateTime(reader.Get<decimal>("CMUPMJ"), reader.Get<decimal>("CMTDAY")));
                    }
                    reader.Close();
                    stepBusinessIds.RowsAffected = currentIds.Count();
                }
                catch (Exception e)
                {
                    stepBusinessIds.Errors.Add(new Error(e));
                }
                finally
                {
                    stepBusinessIds.Done();
                    response.AddStep(stepBusinessIds);
                }
            }

            /********************************************************/
            if (!missing)
            {
                var stepSalesLineBusinessIds = new ProcessStep("Get Sales Line Business Ids");
                try
                {
                    var reader = GetSalesLineBusinessIds(session, plant, processRunSpan, configurationId, rackStatusCodes);
                    while (reader.Read())
                    {
                        if (!currentIds.Contains(reader.Get<decimal>("CMCFGID")))
                        {
                            currentIds.Add(reader.Get<decimal>("CMCFGID"));
                            currentIdDict.Add(reader.Get<decimal>("CMCFGID"), E1TimeManager.CastToDateTime(reader.Get<decimal>("SDUPMJ"), reader.Get<decimal>("SDTDAY")));
                        }
                    }
                    reader.Close();
                    stepSalesLineBusinessIds.RowsAffected = currentIds.Count();
                }
                catch (Exception e)
                {
                    stepSalesLineBusinessIds.Errors.Add(new Error(e));
                }
                finally
                {
                    stepSalesLineBusinessIds.Done();
                    response.AddStep(stepSalesLineBusinessIds);
                }
            }

            /********************************************************/
            if (missing)
            {
                var stepMissingIds = new ProcessStep("Get Missing Ids");
                try
                {
                    var reader = GetMissingIds(sqlCns["PLANT.SHOPFLOOR"], plant, processRunSpan);
                    while (reader.Read())
                    {
                        currentIds.Add(reader.Get<decimal>("ConfigurationId"));
                    }
                    reader.Close();
                    stepMissingIds.RowsAffected = currentIds.Count();
                }
                catch (Exception e)
                {
                    stepMissingIds.Errors.Add(new Error(e));
                }
                finally
                {
                    stepMissingIds.Done();
                    response.AddStep(stepMissingIds);
                }
            }

            /********************************************************/
            if (currentIds.Count > 0)
            {
                var stepTable = new ProcessStep("Bulk Current Data");
                try
                {
                    logger.Info($"{plant.PlantCode} Total Ids={currentIds.Count}");
                    var cmd = GetAttributes();
                    cmd.Connection = session.DbConnection;
                    var i = 0;
                    foreach (var id in currentIds)
                    {
                        i++;
                        cmd.Parameters[0].Value = id;
                        var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                        while (reader.Read())
                        {
                            table.Add((IDataRecord)reader, plant);
                        }
                        reader.Close();
                        if (i % 1000 == 0)
                        {
                            logger.Info($"{plant.PlantCode} Ids={i}");
                            stepTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                            logger.Info($"{plant.PlantCode} Rows={table.Rows.Count}");
                            maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                            table.Rows.Clear();
                        }
                    }
                    if (table.Rows.Count > 0)
                    {
                        logger.Info($"{plant.PlantCode} Ids={i}");
                        stepTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                        logger.Info($"{plant.PlantCode} Rows={table.Rows.Count}");
                        maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                        table.Rows.Clear();
                    }
                    stepTable.RowsAffected = i;
                }
                catch (Exception e)
                {
                    stepTable.Errors.Add(new Error(e));
                }
                finally
                {
                    stepTable.Done();
                    response.AddStep(stepTable);
                }
            }

            /********************************************************/
            if (useHighWaterMark && currentIds.Count > 0)
            {
                var stepMark = new ProcessStep("Update High Water Mark");
                try
                {
                    //stepMark.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, maxDate.AddMinutes(-plant.BusinessSystemOffset)));
                    stepMark.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, currentIdDict.Aggregate((x, y) => x.Value > y.Value ? x : y).Value));
                }
                catch (Exception e)
                {
                    stepMark.Errors.Add(new Error(e));
                }
                finally
                {
                    stepMark.Done();
                    response.AddStep(stepMark);
                }
            }

            response.Close();
            return response;
        }

        /********************************************************************************
        * Business System Ids
        ********************************************************************************/
        public static IDataReader GetBusinessIds(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan, decimal id, List<string> rackStatusCodes)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add(new Sql.SqlFilter("CMEMCU = ?", "AND"));
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Dates
                if (!processRunSpan.IsEmpty)
                {
                    var dtList = new List<string[]>
                    {
                        new string[] { "CMUPMJ", "CMTDAY" }
                    };
                    filters.Add(new Sql.SqlFilter($"{E1TimeManager.GetE1DateFilterString(dtList, processRunSpan.StartDateTime, processRunSpan.EndDateTime)}", "AND"));
                }

                // Configuration Id
                if (id > 0)
                {
                    filters.Add(new Sql.SqlFilter("CMCFGID = ?", "AND"));
                    parameters.Add(new iDB2Parameter("CFGID1", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = id;
                }

                // Rack Status Codes
                if (rackStatusCodes.Any())
                {
                    filters.Add(new Sql.SqlFilter($"(EXISTS (SELECT 1 FROM F580056 WHERE RDMCU = CMEMCU AND RDDOC1 > 0 AND RDDOC1 = CMDOCO AND RDXDCTO = CMDCTO AND RDZ58DST IN ({string.Concat(Enumerable.Repeat("?,", rackStatusCodes.Count)).TrimEnd(',')}))", "OR"));
                    foreach (var rackStatusCode in rackStatusCodes)
                    {
                        parameters.Add(new iDB2Parameter("Z58DST", iDB2DbType.iDB2Graphic, 2));
                        parameters.Last().Value = rackStatusCode;
                    }
                    filters.Add(new Sql.SqlFilter($"EXISTS (SELECT 1 FROM F580056 WHERE RDMCU = CMEMCU AND RDDOC2 > 0 AND RDDOC2 = CMDOCO AND RDXCTO = CMDCTO AND RDZ58DST IN ({string.Concat(Enumerable.Repeat("?,", rackStatusCodes.Count)).TrimEnd(',')}))", "OR"));
                    foreach (var rackStatusCode in rackStatusCodes)
                    {
                        parameters.Add(new iDB2Parameter("Z58DST", iDB2DbType.iDB2Graphic, 2));
                        parameters.Last().Value = rackStatusCode;
                    }
                    filters.Add(new Sql.SqlFilter($"EXISTS (SELECT 1 FROM F580056 WHERE RDMCU = CMEMCU AND RDDOCO > 0 AND RDDOCO = CMDOCO AND RDDCTO = CMDCTO AND RDZ58DST IN ({string.Concat(Enumerable.Repeat("?,", rackStatusCodes.Count)).TrimEnd(',')})))", "AND"));
                    foreach (var rackStatusCode in rackStatusCodes)
                    {
                        parameters.Add(new iDB2Parameter("Z58DST", iDB2DbType.iDB2Graphic, 2));
                        parameters.Last().Value = rackStatusCode;
                    }
                }

                // Command
                var cmd = GetBusinessIds(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                logger.Info(Logging.DbCmdString(cmd));
                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        public static iDB2Command GetBusinessIds(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	            SELECT DISTINCT
                    CMCFGID,
                    CMUPMJ,
                    CMTDAY
	            FROM
		            F3201 
	            WHERE
		            {filters} AND
                    (
                        EXISTS (SELECT 1 FROM F3211 where KZCFGID = CMCFGID) OR
                        EXISTS (SELECT 1 FROM F5832119 where KZCFGID = CMCFGID) 
                    )
                ORDER BY
                    CMCFGID
	            FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetSalesLineBusinessIds(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan, decimal id, List<string> rackStatusCodes)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add(new Sql.SqlFilter("SDMCU = ?", "AND"));
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Dates
                if (!processRunSpan.IsEmpty)
                {
                    var dtList = new List<string[]>
                    {
                        new string[] { "SDUPMJ", "SDTDAY" }
                    };
                    filters.Add(new Sql.SqlFilter($"{E1TimeManager.GetE1DateFilterString(dtList, processRunSpan.StartDateTime, processRunSpan.EndDateTime)}", "AND"));
                }

                // Command
                var cmd = GetSalesLineBusinessIds(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                logger.Info(Logging.DbCmdString(cmd));
                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        public static iDB2Command GetSalesLineBusinessIds(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	            SELECT
                    CMCFGID,
                    SDUPMJ,
                    SDTDAY
	            FROM
		            F4211 
                    INNER JOIN F3201
                    ON  CMKCOO = SDKCOO AND
                        CMDOCO = SDDOCO AND
                        CMDCTO = SDDCTO AND
                        CMLNID = SDLNID 
	            WHERE
		            {filters} AND
                    SDNXTR IN ('523','525') 
                ORDER BY
                    CMCFGID
	            FOR READ ONLY"
            };
            return cmd;
        }

        /********************************************************************************
        * Local System Ids
        ********************************************************************************/
        public static IDataReader GetMissingIds(SqlConnection cn, Plant plant, ProcessRunSpan processRunSpan)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<SqlParameter>();
            try
            {
                // Dates
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(new Sql.SqlFilter($"sud.TransactionDate BETWEEN '{processRunSpan.StartDateTime.AddMinutes(plant.BusinessSystemOffset).AddDays(-1)}' AND '{processRunSpan.EndDateTime.AddMinutes(plant.BusinessSystemOffset)}' ", "AND"));
                }

                // Command
                var cmd = GetMissingIds(filters);
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = cn;
                cmd.CommandTimeout = 0;
                logger.Info(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        public static SqlCommand GetMissingIds(Sql.SqlFilters filters)
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT
                sud.ConfigurationId
            FROM
                EBS.ConfiguredOrder sud
            WHERE
                {filters}
                AND NOT EXISTS
            (   SELECT
                    1
                FROM
                    EBS.ConfiguredUnitAttribute c
                WHERE
                    c.ConfigurationId = sud.ConfigurationId and
                    c.SegmentNum = 50010)
            GROUP BY
                sud.ConfigurationId
            ORDER BY 1";
            return cmd;
        }

        /********************************************************************************
        * Attributes
        ********************************************************************************/
        public static iDB2Command GetAttributes()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT 
	                A.KZCFGID,
	                A.KZCFGCID,
	                A.KZPEL,  
	                A.KZSGVL,
	                A.KZUPMJ,
	                A.KZTDAY
                FROM  (
	                SELECT
		                KZCFGID,
		                KZCFGCID,
		                KZPEL,  
		                KZSGVL,
		                KZUPMJ,
		                KZTDAY,
		                ROW_NUMBER() OVER (PARTITION BY KZCFGID, KZPEL ORDER BY KZCFGCID DESC) AS RN
	                FROM
		                F3211
		                UNION
	                SELECT
		                KZCFGID,
		                KZCFGCID,
		                KZPEL,  
		                KZSGVL,
		                KZUPMJ,
		                KZTDAY,
		                ROW_NUMBER() OVER (PARTITION BY KZCFGID, KZPEL ORDER BY KZCFGCID DESC) AS RN
	                FROM
		                F5832119
	                ) A
                INNER JOIN F580001 B
                ON	B.AMPEL = A.KZPEL
                WHERE 
	                A.KZCFGID = ? AND
	                A.RN = 1 AND
	                A.KZSGVL <> '' AND
	                NOT (B.AMCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000')) 
                ORDER BY
	                A.KZCFGID,
	                A.KZPEL
                FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CFGID"].Precision = 15;
            cmd.Parameters["CFGID"].Scale = 0;
            return cmd;
        }

    }
}
