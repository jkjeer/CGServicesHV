using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using CGServicesHV.ScheduleSvc;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Process
{
    public class MTSScheduleDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string shopFloorSystemCGHV = "3";
        //private const string shopFloorSystemCGHVOutside = "8";

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, SqlConnection cnSF, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum, bool force, ServiceContext serviceContext, bool parallel, ProcessResponse responseStub = null)
        {
            var response = new ProcessResponse($@"Download MTS Incoming Orders ({plant.PlantCode}) Schedule {scheduleNum} {(parallel ? "Parallel" : "Synchronous")}", true);
            if (responseStub != null)
            {
                response.ProcessSteps.AddRange(responseStub.ProcessSteps);
            }
            response.ServiceContext = serviceContext ?? response.ServiceContext;
            response.Plant = plant;
            response.IsParallel = parallel;
            var mtsSchedule = new MTSSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnSFInt", cnSFInt.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnSF", cnSF.ConnectionString));
                var step1GetMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTS Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtsSchedule = MTSScheduleManager.BuildMTSSchedule(e1Session, plant, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTSSchedule.ProcessResponses.Add(childResponse);
                    step1GetMTSSchedule.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                    mtsSchedule.TransferStatus = "N";
                }
                catch (Exception e)
                {
                    step1GetMTSSchedule.Errors.Add(new Error(e));
                }
                step1GetMTSSchedule.Done();
                response.ProcessSteps.Add(step1GetMTSSchedule);

                if (!step1GetMTSSchedule.HasErrors())
                {
                    var step2Validate = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Validate Schedule");
                    try
                    {
                        mtsSchedule.ValidationErrors = MTSScheduleManager.Validate(mtsSchedule, cnSF, shopFloorSystemCGHV);
                        step2Validate.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                    }
                    catch (Exception e)
                    {
                        step2Validate.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2Validate.Done();
                        response.ProcessSteps.Add(step2Validate);
                    }
                }

                if (!step1GetMTSSchedule.HasErrors() && ( !mtsSchedule.ValidationErrors.Any() || force && !mtsSchedule.IsFatal() ))
                {
                    var step3UpdateBeforeProcessingF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update Before Processing F580020");
                    try
                    {
                        step3UpdateBeforeProcessingF580020.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                    }
                    catch (Exception e)
                    {
                        step3UpdateBeforeProcessingF580020.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3UpdateBeforeProcessingF580020.Done();
                        response.ProcessSteps.Add(step3UpdateBeforeProcessingF580020);
                    }

                    if ((!step3UpdateBeforeProcessingF580020.HasErrors() && mtsSchedule.IsValid()) || force && !mtsSchedule.IsFatal())
                    {
                        var step3DownloadHVQueTable = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Download HVQue Table");
                        try
                        {
                            step3DownloadHVQueTable.RowsAffected = DownloadHVQueTable(cnSFInt, mtsSchedule.MTSIncomingOrderLines);
                            response.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                        }
                        catch (Exception e)
                        {
                            step3DownloadHVQueTable.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3DownloadHVQueTable.Done();
                            response.ProcessSteps.Add(step3DownloadHVQueTable);
                        }

                        if (!step3DownloadHVQueTable.HasErrors())
                        {
                            var step4UpdateAfterProcessingSuccessF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update After Processing Success F580020");
                            try
                            {
                                mtsSchedule.TransferStatus = "Y";
                                mtsSchedule.TransferDate = DateTime.Now.ToLocalTime();
                                step4UpdateAfterProcessingSuccessF580020.RowsAffected = F580020Manager.UpdateAfterProcessingSuccessF580020(e1Session, plant.PlantNum, scheduleNum, mtsSchedule.ScheduleDateE1);
                            }
                            catch (Exception e)
                            {
                                step4UpdateAfterProcessingSuccessF580020.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step4UpdateAfterProcessingSuccessF580020.Done();
                                response.ProcessSteps.Add(step4UpdateAfterProcessingSuccessF580020);
                            }
                        } // if (!step4DownloadIncomingOrdersTable.HasErrors())
                    } // if (!step2UpdateBeforeProcessingF580020.HasErrors() && !mtsSchedule.ValidationErrors.Any())
                } // if (!step1GetmtsSchedule.HasErrors() && !mtsSchedule.ValidationErrors.Any())
                else
                {
                    mtsSchedule.TransferStatus = "E";
                    mtsSchedule.TransferMessage = mtsSchedule.ValidationStatus;
                    var step3CancelMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Cancel After Validation Failure F580020");
                    try
                    {
                        mtsSchedule.TransferDate = DateTime.Now.ToLocalTime();
                        step3CancelMTOSchedule.RowsAffected = F580020Manager.CancelF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                    }
                    catch (Exception e)
                    {
                        step3CancelMTOSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3CancelMTOSchedule.Done();
                        response.ProcessSteps.Add(step3CancelMTOSchedule);
                    }
                }
                if (mtsSchedule.ValidationErrors != null)
                {
                    foreach (var validationError in mtsSchedule.ValidationErrors)
                    {
                        response.Errors.Add(new Error(validationError.Message, false));
                    }
                }
                response.Close();
                MTSScheduleManager.SummarizeMTSSchedule(response, mtsSchedule);
                SummarizeMTSScheduleDownload(response, mtsSchedule, force);
                NotifyMTSScheduleDownload(response, mtsSchedule, force);
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}"));
                response.Close();
                throw new FaultException(e.Message);
            }
            return response;
        }

        public static ProcessResponse DownloadMTSIncomingOrders(BusinessSystemSession e1Session, Dictionary<string, SqlConnection> sqlCns, Plant plant, ServiceContext serviceContext, bool parallel = false)
        {
            var startTime = DateTime.Now.ToLocalTime();
            var response = new ProcessResponse($"Download MTS Incoming Orders ({plant.PlantCode})", true);
            response.ServiceContext = serviceContext;
            response.Plant = plant;
            response.IsParallel = parallel;
            response.ConnectionStrings.Add(new ConnectionString("E1", e1Session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            try
            {
                ProcessResponse childResponse;
                var mtsScheduleEvents = MTSScheduleEventsManager.ReserveUnprocessedMTSScheduleEvents(e1Session, plant, out childResponse);
                response.ProcessSteps.AddRange(childResponse.ProcessSteps);

                if (mtsScheduleEvents.Any())
                {
                    try
                    {
                        foreach (var mtsSchedule in mtsScheduleEvents)
                        {
                            try
                            {
                                Logger.Info($"MTS SCHEDULE: {mtsSchedule.ScheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
                                response.ProcessResponses.Add(Process(e1Session, sqlCns["PLANT.INTEGRATION"], sqlCns["PLANT.SHOPFLOOR"], plant, mtsSchedule.ScheduleNum, 0, "", false, null, false));
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"ERROR: PLANT: {plant.PlantCode} Schedule: {mtsSchedule.ScheduleNum}  {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}");
                                response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} Schedule: {mtsSchedule.ScheduleNum}  {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}"));
                                response.Close();
                                MTSScheduleProcessor.NotifyMTSScheduleDownload(response, mtsSchedule.ScheduleNum);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: PLANT: {plant.PlantCode} {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}");
                        response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}"));
                        response.Close();
                        MTSScheduleProcessor.NotifyMTSScheduleDownload(response, 0);
                    }
                    finally
                    {
                        response.Close();
                        Logger.Info($"PLANT: {plant.PlantCode} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: PLANT: {plant.PlantCode} {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}"));
                response.Close();
                MTSScheduleProcessor.NotifyMTSScheduleDownload(response, 0);
            }
            finally
            {
                Logger.Info($"PLANT: {plant.PlantCode} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static void NotifyMTSScheduleDownload(ProcessResponse response, MTSSchedule mtsSchedule, bool force)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.MTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");
                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Downloads - HV Schedule {mtsSchedule.ScheduleNum}";
                if (force) subject = $@"Force - {subject}";
                else if (response.HasErrors() || mtsSchedule.ValidationErrors.Any()) subject = $@"Error - {subject}";
                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void SummarizeMTSScheduleDownload(ProcessResponse response, MTSSchedule mtsSchedule, bool force)
        {
            var stepSummarizeDownload = new ProcessStep("Download Summary Collections");
            try
            {
                var transferSummary = new
                {
                    mtsSchedule.TransferStatus,
                    mtsSchedule.TransferDate,
                    HasErrors = response.HasErrors() || mtsSchedule.ValidationErrors.Any(),
                    mtsSchedule.TransferMessage,
                    Force = force,
                    StartDate = response.ProcessSteps.Min(x => x.StartDate),
                    StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Resend", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/CGServices/CGServicesHV.ScheduleRestService.svc/ResetMTSScheduleEvent?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/CGServices/CGServicesHV.ScheduleRestService.svc/CompleteMTSScheduleEvent?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/CGServices/CGServicesHV.ScheduleRestService.svc/CancelMTSScheduleEvent?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/CGServices/CGServicesHV.ScheduleRestService.svc/SuccessMTSScheduleEvent?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                response.SerializedObjects.Add("ActionLinks", actionLinks);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSummarizeDownload.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarizeDownload.Done();
                response.ProcessSteps.Add(stepSummarizeDownload);
            }
        }

        public static int DownloadHVQueTable(SqlConnection cnSfInt, MTSIncomingOrderLines incomingOrderLines)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount = 0;
            try
            {
                var hvQueTable = new HVQueTable();
                hvQueTable.Add(incomingOrderLines);
                if (hvQueTable.Rows.Count > 0)
                {
                    var bulkResponse = BulkCopyManager.BulkTable(hvQueTable.ExtendedProperties["Target"].ToString(), hvQueTable, cnSfInt);
                    rowCount = (int) bulkResponse.RowsCopied;
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSfInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnSfInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }
    }
}
