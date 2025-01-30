using System;
using System.Collections.Generic;
using System.Linq;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using CGServicesHV.RackSvc;

namespace CGServicesHV.ItemSvc
{
    //[ParameterInspectorLogger, CGIErrorHandler]
    public class ItemService : IItemService
    {

        public ProcessResponse RunSpecUnitAudit(string plants, bool sendReport)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                bool parallel = false;
                var parms = new Dictionary<string, object>();
                parms.Add("SendReport", sendReport);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION"};
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.RunSpecUnitAudit, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadItemAttributes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemAttributes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemAttributeMaster(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemAttributeMaster, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadBillOfMaterials(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadBillOfMaterials, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadAccountBalances(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, int century, int year)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("Century", century);
                parms.Add("Year", year);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.AccountBalanceDownload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadAccounts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.AccountDownload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemBalances(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemBalances, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemCosts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemCosts, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemUOMs(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemUOMs, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadGlassItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum = 0, string stockTypes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StockTypes", stockTypes?.Split(',').ToList()},
                    {"ItemNum", itemNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadGlassItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string stockTypes, string glCategories, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                parms.Add("StockTypes", stockTypes?.Split(',').ToList());
                parms.Add("GlCategories", glCategories?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemXref(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string xrefTypes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"XrefTypes", xrefTypes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemXref, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMtsSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum = 0)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"ItemNum", itemNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMtsSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMtoSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string rackStatusCodes, decimal rackId, decimal configId, decimal documentNum, string documentType, string woStatusCodes, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackStatusCodes", rackStatusCodes?.Split(',').ToList());
                parms.Add("RackId", rackId);
                parms.Add("ConfigId", configId);
                parms.Add("DocumentNum", documentNum);
                parms.Add("DocumentType", documentType);
                parms.Add("WOStatusCodes", woStatusCodes?.Split(',').ToList());
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMtoSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum = 0)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"ItemNum", itemNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse UploadMachinePartTransaction(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                return MachinePartProcessor.ConsumptionUpload(plantCode, "N");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse UploadMachineParts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.UploadMachineParts, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public class ItemRestService : IItemRestService
    {

        public ProcessResponse RunSpecUnitAudit(string plants, bool sendReport)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                bool parallel = false;
                var parms = new Dictionary<string, object>();
                parms.Add("SendReport", sendReport);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.RunSpecUnitAudit, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadItemAttributes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemAttributes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemAttributeMaster(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemAttributeMaster, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadBillOfMaterials(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadBillOfMaterials, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadAccountBalances(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, int century, int year)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("Century", century);
                parms.Add("Year", year);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.AccountBalanceDownload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadAccounts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.AccountDownload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadGlassItemMetrics(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string metrics, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                parms.Add("Metrics", metrics?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadGlassItemMetrics, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemMetrics(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string documentTypes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("DocumentType", documentTypes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemMetrics, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemBalances(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemBalances, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemCosts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemCosts, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemUOMs(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemUOMs, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadGlassItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum = 0, string stockTypes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StockTypes", stockTypes?.Split(',').ToList()},
                    {"ItemNum", itemNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadGlassItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string stockTypes, string glCategories, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                parms.Add("StockTypes", stockTypes?.Split(',').ToList());
                parms.Add("GlCategories", glCategories?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItemXref(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string xrefTypes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"XrefTypes", xrefTypes?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItemXref, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMtsSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum = 0)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"ItemNum", itemNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMtsSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMtoSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string rackStatusCodes, decimal rackId, decimal configId, decimal documentNum, string documentType, string woStatusCodes, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("RackStatusCodes", rackStatusCodes?.Split(',').ToList());
                parms.Add("RackId", rackId);
                parms.Add("ConfigId", configId);
                parms.Add("DocumentNum", documentNum);
                parms.Add("DocumentType", documentType);
                parms.Add("WOStatusCodes", woStatusCodes?.Split(',').ToList());
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMtoSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum = 0)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"ItemNum", itemNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse UploadMachinePartTransaction(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return MachinePartProcessor.ConsumptionUpload(plantCode, "N");
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse UploadMachineParts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.UploadMachineParts, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
