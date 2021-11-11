using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.WOStatusSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class WorkOrderStatusService : IWorkOrderStatusService
    {
        public PendingUpdatesResults GetPlantsWithPendingUpdates()
        {
            var processor = new WorkOrderStatusProcessor();
            return processor.GetPlantsWithPendingUpdates();
        }

        public WorkOrderStatuses GetPendingUpdatesByPlant(string plantCode)
        {
            var processor = new WorkOrderStatusProcessor();
            return processor.GetPendingUpdatesByPlant(plantCode);
        }

        public SqlDmlResponse SetPendingUpdates()
        {
            var processor = new WorkOrderStatusProcessor();
            return processor.SetPendingUpdates();
        }

        public PlantOperationResponses ProcessAll()
        {
            var processor = new WorkOrderStatusProcessor();
            return processor.ProcessAll();
        }

    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class WorkOrderStatusRestService : IWorkOrderStatusRestService
    {
        public PendingUpdatesResults GetPlantsWithPendingUpdates()
        {
            var processor = new WorkOrderStatusProcessor();
            return processor.GetPlantsWithPendingUpdates();
        }

        public SqlDmlResponse SetPendingUpdates()
        {
            var processor = new WorkOrderStatusProcessor();
            return processor.SetPendingUpdates();
        }

        public PlantOperationResponses ProcessAll()
        {
            var processor = new WorkOrderStatusProcessor();
            return processor.ProcessAll();
        }

    }
}
