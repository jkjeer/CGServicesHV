using System;
using System.Collections.Generic;
using System.Linq;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using SoaCore.Wcf;

namespace CGServicesHV.ReferenceSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class ReferenceService : IReferenceService
    {
        public ProcessResponse DownloadSpecLegLengths(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadSpecLegLengths, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadInventoryLocations(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadInventoryLocations, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadEmployees(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadEmployees, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPlants(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadPlants, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        public ProcessResponse DownloadRackTypes(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRackTypes, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadConfiguratorRules(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadConfiguratorRules, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRejectCodes(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRejectCodes, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }




        public ProcessResponse DownloadLabor(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.WAREHOUSE" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadLabor, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadProductionLabor(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.WAREHOUSE" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadProductionLabor, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCustomerShipPreferences(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal customerNum = 0)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCustomerShipPreferences, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCardinalProductType(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal customerNum = 0)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCardinalProductType, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCalendars(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCalendars, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadBusinessUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadBusinessUnits, parallel, parms, resources);
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

        //public ProcessResponse DownloadWorkOrderStatus(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var parms = new Dictionary<string, object>
        //        {
        //            {"TimeSpan", timeSpan},
        //            {"HighWaterMark", useHighWaterMark},
        //            {"WorkOrderNum", workOrderNum}
        //        };
        //        var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
        //        return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadWorkOrderStatus, parallel, parms, resources);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}

        //public ProcessResponse DownloadRelatedOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var parms = new Dictionary<string, object>
        //        {
        //            {"TimeSpan", timeSpan},
        //            {"HighWaterMark", useHighWaterMark}
        //        };
        //        var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
        //        return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRelatedOrders, parallel, parms, resources);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}

        //public ProcessResponse DownloadPurchaseOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var parms = new Dictionary<string, object>
        //        {
        //            {"TimeSpan", timeSpan},
        //            {"HighWaterMark", useHighWaterMark},
        //            {"OrderNum", orderNum}
        //        };
        //        var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
        //        return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadPurchaseOrders, parallel, parms, resources);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class ReferenceRestService : IReferenceRestService
    {

        public ProcessResponse DownloadSpecLegLengths(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadSpecLegLengths, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadInventoryLocations(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                //var parms = new Dictionary<string, object>();
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadInventoryLocations, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadEmployees(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadEmployees, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPlants(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadPlants, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        public ProcessResponse DownloadRackTypes(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRackTypes, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadConfiguratorRules(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadConfiguratorRules, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRejectCodes(string plants, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRejectCodes, parallel, null, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }



        public ProcessResponse DownloadLabor(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.WAREHOUSE" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadLabor, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadProductionLabor(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.WAREHOUSE" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadProductionLabor, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCustomerHierarchy(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCustomerHierarchy, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCustomerShipPreferences(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal customerNum = 0)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCustomerShipPreferences, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCardinalProductType(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal customerNum = 0)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCardinalProductType, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCalendars(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadCalendars, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadBusinessUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadBusinessUnits, parallel, parms, resources);
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

        //public ProcessResponse DownloadWorkOrderStatus(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var parms = new Dictionary<string, object>
        //        {
        //            {"TimeSpan", timeSpan},
        //            {"HighWaterMark", useHighWaterMark},
        //            {"WorkOrderNum", workOrderNum}
        //        };
        //        var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
        //        return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadWorkOrderStatus, parallel, parms, resources);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}

        //public ProcessResponse DownloadRelatedOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var parms = new Dictionary<string, object>
        //        {
        //            {"TimeSpan", timeSpan},
        //            {"HighWaterMark", useHighWaterMark}
        //        };
        //        var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
        //        return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadRelatedOrders, parallel, parms, resources);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}

        //public ProcessResponse DownloadPurchaseOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var parms = new Dictionary<string, object>
        //        {
        //            {"TimeSpan", timeSpan},
        //            {"HighWaterMark", useHighWaterMark},
        //            {"OrderNum", orderNum}
        //        };
        //        var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "E1.HA" };
        //        return BusinessSystemTransferProcessor.Invoke(plants, ReferenceProcessor.DownloadPurchaseOrders, parallel, parms, resources);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}
    }
}
