using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.ScheduleSvc
{
    public class ScheduleProcessor
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse DownloadMTSSchedule(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            // Called by MTS_SCHEDULE subscription
            var parallel = Data.UnboxParm.To(parms, "Parallel", false);
            var serviceContext = Data.UnboxParm.To<ServiceContext>(parms, "ServiceContext", null);
            var response = MTSScheduleDownloadManager.DownloadMTSIncomingOrders(session, sqlCns, plant, serviceContext, parallel);
            response.Close();
            return response;
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


        public static ProcessResponse DownloadScheduleHeader(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Schedule Header Download Processor", plant);
            try
            {
                //Data.UnboxParm.Validate(parms, new string[] { "TimeSpan", "HighWaterMark", "Parallel" });
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var mgr = new ScheduleHeaderDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse BackoutSchedule(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var startTime = DateTime.Now.ToLocalTime();
            var response = new ProcessResponse($"Schedule Backout Upload Processor", plant);
            try
            {
                var events = new List<ScheduleEvent>();
                var schedulePlantNum = Data.UnboxParm.To<string>(parms, "SchedulePlantNum", "");
                var scheduleNum = Data.UnboxParm.To<decimal>(parms, "ScheduleNum", 0);
                Logger.Info($"PLANT: {plant.PlantCode} - {schedulePlantNum} SCHEDULE: {scheduleNum} BEGIN: {startTime}");

                // Get Events
                var step1 = new ProcessStep($"Get {plant.PlantCode} - {schedulePlantNum} Backout Record");
                try
                {
                    events = ScheduleBackoutEventManager.GetEvent(sqlCns["PLANT.SHOPFLOOR"], plant, schedulePlantNum, scheduleNum);
                    if (events.Count == 0)
                    {
                        throw new Exception($"{plant.PlantCode} - {schedulePlantNum} Schedule {scheduleNum} does NOT exist in dbo.HVQue");
                    }
                    step1.RowsAffected = events.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                    Logger.Error(e.Message);
                }
                step1.Done();
                response.AddStep(step1);

                //// Reserve Events
                //if (events.Any())
                //{
                //    var step2 = new ProcessStep($"Reserve {plant.PlantCode} Events");
                //    try
                //    {
                //        foreach (ScheduleEvent evt in events)
                //        {
                //            step2.SqlDmlResponses.Add(ScheduleBackoutEventManager.ReserveStatus(sqlCns["PLANT.SHOPFLOOR"], evt));
                //        }
                //        step2.RowsAffected = step2.SqlDmlResponses.Count;
                //    }
                //    catch (Exception e)
                //    {
                //        step2.Errors.Add(new Error(e));
                //        Logger.Error(e.Message);
                //    }

                //    step2.Done();
                //    response.AddStep(step2);
                //}

                // Process Events
                if (events.Any())
                {
                    var step3 = new ProcessStep($"Process {plant.PlantCode} - {schedulePlantNum} Events");
                    try
                    {
                        foreach (ScheduleEvent evt in events)
                        {
                            var scheduleResponse = ScheduleBackoutUploadManager.Process(session, sqlCns, plant, evt);
                            step3.ProcessResponses.Add(scheduleResponse);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    step3.Done();
                    response.AddStep(step3);
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static WorkOrder GetWorkOrder(string plantCode, decimal workOrderNum)
        {
            var session = new BusinessSystemSession();
            try
            {
                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Process
                var mgr = new WorkOrderManager(session);
                return mgr.GetWorkOrder(session, workOrderNum);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw new Exception(e.ToString());
            }
            finally
            {
                session.Close();
            }
        }
    }
}
