using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
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
    public static class ConfiguredAttributeDownloadManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark, decimal configurationId, List<string> rackStatusCodes, bool archived, bool missing)
        {
            var response = new ProcessResponse("Configured Attribute Download Manager");
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
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "ConfiguredAttribute") : null;
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

            /********************************************************/
            var currentIds = new List<decimal>();
            if (!missing)
            {
                var stepIds = new ProcessStep("Get Current Ids");
                try
                {
                    var reader = GetIds(session, plant, processRunSpan, configurationId, rackStatusCodes);
                    while (reader.Read())
                    {
                        currentIds.Add(reader.Get<decimal>("CMCFGID"));
                    }
                    reader.Close();
                    stepIds.RowsAffected = currentIds.Count();
                }
                catch (Exception e)
                {
                    stepIds.Errors.Add(new Error(e));
                }
                finally
                {
                    stepIds.Done();
                    response.AddStep(stepIds);
                }
            }

            /********************************************************/
            var archivedIds = new List<decimal>();
            if (!useHighWaterMark && !missing)
            {
                var stepArchivedIds = new ProcessStep("Get Archived Ids");
                try
                {
                    var reader = GetArchivedIds(session, plant, processRunSpan, configurationId, rackStatusCodes);
                    while (reader.Read())
                    {
                        archivedIds.Add(reader.Get<decimal>("CMCFGID"));
                    }
                    reader.Close();
                    stepArchivedIds.RowsAffected = archivedIds.Count();
                }
                catch (Exception e)
                {
                    stepArchivedIds.Errors.Add(new Error(e));
                }
                finally
                {
                    stepArchivedIds.Done();
                    response.AddStep(stepArchivedIds);
                }
            }

            /********************************************************/
            var missingIds = new List<decimal>();
            if (!useHighWaterMark && missing)
            {
                var stepMissingIds = new ProcessStep("Get Missing Ids");
                try
                {
                    var reader = GetMissingIds(sqlCns["PLANT.SHOPFLOOR"], plant, processRunSpan);
                    while (reader.Read())
                    {
                        missingIds.Add(reader.Get<decimal>("ConfigurationId"));
                    }
                    reader.Close();
                    stepMissingIds.RowsAffected = missingIds.Count();
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
            var table = new ConfiguredAttributeTable();
            var tableName = table.ExtendedProperties["Target"].ToString();
            var batchSize = int.Parse((string)table.ExtendedProperties["BatchSize"]);
            var maxDate = new DateTime(1900, 1, 1);
            if (currentIds.Count > 0)
            {
                var stepTable = new ProcessStep("Bulk Current Data");
                try
                {
                    var cmd = GetCurrentDataById();
                    cmd.Connection = session.DbConnection;
                    var i = 0;
                    foreach (var id in currentIds)
                    {
                        cmd.Parameters[0].Value = id;
                        var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                        while (reader.Read())
                        {
                            i++;
                            table.Add((IDataRecord)reader, plant);
                            if (i % batchSize == 0)
                            {
                                stepTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                                maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                                table.Rows.Clear();
                            }
                        }
                        reader.Close();
                    }
                    if (table.Rows.Count > 0)
                    {
                        stepTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
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
            if (archivedIds.Count > 0)
            {
                var stepArchivedTable = new ProcessStep("Bulk Archived Data");
                try
                {
                    var cmd = GetArchivedDataById();
                    cmd.Connection = session.DbConnection;
                    cmd.CommandTimeout = 0;
                    int i = 0;
                    foreach (var id in archivedIds)
                    {
                        cmd.Parameters[0].Value = id;
                        var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                        while (reader.Read())
                        {
                            i++;
                            table.Add((IDataRecord)reader, plant);
                            if (i % batchSize == 0)
                            {
                                logger.Debug($"BULK {plant.PlantCode} i = {i} and batchSize = {batchSize}");
                                stepArchivedTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                                table.Rows.Clear();
                            }
                        }
                        reader.Close();
                    }
                    if (table.Rows.Count > 0)
                    {
                        logger.Debug($"BULK LAST {plant.PlantCode} i = {i} and batchSize = {batchSize}");
                        stepArchivedTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                        table.Rows.Clear();
                    }
                    stepArchivedTable.RowsAffected = i;
                }
                catch (Exception e)
                {
                    stepArchivedTable.Errors.Add(new Error(e));
                }
                finally
                {
                    stepArchivedTable.Done();
                    response.AddStep(stepArchivedTable);
                }
            }

            /********************************************************/
            if (missingIds.Count > 0)
            {
                var stepMissingTable = new ProcessStep("Bulk Missing Data");
                try
                {
                    var cmd = GetCurrentDataById();
                    cmd.Connection = session.DbConnection;
                    cmd.CommandTimeout = 0;
                    var i = 0;
                    foreach (var id in missingIds)
                    {
                        cmd.Parameters[0].Value = id;
                        var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                        while (reader.Read())
                        {
                            i++;
                            table.Add((IDataRecord)reader, plant);
                            if (i % batchSize == 0)
                            {
                                logger.Debug($"BULK {plant.PlantCode} i = {i} and batchSize = {batchSize}");
                                stepMissingTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                                maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                                table.Rows.Clear();
                            }
                        }
                        reader.Close();
                    }
                    if (table.Rows.Count > 0)
                    {
                        logger.Debug($"BULK LAST {plant.PlantCode} i = {i} and batchSize = {batchSize}");
                        stepMissingTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                        maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                        table.Rows.Clear();
                    }
                    stepMissingTable.RowsAffected = i;
                }
                catch (Exception e)
                {
                    stepMissingTable.Errors.Add(new Error(e));
                }
                finally
                {
                    stepMissingTable.Done();
                    response.AddStep(stepMissingTable);
                }

                var stepMissingArchiveTable = new ProcessStep("Bulk Missing Archived Data");
                try
                {
                    var cmd = GetArchivedDataById();
                    cmd.Connection = session.DbConnection;
                    cmd.CommandTimeout = 0;
                    var i = 0;
                    foreach (var id in missingIds)
                    {
                        cmd.Parameters[0].Value = id;
                        var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                        while (reader.Read())
                        {
                            i++;
                            table.Add((IDataRecord)reader, plant);
                            if (i % batchSize == 0)
                            {
                                logger.Debug($"{plant.PlantCode} BULK i = {i} and batchSize = {batchSize}");
                                stepMissingArchiveTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                                maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                                table.Rows.Clear();
                            }
                        }
                        reader.Close();
                    }
                    if (table.Rows.Count > 0)
                    {
                        logger.Debug($"{plant.PlantCode} BULK LAST i = {i} and batchSize = {batchSize}");
                        stepMissingArchiveTable.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(tableName, table, sqlCns["PLANT.INTEGRATION"], null, batchSize));
                        maxDate = (table.MaxDate > maxDate) ? table.MaxDate : maxDate;
                        table.Rows.Clear();
                    }
                    stepMissingArchiveTable.RowsAffected = i;
                }
                catch (Exception e)
                {
                    stepMissingArchiveTable.Errors.Add(new Error(e));
                }
                finally
                {
                    stepMissingArchiveTable.Done();
                    response.AddStep(stepMissingArchiveTable);
                }
            }

            /********************************************************/
            if (useHighWaterMark && currentIds.Count > 0)
            {
                var stepMark = new ProcessStep("Update High Water Mark");
                try
                {
                    stepMark.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, maxDate.AddMinutes(-plant.BusinessSystemOffset)));
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
        * Current Ids
        ********************************************************************************/
        public static IDataReader GetIds(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan, decimal id, List<string> rackStatusCodes)
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
                var cmd = GetIds(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                logger.Debug(Logging.DbCmdString(cmd));
                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        public static iDB2Command GetIds(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	            SELECT DISTINCT
                    CMCFGID
	            FROM
		            F3201 
	            WHERE
		            {filters} AND
                    EXISTS (SELECT 1 FROM F3211 where KZCFGID = CMCFGID)
                ORDER BY
                    CMCFGID
	            FOR READ ONLY"
            };
            return cmd;
        }

        /********************************************************************************
        * Archived Ids
        ********************************************************************************/
        public static IDataReader GetArchivedIds(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan, decimal id, List<string> rackStatusCodes)
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
                    filters.Add(new Sql.SqlFilter($"EXISTS (SELECT 1 FROM F580056 WHERE RDMCU = CMEMCU AND RDDOC2 > 0 AND RDDOC2 = CMDOCO AND RDXCTO = CMDCTO AND RDZ58DST IN ({string.Concat(Enumerable.Repeat("?,", rackStatusCodes.Count)).TrimEnd(',')})))", "AND"));
                    foreach (var rackStatusCode in rackStatusCodes)
                    {
                        parameters.Add(new iDB2Parameter("Z58DST", iDB2DbType.iDB2Graphic, 2));
                        parameters.Last().Value = rackStatusCode;
                    }
                }

                // Command
                var cmd = GetArchivedIds(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                logger.Debug(Logging.DbCmdString(cmd));
                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        public static iDB2Command GetArchivedIds(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	            SELECT DISTINCT
                    CMCFGID
	            FROM
		            F3201 
	            WHERE
		            {filters} AND
                    EXISTS (SELECT 1 FROM F5832119 where KZCFGID = CMCFGID)
                ORDER BY
                    CMCFGID
	            FOR READ ONLY"
            };
            return cmd;
        }

        /********************************************************************************
        * Missing Ids
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
                    filters.Add(new Sql.SqlFilter($"sud.TransactionDate BETWEEN '{processRunSpan.StartDateTime}' AND '{processRunSpan.EndDateTime}' ", "AND"));
                }

                // Command
                var cmd = GetMissingIds(filters);
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = cn;
                cmd.CommandTimeout = 0;
                logger.Debug(Logging.DbCmdString(cmd));
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
                    EBS.ConfiguredAttribute c
                WHERE
                    c.ConfigId = sud.ConfigurationId)
            GROUP BY
                sud.ConfigurationId
            ORDER BY 1";
            return cmd;
        }

        /********************************************************************************
        * Current Data
        ********************************************************************************/
        public static iDB2Command GetCurrentDataById()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
		            KZCFGID,
		            KZCFGCID,
		            KZPEL,  
		            KZSGVL,
		            AMZ58ATNM,
		            AMCNUM,
		            KZUPMJ,
		            KZTDAY
                FROM (
	                SELECT
		                KZCFGID,
		                KZCFGCID,
		                KZPEL,  
		                KZSGVL,
		                AMZ58ATNM,
		                AMCNUM,
		                KZUPMJ,
		                KZTDAY,
		                ROW_NUMBER() OVER (PARTITION BY KZCFGID, KZPEL ORDER BY KZCFGCID DESC) AS R
	                FROM
		                F3211 
		                INNER JOIN F580001
		                ON AMPEL = KZPEL
	                WHERE
		                KZCFGID = ?
	                ) A
                where 
	                A.R = 1 AND
		            A.KZSGVL <> '' AND
		            NOT (A.AMCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000')) 
                ORDER BY
                    KZCFGID,
                    KZPEL
	            FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("CFGID1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CFGID1"].Precision = 15;
            cmd.Parameters["CFGID1"].Scale = 0;
            return cmd;
        }

        /********************************************************************************
        * Archived Data
        ********************************************************************************/
        public static iDB2Command GetArchivedDataById()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
		            KZCFGID,
		            KZCFGCID,
		            KZPEL,  
		            KZSGVL,
		            AMZ58ATNM,
		            AMCNUM,
		            KZUPMJ,
		            KZTDAY
                FROM (
	                SELECT
		                KZCFGID,
		                KZCFGCID,
		                KZPEL,  
		                KZSGVL,
		                AMZ58ATNM,
		                AMCNUM,
		                KZUPMJ,
		                KZTDAY,
		                ROW_NUMBER() OVER (PARTITION BY KZCFGID, KZPEL ORDER BY KZCFGCID DESC) AS R
	                FROM
		                F5832119 
		                INNER JOIN F580001
		                ON AMPEL = KZPEL
	                WHERE
		                KZCFGID = ?
	                ) A
                where 
	                A.R = 1 AND
		            A.KZSGVL <> '' AND
		            NOT (A.AMCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000')) 
                ORDER BY
                    KZCFGID,
                    KZPEL
	            FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("CFGID1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CFGID1"].Precision = 15;
            cmd.Parameters["CFGID1"].Scale = 0;
            return cmd;
        }

    }
}
