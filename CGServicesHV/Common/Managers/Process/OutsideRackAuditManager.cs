using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.Process
{
    public class OutsideRackAuditManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public RackAuditManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant)
        {
            var response = new ProcessResponse("Outside Rack Audit Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

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

            /********************************************************/
            var stepBizOnlyRacks = new ProcessStep("Business Only Racks");
            try
            {
                var bizOnlyCollection1 =
                    from z in bizDict.Values.Where(k => !plantDict.ContainsKey(k.Field<decimal>("RHZ58RID"))).ToList()
                    select new
                    {
                        RackNum = z.Field<decimal>("RHZ58RID"),
                        StatusCode = z.Field<string>("RHZ58HST"),
                        Location = z.Field<string>("RHLOCN"),
                        RackType = z.Field<string>("RHZ58RT"),
                        Quantity = (int)1,
                        TransactionDate = E1TimeManager.CastToDateTime(z.Field<decimal>("RHUPMJ"),
                                z.Field<decimal>("RHTDAY"))
                    };
                var bizOnlyCollection = bizOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-1)).ToList();
                stepBizOnlyRacks.RowsAffected = bizOnlyCollection.Count;
                response.SerializedObjects.Add("BizOnlyCollection", bizOnlyCollection, "Racks", "Rack");
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
                        StatusCode = z.Field<string>("StatusCode"),
                        Location = z.Field<string>("Location"),
                        RackType = z.Field<string>("RackType"),
                        Quantity = (int)1,
                        TransactionDate = z.Field<DateTime>("TransactionDate")
                    };
                var plantOnlyCollection = plantOnlyCollection1.ToList();
                stepPlantOnlyRacks.RowsAffected = plantOnlyCollection.Count;
                response.SerializedObjects.Add("PlantOnlyCollection", plantOnlyCollection, "Racks", "Rack");

                // Delete statement Messages
                if (plantOnlyCollection.Any() && plantOnlyCollection.Count <= 1000)
                {
                    var statements = new List<string>();
                    foreach (var rec in plantOnlyCollection)
                    {
                        statements.Add(
                            $"DELETE FROM EBS.Rack WHERE RackId = '{rec.RackNum}';");
                    }
                    //var sqlResponses = Sql.SqlBatchExecution(statements, 1000, sqlCns["PLANT.SHOPFLOOR"]);
                    //stepPlantOnlyRacks.SqlDmlResponses.AddRange(sqlResponses);
                    //stepPlantOnlyRacks.Messages.Add(new Message("Deletes", $"{sqlResponses.Sum(x => x.RowsAffected).ToString()} Racks Deleted"));
                }
                if (plantOnlyCollection.Any() && plantOnlyCollection.Count > 1000)
                {
                    stepPlantOnlyRacks.Messages.Add(new Message("Deletes", $"Too many racks to delete ... Governed at 1000 ... Total of {plantOnlyCollection.Count}"));
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
            if (bizDataDt.Any())
            {
                var stepMail = new ProcessStep("Send Mail");
                try
                {
                    var serializedXml = response.Serialize();
                    Logger.Info("Serialized Process: " + serializedXml);
                    xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.RackAuditReport.xslt", Encoding.UTF8);
                    Logger.Info("Report XML: " + xslOutput);

                    var subject = $"{(response.HasErrors() ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Rack Audit";
                    Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                    var recipients = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                    //var recipients = new List<string>();
                    //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "ADMIN", null, null));
                    //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "PUBLIC", plant.PlantCode, null));
                    //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "USER", plant.PlantCode, null));

                    Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                        System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                        recipients,
                        null,
                        subject,
                        xslOutput);
                }
                catch (Exception e)
                {
                    stepMail.Errors.Add(new Error(e));
                }
                stepMail.Done();
                response.AddStep(stepMail);
            }

            response.Close();
            return response;

        }

        /********************************************************************************
        * Business System Racks
        ********************************************************************************/
        public static DataTable GetBizRacks(BusinessSystemSession session, Plant plant)
        {
            var dt = new DataTable("BizRacks");

            var column = new DataColumn();
            column.DataType = Type.GetType("System.Decimal");
            column.ColumnName = "RHZ58RID";
            dt.Columns.Add(column);
            dt.PrimaryKey = new[] { column };

            try
            {
                // Command
                var cmd = GetBizRacks();
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                dt.Load(cmd.ExecuteReader(CommandBehavior.SingleResult));
                return dt;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetBizRacks()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        RHZ58RID,
                        RHZ58HST,
                        RHLOCN,
                        RHZ58RT,
                        RHZ58SCN,
                        RHUPMJ,
                        RHTDAY
                    FROM 
	                    F580055
	                WHERE 
                        RHZ58HST IN ('1','2','3','4','5','6','7') AND
                        RHLOCN = 'MZRAW'
                    FOR READ ONLY"
            };
            return cmd;
        }

        /********************************************************************************
        * Plant Racks
        ********************************************************************************/
        public static DataTable GetPlantRacks(SqlConnection cn)
        {
            var dt = new DataTable("PlantRacks");

            var column = new DataColumn();
            column.DataType = Type.GetType("System.Decimal");
            column.ColumnName = "RackId";
            dt.Columns.Add(column);
            dt.PrimaryKey = new[] { column };

            try
            {
                // Command
                var cmd = GetPlantRacks();
                cmd.Connection = cn;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                dt.Load(cmd.ExecuteReader(CommandBehavior.SingleResult));
                return dt;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static SqlCommand GetPlantRacks()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        RackId,
                        StatusCode,
                        Location,
                        RackType,
                        ScheduleNum,
                        TransactionDate
                    FROM 
	                    EBS.Rack WITH (NOLOCK)
	                WHERE 
                        StatusCode IN ('1','2','3','4','5','6','7')"
            };
            return cmd;
        }


    }
}
