using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.RackTagSvc
{
    [ServiceContract]
    public interface IRackTagService
    {
        [OperationContract, OperationInvokerLogger]
        List<decimal> GetRackIdListByStatus(string plantCode, string statusCode, TimeSpan timeSpan);

        [OperationContract, OperationInvokerLogger]
        List<decimal> GetRackIdListBySchedule(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        GenericGetResponse<RackStatusCode> GetRackStatusCodes();

        [OperationContract, OperationInvokerLogger]
        List<decimal> GetLockedRacks(string plantCode, decimal minutesLatency);

        // Old
        [OperationContract, OperationInvokerLogger]
        RackTagItems GetRackTagItems(decimal rackId, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        DataTable GetRackTagItemsDataTable(decimal rackId, string filter, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        bool GetRackTagItemsDataTable1(decimal rackId, string filter, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        RackTagItems GetRackTagItemsBySchedule(string plantCode, decimal scheduleNum, string filter, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        DataTable GetRackTagItemsByScheduleDataTable(string plantCode, decimal scheduleNum, string filter, bool byUnitId = true);

        // New
        [OperationContract, OperationInvokerLogger]
        RackTagItems GetRackTags(string plant, decimal rackId, string groupStrategy);

        [OperationContract, OperationInvokerLogger]
        HVRackTagItem GetHVRackTagMfg(string plant, string serialNum);

        [OperationContract, OperationInvokerLogger]
        DataTable GetHVRackTagMfgDataTable(string plant, string serialNum);

        [OperationContract, OperationInvokerLogger]
        HVRackTagItems GetHVRackTagsMfgByWorkOrder(string plant, decimal workOrder);

        [OperationContract, OperationInvokerLogger]
        DataTable GetHVRackTagsMfgByWorkOrderDataTable(string plant, decimal workOrder);

        [OperationContract, OperationInvokerLogger]
        HVRackTagItems GetHVRackTagsMfgBySchedule(string plant, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        DataTable GetHVRackTagsMfgByScheduleDataTable(string plant, decimal scheduleNum);


        [OperationContract, OperationInvokerLogger]
        RackTagItems GetRackTagsShipped(string plant, decimal rackId, string groupStrategy);

        [OperationContract, OperationInvokerLogger]
        DataTable GetRackTagsDataTable(string plant, decimal rackId, string groupStrategy, string filter);

        [OperationContract, OperationInvokerLogger]
        RackTagItems GetRackTagsBySchedule(string plant, decimal scheduleNum, string groupStrategy, string filter);

        [OperationContract, OperationInvokerLogger]
        DataTable GetRackTagsByScheduleDataTable(string plant, decimal scheduleNum, string groupStrategy, string filter);

        [OperationContract, OperationInvokerLogger]
        RackTagItems GetPlacard(string plant, Decimal rackId, string groupStrategy, string filter);

        [OperationContract, OperationInvokerLogger]
        DataTable GetPlacardDataTable(string plant, Decimal rackId, string groupStrategy, string filter);
    }

    [ServiceContract]
    public interface IRackTagRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackIdListByStatus?PlantCode={plantCode}&StatusCode={statusCode}&Days={days}&Hours={hours}&Minutes={minutes}")]
        List<decimal> GetRackIdListByStatus(string plantCode, string statusCode, string days, string hours, string minutes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackIdListBySchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}")]
        List<decimal> GetRackIdListBySchedule(string plantCode, string scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackStatusCodes")]
        GenericGetResponse<RackStatusCode> GetRackStatusCodes();

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetLockedRacks?PlantCode={plantCode}&Minutes={minutesLatency}")]
        List<decimal> GetLockedRacks(string plantCode, string minutesLatency);

        // Old
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItems?RackId={rackId}&Filter={filter}&ByUnitId={byUnitId}")]
        RackTagItems GetRackTagItems(string rackId, string filter, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItemsDataTable?RackId={rackId}&Filter={filter}&ByUnitId={byUnitId}")]
        DataTable GetRackTagItemsDataTable(string rackId, string filter, bool byUnitId = true);

        //[OperationContract, OperationInvokerLogger]
        //[WebGet(UriTemplate = "/GetRackTagItemsDataTable?RackId={rackId}&Filter={filter}&ByUnitId={byUnitId}")]
        //bool GetRackTagItemsDataTable1(string rackId, string filter, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItemsBySchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ByUnitId={byUnitId}")]
        RackTagItems GetRackTagItemsBySchedule(string plantCode, string scheduleNum, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItemsByScheduleDataTable?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ByUnitId={byUnitId}")]
        DataTable GetRackTagItemsByScheduleDataTable(string plantCode, string scheduleNum, bool byUnitId = true);

        // New
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTags?Plant={plant}&RackId={rackId}&Filter={filter}&GroupStrategy={groupStrategy}")]
        RackTagItems GetRackTags(string plant, decimal rackId, string filter, string groupStrategy);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetHVRackTagMfg?Plant={plant}&SerialNum={serialNum}")]
        HVRackTagItem GetHVRackTagMfg(string plant, string serialNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetHVRackTagMfgDataTable?Plant={plant}&SerialNum={serialNum}")]
        DataTable GetHVRackTagMfgDataTable(string plant, string serialNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetHVRackTagsMfgByWorkOrder?Plant={plant}&WorkOrder={workOrder}")]
        HVRackTagItems GetHVRackTagsMfgByWorkOrder(string plant, decimal workOrder);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetHVRackTagsMfgByWorkOrderDataTable?Plant={plant}&WorkOrder={workOrder}")]
        DataTable GetHVRackTagsMfgByWorkOrderDataTable(string plant, decimal workOrder);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetHVRackTagsMfgBySchedule?Plant={plant}&ScheduleNum={scheduleNum}")]
        HVRackTagItems GetHVRackTagsMfgBySchedule(string plant, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetHVRackTagsMfgByScheduleDataTable?Plant={plant}&ScheduleNum={scheduleNum}")]
        DataTable GetHVRackTagsMfgByScheduleDataTable(string plant, decimal scheduleNum);


        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagsShipped?Plant={plant}&RackId={rackId}&GroupStrategy={groupStrategy}")]
        RackTagItems GetRackTagsShipped(string plant, decimal rackId, string groupStrategy);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagsDataTable?Plant={plant}&RackId={rackId}&GroupStrategy={groupStrategy}&Filter={filter}")]
        DataTable GetRackTagsDataTable(string plant, decimal rackId, string groupStrategy, string filter);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagsBySchedule?Plant={plantCode}&ScheduleNum={scheduleNum}&GroupStrategy={groupStrategy}")]
        RackTagItems GetRackTagsBySchedule(string plantCode, decimal scheduleNum, string groupStrategy);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagsByScheduleDataTable?Plant={plantCode}&ScheduleNum={scheduleNum}&GroupStrategy={groupStrategy}")]
        DataTable GetRackTagsByScheduleDataTable(string plantCode, decimal scheduleNum, string groupStrategy);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPlacard?Plant={plant}&RackId={rackId}&Filter={filter}&GroupStrategy={groupStrategy}")]
        RackTagItems GetPlacard(string plant, Decimal rackId, string groupStrategy, string filter);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPlacardDataTable?Plant={plant}&RackId={rackId}&Filter={filter}&GroupStrategy={groupStrategy}")]
        DataTable GetPlacardDataTable(string plant, Decimal rackId, string groupStrategy, string filter);
    }
}
