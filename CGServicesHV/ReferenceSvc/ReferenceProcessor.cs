using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.ReferenceSvc
{
    public class ReferenceProcessor
    {
        /********************************************************************************
        * METHODS
        ********************************************************************************/
        public static ProcessResponse DownloadSpecLegLengths(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Spec Leg Length Download Manager", plant);
            try
            {
                response.ProcessResponses.Add(SpecLegLengthDownloadManager.Process(session, sqlCns, plant));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadInventoryLocations(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Inventory Location Download Processor", plant);
            try
            {
                response.ProcessResponses.Add(InventoryLocationDownloadManager.Process(session, sqlCns, plant));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadEmployees(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Employee Download Processor", plant);
            try
            {
                response.ProcessResponses.Add(EmployeeDownloadManager.Process(session, sqlCns, plant));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadPlants(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Plant Download Processor", plant);
            try
            {
                response.ProcessResponses.Add(PlantDownloadManager.Process(session, sqlCns, plant));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRackTypes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Type Download Manager", plant);
            try
            {
                response.ProcessResponses.Add(RackTypeDownloadManager.Process(session, sqlCns, plant));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadConfiguratorRules(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Configurator Rules Download Manager", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(ConfiguratorRuleDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadRejectCodes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Reject Code Download Manager", plant);
            try
            {
                response.ProcessResponses.Add(RejectCodeDownloadManager.Process(session, sqlCns, plant));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            response.Close();
            return response;
        }


        public static ProcessResponse DownloadCalendars(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Calendar Download Processor", plant);
            try
            {
                //Data.UnboxParm.Validate(parms, new string[] { "TimeSpan", "HighWaterMark", "Parallel", "CustomerNum" });
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(CalendarDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadLabor(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Labor Download Processor", plant);
            try
            {
                //Data.UnboxParm.Validate(parms, new string[] { "TimeSpan", "HighWaterMark", "Parallel" });
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(LaborDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadProductionLabor(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Production Labor Download Processor", plant);
            try
            {
                //Data.UnboxParm.Validate(parms, new string[] { "TimeSpan", "HighWaterMark", "Parallel" });
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(ProductionLaborDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadCustomerHierarchy(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Customer Hierarchy Download Processor", plant);
            try
            {
                //Data.UnboxParm.Validate(parms, new string[] { "TimeSpan", "HighWaterMark", "Parallel", "CustomerNum" });
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var customerNum = Data.UnboxParm.To<decimal>(parms, "CustomerNum", 0);
                response.ProcessResponses.Add(CustomerHierarcyDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadCustomerShipPreferences(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Customer Ship Preference Download Processor", plant);
            try
            {
                //Data.UnboxParm.Validate(parms, new string[] { "TimeSpan", "HighWaterMark", "Parallel", "CustomerNum" });
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var customerNum = Data.UnboxParm.To<decimal>(parms, "CustomerNum", 0);
                response.ProcessResponses.Add(CustomerShipPreferenceDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, customerNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadCardinalProductType(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Cardinal Product Type Download Processor", plant);
            try
            {
                //Data.UnboxParm.Validate(parms, new string[] { "TimeSpan", "HighWaterMark", "Parallel", "CustomerNum" });
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var customerNum = Data.UnboxParm.To<decimal>(parms, "CustomerNum", 0);
                response.ProcessResponses.Add(CardinalProductTypeDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, customerNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadBusinessUnits(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Udc Item Download Processor", plant);
            try
            {
                //Data.UnboxParm.Validate(parms, new string[] { "TimeSpan", "HighWaterMark", "Parallel" });
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(BusinessUnitDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadUdcItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Udc Item Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var productCodes = Data.UnboxParm.To(parms, "Products", new List<string>());
                var userCode = Data.UnboxParm.To(parms, "UserCode", string.Empty);
                response.ProcessResponses.Add(UdcItemDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, productCodes, userCode));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadUdcTypes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Udc Type Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var productCodes = Data.UnboxParm.To(parms, "Products", new List<string>());
                response.ProcessResponses.Add(UdcTypeDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, productCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadCustomers(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Customer Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var customerNum = Data.UnboxParm.To<decimal>(parms, "CustomerNum", 0);
                response.ProcessResponses.Add(CustomerDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, customerNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadSuppliers(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Supplier Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var supplierNum = Data.UnboxParm.To<decimal>(parms, "SupplierNum", 0);
                response.ProcessResponses.Add(SupplierDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, supplierNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadAddressBook(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Address Book Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var searchTypes = Data.UnboxParm.To(parms, "SearchTypes", new List<string>());
                var addressNum = Data.UnboxParm.To<decimal>(parms, "AddressNum", 0);
                response.ProcessResponses.Add(AddressBookDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, searchTypes, addressNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        //public static ProcessResponse DownloadWorkOrderStatus(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"Work Order Status Download Processor", plant);
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        var workOrderNum = Data.UnboxParm.To<decimal>(parms, "WorkOrderNum", 0);
        //        response.ProcessResponses.Add(WorkOrderStatusDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, workOrderNum));
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Close();
        //    return response;
        //}

        //public static ProcessResponse DownloadRelatedOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns,  Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"Related Order Download Processor", plant);
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        response.ProcessResponses.Add(RelatedOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Close();
        //    return response;
        //}

        //public static ProcessResponse DownloadPurchaseOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"Purchase Order Download Processor", plant);
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        var orderNum = Data.UnboxParm.To<decimal>(parms, "OrderNum", 0);
        //        response.ProcessResponses.Add(PurchaseOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, orderNum));
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Close();
        //    return response;
        //}
    }
}
