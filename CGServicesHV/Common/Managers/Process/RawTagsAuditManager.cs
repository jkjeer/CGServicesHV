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
    public class RawTagsAuditManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RawTagsAuditManager(BusinessSystemSession e1Session)
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
            var response = new ProcessResponse("RawTags Audit Manager");
            response.Plant = plant;
            int bizOnlyBufferMinutes = Int32.Parse(ConfigurationManager.AppSettings["BizOnlyBufferMinutes"]);
            int rackDataRetentionDays = Int32.Parse(ConfigurationManager.AppSettings["RackDataRetentionDays"]);
            var deletedRows = 0;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var isBizOnly = false;
            decimal firstRackId = 0;
            /********************************************************/
            var stepBizData = new ProcessStep("Get Business System Data");
            EnumerableRowCollection<DataRow> bizDataDt = null;
            Dictionary<string, DataRow> bizDict = new Dictionary<string, DataRow>();
            try
            {
                bizDataDt = GetBizRacks(session, plant).AsEnumerable();
                //bizDict = bizDataDt.ToDictionary(p => p.Field<decimal>("REZ58RID"));
                //bizDict = bizDataDt.ToDictionary(p => p.Field<string>("RDZ58SN"));
                bizDict = bizDataDt.ToDictionary(p =>
                    p.Field<decimal>("RDZ58RID").ToString() + p.Field<string>("RDZ58SN").Trim());
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
            Dictionary<string, DataRow> plantDict = new Dictionary<string, DataRow>();
            try
            {
                plantDataDt = GetPlantRacks(sqlCns["PLANT.SHOPFLOOR"]).AsEnumerable();
                //plantDict = plantDataDt.ToDictionary(p => p.Field<decimal>("RackId"));
                //plantDict = plantDataDt.ToDictionary(p => p.Field<string>("SerialNum"));
                plantDict = plantDataDt.ToDictionary(p =>
                    p.Field<decimal>("RackId").ToString() + p.Field<string>("SerialNum").Trim());
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
            var stepBizOnlyRacks = new ProcessStep("Business Only RawTags");
            try
            {
                var bizOnlyCollection1 =
                        //from z in bizDict.Values.Where(k => !plantDict.ContainsKey(k.Field<string>("RDZ58SN")))
                        from z in bizDict.Values.Where(k =>
                                !plantDict.ContainsKey(k.Field<decimal>("RDZ58RID").ToString() +
                                                       k.Field<string>("RDZ58SN").Trim()))
                        select new
                        {
                            PlantId = z.Field<string>("RHMCU"),
                            SerialNum = z.Field<string>("RDZ58SN"),
                            RackId = z.Field<decimal>("RDZ58RID"),
                            LotNum = z.Field<string>("RDLOTN"),
                            ItemNum = z.Field<string>("RDLITM"),
                            Status = z.Field<string>("RDZ58DST"),
                            Location = z.Field<string>("RHLOCN"),
                            RackType = z.Field<string>("RHZ58RT"),
                            Quantity = z.Field<decimal>("RDPQOH"),
                            TransactionDate = E1TimeManager.CastToDateTime(z.Field<decimal>("RDUPMJ"), z.Field<decimal>("RDTDAY"))
                        };
                //var bizOnlyCollection = bizOnlyCollection1.ToList();
                var bizOnlyCollection = bizOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-bizOnlyBufferMinutes)).ToList();
                stepBizOnlyRacks.RowsAffected = bizOnlyCollection.Count;
                response.SerializedObjects.Add("BizOnlyCollection", bizOnlyCollection, "RawTags", "RawTag");
                isBizOnly = bizOnlyCollection.Any();
                firstRackId = bizOnlyCollection.Any() ? bizOnlyCollection.First().RackId : 0;
            }
            catch (Exception e)
            {
                stepBizOnlyRacks.Errors.Add(new Error(e));
            }
            stepBizOnlyRacks.Done();
            response.AddStep(stepBizOnlyRacks);

            var stepPlantOnlyRacks = new ProcessStep("Plant Only RawTags");
            try
            {
                var plantOnlyCollection1 =
                    //from z in plantDict.Values.Where(k => !bizDict.ContainsKey(k.Field<string>("SerialNum"))).ToList()
                    from z in plantDict.Values.Where(k =>
                        !bizDict.ContainsKey(k.Field<decimal>("RackId").ToString() +
                                                k.Field<string>("SerialNum").Trim()))
                    select new
                    {
                        RackId = z.Field<decimal>("RackId"),
                        ItemNum = z.Field<string>("ItemNum"),
                        SerialNum = z.Field<string>("SerialNum"),
                        SeqNum = z.Field<decimal>("SequenceNum"),
                        PlantId = z.Field<string>("PlantId"),
                        Status = z.Field<string>("Status"),
                        Location = z.Field<string>("Location"),
                        RackType = z.Field<string>("RackType"),
                        Quantity = z.Field<decimal>("QtyOnHand"),
                        TransactionDate = z.Field<DateTime>("TransactionDate")
                    };
                var plantOnlyCollection2 = plantOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-3)); //.ToList();
                var plantOnlyCollection = plantOnlyCollection2.Where(x => x.Status != "8").ToList();
                stepPlantOnlyRacks.RowsAffected = plantOnlyCollection.Count;
                if (showPlantOnly)
                {
                    response.SerializedObjects.Add("PlantOnlyCollection", plantOnlyCollection, "RawTags", "RawTag");
                }

                //// Delete statement Messages
                //var statements = new List<string>();
                //var i = 0;
                //foreach (var rec in plantOnlyCollection)
                //{
                //    statements.Add($"DELETE FROM dbo.RawTags WHERE SerialNum = '{rec.SerialNum}' and RackId = {rec.RackId};");
                //    //statements.Add($"DELETE FROM dbo.RawTags WHERE SerialNum = {rec.SerialNum} AND TransactionDate < DATEADD(MINUTE, -{retentionMinutes}, GETDATE()); ");
                //    i++;
                //    if (i >= maxDeletes) break;
                //}

                //if (isDelete)
                //{
                //    //var sqlResponses = Sql.SqlBatchExecution(statements, maxDeletes, sqlCns["PLANT.SHOPFLOOR"]);
                //    //deletedRows = statements.Count();
                //    //stepPlantOnlyRacks.Messages.Add(new Message("Deletes", $"{statements.Count} RawTags Deleted"));
                //    //foreach (var statement in statements)
                //    //{
                //    //    stepPlantOnlyRacks.Messages.Add(new Message("Statement", statement));
                //    //}
                //}
                //foreach (var statement in statements)
                //{
                //    stepPlantOnlyRacks.Messages.Add(new Message("", statement));
                //}

            }
            catch (Exception e)
            {
                stepPlantOnlyRacks.Errors.Add(new Error(e));
            }
            stepPlantOnlyRacks.Done();
            response.AddStep(stepPlantOnlyRacks);

            var stepDiffRacks = new ProcessStep("Difference RawTags");
            try
            {
                var bothDiffCollection1 =
                    from plantData in plantDataDt
                    join bizData in bizDataDt
                        on plantData.Field<string>("SerialNum") equals bizData.Field<string>("RDZ58SN")
                    where plantData.Field<string>("Location").Trim() != bizData.Field<string>("RHLOCN").Trim() ||
                          plantData.Field<string>("RackType").Trim() != bizData.Field<string>("RHZ58RT").Trim() ||
                          plantData.Field<string>("Status").Trim() != bizData.Field<string>("RDZ58DST").Trim()
                    select new
                    {
                        LotNum = bizData.Field<string>("RDLOTN"),
                        SerialNum = bizData.Field<string>("RDZ58SN"),
                        PlantItemNum = plantData.Field<string>("ItemNum"),
                        PlantRackId = plantData.Field<decimal>("RackId"),
                        PlantStatus = plantData.Field<string>("Status"),
                        PlantLocation = plantData.Field<string>("Location"),
                        PlantRackType = plantData.Field<string>("RackType"),
                        PlantQuantity = plantData.Field<decimal>("QtyOnHand"),
                        PlantTransactionDate = plantData.Field<DateTime>("TransactionDate"),
                        BizItemNum = bizData.Field<string>("RDLITM"),
                        BizRackId = bizData.Field<decimal>("RHZ58RID"),
                        BizStatus = bizData.Field<string>("RDZ58DST"),
                        BizLocation = bizData.Field<string>("RHLOCN"),
                        BizRackType = bizData.Field<string>("RHZ58RT"),
                        BizQuantity = bizData.Field<decimal>("RDPQOH"),
                        BizTransactionDate = E1TimeManager.CastToDateTime(bizData.Field<decimal>("RDUPMJ"), bizData.Field<decimal>("RDTDAY")),
                        Quantity = (int)1,
                    };
                var bothDiffCollection = bothDiffCollection1.Where(x => x.BizTransactionDate < DateTime.Now.AddMinutes(-1));
                stepDiffRacks.RowsAffected = bothDiffCollection.Count();
                response.SerializedObjects.Add("BothDiffCollection", bothDiffCollection, "RawTags", "RawTag");
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
                Plant = $"{plant.PlantCode} - {plant.PlantNum}",
                E1RawTags = stepBizData.RowsAffected,
                PlantRawTags = stepPlantData.RowsAffected,
                E1OnlyRawTags = stepBizOnlyRacks.RowsAffected,
                PlantOnlyRawTags = stepPlantOnlyRacks.RowsAffected,
                PlantOnlyRawTagsLabel = $@"{stepPlantOnlyRacks.RowsAffected} Records",
                IsDelete = isDelete.ToString().ToUpper(),
                DeletedRows = deletedRows,
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
                actionLinks.Add(new ActionLink("RackId", "Download Raw Tags by RackId", $"http://{hostName}/CGServicesHV/CGServicesHV.RackRestService.svc/DownloadRawTags?Plant={plant.PlantCode}&RackId={firstRackId}"));
                actionLinks.Add(new ActionLink("Rack30", "Download Raw Tags 30 days", $"http://{hostName}/CGServicesHV/CGServicesHV.RackRestService.svc/DownloadRawTags?Plant={plant.PlantCode}&TimeSpan=30.00:00:00"));
                actionLinks.Add(new ActionLink("Rerun", "Re-run the Raw Tags Audit", $"http://{hostName}/CGServicesHV/CGServicesHV.RackRestService.svc/RunRawTagsAudit?Plant={plant.PlantCode}&SendReport=TRUE"));
                response.SerializedObjects.Add("ActionLinks", actionLinks);

                var serializedXml = response.Serialize();
                //Logger.Info("Serialized Process: " + serializedXml);
                var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.RawTagsAuditReport.xslt", Encoding.UTF8);
                //Logger.Info("Report XML: " + xslOutput);

                var subject = $"{plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Raw Tags Audit";
                if (isBizOnly || response.HasErrors()) subject = $"ERROR - {subject}";
                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                if (isBizOnly || response.HasErrors() || sendReport)
                {
                    var recipients = new List<string>();
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CGHV_RAWTAGS_AUDIT", "ADMIN", null, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CGHV_RAWTAGS_AUDIT", "PUBLIC", plant.PlantCode, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CGHV_RAWTAGS_AUDIT", "USER", plant.PlantCode, null));
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

            try
            {
                filters.Add("RDPQOH > 0 AND ");
                filters.Add("RDZ58DST NOT IN ('0', '8') AND ");
                filters.Add("LENGTH(TRIM(RDZ58SN)) > 6 AND ");

                var timeStr = E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", DateTime.Parse("1900-01-01"), DateTime.Now.AddMinutes(-3));
                filters.Add(timeStr);
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
                        RHMCU,
                        TRIM(RDZ58SN) AS RDZ58SN,
                        RHZ58RID,
                        RDLOTN,
                        RDLITM,
                        RHZ58HST,
                        RHLOCN,
                        RHZ58RT,
                        RHZ58SCN,
                        RHUPMJ,
                        RHTDAY
                        RDMCU,
                        RDITM,
                        RDZ58RID,
                        RDZ58DST,
                        RDPQOH / 1000 AS RDPQOH,
                        RDUPMJ,
                        RDTDAY
	                FROM  F580055
	                INNER JOIN F580056
                        ON RDZ58RID = RHZ58RID
	                WHERE 
                        (RHMCU = '          32' 
                            OR (RHMCU IN ('          05', '          04', '          08') AND 
					            RHLOCN LIKE 'MZ%' )
                            ) 
                        AND {filterString}
                    ORDER BY RDZ58SN
                    FOR READ ONLY                
                "
            };
            return cmd;
        }

        private static iDB2Command GetBizRacksOld(IReadOnlyCollection<string> filters)
        {
            //var filterString = new StringBuilder();
            //foreach (var filter in filters)
            //{
            //    filterString.Append(filter);
            //}
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT  *
                    FROM (
                        SELECT 
                            REMCU,
                            RELITM,
                            TRIM(REZ58SN) AS REZ58SN,
                            REZ58RID,
                            RESEQNO,
                            RELOCN,
                            REEDSP,
                            REZ58DST,
                            REPQOH / 1000 AS REPQOH,
                            REUPMJ,
                            RETDAY,
                            REZ58RT,
 						    ROW_NUMBER() OVER (PARTITION BY REMCU, REZ58SN, REZ58RID ORDER BY RESEQNO DESC) AS RN                            
                        FROM 
	                        PRODDTA.F580003A
	                    WHERE 
                            REZ58SN IN (
                                        SELECT DISTINCT REZ58SN
                                        FROM PRODDTA.F580003A
                                        WHERE REZ58DST NOT IN ('0', '8') 
                                            AND REPQOH > 0
                                            AND (REMCU = '          32' 
                                               OR (REMCU IN ('          05', '          04', '          08') AND 
					                               RELOCN LIKE 'MZ%' )
                                                )
                            ) 
                    ) A
                    WHERE RN = 1
                    AND REZ58DST NOT IN ('0', '8') 
                    AND REPQOH > 0
                    AND (REMCU = '          32' 
                                                                   OR (REMCU IN ('          05', '          04', '          08') AND 
					                                                   RELOCN LIKE 'MZ%' )
                                                                    )
                    ORDER BY REZ58SN
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
            //var filterString = new StringBuilder();
            //foreach (var filter in filters)
            //{
            //    filterString.Append(filter);
            //}
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        [ItemNum]
                          ,[SerialNum]
                          ,[RackId]
                          ,[SequenceNum]
                          ,[Writtendate] as TransactionDate
                          ,[QtyOnHand]
                          ,[Location]
                          ,[Status]
                          ,[RackType]
                          ,[PlantID]
                    FROM 
	                    dbo.RawTags WITH (NOLOCK)
                    ORDER BY [SerialNum]
                "
                    //WHERE[Status] NOT IN('0', '8')
                //WHERE 
                //       {filterString}"
            };
            return cmd;
        }

    }
}
