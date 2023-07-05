using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.OrderSvc
{
    [ServiceContract]
    public interface IOrderService
    {
        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadSalesLines(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal workOrderNum = 0, string nextStatusCodes = null, string orderTypes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadSalesLineMessages(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadWorkOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal workOrderNum = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadWorkOrderParts(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal orderNum = 0, string startDate = null, string endDate = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadWorkOrderCosts(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal orderNum = 0, string startDate = null, string endDate = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadOutsideWorkOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal workOrderNum = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadWorkOrderStatus(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal workOrderNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRelatedOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadPurchaseOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal orderNum = 0);

    }

    [ServiceContract]
    public interface IOrderRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadSalesLines?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&SalesOrderNum={workOrderNum}&NextStatusCode={nextStatusCodes}&OrderType={orderTypes}")]
        ProcessResponse DownloadSalesLines(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum, string nextStatusCodes, string orderTypes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadSalesLineMessages?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadSalesLineMessages(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadWorkOrders?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&WorkOrderNum={workOrderNum}&StatusCode={statusCodes}")]
        ProcessResponse DownloadWorkOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum, string statusCodes);


        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadWorkOrderParts?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&OrderNum={orderNum}&StartDate={startDate}&EndDate={endDate}")]
        ProcessResponse DownloadWorkOrderParts(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum, string startDate, string endDate);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadWorkOrderCosts?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&OrderNum={orderNum}&StartDate={startDate}&EndDate={endDate}")]
        ProcessResponse DownloadWorkOrderCosts(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum, string startDate, string endDate);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadOutsideWorkOrders?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&WorkOrderNum={workOrderNum}&StatusCode={statusCodes}")]
        ProcessResponse DownloadOutsideWorkOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadWorkOrderStatus?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&WorkOrderNum={workOrderNum}")]
        ProcessResponse DownloadWorkOrderStatus(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRelatedOrders?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadRelatedOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadPurchaseOrders?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&OrderNum={orderNum}")]
        ProcessResponse DownloadPurchaseOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum);

    }
}
