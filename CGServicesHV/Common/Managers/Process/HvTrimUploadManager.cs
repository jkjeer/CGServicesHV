using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.ShopFloor;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

namespace CGServicesHV.Common.Managers.Process
{
    public class HvTrimUploadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HvTrimUploadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"GetF580200Single", GetF580200Single()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, string status)
        {
            var response = new ProcessResponse("HV Trim Upload Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));

            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var sfManager = new HvTrimItemManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            try
            {
                // Data Selection
                var step1 = new ProcessStep("Get Shop Floor Records");
                var items = new HvTrimItems();
                var validationItems = new List<HvTrimItem>();
                try
                {
                    items = sfManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant, "N");
                    step1.RowsAffected = items.Count;
                    response.Messages.Add(new Message("SF HV Receipt Records", items.Count.ToString()));
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
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
                            if (f580030Mgr.Exists(session, plant.PlantNum, item.HVTrimID.ToString(), "HT"))
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
                long qaukid = 0;
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step3 = new ProcessStep("Assign Ids");
                    try
                    {
                        // Get E1 Required Sequences
                        var sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", items.Count);
                        sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                        qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
                        foreach (var item in items)
                        {
                            item.TransferId = sfukid;
                            item.TransferBatchId = sfedbt;
                            sfukid++;
                        }
                        step3.RowsAffected = items.Count;
                        response.SerializedObjects.Add("HvTrimItems", items);
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
                var f580030s = new F580030s();
                if (items.Count > 0 && !response.HasErrors())
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
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.ProcessSteps.Add(step4);
                    }
                }

                // Save to E1
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step5 = new ProcessStep("Save F580030 Records to E1");
                    var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        step5.SqlDmlResponses.AddRange(f580030Mgr.Save(session, e1Trans, f580030s));
                        e1Trans.Commit();
                        step5.RowsAffected = f580030s.Count;
                        response.RowsAffected = f580030s.Count;
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
                }

                // Submit E1 processing record
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step6 = new ProcessStep("Submit E1 F580030A Trigger Processing Record");
                    var e1Trans1 = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        var f580030A = f580030AMgr.Create("WC", qaukid, sfedbt, plant.PlantNum);
                        step6.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans1, f580030A));
                        e1Trans1.Commit();
                        step6.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        e1Trans1.Rollback();
                        step6.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step6.Done();
                        response.ProcessSteps.Add(step6);
                    }
                }

                // Update Shop Floor Completed
                if ((items.Count > 0 || validationItems.Count > 0) && !response.HasErrors())
                {
                    var step7 = new ProcessStep("Set Shop Floor Status");
                    try
                    {
                        var allItems = new HvTrimItems();
                        allItems.AddRange(validationItems);
                        allItems.AddRange(items);
                        var dmlResponses = sfManager.SaveTransferProperties(allItems, 100, sqlCns["PLANT.SHOPFLOOR"]);
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
                if (items.Count > 0 || validationItems.Count > 0)
                {
                    var stepSummary = new ProcessStep("Batch Summary Collection");
                    try
                    {
                        var headerSummary = new
                        {
                            Plant = plant.PlantNum,
                            plant.PlantCode,
                            BatchNum = sfedbt,
                            StartDate = response.StartDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                            EndDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                            ElapsedSeconds = (DateTime.Now - response.StartDate).Seconds
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
                        var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.HvTrimUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);
                        var subject = $@"{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] HV Trim Upload ";
                        Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            System.Configuration.ConfigurationManager.AppSettings["MailBizToAddress"],
                            null,
                            subject,
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
                throw new FaultException(e.Message);
            }
            response.Done();
            return response;
        }
    }
}
