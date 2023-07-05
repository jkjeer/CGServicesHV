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
    public class SpecUnitMtoDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly E1.ItemAttributeManager _itemAttMgr;

        public SpecUnitMtoDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpecUnitMtoDownloadManager.GetF3215Data", GetF3215Data()},
                {"SpecUnitMtoDownloadManager.GetSpecUnitDocumentsPrepared", GetSpecUnitDocumentsPrepared()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName))) session.DbCommands.Add(keyName, hash[keyName]);
            _itemAttMgr = new E1.ItemAttributeManager(session);
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, List<string> rackStatusCodes, decimal rackId, decimal configId, decimal documentNum, string documentType, List<string> woStatusCodes, decimal itemNum)
        {
            var response = new ProcessResponse("MTO Spec Unit Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns) response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));

            var processRunSpan = new ProcessRunSpan();
            var highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = useHighWaterMark ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "SpecUnitMTO") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan,
                    E1TimeManager.GetE1TimeStamp(session));
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
                var specUnitDocuments = new SpecUnitDocuments();
                var step2 = new ProcessStep("Get Spec Unit Documents");
                try
                {
                    var startTime = DateTime.Now.ToLocalTime();
                    if (useHighWaterMark)
                    {
                        Logger.Debug($@"GetSpecUnitDocumentsPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        specUnitDocuments = GetSpecUnitDocumentsPrepared(session, processRunSpan, plant);
                    }
                    else
                    {
                        Logger.Debug($@"GetSpecUnitDocumentsDynamic PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
                        specUnitDocuments = GetSpecUnitDocumentsDynamic(session, plant, processRunSpan, rackStatusCodes, rackId, configId, documentNum, documentType, woStatusCodes, itemNum);
                    }
                    Logger.Info($"Rows: {specUnitDocuments.Count}");
                    Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                    step2.Errors.Add(new Error(e));
                }
                finally
                {
                    step2.RowsAffected = specUnitDocuments.Count;
                    step2.Done();
                    response.ProcessSteps.Add(step2);
                }

                var specUnitItems = new SpecUnitItems();
                if (!response.HasErrors() && specUnitDocuments.Count > 0)
                {
                    var step2a = new ProcessStep("Get Spec Unit Items");
                    try
                    {
                        var query = specUnitDocuments.GroupBy(x => x.ConfigurationId)
                            .Select(group =>
                                new
                                {
                                    Name = group.Key,
                                    Record = group.OrderByDescending(x => x.TransactionDate).FirstOrDefault()
                                });

                        foreach (var group in query)
                        {
                            var specUnitItem = new SpecUnitItem();
                            specUnitItem.ConfigId = group.Record.ConfigurationId;
                            specUnitItem.ItemNum = group.Record.ItemNum;
                            specUnitItem.UnitType = group.Record.UnitType;
                            specUnitItem.StockType = group.Record.StockType;
                            specUnitItem.CMDate = group.Record.CMDate;
                            specUnitItem.TransactionDate = group.Record.TransactionDate;
                            specUnitItems.Add(specUnitItem);
                        }
                    }
                    catch (Exception e)
                    {
                        step2a.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2a.RowsAffected = specUnitItems.Count;
                        step2a.Done();
                        response.ProcessSteps.Add(step2a);
                    }

                    /********************************************************/
                    if (!response.HasErrors() && specUnitItems.Count > 0)
                    {
                        var step3 = new ProcessStep("Assign Spec Unit");
                        try
                        {
                            var j = 0;
                            foreach (var specUnitItem in specUnitItems)
                            {
                                j++;
                                if (j % 1000 == 0) Logger.Info($"{plant.PlantCode} Attributes {j}");

                                var itemAtts = _itemAttMgr.GetConfiguredItemAttributes(session, specUnitItem.ConfigId);
                                var salesLineAtts = _itemAttMgr.GetConfiguredItemSalesLineAttributes(session, specUnitItem.ConfigId);
                                foreach (var att in salesLineAtts)
                                {
                                    if (!itemAtts.Contains(att.Name))
                                    {
                                        itemAtts.Add(att);
                                    }
                                    else
                                    {
                                        itemAtts[att.Name].Value = att.Value;
                                        //Logger.Info($"{att.Name}");
                                    }
                                }
                                specUnitItem.ItemAttributes = itemAtts;
                            }
                        }
                        catch (Exception e)
                        {
                            step3.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3.RowsAffected = specUnitItems.Count;
                            step3.Done();
                            response.ProcessSteps.Add(step3);
                        }
                    }

                    /********************************************************/
                    var specUnitTable = new SpecUnitTable();
                    if (!response.HasErrors() && specUnitItems.Count > 0)
                    {
                        var step4 = new ProcessStep("Build SpecUnit Data Table");
                        try
                        {
                            specUnitTable.Add(specUnitItems, plant);
                        }
                        catch (Exception e)
                        {
                            step4.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step4.RowsAffected = specUnitTable.Rows.Count;
                            step4.Done();
                            response.ProcessSteps.Add(step4);
                        }
                    }

                    /********************************************************/
                    var specUnitDocumentTable = new SpecUnitDocumentTable();
                    if (!response.HasErrors() && specUnitDocuments.Count > 0)
                    {
                        var step4a = new ProcessStep("Build SpecUnitDocument Data Table");
                        try
                        {
                            specUnitDocumentTable.Add(specUnitDocuments, plant);
                        }
                        catch (Exception e)
                        {
                            step4a.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step4a.RowsAffected = specUnitDocumentTable.Rows.Count;
                            step4a.Done();
                            response.ProcessSteps.Add(step4a);
                        }

                        //if (!response.HasErrors())
                        //{
                        //    var stepTranDate = new ProcessStep("Set Transaction Date");
                        //    try
                        //    {
                        //        specUnitTable.SetTransactionDate(plant);
                        //        stepTranDate.RowsAffected = specUnitTable.Rows.Count;
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        stepTranDate.Errors.Add(new Error(e));
                        //    }
                        //    finally
                        //    {
                        //        stepTranDate.Done();
                        //        response.ProcessSteps.Add(stepTranDate);
                        //    }
                        //}

                        /********************************************************/
                        if (!response.HasErrors() && specUnitDocumentTable.Rows.Count > 0)
                        {
                            var step5 = new ProcessStep("Bulk Copy SpecUnitDocument");
                            var bulkResponse = new SqlBulkCopyResponse();
                            try
                            {
                                bulkResponse = BulkCopyManager.BulkTable(specUnitDocumentTable.ExtendedProperties["Target"].ToString(), specUnitDocumentTable, sqlCns["PLANT.INTEGRATION"]);
                            }
                            catch (Exception e)
                            {
                                step5.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step5.RowsAffected = (int) bulkResponse.RowsCopied;
                                step5.SqlBulkCopyResponses.Add(bulkResponse);
                                step5.Done();
                                response.RowsAffected = step5.RowsAffected;
                                response.AddStep(step5);
                            }
                        }

                        /********************************************************/
                        if (!response.HasErrors() && specUnitTable.Rows.Count > 0)
                        {
                            var step5a = new ProcessStep("Bulk Copy SpecUnit");
                            var bulkResponse = new SqlBulkCopyResponse();
                            try
                            {
                                bulkResponse = BulkCopyManager.BulkTable(specUnitTable.ExtendedProperties["Target"].ToString(), specUnitTable, sqlCns["PLANT.INTEGRATION"]);
                            }
                            catch (Exception e)
                            {
                                step5a.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step5a.RowsAffected = (int) bulkResponse.RowsCopied;
                                step5a.SqlBulkCopyResponses.Add(bulkResponse);
                                step5a.Done();
                                response.RowsAffected = step5a.RowsAffected;
                                response.AddStep(step5a);
                            }
                        }

                        /********************************************************/
                        var table1 = new ConfiguratorPartTable();
                        var stepF3215 = new ProcessStep("Get F3215 Data");
                        try
                        {
                            var cmd = session.DbCommands["SpecUnitMtoDownloadManager.GetF3215Data"];
                            cmd.Connection = session.DbConnection;
                            foreach (SpecUnitItem specUnit in specUnitItems)
                            {
                                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                                cmd.Parameters[1].Value = specUnit.ConfigId;
                                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                                table1.Add(reader, plant);
                            }
                            stepF3215.RowsAffected = table1.Rows.Count;
                        }
                        catch (Exception e)
                        {
                            stepF3215.Errors.Add(new Error(e));
                        }
                        stepF3215.Done();
                        response.AddStep(stepF3215);

                        /********************************************************/
                        if (!response.HasErrors() && table1.Rows.Count > 0)
                        {
                            var stepF3215Bulk = new ProcessStep("Bulk Copy Configurator Part");
                            SqlBulkCopyResponse bulkResponse = new SqlBulkCopyResponse();
                            try
                            {
                                if (table1.Rows.Count > 0)
                                {
                                    bulkResponse =
                                        BulkCopyManager.BulkTable(table1.ExtendedProperties["Target"].ToString(),
                                            table1, sqlCns["PLANT.INTEGRATION"]);
                                }
                            }
                            catch (Exception e)
                            {
                                stepF3215Bulk.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                stepF3215Bulk.RowsAffected = (int)bulkResponse.RowsCopied;
                                stepF3215Bulk.SqlBulkCopyResponses.Add(bulkResponse);
                                stepF3215Bulk.Done();
                                response.RowsAffected = stepF3215Bulk.RowsAffected;
                                response.AddStep(stepF3215Bulk);
                            }
                        }

                        /********************************************************/
                        if (useHighWaterMark && !response.HasErrors() && specUnitDocumentTable.Rows.Count > 0)
                        {
                            var step6 = new ProcessStep("Update High Water Mark");
                            try
                            {
                                var x = specUnitDocumentTable.MaxDate;
                                highWaterMark.EnterpriseDateTime = specUnitDocumentTable.MaxDate.AddHours(-plant.TimeOffset);
                                step6.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, highWaterMark, highWaterMark.EnterpriseDateTime));
                                step6.RowsAffected = 1;
                            }
                            catch (Exception e)
                            {
                                step6.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step6.Done();
                                response.AddStep(step6);
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
        public iDB2Command GetSpecUnitDocumentsPrepared()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                select 
                    RDZ58RID,
                    RDZ58DST,
	                CMEMCU,
                    CMCFGID,
                    CMKCOO,
                    CMDOCO,
                    CMDCTO,
                    CMLNID,
	                RDUPMJ,
	                RDTDAY,
					KZKIT,
					IBSRP4,
					IBSTKT
                from (
	                select 
		                RDZ58RID,
                        RDZ58DST,
	                    CMEMCU,
                        CMCFGID,
                        CMKCOO,
                        CMDOCO,
                        CMDCTO,
                        CMLNID,
	                    RDUPMJ,
	                    RDTDAY,
	                    KZKIT,
					    IBSRP4,
					    IBSTKT
	                from 
		                F3201 
                        LEFT OUTER JOIN F3211
                                ON KZCFGID = CMCFGID AND
                                KZCFGCID = 1 AND 
                                KZPEL = 40275	
				        INNER JOIN F4102
                            ON IBITM = KZKIT AND
                            IBMCU = CMEMCU 
		                INNER JOIN F580056 
		                    ON CMEMCU = RDMCU AND
			                CMDOCO = RDDOC1 AND 
			                CMDCTO = RDXDCTO AND 
			                CMKCOO = '*'
	                UNION
	                select 
		                RDZ58RID,
                        RDZ58DST,
	                    CMEMCU,
                        CMCFGID,
                        CMKCOO,
                        CMDOCO,
                        CMDCTO,
                        CMLNID,
	                    RDUPMJ,
	                    RDTDAY,
	                    KZKIT,
					    IBSRP4,
					    IBSTKT
	                from 
		                F3201 
                        LEFT OUTER JOIN F3211
                                ON KZCFGID = CMCFGID AND
                                KZCFGCID = 1 AND 
                                KZPEL = 40275	
				        INNER JOIN F4102
                            ON IBITM = KZKIT AND
                            IBMCU = CMEMCU 
		                INNER JOIN F580056 
		                ON	
			                CMEMCU = RDMCU AND
			                CMDOCO = RDDOCO AND 
			                CMDCTO = RDDCTO AND 
			                CMLNID = RDLNID AND
			                CMKCOO = RDKCOO 
	                UNION
	                select 
		                RDZ58RID,
                        RDZ58DST,
	                    CMEMCU,
                        CMCFGID,
                        CMKCOO,
                        CMDOCO,
                        CMDCTO,
                        CMLNID,
	                    RDUPMJ,
	                    RDTDAY,
	                    KZKIT,
					    IBSRP4,
					    IBSTKT
	                from 
		                F3201 
                        LEFT OUTER JOIN F3211
                                ON KZCFGID = CMCFGID AND
                                KZCFGCID = 1 AND 
                                KZPEL = 40275	
				        INNER JOIN F4102
                            ON IBITM = KZKIT AND
                            IBMCU = CMEMCU 
		                INNER JOIN F580056 
		                ON	
			                CMEMCU = RDMCU AND
			                CMDOCO = RDDOC2 AND 
			                CMDCTO = RDXCTO AND 
			                CMLNID = RDLNIC AND
			                CMKCOO = RDPKCO) A
                where 
                    CMEMCU IN (?,?,?,?) AND 
                    ((RDUPMJ = ? AND RDTDAY >= ? AND RDTDAY <= ?) OR (RDUPMJ >= ? AND RDUPMJ <= ?) OR (RDUPMJ = ? AND RDTDAY <= ?)) 
                ORDER BY
                    RDUPMJ, RDTDAY
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

        public SpecUnitDocuments GetSpecUnitDocumentsPrepared(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, Plant plant)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Debug($@"GetSpecUnitDocumentsPrepared PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            var specUnitDocuments = new SpecUnitDocuments();
            try
            {
                var cmd = e1Session.DbCommands["SpecUnitMtoDownloadManager.GetSpecUnitDocumentsPrepared"];
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
                        if (Data.CastDbNull.To<decimal>(reader["CMDOCO"], 0) > 0)
                        {
                            var specUnitDocument = new SpecUnitDocument();
                            specUnitDocument.ConfigurationId = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                            specUnitDocument.DocumentNum = Data.CastDbNull.To<decimal>(reader["CMDOCO"], 0);
                            specUnitDocument.DocumentType = Data.CastDbNull.To(reader["CMDCTO"], string.Empty).Trim();
                            specUnitDocument.DocumentLineNum = Data.CastDbNull.To<decimal>(reader["CMLNID"], 0) / 1000;
                            specUnitDocument.ItemNum = Data.CastDbNull.To<decimal>(reader["KZKIT"], 0);
                            specUnitDocument.StockType = Data.CastDbNull.To(reader["IBSTKT"], String.Empty).Trim();
                            specUnitDocument.UnitType = Data.CastDbNull.To(reader["IBSRP4"], String.Empty).Trim();
                            specUnitDocument.CMDate = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["RDUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["RDTDAY"], 0));
                            specUnitDocument.TransactionDate = specUnitDocument.CMDate.AddHours(plant.TimeOffset);
                            specUnitDocuments.Add(specUnitDocument);
                        }

                    reader.Close();
                }

                Logger.Info($"Rows: {specUnitDocuments.Count}");
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

            return specUnitDocuments;
        }

        public iDB2Command GetSpecUnitDocuments(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters) filterString.Append(filter);

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT
	                CMEMCU,
                    CMCFGID,
                    CMKCOO,
                    CMDOCO,
                    CMDCTO,
                    CMLNID,
	                CMUPMJ AS RDUPMJ,
	                CMTDAY AS RDUPMT,
	                KZKIT,
					IBSRP4,
					IBSTKT
                FROM 
	                F3201
                    LEFT OUTER JOIN F3211
                        ON KZCFGID = CMCFGID AND
                        KZCFGCID = 1 AND 
                        KZPEL = 40275	
					INNER JOIN F4102
                        ON IBITM = KZKIT AND
                        IBMCU = CMEMCU 
                WHERE 
                    {filterString}
                ORDER BY
                    CMUPMJ, CMTDAY
                FOR READ ONLY";
            return cmd;
        }

        public iDB2Command GetSpecUnitDocumentsByRack(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters) filterString.Append(filter);

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                select 
                    RDZ58RID,
                    RDZ58DST,
	                CMEMCU,
                    CMCFGID,
                    CMKCOO,
                    CMDOCO,
                    CMDCTO,
                    CMLNID,
	                RDUPMJ,
	                RDTDAY,
					KZKIT,
					IBSRP4,
					IBSTKT
                from (
	                select 
		                RDZ58RID,
                        RDZ58DST,
	                    CMEMCU,
                        CMCFGID,
                        CMKCOO,
                        CMDOCO,
                        CMDCTO,
                        CMLNID,
	                    RDUPMJ,
	                    RDTDAY,
	                    KZKIT,
					    IBSRP4,
					    IBSTKT
	                from 
		                F3201 
                        LEFT OUTER JOIN F3211
                                ON KZCFGID = CMCFGID AND
                                KZCFGCID = 1 AND 
                                KZPEL = 40275	
				        INNER JOIN F4102
                            ON IBITM = KZKIT AND
                            IBMCU = CMEMCU 
		                INNER JOIN F580056 
		                    ON CMEMCU = RDMCU AND
			                CMDOCO = RDDOC1 AND 
			                CMDCTO = RDXDCTO AND 
			                CMKCOO = '*'
	                UNION
	                select 
		                RDZ58RID,
                        RDZ58DST,
	                    CMEMCU,
                        CMCFGID,
                        CMKCOO,
                        CMDOCO,
                        CMDCTO,
                        CMLNID,
	                    RDUPMJ,
	                    RDTDAY,
	                    KZKIT,
					    IBSRP4,
					    IBSTKT
	                from 
		                F3201 
                        LEFT OUTER JOIN F3211
                                ON KZCFGID = CMCFGID AND
                                KZCFGCID = 1 AND 
                                KZPEL = 40275	
				        INNER JOIN F4102
                            ON IBITM = KZKIT AND
                            IBMCU = CMEMCU 
		                INNER JOIN F580056 
		                ON	
			                CMEMCU = RDMCU AND
			                CMDOCO = RDDOCO AND 
			                CMDCTO = RDDCTO AND 
			                CMLNID = RDLNID AND
			                CMKCOO = RDKCOO 
	                UNION
	                select 
		                RDZ58RID,
                        RDZ58DST,
	                    CMEMCU,
                        CMCFGID,
                        CMKCOO,
                        CMDOCO,
                        CMDCTO,
                        CMLNID,
	                    RDUPMJ,
	                    RDTDAY,
	                    KZKIT,
					    IBSRP4,
					    IBSTKT
	                from 
		                F3201 
                        LEFT OUTER JOIN F3211
                                ON KZCFGID = CMCFGID AND
                                KZCFGCID = 1 AND 
                                KZPEL = 40275	
				        INNER JOIN F4102
                            ON IBITM = KZKIT AND
                            IBMCU = CMEMCU 
		                INNER JOIN F580056 
		                ON	
			                CMEMCU = RDMCU AND
			                CMDOCO = RDDOC2 AND 
			                CMDCTO = RDXCTO AND 
			                CMLNID = RDLNIC AND
			                CMKCOO = RDPKCO) A
                where 
                    {filterString}
                FOR READ ONLY";
            return cmd;
        }

        public SpecUnitDocuments GetSpecUnitDocumentsDynamic(BusinessSystemSession session, Plant plant, ProcessRunSpan span, List<string> rackStatusCodes, decimal rackId, decimal configId, decimal documentNum, string documentType, List<string> woStatusCodes, decimal itemNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"GetSpecUnitDocumentsDynamic PLANT: {plant.PlantCode}-{plant.PlantNum} BEGIN: {startTime}");
            var specUnitDocuments = new SpecUnitDocuments();
            var cmd = new iDB2Command();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Don't allow to run wide-open
                if (!rackStatusCodes.Any() && documentNum == 0 && rackId == 0 && configId == 0 && span.IsEmpty && itemNum == 0)
                {
                    throw new Exception("If not using HighWaterMark, TimeSpan, a RackId or ConfigId or ItemNum or DocNum or RackStatusCode(s) are REQUIRED.");
                }

                filters.Add("CMEMCU IN ('          32', '          05', '          04', '          08') AND \r\n");

                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND \r\n");
                }

                // Document Num
                if (documentNum > 0)
                {
                    filters.Add("CMDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = documentNum;
                }

                // Document Type
                if (!string.IsNullOrEmpty(documentType))
                {
                    filters.Add("CMDCTO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
                    parameters.Last().Value = documentType;
                }

                // Config Id
                if (configId > 0)
                {
                    filters.Add("CMCFGID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = configId;
                }

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("RDZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }

                // Rack Status
                if (rackStatusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var rackStatusCode in rackStatusCodes) strInClause = $"{strInClause} '{rackStatusCode}',";
                    filters.Add("RDZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Work Order Status
                if (woStatusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var woStatusCode in woStatusCodes) strInClause = $"{strInClause} '{woStatusCode}',";
                    filters.Add("RDZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                if (itemNum > 0)
                {
                    filters.Add("KZKIT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("KZKIT", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                //if (rackId > 0 || rackStatusCodes.Any() || !span.IsEmpty)
                //{
                    cmd = GetSpecUnitDocumentsByRack(filters);
                //}
                //else
                //{
                //    cmd = GetSpecUnitDocuments(filters);
                //}

                foreach (var parameter in parameters) cmd.Parameters.Add(parameter);
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                Logger.Info(Logging.DbCmdString(cmd));

                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                        if (Data.CastDbNull.To<decimal>(reader["CMDOCO"], 0) > 0)
                        {
                            var specUnitDocument = new SpecUnitDocument();
                            specUnitDocument.ConfigurationId = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                            specUnitDocument.DocumentNum = Data.CastDbNull.To<decimal>(reader["CMDOCO"], 0);
                            specUnitDocument.DocumentType = Data.CastDbNull.To(reader["CMDCTO"], string.Empty).Trim();
                            specUnitDocument.DocumentLineNum = Data.CastDbNull.To<decimal>(reader["CMLNID"], 0) / 1000;
                            specUnitDocument.ItemNum = Data.CastDbNull.To<decimal>(reader["KZKIT"], 0);
                            specUnitDocument.StockType = Data.CastDbNull.To(reader["IBSTKT"], String.Empty).Trim();
                            specUnitDocument.UnitType = Data.CastDbNull.To(reader["IBSRP4"], String.Empty).Trim();
                            specUnitDocument.CMDate = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["RDUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["RDTDAY"], 0));
                            specUnitDocument.TransactionDate = specUnitDocument.CMDate.AddHours(plant.TimeOffset);
                            specUnitDocuments.Add(specUnitDocument);
                        }

                    reader.Close();
                }

                Logger.Info($"Rows: {specUnitDocuments.Count}");
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

            return specUnitDocuments;
        }

        private iDB2Command GetF3215Data()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT 
	                CCCFGID, 
	                CCITM, 
	                CCMCU, 
	                CCDSC1, 
                    CCLOCN,
                    CCDSC1,
	                CCQNTY, 
	                CCUOM, 
	                CCOPSQ, 
	                CCECST, 
	                CCUPMJ,
	                CCTDAY, 
	                ROW_NUMBER() OVER (ORDER BY CCCFGCID) AS CCCPNB
                FROM 
	                F3215 
                WHERE 
	                CCMCU = ? AND
                    CCCFGID = ? AND
	                CCCFGSID = ''
                ORDER BY 
	                CCCFGCID
                FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CFGID"].Precision = 15;
            cmd.Parameters["CFGID"].Scale = 0;
            return cmd;
        }
    }
}