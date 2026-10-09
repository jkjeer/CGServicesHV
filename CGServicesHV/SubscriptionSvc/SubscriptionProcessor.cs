using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.SubscriptionSvc
{
    public class SubscriptionProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * METHODS
        ********************************************************************************/
        public static Plants GetPlants(string code)
        {
            try
            {
                return PlantManager.GetPlants(code);
            }
            catch (Exception e)
            {
                throw new  Exception(e.Message);
            }
        }

        public static PlantGroups GetPlantGroups()
        {
            try
            {
                return PlantManager.GetPlantGroupCache();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static SubscriptionTypes GetSubscriptionTypes(string code)
        {
            try
            {
                return SubscriptionManager.GetSubscriptionTypes(code);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static ProcessResponse SetSubscription(string plants, string subscriptionTypes, bool isActive, bool isEnabled)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plants} SUBSCRIPTION: {subscriptionTypes} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Set subscription", true);
            var cnInt = new SqlConnection();
            try
            {
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();
                var plantItems = PlantManager.GetPlants(plants);
                var plantString = new StringBuilder();
                foreach (var plant in plantItems)
                {
                    plantString.Append($"'{plant.PlantCode}',");
                }
                if (plantString.Length > 0) plantString.Length -= 1; //Strip off trailing comma.
                var subscriptions = SubscriptionManager.GetSubscriptionTypes(subscriptionTypes);
                var subscriptionTypeString = new StringBuilder();
                foreach (var subscription in subscriptions)
                {
                    subscriptionTypeString.Append($"'{subscription.SubscriptionTypeCode}',");
                }
                if (subscriptionTypeString.Length > 0) subscriptionTypeString.Length -= 1; //Strip off trailing comma.
                response.ProcessSteps.AddRange(SubscriptionManager.SetSubscription(cnInt, plantString.ToString(), subscriptionTypeString.ToString(), isActive, isEnabled));
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnInt.ConnectionString}  {e.Message}");
                response.Errors.Add(new Error($"ERROR: {cnInt.ConnectionString}  {e.Message}"));
            }
            finally
            {
                cnInt.Close();
                response.Done();
                //SummarizeSetSubscription(response);
                //NotifySetSubscription(response);
                Logger.Info($"PLANT: {plants} SUBSCRIPTION: {subscriptionTypes} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        
        public static string GetMTOScheduleEnvironment(string plantCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var environ = "";
            var cnInt = new SqlConnection();
            try
            {
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();
                var plant = PlantManager.GetPlant(plantCode);
                environ = SubscriptionManager.GetMTOScheduleEnvironment(cnInt, plant);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnInt.ConnectionString}  {e.Message}");
            }
            finally
            {
                cnInt.Close();
                Logger.Info($"PLANT: {plantCode} SUBSCRIPTION: {environ} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                switch (environ)
                {
                    case "MTO_SCHEDULE": environ = "Karl";
                        break;
                    case "FUSE_SCHEDULE": environ = "Fuse";
                        break;
                    case "DUAL_MTO_SCHEDULE": environ = "Both";
                        break;
                    default: environ = "None";
                        break;
                }
            }
            return environ;
        }

        public static void NotifySetSubscription(ProcessResponse response)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.ScheduleEnvironmentResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");
                var subject = response.Plant.PlantCode + " [" + ConfigurationManager.AppSettings["PrimaryEnvironment"] + "] " + "Schedule Downloads - MTO Schedule ";
                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"], ConfigurationManager.AppSettings["MailFromAddress"], toConfigAddresses, null, subject, xslOutput);
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

        public static void SummarizeSetSubscription(ProcessResponse response)
        {
            var stepSummarizeSetSubscription = new ProcessStep("Summary Collections");
            try
            {
                //var transferSummary = new
                //{
                //    mtoSchedule.TransferStatus,
                //    mtoSchedule.TransferDate,
                //    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any(),
                //    mtoSchedule.TransferMessage,
                //    Force = force,
                //    StartDate = response.ProcessSteps.Min(x => x.StartDate),
                //    StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                //    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                //    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                //    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                //};
                //response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));
                //if (mtoSchedule.TransferStatus != "Y")
                //{
                //    var transferError = new
                //    {
                //        ForceLink = mtoSchedule.TransferStatus == "Y" ? "" : $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/CGServices/CGServicesHV.ScheduleRestService.svc/DownloadMTOIncomingOrdersTable?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}&Force=true"
                //    };
                //    response.SerializedObjects.Add(new SerializedObject("TransferErrorNode", XmlSerializationUtility.SerializeAnonymousType(transferError, "TransferError")));
                //}
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSummarizeSetSubscription.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarizeSetSubscription.Done();
                response.ProcessSteps.Add(stepSummarizeSetSubscription);
            }
        }

        public static SubscriptionTypeItems RefreshSubscriptionTypeCache()
        {
            try
            {
                return SubscriptionManager.RefreshSubscriptionTypeCache();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static ProcessResponse InitHighWaterMarks(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Init High Water Marks");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var markNames = Data.UnboxParm.To(parms, "MarkNames", new List<string>());
                var mgr = new HighWaterMarkInitManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, markNames);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }
    }
}
