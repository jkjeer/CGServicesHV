using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.ShippingSvc
{
    [ServiceContract]
    public interface IShippingService
    {
        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadShipmentPlanning(string plant, TimeSpan timeSpan, bool useHighWaterMark = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadShipmentSummary(string plant, TimeSpan timeSpan, bool useHighWaterMark = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadLoadHeader(string plant, TimeSpan timeSpan, bool useHighWaterMark = false);

    }

    [ServiceContract]
    public interface IShippingRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadShipmentPlanning?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}")]
        ProcessResponse DownloadShipmentPlanning(string plant, TimeSpan timeSpan, bool useHighWaterMark);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadShipmentSummary?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}")]
        ProcessResponse DownloadShipmentSummary(string plant, TimeSpan timeSpan, bool useHighWaterMark);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadLoadHeader?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}")]
        ProcessResponse DownloadLoadHeader(string plant, TimeSpan timeSpan, bool useHighWaterMark);
    }

}
