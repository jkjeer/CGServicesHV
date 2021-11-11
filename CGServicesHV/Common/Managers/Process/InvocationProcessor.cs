using System.Collections.Generic;
using System.Data.SqlClient;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Types.Response;

namespace CGServices.Common.Managers.Process
{
    public class InvocationProcessor
    {
        //public delegate ProcessResponse ProcessDelegate(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms);

        //public class InvocationObject
        //{
        //    public ProcessDelegate Processor { get; set; }
        //    public Dictionary<string, object> Parms { get; set; }
        //}

        /********************************************************************************
        * INVOKE
        ********************************************************************************/
        // Subscriptions
        //public static ProcessResponse Invoke(string plants, SubscriptionTypes subscriptionTypes, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        //{
        //    var dict = new Dictionary<Plant, List<InvocationObject>>();
        //    // Common Parameters
        //    var parms = new Dictionary<string, object>();
        //    parms.Add("TimeSpan", timeSpan);
        //    parms.Add("HighWaterMark", useHighWaterMark);
        //    parms.Add("Parallel", parallel);
        //    parms.Add("ServiceContext", new ServiceContext());
        //    var plantItems = PlantManager.GetPlants(plants);
        //    foreach (var plant in plantItems)
        //    {
        //        var activeList = SubscriptionManager.GetSubscriptionList(plant);
        //        var invocations = new List<InvocationObject>();
        //        foreach (var subscriptionType in subscriptionTypes)
        //        {
        //            if (activeList.Contains(subscriptionType.SubscriptionTypeCode))
        //            {
        //                Logger.Info($"Adding Subscription {plant.PlantCode} {subscriptionType.SubscriptionTypeCode}");
        //                var invocation = new InvocationObject();
        //                invocation.Processor = subscriptionType.DelegateMethod;
        //                invocation.Parms = parms;
        //                invocations.Add(invocation);
        //            }
        //            else
        //            {
        //                Logger.Info($"WARNING: Plant {plant.PlantCode} Subscription {subscriptionType.SubscriptionTypeCode} is INACTIVE");
        //                //throw new Exception($"WARNING: Plant {plant.PlantCode} Subscription {subscriptionType.SubscriptionTypeCode} is INACTIVE");
        //            }
        //        }
        //        if (invocations.Any())
        //        {
        //            Logger.Info($"Adding {invocations.Count} Invocations for {plant.PlantCode}");
        //            dict.Add(plant, invocations);
        //        }
        //    }
        //    return parallel ? DownloadParallel(dict) : DownloadSynchronous(dict);
        //}

        // Direct
        //public static ProcessResponse Invoke(string plants, ProcessDelegate method, bool parallel, Dictionary<string, object> parms)
        //{
        //    var dict = new Dictionary<Plant, List<InvocationObject>>();
        //    var plantItems = PlantManager.GetPlants(plants);
        //    foreach (var plant in plantItems)
        //    {
        //        var invocations = new List<InvocationObject>();
        //        var invocation = new InvocationObject();
        //        invocation.Processor = method;
        //        invocation.Parms = parms;
        //        invocations.Add(invocation);
        //        if (invocations.Any())
        //        {
        //            Logger.Info($"Adding {invocations.Count} Invocations for {plant.PlantCode}");
        //            dict.Add(plant, invocations);
        //        }
        //    }
        //    return parallel ? DownloadParallel(dict) : DownloadSynchronous(dict);
        //}

        /********************************************************************************
        * PARALLEL
        ********************************************************************************/
        //public static ProcessResponse DownloadParallel(Dictionary<Plant, List<InvocationObject>> plantJobs)
        //{
        //    var parallelResponse = new ProcessResponse("Parallel Processor");
        //    var exceptions = new ConcurrentQueue<Exception>();
        //    try
        //    {
        //        Parallel.ForEach(plantJobs, kvp =>
        //        {
        //            var method = $"{kvp.Value.First().Processor.Method.DeclaringType?.FullName}.{kvp.Value.First().Processor.Method.Name}";
        //            Logger.Info($"{kvp.Key.PlantCode} {method} Parallel Processor");
        //            var response = new ProcessResponse($"{kvp.Key.PlantCode} {method} Parallel Processor");
        //            response.IsParallel = true;
        //            var session = new BusinessSystemSession();
        //            var sqlCns = new Dictionary<string, SqlConnection>();
        //            try
        //            {
        //                var step1 = new ProcessStep($"Resolve Plant {kvp.Key.PlantCode}");
        //                try
        //                {
        //                    response.Plant = kvp.Key;
        //                }
        //                catch (Exception e)
        //                {
        //                    step1.Errors.Add(new Error(e));
        //                }
        //                step1.Done();
        //                response.AddStep(step1);
        //                var step2 = new ProcessStep($"Get {kvp.Key.PlantCode} Database Connections");
        //                try
        //                {
        //                    // Biz Session
        //                    session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["PrimaryEnvironment"]);
        //                    // Plant Integration Connection
        //                    var cnPlantInt = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".INTEGRATION");
        //                    cnPlantInt.Open();
        //                    sqlCns.Add("INTEGRATION", cnPlantInt);
        //                    // Plant Shop Floor Connection
        //                    var cnPlantSf = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOOR", false);
        //                    cnPlantSf.Open();
        //                    sqlCns.Add("SHOPFLOOR", cnPlantSf);
        //                    // Plant Clear Path Connection
        //                    var cnPlantCp = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".CLEARPATH", false);
        //                    cnPlantCp.Open();
        //                    sqlCns.Add("CLEARPATH", cnPlantCp);
        //                    // Corporate Integration Connection
        //                    var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
        //                    cnCorpInt.Open();
        //                    sqlCns.Add("CORP.INTEGRATION", cnCorpInt);
        //                }
        //                catch (Exception e)
        //                {
        //                    step2.Errors.Add(new Error($"{method} Plant: {kvp.Key.PlantCode} Error: {e.Message}"));
        //                }
        //                step2.Done();
        //                response.AddStep(step2);
        //                if (response.HasErrors())
        //                {
        //                    Logging.InsertProcessLog(response);
        //                }
        //                if (!response.HasErrors())
        //                {
        //                    foreach (var invocation in kvp.Value)
        //                    {
        //                        var connectionState = true;
        //                        foreach (var sqlCn in sqlCns)
        //                        {
        //                            //if (sqlCn.Key != "CLEARPATH" && sqlCn.Value.State != ConnectionState.Open)
        //                            if (sqlCn.Value.State != ConnectionState.Open)
        //                            {
        //                                connectionState = false;
        //                                Logger.Info($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open");
        //                                parallelResponse.Errors.Add(new Error($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open"));
        //                            }
        //                        }
        //                        if (connectionState)
        //                        {
        //                            Logger.Info($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
        //                            var step3 = new ProcessStep($"Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
        //                            try
        //                            {
        //                                step3.ProcessResponses.Add(invocation.Processor(kvp.Key, session, sqlCns, invocation.Parms));
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                step3.Errors.Add(new Error($"{method} Plant {kvp.Key.PlantCode}  Error: {e.Message}"));
        //                            }
        //                            finally
        //                            {
        //                                step3.Done();
        //                                response.AddStep(step3);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                response.Errors.Add(new Error($"{method} Plant {kvp.Key.PlantCode}  Error: {e.Message}"));
        //            }
        //            finally
        //            {
        //                session.Close();
        //                foreach (var sqlCn in sqlCns)
        //                {
        //                    sqlCn.Value.Close();
        //                }
        //            }
        //            response.Done();
        //            parallelResponse.ProcessResponses.Add(response);
        //        });
        //    }
        //    catch (AggregateException e)
        //    {
        //        foreach (var ex in e.InnerExceptions)
        //        {
        //            parallelResponse.Errors.Add(new Error(ex));
        //        }
        //    }
        //    parallelResponse.Done();
        //    return parallelResponse;
        //}

        /********************************************************************************
        * SYNCHRONOUS
        ********************************************************************************/
        //public static ProcessResponse DownloadSynchronous(Dictionary<Plant, List<InvocationObject>> invocations)
        //{
        //    //var tokens = new List<string>();
        //    var synchResponse = new ProcessResponse("Synchronous Processor");
        //    try
        //    {
        //        foreach (var kvp in invocations)
        //        {
        //            var method = $"{kvp.Value.First().Processor.Method.DeclaringType.FullName}.{kvp.Value.First().Processor.Method.Name}";
        //            Logger.Info($"{kvp.Key.PlantCode} {method} Synchronous Processor");
        //            var response = new ProcessResponse($"{kvp.Key.PlantCode} {method} Synchronous Processor");
        //            var session = new BusinessSystemSession();
        //            var sqlCns = new Dictionary<string, SqlConnection>();
        //            try
        //            {
        //                var step1 = new ProcessStep($"Resolve Plant {kvp.Key.PlantCode}");
        //                try
        //                {
        //                    response.Plant = kvp.Key;
        //                }
        //                catch (Exception e)
        //                {
        //                    step1.Errors.Add(new Error($"{method} Plant {kvp.Key.PlantCode}  Error: {e.Message}"));
        //                }
        //                step1.Done();
        //                response.AddStep(step1);
        //                var step2 = new ProcessStep($"Get {kvp.Key.PlantCode} Database Connections");
        //                try
        //                {
        //                    // Biz Session
        //                    session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["PrimaryEnvironment"]);
        //                    // Plant Integration Connection
        //                    var cnPlantInt = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".INTEGRATION");
        //                    cnPlantInt.Open();
        //                    sqlCns.Add("INTEGRATION", cnPlantInt);
        //                    // Plant Shop Floor Connection
        //                    var cnPlantSf = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOOR");
        //                    cnPlantSf.Open();
        //                    sqlCns.Add("SHOPFLOOR", cnPlantSf);
        //                    // Plant Clear Path Connection
        //                    var cnPlantCp = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".CLEARPATH");
        //                    cnPlantCp.Open();
        //                    sqlCns.Add("CLEARPATH", cnPlantCp);
        //                    // Corporate Integration Connection
        //                    var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
        //                    cnCorpInt.Open();
        //                    sqlCns.Add("CORP.INTEGRATION", cnCorpInt);
        //                }
        //                catch (Exception e)
        //                {
        //                    step2.Errors.Add(new Error($"{method} Plant: {kvp.Key.PlantCode} Error: {e.Message}"));
        //                }
        //                step2.Done();
        //                response.AddStep(step2);
        //                if (response.HasErrors())
        //                {
        //                    Logging.InsertProcessLog(response);
        //                }
        //                if (!response.HasErrors())
        //                {
        //                    foreach (var invocation in kvp.Value)
        //                    {
        //                        var connectionState = true;
        //                        foreach (var sqlCn in sqlCns)
        //                        {
        //                            //if (sqlCn.Key != "CLEARPATH" && sqlCn.Value.State != ConnectionState.Open)
        //                            if (sqlCn.Value.State != ConnectionState.Open)
        //                            {
        //                                connectionState = false;
        //                                Logger.Info($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open");
        //                                synchResponse.Errors.Add(new Error($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open"));
        //                            }
        //                        }
        //                        if (connectionState)
        //                        {
        //                            Logger.Info($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
        //                            var step3 = new ProcessStep($"Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
        //                            try
        //                            {
        //                                step3.ProcessResponses.Add(invocation.Processor(kvp.Key, session, sqlCns, invocation.Parms));
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                step3.Errors.Add(new Error($"{method} Plant {kvp.Key.PlantCode}  Error: {e.Message}"));
        //                            }
        //                            finally
        //                            {
        //                                step3.Done();
        //                                response.AddStep(step3);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                response.Errors.Add(new Error($"{method} Plant {kvp.Key.PlantCode}  Error: {e.Message}"));
        //            }
        //            finally
        //            {
        //                session.Close();
        //                foreach (var sqlCn in sqlCns)
        //                {
        //                    sqlCn.Value.Close();
        //                }
        //            }
        //            response.Done();
        //            synchResponse.ProcessResponses.Add(response);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        synchResponse.Errors.Add(new Error(e));
        //    }
        //    synchResponse.Done();
        //    return synchResponse;
        //}

        /********************************************************************************
        * UPLOAD
        ********************************************************************************/
        // Direct
        //public static ProcessResponse InvokeUpload(string plants, ProcessDelegate method, bool parallel, Dictionary<string, object> parms)
        //{
        //    var dict = new Dictionary<Plant, List<InvocationObject>>();
        //    var plantItems = PlantManager.GetPlants(plants);
        //    foreach (var plant in plantItems)
        //    {
        //        var invocations = new List<InvocationObject>();
        //        var invocation = new InvocationObject();
        //        invocation.Processor = method;
        //        invocation.Parms = parms;
        //        invocations.Add(invocation);
        //        if (invocations.Any())
        //        {
        //            dict.Add(plant, invocations);
        //        }
        //    }
        //    return UploadSynchronous(dict);
        //}

        //public static ProcessResponse UploadSynchronous(Dictionary<Plant, List<InvocationObject>> invocations)
        //{
        //    //var tokens = new List<string>();
        //    var synchResponse = new ProcessResponse("Synchronous Processor");
        //    try
        //    {
        //        foreach (var kvp in invocations)
        //        {
        //            Logger.Info($"{kvp.Key.PlantCode} Synchronous Processor");
        //            var sqlCns = new Dictionary<string, SqlConnection>();
        //            var response = new ProcessResponse($"{kvp.Key} Synchronous Processor");
        //            var session = new BusinessSystemSession();
        //            //var cnSfInt = new SqlConnection();
        //            //var plant = new Plant();
        //            try
        //            {
        //                var step1 = new ProcessStep($"Resolve Plant {kvp.Key.PlantCode}");
        //                try
        //                {
        //                    response.Plant = kvp.Key;
        //                }
        //                catch (Exception e)
        //                {
        //                    step1.Errors.Add(new Error(e));
        //                }
        //                step1.Done();
        //                response.AddStep(step1);
        //                var step2 = new ProcessStep($"Get {kvp.Key.PlantCode} Database Connections");
        //                try
        //                {
        //                    // Biz Session
        //                    session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["PrimaryEnvironment"]);
        //                    // Plant Integration Connection
        //                    var cnPlantInt = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".INTEGRATION");
        //                    cnPlantInt.Open();
        //                    sqlCns.Add("INTEGRATION", cnPlantInt);
        //                    // Plant Shop Floor Connection
        //                    var cnPlantSf = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOOR");
        //                    cnPlantSf.Open();
        //                    sqlCns.Add("SHOPFLOOR", cnPlantSf);
        //                    // Plant Clear Path Connection
        //                    var cnPlantCp = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".CLEARPATH");
        //                    //cnPlantCp.Open();
        //                    sqlCns.Add("CLEARPATH", cnPlantCp);
        //                    // Corporate Integration Connection
        //                    var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
        //                    cnCorpInt.Open();
        //                    sqlCns.Add("CORP.INTEGRATION", cnCorpInt);
        //                }
        //                catch (Exception e)
        //                {
        //                    step2.Errors.Add(new Error(e));
        //                }
        //                step2.Done();
        //                response.AddStep(step2);
        //                if (response.HasErrors())
        //                {
        //                    Logging.InsertProcessLog(response);
        //                }
        //                if (!response.HasErrors())
        //                {
        //                    foreach (var invocation in kvp.Value)
        //                    {
        //                        Logger.Info($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
        //                        var step3 = new ProcessStep($"Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
        //                        try
        //                        {
        //                            step3.ProcessResponses.Add(invocation.Processor(kvp.Key, session, sqlCns, invocation.Parms));
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            step3.Errors.Add(new Error(e));
        //                        }
        //                        finally
        //                        {
        //                            step3.Done();
        //                            response.AddStep(step3);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                response.Errors.Add(new Error(e));
        //            }
        //            finally
        //            {
        //                session.Close();
        //                foreach (var sqlCn in sqlCns)
        //                {
        //                    sqlCn.Value.Close();
        //                }
        //            }
        //            response.Done();
        //            synchResponse.ProcessResponses.Add(response);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        synchResponse.Errors.Add(new Error(e));
        //    }
        //    synchResponse.Done();
        //    return synchResponse;
        //}

        /********************************************************************************
        * DO PLANT
        ********************************************************************************/
        //public static ProcessResponse DoPlant(Plant plant, List<InvocationObject> invocations)
        //{
        //    var response = new ProcessResponse($"{plant.PlantCode} Synchronous Processor");
        //    var sqlCns = new Dictionary<string, SqlConnection>();
        //    var session = new BusinessSystemSession();
        //    try
        //    {
        //        var step1 = new ProcessStep($"Resolve Plant {plant.PlantCode}");
        //        try
        //        {
        //            response.Plant = plant;
        //        }
        //        catch (Exception e)
        //        {
        //            step1.Errors.Add(new Error(e));
        //        }
        //        step1.Done();
        //        response.AddStep(step1);
        //        var step2 = new ProcessStep($"Get {plant.PlantCode} Database Connections");
        //        try
        //        {
        //            // Biz Session
        //            session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["PrimaryEnvironment"]);
        //            // Plant Integration Connection
        //            var cnPlantInt = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
        //            cnPlantInt.Open();
        //            sqlCns.Add("INTEGRATION", cnPlantInt);
        //            // Plant Shop Floor Connection
        //            var cnPlantSf = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
        //            cnPlantSf.Open();
        //            sqlCns.Add("SHOPFLOOR", cnPlantSf);
        //            // Plant Clear Path Connection
        //            var cnPlantCp = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".CLEARPATH");
        //            //cnPlantCp.Open();
        //            sqlCns.Add("CLEARPATH", cnPlantCp);
        //            // Corporate Integration Connection
        //            var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
        //            cnCorpInt.Open();
        //            sqlCns.Add("CORP.INTEGRATION", cnCorpInt);
        //        }
        //        catch (Exception e)
        //        {
        //            step2.Errors.Add(new Error(e));
        //        }
        //        step2.Done();
        //        response.AddStep(step2);
        //        if (response.HasErrors())
        //        {
        //            Logging.InsertProcessLog(response);
        //        }
        //        if (!response.HasErrors())
        //        {
        //            foreach (var invocation in invocations)
        //            {
        //                Logger.Info($"Synchronous Call {plant.PlantCode} {invocation.Processor.Method.Name} Processor");
        //                var step3 = new ProcessStep($"Call {plant.PlantCode} {invocation.Processor.Method.Name} Processor");
        //                try
        //                {
        //                    step3.ProcessResponses.Add(invocation.Processor(plant, session, sqlCns, invocation.Parms));
        //                }
        //                catch (Exception e)
        //                {
        //                    step3.Errors.Add(new Error(e));
        //                }
        //                finally
        //                {
        //                    step3.Done();
        //                    response.AddStep(step3);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    finally
        //    {
        //        session.Close();
        //        foreach (var sqlCn in sqlCns)
        //        {
        //            sqlCn.Value.Close();
        //        }
        //    }
        //    response.Done();
        //    return response;
        //}
    }
}