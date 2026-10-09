using System;
using System.Collections.Generic;
using System.Linq;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using SoaCore.Wcf;

namespace CGServicesHV.OrderSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class OrderService : IOrderService
    {
        public ProcessResponse DownloadConfiguredOrders(string plants, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("TimeSpanEnd", timeSpanEnd);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadConfiguredOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadConfiguredAttributes(string plants, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark, bool parallel, decimal configurationId, string rackStatusCodes, bool archived, bool missing)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("TimeSpanEnd", timeSpanEnd);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ConfigurationId", configurationId);
                parms.Add("RackStatusCodes", rackStatusCodes?.Split(',').ToList());
                parms.Add("Archived", archived);
                parms.Add("Missing", missing);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadConfiguredAttributes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadConfiguredUnitAttributes(string plants, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark, bool parallel, decimal configurationId, string rackStatusCodes, bool archived, bool missing)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("TimeSpanEnd", timeSpanEnd);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ConfigurationId", configurationId);
                parms.Add("RackStatusCodes", rackStatusCodes?.Split(',').ToList());
                parms.Add("Archived", archived);
                parms.Add("Missing", missing);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadConfiguredUnitAttributes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadSalesLines(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal salesOrderNum = 0, string nextStatusCodes = null, string orderTypes = null)
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
                parms.Add("WorkOrderNum", salesOrderNum);
                parms.Add("StatusCodes", nextStatusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                parms.Add("OrderTypes", orderTypes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadSalesLines, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadSalesLineMessages(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadSalesLineMessages, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0, string statusCodes = null)
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
                parms.Add("WorkOrderNum", workOrderNum);
                parms.Add("StatusCodes", statusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                //parms.Add("StatusCodes", (!String.IsNullOrEmpty(statusCodes)) ? statusCodes?.Split(',').ToList() : null);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrderParts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum, string startDate, string endDate)
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
                parms.Add("OrderNum", orderNum);
                parms.Add("StartDate", startDate);
                parms.Add("EndDate", endDate);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrderParts, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrderCosts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum, string startDate, string endDate)
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
                parms.Add("OrderNum", orderNum);
                parms.Add("StartDate", startDate);
                parms.Add("EndDate", endDate);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrderCosts, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideWorkOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0, string statusCodes = null)
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
                parms.Add("WorkOrderNum", workOrderNum);
                parms.Add("StatusCodes", statusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                //parms.Add("StatusCodes", (!String.IsNullOrEmpty(statusCodes)) ? statusCodes?.Split(',').ToList() : null);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadOutsideWorkOrders, parallel, parms, resources);
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
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("WorkOrderNum", workOrderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrderStatus, parallel, parms, resources);
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
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadRelatedOrders, parallel, parms, resources);
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
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("OrderNum", orderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadPurchaseOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }


    [ParameterInspectorLogger, CGIErrorHandler]
    public class OrderRestService : IOrderRestService
    {
        public ProcessResponse DownloadConfiguredOrders(string plants, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("TimeSpanEnd", timeSpanEnd);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadConfiguredOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadConfiguredAttributes(string plants, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark, bool parallel, decimal configurationId, string rackStatusCodes, bool archived, bool missing)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("TimeSpanEnd", timeSpanEnd);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ConfigurationId", configurationId);
                parms.Add("RackStatusCodes", rackStatusCodes?.Split(',').ToList());
                parms.Add("Archived", archived);
                parms.Add("Missing", missing);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadConfiguredAttributes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadConfiguredUnitAttributes(string plants, TimeSpan timeSpan, TimeSpan timeSpanEnd, bool useHighWaterMark, bool parallel, decimal configurationId, string rackStatusCodes, bool archived, bool missing)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("TimeSpanEnd", timeSpanEnd);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ConfigurationId", configurationId);
                parms.Add("RackStatusCodes", rackStatusCodes?.Split(',').ToList());
                parms.Add("Archived", archived);
                parms.Add("Missing", missing);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadConfiguredUnitAttributes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadSalesLines(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal salesOrderNum = 0, string nextStatusCodes = null, string orderTypes = null)
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
                parms.Add("WorkOrderNum", salesOrderNum);
                parms.Add("StatusCodes", nextStatusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                parms.Add("OrderTypes", orderTypes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadSalesLines, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadSalesLineMessages(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadSalesLineMessages, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0, string statusCodes = null)
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
                parms.Add("WorkOrderNum", workOrderNum);
                parms.Add("StatusCodes", statusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                //parms.Add("StatusCodes", (!String.IsNullOrEmpty(statusCodes)) ? statusCodes?.Split(',').ToList() : null);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrderParts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum, string startDate, string endDate)
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
                parms.Add("OrderNum", orderNum);
                parms.Add("StartDate", startDate);
                parms.Add("EndDate", endDate);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrderParts, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrderCosts(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum, string startDate, string endDate)
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
                parms.Add("OrderNum", orderNum);
                parms.Add("StartDate", startDate);
                parms.Add("EndDate", endDate);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrderCosts, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideWorkOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0, string statusCodes = null)
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
                parms.Add("WorkOrderNum", workOrderNum);
                parms.Add("StatusCodes", statusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                //parms.Add("StatusCodes", (!String.IsNullOrEmpty(statusCodes)) ? statusCodes?.Split(',').ToList() : null);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadOutsideWorkOrders, parallel, parms, resources);
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
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("WorkOrderNum", workOrderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrderStatus, parallel, parms, resources);
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
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadRelatedOrders, parallel, parms, resources);
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
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("OrderNum", orderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadPurchaseOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

    }
}
