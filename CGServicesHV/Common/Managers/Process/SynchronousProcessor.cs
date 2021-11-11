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
    public class SynchronousProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public delegate ProcessResponse DownloadMethod(Plant plant, BusinessSystemSession session, SqlConnection snSfInt, TimeSpan timeSpan, bool useHighWaterMark, List<string> tokens, Dictionary<string, object> parms);

        public static ProcessResponse Process(string plants, List<DownloadMethod> methods, TimeSpan timeSpan, bool useHighWaterMark, bool useSubscription, Dictionary<string, object> parms, List<string> tokens)
        {
            var parallelResponse = new ProcessResponse("Download Parallel Process");
            try
            {
                var plantList = plants.Split(',').ToList();
                foreach (var plantItem in plantList)
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
                            plant = PlantManager.GetPlant(plantItem);
                            response.Plant = plant;
                            // Subscriptions
                            if (useSubscription)
                            {
                                tokens = SubscriptionManager.GetSubscriptionList(plant);
                            }
                        }
                        catch (Exception e)
                        {
                            step1.Errors.Add(new Error(e));
                        }
                        step1.Done();
                        response.AddStep(step1);

                        var step2 = new ProcessStep($"Get {plantItem} Database Connections");
                        try
                        {
                            // Biz Session
                            session = BusinessSystemSessionManager.GetBusinessSession();
                            // Shop Floor Integration Connection
                            cnSfInt = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
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

                        foreach (var method in methods)
                        {
                            var step3 = new ProcessStep($"Call {plantItem} {method.Method.Name} Method");
                            try
                            {
                                step3.ProcessResponses.Add(method(plant, session, cnSfInt, timeSpan, useHighWaterMark, tokens, parms));
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
                }
            }
            catch (Exception e)
            {
                parallelResponse.Errors.Add(new Error(e));
            }
            parallelResponse.Done();
            return parallelResponse;
        }
    }
}
