using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CGServices.Common.Managers.E1;
using CGServices.Common.Managers.Enterprise;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Types.Response;
using CGServices.Common.Utilities;
using NLog;

namespace CGServices.Common.Managers.Process
{
    public class DownloadProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public delegate ProcessResponse DownloadMethod(Plant plant, BusinessSystemSession session, SqlConnection snSfInt, TimeSpan timeSpan, bool useHighWaterMark, List<string> tokens, Dictionary<string, object> parms);

        /********************************************************************************
        * PARALLEL
        ********************************************************************************/
        public static ProcessResponse DownloadParallel(string plants, SubscriptionTypes subscriptionTypes, TimeSpan timeSpan, bool useHighWaterMark, bool useSubscription, Dictionary<string, object> parms )
        {
            var tokens = new List<string>();
            var parallelResponse = new ProcessResponse("Reference Download Parallel Process");
            var exceptions = new ConcurrentQueue<Exception>();
            try
            {
                var plantItems = PlantManager.GetPlants(plants);
                Parallel.ForEach(plantItems, (plantItem) =>
                {
                    var response = new ProcessResponse($"{plantItem.PlantCode} Download Processor");
                    var session = new BusinessSystemSession();
                    var cnSfInt = new SqlConnection();
                    //var plant = new Plant();
                    try
                    {
                        var step1 = new ProcessStep($"Resolve Plant {plantItem.PlantCode}");
                        try
                        {
                            // resolve Plant
                            response.Plant = plantItem;
                            // Subscriptions
                            if (useSubscription)
                            {
                                tokens = SubscriptionManager.GetSubscriptionList(plantItem);
                            }
                            else
                            {
                                tokens = SubscriptionManager.GetTokens();
                            }
                        }
                        catch (Exception e)
                        {
                            step1.Errors.Add(new Error(e));
                        }
                        step1.Done();
                        response.AddStep(step1);

                        var step2 = new ProcessStep($"Get {plantItem.PlantCode} Database Connections");
                        try
                        {
                            // Biz Session
                            session = BusinessSystemSessionManager.GetBusinessSession();
                            // Shop Floor Integration Connection
                            cnSfInt = ConnectionManager.GetSqlConnectionFromKey(plantItem.PlantCode + ".INTEGRATION");
                            cnSfInt.Open();
                        }
                        catch (Exception e)
                        {
                            step2.Errors.Add(new Error(e));
                        }
                        step2.Done();
                        response.AddStep(step2);
                        if (response.HasErrors())
                        {
                            Logging.InsertProcessLog(response);
                        }

                        if (!response.HasErrors())
                        {
                            foreach (var subscriptionType in subscriptionTypes)
                            {
                                var step3 = new ProcessStep(
                                    $"Call {plantItem.PlantCode} {subscriptionType.DownloadDelegate.Method.Name} Method");
                                try
                                {
                                    step3.ProcessResponses.Add(subscriptionType.DownloadDelegate(plantItem, session,
                                        cnSfInt, timeSpan, useHighWaterMark, tokens, parms));
                                }
                                catch (Exception e)
                                {
                                    step3.Errors.Add(new Error(e));
                                }
                                finally
                                {
                                    step3.Done();
                                    response.AddStep(step3);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        response.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        session.Close();
                        cnSfInt.Close();
                    }
                    response.Done();
                    parallelResponse.ProcessResponses.Add(response);
                });
            }
            catch (Exception e)
            {
                parallelResponse.Errors.Add(new Error(e));
            }
            parallelResponse.Done();
            return parallelResponse;
        }

        /********************************************************************************
        * SYNCHRONOUS
        ********************************************************************************/
        public static ProcessResponse DownloadSynchronous(string plants, SubscriptionTypes subscriptionTypes, TimeSpan timeSpan, bool useHighWaterMark, bool useSubscription, Dictionary<string, object> parms )
        {
            var tokens = new List<string>();
            var synchResponse = new ProcessResponse("Download Synchronous Process");
            try
            {
                var plantItems = PlantManager.GetPlants(plants);
                foreach (var plantItem in plantItems)
                {
                    var response = new ProcessResponse($"{plantItem} Download Processor");
                    var session = new BusinessSystemSession();
                    var cnSfInt = new SqlConnection();
                    var plant = new Plant();
                    try
                    {
                        var step1 = new ProcessStep($"Resolve Plant {plantItem}");
                        try
                        {
                            // resolve Plant
                            response.Plant = plantItem;
                            // Subscriptions
                            if (useSubscription)
                            {
                                tokens = SubscriptionManager.GetSubscriptionList(plantItem);
                            }
                            else
                            {
                                tokens = SubscriptionManager.GetTokens();
                            }
                        }
                        catch (Exception e)
                        {
                            step1.Errors.Add(new Error(e));
                        }
                        step1.Done();
                        response.AddStep(step1);

                        var step2 = new ProcessStep($"Get {plantItem.PlantCode} Database Connections");
                        try
                        {
                            // Biz Session
                            session = BusinessSystemSessionManager.GetBusinessSession();
                            // Shop Floor Integration Connection
                            cnSfInt = ConnectionManager.GetSqlConnectionFromKey(plantItem.PlantCode + ".INTEGRATION");
                            cnSfInt.Open();
                        }
                        catch (Exception e)
                        {
                            step2.Errors.Add(new Error(e));
                        }
                        step2.Done();
                        response.AddStep(step2);
                        if (response.HasErrors())
                        {
                            Logging.InsertProcessLog(response);
                        }

                        if (!response.HasErrors())
                        {
                            foreach (var subscriptionType in subscriptionTypes)
                            {
                                var step3 = new ProcessStep(
                                    $"Call {plantItem.PlantCode} {subscriptionType.DownloadDelegate.Method.Name} Method");
                                try
                                {
                                    step3.ProcessResponses.Add(subscriptionType.DownloadDelegate(plantItem, session,
                                        cnSfInt, timeSpan, useHighWaterMark, tokens, parms));
                                }
                                catch (Exception e)
                                {
                                    step3.Errors.Add(new Error(e));
                                }
                                finally
                                {
                                    step3.Done();
                                    response.AddStep(step3);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        response.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        session.Close();
                        cnSfInt.Close();
                    }
                    response.Done();
                    synchResponse.ProcessResponses.Add(response);
                }
            }
            catch (Exception e)
            {
                synchResponse.Errors.Add(new Error(e));
            }
            synchResponse.Done();
            return synchResponse;
        }
    }
}
