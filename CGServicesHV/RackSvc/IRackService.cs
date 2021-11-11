using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.RackSvc
{
    [ServiceContract]
    public interface IRackService
    {

        [OperationContract, OperationInvokerLogger]
        Racks GetRacks(decimal rackId, string serialNum, string plantCode, string glCategory, double days, bool attributes);

        [OperationContract, OperationInvokerLogger]
        DataTable GetRackedOrders(string plant, decimal scheduleNum = 0, List<decimal> workOrderNums = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse UpdateScheduleRackStatus(string plant, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRacks(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadOutsideRacks(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackAssetXref(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string assetTypeCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackAssetTypes(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string assetTypeCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackAssets(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackItems(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackDetails(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadOutsideRackDetails(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse RunRackAudit(string plant, bool showPlantOnly = false, bool isDelete = false, short maxDeletes = 100, short retentionMinutes = 90);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse RunRackDetailAudit(string plant, bool showPlantOnly = false, bool isDelete = false, short maxDeletes = 0, short retentionMinutes = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse RunRawTagsAudit(string plant, bool showPlantOnly = false, bool isDelete = false, short maxDeletes = 100, short retentionMinutes = 90);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackEvents(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackItemEvents(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRawTags(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadOutsideRawTags(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);
    }

    [ServiceContract]
    public interface IRackRestService
    {

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRacks?RackId={rackId}&SerialNum={serialNum}&PlantCode={plantCode}&GlCategory={glCategory}&Days={days}&Attributes={attributes}")]
        Racks GetRacks(string rackId, string serialNum, string plantCode, string glCategory, string days, string attributes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackedOrders?Plant={plant}&ScheduleNum={scheduleNum}&WorkOrderNums={workOrderNums}")]
        DataTable GetRackedOrders(string plant, decimal scheduleNum, string workOrderNums);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/UpdateScheduleRackStatus?Plant={plant}&Parallel={parallel}")]
        ProcessResponse UpdateScheduleRackStatus(string plant, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRacks?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRacks(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadOutsideRacks?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadOutsideRacks(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackAssetTypes?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&AssetTypeCodes={assetTypeCodes}")]
        ProcessResponse DownloadRackAssetTypes(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string assetTypeCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackAssetXref?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&AssetTypeCodes={assetTypeCodes}")]
        ProcessResponse DownloadRackAssetXref(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string assetTypeCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackAssets?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRackAssets(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackItems?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRackItems(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackDetails?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRackDetails(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadOutsideRackDetails?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadOutsideRackDetails(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/RunRackAudit?Plant={plant}&ShowPlantOnly={showPlantOnly}&IsDelete={isDelete}&maxDeletes={MaxDeletes}&RetentionMinutes={retentionMinutes}")]
        ProcessResponse RunRackAudit(string plant, bool showPlantOnly = false, bool isDelete = false, short maxDeletes = 100, short retentionMinutes = 90);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/RunRackDetailAudit?Plant={plant}&ShowPlantOnly={showPlantOnly}&IsDelete={isDelete}&maxDeletes={MaxDeletes}&RetentionMinutes={retentionMinutes}")]
        ProcessResponse RunRackDetailAudit(string plant, bool showPlantOnly = false, bool isDelete = false, short maxDeletes = 0, short retentionMinutes = 0);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/RunRawTagsAudit?Plant={plant}&ShowPlantOnly={showPlantOnly}&IsDelete={isDelete}&maxDeletes={MaxDeletes}&RetentionMinutes={retentionMinutes}")]
        ProcessResponse RunRawTagsAudit(string plant, bool showPlantOnly = false, bool isDelete = false, short maxDeletes = 100, short retentionMinutes = 90);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackEvents?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRackEvents(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackItemEvents?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRackItemEvents(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRawTags?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRawTags(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadOutsideRawTags?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadOutsideRawTags(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);
    }
}


