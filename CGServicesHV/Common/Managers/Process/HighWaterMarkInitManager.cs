using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CGServicesHV.Common.Managers.Process
{
    public class HighWaterMarkInitManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public HighWaterMarkInitManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, List<string> markNames)
        {
            var response = new ProcessResponse("High Water Mark Init Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));

            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            ProcessRunSpan processRunSpan = new ProcessRunSpan();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                if (useHighWaterMark || timeSpan == TimeSpan.Zero)
                {
                    timeSpan = new TimeSpan(0, 0, 0, 1);
                }
                processRunSpan = ProcessRunSpanManager.GetRunSpan(null, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            if (!response.HasErrors())
            {
                /********************************************************/
                var step2a = new ProcessStep("Get High Water Marks");
                var marks = new HighWaterMarks();
                try
                {
                    marks = HighWaterMarkManager.GetHighWaterMarks(sqlCns["PLANT.INTEGRATION"], plant, markNames);
                    step2a.RowsAffected = marks.Count;
                }
                catch (Exception e)
                {
                    step2a.Errors.Add(new Error(e));
                }
                finally
                {
                    step2a.Done();
                    response.AddStep(step2a);
                }

                if (!response.HasErrors())
                {
                    var step2b = new ProcessStep("Init Marks");
                    try
                    {
                        foreach (HighWaterMark mark in marks)
                        {
                            mark.EnterpriseDateTime = processRunSpan.StartDateTime;
                            mark.LocalDateTime = processRunSpan.StartDateTime.AddHours(-plant.TimeOffset);
                            if (!String.IsNullOrEmpty(mark.InitStatement))
                            {
                                mark.UniqueID = (long)GetUniqueId(session, plant, mark.InitStatement, processRunSpan);
                            }
                            else
                            {
                                mark.UniqueID = 9999999999;
                            }
                            var dmlResponse = HighWaterMarkManager.SaveHighWaterMarkId(sqlCns["PLANT.INTEGRATION"], plant, mark, mark.EnterpriseDateTime, mark.UniqueID);
                            step2b.RowsAffected += dmlResponse.RowsAffected;
                        }
                        response.SerializedObjects.Add(new SerializedObject("Marks",
                            XmlSerializationUtility.SerializeObject(Encoding.UTF8, marks)));
                    }
                    catch (Exception e)
                    {
                        step2b.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2b.Done();
                        response.AddStep(step2b);
                    }
                }

                /********************************************************/
                // Summary
                var stepSummary = new ProcessStep("Schedule Summary Collections");
                try
                {
                    var headerSummary = new
                    {
                        Plant = plant.PlantCode,
                        StartDate = response.ProcessSteps.Min(x => x.StartDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                        EndDate = response.ProcessSteps.Max(x => x.EndDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                        EffectiveDate = processRunSpan.StartDateTime.ToString("MM/dd/yyyy hh:mm:ss tt"),
                        ElapsedSeconds =
                            (decimal)
                                (response.ProcessSteps.Max(x => x.EndDate) -
                                    response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                    };
                    response.SerializedObjects.Add("Header", headerSummary, null);
                }
                catch (Exception e)
                {
                    stepSummary.Errors.Add(new Error(e));
                }
                stepSummary.Done();
                response.AddStep(stepSummary);

                /********************************************************/
                // Send Mail
                _logger.Info("Serialized Process: " + XmlSerializationUtility.SerializeObject(Encoding.UTF8, response));

                var stepMail = new ProcessStep("Send Mail");
                try
                {
                    var serializedXml = response.Serialize();
                    _logger.Info("Serialized Process: " + serializedXml);
                    var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.HighWaterMarkInitReport.xslt", Encoding.UTF8);
                    _logger.Info("Report XML: " + xslOutput);

                    var recipients = new List<string>();
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CG_HIGH_WATER_MARK", "ADMIN", null, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CG_HIGH_WATER_MARK", "PUBLIC", plant.PlantCode, null));
                    recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "CG_HIGH_WATER_MARK", "USER", plant.PlantCode, null));
                    var subject = $"{(response.HasErrors() ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] High Water Mark Init";

                    Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                    if (recipients.Count > 0)
                    {
                        stepMail.RowsAffected = recipients.Count;

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
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


            }
            response.Close();
            return response;
        }

        public decimal GetUniqueId(BusinessSystemSession session, Plant plant, string statement, ProcessRunSpan runSpan)
        {
            decimal uniqueId = 9999999999;
            try
            {
                var cmd = new iDB2Command();
                cmd.CommandText = statement;
                cmd.Connection = session.DbConnection;
                cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                cmd.Parameters.Add(new iDB2Parameter("MUPM1", iDB2DbType.iDB2Decimal));
                cmd.Parameters["MUPM1"].Precision = 6;
                cmd.Parameters["MUPM1"].Scale = 0;
                cmd.Parameters.Add(new iDB2Parameter("MUPM2", iDB2DbType.iDB2Decimal));
                cmd.Parameters["MUPM2"].Precision = 6;
                cmd.Parameters["MUPM2"].Scale = 0;
                cmd.Parameters.Add(new iDB2Parameter("UPMT", iDB2DbType.iDB2Decimal));
                cmd.Parameters["UPMT"].Precision = 6;
                cmd.Parameters["UPMT"].Scale = 0;
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = E1TimeManager.CastToE1DateId(runSpan.StartDateTime.AddDays(-1));
                cmd.Parameters[2].Value = E1TimeManager.CastToE1DateId(runSpan.StartDateTime);
                cmd.Parameters[3].Value = E1TimeManager.CastToE1TimeId(runSpan.StartDateTime);
                _logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    uniqueId = Data.CastDbNull.To<decimal>(reader["UniqueID"], 0);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return uniqueId;
        }

    }
}
