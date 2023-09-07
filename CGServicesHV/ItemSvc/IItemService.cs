using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.ItemSvc
{
    [ServiceContract]
    public interface IItemService
    {
        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadItemBalances(string plants, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadItems(string plants, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string stockTypes = null, string glCategories = null, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadItemCosts(string plants, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadItemUOMs(string plants, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadGlassItems(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal itemNum = 0, string stockTypes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadItemXref(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string xrefTypes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse UploadMachinePartTransaction(string plantCode);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse UploadMachineParts(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMtsSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMtoSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string rackStatusCodes = null, decimal rackId = 0, decimal configId = 0, decimal documentNum = 0, string documentType = null, string woStatusCodes = null, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadAccountBalances(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, int century, int year);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadAccounts(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

    }

    [ServiceContract]
    public interface IItemRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadItemBalances?Plant={plantCode}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadItemBalances(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadItems?Plant={plants}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&StockTypes={stockTypes}&GlCategories={glCategories}&ItemNum={itemNum}")]
        ProcessResponse DownloadItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string stockTypes, string glCategories, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadItemCosts?Plant={plants}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}")]
        ProcessResponse DownloadItemCosts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadItemUOMs?Plant={plants}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}")]
        ProcessResponse DownloadItemUOMs(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadGlassItemMetrics?Plant={plantCode}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Metrics={metrics}&ItemNum={itemNum}")]
        ProcessResponse DownloadGlassItemMetrics(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string metrics, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadItemMetrics?Plant={plantCode}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&DocumentType={documentTypes}")]
        ProcessResponse DownloadItemMetrics(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string documentTypes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadGlassItems?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}&StockTypes={stockTypes}")]
        ProcessResponse DownloadGlassItems(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum, string stockTypes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadItemXref?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&XrefTypes={xrefTypes}")]
        ProcessResponse DownloadItemXref(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string xrefTypes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/UploadMachinePartTransaction?Plant={plant}")]
        ProcessResponse UploadMachinePartTransaction(string plant);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/UploadMachineParts?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse UploadMachineParts(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMtsSpecUnits?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}")]
        ProcessResponse DownloadMtsSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMtoSpecUnits?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackStatusCodes={rackStatusCodes}&RackId={rackId}&ConfigId={configId}&DocumentNum={documentNum}&DocumentType={documentType}&WOStatusCodes={woStatusCodes}&ItemNum={itemNum}")]
        ProcessResponse DownloadMtoSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string rackStatusCodes, decimal rackId, decimal configId, decimal documentNum, string documentType, string woStatusCodes, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadSpecUnits?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}")]
        ProcessResponse DownloadSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum);


        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadAccountBalances?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Century={century}&Year={year}")]
        ProcessResponse DownloadAccountBalances(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, int century, int year);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadAccounts?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadAccounts(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);
    }
}
