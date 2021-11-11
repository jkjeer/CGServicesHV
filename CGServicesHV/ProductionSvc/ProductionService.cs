using System;
using System.Collections.Generic;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.ProductionSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class ProductionService : IProductionService
    {

        public ProcessResponse ProductionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.CLEARPATH", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.ProductionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse ConsumedStoceUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.ConsumedStoceUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse CustPackUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.CLEARPATH", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.CustPackUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse HvCompletionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvCompletionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse HvReceiptUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvReceiptUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse HvTrimUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvTrimUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse HvIssueUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvIssueUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse HvWoIssueUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvWoIssueUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse WoMgmtUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.WoMgmtUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RackTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.RackTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse RackAssetTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.RackAssetTransactionUpload, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ProductionProcessor.RackAssetTransactionUpload, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class ProductionRestService : IProductionRestService
    {

        public ProcessResponse ProductionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.CLEARPATH", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.ProductionUpload, parallel, parms, resources);
                //return ProductionProcessor.ProductionUpload(plant, status);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse ConsumedStoceUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.ConsumedStoceUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse CustPackUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.CustPackUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse HvCompletionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvCompletionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse HvReceiptUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvReceiptUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse HvTrimUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvTrimUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse HvIssueUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvIssueUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse HvWoIssueUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvWoIssueUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }


        public ProcessResponse WoMgmtUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.WoMgmtUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RackTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.RackTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse RackAssetTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode)
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
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.RackAssetTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}
