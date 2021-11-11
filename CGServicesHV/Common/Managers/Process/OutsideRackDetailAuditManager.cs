using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
using NLog.LayoutRenderers.Wrappers;
using RejectCode = CGServicesHV.Common.Types.Enterprise.RejectCode;

namespace CGServicesHV.Common.Managers.Process
{
    public class OutsideRackDetailAuditManager
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
            var response = new ProcessResponse("Rack Detail Audit Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

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
                //var bizOnlyCollection =
                //    from z in bizDict.Values.Where(k =>
                //        !plantDict.ContainsKey(k.Field<string>("RDLOTN").Trim() +
                //                               k.Field<string>("RDZ58SN").Trim()))
                //    select new
                //    {
                //        LotNum = z.Field<string>("RDLOTN"),
                //        SerialNum = z.Field<string>("RDZ58SN"),
                //        ItemNum = z.Field<decimal>("RDITM"),
                //        StatusCode = z.Field<string>("RDZ58DST"),
                //        Quantity = z.Field<decimal>("RDPQOH"),
                //        RackId = z.Field<decimal>("RDZ58RID"),
                //        TransactionDate = E1TimeManager.CastToDateTime(z.Field<decimal>("RDUPMJ"),
                //            z.Field<decimal>("RDTDAY"))
                //    };
                //stepBizOnlyRacks.RowsAffected = bizOnlyCollection.Count();
                //response.SerializedObjects.Add("BizOnlyCollection", bizOnlyCollection, "RackDetails", "RackDetail");

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
                            TransactionDate = E1TimeManager.CastToDateTime(t.Field<decimal>("RDUPMJ"),
                                t.Field<decimal>("RDTDAY"))
                        })
                    select z;
                var bizOnlyCollection = bizOnlyCollection1.Where(x => x.TransactionDate < DateTime.Now.AddMinutes(-1)).ToList();
                stepBizOnlyRacks.RowsAffected = bizOnlyCollection.Count();
                response.SerializedObjects.Add("BizOnlyCollection", bizOnlyCollection, "RackDetails", "RackDetail");
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
                            TransactionDate = t.Field<DateTime>("TransactionDate")
                        }).ToList()
                    select z;
                var plantOnlyCollection = plantOnlyCollection1.ToList();
                stepPlantOnlyRacks.RowsAffected = plantOnlyCollection.Count();
                response.SerializedObjects.Add("PlantOnlyCollection", plantOnlyCollection, "RackDetails", "RackDetail");

                // Delete statement Messages
                if (plantOnlyCollection.Any() && plantOnlyCollection.Count <= 1000)
                {
                    var statements = new List<string>();
                    foreach (var rec in plantOnlyCollection)
                    {
                        statements.Add(
                            $"DELETE FROM EBS.RackDetail WHERE LotNum = '{rec.LotNum}' and SerialNum = '{rec.SerialNum}';");
                    }
                    //var sqlResponses = Sql.SqlBatchExecution(statements, 1000, sqlCns["PLANT.SHOPFLOOR"]);
                    //stepPlantOnlyRacks.SqlDmlResponses.AddRange(sqlResponses);
                    //stepPlantOnlyRacks.Messages.Add(new Message("Deletes", sqlResponses.Sum(x => x.RowsAffected).ToString()));
                }
                if (plantOnlyCollection.Any() && plantOnlyCollection.Count > 1000)
                {
                    stepPlantOnlyRacks.Messages.Add(new Message("Deletes", $"Too many racks details to delete ... Governed at 1000 ... Total of {plantOnlyCollection.Count}"));
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
                        BizTransactionDate = E1TimeManager.CastToDateTime(bizData.Field<decimal>("RDUPMJ"),
                            bizData.Field<decimal>("RDTDAY"))
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
                    xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.RackDetailAuditReport.xslt", Encoding.UTF8);
                    Logger.Info("Report XML: " + xslOutput);

                    var recipients = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                    //var recipients = new List<string>();
                    //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "ADMIN", null, null));
                    //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "PUBLIC", plant.PlantCode, null));
                    //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_RACK_AUDIT", "USER", plant.PlantCode, null));

                    Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                        System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                        recipients,
                        null,
                        $"{(response.HasErrors() ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Rack Detail Audit",
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
                        RDLOTN,
                        RDZ58SN,
                        RDITM,
                        RDZ58RID,
                        RDZ58DST,
                        RDPQOH / 1000 AS RDPQOH,
                        RDUPMJ,
                        RDTDAY
                    FROM 
	                    F580056
	                WHERE 
                        RDZ58DST IN ('1','2','3','4','5','6','7') AND
                        RDZ58RID IN (select RHZ58RID from F580055 where RHLOCN = 'MZRAW')
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
                        LotNum,
                        SerialNum,
                        ItemNum,
                        RackId,
                        StatusCode,
                        QtyOnHand,
                        TransactionDate
                    FROM 
	                    EBS.RackDetail WITH (NOLOCK)
	                WHERE 
                        StatusCode IN ('1','2','3','4','5','6','7')"
            };
            return cmd;
        }


    }
}
