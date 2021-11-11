using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.SubscriptionSvc
{
    [ServiceContract]
    public interface ISubscriptionService
    {
        [OperationContract, OperationInvokerLogger]
        Plants GetPlants(string code);

        [OperationContract, OperationInvokerLogger]
        PlantGroups GetPlantGroups();

        [OperationContract, OperationInvokerLogger]
        SubscriptionTypes GetSubscriptionTypes(string code);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse SetSubscription(string plant, string subscriptionType, bool isActive, bool isEnabled);

        //[OperationContract, OperationInvokerLogger]
        //ProcessResponse DownloadSubscription(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType);

        [OperationContract, OperationInvokerLogger]
        SubscriptionTypeItems RefreshSubscriptionTypeCache();

        [OperationContract, OperationInvokerLogger]
        ProcessResponse RunSubscriptionType(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse InitHighWaterMarks(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string markName);
    }

    [ServiceContract]
    public interface ISubscriptionRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPlants?Plant={plant}")]
        Plants GetPlants(string plant);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPlantGroups")]
        PlantGroups GetPlantGroups();

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetSubscriptionTypes?SubscriptionType={subscriptionType}")]
        SubscriptionTypes GetSubscriptionTypes(string subscriptionType);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/SetSubscription?Plant={plant}&SubscriptionType={subscriptionType}&IsActive={isActive}&IsEnabled={isEnabled}")]
        ProcessResponse SetSubscription(string plant, string subscriptionType, bool isActive, bool isEnabled);

        //[OperationContract, OperationInvokerLogger]
        //[WebGet(UriTemplate = "/DownloadSubscription?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&SubscriptionType={subscriptionType}")]
        //ProcessResponse DownloadSubscription(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/RefreshSubscriptionTypeCache")]
        SubscriptionTypeItems RefreshSubscriptionTypeCache();

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/RunSubscriptionType?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&SubscriptionType={subscriptionType}")]
        ProcessResponse RunSubscriptionType(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/InitHighWaterMarks?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&MarkName={markName}")]
        ProcessResponse InitHighWaterMarks(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string markName);
    }
}
