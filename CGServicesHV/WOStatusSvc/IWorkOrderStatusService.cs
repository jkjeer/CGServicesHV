using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.WOStatusSvc
{
    [ServiceContract]
    public interface IWorkOrderStatusService
    {
        [OperationContract, OperationInvokerLogger]
        PendingUpdatesResults GetPlantsWithPendingUpdates();

        [OperationContract, OperationInvokerLogger]
        SqlDmlResponse SetPendingUpdates();

        [OperationContract, OperationInvokerLogger]
        WorkOrderStatuses GetPendingUpdatesByPlant(string plantCode);

        [OperationContract, OperationInvokerLogger]
        PlantOperationResponses ProcessAll();
    }

    [ServiceContract]
    public interface IWorkOrderStatusRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPlantsWithPendingUpdates")]
        PendingUpdatesResults GetPlantsWithPendingUpdates();

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/SetPendingUpdates")]
        SqlDmlResponse SetPendingUpdates();

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/ProcessAll")]
        PlantOperationResponses ProcessAll();
    }
}
