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
    public class RackDetailAuditManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackDetailAuditManager(BusinessSystemSession e1Session)
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
            var response = new ProcessResponse("Rack Detail Audit Manager");
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
            Dictionary<string, DataRow> bizDict = new Dictionary<string, DataRow>();
            try
            {
                bizDataDt = GetBizRacks(session, plant).AsEnumerable();
                bizDict = bizDataDt.ToDictionary(p => p.Field<string>("RDLOTN").Trim() + p.Field<string>("RDZ58SN").Trim());
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
                plantDict = plantDataDt.ToDictionary(p => p.Field<string>("LotNum").Trim() + p.Field<string>("SerialNum").Trim());
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
                    from z in bizDict.Values.Where(k =>
                            !plantDict.ContainsKey(k.Field<string>("RDLOTN").Trim() +
                                                   k.Field<string>("RDZ58SN").Trim()))
                        .Select(t => new
                        {
                            LotNum = t.Field<string>("RDLOTN"),
                            SerialNum = t.Field<string>("RDZ58SN"),
                            ItemNum = t.Field<decimal>("RDITM"),
                            StatusCode = t.Field<string>("RDZ58DST"),
                            Quantity = t.Field<decimal>("RDPQOH"),
                            RackId = t.Field<decimal>("RDZ58RID"),
                            TransactionDate = E1TimeManager.CastToDateTime(t.Field<decimal>("RDUPMJ"), t.Field<decimal>("RDTDAY")),
                            Location = t.Field<string>("RHLOCN")
                        })
                    select z;
                var bizOnlyCollection = bizOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-3)).ToList();
                stepBizOnlyRacks.RowsAffected = bizOnlyCollection.Count();
                response.SerializedObjects.Add("BizOnlyCollection", bizOnlyCollection, "RackDetails", "RackDetail");
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
                    from z in plantDict.Values.Where(k =>
                            !bizDict.ContainsKey(k.Field<string>("LotNum").Trim() +
                                                 k.Field<string>("SerialNum").Trim()))
                        .Select(t => new
                        {
                            LotNum = t.Field<string>("LotNum"),
                            SerialNum = t.Field<string>("SerialNum"),
                            ItemNum = t.Field<decimal>("ItemNum"),
                            StatusCode = t.Field<string>("StatusCode"),
                            Quantity = t.Field<decimal>("QtyOnHand"),
                            RackId = t.Field<decimal>("RackId"),
                            TransactionDate = t.Field<DateTime>("TransactionDate"),
                            Location = t.Field<string>("Location")
                        }).ToList()
                    select z;
                var plantOnlyCollection2 = plantOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-10)); //.ToList();
                var plantOnlyCollection = plantOnlyCollection2.Where(x => x.StatusCode != "8").ToList();
                stepPlantOnlyRacks.RowsAffected = plantOnlyCollection.Count();
                if (showPlantOnly)
                {
                    response.SerializedObjects.Add("PlantOnlyCollection", plantOnlyCollection, "RackDetails",
                        "RackDetail");
                }

                // Delete statement Messages
                var statements = new List<string>();
                var i = 0;
                foreach (var rec in plantOnlyCollection)
                {
                    statements.Add($"DELETE FROM EBS.RackDetail WHERE LotNum = '{rec.LotNum}' and SerialNum = '{rec.SerialNum}' and RackId = {rec.RackId} AND TransactionDate < DATEADD(MINUTE, -{retentionMinutes}, GETDATE());");
                    statements.Add($"DELETE FROM EBS.RackItem WHERE LotNum = '{rec.LotNum}' and SerialNum = '{rec.SerialNum}' and RackId = {rec.RackId} AND TransactionDate < DATEADD(MINUTE, -{retentionMinutes}, GETDATE());");
                    //statements.Add($"DELETE FROM EBS.RackDetail WHERE LotNum = '{rec.LotNum}' and SerialNum = '{rec.SerialNum}' and RackId = {rec.RackId} AND StatusCode IN ('0','8');");
                    //statements.Add($"DELETE FROM EBS.RackItem WHERE LotNum = '{rec.LotNum}' and SerialNum = '{rec.SerialNum}' and RackId = {rec.RackId} AND StatusCode IN ('0','8');");
                    i++;
                    if (i >= maxDeletes) break;
                }

                if (isDelete)
                {
                    var sqlResponses = Sql.SqlBatchExecution(statements, maxDeletes, sqlCns["PLANT.SHOPFLOOR"]);
                    //stepPlantOnlyRacks.SqlDmlResponses.AddRange(sqlResponses);
                    deletedRows = statements.Count();
                    stepPlantOnlyRacks.Messages.Add(new Message("Deletes", $"{statements.Count / 2} Racks Deleted"));
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
                        on new
                        {
                            LotNum = plantData.Field<string>("LotNum").Trim(),
                            SerialNum = plantData.Field<string>("SerialNum").Trim()
                        } equals new
                        {
                            LotNum = bizData.Field<string>("RDLOTN").Trim(),
                            SerialNum = bizData.Field<string>("RDZ58SN").Trim()
                        }
                    where plantData.Field<decimal>("RackId") != bizData.Field<decimal>("RDZ58RID") ||
                          plantData.Field<decimal>("ItemNum") != bizData.Field<decimal>("RDITM") ||
                          plantData.Field<decimal>("QtyOnHand") != bizData.Field<decimal>("RDPQOH") ||
                          plantData.Field<string>("StatusCode").Trim() != bizData.Field<string>("RDZ58DST").Trim()
                    select new
                    {
                        LotNum = plantData.Field<string>("LotNum"),
                        SerialNum = plantData.Field<string>("SerialNum"),
                        PlantStatusCode = plantData.Field<string>("StatusCode"),
                        PlantRackId = plantData.Field<decimal>("RackId"),
                        PlantItemNum = plantData.Field<decimal>("ItemNum"),
                        PlantQuantity = plantData.Field<decimal>("QtyOnHand"),
                        PlantTransactionDate = plantData.Field<DateTime>("TransactionDate"),
                        BizStatusCode = bizData.Field<string>("RDZ58DST"),
                        BizRackId = bizData.Field<decimal>("RDZ58RID"),
                        BizItemNum = bizData.Field<decimal>("RDITM"),
                        BizQuantity = bizData.Field<decimal>("RDPQOH"),
                        BizTransactionDate = E1TimeManager.CastToDateTime(bizData.Field<decimal>("RDUPMJ"), bizData.Field<decimal>("RDTDAY"))
                    };
                var bothDiffCollection = bothDiffCollection1.Where(x => x.BizTransactionDate < DateTime.Now.AddMinutes(-1));

                stepDiffRacks.RowsAffected = bothDiffCollection.Count();
                response.SerializedObjects.Add("BothDiffCollection", bothDiffCollection, "RackDetails", "RackDetail");

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
                    xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.RackDetailAuditReport.xslt", Encoding.UTF8);
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
                        $"{(isBizOnly ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Rack Detail Audit",
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
                    SELECT 
                        RDLOTN,
						RDZ58SN,
						RDITM,
						RDZ58RID,
						RDZ58DST,
						RDPQOH,
						RDUPMJ,
						RDTDAY,
                        RHLOCN
                    FROM (
                        SELECT * FROM (
					        SELECT 
						        RDLOTN,
						        RDZ58SN,
						        RDITM,
						        RDZ58RID,
						        RDZ58DST,
						        RDPQOH / 1000 AS RDPQOH,
						        RDUPMJ,
						        RDTDAY,
						        ROW_NUMBER() OVER (PARTITION BY RDLOTN, RDZ58SN ORDER BY RDUPMJ DESC, RDTDAY DESC) AS RN
					        FROM 
						        F580056
					        WHERE 
						        RDZ58DST NOT IN ('0') AND
                                RDPQOH > 0 AND
						        RDMCU = '          32'  ) A 
				        WHERE A.RN = 1
                    UNION
                        SELECT * FROM (
					        SELECT 
						        RDLOTN,
						        RDZ58SN,
						        RDITM,
						        RDZ58RID,
						        RDZ58DST,
						        RDPQOH / 1000 AS RDPQOH,
						        RDUPMJ,
						        RDTDAY,
						        ROW_NUMBER() OVER (PARTITION BY RDLOTN, RDZ58SN ORDER BY RDUPMJ DESC, RDTDAY DESC) AS RN
					        FROM 
						        F580056
                                INNER JOIN F580055 ON RHZ58RID = RDZ58RID
					        WHERE 
						        RDZ58DST NOT IN ('0') AND
                                RDPQOH > 0 AND
						        RDMCU IN ('          05', '          04', '          08') AND 
					            RHLOCN LIKE 'MZ%'
						        ) B 
				        WHERE B.RN = 1 ) C LEFT OUTER JOIN F580055 ON RHZ58RID = RDZ58RID
                        FOR READ ONLY"
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
                        rd.LotNum,
                        rd.SerialNum,
                        rd.ItemNum,
                        rd.RackId,
                        rd.StatusCode,
                        rd.QtyOnHand,
                        rd.TransactionDate,
					    r.Location
                    FROM 
	                    EBS.RackDetail rd WITH (NOLOCK)
					    LEFT OUTER JOIN EBS.Rack r WITH (NOLOCK) ON r.RackId = rd.RackId"
                    //WHERE
                    //{ filterString} "
            };
            return cmd;
        }


    }
}
