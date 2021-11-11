using System;
using System.Collections.Generic;
using System.Linq;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.ReferenceSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class ReferenceService : IReferenceService
    {
        public ProcessResponse DownloadInventoryLocations(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string locationCode = null)
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
                    {"LocationCode", locationCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadInventoryLocations, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadUdcItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string products = null, string userCode = null)
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
                    {"Products", products?.Split(',').ToList()},
                    {"UserCode", userCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadUdcItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadUdcTypes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string products = null)
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
                    {"Products", products?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadUdcTypes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackTypes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string rackTypeCode = null)
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
                    {"RackTypeCode", rackTypeCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRackTypes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadConfiguratorRules(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string tableName = null)
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
                    {"TableName", tableName}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadConfiguratorRules, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPlants(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string plantCode)
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
                    {"PlantCodes", plantCode?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadPlants, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCustomers(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal customerNum = 0)
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
                    {"CustomerNum", customerNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCustomers, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadSuppliers(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal supplierNum = 0)
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
                    {"SupplierNum", supplierNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadSuppliers, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadEmployees(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal employeeNum = 0)
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
                    {"EmployeeNum", employeeNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadEmployees, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadAddressBook(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string searchTypes = "", decimal addressNum = 0)
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
                    {"SearchTypes", searchTypes?.Split(',').ToList()},
                    {"AddressNum", addressNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadAddressBook, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRejectCodes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRejectCodes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrderStatus(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0)
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
                    {"WorkOrderNum", workOrderNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadWorkOrderStatus, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRelatedOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRelatedOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPurchaseOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum)
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
                    {"OrderNum", orderNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadPurchaseOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class ReferenceRestService : IReferenceRestService
    {

        public ProcessResponse DownloadInventoryLocations(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string locationCode = null)
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
                    {"LocationCode", locationCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadInventoryLocations, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadUdcItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string products = null, string userCode = null)
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
                    {"Products", products?.Split(',').ToList()},
                    {"UserCode", userCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadUdcItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadUdcTypes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string products = null)
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
                    {"Products", products?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadUdcTypes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackTypes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string rackTypeCode = null)
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
                    {"RackTypeCode", rackTypeCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRackTypes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadConfiguratorRules(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string tableName = null)
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
                    {"TableName", tableName}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadConfiguratorRules, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPlants(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string plantCode)
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
                    {"PlantCodes", plantCode?.Split(',').ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadPlants, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCustomers(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal customerNum = 0)
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
                    {"CustomerNum", customerNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCustomers, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadSuppliers(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal supplierNum = 0)
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
                    {"SupplierNum", supplierNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadSuppliers, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadEmployees(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal employeeNum = 0)
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
                    {"EmployeeNum", employeeNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadEmployees, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadAddressBook(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string searchTypes = "", decimal addressNum = 0)
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
                    {"SearchTypes", searchTypes?.Split(',').ToList()},
                    {"AddressNum", addressNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadAddressBook, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRejectCodes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRejectCodes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrderStatus(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0)
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
                    {"WorkOrderNum", workOrderNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadWorkOrderStatus, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRelatedOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRelatedOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPurchaseOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum)
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
                    {"OrderNum", orderNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadPurchaseOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}
