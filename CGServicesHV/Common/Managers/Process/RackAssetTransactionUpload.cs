using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

namespace CGServicesHV.Common.Managers.Process
{
    public class RackAssetTransactionUpload
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackAssetTransactionUpload(BusinessSystemSession e1Session)
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
            var response = new ProcessResponse("Rack Asset Transaction Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var sfManager = new RackAssetTransactionItemManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            try
            {
                // Set Data Selection
                //var step1 = new ProcessStep("Set Data Selection");
                //try
                //{
                //    step1.SqlDmlResponses.Add(sfManager.SetTransferStatus(cn, "N", "Q"));
                //    step1.RowsAffected = step1.SqlDmlResponses[0].RowsAffected;
                //}
                //catch (Exception e)
                //{
                //    step1.Errors.Add(new Error(e));
                //}

                // Data Selection
                var step2 = new ProcessStep("Data Selection");
                var items = new RackAssetTransactionItems();
                var validationItems = new List<RackAssetTransactionItem>();
                try
                {
                    items = sfManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant);
                    step2.RowsAffected = items.Count;
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

                // Validation
                if (items.Count > 0)
                {
                    var step3 = new ProcessStep("Validate");
                    try
                    {
                        foreach (var item in items)
                        {
                            item.TransferStatus = "Y";
                            if (f580030Mgr.Exists(session, plant.PlantNum, item.RackAssetTransactionId.ToString(), item.TransactionType))
                            {
                                item.TransferStatus = "E";
                                item.TransferMessage = "Previously Uploaded to F580030";
                            }
                        }
                        validationItems = items.Where(x => x.TransferStatus == "E").ToList();
                        items.RemoveAll(x => x.TransferStatus == "E");

                        response.SerializedObjects.Add("ValidationItems", validationItems);
                        step3.RowsAffected = validationItems.Count;
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

                // Assign Ids
                long sfukid = 0;
                long sfedbt = 0;
                long qaukid = 0;
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step4 = new ProcessStep("Assign Ids");
                    try
                    {
                        sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", items.Count);
                        sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                        qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");

                        if (sfukid == 0)
                        {
                            step4.Errors.Add(new Error("SFUKID = 0"));
                        }
                        if (sfedbt == 0)
                        {
                            step4.Errors.Add(new Error("SFEDBT = 0"));
                        }
                        if (qaukid == 0)
                        {
                            step4.Errors.Add(new Error("QAUKID = 0"));
                        }
                        if (step4.Errors.Count == 0)
                        {
                            foreach (var item in items)
                            {
                                item.TransferId = sfukid;
                                item.BatchNum = sfedbt.ToString();
                                sfukid++;
                            }
                            step4.RowsAffected = items.Count;
                            response.SerializedObjects.Add("RackAssetTransactionItems", items);
                        }
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

                // Build Objects
                var f580030s = new F580030s();
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step5 = new ProcessStep("Build Objects");
                    try
                    {
                        f580030s = sfManager.Transform(items, plant);
                        response.RowsAffected = f580030s.Count;
                        response.SerializedObjects.Add("F580030s", f580030s);
                    }
                    catch (Exception e)
                    {
                        step5.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step5.Done();
                        response.ProcessSteps.Add(step5);
                    }
                }

                // Save to E1
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step6 = new ProcessStep("Save Objects to E1");
                    var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        step6.SqlDmlResponses.AddRange(f580030Mgr.Save(session, e1Trans, f580030s));
                        e1Trans.Commit();
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

                // Submit E1 processing record
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step7 = new ProcessStep("Submit E1 Trigger Processing Record");
                    var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        var f580030A = f580030AMgr.Create("WC", qaukid, sfedbt, plant.PlantNum);
                        step7.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans, f580030A));
                        e1Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        e1Trans.Rollback();
                        step7.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step7.Done();
                        response.ProcessSteps.Add(step7);
                    }
                }

                // Set Shop Floor Status
                if ((items.Count > 0 && !response.HasErrors()) || validationItems.Count > 0)
                {
                    var step8 = new ProcessStep("Set Shop Floor Status");
                    try
                    {
                        var allItems = new RackAssetTransactionItems();
                        allItems.AddRange(validationItems);
                        allItems.AddRange(items);
                        var dmlResponses = sfManager.SaveTransferProperties(allItems, 100, sqlCns["PLANT.SHOPFLOOR"]);
                        //step8.SqlDmlResponses.AddRange(dmlResponses);
                        step8.RowsAffected = dmlResponses[0].RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step8.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step8.Done();
                        response.ProcessSteps.Add(step8);
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
                            PlantCode = plant.PlantCode,
                            BatchNum = sfedbt,
                            StartDate = response.StartDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                            EndDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                            ElapsedSeconds = (DateTime.Now - response.StartDate).TotalSeconds
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
                        var serializedXml = response.Serialize();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml,
                            "CGServicesHV.Common.Xslt.RackAssetTransactionUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);
                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            "jpjenkin@cardinalcorp.com",
                            null,
                            $"{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Rack Asset Upload {(validationItems.Count > 0 ? " **REVIEW** " : "")} ",
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
                throw new Exception(e.Message);
            }
            finally
            {
                response.Done();
                Logging.InsertProcessLog(response);
            }
            return response;
        }

    }
}
