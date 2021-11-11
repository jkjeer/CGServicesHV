using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, bool showPlantOnly, bool isDelete, short maxDeletes, short retentionMinutes)
        {
            var response = new ProcessResponse("RawTags Audit Manager");
            response.Plant = plant;
            var deletedRows = 0;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var isBizOnly = false;

            /********************************************************/
            var stepBizData = new ProcessStep("Get Business System Data");
            EnumerableRowCollection<DataRow> bizDataDt = null;
            Dictionary<decimal, DataRow> bizDict = new Dictionary<decimal, DataRow>();
            try
            {
                bizDataDt = GetBizRacks(session, plant).AsEnumerable();
                bizDict = bizDataDt.ToDictionary(p => p.Field<decimal>("REZ58RID"));
                //bizDict = bizDataDt.ToDictionary(p => p.Field<string>("REZ58SN"));
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

            /********************************************************/
            var stepBizOnlyRacks = new ProcessStep("Business Only Racks");
            try
            {
                var bizOnlyCollection1 =
                    from z in bizDict.Values.Where(k => !plantDict.ContainsKey(k.Field<decimal>("REZ58RID")))
                    select new
                    {
                        PlantId = z.Field<string>("REMCU"),
                        ItemNum = z.Field<string>("RELITM"),
                        SerialNum = z.Field<string>("REZ58SN"),
                        RackNum = z.Field<decimal>("REZ58RID"),
                        SeqNum = z.Field<decimal>("RESEQNO"),
                        StatusCode = z.Field<string>("REZ58DST"),
                        ProcessCode = z.Field<string>("REEDSP"),
                        Location = z.Field<string>("RELOCN"),
                        RackType = z.Field<string>("REZ58RT"),
                        Quantity = z.Field<decimal>("REPQOH"),
                        TransactionDate = E1TimeManager.CastToDateTime(z.Field<decimal>("REUPMJ"),
                                z.Field<decimal>("RETDAY"))
                    };
                var bizOnlyCollection = bizOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-3)).ToList();
                stepBizOnlyRacks.RowsAffected = bizOnlyCollection.Count;
                response.SerializedObjects.Add("BizOnlyCollection", bizOnlyCollection, "Racks", "Rack");
                isBizOnly = bizOnlyCollection.Any();
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
                        RackNum = z.Field<decimal>("RackId"),
                        ItemNum = z.Field<decimal>("ItemNum"),
                        SerialNum = z.Field<decimal>("SerialNum"),
                        SeqNum = z.Field<decimal>("SequenceNum"),
                        PlantId = z.Field<string>("PlantId"),
                        StatusCode = z.Field<string>("StatusCode"),
                        Location = z.Field<string>("Location"),
                        RackType = z.Field<string>("RackType"),
                        Quantity = z.Field<decimal>("QtyOnHand"),
                        TransactionDate = z.Field<DateTime>("Writtendate")
                    };
                var plantOnlyCollection2 = plantOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-10)); //.ToList();
                var plantOnlyCollection = plantOnlyCollection2.Where(x => x.StatusCode != "8").ToList();
                stepPlantOnlyRacks.RowsAffected = plantOnlyCollection.Count;
                if (showPlantOnly)
                {
                    response.SerializedObjects.Add("PlantOnlyCollection", plantOnlyCollection, "Racks", "Rack");
                }

                // Delete statement Messages
                var statements = new List<string>();
                var i = 0;
                foreach (var rec in plantOnlyCollection)
                {
                    statements.Add($"DELETE FROM dbo.RawTags WHERE RackId = {rec.RackNum} AND TransactionDate < DATEADD(MINUTE, -{retentionMinutes}, GETDATE()); ");
                    i++;
                    if (i >= maxDeletes) break;
                }

                if (isDelete)
                {
                    deletedRows = statements.Count();
                    stepPlantOnlyRacks.Messages.Add(new Message("Deletes", $"{statements.Count} Racks Deleted"));
                    foreach (var statement in statements)
                    {
                        stepPlantOnlyRacks.Messages.Add(new Message("Statement", statement));
                    }
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
                        RackNum = plantData.Field<decimal>("RackId"),
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
            string xslOutput = "";
            //if (bizDataDt.Any())
            //{
            var stepMail = new ProcessStep("Send Mail");
            try
            {
                var serializedXml = response.Serialize();
                //Logger.Info("Serialized Process: " + serializedXml);
                xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.RawTagsAuditReport.xslt", Encoding.UTF8);
                //Logger.Info("Report XML: " + xslOutput);

                //var recipients = new List<string>();
                //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "ADMIN", null, null));
                //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "PUBLIC", plant.PlantCode, null));
                //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "USER", plant.PlantCode, null));
                var recipients = ConfigurationManager.AppSettings["MailAuditConfirmToAddress"];

                Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                    System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                    recipients,
                    null,
                    $"{(isBizOnly ? "ERROR - " : string.Empty)}{plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Rack Audit",
                    xslOutput);
            }
            catch (Exception e)
            {
                stepMail.Errors.Add(new Error(e));
            }
            stepMail.Done();
            response.AddStep(stepMail);
            //}

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
            //var filterString = new StringBuilder();
            //foreach (var filter in filters)
            //{
            //    filterString.Append(filter);
            //}
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT  REMCU,
                            RELITM,
                            REZ58SN,
                            REZ58RID,
                            RESEQNO,
                            RELOCN,
                            REEDSP,
                            REZ58DST,
                            REPQOH,
                            REUPMJ,
                            RETDAY,
                            REZ58RT
                    FROM (
                        SELECT 
                            REMCU,
                            RELITM,
                            REZ58SN,
                            REZ58RID,
                            RESEQNO,
                            RELOCN,
                            REEDSP,
                            REZ58DST,
                            REPQOH / 1000 AS REPQOH,
                            REUPMJ,
                            RETDAY,
                            REZ58RT,
 						    ROW_NUMBER() OVER (PARTITION BY REMCU, RELITM, REZ58SN, REZ58RID ORDER BY RESEQNO DESC) AS RN
                        FROM 
	                        F580003A
	                    WHERE 
					        REZ58DST NOT IN ('0', '8') AND
                            REMCU = '          32' ) A
			            WHERE A.RN = 1
                    UNION
                    SELECT * FROM (
				        SELECT 
                            REMCU,
                            RELITM,
                            REZ58SN,
                            REZ58RID,
                            RESEQNO,
                            RELOCN,
                            REEDSP,
                            REZ58DST,
                            REPQOH / 1000 AS REPQOH,
                            REUPMJ,
                            RETDAY,
                            REZ58RT,
 						    ROW_NUMBER() OVER (PARTITION BY REMCU, RELITM, REZ58SN, REZ58RID ORDER BY RESEQNO DESC) AS RN
                        FROM 
	                        F580003A
	                    WHERE 
					        REZ58DST NOT IN ('0', '8') AND
                            REMCU IN ('          05', '          04', '          08') AND 
					        RELOCN LIKE 'MZ%' ) B 
				    WHERE B.RN = 1
                    FOR READ ONLY  "
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
                      ,[Writtendate]
                      ,[QtyOnHand]
                      ,[Location]
                      ,[Status]
                      ,[RackType]
                      ,[PlantID]
                FROM 
	                dbo.RawTags WITH (NOLOCK)"
                //WHERE 
                //       {filterString}"
            };
            return cmd;
        }


    }
}
