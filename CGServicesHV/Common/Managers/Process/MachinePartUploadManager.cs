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
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Process
{
    public class MachinePartUploadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MachinePartUploadManager(BusinessSystemSession e1Session)
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
            var response = new ProcessResponse("Machine Part Consumption", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var sfManager = new MachinePartManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            try
            {
                // Set Eligible Extract Records
                var step1 = new ProcessStep("SF Set Transfer Status");
                try
                {
                    step1.SqlDmlResponses.Add(sfManager.SetTransferStatus(sqlCns["PLANT.SHOPFLOOR"], "N", "Q"));
                    step1.RowsAffected = step1.SqlDmlResponses.Last().RowsAffected;
                    response.RowsAffected = step1.RowsAffected;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);

                // Extract Records
                var selectedItems = new List<MachinePartConsumedItem>();
                if (response.RowsAffected > 0 && !response.HasErrors())
                {
                    var step2 = new ProcessStep("SF Extract");
                    try
                    {
                        selectedItems = sfManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant, "Q");
                        step2.RowsAffected = selectedItems.Count;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }
                    step2.Done();
                    response.AddStep(step2);
                }

                // Validate Shop Floor Transaction
                var validationItems = new List<MachinePartConsumedItem>();
                if (selectedItems.Count > 0 && !response.HasErrors())
                {
                    var step3 = new ProcessStep("Validate Items");
                    try
                    {
                        foreach (var item in selectedItems)
                        {
                            if (f580030Mgr.Exists(session, plant.PlantNum, item.ConsumedId.ToString(), "HM"))
                            {
                                item.TransferStatus = "E";
                                item.TransferMessage = "Previously Uploaded to F580030";
                                validationItems.Add(item);
                            }
                        }
                        selectedItems.RemoveAll(x => x.TransferStatus == "E");
                        step3.RowsAffected = validationItems.Count;
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    step3.Done();
                    response.AddStep(step3);
                }

                // Get E1 Required Sequences
                long sfukid = 0;
                long sfedbt = 0;
                long qaukid = 0;
                if (selectedItems.Count > 0)
                {
                    var step4 = new ProcessStep("Get Sequences");
                    try
                    {
                        sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", selectedItems.Count);
                        sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                        qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
                        foreach (var item in selectedItems)
                        {
                            if (item.TransferStatus != "E")
                            {
                                item.TransferId = sfukid;
                                item.TransferBatchNum = sfedbt;
                                sfukid++;
                            }
                        }
                        step4.RowsAffected = selectedItems.Count;
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    step4.Done();
                    response.AddStep(step4);
                }

                // Transform E1 Records
                F580030s f580030s = new F580030s();
                if (selectedItems.Count > 0 && !response.HasErrors())
                {
                    var step5 = new ProcessStep("Transform F580030 Records");
                    try
                    {
                        f580030s = sfManager.Transform(selectedItems);
                        response.SerializedObjects.Add("F580030s", f580030s);
                        step5.RowsAffected = f580030s.Count;
                    }
                    catch (Exception e)
                    {
                        step5.Errors.Add(new Error(e));
                    }
                    step5.Done();
                    response.AddStep(step5);
                }

                // Save to E1
                if (selectedItems.Count > 0)
                {
                    var step6 = new ProcessStep("Save F580030 Records");
                    //var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        foreach (var f580030 in f580030s)
                        {
                            var dmlResponse = f580030Mgr.Save(session, null, f580030);
                            var item = selectedItems.First(x => x.TransferId == f580030.SFUKID);
                            item.TransferDate = DateTime.Now;
                            if (dmlResponse.Errors.Any())
                            {
                                item.TransferStatus = "E";
                                item.TransferMessage = dmlResponse.Errors[0].Message;
                                validationItems.Add(item);
                                selectedItems.Remove(item);
                            }
                            else
                            {
                                item.TransferStatus = "Y";
                                item.TransferMessage = "";
                            }
                            step6.SqlDmlResponses.Add(dmlResponse);
                        }
                        step6.RowsAffected = f580030s.Count;
                    }
                    catch (Exception e)
                    {
                        step6.Errors.Add(new Error(e));
                    }
                    step6.Done();
                    response.AddStep(step6);
                }

                // Submit E1 processing record
                if (selectedItems.Count > 0)
                {
                    var step7 = new ProcessStep("Save F580030A Record");
                    var e1Trans1 = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        var f580030A = f580030AMgr.Create("WC", qaukid, sfedbt, plant.PlantNum);
                        step7.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans1, f580030A));
                        e1Trans1.Commit();
                        step7.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        e1Trans1.Rollback();
                        step7.Errors.Add(new Error(e));
                    }
                    step7.Done();
                    response.AddStep(step7);
                }

                // Update Shop Floor Completed
                var allItems = new List<MachinePartConsumedItem>();
                allItems.AddRange(selectedItems);
                allItems.AddRange(validationItems);
                if (allItems.Count > 0)
                {
                    var step8 = new ProcessStep("Set SF Transfer Properties");
                    try
                    {
                        step8.SqlDmlResponses.AddRange(sfManager.SaveTransferProperties(allItems, 100, sqlCns["PLANT.SHOPFLOOR"]));
                        step8.RowsAffected = step8.SqlDmlResponses.Count();
                    }
                    catch (Exception e)
                    {
                        step8.Errors.Add(new Error(e));
                    }
                    step8.Done();
                    response.AddStep(step8);
                }

                // Summary Record
                if (allItems.Count > 0)
                {
                    var step8a = new ProcessStep("Batch Summary Collection");
                    try
                    {
                        var headerSummary = new
                        {
                            Plant = plant.PlantNum,
                            PlantCode = plant.PlantCode,
                            BatchNum = selectedItems.Select(x => x.TransferBatchNum).FirstOrDefault(),
                            StartDate = response.StartDate,
                            EndDate = DateTime.Now,
                            ElapsedSeconds = (DateTime.Now - response.StartDate).TotalSeconds
                        };
                        response.SerializedObjects.Add("Header", headerSummary, null);
                        step8a.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        step8a.Errors.Add(new Error(e));
                    }
                    step8a.Done();
                    response.ProcessSteps.Add(step8a);
                }

                // Finalize Response
                response.SerializedObjects.Add("Items", allItems);
                response.Done();

                // Send Mail
                if (allItems.Count > 0 || response.HasErrors())
                {
                    var step9 = new ProcessStep("Send Mail");
                    try
                    {
                        var serializedXml = response.Serialize();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml,
                            "CGServicesHV.Common.Xslt.MachinePartUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);
                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            System.Configuration.ConfigurationManager.AppSettings["MailMachinePartAddress"],
                            null,
                            plant.PlantCode + " [" +
                            System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"] + "] " +
                            "Machine Part Upload ",
                            xslOutput);
                    }
                    catch (Exception e)
                    {
                        step9.Errors.Add(new Error(e));
                    }
                    step9.Done();
                    response.AddStep(step9);
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        //public Errors Validate1(BusinessSystemSession session, Plant plant, MachinePartConsumedItems items, F580030Manager mgr)
        //{
        //    var errors = new Errors();
        //    foreach (var item in items)
        //    {
        //        if (mgr.Exists(session, plant.PlantNum, item.ConsumedId.ToString(), "HM"))
        //        {
        //            item.TransferStatus = "E";
        //            item.TransferMessage = "Previously Uploaded to F580030";
        //            errors.Add(new Error($"MachinePartConsumedId: {item.ConsumedId} Previously Uploaded to F580030", false));
        //        }
        //    }
        //    return errors;
        //}

        //public Errors Validate2(BusinessSystemSession session, Plant plant, MachinePartConsumedItems items, F580030Manager mgr)
        //{
        //    var errors = new Errors();
        //    foreach (var item in items)
        //    {
        //        if (mgr.UKIDExists(session, item.TransferId))
        //        {
        //            item.TransferStatus = "E";
        //            item.TransferMessage = "Duplicate SFUKID Value";
        //            errors.Add(new Error($"{item.TransferMessage} {item.TransferId}", false));
        //        }
        //    }
        //    return errors;
        //}
    }
}
