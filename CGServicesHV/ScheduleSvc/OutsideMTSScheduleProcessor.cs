using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.ScheduleSvc
{
    public class OutsideMTSScheduleProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static MTSScheduleEvents GetUnprocessedOutsideMTSScheduleEvents(string plantCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var e1Session = new BusinessSystemSession();
            MTSScheduleEvents mtsScheduleEvents;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                ProcessResponse childResponse;
                mtsScheduleEvents = MTSScheduleEventsManager.GetUnprocessedOutsideMTSScheduleEvents(e1Session, plant, out childResponse);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: Plant: {plantCode} {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"Plant: {plantCode} {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                e1Session.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return mtsScheduleEvents;
        }

        public static ProcessResponse DownloadOutsideMTSSchedule(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            // Called by OUTSIDE_MTS_SCHEDULE subscription
            var parallel = Data.UnboxParm.To(parms, "Parallel", false);
            var serviceContext = Data.UnboxParm.To<ServiceContext>(parms, "ServiceContext", null);
            var response = OutsideMTSScheduleDownloadManager.DownloadOutsideMTSSchedule(session, sqlCns, plant, serviceContext, parallel);
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadOutsideHVQueTable(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Download Outside MTS Schedule", true);
            var e1Session = new BusinessSystemSession();
            var cnSFInt = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSFInt = GetSFIntSqlConnection(plant.PlantNum);
                response = OutsideMTSScheduleDownloadManager.Process(e1Session, cnSFInt, plant, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum, false, null, false );
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}"));
                NotifyOutsideMTSScheduleDownload(response, scheduleNum);
            }
            finally
            {
                e1Session.Close();
                cnSFInt.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static void NotifyOutsideMTSScheduleDownload(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.OutsideMTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");
                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Downloads - Outside MTS Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $@"Error - {subject}";

                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

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

        private static SqlConnection GetSFIntSqlConnection(string plantNum)
        {
            var cnSFInt = new SqlConnection();
            try
            {
                cnSFInt = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".INTEGRATION", false);
                cnSFInt.Open();
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            return cnSFInt;
        }
    }
}
