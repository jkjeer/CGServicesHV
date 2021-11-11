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
    public class HvIssueUploadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HvIssueUploadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, string status)
        {
            var response = new ProcessResponse("HV Issue Upload Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var sfManager = new HvIssueItemManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            var items = new HvIssueItems();
            var validationItems = new List<HvIssueItem>();

            try
            {
                var stepGet = new ProcessStep("Get Shop Floor Records");
                try
                {
                    items = sfManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant, "N");
                    stepGet.RowsAffected = items.Count;
                    response.Messages.Add(new Message("SF HV Issue Records", items.Count.ToString()));
                }
                catch (Exception e)
                {
                    Logger.Error($@"ERROR: {sqlCns["PLANT.SHOPFLOOR"].ConnectionString}  {e.Message}: {e.StackTrace}");
                    stepGet.Errors.Add(new Error(e));
                    throw new Exception(e.Message);
                }
                finally
                {
                    stepGet.Done();
                    response.ProcessSteps.Add(stepGet);
                }

                // Validation
                if (items.Count > 0)
                {
                    var stepValidate = new ProcessStep("Validate");
                    try
                    {
                        foreach (var item in items)
                        {
                            if (f580030Mgr.Exists(session, item.PlantID, item.HVIssuesID.ToString(), "HI"))
                            {
                                item.Transferred = true;
                                item.TransferStatus = "E";
                                item.TransferMessage = "Previously Uploaded to F580030";
                            }
                        }
                        validationItems = items.Where(x => x.TransferStatus == "E").ToList();
                        items.RemoveAll(x => x.TransferStatus == "E");
                        response.SerializedObjects.Add("ValidationItems", validationItems);
                        stepValidate.RowsAffected = validationItems.Count;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                        stepValidate.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepValidate.Done();
                        response.ProcessSteps.Add(stepValidate);
                    }
                }

                long sfedbt = 0;
                // Assign Ids
                if (items.Count > 0 && !response.HasErrors())
                {
                    var stepSequences = new ProcessStep("Sequences");
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
                            var sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", batchGroup.Count());
                            foreach (var item in batchGroup)
                            {
                                item.TransferId = sfukid;
                                item.TransferBatchId = sfedbt;
                                item.PlantID = batchGroup.Key.PlantID;
                                sfukid++;
                                i++;
                            }
                        }
                        if (!i.Equals(items.Count))
                        {
                            Logger.Error($@"ERROR: UKIDs assigned ({i}) does not match UKIDs requested from sequence ({items.Count})");
                            stepSequences.Errors.Add(new Error($@"ERROR: UKIDs assigned ({i}) does not match UKIDs requested from sequence ({items.Count})"));
                        }

                        stepSequences.RowsAffected = f580030As.Count;
                        response.SerializedObjects.Add("HvIssueItems", items);
                        response.SerializedObjects.Add(new SerializedObject("F580030As", XmlSerializationUtility.SerializeObject(Encoding.UTF8, f580030As)));
                    }
                    catch (Exception e)
                    {
                        stepSequences.Errors.Add(new Error(e));
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        stepSequences.Done();
                        response.AddStep(stepSequences);
                    }

                    // Build E1 Interface Records
                    var f580030s = new F580030s();
                    if (!response.HasErrors())
                    {
                        var step4 = new ProcessStep("Build Objects");
                        try
                        {
                            f580030s = sfManager.Transform(items);
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
                        if (items.Count > 0 && !response.HasErrors())
                        {
                            var step5 = new ProcessStep("Save F580030 Records to E1");
                            var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
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
                            if (f580030As.Count > 0 && !response.HasErrors())
                            {
                                var step6 = new ProcessStep("Submit E1 F580030A Trigger Processing Record");
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
                // Update Shop Floor Completed
                if ((items.Count > 0 || validationItems.Count > 0) && !response.HasErrors())
                {
                    var step7 = new ProcessStep("Set Shop Floor Status");
                    try
                    {
                        var allItems = new HvIssueItems();
                        allItems.AddRange(validationItems);
                        allItems.AddRange(items);
                        var dmlResponses = sfManager.SaveTransferProperties(allItems, 100, sqlCns["PLANT.SHOPFLOOR"]);
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
                            StartDate = response.StartDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                            EndDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                            ElapsedSeconds = (DateTime.Now - response.StartDate).Seconds
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
                    var stepMail = new ProcessStep("Send Mail");
                    try
                    {
                        XElement root = new XElement("root", XElement.Load(new StringReader(XmlSerializationUtility.SerializeObject(Encoding.UTF8, response))));
                            foreach (var serializedObject in response.SerializedObjects)
                            {
                                root.Add(XElement.Load(new StringReader(serializedObject.Value)));
                            }
                            var serializedXml = root.ToString();
                            Logger.Info("Serialized Process: " + serializedXml);
                            var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.HvIssueUploadReport.xslt", Encoding.UTF8);
                            Logger.Info("Report XML: " + xslOutput);
                            Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                                System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                                System.Configuration.ConfigurationManager.AppSettings["MailBizToAddress"],
                                null,
                                plant.PlantCode + " [" + System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"] + "] " +
                                "HV Issue Upload ",
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
                Logger.Error($@"ERROR:{e.Message}: {e.StackTrace}");
                throw new FaultException(e.Message);
            }
            response.Done();
            return response;
        }
    }
}
