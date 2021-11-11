using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.ReferenceSvc
{
    [ServiceContract]
    public interface IReferenceService
    {
        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadInventoryLocations(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string locationCode = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadUdcTypes(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string productCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadUdcItems(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string productCodes = null, string userCode = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackTypes(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string rackTypeCode = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadConfiguratorRules(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string tableName = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadCustomers(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal customerNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadSuppliers(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal supplierNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadEmployees(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal employeeNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadPlants(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string plantCode);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadAddressBook(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string searchTypes = null, decimal addressNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRejectCodes(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadWorkOrderStatus(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal workOrderNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRelatedOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadPurchaseOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal orderNum = 0);
    }

    [ServiceContract]
    public interface IReferenceRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadInventoryLocations?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&LocationCode={locationCode}")]
        ProcessResponse DownloadInventoryLocations(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string locationCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadUdcTypes?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ProductCodes={products}")]
        ProcessResponse DownloadUdcTypes(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string products);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadUdcItems?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ProductCodes={products}&UserCode={userCode}")]
        ProcessResponse DownloadUdcItems(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string products, string userCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackTypes?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackTypeCode={rackTypeCode}")]
        ProcessResponse DownloadRackTypes(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string rackTypeCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadConfiguratorRules?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&TableName={tableName}")]
        ProcessResponse DownloadConfiguratorRules(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string tableName);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadCustomers?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&CustomerNum={customerNum}")]
        ProcessResponse DownloadCustomers(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal customerNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadSuppliers?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&SupplierNum={supplierNum}")]
        ProcessResponse DownloadSuppliers(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal supplierNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadEmployees?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&EmployeeNum={employeeNum}")]
        ProcessResponse DownloadEmployees(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal employeeNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadPlants?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&PlantCode={plantCode}")]
        ProcessResponse DownloadPlants(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadAddressBook?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&SearchTypes={searchTypes}&AddressNum={addressNum}")]
        ProcessResponse DownloadAddressBook(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string searchTypes, decimal addressNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRejectCodes?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadRejectCodes(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

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
