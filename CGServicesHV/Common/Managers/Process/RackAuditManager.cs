using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class RackAuditManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackAuditManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, bool showPlantOnly, bool isDelete, short maxDeletes, short retentionMinutes, bool sendReport)
        {
            var response = new ProcessResponse("Spec Unit Audit Manager") { Plant = plant };
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            int bizOnlyBufferMinutes = Int32.Parse(ConfigurationManager.AppSettings["BizOnlyBufferMinutes"]);
            int rackDataRetentionDays = Int32.Parse(ConfigurationManager.AppSettings["RackDataRetentionDays"]);
            var archivedRows = 0;
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var isBizOnly = false;
            decimal firstRackId = 0;
            /********************************************************/
            var stepBizData = new ProcessStep("Get Business System Data");
            EnumerableRowCollection<DataRow> bizDataDt = null;
            Dictionary<decimal, DataRow> bizDict = new Dictionary<decimal, DataRow>();
            try
            {
                bizDataDt = GetBizRacks(session, plant).AsEnumerable();
                bizDict = bizDataDt.ToDictionary(p => p.Field<decimal>("RHZ58RID"));
                stepBizData.RowsAffected = bizDataDt.Count();
            }
            catch (Exception e)
            {
                stepBizData.Errors.Add(new Error(e));
            }
            stepBizData.Done();
            response.AddStep(stepBizData);

            /********************************************************/
            var stepPlantData = new ProcessStep("Get Plant Data");
            EnumerableRowCollection<DataRow> plantDataDt = null;
            Dictionary<decimal, DataRow> plantDict = new Dictionary<decimal, DataRow>();
            try
            {
                plantDataDt = GetPlantRacks(sqlCns["PLANT.SHOPFLOOR"]).AsEnumerable();
                plantDict = plantDataDt.ToDictionary(p => p.Field<decimal>("RackId"));
                stepPlantData.RowsAffected = plantDataDt.Count();
            }
            catch (Exception e)
            {
                stepPlantData.Errors.Add(new Error(e));
            }
            stepPlantData.Done();
            response.AddStep(stepPlantData);

            if (stepPlantData.HasErrors() || stepBizData.HasErrors())
            {
                response.Close();
                return response;
            }
            /********************************************************/
            var stepBizOnlyRacks = new ProcessStep("Business Only Racks");
            try
            {
                var bizOnlyCollection1 =
                    from z in bizDict.Values.Where(k => !plantDict.ContainsKey(k.Field<decimal>("RHZ58RID"))).ToList()
                    select new
                    {
                        RackId = z.Field<decimal>("RHZ58RID"),
                        PlantId = z.Field<string>("RHMCU"),
                        StatusCode = z.Field<string>("RHZ58HST"),
                        Location = z.Field<string>("RHLOCN"),
                        RackType = z.Field<string>("RHZ58RT"),
                        Quantity = (int)1,
                        TransactionDate = E1TimeManager.CastToDateTime(z.Field<decimal>("RHUPMJ"), z.Field<decimal>("RHTDAY"))
                    };
                var bizOnlyCollection = bizOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-bizOnlyBufferMinutes)).ToList();
                stepBizOnlyRacks.RowsAffected = bizOnlyCollection.Count;
                response.SerializedObjects.Add("BizOnlyCollection", bizOnlyCollection, "Racks", "Rack");
                isBizOnly = bizOnlyCollection.Any();
                firstRackId = bizOnlyCollection.Any() ? bizOnlyCollection.First().RackId : 0;
            }
            catch (Exception e)
            {
                stepBizOnlyRacks.Errors.Add(new Error(e));
            }
            stepBizOnlyRacks.Done();
            response.AddStep(stepBizOnlyRacks);

            var stepPlantOnlyRacks = new ProcessStep("Plant Only Racks");
            try
            {
                var plantOnlyCollection1 =
                    from z in plantDict.Values.Where(k => !bizDict.ContainsKey(k.Field<decimal>("RackId"))).ToList()
                    select new
                    {
                        RackId = z.Field<decimal>("RackId"),
                        PlantId = z.Field<string>("PlantId"),
                        StatusCode = z.Field<string>("StatusCode"),
                        Location = z.Field<string>("Location"),
                        RackType = z.Field<string>("RackType"),
                        Quantity = (int)1,
                        TransactionDate = z.Field<DateTime>("TransactionDate")
                    };
                var plantOnlyCollection2 = plantOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-10)); //.ToList();
                var plantOnlyCollection = plantOnlyCollection2.Where(x => x.StatusCode != "8").ToList();
                stepPlantOnlyRacks.RowsAffected = plantOnlyCollection.Count;
                if (showPlantOnly)
                {
                    response.SerializedObjects.Add("PlantOnlyCollection", plantOnlyCollection, "Racks", "Rack");
                }

                // Archive statement Messages
                var statements = new List<string>();
                var i = 0;
                foreach (var rec in plantOnlyCollection)
                {
                    statements.Add($"UPDATE r SET r.[Archive] = 1 FROM EBS.Rack r WHERE r.RackId = {rec.RackId} AND TransactionDate < GETDATE()-{rackDataRetentionDays}; ");
                    //statements.Add($"DELETE FROM EBS.Rack WHERE RackId = {rec.RackId} AND TransactionDate < DATEADD(MINUTE, -{retentionMinutes}, GETDATE()); ");
                    //statements.Add($"DELETE FROM EBS.Rack WHERE RackId = {rec.RackId} AND StatusCode IN ('0','8'); ");
                    i++;
                    if (i >= maxDeletes) break;
                }

                //if (isDelete)
                //{
                    var sqlResponses = Sql.SqlBatchExecution(statements, maxDeletes, sqlCns["PLANT.SHOPFLOOR"]);
                    archivedRows = statements.Count();
                    stepPlantOnlyRacks.SqlDmlResponses.AddRange(sqlResponses);
                    stepPlantOnlyRacks.Messages.Add(new Message("Archives", $"{statements.Count} Racks Archived"));
                    //foreach (var statement in statements)
                    //{
                    //    stepPlantOnlyRacks.Messages.Add(new Message("Statement", statement));
                    //}
                //}
                foreach (var statement in statements)
                {
                    stepPlantOnlyRacks.Messages.Add(new Message("", statement));
                }

            }
            catch (Exception e)
            {
                stepPlantOnlyRacks.Errors.Add(new Error(e));
            }
            stepPlantOnlyRacks.Done();
            response.AddStep(stepPlantOnlyRacks);

            var stepDiffRacks = new ProcessStep("Difference Racks");
            try
            {
                var bothDiffCollection1 =
                    from plantData in plantDataDt
                    join bizData in bizDataDt
                        on plantData.Field<decimal>("RackId") equals bizData.Field<decimal>("RHZ58RID")
                    where plantData.Field<string>("Location").Trim() != bizData.Field<string>("RHLOCN").Trim() ||
                          plantData.Field<string>("RackType").Trim() != bizData.Field<string>("RHZ58RT").Trim() ||
                          plantData.Field<string>("StatusCode").Trim() != bizData.Field<string>("RHZ58HST").Trim()
                    select new
                    {
                        RackId = plantData.Field<decimal>("RackId"),
                        PlantId = plantData.Field<string>("PlantId"),
                        PlantStatusCode = plantData.Field<string>("StatusCode"),
                        PlantLocation = plantData.Field<string>("Location"),
                        PlantRackType = plantData.Field<string>("RackType"),
                        PlantTransactionDate = plantData.Field<DateTime>("TransactionDate"),
                        BizStatusCode = bizData.Field<string>("RHZ58HST"),
                        BizLocation = bizData.Field<string>("RHLOCN"),
                        BizRackType = bizData.Field<string>("RHZ58RT"),
                        BizTransactionDate = E1TimeManager.CastToDateTime(bizData.Field<decimal>("RHUPMJ"), bizData.Field<decimal>("RHTDAY")),
                        Quantity = (int)1,
                    };
                var bothDiffCollection = bothDiffCollection1.Where(x => x.BizTransactionDate < DateTime.Now.AddMinutes(-1));
                stepDiffRacks.RowsAffected = bothDiffCollection.Count();
                response.SerializedObjects.Add("BothDiffCollection", bothDiffCollection, "Racks", "Rack");
            }
            catch (Exception e)
            {
                stepDiffRacks.Errors.Add(new Error(e));
            }
            stepDiffRacks.Done();
            response.AddStep(stepDiffRacks);

            /********************************************************/
            var reportSummary = new
            {
                Plant = plant.PlantCode,
                E1Racks = stepBizData.RowsAffected,
                PlantRacks = stepPlantData.RowsAffected,
                E1OnlyRacks = stepBizOnlyRacks.RowsAffected,
                PlantOnlyRacks = stepPlantOnlyRacks.RowsAffected,
                PlantOnlyRacksLabel = $@"{stepPlantOnlyRacks.RowsAffected} Records",
                IsDelete = isDelete.ToString().ToUpper(),
                ArchivedRows = archivedRows,
                StartDate = response.ProcessSteps.Min(x => x.StartDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                EndDate = response.ProcessSteps.Max(x => x.EndDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                ElapsedSeconds =
                    (decimal)
                    (response.ProcessSteps.Max(x => x.EndDate) -
                     response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
            };
            response.SerializedObjects.Add("ReportHeader", reportSummary, null);

            /********************************************************/
            // Send Mail
            var stepMail = new ProcessStep("Send Mail");
            var hostName = $"SVC{ConfigurationManager.AppSettings["PrimaryEnvironment"]}";
            try
            {
                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("RackId", "Download Racks by RackId", $"http://{hostName}/CGServicesHV/CGServicesHV.RackRestService.svc/DownloadRacks?Plant={plant.PlantCode}&RackId={firstRackId}"));
                actionLinks.Add(new ActionLink("Rack30", "Download Racks 30 days", $"http://{hostName}/CGServicesHV/CGServicesHV.RackRestService.svc/DownloadRacks?Plant={plant.PlantCode}&TimeSpan=30.00:00:00"));
                actionLinks.Add(new ActionLink("Rerun", "Re-run the Rack Audit", $"http://{hostName}/CGServicesHV/CGServicesHV.RackRestService.svc/RunRackAudit?Plant={plant.PlantCode}&SendReport=TRUE"));
                response.SerializedObjects.Add("ActionLinks", actionLinks);

                var serializedXml = response.Serialize();
                Logger.Info("Serialized Process: " + serializedXml);
                var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.RackAuditReport.xslt", Encoding.UTF8);
                Logger.Info("Report XML: " + xslOutput);

                var subject = $"{plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Rack Audit";
                if (isBizOnly || response.HasErrors()) subject = $"ERROR - {subject}";
                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                if (isBizOnly || response.HasErrors() || sendReport)
                {
                    var recipients = new List<string>();
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CGHV_RACK_AUDIT", "ADMIN", null, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CGHV_RACK_AUDIT", "PUBLIC", plant.PlantCode, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CGHV_RACK_AUDIT", "USER", plant.PlantCode, null));
                    Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                        ConfigurationManager.AppSettings["MailFromAddress"],
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
            stepMail.Done();
            response.AddStep(stepMail);
            response.Close();
            return response;
        }

        /********************************************************************************
        * Business System Racks
        ********************************************************************************/
        public static DataTable GetBizRacks(BusinessSystemSession session, Plant plant)
        {
            var filters = new List<string>();
            var dt = new DataTable("BizRacks");

            var column = new DataColumn();
            column.DataType = Type.GetType("System.Decimal");
            column.ColumnName = "RHZ58RID";
            dt.Columns.Add(column);
            dt.PrimaryKey = new[] { column };

            try
            {
                // Command
                var cmd = GetBizRacks(filters);
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                dt.Load(cmd.ExecuteReader(CommandBehavior.SingleResult));
                return dt;

            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetBizRacks(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        RHZ58RID,
                        RHMCU,
                        RHZ58HST,
                        RHLOCN,
                        RHZ58RT,
                        RHZ58SCN,
                        RHUPMJ,
                        RHTDAY
                    FROM 
	                    F580055
	                WHERE 
                        RHMCU = '          32'  
                        AND RHZ58HST NOT IN ('0','8')
                UNION
				    SELECT 
                        RHZ58RID,
                        RHMCU,
                        RHZ58HST,
                        RHLOCN,
                        RHZ58RT,
                        RHZ58SCN,
                        RHUPMJ,
                        RHTDAY
                    FROM 
	                    F580055
	                WHERE 
                        RHMCU IN ('          05', '          04', '          08') AND 
					    RHLOCN LIKE 'MZ%'
                        AND RHZ58HST NOT IN ('0','8')
                    FOR READ ONLY  
            "
            };
            return cmd;
        }

        /********************************************************************************
        * Plant Racks
        ********************************************************************************/
        public static DataTable GetPlantRacks(SqlConnection cn)
        {
            var filters = new List<string>();
            var dt = new DataTable("PlantRacks");

            var column = new DataColumn();
            column.DataType = Type.GetType("System.Decimal");
            column.ColumnName = "RackId";
            dt.Columns.Add(column);
            dt.PrimaryKey = new[] { column };

            try
            {
                // Command
                var cmd = GetPlantRacks(filters);
                cmd.Connection = cn;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                dt.Load(cmd.ExecuteReader(CommandBehavior.SingleResult));
                return dt;

            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e.Message);
            }
        }

        private static SqlCommand GetPlantRacks(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        RackId,
                        PlantId,
                        StatusCode,
                        Location,
                        RackType,
                        ScheduleNum,
                        TransactionDate
                    FROM 
	                    EBS.Rack WITH (NOLOCK)
                "
	            //WHERE 
             //       {filterString}"
            };
            return cmd;
        }


    }
}
