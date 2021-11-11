using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.ProductionSvc
{
    [ServiceContract]
    public interface IProductionService
    {
        [OperationContract, OperationInvokerLogger]
        ProcessResponse ProductionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        //[OperationContract, OperationInvokerLogger]
        //ProcessResponse ConsumedStoceUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        //[OperationContract, OperationInvokerLogger]
        //ProcessResponse CustPackUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse HvCompletionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse HvReceiptUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        //[OperationContract, OperationInvokerLogger]
        //ProcessResponse HvTrimUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse HvIssueUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse HvWoIssueUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse WoMgmtUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse RackTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse RackAssetTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);
    }

    [ServiceContract]
    public interface IProductionRestService
    {

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/ProductionUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse ProductionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        //[OperationContract, OperationInvokerLogger]
        //[WebGet(UriTemplate = "/ConsumedStoceUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        //ProcessResponse ConsumedStoceUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        //[OperationContract, OperationInvokerLogger]
        //[WebGet(UriTemplate = "/CustPackUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        //ProcessResponse CustPackUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/HvCompletionUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse HvCompletionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/HvReceiptUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse HvReceiptUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        //[OperationContract, OperationInvokerLogger]
        //[WebGet(UriTemplate = "/HvTrimUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        //ProcessResponse HvTrimUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/HvIssueUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse HvIssueUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/HvWoIssueUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse HvWoIssueUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/WoMgmtUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse WoMgmtUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/RackTransactionUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse RackTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/RackAssetTransactionUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse RackAssetTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);
    }
}


