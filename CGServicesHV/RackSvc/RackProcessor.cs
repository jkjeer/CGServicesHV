using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.RackSvc
{
    public class RackProcessor
    {
        /********************************************************************************
        * METHODS
        ********************************************************************************/
        public static ProcessResponse DownloadRackAssetXref(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Asset Xref Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var assetTypeCodes = Data.UnboxParm.To(parms, "AssetTypeCodes", new List<string>());     
                var mgr = new RackAssetXrefDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, assetTypeCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRackAssetTypes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Asset Type Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var assetTypeCodes = Data.UnboxParm.To(parms, "AssetTypeCodes", new List<string>());
                var mgr = new RackAssetTypeDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, assetTypeCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRackAssets(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Asset Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackAssetDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRackEvents(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackEventDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRackItemEvents(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Item Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackItemEventDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRacks(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRackDetails(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Detail Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackDetailDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadRackItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Item Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackItemDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse RunRackAudit(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Audit Processor", plant);
            try
            {
                var showPlantOnly = Data.UnboxParm.To(parms, "ShowPlantOnly", false);
                var isDelete = Data.UnboxParm.To(parms, "IsDelete", false);
                var maxDeletes = Data.UnboxParm.To<short>(parms, "MaxDeletes", 100);
                if (maxDeletes == 0) maxDeletes = 100;
                var retentionMinutes = Data.UnboxParm.To<short>(parms, "RetentionMinutes", 90);
                if (retentionMinutes == 0) retentionMinutes = 90;
                var mgr = new RackAuditManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, showPlantOnly, isDelete, maxDeletes, retentionMinutes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse RunRackDetailAudit(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Detail Audit Processor", plant);
            try
            {
                var showPlantOnly = Data.UnboxParm.To(parms, "ShowPlantOnly", false);
                var isDelete = Data.UnboxParm.To(parms, "IsDelete", false);
                var maxDeletes = Data.UnboxParm.To<short>(parms, "MaxDeletes", 100);
                if (maxDeletes == 0) maxDeletes = 100;
                var retentionMinutes = Data.UnboxParm.To<short>(parms, "RetentionMinutes", 90);
                if (retentionMinutes == 0) retentionMinutes = 90;
                var mgr = new RackDetailAuditManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, showPlantOnly, isDelete, maxDeletes, retentionMinutes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse RunRawTagsAudit(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"RawTags Audit Processor", plant);
            try
            {
                var showPlantOnly = Data.UnboxParm.To(parms, "ShowPlantOnly", false);
                var isDelete = Data.UnboxParm.To(parms, "IsDelete", false);
                var maxDeletes = Data.UnboxParm.To<short>(parms, "MaxDeletes", 100);
                if (maxDeletes == 0) maxDeletes = 100;
                var retentionMinutes = Data.UnboxParm.To<short>(parms, "RetentionMinutes", 90);
                if (retentionMinutes == 0) retentionMinutes = 90;
                var mgr = new RawTagsAuditManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, showPlantOnly, isDelete, maxDeletes, retentionMinutes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRawTags(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Raw Tags Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RawTagsDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }


        public static Racks GetRacks(decimal rackId, string serialNum, string plantCode, string glCategory, double days, bool attributes)
        {
            var session = new BusinessSystemSession();
            try
            {
                session = BusinessSystemSessionManager.GetBusinessSession();
                var rackManager = new RackManager(session);
                return rackManager.GetRacks(session, rackId, serialNum, plantCode, attributes);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
            }
        }

        public static DataTable GetRackedOrders(string plantCode, decimal scheduleNum, List<decimal> workOrderNums)
        {
            var session = new BusinessSystemSession();
            var cnShopFloor = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                session = BusinessSystemSessionManager.GetBusinessSession();
                var rackManager = new RackManager(session);
                return rackManager.GetRackedOrders(session, plant, scheduleNum, workOrderNums);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message} {e.StackTrace}");
            }
            finally
            {
                session.Close();
                cnShopFloor.Close();
            }
        }

        public static ProcessResponse UpdateScheduleRackStatus(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Schedule Rack Status Manager");
            try
            {
                var mgr = new RackedScheduleUpdateManager(session);
                response = mgr.Process(session, sqlCns, plant);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }
    }
}
