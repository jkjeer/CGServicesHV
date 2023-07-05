using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.RackTagSvc;
using SoaCore.Wcf;

namespace CGServicesHV.RackSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class RackService : IRackService
    {
        public RackLoadItems GetFloatRackLoadItems(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            var processor = new RackProcessor();
            return processor.GetFloatRackLoadItems(plantCode);
        }

        public DataTable GetFloatRackLoadItemsDataTable(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            var processor = new RackProcessor();
            return processor.GetFloatRackLoadItems(plantCode).GetDataTable(); ;
        }

        public ProcessResponse DownloadRackEvents(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackEvents, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackItemEvents(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackItemEvents, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRacks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRacks, parallel, parms, resources);
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRacks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRacksArchive(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRacksArchive, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideRacks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRacks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackDetails(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                //BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRackDetails, parallel, parms, resources);
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackDetails, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackDetailsArchive(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackDetailsArchive, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideRackDetails(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRackDetails, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunRackAudit(string plants, bool showPlantOnly, bool isDelete, short maxDeletes, short retentionMinutes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ShowPlantOnly", showPlantOnly},
                    {"IsDelete", isDelete},
                    {"MaxDeletes", maxDeletes},
                    {"RetentionMinutes", retentionMinutes}
                };
                var parallel = false;
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.RunRackAudit, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunRackDetailAudit(string plants, bool showPlantOnly, bool isDelete, short maxDeletes, short retentionMinutes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ShowPlantOnly", showPlantOnly},
                    {"IsDelete", isDelete},
                    {"MaxDeletes", maxDeletes},
                    {"RetentionMinutes", retentionMinutes}
                };
                var parallel = false;
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.RunRackDetailAudit, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunRawTagsAudit(string plants, bool showPlantOnly, bool isDelete, short maxDeletes, short retentionMinutes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ShowPlantOnly", showPlantOnly},
                    {"IsDelete", isDelete},
                    {"MaxDeletes", maxDeletes},
                    {"RetentionMinutes", retentionMinutes}
                };
                var parallel = false;
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.RunRawTagsAudit, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackAssetXref(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string assetTypeCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"AssetTypeCodes", assetTypeCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackAssetXref, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackAssetTypes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string assetTypeCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"AssetTypeCodes", assetTypeCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackAssetTypes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackAssets(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackAssets, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRawTags(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                //BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRawTags, parallel, parms, resources);
                //return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRawTags, parallel, parms, resources);
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRawTags, parallel, parms, resources);
                //return BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRawTags, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideRawTags(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRawTags, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public Racks GetRacks(decimal rackId, string serialNum, string plantCode, string glCategory, double days, bool attributes)
        {
            try
            {
                return RackProcessor.GetRacks(rackId, serialNum, plantCode, glCategory, days, attributes);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetRackedOrders(string plant, decimal scheduleNum = 0, List<decimal> workOrderNums = null)
        {
            try
            {
                return RackProcessor.GetRackedOrders(plant, scheduleNum, workOrderNums);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse UpdateScheduleRackStatus(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.UpdateScheduleRackStatus, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class RackRestService : IRackRestService
    {
        public RackLoadItems GetFloatRackLoadItems(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            var processor = new RackProcessor();
            return processor.GetFloatRackLoadItems(plantCode);
        }

        public DataTable GetFloatRackLoadItemsDataTable(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            var processor = new RackProcessor();
            return processor.GetFloatRackLoadItems(plantCode).GetDataTable(); ;
        }

        public ProcessResponse DownloadRackEvents(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackEvents, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackItemEvents(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackItemEvents, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRacks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRacks, parallel, parms, resources);
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRacks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRacksArchive(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRacksArchive, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideRacks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRacks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackDetails(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                //BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRackDetails, parallel, parms, resources);
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackDetails, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackDetailsArchive(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackDetailsArchive, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideRackDetails(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRackDetails, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunRackAudit(string plants, bool showPlantOnly, bool isDelete, short maxDeletes, short retentionMinutes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ShowPlantOnly", showPlantOnly},
                    {"IsDelete", isDelete},
                    {"MaxDeletes", maxDeletes},
                    {"RetentionMinutes", retentionMinutes}
                };
                var parallel = false;
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.RunRackAudit, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunRackDetailAudit(string plants, bool showPlantOnly, bool isDelete, short maxDeletes, short retentionMinutes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ShowPlantOnly", showPlantOnly},
                    {"IsDelete", isDelete},
                    {"MaxDeletes", maxDeletes},
                    {"RetentionMinutes", retentionMinutes}
                };
                var parallel = false;
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.RunRackDetailAudit, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunRawTagsAudit(string plants, bool showPlantOnly, bool isDelete, short maxDeletes, short retentionMinutes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ShowPlantOnly", showPlantOnly},
                    {"IsDelete", isDelete},
                    {"MaxDeletes", maxDeletes},
                    {"RetentionMinutes", retentionMinutes}
                };
                var parallel = false;
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.RunRawTagsAudit, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackAssetXref(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string assetTypeCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"AssetTypeCodes", assetTypeCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackAssetXref, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackAssetTypes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string assetTypeCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"AssetTypeCodes", assetTypeCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackAssetTypes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackAssets(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackAssets, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRawTags(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRawTags, parallel, parms, resources);
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRawTags, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideRawTags(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"RackId", rackId},
                    {"StatusCodes", statusCodes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, OutsideRackProcessor.DownloadOutsideRawTags, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public Racks GetRacks(string rackId = null, string serialNum = null, string plantCode = null, string glCategory = null, string days = null, string attributes = null)
        {
            try
            {
                return RackProcessor.GetRacks(Convert.ToDecimal(rackId), serialNum, plantCode, glCategory, Convert.ToDouble(days), Convert.ToBoolean(attributes));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetRackedOrders(string plant, decimal scheduleNum, string workOrderNums)
        {
            try
            {
                var nums = workOrderNums?.Split(',').Select(decimal.Parse).ToList();         
                return RackProcessor.GetRackedOrders(plant, scheduleNum, nums);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message} {e.StackTrace}");
            }
        }

        public ProcessResponse UpdateScheduleRackStatus(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.UpdateScheduleRackStatus, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}
