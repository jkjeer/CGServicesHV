using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.OrderSvc
{
    public class OrderProcessor
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse DownloadConfiguredOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Work Order Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var timeSpanEnd = Data.UnboxParm.To(parms, "TimeSpanEnd", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(ConfiguredOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, timeSpanEnd, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadConfiguredAttributes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Configured Attribute Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var timeSpanEnd = Data.UnboxParm.To(parms, "TimeSpanEnd", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var configurationId = Data.UnboxParm.To<decimal>(parms, "ConfigurationId", 0);
                var rackStatusCodes = Data.UnboxParm.To(parms, "RackStatusCodes", new List<string>());
                var archived = Data.UnboxParm.To<bool>(parms, "Archived", false);
                var missing = Data.UnboxParm.To<bool>(parms, "Missing", false);
                response.ProcessResponses.Add(ConfiguredAttributeDownloadManager.Process(session, sqlCns, plant, timeSpan, timeSpanEnd, useHighWaterMark, configurationId, rackStatusCodes, archived, missing));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadConfiguredUnitAttributes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Configured Unit Attribute Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var timeSpanEnd = Data.UnboxParm.To(parms, "TimeSpanEnd", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var configurationId = Data.UnboxParm.To<decimal>(parms, "ConfigurationId", 0);
                var rackStatusCodes = Data.UnboxParm.To(parms, "RackStatusCodes", new List<string>());
                var archived = Data.UnboxParm.To<bool>(parms, "Archived", false);
                var missing = Data.UnboxParm.To<bool>(parms, "Missing", false);
                response.ProcessResponses.Add(ConfiguredUnitAttributeDownloadManager.Process(session, sqlCns, plant, timeSpan, timeSpanEnd, useHighWaterMark, configurationId, rackStatusCodes, archived, missing));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadPurchaseOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Purchase Order Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var orderNum = Data.UnboxParm.To<decimal>(parms, "OrderNum", 0);
                response = PurchaseOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, orderNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadSalesLines(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Sales Line Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var workOrderNum = Data.UnboxParm.To<decimal>(parms, "SalesOrderNum", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var orderTypes = Data.UnboxParm.To(parms, "OrderTypes", new List<string>());
                response.ProcessResponses.Add(SalesLineDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, workOrderNum, statusCodes, orderTypes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadSalesLineMessages(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Sales Line Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(SalesLineMessageDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadWorkOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Work Order Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var workOrderNum = Data.UnboxParm.To<decimal>(parms, "WorkOrderNum", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                response.ProcessResponses.Add(WorkOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, workOrderNum, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadWorkOrderParts(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Work Order Part Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var orderNum = Data.UnboxParm.To<decimal>(parms, "OrderNum", 0);
                var startDate = Data.UnboxParm.To<string>(parms, "StartDate", String.Empty);
                var endDate = Data.UnboxParm.To<string>(parms, "EndDate", String.Empty);
                var mgr = new WorkOrderPartDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, orderNum, startDate, endDate));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadWorkOrderCosts(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Work Order Cost Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var orderNum = Data.UnboxParm.To<decimal>(parms, "OrderNum", 0);
                var startDate = Data.UnboxParm.To<string>(parms, "StartDate", String.Empty);
                var endDate = Data.UnboxParm.To<string>(parms, "EndDate", String.Empty);
                var mgr = new WorkOrderCostDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, orderNum, startDate, endDate));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadOutsideWorkOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Work Order Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var workOrderNum = Data.UnboxParm.To<decimal>(parms, "WorkOrderNum", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                response.ProcessResponses.Add(OutsideWorkOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, workOrderNum, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadWorkOrderStatus(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Work Order Status Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var workOrderNum = Data.UnboxParm.To<decimal>(parms, "WorkOrderNum", 0);
                response.ProcessResponses.Add(WorkOrderStatusDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, workOrderNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadRelatedOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Related Order Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(RelatedOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }
    }
}
