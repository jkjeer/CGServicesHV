using System;
using System.Collections.Generic;
using System.Linq;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.SubscriptionSvc
{
    public class SubscriptionService : ISubscriptionService
    {
        public Plants GetPlants(string code)
        {
            try
            {
                return SubscriptionProcessor.GetPlants(code);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public PlantGroups GetPlantGroups()
        {
            try
            {
                return SubscriptionProcessor.GetPlantGroups();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public SubscriptionTypes GetSubscriptionTypes(string subscriptionTypes)
        {
            try
            {
                return SubscriptionProcessor.GetSubscriptionTypes(subscriptionTypes);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse SetSubscription(string plants, string subscriptionTypes, bool isActive, bool isEnabled)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(subscriptionTypes))
            {
                throw new Exception("Subscription Type is REQUIRED.");
            }
            try
            {
                return SubscriptionProcessor.SetSubscription(plants, subscriptionTypes, isActive, isEnabled);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        //public ProcessResponse DownloadSubscription(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var subscriptionTypes = SubscriptionManager.GetSubscriptionTypes(subscriptionType);
        //        var resources = SubscriptionManager.GetSubscriptionResources(subscriptionType);
        //        return BusinessSystemTransferProcessor.Invoke(plants, subscriptionTypes, timeSpan, useHighWaterMark, parallel, resources);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}

        public SubscriptionTypeItems RefreshSubscriptionTypeCache()
        {
            try
            {
                return SubscriptionProcessor.RefreshSubscriptionTypeCache();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunSubscriptionType(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var subscriptionTypes = SubscriptionManager.GetSubscriptionTypes(subscriptionType);
                var response = BusinessSystemTransferProcessor.Invoke(plants, subscriptionTypes, timeSpan, useHighWaterMark, parallel);
                response.Close();
                if (response.HasErrors())
                {
                    SubscriptionManager.SummarizeRunSubscriptionType(response, plants, timeSpan, useHighWaterMark, parallel, subscriptionType);
                    //SubscriptionManager.NotifyRunSubscriptionType(response);
                    Logging.InsertProcessLog(response);
                }
                return response;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse InitHighWaterMarks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string markNames)
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
                parms.Add("MarkNames", markNames?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, SubscriptionProcessor.InitHighWaterMarks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public class SubscriptionRestService : ISubscriptionRestService
    {
        public Plants GetPlants(string code)
        {
            try
            {
                return SubscriptionProcessor.GetPlants(code);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public PlantGroups GetPlantGroups()
        {
            try
            {
                return SubscriptionProcessor.GetPlantGroups();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public SubscriptionTypes GetSubscriptionTypes(string subscriptionTypes)
        {
            try
            {
                return SubscriptionProcessor.GetSubscriptionTypes(subscriptionTypes);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse SetSubscription(string plants, string subscriptionTypes, bool isActive, bool isEnabled)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(subscriptionTypes))
            {
                throw new Exception("Subscription Type is REQUIRED.");
            }
            try
            {
                return SubscriptionProcessor.SetSubscription(plants, subscriptionTypes, isActive, isEnabled);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        //public ProcessResponse DownloadSubscription(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var subscriptionTypes = SubscriptionManager.GetSubscriptionTypes(subscriptionType);
        //        var resources = SubscriptionManager.GetSubscriptionResources(subscriptionType);
        //        return BusinessSystemTransferProcessor.Invoke(plants, subscriptionTypes, timeSpan, useHighWaterMark, parallel, resources);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}

        public SubscriptionTypeItems RefreshSubscriptionTypeCache()
        {
            try
            {
                return SubscriptionProcessor.RefreshSubscriptionTypeCache();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunSubscriptionType(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var subscriptionTypes = SubscriptionManager.GetSubscriptionTypes(subscriptionType);
                var response = BusinessSystemTransferProcessor.Invoke(plants, subscriptionTypes, timeSpan, useHighWaterMark, parallel);
                response.Close();
                if (response.HasErrors())
                {
                    SubscriptionManager.SummarizeRunSubscriptionType(response, plants, timeSpan, useHighWaterMark, parallel, subscriptionType);
                    //SubscriptionManager.NotifyRunSubscriptionType(response);
                    Logging.InsertProcessLog(response);
                }
                return response;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse InitHighWaterMarks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string markNames)
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
                parms.Add("MarkNames", markNames?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, SubscriptionProcessor.InitHighWaterMarks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
