using System;
using System.Collections.Generic;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using SoaCore.Wcf;

namespace CGServicesHV.ShippingSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class ShippingService : IShippingService
    {
        public ProcessResponse DownloadShipmentPlanning(string plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            if (string.IsNullOrEmpty(plant))
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
                return BusinessSystemTransferProcessor.Invoke(plant, ShippingProcessor.DownloadShipmentPlanning, true, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadShipmentSummary(string plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            if (string.IsNullOrEmpty(plant))
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
                return BusinessSystemTransferProcessor.Invoke(plant, ShippingProcessor.DownloadShipmentSummary, true, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadLoadHeader(string plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            if (string.IsNullOrEmpty(plant))
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
                return BusinessSystemTransferProcessor.Invoke(plant, ShippingProcessor.DownloadLoadHeader, true, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class ShippingRestService : IShippingRestService
    {
        public ProcessResponse DownloadShipmentPlanning(string plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            if (string.IsNullOrEmpty(plant))
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
                return BusinessSystemTransferProcessor.Invoke(plant, ShippingProcessor.DownloadShipmentPlanning, true, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadShipmentSummary(string plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            if (string.IsNullOrEmpty(plant))
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
                return BusinessSystemTransferProcessor.Invoke(plant, ShippingProcessor.DownloadShipmentSummary, true, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadLoadHeader(string plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            if (string.IsNullOrEmpty(plant))
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
                return BusinessSystemTransferProcessor.Invoke(plant, ShippingProcessor.DownloadLoadHeader, true, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }

}
