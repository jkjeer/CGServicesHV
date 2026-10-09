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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Process
{
    public class UnitBusinessTransactionUploadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UnitBusinessTransactionUploadManager(BusinessSystemSession e1Session)
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
            var response = new ProcessResponse("CG Completion Upload Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var sfManager = new UnitBusinessTransactionManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            var items = new UnitBusinessTransactions();
            long sfedbt = 0;

            try
            {
                var stepInserts = new ProcessStep("Exec ebs.usp_Insert_UnitBusinessTransaction");
                try
                {
                    stepInserts.SqlDmlResponses.Add(sfManager.InsertBusinessTransactions(sqlCns["PLANT.SHOPFLOOR"]));
                    stepInserts.RowsAffected = stepInserts.SqlDmlResponses.Last().RowsAffected;
                    stepInserts.Messages.Add("Results", stepInserts.SqlDmlResponses.Last().Message, Message.SeverityType.Info);
                }
                catch (Exception e)
                {
                    stepInserts.Errors.Add(new Error(e));
                }
                finally
                {
                    stepInserts.Done();
                    response.AddStep(stepInserts);
                }

                var stepReserve = new ProcessStep("Reserve Shop Floor Records");
                try
                {
                    response.SqlDmlResponses.Add(sfManager.SetTransferStatus(sqlCns["PLANT.SHOPFLOOR"], "N", "R"));
                    stepReserve.RowsAffected = response.SqlDmlResponses.Last().RowsAffected;
                    response.RowsAffected = stepReserve.RowsAffected;
                }
                catch (Exception e)
                {
                    stepReserve.Errors.Add(new Error(e));
                }
                finally
                {
                    stepReserve.Done();
                    response.AddStep(stepReserve);
                }

                var stepGet = new ProcessStep("Get Shop Floor Records");
                try
                {
                    items = sfManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant, "R");
                    stepGet.RowsAffected = items.Count;
                    response.RowsAffected = items.Count;
                }
                catch (Exception e)
                {
                    stepGet.Errors.Add(new Error(e));
                }
                finally
                {
                    stepGet.Done();
                    response.AddStep(stepGet);
                }

                if (items.Count > 0)
                {
                    var stepValidate = new ProcessStep("Validate");
                    try
                    {
                        var validateMsgs = Validate(session, plant, items, f580030Mgr);
                        if (validateMsgs.Any())
                        {
                            stepValidate.Messages.Add(validateMsgs);
                            response.Messages.Add(validateMsgs);
                        }
                        stepValidate.RowsAffected = validateMsgs.Count();
                    }
                    catch (Exception e)
                    {
                        stepValidate.Errors.Add(new Error(e));
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        stepValidate.Done();
                        response.AddStep(stepValidate);
                    }
                }

                long qaukid = 0;
                if (items.Any(x => x.ProcessingStatus.TransferStatus == ProcessingStatus.TransferStatusCode.Reserved))
                {
                    var stepSequences = new ProcessStep("Assign Ids");
                    try
                    {
                        var sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", items.Count(x => x.ProcessingStatus.TransferStatus == ProcessingStatus.TransferStatusCode.Reserved));
                        sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                        qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
                        foreach (var item in items.Where(x => x.ProcessingStatus.TransferStatus == ProcessingStatus.TransferStatusCode.Reserved))
                        {
                            item.ProcessingStatus.TransferId = sfukid;
                            item.ProcessingStatus.TransferBatchId = sfedbt;
                            sfukid++;
                        }
                        stepSequences.RowsAffected = items.Count;
                    }
                    catch (Exception e)
                    {
                        stepSequences.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepSequences.Done();
                        response.AddStep(stepSequences);
                    }
                }

                var f580030s = new F580030s();
                if (items.Any(x => x.ProcessingStatus.TransferStatus == ProcessingStatus.TransferStatusCode.Reserved))
                {
                    var stepBuildF580030 = new ProcessStep("Build F580030s");
                    try
                    {
                        f580030s = sfManager.Transform(items.Where(x => x.ProcessingStatus.TransferStatus == ProcessingStatus.TransferStatusCode.Reserved).ToList());
                        stepBuildF580030.RowsAffected = f580030s.Count;
                        response.SerializedObjects.Add("F580030s", f580030s);
                    }
                    catch (Exception e)
                    {
                        stepBuildF580030.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepBuildF580030.Done();
                        response.AddStep(stepBuildF580030);
                    }
                }

                if (f580030s.Any())
                {
                    var stepSaveF580030 = new ProcessStep("Save F580030 Records to E1");
                    var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        stepSaveF580030.SqlDmlResponses.AddRange(f580030Mgr.Save(session, e1Trans, f580030s));
                        stepSaveF580030.RowsAffected = f580030s.Count;
                    }
                    catch (Exception e)
                    {
                        e1Trans.Rollback();
                        stepSaveF580030.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepSaveF580030.Done();
                        response.AddStep(stepSaveF580030);
                    }

                    var stepSaveF580030A = new ProcessStep("Submit E1 F580030A Trigger Processing Record");
                    try
                    {
                        var f580030A = f580030AMgr.Create("WC", qaukid, sfedbt, plant.PlantNum);
                        stepSaveF580030A.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans, f580030A));
                        e1Trans.Commit();
                        stepSaveF580030A.RowsAffected = stepSaveF580030A.SqlDmlResponses[0].RowsAffected;
                    }
                    catch (Exception e)
                    {
                        e1Trans.Rollback();
                        stepSaveF580030A.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepSaveF580030A.Done();
                        response.AddStep(stepSaveF580030A);
                    }
                }

                if (f580030s.Any())
                {
                    var stepSetTransferStatus = new ProcessStep("Set Transfer Status Success");
                    try
                    {
                        foreach (var item in items.Where(x => x.ProcessingStatus.TransferStatus == ProcessingStatus.TransferStatusCode.Reserved))
                        {
                            item.ProcessingStatus.ValidationStatus = ProcessingStatus.ValidationStatusCode.Success;
                            item.ProcessingStatus.TransferStatus = ProcessingStatus.TransferStatusCode.Success;
                            item.ProcessingStatus.TransferredDate = DateTime.Now;
                        }
                        stepSetTransferStatus.RowsAffected = items.Count(x =>
                            x.ProcessingStatus.TransferStatus == ProcessingStatus.TransferStatusCode.Success);
                    }
                    catch (Exception e)
                    {
                        stepSetTransferStatus.Errors.Add(new Error(e));
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        stepSetTransferStatus.Done();
                        response.AddStep(stepSetTransferStatus);
                    }
                }

                if (items.Any())
                {
                    var stepSaveTransferStatus = new ProcessStep("Set ShopFloor Transfer Status");
                    try
                    {
                        stepSaveTransferStatus.SqlDmlResponses.AddRange(
                            sfManager.SaveTransferProperties(items, 100, sqlCns["PLANT.SHOPFLOOR"]));
                    }
                    catch (Exception e)
                    {
                        stepSaveTransferStatus.Errors.Add(new Error(e));
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        stepSaveTransferStatus.Done();
                        response.AddStep(stepSaveTransferStatus);
                    }
                }
            }
            catch (Exception)
            {
                // Each Step already catches errors ... no need to throw again
                //  throw new Exception(e.Message);
            }

            if (items.Any() || response.HasErrors())
            {
                var stepSummary = new ProcessStep("Batch Summary Collection");
                try
                {
                    var serviceContext = ServiceContextManager.GetServiceContext();
                    response.SerializedObjects.Add("ServiceContext", serviceContext);

                    response.SerializedObjects.Add(new SerializedObject("UnitBusinessTransactions",
                        XmlSerializationUtility.SerializeObject(Encoding.UTF8, items)));

                    var headerSummary = new
                    {
                        Plant = plant.PlantCode,
                        BatchNum = sfedbt.ToString(),
                        Transactions = response.RowsAffected,
                        StartDate = response.StartDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                        EndDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                        ElapsedSeconds = (DateTime.Now - response.StartDate).TotalSeconds
                    };
                    response.SerializedObjects.Add("Header", headerSummary, null);
                }
                catch (Exception e)
                {
                    stepSummary.Errors.Add(new Error(e));
                }
                finally
                {
                    stepSummary.Done();
                    response.AddStep(stepSummary);
                }
            }

            // Send Mail
            if (items.Any() || response.HasErrors())
            {
                var stepMail = new ProcessStep("Send Mail");
                try
                {
                    var serializedXml = response.Serialize();
                    Logger.Info("Serialized Process: " + serializedXml);
                    var xslOutput = XsltManager.Transform(serializedXml,
                        "CGServicesHV.Common.Xslt.UnitBusinessTransactionUploadReport.xslt", Encoding.UTF8);
                    Logger.Info("Report XML: " + xslOutput);

                    var recipients = new List<string>();
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "LG_PRODUCTION", "ADMIN", null, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "LG_PRODUCTION", "PUBLIC", plant.PlantCode, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "LG_PRODUCTION", "USER", plant.PlantCode, null));
                    string mailPrefix = "";
                    if (response.HasErrors() || items.Any(x => x.ProcessingStatus.TransferStatus == ProcessingStatus.TransferStatusCode.Error))
                    {
                        mailPrefix = "ERROR - ";
                    }
                    else
                    {
                        if (items.Any(x => x.ProcessingStatus.ValidationStatus == ProcessingStatus.ValidationStatusCode.Failure))
                            mailPrefix = "WARNING - ";
                    }
                    var subject = $"{mailPrefix}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Unit Transactions [{sfedbt}]";

                    Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                    if (recipients.Count > 0)
                    {
                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            recipients,
                            null,
                            subject,
                            xslOutput);
                    }
                }
                catch (Exception e)
                {
                    stepMail.Errors.Add(new Error(e));
                }
                finally
                {
                    stepMail.Done();
                    response.AddStep(stepMail);
                }
            }
            response.Done();
            return response;
        }

        public static Messages Validate(BusinessSystemSession session, Plant plant, UnitBusinessTransactions items, F580030Manager mgr)
        {
            var messages = new Messages();
            foreach (var item in items)
            {
                if (mgr.Exists(session, plant.PlantNum, item.UnitBusinessTransactionId.ToString(), item.TransactionType))
                {
                    item.ProcessingStatus.ValidationStatus = ProcessingStatus.ValidationStatusCode.Failure;
                    item.ProcessingStatus.CanOverrideValidationFailure = false;
                    item.ProcessingStatus.TransferStatus = ProcessingStatus.TransferStatusCode.Skip;
                    item.ProcessingStatus.TransferMessage = $"Previously Uploaded to F580030";
                    messages.Add(new Message($"Transaction Id {item.TransactionId} - Duplicate", $"Previously Uploaded to F580030", Message.SeverityType.Error));
                }
            }
            return messages;
        }
    }
}
