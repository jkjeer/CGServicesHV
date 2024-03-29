using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public class OutsideMTSScheduleDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //private const string shopFloorSystemCGHV = "3";
        private const string shopFloorSystemCGHVOutside = "8";

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum, bool force, ServiceContext serviceContext, bool parallel, ProcessResponse responseStub = null)
        {
            var response = new ProcessResponse($@"Download Outside MTS Incoming Orders ({plant.PlantCode}) Schedule {scheduleNum} {(parallel ? "Parallel" : "Synchronous")}", true);
            if (responseStub != null)
            {
                response.ProcessSteps.AddRange(responseStub.ProcessSteps);
            }
            response.ServiceContext = serviceContext ?? response.ServiceContext;
            response.Plant = plant;
            response.IsParallel = parallel;
            var mtsSchedule = new MTSSchedule(schedulePlantNum, scheduleNum);
            var cnSF = new SqlConnection();
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTS Schedule({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtsSchedule = MTSScheduleManager.BuildMTSSchedule(e1Session, plant, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum, out childResponse);
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
                    var step2SetDestinationPlant = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Set Destination Plant and Routing Code");
                    try
                    {
                        //mtsSchedule.SchedulePlantNum = mtsSchedule.PlantNum;
                        //mtsSchedule.MfgPlantNum = "32";
                        //mtsSchedule.PlantNum = mtsSchedule.MfgPlantNum;
                        //response.Plant = PlantManager.GetPlant(mtsSchedule.MfgPlantNum);
                        var mfgPlant = PlantManager.GetPlant(mtsSchedule.MfgPlantNum);
                        cnSFInt = GetSFIntSqlConnection(mfgPlant.PlantCode);
                        response.ConnectionStrings.Add(new ConnectionString("cnSFInt", cnSFInt.ConnectionString));
                        cnSF = GetSFSqlConnection(mfgPlant.PlantCode);
                        response.ConnectionStrings.Add(new ConnectionString("cnSF", cnSF.ConnectionString));
                        //foreach (var mtsScheduleLine in mtsSchedule.MTSScheduleLines)
                        //{
                        //    mtsScheduleLine.SchedulePlantNum = mtsSchedule.SchedulePlantNum;
                        //    mtsScheduleLine.MfgPlantNum = mtsSchedule.MfgPlantNum;
                        //}
                        foreach (var mtsIncomingOrderLine in mtsSchedule.MTSIncomingOrderLines)
                        {
                            mtsIncomingOrderLine.RoutingCode = "T";
                        }
                        step2SetDestinationPlant.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                    }
                    catch (Exception e)
                    {
                        step2SetDestinationPlant.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2SetDestinationPlant.Done();
                        response.ProcessSteps.Add(step2SetDestinationPlant);
                    }
                
                    if (!step2SetDestinationPlant.HasErrors())
                    {
                        var step3Validate = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Validate Schedule");
                        try
                        {
                            mtsSchedule.ValidationErrors = MTSScheduleManager.Validate(mtsSchedule, cnSF, shopFloorSystemCGHVOutside);
                            if (mtsSchedule.ValidationErrors.Any())
                            {
                                step3Validate.Errors.Add(new Error(mtsSchedule.ValidationErrors[0].Message));
                                mtsSchedule.TransferStatus = "E";
                                mtsSchedule.TransferMessage = mtsSchedule.ValidationStatus;
                                //MTSScheduleEventsManager.CancelMTSSchedule(e1Session, plant, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
                                F580020Manager.CancelF580020(e1Session, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
                            }
                            step3Validate.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                        }
                        catch (Exception e)
                        {
                            step3Validate.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3Validate.Done();
                            response.ProcessSteps.Add(step3Validate);
                        }

                        if (!step3Validate.HasErrors() && (!mtsSchedule.ValidationErrors.Any() || force && !mtsSchedule.IsFatal()))
                        {
                            var step4UpdateBeforeProcessingF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update Before Processing F580020");
                            try
                            {
                                F580020Manager.UpdateBeforeProcessingF580020(e1Session, schedulePlantNum, scheduleNum, mtsSchedule.ScheduleDateE1);
                                step4UpdateBeforeProcessingF580020.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                            }
                            catch (Exception e)
                            {
                                step4UpdateBeforeProcessingF580020.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step4UpdateBeforeProcessingF580020.Done();
                                response.ProcessSteps.Add(step4UpdateBeforeProcessingF580020);
                            }

                            if (!step4UpdateBeforeProcessingF580020.HasErrors() && !mtsSchedule.ValidationErrors.Any())
                            {
                                var step5DownloadHVQueTable = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Download HVQue Table");
                                try
                                {
                                    step5DownloadHVQueTable.RowsAffected = DownloadHVQueTable(cnSFInt, mtsSchedule.MTSIncomingOrderLines);
                                    response.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                                }
                                catch (Exception e)
                                {
                                    step5DownloadHVQueTable.Errors.Add(new Error(e));
                                }
                                finally
                                {
                                    step5DownloadHVQueTable.Done();
                                    response.ProcessSteps.Add(step5DownloadHVQueTable);
                                }

                                if (!step5DownloadHVQueTable.HasErrors())
                                {
                                    var step6UpdateAfterProcessingSuccessF580020 = new ProcessStep("Step 5. Update After Processing Success F580020");
                                    try
                                    {
                                        mtsSchedule.TransferStatus = "Y";
                                        mtsSchedule.TransferDate = DateTime.Now.ToLocalTime();
                                        F580020Manager.UpdateAfterProcessingSuccessF580020(e1Session, schedulePlantNum, scheduleNum, mtsSchedule.ScheduleDateE1);
                                        step6UpdateAfterProcessingSuccessF580020.RowsAffected = 1;
                                    }
                                    catch (Exception e)
                                    {
                                        step6UpdateAfterProcessingSuccessF580020.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step6UpdateAfterProcessingSuccessF580020.Done();
                                        response.ProcessSteps.Add(step6UpdateAfterProcessingSuccessF580020);
                                    }
                                } // if (!step5DownloadHVQueTable.HasErrors())
                            } // if (!step4UpdateBeforeProcessingF580020.HasErrors() && !mtsSchedule.ValidationErrors.Any())
                        } // if (!step3Validate.HasErrors() && ( !mtsSchedule.ValidationErrors.Any() || force && !mtsSchedule.IsFatal() ))
                    } // if (!step2SetDestinationPlant.HasErrors() && !mtsSchedule.ValidationErrors.Any())
                } // if (!step1GetmtsSchedule.HasErrors() && !mtsSchedule.ValidationErrors.Any())
                else
                {
                    mtsSchedule.TransferStatus = "E";
                    mtsSchedule.TransferMessage = mtsSchedule.ValidationStatus;
                }
                if (mtsSchedule.ValidationErrors != null)
                {
                    foreach (var validationError in mtsSchedule.ValidationErrors)
                    {
                        response.Errors.Add(new Error(validationError.Message, false));
                    }
                }
                response.Done();
                SummarizeOutsideMTSSchedule(response, mtsSchedule);
                SummarizeOutsideMTSScheduleDownload(response, mtsSchedule);
                NotifyOutsideMTSScheduleDownload(response, mtsSchedule);
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}"));
                throw new FaultException(e.Message);
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadOutsideMTSSchedule(BusinessSystemSession e1Session, Dictionary<string, SqlConnection> sqlCns, Plant plant, ServiceContext serviceContext, bool parallel = false)
        {
            var startTime = DateTime.Now.ToLocalTime();
            var response = new ProcessResponse($"Download Outside MTS Incoming Orders ({plant.PlantCode})", true);
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
                var mtsScheduleEvents = OutsideMTSScheduleEventsManager.ReserveUnprocessedOutsideMTSScheduleEvents(e1Session, plant, out childResponse);
                response.ProcessSteps.AddRange(childResponse.ProcessSteps);

                if (mtsScheduleEvents.Any())
                {
                    try
                    {
                        foreach (var mtsSchedule in mtsScheduleEvents)
                        {
                            try
                            {
                                Logger.Info($"Outside MTS SCHEDULE: {mtsSchedule.ScheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
                                response.ProcessResponses.Add(Process(e1Session, sqlCns["PLANT.INTEGRATION"], plant, mtsSchedule.SchedulePlantNum, mtsSchedule.ScheduleNum, 0, "", false, null, false));
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"ERROR: PLANT: {plant.PlantCode} Schedule: {mtsSchedule.ScheduleNum}  {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}");
                                response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} Schedule: {mtsSchedule.ScheduleNum}  {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}"));
                                OutsideMTSScheduleProcessor.NotifyOutsideMTSScheduleDownload(response, mtsSchedule.ScheduleNum);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: PLANT: {plant.PlantCode} {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}");
                        response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}"));
                        OutsideMTSScheduleProcessor.NotifyOutsideMTSScheduleDownload(response, 0);
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
                OutsideMTSScheduleProcessor.NotifyOutsideMTSScheduleDownload(response, 0);
            }
            finally
            {
                response.Close();
                Logger.Info($"PLANT: {plant.PlantCode} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }

        public static void SummarizeOutsideMTSSchedule(ProcessResponse response, MTSSchedule mtsSchedule)
        {
            var stepSummarize = new ProcessStep("Schedule Summary Collections");
            try
            {
                var scheduleCustomerLines =
                    mtsSchedule.MTSIncomingOrderLines.GroupBy(x => new
                        {
                            x.DisplayCustomer,
                            x.CustName,
                            x.WOrderNum,
                            x.ItemNum,
                            x.Construction,
                            x.Dimensions,
                            x.RackType
                        })
                        .Select(
                            ng =>
                                new
                                {
                                    ng.Key.DisplayCustomer,
                                    ng.Key.CustName,
                                    ng.Key.WOrderNum,
                                    ng.Key.ItemNum,
                                    ng.Key.Construction,
                                    ng.Key.Dimensions,
                                    ng.Key.RackType,
                                    ScheduleLines = (ng.Count()),
                                    WorkOrders = (ng.Select(x => x.WOrderNum).Distinct().Count()),
                                    LitesQty = (ng.Sum(x => x.OrderQty))
                                })
                        .ToList();
                response.SerializedObjects.Add("ScheduleCustomerLine", scheduleCustomerLines, "ScheduleCustomerLines");

                var schedPlant = PlantManager.GetPlant(mtsSchedule.SchedulePlantNum);

                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    ScheduleLocation = $@"{schedPlant.PlantCode} - {schedPlant.PlantNum}",
                    mtsSchedule.ScheduleNum,
                    mtsSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtsSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Lites = Convert.ToInt32(mtsSchedule.MTSIncomingOrderLines.Select(x => x.OrderQty).Sum()),
                    mtsSchedule.DueDate,
                    HasErrors = response.HasErrors() || mtsSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                stepSummarize.RowsAffected = mtsSchedule.MTSIncomingOrderLines.Count;
            }
            catch (Exception e)
            {
                stepSummarize.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarize.Done();
                response.ProcessSteps.Add(stepSummarize);
            }
        }

        public static void NotifyOutsideMTSScheduleDownload(ProcessResponse response, MTSSchedule mtsSchedule)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.OutsideMTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");

                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Outside HV Schedule {mtsSchedule.ScheduleNum} Download";
                if (response.HasErrors() || mtsSchedule.ValidationErrors.Any()) subject = $@"Error - {subject}";

                var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}";
                Directory.CreateDirectory(plantPathName);
                var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                File.Delete(fileName);
                File.WriteAllText(fileName, xslOutput);

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

        public static void SummarizeOutsideMTSScheduleDownload(ProcessResponse response, MTSSchedule mtsSchedule)
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
                    StartDate = response.ProcessSteps.Min(x => x.StartDate),
                    StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));
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

        private static SqlConnection GetSFIntSqlConnection(string plantCode)
        {
            var cnSFInt = new SqlConnection();
            try
            {
                cnSFInt = ConnectionManager.GetSqlConnectionFromKey(plantCode + ".INTEGRATION", false);
                cnSFInt.Open();
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            return cnSFInt;
        }

        private static SqlConnection GetSFSqlConnection(string plantCode)
        {
            var cnSF = new SqlConnection();
            try
            {
                cnSF = ConnectionManager.GetSqlConnectionFromKey(plantCode + ".SHOPFLOOR", false);
                cnSF.Open();
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            return cnSF;
        }
    }
}
