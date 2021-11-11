using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.ShopFloor;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class MTSScheduleManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static MTSSchedule BuildMTSSchedule(BusinessSystemSession e1Session, Plant plant, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum, out ProcessResponse returnResponse)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plant.PlantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var processResponse = new ProcessResponse("Build MTS Schedule", true);
            processResponse.RowsAffected = 1;
            var f580020 = new F580020();
            var mtsSchedule = new MTSSchedule(schedulePlantNum, scheduleNum);
            var mtsScheduleEvent = new MTSScheduleEvent();
            try
            {
                var step1GetMTSScheduleEvent = new ProcessStep("Step 1. Build MTS Schedule Event from F580020");
                try
                {
                    f580020 = F580020Manager.GetF580020(e1Session, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
                    step1GetMTSScheduleEvent.RowsAffected = 1;
                    if (!step1GetMTSScheduleEvent.HasErrors() && f580020.S6Z58SCD > 0)
                    {
                        mtsScheduleEvent = MTSScheduleEventsManager.BuildMTSScheduleEventFromF580020(f580020, plant);
                        step1GetMTSScheduleEvent.RowsAffected = 1;
                    }
                }
                catch (Exception e)
                {
                    step1GetMTSScheduleEvent.Errors.Add(new Error(e));
                }
                finally
                {
                    step1GetMTSScheduleEvent.Done();
                    processResponse.AddStep(step1GetMTSScheduleEvent);
                }

                if (!step1GetMTSScheduleEvent.HasErrors() && f580020.S6Z58SCD > 0)
                {
                    var step2BuildMTSScheduleHeader = new ProcessStep("Step 2. Build MTS Schedule Header");
                    try
                    {
                        mtsSchedule = BuildMTSScheduleHeader(e1Session, schedulePlantNum, scheduleNum, f580020.S6Z58SCD);
                        mtsSchedule.E1ScheduleData.F580020 = f580020;
                        mtsSchedule.ScheduleDateE1 = mtsSchedule.E1ScheduleData.F580020.S6Z58SCD;
                        mtsSchedule.BatchNum = mtsSchedule.E1ScheduleData.F580020.S6EDBT;
                        mtsSchedule.MTSScheduleEvent = mtsScheduleEvent;
                        mtsSchedule.ScheduleDate = Utilities.Common.CastToDateTime(mtsSchedule.ScheduleDateE1);
                        mtsSchedule.MfgPlantNum = plant.PlantCode == "MZCG" ? "32" : mtsSchedule.PlantNum;
                        mtsSchedule.SchedulePlantNum = mtsSchedule.PlantNum;
                        step2BuildMTSScheduleHeader.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        step2BuildMTSScheduleHeader.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2BuildMTSScheduleHeader.Done();
                        processResponse.AddStep(step2BuildMTSScheduleHeader);
                    }

                    if (!step2BuildMTSScheduleHeader.HasErrors() && f580020.S6Z58SCD > 0)
                    {
                        var step3BuildMTSScheduleDetail = new ProcessStep("Step 3. Build MTS Schedule Detail");
                        try
                        {
                            mtsSchedule.MTSScheduleLines = BuildMTSScheduleDetail(e1Session, schedulePlantNum, scheduleNum, f580020.S6Z58SCD);
                            step3BuildMTSScheduleDetail.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                        }
                        catch (Exception e)
                        {
                            step3BuildMTSScheduleDetail.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3BuildMTSScheduleDetail.Done();
                            processResponse.AddStep(step3BuildMTSScheduleDetail);
                        }

                        if (!step3BuildMTSScheduleDetail.HasErrors() && mtsSchedule.ScheduleNum > 0 && !string.IsNullOrEmpty(mtsSchedule.E1ScheduleData.F580020.S6EDBT))
                        {
                            var step3GetF580021s = new ProcessStep("Step 4. Get MTS Schedule Lines F580021s");
                            try
                            {
                                mtsSchedule.E1ScheduleData.F580021s = F580021Manager.GetF580021s(e1Session, schedulePlantNum, scheduleNum, mtsSchedule.BatchNum);
                                step3GetF580021s.RowsAffected = mtsSchedule.E1ScheduleData.F580021s.Count;
                            }
                            catch (Exception e)
                            {
                                step3GetF580021s.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step3GetF580021s.Done();
                                processResponse.AddStep(step3GetF580021s);
                            }

                            if (!step3GetF580021s.HasErrors() && mtsSchedule.E1ScheduleData.F580021s.Any())
                            {
                                var step4GetF580024s = new ProcessStep("Step 5. Get MTS Schedule Lines F580024s");
                                try
                                {
                                    mtsSchedule.E1ScheduleData.F580024s = F580024Manager.GetF580024s(e1Session, schedulePlantNum, scheduleNum, mtsSchedule.BatchNum);
                                    step4GetF580024s.RowsAffected = mtsSchedule.E1ScheduleData.F580024s.Count;
                                }
                                catch (Exception e)
                                {
                                    step4GetF580024s.Errors.Add(new Error(e));
                                }
                                finally
                                {
                                    step4GetF580024s.Done();
                                    processResponse.AddStep(step4GetF580024s);
                                }

                                if (!step4GetF580024s.HasErrors() && mtsSchedule.E1ScheduleData.F580024s.Any())
                                {
                                    var step8BuildMTSIncomingOrderLines = new ProcessStep("Step 6. Build MTS Incoming Order Lines");
                                    try
                                    {
                                        mtsSchedule.MTSIncomingOrderLines = MTSIncomingOrderManager.BuildMTSIncomingOrderLines(e1Session, mtsSchedule.E1ScheduleData);
                                        step8BuildMTSIncomingOrderLines.RowsAffected = mtsSchedule.MTSIncomingOrderLines.Count;
                                    }
                                    catch (Exception e)
                                    {
                                        step8BuildMTSIncomingOrderLines.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step8BuildMTSIncomingOrderLines.Done();
                                        processResponse.AddStep(step8BuildMTSIncomingOrderLines);
                                    }

                                    var stepCustOverride = new ProcessStep("Customer Planner Override");
                                    try
                                    {
                                        var f4102Mgr = new F4102Manager(e1Session);
                                        var custNameManager = new CustomerNameManager(e1Session);
                                        int i = 0;
                                        foreach (var line in mtsSchedule.MTSIncomingOrderLines)
                                        {
                                            if (line.ProdType == "MTS")
                                            {
                                                // Immediate Customer via Planner Number
                                                var plannerNum = f4102Mgr.GetPlannerNum(e1Session, schedulePlantNum, Data.ParseStringToDecimal(line.ItemNum));
                                                if (plannerNum > 0)
                                                {
                                                    line.CustNum = plannerNum;
                                                    line.CustName = custNameManager.GetName(e1Session, plannerNum);
                                                    i++;
                                                }
                                                else if (plant.PlantCode == "NFCG")
                                                {
                                                    // If they didn't set up the PLN correctly, we don't want to use the wrong customer
                                                    line.CustNum = 0;
                                                    line.CustName = "";
                                                }

                                                // Orig Cust Num Sold To Name
                                                if (line.OrigCustNumSoldTo > 0)
                                                {
                                                    line.OrigCustNameSoldTo = custNameManager.GetName(e1Session, line.OrigCustNumSoldTo);
                                                }
                                            }
                                        }
                                        stepCustOverride.RowsAffected = i;
                                    }
                                    catch (Exception e)
                                    {
                                        stepCustOverride.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        stepCustOverride.Done();
                                        processResponse.AddStep(stepCustOverride);
                                    }

                                } // if (mtsSchedule.E1ScheduleData.F580024s.Any())
                                else
                                {
                                    var validationError = new ValidationError($"E1 no F580024 records found", "E1 Validation", "Error", true);
                                    mtsSchedule.ValidationErrors.Add(validationError);
                                    mtsSchedule.ValidationStatus = validationError.Message;
                                }
                            } // if (mtsSchedule.E1ScheduleData.F580021s.Any())
                            else
                            {
                                var validationError = new ValidationError($"E1 no F580021 records found", "E1 Validation", "Error", true);
                                mtsSchedule.ValidationErrors.Add(validationError);
                                mtsSchedule.ValidationStatus = validationError.Message;
                            }
                        } // if (!step2BuildMTSScheduleHeader.HasErrors() && f580020.S6Z58SCD > 0)
                    } // if (!step2BuildMTSScheduleHeader.HasErrors() && f580020.S6Z58SCD > 0)
                    else
                    {
                        var validationError = new ValidationError($"E1 no F5834A01 records found", "E1 Validation", "Error", true);
                        mtsSchedule.ValidationErrors.Add(validationError);
                        mtsSchedule.ValidationStatus = validationError.Message;
                    }
                } // if (!step1GetRecentScheduleDateE1.HasErrors() && scheduleDateE1 > 0)
                else
                {
                    var validationError = new ValidationError($"E1 no F580020 records found", "E1 Validation", "Error", true);
                    mtsSchedule.ValidationErrors.Add(validationError);
                    mtsSchedule.ValidationStatus = validationError.Message;
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                Logger.Info($"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                processResponse.Done();
                returnResponse = processResponse;
            }
            return mtsSchedule;
        }

        public static void NotifyMTSSchedule(ProcessResponse response, MTSSchedule mtsSchedule)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.MTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");
                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Get MTS Schedule - Schedule {mtsSchedule.ScheduleNum}";
                if (response.HasErrors() || mtsSchedule.ValidationErrors.Any()) subject = "Error - " + subject;
                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void SummarizeMTSSchedule(ProcessResponse response, MTSSchedule mtsSchedule)
        {
            var stepSummarize = new ProcessStep("Schedule Summary Collections");
            try
            {
                var scheduleCustomerLines =
                    mtsSchedule.MTSIncomingOrderLines.GroupBy(x => new
                    {
                        x.DisplayCustomer,
                        x.CustName,
                        x.WOrderNum,
                        x.ProdType,
                        x.AnsiiLogoType,
                        x.ItemNum,
                        x.Construction,
                        x.Dimensions,
                        x.RackType
                    })
                        .Select(
                            ng =>
                                new
                                {
                                    ng.Key.DisplayCustomer,
                                    ng.Key.CustName,
                                    ng.Key.WOrderNum,
                                    ng.Key.ProdType,
                                    ng.Key.AnsiiLogoType,
                                    ng.Key.ItemNum,
                                    ng.Key.Construction,
                                    ng.Key.Dimensions,
                                    ng.Key.RackType,
                                    ScheduleLines = (ng.Count()),
                                    WorkOrders = (ng.Select(x => x.WOrderNum).Distinct().Count()),
                                    LitesQty = (ng.Sum(x => x.OrderQty))
                                })
                        .ToList();
                response.SerializedObjects.Add("ScheduleCustomerLine", scheduleCustomerLines, "ScheduleCustomerLines");

                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtsSchedule.ScheduleNum,
                    mtsSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtsSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Lites = Convert.ToInt32(mtsSchedule.MTSIncomingOrderLines.Select(x => x.OrderQty).Sum()),
                    mtsSchedule.DueDate,
                    HasErrors = response.HasErrors() || mtsSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                stepSummarize.RowsAffected = mtsSchedule.MTSIncomingOrderLines.Count;
            }
            catch (Exception e)
            {
                stepSummarize.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarize.Done();
                response.ProcessSteps.Add(stepSummarize);
            }
        }

        /******************************************************************
         * Get MTSSchedule Lines
         ******************************************************************/
        private static iDB2Command GetMTSScheduleDetail()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        S5Z58SCSN,
                        S5Z58SCSTS,
                        S5DOCO,
                        S5DCTO,
					    S5RKCO,
                        S5ODOC,
                        S5RCTO,
                        S5LNID,
                        S5RORN,
                        S5RLLN,
                        S5SHAN,
                        S5ALPH,
                        S5VR01,
                        S5VR02,
                        S5ITM,
                        S5LITM,
                        S5DSC1,
                        S5DSC2,
                        S5Z58QTY,
                        S5UM,
                        S5DRQJ,
                        S5RSDJ,
                        S5RSDT,
                        S5Z58CSQNO,
                        S5Z58GSQNO,
                        S5Z58PT,
                        S5Z58IGLN,
                        S5Z58BOXP,
                        S5Z58BGEE,
                        S5Z58BGSZ,
                        S5Z58L1RG,
                        S5Z58L2RG,
                        S5Z58L3RG,
                        S5Z58MSG1,
                        S5Z58MSG2,
                        S5Z58CMT1,
                        S5Z58CMT2,
                        S5Z58CMT3,
                        BDZ58IGLN,
                        BDZ58CCV01,
                        BDZ58CCV02,
                        BDZ58CCV03
                    FROM
                        F5834A11     
                        LEFT OUTER JOIN F580232 
                            ON S5DOCO = BDZ58WODC
                    WHERE
                        S5MCU = ? AND
                        S5Z58SCN = ? AND
                        S5Z58SCD = ?
                    ORDER BY S5Z58SCSN"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ScheduleNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleNum"].Precision = 5;
            cmd.Parameters["ScheduleNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("ScheduleDateId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleDateId"].Precision = 8;
            cmd.Parameters["ScheduleDateId"].Scale = 0;
            return cmd;
        }

        public static MTSScheduleLines BuildMTSScheduleDetail(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var lines = new MTSScheduleLines();
            try
            {
                var cmd = GetMTSScheduleDetail();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleDateId;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var line = new MTSScheduleLine
                        {
                            WorkOrderNum = CastDbNull.To<decimal>(reader["S5DOCO"], 0),
                            ScheduleSeqNum = CastDbNull.To<decimal>(reader["S5Z58SCSN"], 0),
                            Quantity = CastDbNull.To<decimal>(reader["S5Z58QTY"], 0),
                            IgLine = CastDbNull.To(reader["S5Z58IGLN"], string.Empty).Trim(),
                            BoxPriority = CastDbNull.To(reader["S5Z58BOXP"], string.Empty).Trim(),
                            Bungee = CastDbNull.To(reader["S5Z58BGEE"], string.Empty).Trim(),
                            BungeeSize = CastDbNull.To<decimal>(reader["S5Z58BGSZ"], 0),
                            SchedulerMsg1 = CastDbNull.To(reader["S5Z58MSG1"], string.Empty).Trim(),
                            SchedulerMsg2 = CastDbNull.To(reader["S5Z58MSG2"], string.Empty).Trim(),
                            Comment1 = CastDbNull.To(reader["S5Z58CMT1"], string.Empty).Trim(),
                            Comment2 = CastDbNull.To(reader["S5Z58CMT2"], string.Empty).Trim(),
                            Comment3 = "",
                            ReleaseDate = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["S5RSDJ"], 0)),
                            RequestedDate = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["S5DRQJ"], 0)),
                            Value01 = CastDbNull.To(reader["BDZ58CCV01"], string.Empty).Trim(),
                            Value02 = CastDbNull.To(reader["BDZ58CCV02"], string.Empty).Trim(),
                            Value03 = CastDbNull.To(reader["BDZ58CCV03"], string.Empty).Trim(),
                            ShipToCustNum = CastDbNull.To<decimal>(reader["S5SHAN"], 0),
                        };
                        lines.Add(line);
                    }

                    reader.Close();
                    Logger.Info($"ROWS: {lines.Count}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return lines;
        }

        public static MTSScheduleLines GetMTSScheduleDetail(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateId, ItemAttributeCache cache)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var lines = new MTSScheduleLines();
            var customerMgr = new CustomerManager(e1Session);
            var workOrderMgr = new WorkOrderManager(e1Session);
            try
            {
                var cmd = GetMTSScheduleDetail();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleDateId;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var line = new MTSScheduleLine
                        {
                            WorkOrderNum = CastDbNull.To<decimal>(reader["S5DOCO"], 0),
                            ScheduleSeqNum = CastDbNull.To<decimal>(reader["S5Z58SCSN"], 0),
                            Quantity = CastDbNull.To<decimal>(reader["S5Z58QTY"], 0),
                            IgLine = CastDbNull.To(reader["S5Z58IGLN"], string.Empty).Trim(),
                            BoxPriority = CastDbNull.To(reader["S5Z58BOXP"], string.Empty).Trim(),
                            Bungee = CastDbNull.To(reader["S5Z58BGEE"], string.Empty).Trim(),
                            BungeeSize = CastDbNull.To<decimal>(reader["S5Z58BGSZ"], 0),
                            SchedulerMsg1 = CastDbNull.To(reader["S5Z58MSG1"], string.Empty).Trim(),
                            SchedulerMsg2 = CastDbNull.To(reader["S5Z58MSG2"], string.Empty).Trim(),
                            Comment1 = CastDbNull.To(reader["S5Z58CMT1"], string.Empty).Trim(),
                            Comment2 = CastDbNull.To(reader["S5Z58CMT2"], string.Empty).Trim()
                        };
                        var releaseDateId = CastDbNull.To<decimal>(reader["S5RSDJ"], 0);
                        line.ReleaseDate = Utilities.Common.CastToDateTime(releaseDateId);
                        var requestedDateId = CastDbNull.To<decimal>(reader["S5DRQJ"], 0);
                        line.RequestedDate = Utilities.Common.CastToDateTime(requestedDateId);
                        line.Value01 = CastDbNull.To(reader["BDZ58CCV01"], string.Empty).Trim();
                        line.Value02 = CastDbNull.To(reader["BDZ58CCV02"], string.Empty).Trim();
                        line.Value03 = CastDbNull.To(reader["BDZ58CCV03"], string.Empty).Trim();
                        var shipCustomerNum = CastDbNull.To<decimal>(reader["S5SHAN"], 0);
                        line.ShipToCustomer = customerMgr.GetCustomer(e1Session, shipCustomerNum);
                        lines.Add(line);
                    }

                    reader.Close();
                }

                Logger.Trace($"ROWS: {lines.Count}");
                foreach (var line in lines)
                {
                    line.WorkOrder = workOrderMgr.GetWorkOrder(e1Session, line.WorkOrderNum, cache);
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return lines;
        }

        public static ValidationErrors Validate(MTSSchedule mtsSchedule, SqlConnection cnSF, string shopFloorSystem)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var validationErrors = new ValidationErrors();
            var plantCode = PlantManager.GetPlant(mtsSchedule.PlantNum).PlantCode;
            try
            {
                if (!mtsSchedule.E1ScheduleData.F580021s.Any())
                {
                    var validationError = new ValidationError($"{plantCode} - {mtsSchedule.PlantNum} Sched {mtsSchedule.ScheduleNum} E1: no F580021 records found", "E1 Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtsSchedule.ValidationStatus = validationError.Message;
                }
                if (!mtsSchedule.E1ScheduleData.F580024s.Any())
                {
                    var validationError = new ValidationError($"{plantCode} - {mtsSchedule.PlantNum} Sched {mtsSchedule.ScheduleNum} E1: no F580024 records found", "E1 Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtsSchedule.ValidationStatus = validationError.Message;
                }
                if (mtsSchedule.E1ScheduleData.F580021s.Count != mtsSchedule.E1ScheduleData.F580024s.Count)
                {
                    var validationError = new ValidationError($"{plantCode} - {mtsSchedule.PlantNum} Sched {mtsSchedule.ScheduleNum} E1: F580021 record count does not match F580024 record count", "E1 Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtsSchedule.ValidationStatus = validationError.Message;
                }
                var missingDueDateCount = mtsSchedule.E1ScheduleData.F580021s.Count(f580021 => f580021.S7Z58SCDDJ == 0);
                if (missingDueDateCount != 0)
                {
                    var validationError = new ValidationError($@"{plantCode} - {mtsSchedule.PlantNum} Sched {mtsSchedule.ScheduleNum} E1: {missingDueDateCount} work orders on this MTS Schedule have a missing Due Date", "E1 Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtsSchedule.ValidationStatus = validationError.Message;
                }
                //else
                //{
                //    var zeroQuantityCount = mtsSchedule.E1ScheduleData.F580021s.Count(f580021 => f580021.S7Z58QTY < 1);
                //    if (zeroQuantityCount != 0)
                //    {
                //        var validationError = new ValidationError($@"{plantCode} - {mtsSchedule.PlantNum} Sched {mtsSchedule.ScheduleNum} E1: {missingDueDateCount} work orders on this MTO Schedule have a Quantity < 1", "E1 Validation", "Error", true);
                //        validationErrors.Add(validationError);
                //        mtsSchedule.ValidationStatus = validationError.Message;
                //    }
                //}
                if (mtsSchedule.E1ScheduleData.F580020.S6Z58SFS != shopFloorSystem)
                {
                    var validationError = new ValidationError($"{plantCode} - {mtsSchedule.PlantNum} Sched {mtsSchedule.ScheduleNum} E1: F580020 SFS does not equal {shopFloorSystem}", "E1 Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtsSchedule.ValidationStatus = validationError.Message;
                }
                var downloadedWorkOrdersCount = GetDownloadedWorkOrdersCountDynamic(cnSF, mtsSchedule);
                if (downloadedWorkOrdersCount != 0)
                {
                    var validationError = new ValidationError($@"{downloadedWorkOrdersCount} work orders on this MTS Schedule already exist in HVQue", "SF Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtsSchedule.ValidationStatus = validationError.Message;
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                if (!validationErrors.Any()) mtsSchedule.ValidationStatus = "Valid";
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return validationErrors;
        }

        public static int GetDownloadedWorkOrdersCountDynamic(SqlConnection cnSF, MTSSchedule mtsSchedule)
        {
            if (!mtsSchedule.MTSIncomingOrderLines.Any())
            {
                return 0;
            }
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var hvqueCount = 0;
            var filters = new List<string>();
            try
            {
                filters.Add($"[E1Schedule] = {mtsSchedule.ScheduleNum} AND \r\n");
                var inString = new StringBuilder();
                foreach (var line in mtsSchedule.MTSIncomingOrderLines)
                {
                    inString.Append(line.WOrderNum + ",");
                }

                if (inString.Length > 0) inString.Length -= 1; //Strip off trailing comma.
                filters.Add("[WOrderNum] IN (" + inString + ") \r\n");
                var cmd = GetHVQueByWorkOrder(filters);
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        hvqueCount = CastDbNull.To(reader["RECS"], 0);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + hvqueCount);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + cnSF.ConnectionString + "  " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return hvqueCount;
        }

        public static SqlCommand GetHVQueByWorkOrder(List<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT  COUNT(DISTINCT [WOrderNum]) AS 'RECS'
                    FROM    [dbo].[HVQue] hv WITH (NOLOCK)
                    {filterString}"
            };
            return cmd;
        }


        /******************************************************************
        * Get MTSSchedule
        ******************************************************************/
        private static iDB2Command GetMTSScheduleBySCD()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT
                        S1.S1MCU,
                        S1.S1Z58SCN,
                        S1.S1Z58SCD,
                        S1.S1Z58SFS,
                        DRDL01,
                        S1.S1Z58SCTYP,
                        S1.S1Z58SCSTS,
                        S1.S1Z58SCDDJ,
                        S1.S1Z58SAREA
                    FROM
                        F5834A01 S1
                        INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                        ON  DRSY = '58' AND
                            DRRT = 'SY' AND
                            TRIM(DRKY) = S1.S1Z58SFS
                    WHERE
                        S1.S1MCU = ? AND
                        S1.S1Z58SCN = ? AND
                        S1.S1Z58SCD = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ScheduleNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleNum"].Precision = 5;
            cmd.Parameters["ScheduleNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("S1Z58SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S1Z58SCD"].Precision = 6;
            cmd.Parameters["S1Z58SCD"].Scale = 0;
            return cmd;
        }

        public static MTSSchedule BuildMTSScheduleHeader(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var mtsSchedule = new MTSSchedule(plantNum, scheduleNum);
            try
            {
                var cmd = GetMTSScheduleBySCD();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                cmd.Parameters["ScheduleNum"].Value = scheduleNum;
                cmd.Parameters["S1Z58SCD"].Value = scheduleDateE1;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var rowCount = 0;
                    while (reader.Read())
                    {
                        mtsSchedule.PlantNum = plantNum;
                        mtsSchedule.ScheduleNum = CastDbNull.To<decimal>(reader["S1Z58SCN"], 0);
                        mtsSchedule.ScheduleDateE1 = CastDbNull.To<decimal>(reader["S1Z58SCD"], 0);
                        mtsSchedule.ScheduleDate = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["S1Z58SCD"], 0));
                        mtsSchedule.DueDate = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0));
                        mtsSchedule.ShopFloorCode = CastDbNull.To(reader["S1Z58SFS"], string.Empty).Trim();
                        mtsSchedule.ShopFloorName = CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        mtsSchedule.ScheduleStatus = CastDbNull.To(reader["S1Z58SCSTS"], string.Empty).Trim();
                        mtsSchedule.ScheduleType = CastDbNull.To(reader["S1Z58SCTYP"], string.Empty).Trim();
                        rowCount++;
                    }

                    reader.Close();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return mtsSchedule;
        }
    }
}