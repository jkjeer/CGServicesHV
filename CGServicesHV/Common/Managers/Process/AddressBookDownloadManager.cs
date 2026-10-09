using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
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
    public class AddressBookDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public AddressBookDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"UdcItemDownloadManager.GetUdcItem", GetUdcItem()}
                {"AddressBookDownloadManager.GetEventData", GetEventData()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, List<string> searchTypes = null, decimal addressNum = 0)
        {
            var response = new ProcessResponse("Address Book Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));

            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new AddressBookTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "AddressBook") : null;
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
                if (useHighWaterMark)
                {
                    /********************************************************/
                    var stepEvents = new ProcessStep("Get Events");
                    try
                    {
                        table = GetEvents(session, plant, processRunSpan, searchTypes, addressNum);
                        Logger.Info($"Rows: {table.Rows.Count}  Plant: {plant.PlantCode}");
                        stepEvents.RowsAffected = table.Rows.Count;
                        response.RowsAffected = table.Rows.Count;
                    }
                    catch (Exception e)
                    {
                        stepEvents.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepEvents.Done();
                        response.AddStep(stepEvents);
                    }

                    if (!processRunSpan.IsEmpty && !searchTypes.Any() && addressNum == 0)
                    {
                        var stepRelatedEvents = new ProcessStep("Get Related Events");
                        try
                        {
                            var relatedNums = GetRelatedEvents(session, plant, processRunSpan);
                            stepRelatedEvents.RowsAffected = relatedNums.Count;
                            foreach (var num in relatedNums)
                            {
                                if (table.AsEnumerable().All(row =>
                                    num != DataRowExtensions.Field<decimal>(row, "AddressNum")))
                                {
                                    DataRow dr = table.NewRow();
                                    dr["AddressNum"] = num;
                                    dr["ABDate"] = (DateTime) SqlDateTime.MinValue;
                                    dr["ALDate"] = (DateTime) SqlDateTime.MinValue;
                                    dr["MADate"] = (DateTime) SqlDateTime.MinValue;
                                    dr["WPDate"] = (DateTime) SqlDateTime.MinValue;
                                    table.Rows.Add(dr);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            stepRelatedEvents.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            stepRelatedEvents.Done();
                            response.AddStep(stepRelatedEvents);
                        }
                    }

                    /********************************************************/
                    if (table.Rows.Count > 0)
                    {
                        var stepEventData = new ProcessStep("Get Event Data");
                        try
                        {
                            GetEventData(session, plant, table);
                            stepEventData.RowsAffected = table.Rows.Count;
                            response.RowsAffected = table.Rows.Count;
                        }
                        catch (Exception e)
                        {
                            stepEventData.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            stepEventData.Done();
                            response.AddStep(stepEventData);
                        }

                    }
                }
                else
                {
                    var stepAllData = new ProcessStep("Get All Data");
                    try
                    {
                        var reader = GetReader(session, plant, processRunSpan, searchTypes, addressNum);
                        table.Add(reader, plant);
                        stepAllData.RowsAffected = table.Rows.Count;
                        response.RowsAffected = table.Rows.Count;
                    }
                    catch (Exception e)
                    {
                        stepAllData.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepAllData.Done();
                        response.AddStep(stepAllData);
                    }
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
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
                if (useHighWaterMark && !response.HasErrors() && table.Rows.Count > 0)
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
        * EVENTS
        ********************************************************************************/
        public static iDB2Command GetEvents(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
                SELECT
                    ABAN8
                FROM 
                    F0101
                WHERE 
                    {filterString.Remove(filterString.Length - 4, 3)}
                FOR READ ONLY";
            return cmd;
        }

        public static AddressBookTable GetEvents(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan, List<string> searchTypes, decimal addressNum)
        {
            var table = new AddressBookTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Supplier Num
                if (addressNum > 0)
                {
                    filters.Add("ABAN8 = ? AND ");
                    parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = addressNum;
                }

                // Search Types
                if (searchTypes.Any())
                {
                    var strInClause = "";
                    foreach (var searchType in searchTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("SearchTypes", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = searchType;
                    }
                    filters.Add("ABAT1 IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND ");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString + ")" + " AND ");
                }

                cmd = GetEvents(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr["AddressNum"] = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0);
                    dr["ABDate"] = (DateTime)SqlDateTime.MinValue;
                    dr["ALDate"] = (DateTime)SqlDateTime.MinValue;
                    dr["MADate"] = (DateTime)SqlDateTime.MinValue;
                    dr["WPDate"] = (DateTime)SqlDateTime.MinValue;
                    table.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return table;
        }

        public static iDB2Command GetRelatedEvents(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
                SELECT
                    ALAN8 AS AN8
                FROM 
                    F0116
                WHERE 
                    {filters[0]}
            UNION
                SELECT
                    WPAN8 AS AN8
                FROM 
                    F0115
                WHERE 
                    WPIDLN = 0 AND
                    WPRCK7 = 1 AND
                    WPCNLN = 0 AND
                ({filters[1]})
            UNION
                SELECT
                    MAAN8 AS AN8
                FROM 
                    F0150
                WHERE 
                    {filters[2]}
            FOR READ ONLY";
            return cmd;
        }

        public static List<decimal> GetRelatedEvents(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var nums = new List<decimal>();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("ALUPMJ", "ALUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add(queryString1);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add(queryString2);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("MAUPMJ", "MAUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add(queryString3);
                }

                cmd = GetRelatedEvents(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    nums.Add(Data.CastDbNull.To<decimal>(reader["AN8"], 0));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return nums;
        }

        /******************************************************************
         * Event Data
         ******************************************************************/
        public static iDB2Command GetEventData()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
              SELECT *
              FROM (
                   SELECT 
                       ABAN8,
                        ABAT1,
                        DRDL01,
                        ABALPH,
                        ALADD1,
                        ALADD2,
                        ALADD3,
                        ALADD4,
                        ALADDZ,
                        ALCTY1,
                        ALADDS,
                        ALCTR,
                        WPAR1,
                        WPPH1,
                        COALESCE(MAPA8,ABAN8) AS MAPA8,
                        ABAC05,
                        ABAC29,
                        ABUPMJ,
                        ABUPMT,
                        COALESCE(ALUPMJ, 0) AS ALUPMJ,
                        COALESCE(ALUPMT, 0) AS ALUPMT,
                        COALESCE(WPUPMJ, 0) AS WPUPMJ,
                        COALESCE(WPUPMT, 0) AS WPUPMT,
                        COALESCE(MAUPMJ, 0) AS MAUPMJ,
                        COALESCE(MAUPMT, 0) AS MAUPMT,
                        RowNumber() over (PARTITION BY ABAN8 ORDER BY ABUPMJ DESC, ABUPMT DESC, ALUPMJ DESC, ALUPMT DESC, MAUPMJ DESC, MAUPMT DESC, WPUPMJ DESC, WPUPMT DESC) AS ROWNUM
                    FROM 
                        F0101
                        LEFT OUTER JOIN F0116 
                            ON  ALAN8 = ABAN8 AND
                                ALEFTB = ABEFTB
                        LEFT OUTER JOIN F0115 
                            ON  WPAN8 = ABAN8 AND
                                WPIDLN = 0 AND
                                WPRCK7 = 1 AND
                                WPCNLN = 0
                        LEFT OUTER JOIN F0150 
                            ON  MAAN8 = ABAN8
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 
                            ON  DRSY = '01' AND
                                DRRT = 'ST' AND
                                TRIM(DRKY) = ABAT1
                    WHERE
                        ABAN8 = ?
                ) X WHERE ROWNUM = 1";
            cmd.Parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AN8"].Precision = 8;
            cmd.Parameters["AN8"].Scale = 0;
            return cmd;
        }

        public static void GetEventData(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = GetEventData(); //session.DbCommands["AddressBookDownloadManager.GetEventData"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = (decimal)dr["AddressNum"];
                    Logger.Info(Logging.DbCmdString(cmd));
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        var addressNum = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0);
                        dr["AddressNum"] = addressNum;
                        dr["Name"] = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        dr["AddressType"] = Data.CastDbNull.To(reader["ABAT1"], string.Empty).Trim();
                        dr["AddressTypeName"] = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        dr["NationalAccountGroup"] = Data.CastDbNull.To(reader["ABAC05"], string.Empty).Trim();
                        dr["Address1"] = Data.CastDbNull.To(reader["ALADD1"], string.Empty).Trim();
                        dr["Address2"] = Data.CastDbNull.To(reader["ALADD2"], string.Empty).Trim();
                        dr["Address3"] = Data.CastDbNull.To(reader["ALADD3"], string.Empty).Trim();
                        dr["Address4"] = Data.CastDbNull.To(reader["ALADD4"], string.Empty).Trim();
                        dr["PostalCode"] = Data.CastDbNull.To(reader["ALADDZ"], string.Empty).Trim();
                        dr["City"] = Data.CastDbNull.To(reader["ALCTY1"], string.Empty).Trim();
                        dr["State"] = Data.CastDbNull.To(reader["ALADDS"], string.Empty).Trim();
                        dr["Country"] = Data.CastDbNull.To(reader["ALCTR"], string.Empty).Trim();
                        dr["AreaCode"] = Data.CastDbNull.To(reader["WPAR1"], string.Empty).Trim();
                        dr["Phone"] = Data.CastDbNull.To(reader["WPPH1"], string.Empty).Trim();
                        var parentAddressNum = Data.CastDbNull.To<decimal>(reader["MAPA8"], 0);
                        dr["ParentAddressNum"] = (parentAddressNum == 0) ? addressNum : parentAddressNum;
                        var company = Data.CastDbNull.To(reader["ABAC29"], string.Empty).Trim();
                        dr["Company"] = (company == "") ? 0 : Convert.ToDecimal(company);
                        dr["Company"] = (company == "") ? 0 : Convert.ToDecimal(company);
                        dr["ABDate"] = E1TimeManager
                            .CastToDateTime(Data.CastDbNull.To<decimal>(reader["ABUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["ABUPMT"], 0))
                            .AddHours(plant.TimeOffset);
                        dr["ALDate"] = E1TimeManager
                            .CastToDateTime(Data.CastDbNull.To<decimal>(reader["ALUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["ALUPMT"], 0))
                            .AddHours(plant.TimeOffset);
                        dr["MADate"] = E1TimeManager
                            .CastToDateTime(Data.CastDbNull.To<decimal>(reader["MAUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["MAUPMT"], 0))
                            .AddHours(plant.TimeOffset);
                        dr["WPDate"] = E1TimeManager
                            .CastToDateTime(Data.CastDbNull.To<decimal>(reader["WPUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["WPUPMT"], 0))
                            .AddHours(plant.TimeOffset);
                        dr["UpdateDate"] = new DateTime[] { (DateTime)dr["ABDate"], (DateTime)dr["ALDate"], (DateTime)dr["MADate"], (DateTime)dr["WPDate"] }.Max();
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /********************************************************************************
        * ALL DATA
        ********************************************************************************/
        private static iDB2Command GetReader(IReadOnlyCollection<string> filters)
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
                      SELECT *
                      FROM (
                           SELECT 
                               ABAN8,
                                ABAT1,
                                DRDL01,
                                ABALPH,
                                ALADD1,
                                ALADD2,
                                ALADD3,
                                ALADD4,
                                ALADDZ,
                                ALCTY1,
                                ALADDS,
                                ALCTR,
                                WPAR1,
                                WPPH1,
                                COALESCE(MAPA8,ABAN8) AS MAPA8,
                                ABAC05,
                                ABAC29,
                                ABUPMJ,
                                ABUPMT,
                                ALUPMJ,
                                ALUPMT,
                                MAUPMJ,
                                MAUPMT,
                                WPUPMJ,
                                WPUPMT,
                                RowNumber() over (PARTITION BY ABAN8 ORDER BY ABUPMJ DESC, ABUPMT DESC, ALUPMJ DESC, ALUPMT DESC, MAUPMJ DESC, MAUPMT DESC, WPUPMJ DESC, WPUPMT DESC) AS ROWNUM
                            FROM 
                                F0101
                                LEFT OUTER JOIN F0116 
                                    ON  ALAN8 = ABAN8 AND
                                        ALEFTB = ABEFTB
                                LEFT OUTER JOIN F0115 
                                    ON  WPAN8 = ABAN8 AND
                                        WPIDLN = 0 AND
                                        WPRCK7 = 1 AND
                                        WPCNLN = 0
                                LEFT OUTER JOIN F0150 
                                    ON  MAAN8 = ABAN8
                                LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 
                                    ON  DRSY = '01' AND
                                        DRRT = 'ST' AND
                                        TRIM(DRKY) = ABAT1
                                {filterString}
                    ) X WHERE ROWNUM = 1"
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan, List<string> searchTypes, decimal addressNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Search Types
                if (searchTypes.Any())
                {
                    var strInClause = "";
                    foreach (var searchType in searchTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("SearchTypes", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = searchType;
                    }
                    filters.Add("ABAT1 IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }
                // Address Num
                if (addressNum > 0)
                {
                    filters.Add("ABAN8 = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = addressNum;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("ALUPMJ", "ALUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("MAUPMJ", "MAUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString4 + " OR " + queryString5 + ")" + " AND \r\n");
                }
                // Command
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetReader(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetReader(filters);
                }
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
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
