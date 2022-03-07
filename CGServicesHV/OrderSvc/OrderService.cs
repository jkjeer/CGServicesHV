using System;
using System.Collections.Generic;
using System.Linq;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.OrderSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class OrderService : IOrderService
    {
        public ProcessResponse DownloadSalesLines(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal salesOrderNum = 0, string nextStatusCodes = null)
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
                parms.Add("SalesOrderNum", salesOrderNum);
                parms.Add("StatusCodes", nextStatusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadSalesLines, parallel, parms, resources);
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
                parms.Add("StatusCodes", (!String.IsNullOrEmpty(statusCodes)) ? statusCodes?.Split(',').ToList() : null);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrders, parallel, parms, resources);
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
        public ProcessResponse DownloadSalesLines(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal salesOrderNum = 0, string nextStatusCodes = null)
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
                parms.Add("SalesOrderNum", salesOrderNum);
                parms.Add("StatusCodes", nextStatusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadSalesLines, parallel, parms, resources);
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
