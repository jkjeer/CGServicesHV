using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.ScheduleSvc
{
    [ServiceContract]
    public interface IScheduleService
    {
        [OperationContract, OperationInvokerLogger]
        MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        MTSScheduleEvents GetUnprocessedOutsideMTSScheduleEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse BackoutSchedule(string plantCode, string schedulePlantNum, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse CancelMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse ResetOutsideMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse SuccessMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse CompleteMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadHVQueTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadOutsideHVQueTable(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);
    }

    [ServiceContract]
    public interface IScheduleRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetUnprocessedMTSScheduleEvents?PlantCode={plantCode}")]
        MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetUnprocessedOutsideMTSScheduleEvents?PlantCode={plantCode}")]
        MTSScheduleEvents GetUnprocessedOutsideMTSScheduleEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/BackoutSchedule?PlantCode={plantCode}&SchedulePlantNum={schedulePlantNum}&ScheduleNum={scheduleNum}")]
        ProcessResponse BackoutSchedule(string plantCode, string schedulePlantNum, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/CancelMTSScheduleEvent?PlantCode={plantCode}&SchedulePlantNum={schedulePlantNum}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse CancelMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/ResetMTSScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/ResetOutsideMTSScheduleEvent?PlantCode={plantCode}&SchedulePlantNum={schedulePlantNum}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse ResetOutsideMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/SuccessMTSScheduleEvent?PlantCode={plantCode}&SchedulePlantNum={schedulePlantNum}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse SuccessMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/CompleteMTSScheduleEvent?PlantCode={plantCode}&SchedulePlantNum={schedulePlantNum}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse CompleteMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadHVQueTable?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse DownloadHVQueTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadOutsideHVQueTable?PlantCode={plantCode}&SchedulePlantNum={schedulePlantNum}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse DownloadOutsideHVQueTable(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);
    }
}
