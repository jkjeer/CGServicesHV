using System;
using System.Collections.Generic;
using System.Data;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.RackTagSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class RackTagService : IRackTagService
    {

        public RackTagItems GetRackTagItems(decimal rackId, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItems(rackId, byUnitId);
        }

        public DataTable GetRackTagItemsDataTable(decimal rackId, string filter = null, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            if (String.IsNullOrEmpty(filter))
            {
                return processor.GetRackTagItems(rackId, byUnitId).GetDataTable();
            }
            var rackTagItems = processor.GetRackTagItems(rackId, byUnitId);
            return processor.FilterRackTags(rackTagItems, filter).GetDataTable();
        }

        public bool GetRackTagItemsDataTable1(decimal rackId, string filter = null, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            if (String.IsNullOrEmpty(filter))
            {
                processor.GetRackTagItems(rackId, byUnitId).GetDataTable();
                return true;
            }
            var rackTagItems = processor.GetRackTagItems(rackId, byUnitId);
            processor.FilterRackTags(rackTagItems, filter).GetDataTable();
            return true;
        }

        public RackTagItems GetRackTagItemsBySchedule(string plantCode, decimal scheduleNum, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItemsBySchedule(plantCode, scheduleNum, byUnitId);
        }

        public RackTagItems GetRackTagItemsBySchedule(string plantCode, decimal scheduleNum, string filter, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            if (String.IsNullOrEmpty(filter))
            {
                return processor.GetRackTagItemsBySchedule(plantCode, scheduleNum, byUnitId);
            }
            var items = processor.GetRackTagItemsBySchedule(plantCode, scheduleNum, byUnitId);
            return processor.FilterRackTags(items, filter);
        }

        public DataTable GetRackTagItemsByScheduleDataTable(string plantCode, decimal scheduleNum, string filter, bool byUnitId = true)
        {
            return GetRackTagItemsBySchedule(plantCode, scheduleNum, filter, byUnitId).GetDataTable();
        }

        public List<decimal> GetRackIdListByStatus(string plantCode, string statusCode, TimeSpan timeSpan)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackIdListByStatus(plantCode, statusCode, timeSpan);
        }

        public List<Decimal> GetRackIdListBySchedule(string plantCode, decimal scheduleNum)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackIdListBySchedule(plantCode, scheduleNum);
        }

        public GenericGetResponse<RackStatusCode> GetRackStatusCodes()
        {
            var processor = new RackTagProcessor();
            return processor.GetRackStatusCodes();
        }

        public List<Decimal> GetLockedRacks(string plantCode, decimal minutesLatency)
        {
            var processor = new RackTagProcessor();
            return processor.GetLockedRacks(plantCode, minutesLatency);
        }

        // NEW
        public RackTagItems GetRackTags(string plant, decimal rackId, string groupStrategy)
        {
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetRackTags(plant, rackId, groupStrategy);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public HVRackTagItem GetHVRackTagMfg(string plant, string serialNum)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(serialNum))
            {
                throw new Exception("Serial Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagMfg(plant, serialNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetHVRackTagMfgDataTable(string plant, string serialNum)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(serialNum))
            {
                throw new Exception("Serial Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagMfg(plant, serialNum).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public HVRackTagItems GetHVRackTagsMfgByWorkOrder(string plant, decimal workOrder)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (workOrder == 0)
            {
                throw new Exception("Work Order Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagsMfgByWorkOrder(plant, workOrder);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetHVRackTagsMfgByWorkOrderDataTable(string plant, decimal workOrder)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (workOrder == 0)
            {
                throw new Exception("Work Order Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagsMfgByWorkOrder(plant, workOrder).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public HVRackTagItems GetHVRackTagsMfgBySchedule(string plant, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagsMfgBySchedule(plant, scheduleNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetHVRackTagsMfgByScheduleDataTable(string plant, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagsMfgBySchedule(plant, scheduleNum).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public RackTagItems GetRackTagsShipped(string plant, decimal rackId, string groupStrategy)
        {
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetRackTagsShipped(plant, rackId, groupStrategy);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetRackTagsDataTable(string plant, decimal rackId, string groupStrategy, string filter = null)
        {
            try
            {
                var processor = new RackTagProcessor();
                if (String.IsNullOrEmpty(filter))
                {
                    return processor.GetRackTags(plant, rackId, groupStrategy).GetDataTable();
                }
                var rackTagItems = processor.GetRackTags(plant, rackId, groupStrategy);
                return processor.FilterRackTags(rackTagItems, filter).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public RackTagItems GetRackTagsBySchedule(string plantCode, decimal scheduleNum, string groupStrategy)
        {
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetRackTagsBySchedule(plantCode, scheduleNum, groupStrategy);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public RackTagItems GetRackTagsBySchedule(string plantCode, decimal scheduleNum, string groupStrategy, string filter)
        {
            try
            {
                var processor = new RackTagProcessor();
                if (String.IsNullOrEmpty(filter))
                {
                    return processor.GetRackTagsBySchedule(plantCode, scheduleNum, groupStrategy);
                }
                var items = processor.GetRackTagsBySchedule(plantCode, scheduleNum, groupStrategy);
                return processor.FilterRackTags(items, filter);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetRackTagsByScheduleDataTable(string plantCode, decimal scheduleNum, string groupStrategy, string filter)
        {
            try
            {
                return GetRackTagsBySchedule(plantCode, scheduleNum, groupStrategy, filter).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public RackTagItems GetPlacard(string plant, Decimal rackId, string groupStrategy, string filter)
        {
            try
            {
                return new RackTagProcessor().GetPlacard(plant, rackId, groupStrategy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable GetPlacardDataTable(
            string plant,
            Decimal rackId,
            string groupStrategy,
            string filter)
        {
            try
            {
                RackTagProcessor rackTagProcessor = new RackTagProcessor();
                if (string.IsNullOrEmpty(filter))
                    return rackTagProcessor.GetPlacard(plant, rackId, groupStrategy).GetDataTable();
                RackTagItems placard = rackTagProcessor.GetPlacard(plant, rackId, groupStrategy);
                return rackTagProcessor.FilterRackTags(placard, filter).GetDataTable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class RackTagRestService : IRackTagRestService
    {

        public RackTagItems GetRackTagItems(string rackId, string filter, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            if (String.IsNullOrEmpty(filter))
            {
                return processor.GetRackTagItems(Convert.ToDecimal(rackId), byUnitId);
            }
            var rackTagItems = processor.GetRackTagItems(Convert.ToDecimal(rackId), byUnitId);
            return processor.FilterRackTags(rackTagItems, filter);
        }

        public DataTable GetRackTagItemsDataTable(string rackId, string filter, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            if (String.IsNullOrEmpty(filter))
            {
                return processor.GetRackTagItems(Convert.ToDecimal(rackId), byUnitId).GetDataTable();
            }
            var rackTagItems = processor.GetRackTagItems(Convert.ToDecimal(rackId), byUnitId);
            return processor.FilterRackTags(rackTagItems, filter).GetDataTable();
            //var processor = new RackTagProcessor();
            //return processor.GetRackTagItems(Convert.ToDecimal(RackId), filter, byUnitId).GetDataTable();
        }

        public RackTagItems GetRackTagItemsBySchedule(string plantCode, string scheduleNum, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItemsBySchedule(plantCode, Convert.ToDecimal(scheduleNum), byUnitId);
        }

        public DataTable GetRackTagItemsByScheduleDataTable(string plantCode, string scheduleNum, bool byUnitId = true)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItemsBySchedule(plantCode, Convert.ToDecimal(scheduleNum), byUnitId).GetDataTable();
        }

        public List<Decimal> GetRackIdListByStatus(string plantCode, string statusCode, string days, string hours, string minutes)
        {
            var processor = new RackTagProcessor();
            TimeSpan ts = new TimeSpan(Convert.ToInt16(days), Convert.ToInt16(hours), Convert.ToInt16(minutes), 0);
            return processor.GetRackIdListByStatus(plantCode, statusCode, ts);
        }

        public List<Decimal> GetRackIdListBySchedule(string plantCode, string scheduleNum)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackIdListBySchedule(plantCode, Convert.ToDecimal(scheduleNum));
        }

        public GenericGetResponse<RackStatusCode> GetRackStatusCodes()
        {
            var processor = new RackTagProcessor();
            return processor.GetRackStatusCodes();
        }

        public List<Decimal> GetLockedRacks(string plantCode, string minutesLatency)
        {
            var processor = new RackTagProcessor();
            return processor.GetLockedRacks(plantCode, Convert.ToDecimal(minutesLatency));
        }

        // NEW
        public RackTagItems GetRackTags(string plant, decimal rackId, string filter, string groupStrategy)
        {
            try
            {
                var processor = new RackTagProcessor();
                if (String.IsNullOrEmpty(filter))
                {
                    return processor.GetRackTags(plant, rackId, groupStrategy);
                }
                var rackTagItems = processor.GetRackTags(plant, rackId, groupStrategy);
                return processor.FilterRackTags(rackTagItems, filter);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public HVRackTagItem GetHVRackTagMfg(string plant, string serialNum)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(serialNum))
            {
                throw new Exception("Serial Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagMfg(plant, serialNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetHVRackTagMfgDataTable(string plant, string serialNum)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(serialNum))
            {
                throw new Exception("Serial Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagMfg(plant, serialNum).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public HVRackTagItems GetHVRackTagsMfgByWorkOrder(string plant, decimal workOrder)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (workOrder == 0)
            {
                throw new Exception("Work Order Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagsMfgByWorkOrder(plant, workOrder);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetHVRackTagsMfgByWorkOrderDataTable(string plant, decimal workOrder)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (workOrder == 0)
            {
                throw new Exception("Work Order Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagsMfgByWorkOrder(plant, workOrder).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public HVRackTagItems GetHVRackTagsMfgBySchedule(string plant, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagsMfgBySchedule(plant, scheduleNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetHVRackTagsMfgByScheduleDataTable(string plant, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plant))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetHVRackTagsMfgBySchedule(plant, scheduleNum).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public RackTagItems GetRackTagsShipped(string plant, decimal rackId, string groupStrategy)
        {
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetRackTagsShipped(plant, rackId, groupStrategy);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetRackTagsDataTable(string plant, decimal rackId, string groupStrategy, string filter)
        {
            try
            {
                var processor = new RackTagProcessor();
                if (String.IsNullOrEmpty(filter))
                {
                    return processor.GetRackTags(plant, rackId, groupStrategy).GetDataTable();
                }
                var rackTagItems = processor.GetRackTags(plant, rackId, groupStrategy);
                return processor.FilterRackTags(rackTagItems, filter).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public RackTagItems GetRackTagsBySchedule(string plantCode, decimal scheduleNum, string groupStrategy)
        {
            try
            {
                var processor = new RackTagProcessor();
                return processor.GetRackTagsBySchedule(plantCode, scheduleNum, groupStrategy);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataTable GetRackTagsByScheduleDataTable(string plantCode, decimal scheduleNum, string groupStrategy)
        {
            try
            {
                return GetRackTagsBySchedule(plantCode, scheduleNum, groupStrategy).GetDataTable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public RackTagItems GetPlacard(string plant, Decimal rackId, string groupStrategy, string filter)
        {
            try
            {
                return new RackTagProcessor().GetPlacard(plant, rackId, groupStrategy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable GetPlacardDataTable(
            string plant,
            Decimal rackId,
            string groupStrategy,
            string filter)
        {
            try
            {
                RackTagProcessor rackTagProcessor = new RackTagProcessor();
                if (string.IsNullOrEmpty(filter))
                    return rackTagProcessor.GetPlacard(plant, rackId, groupStrategy).GetDataTable();
                RackTagItems placard = rackTagProcessor.GetPlacard(plant, rackId, groupStrategy);
                return rackTagProcessor.FilterRackTags(placard, filter).GetDataTable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
