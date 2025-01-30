using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.ShopFloor;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Process
{
    public class WoMgmtUploadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public WoMgmtUploadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse ProcessByWO(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, string status)
        {
            var response = new ProcessResponse("WO Management Upload", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var sfManager = new WoMgmtItemManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            var items = new WoMgmtItems();
            var validationItems = new List<WoMgmtItem>();
            try
            {
                // Data Selection
                var step1 = new ProcessStep("Get Shop Floor Records");
                try
                {
                    items = sfManager.ExtractByWO(session, sqlCns["PLANT.SHOPFLOOR"], plant);
                    step1.RowsAffected = items.Count;
                    response.Messages.Add(new Message("WO Management Records", items.Count.ToString()));
                }
                catch (Exception e)
                {
                    Logger.Error($@"ERROR: {sqlCns["PLANT.SHOPFLOOR"].ConnectionString}  {e.Message}: {e.StackTrace}");
                    step1.Errors.Add(new Error(e));
                }
                finally
                {
                    step1.Done();
                    response.ProcessSteps.Add(step1);
                }

                // Validation
                if (items.Count > 0)
                {
                    var step2 = new ProcessStep("Validate");
                    try
                    {
                        foreach (var item in items)
                        {
                            if (f580030Mgr.Exists(session, item.PlantID, item.WOManagementID.ToString(), item.WODocType))
                            {
                                item.Transferred = true;
                                item.TransferStatus = "E";
                                item.TransferMessage = "Previously Uploaded to F580030";
                            }
                        }
                        validationItems = items.Where(x => x.TransferStatus == "E").ToList();
                        items.RemoveAll(x => x.TransferStatus == "E");
                        response.SerializedObjects.Add("ValidationItems", validationItems);
                        step2.RowsAffected = validationItems.Count;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                        step2.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2.Done();
                        response.ProcessSteps.Add(step2);
                    }
                }

                // Assign Ids
                long sfedbt = 0;
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step3 = new ProcessStep("Assign Ids");
                    long qaukid = 0;
                    var f580030As = new F580030As();
                    try
                    {
                        var i = 0;
                        var batchGroups = items.GroupBy(x => new {x.PlantID});
                        foreach (var batchGroup in batchGroups)
                        {
                            sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                            qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
                            f580030As.Add(f580030AMgr.Create("WC", qaukid, sfedbt, batchGroup.Key.PlantID));
                            var batchGroupCount = batchGroup.Count();
                            var sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", batchGroupCount);
                            foreach (var item in batchGroup)
                            {
                                item.TransferId = sfukid;
                                item.TransferBatchId = sfedbt;
                                item.PlantID = batchGroup.Key.PlantID;
                                sfukid++;
                                i++;
                            }
                        }
                        if ( !i.Equals(items.Count))
                        {
                            Logger.Error($@"ERROR: UKIDs assigned ({i}) does not match UKIDs requested from sequence ({items.Count})");
                            step3.Errors.Add(new Error($@"ERROR: UKIDs assigned ({i}) does not match UKIDs requested from sequence ({items.Count})"));
                        }
                        step3.RowsAffected = items.Count;
                        response.SerializedObjects.Add("WoMgmtItems", items);
                        response.SerializedObjects.Add(new SerializedObject("F580030As", XmlSerializationUtility.SerializeObject(Encoding.UTF8, f580030As)));
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.Done();
                        response.ProcessSteps.Add(step3);
                    }

                    // Build E1 Interface Records
                    var f580030s = new F580030s();
                    if (!response.HasErrors())
                    {
                        var step4 = new ProcessStep("Build Objects");
                        try
                        {
                            f580030s = sfManager.TransformByWO(items);
                            step4.RowsAffected = f580030s.Count;
                            response.RowsAffected = f580030s.Count;
                            response.Messages.Add(new Message("E1 F580030 Records", f580030s.Count.ToString()));
                            response.SerializedObjects.Add("F580030s", f580030s);
                        }
                        catch (Exception e)
                        {
                            Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                            step4.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step4.Done();
                            response.ProcessSteps.Add(step4);
                        }

                        // Save to E1
                        if (f580030s.Count > 0 && !response.HasErrors())
                        {
                            var step5 = new ProcessStep("Save Objects to E1");
                            iDB2Transaction e1Trans =
                                session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead) as iDB2Transaction;
                            try
                            {
                                step5.SqlDmlResponses.AddRange(f580030Mgr.Save(session, e1Trans, f580030s));
                                step5.RowsAffected = f580030s.Count;
                                response.RowsAffected = f580030s.Count;
                            }
                            catch (Exception e)
                            {
                                e1Trans.Rollback();
                                Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                                step5.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step5.Done();
                                response.ProcessSteps.Add(step5);
                            }

                            // Submit E1 processing record
                            if (!response.HasErrors())
                            {
                                var step6 = new ProcessStep("Submit E1 Trigger Processing Record");
                                try
                                {
                                    foreach (var f580030A in f580030As)
                                    {
                                        step6.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans, f580030A));
                                        step6.RowsAffected += 1;
                                    }

                                    e1Trans.Commit();
                                }
                                catch (Exception e)
                                {
                                    e1Trans.Rollback();
                                    Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                                    step6.Errors.Add(new Error(e));
                                }
                                finally
                                {
                                    step6.Done();
                                    response.ProcessSteps.Add(step6);
                                }
                            }
                        }

                    }
                }

                // Set Shop Floor Status
                if ((items.Count > 0 || validationItems.Count > 0) && !response.HasErrors())
                {
                    var step7 = new ProcessStep("Set Shop Floor Status");
                    try
                    {
                        //var allItems = new WoMgmtItems();
                        //allItems.AddRange(validationItems);
                        //allItems.AddRange(items);
                        items.AddRange(validationItems);
                        var dmlResponses =
                            sfManager.SaveTransferPropertiesByWO(items, 100, sqlCns["PLANT.SHOPFLOOR"]);
                        step7.RowsAffected = dmlResponses[0].RowsAffected;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                        step7.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step7.Done();
                        response.ProcessSteps.Add(step7);
                    }
                }

                // Summary Record
                if (items.Count > 0 || validationItems.Count > 0)
                {
                    var stepSummary = new ProcessStep("Batch Summary Collection");
                    try
                    {
                        var headerSummary = new
                        {
                            Location = $"{plant.PlantCode} - {plant.PlantNum}",
                            Plant = plant.PlantNum,
                            plant.PlantCode,
                            StartDate = response.ProcessSteps.Min(x => x.StartDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                            EndDate = response.ProcessSteps.Max(x => x.EndDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                            ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                        };
                        response.SerializedObjects.Add("SummaryHeader", headerSummary, null);
                        stepSummary.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                        stepSummary.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepSummary.Done();
                        response.ProcessSteps.Add(stepSummary);
                    }
                }

                // Send Mail
                if (items.Count > 0 || validationItems.Count > 0)
                {
                    response.Done();
                    var stepMail = new ProcessStep("Send Mail");
                    try
                    {
                        var serializedXml = response.Serialize();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.WoMgmtUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);

                        var recipients = new List<string>();
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "ADMIN"));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "PUBLIC", plant.PlantCode));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "USER", plant.PlantCode));

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            recipients,
                            null,
                            $"{(response.HasErrors() ? "ERROR - " : String.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] WO Mgmt Upload {(validationItems.Count > 0 ? "Validation" : "")} ",
                            xslOutput);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                        stepMail.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepMail.Done();
                        response.ProcessSteps.Add(stepMail);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error(e));
                throw new Exception(e.Message);
            }
            response.Done();
            return response;
        }

        public ProcessResponse ProcessBySched(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, string status)
        {
            var response = new ProcessResponse("WO Management (By Schedule) Upload", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var sfManager = new WoMgmtItemManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            WoMgmtItems items;
            var validationItems = new List<WoMgmtItem>();
            try
            {
                // Data Selection
                var step1 = new ProcessStep("Data Selection");
                try
                {
                    items = sfManager.ExtractBySched(session, sqlCns["PLANT.SHOPFLOOR"], plant);
                    step1.RowsAffected = items.Count;
                    response.Messages.Add(new Message("WO Management By Schedule Records", items.Count.ToString()));
                }
                catch (Exception e)
                {
                    Logger.Error($@"ERROR: {sqlCns["PLANT.SHOPFLOOR"].ConnectionString}  {e.Message}: {e.StackTrace}");
                    step1.Errors.Add(new Error(e));
                    throw new Exception(e.Message);
                }
                finally
                {
                    step1.Done();
                    response.ProcessSteps.Add(step1);
                }

                // Validation
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step2 = new ProcessStep("Validate");
                    try
                    {
                        foreach (var item in items)
                        {
                            string docType;
                            switch (item.WODocType)
                            {
                                case "RE":
                                    docType = "BX";
                                    break;
                                case "BO":
                                    docType = "BD";
                                    break;
                                default:
                                    docType = item.WODocType;
                                    break;
                            }

                            if (f580030Mgr.Exists(session, item.PlantID, item.WOManagementID.ToString(), docType))
                            {
                                item.Transferred = true;
                                item.TransferStatus = "E";
                                item.TransferMessage = "Previously Uploaded to F580030";
                                step2.Errors.Add(new Error("Previously Uploaded to F580030"));
                            }
                        }
                        validationItems = items.Where(x => x.TransferStatus == "E").ToList();
                        items.RemoveAll(x => x.TransferStatus == "E");
                        response.SerializedObjects.Add("ValidationItems", validationItems);
                        step2.RowsAffected = validationItems.Count;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                        step2.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2.Done();
                        response.ProcessSteps.Add(step2);
                    }
                }

                foreach (var item in items)
                {
                    response = new ProcessResponse("WO Management (By Schedule) Upload", plant);
                    response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                    foreach (var sqlCn in sqlCns)
                    {
                        response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
                    }

                    // Assign Ids
                    long sfedbt = 0;
                    long qaukid = 0;
                    if (!response.HasErrors())
                    {
                        var step3 = new ProcessStep("Assign Ids");
                        try
                        {
                            var sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", 1);
                            sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                            qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
                            item.TransferId = sfukid;
                            item.TransferBatchId = sfedbt;
                            step3.RowsAffected = 1;
                        }
                        catch (Exception e)
                        {
                            step3.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3.Done();
                            response.ProcessSteps.Add(step3);
                        }
                    }

                    // Build E1 Interface Records
                    var f580030 = new F580030();
                    if (!response.HasErrors())
                    {
                        var step4 = new ProcessStep("Build Object");
                        try
                        {
                            f580030 = sfManager.TransformBySched(item);
                            step4.RowsAffected = 1;
                            response.RowsAffected = 1;
                            response.Messages.Add(new Message("E1 F580030 Records", "1"));
                            response.SerializedObjects.Add("F580030", f580030);
                        }
                        catch (Exception e)
                        {
                            step4.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step4.Done();
                            response.ProcessSteps.Add(step4);
                        }
                    }

                    // Save to E1
                    if (!response.HasErrors())
                    {
                        var step5 = new ProcessStep("Save Object to E1");
                        var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                        try
                        {
                            step5.SqlDmlResponses.Add(f580030Mgr.Save(session, e1Trans as iDB2Transaction, f580030));
                            step5.RowsAffected = 1;
                            response.RowsAffected = 1;
                        }
                        catch (Exception e)
                        {
                            e1Trans.Rollback();
                            step5.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step5.Done();
                            response.ProcessSteps.Add(step5);
                        }

                        // Submit E1 processing record
                        if (!response.HasErrors())
                        {
                            var step6 = new ProcessStep("Submit E1 Trigger Processing Record");
                            try
                            {
                                var f580030A = f580030AMgr.Create(f580030.SFEDCT, qaukid, sfedbt, item.PlantID,
                                    f580030.SFZ58SCN);
                                step6.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans, f580030A));
                                e1Trans.Commit();
                                step6.RowsAffected = 1;
                            }
                            catch (Exception e)
                            {
                                e1Trans.Rollback();
                                step6.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step6.Done();
                                response.ProcessSteps.Add(step6);
                            }
                        }
                    }

                    // Set Shop Floor Status
                    var step7 = new ProcessStep("Set Shop Floor Status");
                    try
                    {
                        var dmlResponses = sfManager.SaveTransferPropertiesBySched(item, 100, sqlCns["PLANT.SHOPFLOOR"]);
                        step7.RowsAffected = dmlResponses[0].RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step7.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step7.Done();
                        response.ProcessSteps.Add(step7);
                    }

                    // Summary Record
                    var stepSummary = new ProcessStep("Batch Summary Collection");
                    try
                    {
                        var headerSummary = new
                        {
                            Location = $"{plant.PlantCode} - {plant.PlantNum}",
                            Plant = plant.PlantNum,
                            plant.PlantCode,
                            BatchNum = sfedbt.ToString(),
                            ScheduleNum = f580030.SFZ58SCN,
                            StartDate = response.ProcessSteps.Min(x => x.StartDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                            EndDate = response.ProcessSteps.Max(x => x.EndDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                            ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                        };
                        response.SerializedObjects.Add("SummaryHeader", headerSummary, null);
                        stepSummary.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        stepSummary.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepSummary.Done();
                        response.ProcessSteps.Add(stepSummary);
                    }

                    // Send Mail
                    response.Done();
                    var stepMail = new ProcessStep("Send Mail");
                    try
                    {
                        var serializedXml = response.Serialize();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml,
                            "CGServicesHV.Common.Xslt.WoMgmtUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);

                        var recipients = new List<string>();
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "ADMIN"));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "PUBLIC", plant.PlantCode));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "USER", plant.PlantCode));

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            recipients,
                            null,
                            $"{(response.HasErrors() ? "ERROR - " : String.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] WO Mgmt Upload {(validationItems.Count > 0 ? "Validation" : "")} ",
                            xslOutput);
                    }
                    catch (Exception e)
                    {
                        stepMail.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepMail.Done();
                        response.ProcessSteps.Add(stepMail);
                    }
                }

                foreach (var item in validationItems)
                {
                    // Set Shop Floor Status
                    if (!response.HasErrors())
                    {
                        var step7 = new ProcessStep("Set Shop Floor Status");
                        try
                        {
                            var dmlResponses = sfManager.SaveTransferPropertiesBySched(item, 100, sqlCns["PLANT.SHOPFLOOR"]);
                            step7.RowsAffected = dmlResponses[0].RowsAffected;
                        }
                        catch (Exception e)
                        {
                            step7.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step7.Done();
                            response.ProcessSteps.Add(step7);
                        }
                    }

                    // Summary Record
                    var stepSummary = new ProcessStep("Batch Summary Collection");
                    try
                    {
                        var headerSummary = new
                        {
                            Location = $"{plant.PlantCode} - {plant.PlantNum}",
                            Plant = plant.PlantNum,
                            plant.PlantCode,
                            BatchNum = "",
                            item.ScheduleNum,
                            StartDate = response.ProcessSteps.Min(x => x.StartDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                            EndDate = response.ProcessSteps.Max(x => x.EndDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                            ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                        };
                        response.SerializedObjects.Add("SummaryHeader", headerSummary, null);
                        stepSummary.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        stepSummary.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepSummary.Done();
                        response.ProcessSteps.Add(stepSummary);
                    }

                    // Send Mail
                    response.Done();
                    var stepMail = new ProcessStep("Send Mail");
                    try
                    {
                        var serializedXml = response.Serialize();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml,
                            "CGServicesHV.Common.Xslt.WoMgmtUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);

                        var recipients = new List<string>();
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "ADMIN"));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "PUBLIC", plant.PlantCode));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "WO_MGMT", "USER", plant.PlantCode));

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            recipients,
                            null,
                            $"{(response.HasErrors() ? "ERROR - " : String.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] WO Mgmt Upload Validation",
                            xslOutput);
                    }
                    catch (Exception e)
                    {
                        stepMail.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepMail.Done();
                        response.ProcessSteps.Add(stepMail);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error(e));
                throw new Exception(e.Message);
            }
            response.Done();
            return response;
        }

    }
}
