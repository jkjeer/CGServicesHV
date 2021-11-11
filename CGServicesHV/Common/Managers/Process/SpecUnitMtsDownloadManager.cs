using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class SpecUnitMtsDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly E1.ItemAttributeManager _itemAttMgr;

        public SpecUnitMtsDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpecUnitMtsDownloadManager.GetWeightPrepared", GetWeightPrepared()},
                {"SpecUnitMtsDownloadManager.GetMtsItemsPrepared", GetMtsItemsPrepared()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
            _itemAttMgr = new E1.ItemAttributeManager(session);
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum)
        {
            var response = new ProcessResponse("MTS Spec Unit Download Manager");
            var table = new SpecUnitTable();

            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
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
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "SpecUnitMTS") : null;
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
                var specUnits = new SpecUnits();
                var step2 = new ProcessStep("Get MTS Items");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (useHighWaterMark)
                    {
                        Logger.Debug($@"GetMtsItemsPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        specUnits = GetMtsItemsPrepared(session, processRunSpan, plant);
                    }
                    else
                    {
                        Logger.Debug($@"GetMtsItemsDynamic PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        specUnits = GetMtsItemsDynamic(session, plant, processRunSpan, itemNum);
                    }
                    Logger.Info($"Rows: {specUnits.Count}");
                    Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                    step2.Errors.Add(new Error(e));
                }
                finally
                {
                    step2.RowsAffected = specUnits.Count;
                    step2.Done();
                    response.ProcessSteps.Add(step2);
                }

                if (!response.HasErrors() && specUnits.Count > 0)
                {
                    /********************************************************/
                    var stepGetItemAttributes = new ProcessStep("Get Item Attributes");
                    try
                    {
                        table.Add(specUnits, plant);
                    }
                    catch (Exception e)
                    {
                        stepGetItemAttributes.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepGetItemAttributes.RowsAffected = specUnits.Count;
                        stepGetItemAttributes.Done();
                        response.ProcessSteps.Add(stepGetItemAttributes);
                    }

                    if (!response.HasErrors() && table.Rows.Count > 0)
                    {
                        var stepGetWeight = new ProcessStep("Get Weight");
                        try
                        {
                            GetWeightPrepared(session, plant, table);
                        }
                        catch (Exception e)
                        {
                            stepGetWeight.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            stepGetWeight.RowsAffected = table.Rows.Count;
                            stepGetWeight.Done();
                            response.ProcessSteps.Add(stepGetWeight);
                        }

                        //var stepTranDate = new ProcessStep("Set Transaction Date");
                        //try
                        //{
                        //    table.SetTransactionDate();
                        //    stepTranDate.RowsAffected = table.Rows.Count;
                        //}
                        //catch (Exception e)
                        //{
                        //    stepTranDate.Errors.Add(new Error(e));
                        //}
                        //finally
                        //{
                        //    stepTranDate.Done();
                        //    response.ProcessSteps.Add(stepTranDate);
                        //}

                        /********************************************************/
                        if (!response.HasErrors() && table.Rows.Count > 0)
                        {
                            var step3 = new ProcessStep("Bulk Copy");
                            SqlBulkCopyResponse bulkResponse = new SqlBulkCopyResponse();
                            try
                            {
                                bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                                step3.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step3.RowsAffected = (int) bulkResponse.RowsCopied;
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
                                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                                step4.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step4.Done();
                                response.AddStep(step4);
                            }
                        }
                    }
                }
            }
            response.Close();
            return response;
        }

        /******************************************************************
         * Item Attributes
         ******************************************************************/
        public iDB2Command GetMtsItemsPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
	                *
                FROM (
	                SELECT
		                IAITM,
		                IAUPMJ,
		                IATDAY,
                        IBSRP4,
                        IBSTKT,
		                ROW_NUMBER() OVER (PARTITION BY IAITM ORDER BY IAUPMJ DESC, IATDAY DESC) AS ROWNUM
	                FROM 
		                F4102
		                INNER JOIN F580002
		                ON IAITM = IBITM
	                WHERE 
                        IBMCU IN (?,?,?,?) AND 
                        ((IAUPMJ = ? AND IATDAY > ? AND IATDAY <= ?) OR (IAUPMJ >= ? AND IAUPMJ <= ?) OR (IAUPMJ = ? AND IATDAY <= ?)) AND
                        IBSTKT != 'C' AND
                        IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC')) A
                WHERE
	                A.ROWNUM = 1
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

        public SpecUnits GetMtsItemsPrepared(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, Plant plant)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Debug($@"GetMtsItemsPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            var specUnits = new SpecUnits();
            try
            {
                var cmd = e1Session.DbCommands["SpecUnitMtsDownloadManager.GetMtsItemsPrepared"];
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
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var specUnit = new SpecUnit();
                        specUnit.Id = 0;
                        specUnit.ItemNum = Data.CastDbNull.To<decimal>(reader["IAITM"], 0);
                        specUnit.StockType = Data.CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                        specUnit.UnitType = Data.CastDbNull.To<string>(reader["IBSRP4"], String.Empty).Trim();
                        specUnit.IADate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IAUPMJ"], 0), CastDbNull.To<decimal>(reader["IATDAY"], 0));
                        specUnit.TransactionDate = specUnit.IADate.AddHours(plant.TimeOffset);
                        //specUnit.IBDate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0), CastDbNull.To<decimal>(reader["IBTDAY"], 0)).AddHours(plant.TimeOffset);
                        var itemAtts = _itemAttMgr.GetDiscreteItemAttributes(e1Session, specUnit.ItemNum);
                        specUnit.ItemAttributes = itemAtts;
                        specUnits.Add(specUnit);
                    }

                    reader.Close();
                }
                Logger.Info($"Rows: {specUnits.Count}");
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
            return specUnits;
        }

        public iDB2Command GetMtsItemsDynamic(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
	                *
                FROM (
	                SELECT
		                IAITM,
		                IAUPMJ,
		                IATDAY,
                        IBSRP4,
                        IBSTKT,
		                ROW_NUMBER() OVER (PARTITION BY IAITM ORDER BY IAUPMJ DESC, IATDAY DESC) AS ROWNUM
	                FROM 
		                F4102
		                INNER JOIN F580002
		                ON IAITM = IBITM
	                WHERE 
                        {filterString}
                        IBSTKT != 'C' AND
                        IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC')) A
                WHERE
	                A.ROWNUM = 1
                FOR READ ONLY";
            return cmd;
        }

        public SpecUnits GetMtsItemsDynamic(BusinessSystemSession session, Plant plant, ProcessRunSpan span, decimal itemNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetMtsItemsDynamic PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            var specUnits = new SpecUnits();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                if (plant.PlantCode == "MZCG")
                {
                    filters.Add("IBMCU IN ('          32', '          05', '          04', '          08') AND \r\n");
                }
                else
                {
                    filters.Add("IBMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Run Span
                if (!span.IsEmpty)
                {
                    //var queryString1 = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);
                    //var queryString2 = E1TimeManager.GetE1DateTimeQueryString("IAUPMJ", "IATDAY", span.E1TimeIdSpan);
                    //filters.Add("(" + queryString1 + " OR " + queryString2 + ")" + " AND \r\n");
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("IAUPMJ", "IATDAY", span.E1TimeIdSpan) + " AND \r\n");
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("IAITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                cmd = GetMtsItemsDynamic(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

                Logger.Info("Start MTS Units");
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var specUnit = new SpecUnit();
                        specUnit.Id = 0;
                        specUnit.ItemNum = Data.CastDbNull.To<decimal>(reader["IAITM"], 0);
                        specUnit.StockType = Data.CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                        specUnit.UnitType = Data.CastDbNull.To<string>(reader["IBSRP4"], String.Empty).Trim();
                        specUnit.IADate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IAUPMJ"], 0), CastDbNull.To<decimal>(reader["IATDAY"], 0));
                        specUnit.TransactionDate = specUnit.IADate.AddHours(plant.TimeOffset);
                        //specUnit.IBDate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0), CastDbNull.To<decimal>(reader["IBTDAY"], 0)).AddHours(plant.TimeOffset);
                        var itemAtts = _itemAttMgr.GetDiscreteItemAttributes(session, specUnit.ItemNum);
                        specUnit.ItemAttributes = itemAtts;
                        specUnits.Add(specUnit);
                    }

                    reader.Close();
                }
                Logger.Info($"Rows: {specUnits.Count}");
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
            return specUnits;
        }

        /******************************************************************
         * Weight
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
		            UMMCU = ? AND
                    UMITM = ? AND
		            UMUM = 'EA' AND
		            UMRUM = 'LB'";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
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
                var cmd = session.DbCommands["SpecUnitMtsDownloadManager.GetWeightPrepared"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    var i = 0;
                    Logger.Info(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["GlassWeightLbs"] = CastDbNull.To<decimal>(reader["UMCONV"], 0);
                            i++;
                        }

                        reader.Close();
                    }

                    Logger.Info($"Rows: {i}");
                }
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
