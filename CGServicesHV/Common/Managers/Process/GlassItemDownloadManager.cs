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
    public class GlassItemDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GlassItemDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GlassItemDownloadManager.GetDefinitionKeysPrepared", GetDefinitionKeysPrepared()},
                {"GlassItemDownloadManager.GetBalanceKeysPrepared", GetBalanceKeysPrepared()},
                {"GlassItemDownloadManager.GetItemDefinitionPrepared", GetItemDefinitionPrepared()},
                {"GlassItemDownloadManager.GetCustomerPartPrepared", GetCustomerPartPrepared()},
                {"GlassItemDownloadManager.GetPrimaryLocationPrepared", GetPrimaryLocationPrepared()},
                //{"GlassItemDownloadManager.GetQuantityPrepared", GetQuantityPrepared()},
                {"GlassItemDownloadManager.GetWeightPrepared", GetWeightPrepared()},
                {"GlassItemDownloadManager.GetCostPrepared", GetCostPrepared()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession e1Session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum, List<string> stockTypes)
        {
            var response = new ProcessResponse("Glass Item Download Manager");
            var itemsTable = new GlassItemTable();
            //var balKeysTable = new GlassItemTable();

            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", e1Session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "GlassItem") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(e1Session));
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
                var step2a = new ProcessStep("Get Definition Keys");
                try
                {
                    if (useHighWaterMark)
                    {
                        itemsTable = GetDefinitionKeysPrepared(e1Session, plant, processRunSpan);
                    }
                    else
                    {
                        itemsTable = GetDefinitionKeysDynamic(e1Session, plant, processRunSpan, itemNum, stockTypes);
                    }
                    step2a.RowsAffected = itemsTable.Rows.Count;
                }
                catch (Exception e)
                {
                    step2a.Errors.Add(new Error(e));
                }
                finally
                {
                    step2a.Done();
                    response.ProcessSteps.Add(step2a);
                }

                //if (!processRunSpan.IsEmpty)
                //{
                //    var step2b = new ProcessStep("Get Balance Keys");
                //    try
                //    {
                //        if (useHighWaterMark)
                //        {
                //            balKeysTable = GetBalanceKeysPrepared(e1Session, plant, processRunSpan);
                //        }
                //        else
                //        {
                //            balKeysTable = GetBalanceKeysDynamic(e1Session, plant, processRunSpan, itemNum, stockTypes);
                //        }
                //        step2b.RowsAffected = balKeysTable.Rows.Count;
                //        foreach (DataRow dr1 in balKeysTable.Rows)
                //        {
                //            var balanceItemNum = (decimal)dr1["ItemNum"];
                //            var balanceItemPlant = (string)dr1["Plant"];
                //            if (!itemsTable.AsEnumerable().Any(row => balanceItemNum == row.Field<decimal>("ItemNum") && balanceItemPlant == row.Field<string>("Plant")))
                //            {
                //                DataRow dr = itemsTable.NewRow();
                //                dr["ItemNum"] = dr1["ItemNum"];
                //                dr["Plant"] = dr1["Plant"];
                //                itemsTable.Rows.Add(dr);
                //            }
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        step2b.Errors.Add(new Error(e));
                //    }
                //    finally
                //    {
                //        step2b.Done();
                //        response.ProcessSteps.Add(step2b);
                //    }
                //}

                var step2c = new ProcessStep("Get Item Definition");
                try
                {
                    GetItemDefinitionPrepared(e1Session, plant, itemsTable);
                    step2c.RowsAffected = itemsTable.Rows.Count;
                    response.RowsAffected = itemsTable.Rows.Count;
                }
                catch (Exception e)
                {
                    step2c.Errors.Add(new Error(e));
                }
                finally
                {
                    step2c.Done();
                    response.ProcessSteps.Add(step2c);
                }

                //var step2d = new ProcessStep("Get Quantity");
                //try
                //{
                //    GetQuantity(session, plant, table);
                //    step2d.RowsAffected = table.Rows.Count;
                //}
                //catch (Exception e)
                //{
                //    step2d.Errors.Add(new Error(e));
                //}
                //finally
                //{
                //    step2d.Done();
                //    response.ProcessSteps.Add(step2d);
                //}

                var step2e = new ProcessStep("Get Cost");
                try
                {
                    GetCostPrepared(e1Session, plant, itemsTable);
                    step2e.RowsAffected = itemsTable.Rows.Count;
                }
                catch (Exception e)
                {
                    step2e.Errors.Add(new Error(e));
                }
                finally
                {
                    step2e.Done();
                    response.ProcessSteps.Add(step2e);
                }

                var step2f = new ProcessStep("Get Customer Part");
                try
                {
                    GetCustomerPartPrepared(e1Session, plant, itemsTable);
                    step2f.RowsAffected = itemsTable.Rows.Count;
                }
                catch (Exception e)
                {
                    step2f.Errors.Add(new Error(e));
                }
                finally
                {
                    step2f.Done();
                    response.ProcessSteps.Add(step2f);
                }

                var step2g = new ProcessStep("Get Primary Location");
                try
                {
                    GetPrimaryLocationPrepared(e1Session, plant, itemsTable);
                    step2g.RowsAffected = itemsTable.Rows.Count;
                }
                catch (Exception e)
                {
                    step2g.Errors.Add(new Error(e));
                }
                finally
                {
                    step2g.Done();
                    response.ProcessSteps.Add(step2g);
                }

                var step2h = new ProcessStep("Get Weight");
                try
                {
                    GetWeightPrepared(e1Session, plant, itemsTable);
                    step2h.RowsAffected = itemsTable.Rows.Count;
                }
                catch (Exception e)
                {
                    step2h.Errors.Add(new Error(e));
                }
                finally
                {
                    step2h.Done();
                    response.ProcessSteps.Add(step2h);
                }

                var stepTranDate = new ProcessStep("Set Transaction Date");
                try
                {
                    itemsTable.SetTransactionDate(plant);
                    stepTranDate.RowsAffected = itemsTable.Rows.Count;
                }
                catch (Exception e)
                {
                    stepTranDate.Errors.Add(new Error(e));
                }
                finally
                {
                    stepTranDate.Done();
                    response.ProcessSteps.Add(stepTranDate);
                }

                /********************************************************/
                if (itemsTable.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    SqlBulkCopyResponse bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        bulkResponse = BulkCopyManager.BulkTable(itemsTable.ExtendedProperties["Target"].ToString(), itemsTable, sqlCns["PLANT.INTEGRATION"]);
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
                if (useHighWaterMark && !response.HasErrors() && itemsTable.Rows.Count > 0)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        highWaterMark.EnterpriseDateTime = itemsTable.MaxDate.AddHours(-plant.TimeOffset);
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
            response.Done();
            return response;
        }

        /******************************************************************
         * Definition Keys
         ******************************************************************/
        public iDB2Command GetDefinitionKeysPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
                SELECT * FROM (
                    SELECT 
	                    IBMCU,
	                    IBITM,
                        IBUPMJ,
                        IBTDAY,
                        ROW_NUMBER() OVER (PARTITION BY IBMCU, IBITM ORDER BY IBUPMJ DESC, IBTDAY DESC) AS RN
                    FROM 
                        F4101
	                    INNER JOIN F4102 
	                        ON	IMITM = IBITM
                    WHERE 
                        IBMCU IN (?,?,?,?) AND
                        ((IBUPMJ = ? AND IBTDAY > ? AND IBTDAY <= ?) OR (IBUPMJ >= ? AND IBUPMJ <= ?) OR (IBUPMJ = ? AND IBTDAY <= ?)) AND
                        IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC')
                    GROUP BY
                        IBMCU,
	                    IBITM,
                        IBUPMJ,
                        IBTDAY
                ) A WHERE RN = 1
                FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU2", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU3", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU4", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("DT1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DT1"].Precision = 6;
            cmd.Parameters["DT1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("TM1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TM1"].Precision = 6;
            cmd.Parameters["TM1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("TM2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TM2"].Precision = 6;
            cmd.Parameters["TM2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DT2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DT2"].Precision = 6;
            cmd.Parameters["DT2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DT3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DT3"].Precision = 6;
            cmd.Parameters["DT3"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DT4", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DT4"].Precision = 6;
            cmd.Parameters["DT4"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("TM3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TM3"].Precision = 6;
            cmd.Parameters["TM3"].Scale = 0;
            return cmd;
        }

        public GlassItemTable GetDefinitionKeysPrepared(BusinessSystemSession e1Session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetDefinitionKeysPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            var table = new GlassItemTable();
            try
            {
                var cmd = e1Session.DbCommands["GlassItemDownloadManager.GetDefinitionKeysPrepared"];
                cmd.Parameters["MCU1"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU2"].Value = (plant.PlantCode == "MZCG") ? "05".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU3"].Value = (plant.PlantCode == "MZCG") ? "04".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU4"].Value = (plant.PlantCode == "MZCG") ? "08".PadLeft(12) : plant.PlantNum.PadLeft(12);
                var e1Timestamps = E1TimeManager.GetE1TimeIdSpan(processRunSpan.StartDateTime, processRunSpan.EndDateTime);
                cmd.Parameters["DT1"].Value = e1Timestamps.StartDateId;
                cmd.Parameters["TM1"].Value = e1Timestamps.StartTimeId1;
                cmd.Parameters["TM2"].Value = e1Timestamps.StartTimeId2;
                cmd.Parameters["DT2"].Value = e1Timestamps.MidDateId1;
                cmd.Parameters["DT3"].Value = e1Timestamps.MidDateId2;
                cmd.Parameters["DT4"].Value = e1Timestamps.EndDateId;
                cmd.Parameters["TM3"].Value = e1Timestamps.EndTimeId;

                cmd.Connection = e1Session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        DataRow dr = table.NewRow();
                        dr["ItemNum"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
                        dr["Plant"] = CastDbNull.To<string>(reader["IBMCU"], "");
                        dr["IBDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0), CastDbNull.To<decimal>(reader["IBTDAY"], 0));
                        table.Rows.Add(dr);
                    }
                    reader.Close();
                }
                Logger.Info($"Rows: {table.Rows.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return table;
        }

        public iDB2Command GetDefinitionKeysDynamic(List<string> filters)
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
                SELECT * FROM (
                    SELECT 
	                    IBMCU,
	                    IBITM,
                        IBUPMJ,
                        IBTDAY,
                        ROW_NUMBER() OVER (PARTITION BY IBMCU, IBITM ORDER BY IBUPMJ DESC, IBTDAY DESC) AS RN
                    FROM 
                        F4101
	                    INNER JOIN F4102 
	                        ON	IMITM = IBITM
                    WHERE 
                        {filterString} 
                        IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC')
                    GROUP BY
                        IBMCU,
	                    IBITM,
                        IBUPMJ,
                        IBTDAY
                ) A WHERE RN = 1
                FOR READ ONLY";
            return cmd;
        }

        public GlassItemTable GetDefinitionKeysDynamic(BusinessSystemSession session, Plant plant, ProcessRunSpan span, decimal itemNum, List<string> stockTypes)
        {
            var table = new GlassItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                if (plant.PlantCode == "MZCG")
                {
                    filters.Add("IBMCU in ('          32', '          05', '          04', '          08') AND \r\n");
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("IBITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                // Stock Types
                if (stockTypes.Any())
                {
                    string strInClause = "";
                    foreach (string stockType in stockTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = stockType;
                    }
                    filters.Add("IBSTKT IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan) + " AND \r\n");
                    //var queryString1 = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);
                    //var queryString2 = E1TimeManager.GetE1DateTimeQueryString("IMUPMJ", "IMTDAY", span.E1TimeIdSpan);
                    //filters.Add("(" + queryString1 + " OR " + queryString2 + ")" + " AND \r\n");
                    //var queryString3 = E1TimeManager.GetE1DateTimeQueryString("IEUPMJ", "IETDAY", span.E1TimeIdSpan);
                    //var queryString4 = E1TimeManager.GetE1DateTimeQueryString("UMUPMJ", "UMTDAY", span.E1TimeIdSpan);
                    //var queryString5 = E1TimeManager.GetE1DateTimeQueryString("IVUPMJ", "IVTDAY", span.E1TimeIdSpan);
                    //filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + ")" + " AND \r\n");
                }

                cmd = GetDefinitionKeysDynamic(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        DataRow dr = table.NewRow();
                        dr["ItemNum"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
                        dr["Plant"] = CastDbNull.To<string>(reader["IBMCU"], "");
                        dr["IBDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0), CastDbNull.To<decimal>(reader["IBTDAY"], 0));
                        table.Rows.Add(dr);
                    }

                    reader.Close();
                }
                Logger.Trace($"ROWS: {table.Rows.Count}");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return table;
        }

        /******************************************************************
          * Balance Keys
          ******************************************************************/
        public iDB2Command GetBalanceKeysPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
                SELECT * FROM (
                    SELECT 
	                    LIMCU,
	                    LIITM,
                        LIUPMJ,
                        LITDAY,
                        ROW_NUMBER() OVER (PARTITION BY LIMCU, LIITM ORDER BY LIUPMJ DESC, LITDAY DESC) AS RN
                    FROM 
                        F41021
                    WHERE 
                        LIMCU IN (?,?,?,?) AND
                        ((LIUPMJ = ? AND LITDAY > ? AND LITDAY <= ?) OR (LIUPMJ >= ? AND LIUPMJ <= ?) OR (LIUPMJ = ? AND LITDAY <= ?)) AND
                        LIGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC')
                    GROUP BY
                        LIMCU,
	                    LIITM,
                        LIUPMJ,
                        LITDAY
                ) A WHERE RN = 1
                FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU2", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU3", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("MCU4", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("DT1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DT1"].Precision = 6;
            cmd.Parameters["DT1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("TM1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TM1"].Precision = 6;
            cmd.Parameters["TM1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("TM2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TM2"].Precision = 6;
            cmd.Parameters["TM2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DT2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DT2"].Precision = 6;
            cmd.Parameters["DT2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DT3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DT3"].Precision = 6;
            cmd.Parameters["DT3"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DT4", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DT4"].Precision = 6;
            cmd.Parameters["DT4"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("TM3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TM3"].Precision = 6;
            cmd.Parameters["TM3"].Scale = 0;
            return cmd;
        }

        public GlassItemTable GetBalanceKeysPrepared(BusinessSystemSession e1Session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetBalanceKeysPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            var table = new GlassItemTable();
            try
            {
                var cmd = e1Session.DbCommands["GlassItemDownloadManager.GetBalanceKeysPrepared"];
                cmd.Parameters["MCU1"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU2"].Value = (plant.PlantCode == "MZCG") ? "05".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU3"].Value = (plant.PlantCode == "MZCG") ? "04".PadLeft(12) : plant.PlantNum.PadLeft(12);
                cmd.Parameters["MCU4"].Value = (plant.PlantCode == "MZCG") ? "08".PadLeft(12) : plant.PlantNum.PadLeft(12);
                var e1Timestamps = E1TimeManager.GetE1TimeIdSpan(processRunSpan.StartDateTime, processRunSpan.EndDateTime);
                cmd.Parameters["DT1"].Value = e1Timestamps.StartDateId;
                cmd.Parameters["TM1"].Value = e1Timestamps.StartTimeId1;
                cmd.Parameters["TM2"].Value = e1Timestamps.StartTimeId2;
                cmd.Parameters["DT2"].Value = e1Timestamps.MidDateId1;
                cmd.Parameters["DT3"].Value = e1Timestamps.MidDateId2;
                cmd.Parameters["DT4"].Value = e1Timestamps.EndDateId;
                cmd.Parameters["TM3"].Value = e1Timestamps.EndTimeId;

                cmd.Connection = e1Session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        DataRow dr = table.NewRow();
                        dr["ItemNum"] = CastDbNull.To<decimal>(reader["LIITM"], 0);
                        dr["Plant"] = CastDbNull.To<string>(reader["LIMCU"], "");
                        dr["LIDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["LIUPMJ"], 0), CastDbNull.To<decimal>(reader["LITDAY"], 0));
                        table.Rows.Add(dr);
                    }

                    reader.Close();
                }
                Logger.Info($"Rows: {table.Rows.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return table;
        }

        public iDB2Command GetBalanceKeysDynamic(List<string> filters)
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
                SELECT * FROM (
                    SELECT 
	                    LIMCU,
	                    LIITM,
                        LIUPMJ,
                        LITDAY,
                        ROW_NUMBER() OVER (PARTITION BY LIMCU, LIITM ORDER BY LIUPMJ DESC, LITDAY DESC) AS RN
                    FROM 
                        F41021
                    INNER JOIN F4102 ON IBITM = LIITM AND IBMCU = LIMCU
                    WHERE 
                        {filterString} 
                        LIGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC')
                    GROUP BY
                        LIMCU,
	                    LIITM,
                        LIUPMJ,
                        LITDAY
                ) A WHERE RN = 1
                FOR READ ONLY";
            return cmd;
        }
        //,
        //LIUPMJ,
        //LITDAY

        public GlassItemTable GetBalanceKeysDynamic(BusinessSystemSession session, Plant plant, ProcessRunSpan span, decimal itemNum, List<string> stockTypes)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetBalanceKeysDynamic PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            var table = new GlassItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (plant.PlantCode == "MZCG")
                {
                    filters.Add("LIMCU IN ('          32', '          05', '          04', '          08') AND \r\n");
                }
                else
                {
                    filters.Add("LIMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("LIITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                // Stock Types
                if (stockTypes != null && stockTypes.Any())
                {
                    string strInClause = "";
                    foreach (string stockType in stockTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = stockType;
                    }
                    filters.Add("IBSTKT IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("LIUPMJ", "LITDAY", span.E1TimeIdSpan) + " AND \r\n");
                }

                cmd = GetBalanceKeysDynamic(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        DataRow dr = table.NewRow();
                        dr["ItemNum"] = CastDbNull.To<decimal>(reader["LIITM"], 0);
                        dr["Plant"] = CastDbNull.To<string>(reader["LIMCU"], "");
                        dr["LIDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["LIUPMJ"], 0), CastDbNull.To<decimal>(reader["LITDAY"], 0));
                        table.Rows.Add(dr);
                    }

                    reader.Close();
                }
                Logger.Info($"Rows: {table.Rows.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return table;
        }

        /******************************************************************
         * Item Definition
         ******************************************************************/
        public static iDB2Command GetItemDefinitionPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
                SELECT 
	                IBMCU,
	                IBITM,
	                IMSRP4,
	                IMDSC1,
	                IMDSC2,
	                IMUOM1,
	                IBSTKT,
	                IBGLPT,
	                IBVEND,
	                AB2.ABALPH AS VENDALPH,
	                IBANPL,
	                AB1.ABALPH AS ANPLALPH,
	                IMUPMJ,
	                IMTDAY,
	                IBUPMJ,
	                IBTDAY,
                    IA1.IADVAL AS Z58USF,
                    IA2.IADVAL AS Z58BSF
                FROM 
	                F4101 
	                INNER JOIN F4102 
	                    ON IMITM = IBITM
	                LEFT OUTER JOIN F0101 AB1
	                    ON AB1.ABAN8 = IBANPL
	                LEFT OUTER JOIN F0101 AB2
	                    ON AB2.ABAN8 = IBVEND
                    LEFT OUTER JOIN F580002 IA1
                    ON IA1.IAITM = IMITM and
                        IA1.IAZ58ATNM = 'Z58USF'
                    LEFT OUTER JOIN F580002 IA2
                    ON IA2.IAITM = IMITM and
                        IA2.IAZ58ATNM = 'Z58BSF'
                WHERE
                    IBMCU IN (?) AND
                    IBITM = ?
                FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetItemDefinitionPrepared(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetItemDefinitionPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            try
            {
                Enterprise.ItemAttributeManager itemAttMgr = new Enterprise.ItemAttributeManager(session);
                CustomerNameManager custNameMgr = new CustomerNameManager(session);
                var cmd = session.DbCommands["GlassItemDownloadManager.GetItemDefinitionPrepared"];
                cmd.Connection = session.DbConnection;
                var i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    var j = 0;
                    cmd.Parameters["MCU1"].Value = dr["Plant"].ToString().PadLeft(12); 
                    cmd.Parameters["ITM"].Value = (decimal)dr["ItemNum"];
                    Logger.Info(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["Plant"] = CastDbNull.To(reader["IBMCU"], String.Empty).Trim();
                            dr["UnitType"] = CastDbNull.To(reader["IMSRP4"], String.Empty).Trim();
                            dr["Description1"] = CastDbNull.To(reader["IMDSC1"], String.Empty).Trim();
                            dr["Description2"] = CastDbNull.To(reader["IMDSC2"], String.Empty).Trim();
                            dr["StockType"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                            dr["GlCategory"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
                            dr["CustomerNum"] = CastDbNull.To<decimal>(reader["IBANPL"], 0);
                            dr["CustomerName"] = CastDbNull.To<string>(reader["ANPLALPH"], String.Empty).Trim();
                            dr["PrimaryUom"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
                            dr["SupplierNum"] = CastDbNull.To<decimal>(reader["IBVEND"], 0);
                            dr["SupplierName"] = CastDbNull.To<string>(reader["VENDALPH"], String.Empty).Trim();
                            dr["ActualSqft"] = Data.ParseStringToDecimal(CastDbNull.To<string>(reader["Z58USF"], String.Empty).Trim(), 4);
                            dr["BillableSqft"] = Data.ParseStringToDecimal(CastDbNull.To<string>(reader["Z58BSF"], String.Empty).Trim(), 4);
                            dr["IMDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IMUPMJ"], 0), CastDbNull.To<decimal>(reader["IMTDAY"], 0));
                            dr["IBDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0), CastDbNull.To<decimal>(reader["IBTDAY"], 0));
                            i++;
                            j++;
                        }

                        reader.Close();
                        Logger.Info($"Rows: {j}");
                    }
                    //var atts = itemAttMgr.GetDiscreteItemAttributes(session, (decimal)dr["ItemNum"]);
                    //if (atts.GetDecimalAttributeValue("AN8DR") > 0)
                    //{
                    //    dr["CustomerNum"] = atts.GetDecimalAttributeValue("AN8DR");
                    //    dr["CustomerName"] = custNameMgr.GetName(session, (decimal)dr["CustomerNum"]);
                    //}
                }
                Logger.Info($"Total rows: {i}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        /******************************************************************
         * Customer Part
         ******************************************************************/
        public static iDB2Command GetCustomerPartPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
                SELECT 
                    IVCITM,
                    IVUPMJ,
                    IVTDAY
                FROM 
                    F4104
	            WHERE  
		            IVXRT = 'C' AND
		            IVEXDJ >= ? AND
		            IVEFTJ <= ? AND
                    IVITM = ? AND
		            IVAN8 = ?
                FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("EXDJ", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EXDJ"].Precision = 6;
            cmd.Parameters["EXDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("EFTJ", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EFTJ"].Precision = 6;
            cmd.Parameters["EFTJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AN8"].Precision = 8;
            cmd.Parameters["AN8"].Scale = 0;
            return cmd;
        }

        public void GetCustomerPartPrepared(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetCustomerPartPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetCustomerPartPrepared"];
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
                var i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (CastDbNull.To<decimal>(dr["CustomerNum"], 0) > 0)
                    {
                        cmd.Parameters["EXDJ"].Value = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                        cmd.Parameters["EFTJ"].Value = E1TimeManager.CastToE1Date(DateTime.Now.Date);

                        Logger.Info($"Param Item {dr["ItemNum"]}");
                        cmd.Parameters["ITM"].Value = (decimal) dr["ItemNum"];
                        Logger.Info($"Param CustomerNum {dr["CustomerNum"]}");
                        cmd.Parameters["AN8"].Value = (decimal) dr["CustomerNum"];
                        Logger.Info(Logging.DbCmdString(cmd));
                        var j = 0;
                        using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            while (reader.Read())
                            {
                                //Logger.Info($"Item {(decimal) dr["ItemNum"]}");
                                //Logger.Info($"CustomerNum {(decimal) dr["CustomerNum"]}");
                                //Logger.Info(CastDbNull.To<string>(reader["IVCITM"], String.Empty).Trim());
                                dr["CustomerPartNum"] = CastDbNull.To<string>(reader["IVCITM"], String.Empty).Trim();
                                //Logger.Info(CastDbNull.To<decimal>(reader["IVUPMJ"], 0));
                                //Logger.Info(CastDbNull.To<decimal>(reader["IVTDAY"], 0));
                                dr["IVDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IVUPMJ"], 0), CastDbNull.To<decimal>(reader["IVTDAY"], 0));
                                i++;
                                j++;
                            }

                            reader.Close();
                            Logger.Info($"Rows: {j}");
                        }
                    }
                }
                Logger.Info($"Total Rows: {i}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        /******************************************************************
         * Primary Location
         ******************************************************************/
        public static iDB2Command GetPrimaryLocationPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
                SELECT
                    LILOCN, 
                    LIUPMJ,
                    LITDAY
                FROM
                    F41021
                WHERE
                    LIPBIN = 'P' AND
                    LIMCU IN (?) AND
                    LIITM = ?
                FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetPrimaryLocationPrepared(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetPrimaryLocationPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetPrimaryLocationPrepared"];
                cmd.Connection = session.DbConnection;
                var i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    var j = 0;
                    cmd.Parameters["MCU1"].Value = dr["Plant"].ToString().PadLeft(12);
                    cmd.Parameters["ITM"].Value = (decimal)dr["ItemNum"];
                    Logger.Info(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["PrimaryLocation"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty).Trim();
                            dr["LIDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["LIUPMJ"], 0), CastDbNull.To<decimal>(reader["LITDAY"], 0));
                            i++;
                            j++;
                        }

                        reader.Close();
                        Logger.Info($"Rows: {j}");
                    }
                }
                Logger.Info($"Total rows: {i}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        /******************************************************************
         * Cost
         ******************************************************************/
        public static iDB2Command GetCostPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
                SELECT
                    SUM(IECSL / 10000) AS IECSL,
                    MAX(IEUPMJ*1000000+IETDAY) AS IEUPMJTDAY
                FROM
                    F30026
                WHERE
                    IELEDG = '07' AND
                    IEMMCU IN (?) AND
                    IEITM = ?
                HAVING
                    SUM(IECSL) IS NOT NULL
                FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetCostPrepared(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetCostPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetCostPrepared"];
                cmd.Connection = session.DbConnection;
                var i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    var j = 0;
                    cmd.Parameters["MCU1"].Value = dr["Plant"].ToString().PadLeft(12);
                    cmd.Parameters["ITM"].Value = (decimal)dr["ItemNum"];
                    Logger.Info(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["CostPerEa"] = CastDbNull.To<decimal>(reader["IECSL"], 0);
                            var ts = CastDbNull.To<decimal>(reader["IEUPMJTDAY"], 0);
                            dr["IEDate"] = E1TimeManager.CastToDateTime(Convert.ToDecimal(ts.ToString().Substring(0, 6)), Convert.ToDecimal(ts.ToString().Substring(6, 6)));
                            i++;
                            j++;
                        }

                        reader.Close();
                        Logger.Info($"Rows: {j}");
                    }
                }
                Logger.Info($"Total rows: {i}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        /******************************************************************
         * Cost
         ******************************************************************/
        public static iDB2Command GetWeightPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
                SELECT
                    UMCONV / 10000000 AS UMCONV,
                    UMUPMJ,
                    UMTDAY
                FROM
                    F41002
                WHERE
		            UMMCU IN (?) AND
                    UMITM = ? AND
		            UMUM = 'EA' AND
		            UMRUM = 'LB'
                FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetWeightPrepared(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetWeightPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetWeightPrepared"];
                cmd.Connection = session.DbConnection;
                var i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    var j = 0;
                    cmd.Parameters["MCU1"].Value = dr["Plant"].ToString().PadLeft(12);
                    cmd.Parameters["ITM"].Value = (decimal)dr["ItemNum"];
                    Logger.Info(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["LbsPerEa"] = CastDbNull.To<decimal>(reader["UMCONV"], 0);
                            dr["UMDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["UMUPMJ"], 0), CastDbNull.To<decimal>(reader["UMTDAY"], 0));
                            i++;
                            j++;
                        }

                        reader.Close();
                        Logger.Info($"Rows: {j}");
                    }
                }
                Logger.Info($"Total rows: {i}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

    }
}
