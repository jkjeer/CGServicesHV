using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;
using System.Globalization;

namespace CGServicesHV.Common.Managers.Process
{
    public class SpecUnitAuditManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public SpecUnitAuditManager()
        {
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(Plant plant, Dictionary<string, SqlConnection> sqlCns, int lookbackDays, bool sendReport)
        {
            var response = new ProcessResponse("Spec Unit Audit Manager");
            response.Plant = plant;
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            try
            {
                var stepPlantData = new ProcessStep("Get Plant Rack Data");
                var rackItems = GetPlantRackItems(sqlCns["PLANT.SHOPFLOOR"], lookbackDays);
                stepPlantData.RowsAffected = rackItems.Count();
                stepPlantData.Done();
                response.AddStep(stepPlantData);
                if (rackItems.Any() || sendReport) 
                {
                    response.Done();
                    SummarizeMissingSpecUnits(response, rackItems, plant, lookbackDays);
                    NotifyMissingSpecUnits(response, rackItems, plant, sqlCns["CORP.INTEGRATION"]);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            response.Close();
            return response;
        }

        public static void SummarizeMissingSpecUnits(ProcessResponse response, RackItems rackItems, Plant plant, int lookbackDays)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var rackItemLines = rackItems.ToList();
                response.SerializedObjects.Add("RackItemLine", rackItemLines, "RackItemLines");

                var headerSummary = new
                {
                    Location = $"{plant.PlantCode} - {plant.PlantNum}",
                    LookbackDays = lookbackDays,
                    MissingSpecUnits = rackItemLines.Count,
                    StartDate = response.ProcessSteps.Min(x => x.StartDate),
                    StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
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

        public static void NotifyMissingSpecUnits(ProcessResponse response, RackItems rackItems, Plant plant, SqlConnection cn)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            var hostName = $"SVC{ConfigurationManager.AppSettings["PrimaryEnvironment"]}";
            try
            {
                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("MTSItem", "Download MTS SpecUnits ItemNum", $"http://{hostName}/CGServicesHV/CGServicesHV.ItemRestService.svc/DownloadMtsSpecUnits?Plant=MZCG&ItemNum=566602"));
                actionLinks.Add(new ActionLink("MTS30", "Download MTS SpecUnits 30 days", $"http://{hostName}/CGServicesHV/CGServicesHV.ItemRestService.svc/DownloadMtsSpecUnits?Plant=MZCG&TimeSpan=30.00:00:00"));
                actionLinks.Add(new ActionLink("Rerun", "Re-run the SpecUnit Audit", $"http://{hostName}/CGServicesHV/CGServicesHV.ItemRestService.svc/RunSpecUnitAudit?Plant=MZCG&SendReport=TRUE"));
                response.SerializedObjects.Add("ActionLinks", actionLinks);

                string xslOutput = "";
                var serializedXml = response.Serialize();
                Logger.Info("Serialized Process: " + serializedXml);
                xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.SpecUnitAuditReport.xslt", Encoding.UTF8);
                Logger.Info("Report XML: " + xslOutput);

                var recipients = new List<string>();
                recipients.AddRange(Mail.GetRecipients(cn, "CGHV_SPECUNIT_AUDIT", "ADMIN", null, null));
                recipients.AddRange(Mail.GetRecipients(cn, "CGHV_SPECUNIT_AUDIT", "PUBLIC", plant.PlantCode, null));
                recipients.AddRange(Mail.GetRecipients(cn, "CGHV_SPECUNIT_AUDIT", "USER", plant.PlantCode, null));

                Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                    System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                    recipients,
                    null,
                    $"{(response.HasErrors() ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Spec Unit Audit",
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

        /********************************************************************************
        * Plant Racks
        ********************************************************************************/
        public static RackItems GetPlantRackItems(SqlConnection cn, int lookbackDays)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var rackItems = new RackItems();
            try
            {
                var cmd = GetPlantRackItems(lookbackDays);
                cmd.Connection = cn;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackItem = new RackItem
                        {
                            RackId = CastDbNull.To<decimal>(reader["RackId"], 0),
                            LotNum = CastDbNull.To(reader["LotNum"], string.Empty).Trim(),
                            SerialNum = CastDbNull.To(reader["SerialNum"], string.Empty).Trim(),
                            ItemNum = CastDbNull.To<decimal>(reader["ItemNum"], 0),
                            StatusCode = CastDbNull.To(reader["StatusCode"], string.Empty).Trim(),
                            Location = CastDbNull.To(reader["Location"], string.Empty).Trim(),
                            ScheduleNum = CastDbNull.To<decimal>(reader["ScheduleNum"], 0),
                            TransactionDate = CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.MinValue),
                            PlantNum = CastDbNull.To(reader["PlantID"], string.Empty).Trim(),
                            Quantity = CastDbNull.To<decimal>(reader["QtyOnHand"], 0),
                            WorkOrderNum = CastDbNull.To<decimal>(reader["WorkOrderNum"], 0),
                            ConfigId = CastDbNull.To<decimal>(reader["ConfigId"], 0),
                            StockCode = CastDbNull.To(reader["StockCode"], string.Empty).Trim()
                        };
                        rackItems.Add(rackItem);
                    }
                    reader.Close();
                }
                Logger.Info($"ROWS: {rackItems.Count}");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rackItems;
        }

        private static SqlCommand GetPlantRackItems(int lookbackDays)
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        r.RackId,
                        rd.LotNum,
                        rd.SerialNum,
                        rd.ItemNum,
                        r.StatusCode,
                        r.Location,
                        r.ScheduleNum,
                        r.TransactionDate,
                        r.PlantID,
                        rd.QtyOnHand,
                        rd.WorkOrderNum,
                        rd.ConfigId,
                        rd.StockCode
                    FROM EBS.Rack r WITH (NOLOCK)
                    INNER JOIN EBS.RackDetail rd WITH (NOLOCK)
	                    ON rd.RackId = r.RackId
                    LEFT OUTER JOIN EBS.SpecUnit su WITH (NOLOCK) 
	                    ON su.ItemNum = rd.ItemNum
	                    AND su.ConfigurationId = rd.ConfigId
                    WHERE su.SpecUnitId IS NULL
                        --AND rd.ConfigId > 0
                        AND r.StatusCode IN ('1','2','3','4','5','6','7','9','W','P')
                        AND r.TransactionDate > GETDATE()-{lookbackDays}   
                    ORDER BY r.TransactionDate DESC
                "
            };
            return cmd;
        }

    }
}
