using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Managers.ShopFloor;
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
            var response = new ProcessResponse($"{plant.PlantCode} Sales Line Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var salesOrderNum = Data.UnboxParm.To<decimal>(parms, "SalesOrderNum", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                response.ProcessResponses.Add(SalesLineDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, salesOrderNum, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
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
    }
}
