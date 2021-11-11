using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.ShopFloor;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Process
{
    public class ScheduleBackoutUploadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, ScheduleEvent evt)
        {
            var response = new ProcessResponse($"Upload Backout Schedule {plant.PlantCode} - {evt.PlantNum} {evt.ScheduleNum}", true);
            long sfukid = 0;
            long sfedbt = 0;
            long qaukid = 0;

            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                foreach (var sqlCn in sqlCns)
                {
                    response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
                }

                var step1 = new ProcessStep("Lock ShopFloor");
                try
                {
                    var dmlResponse = ScheduleBackoutEventManager.UpdateStatus(sqlCns["PLANT.SHOPFLOOR"], evt, "U");
                    step1.SqlDmlResponses.Add(dmlResponse);
                    step1.RowsAffected = dmlResponse.RowsAffected;
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

                var step1a = new ProcessStep("Get E1 Schedule Event");
                try
                {
                    var bizEvents = ScheduleEventManager.GetEvents(session, plant, null, evt.ScheduleNum, new List<string>());
                    if (bizEvents.Any())
                    {
                        evt.BatchNum = bizEvents[0].BatchNum;
                    }
                    step1a.RowsAffected = bizEvents.Count;
                }
                catch (Exception e)
                {
                    step1a.Errors.Add(new Error(e));
                }
                step1a.Done();
                response.ProcessSteps.Add(step1a);


                var step1b = new ProcessStep("Validate Schedule");
                try
                {
                    var messages = Validate(session, evt);
                    step1b.Errors.Add(messages.Errors(), false);
                    response.Errors.Add(messages.Errors());
                    response.Messages.Add(messages.Warnings());
                    response.SerializedObjects.Add("ValidationMessages", messages);
                }
                catch (Exception e)
                {
                    step1b.Errors.Add(new Error(e));
                }
                finally
                {
                    step1b.Done();
                    response.ProcessSteps.Add(step1b);
                }

                if (!response.HasErrors())
                {

                    //var step2 = new ProcessStep("Set F580020 Status");
                    //try
                    //{
                    //    var sqlDmlResponse = F580020Manager.UpdateStatus(session, evt, "E", "Cancelled");
                    //    step2.SqlDmlResponses.Add(sqlDmlResponse);
                    //}
                    //catch (Exception e)
                    //{
                    //    step2.Errors.Add(new Error(e));
                    //}

                    //step2.Done();
                    //response.ProcessSteps.Add(step2);

                    var step3 = new ProcessStep("Get Sequences");
                    try
                    {
                        sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", 1);
                        sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                        qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
                        step3.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }

                    step3.Done();
                    response.AddStep(step3);

                    var step4 = new ProcessStep("F580030 BX Transaction");
                    var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        int edtn = 1;
                        var f580030Mgr = new F580030Manager(session);
                        var f580030 = new F580030();
                        f580030.SFUKID = sfukid;
                        f580030.SFEDUS = "SVCSOA";
                        f580030.SFEDBT = sfedbt.ToString();
                        f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                        f580030.SFEDLN = 1000;
                        f580030.SFEDCT = "BX";
                        f580030.SFTYTN = "CGMTO";
                        f580030.SFEDFT = "CGBX";
                        f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                        f580030.SFDRIN = "1";
                        f580030.SFEDDL = 0;
                        f580030.SFEDSP = "";
                        f580030.SFPNID = "Backout";
                        f580030.SFTNAC = "01";
                        f580030.SFMMCU = evt.Plant.PlantNum.PadLeft(12);
                        f580030.SFSTTM = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        f580030.SFDTMS = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        f580030.SFZ58SCN = evt.ScheduleNum;
                        f580030.SFACDT = evt.ScheduleDateE1;
                        f580030.SFUSER = "SVCSOA";
                        f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                        f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
                        var f580030Response = f580030Mgr.Save(session, e1Trans, f580030);
                        e1Trans.Commit();
                        step4.SqlDmlResponses.Add(f580030Response);
                    }
                    catch (Exception e)
                    {
                        e1Trans.Rollback();
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.ProcessSteps.Add(step4);
                    }

                    var step5 = new ProcessStep("F580030A Record");
                    try
                    {
                        var f580030AMgr = new F580030AManager(session);
                        var f580030A = new F580030A();
                        f580030A.QAUKID = qaukid;
                        f580030A.QAEDCT = "BX";
                        f580030A.QAZ58SCN = evt.ScheduleNum;
                        f580030A.QAMCU = evt.Plant.PlantNum.PadLeft(12, ' ');
                        f580030A.QAEDBT = sfedbt.ToString(CultureInfo.InvariantCulture);
                        f580030A.QACRTU = "SVCSOA";
                        f580030A.QACPGM = "SVCSOA";
                        f580030A.QAJOBN = "SVCSOA";
                        f580030A.QACRTDT = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
                        f580030A.QACRTM = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
                        var f580030Response = f580030AMgr.Save(session, null, f580030A);
                        step3.SqlDmlResponses.Add(f580030Response);
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

                var step7 = new ProcessStep("Update ShopFloor Event Transfer Status");
                try
                {
                    evt.TransferBatchNum = sfedbt.ToString();
                    evt.TransferTransactionId = sfukid;
                    if (response.HasErrors())
                    {
                        evt.TransferStatusCode = "E";
                        evt.TransferMessage = response.Errors[0]?.Message;
                        var dmlResponse = ScheduleBackoutEventManager.SaveTransfer(sqlCns["PLANT.SHOPFLOOR"], evt);
                        step7.RowsAffected = dmlResponse.RowsAffected;
                        step7.SqlDmlResponses.Add(dmlResponse);
                        ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "BACKOUT", "FAILURE");
                    }
                    else
                    {
                        evt.TransferStatusCode = "Y";
                        var dmlResponse = ScheduleBackoutEventManager.SaveTransfer(sqlCns["PLANT.SHOPFLOOR"], evt);
                        step7.RowsAffected = dmlResponse.RowsAffected;
                        step7.SqlDmlResponses.Add(dmlResponse);
                        ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "BACKOUT", "SUCCESS");
                    }
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

                var step8 = new ProcessStep("Summary Collections");
                try
                {
                    var headerSummary = new
                    {
                        Location = $"{evt.Plant.PlantCode} - {evt.Plant.PlantNum}",
                        ScheduleNum = evt.ScheduleNum,
                        ScheduleDate = evt.ScheduleDate.ToString("MM/dd/yyyy"),
                        BatchNum = evt.BatchNum,
                        System = evt.ShopFloorSystem,
                        StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                        EndDateString = response.ProcessSteps.Max(x => x.EndDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                        ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                    };
                    response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                    step8.Errors.Add(new Error(e));
                }
                finally
                {
                    step8.Done();
                    response.ProcessSteps.Add(step8);
                }

                response.Done();

                // Send Mail
                var stepMail = new ProcessStep("Send Mail");
                try
                {
                    var serializedXml = response.Serialize();
                    Logger.Info("Serialized Process: " + serializedXml);
                    var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.ScheduleBackoutReport.xslt", Encoding.UTF8);
                    Logger.Info("Report XML: " + xslOutput);

                    var recipients = new List<string>();
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CG_SCHEDULE_BACKOUT", "ADMIN", null, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CG_SCHEDULE_BACKOUT", "PUBLIC", evt.Plant.PlantCode, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CG_SCHEDULE_BACKOUT", "USER", evt.Plant.PlantCode, null));

                    Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                        System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                        recipients,
                        null,
                        $"{(response.HasErrors() ? "ERROR - " : String.Empty)}{evt.Plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule {evt.ScheduleNum} Backout",
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
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            response.Close();
            return response;
        }

        /*****************************************************************************
        *   VALIDATE
        ******************************************************************************/
        public static Messages Validate(BusinessSystemSession session, ScheduleEvent evt)
        {
            var messages = new Messages();

            if (String.IsNullOrEmpty(evt.BatchNum))
            {
                messages.Add("Validation",
                    $"{evt.Plant.PlantCode} Schedule Num {evt.ScheduleNum} does NOT exist in E1", Message.SeverityType.Error);
            }
            return messages;
        }

    }
}
