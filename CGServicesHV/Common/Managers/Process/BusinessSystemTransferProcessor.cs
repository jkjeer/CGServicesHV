using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using NLog;
// ReSharper disable StringLiteralTypo

namespace CGServicesHV.Common.Managers.Process
{
    public class BusinessSystemTransferProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public delegate ProcessResponse ProcessDelegate(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms);

        public class InvocationObject
        {
            public ProcessDelegate Processor { get; set; }
            public Dictionary<string, object> Parms { get; set; }
            public List<string> Resources { get; set; }

            public string Arguments { get; set; }
            public Dictionary<string, object> ArgumentsHash { get; set; }

            public string Serialize(Plant plant)
            {
                var sb = new StringBuilder();
                sb.Append("===================================");
                sb.Append(Environment.NewLine);
                sb.Append($"Plant: {plant.PlantCode}");
                sb.Append(Environment.NewLine);
                sb.Append($"Processor: {Processor.Method.Name}");
                sb.Append(Environment.NewLine);
                if (Parms != null)
                {
                    foreach (var p in Parms)
                    {
                        sb.Append($"Parm: {p.Key} {p.Value}");
                        sb.Append(Environment.NewLine);
                    }
                }
                foreach (var r in Resources)
                {
                    sb.Append($"Resource: {r}");
                    sb.Append(Environment.NewLine);
                }
                return sb.ToString();
            }
        }

        public static string SerializeInvocations(Plant plant, List<InvocationObject> invocations)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Invocations:");
            sb.Append(Environment.NewLine);
            foreach (var i in invocations)
            {
                sb.Append(i.Serialize(plant));
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /********************************************************************************
        * INVOKE
        ********************************************************************************/
        // Subscriptions
        public static ProcessResponse Invoke(string plants, SubscriptionTypes subscriptionTypes, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            var auxResponse = new ProcessResponse($"Invoke Plants: {plants} Subscription Types: {subscriptionTypes}");
            var dict = new Dictionary<Plant, List<InvocationObject>>();
            // Common Parameters
            var parms = new Dictionary<string, object>
            {
                {"TimeSpan", timeSpan},
                {"HighWaterMark", useHighWaterMark},
                {"Parallel", parallel},
                {"ServiceContext", new ServiceContext()}
            };
            var plantItems = PlantManager.GetPlants(plants);
            foreach (var plant in plantItems)
            {
                var activeList = SubscriptionManager.GetSubscriptionList(plant);
                var invocations = new List<InvocationObject>();
                foreach (var subscriptionType in subscriptionTypes)
                {
                    if (activeList.Contains(subscriptionType.SubscriptionTypeCode))
                    {
                        Logger.Trace($"Adding Subscription {plant.PlantCode} {subscriptionType.SubscriptionTypeCode}");
                        var invocation = new InvocationObject
                        {
                            Processor = subscriptionType.DelegateMethod,
                            Parms = parms,
                            Resources = subscriptionType.ResourceList,
                            Arguments = subscriptionType.Arguments,
                            ArgumentsHash = subscriptionType.ArgumentsHash
                        };
                        if (subscriptionType.Arguments.StartsWith("MarkNames="))
                        {
                            if (!invocation.Parms.Keys.Contains("MarkNames")) invocation.Parms.Add("MarkNames", subscriptionType.Arguments?.Split(',').ToList());
                        }

                        invocations.Add(invocation);
                    }
                    else
                    {
                        Logger.Trace($"WARNING: Plant {plant.PlantCode} Subscription {subscriptionType.SubscriptionTypeCode} is INACTIVE");
                        //auxResponse.Errors.Add(new Error($"WARNING: Plant {plant.PlantCode} Subscription {subscriptionType.SubscriptionTypeCode} is INACTIVE"));
                    }
                }
                if (invocations.Any())
                {
                    Logger.Trace($"Adding {invocations.Count} Invocations for {plant.PlantCode}");
                    dict.Add(plant, invocations);
                }
            }
            var response = parallel ? DownloadParallel(dict) : DownloadSynchronous(dict);
            if (auxResponse.HasErrors())
            {
                response.Errors.AddRange(auxResponse.Errors);
            }
            return response;
        }

        // Direct
        public static ProcessResponse Invoke(string plants, ProcessDelegate method, bool parallel, Dictionary<string, object> parms, List<string> resources)
        {
            var dict = new Dictionary<Plant, List<InvocationObject>>();
            var plantItems = PlantManager.GetPlants(plants);
            foreach (var plant in plantItems)
            {
                var invocations = new List<InvocationObject>();
                var invocation = new InvocationObject();
                invocation.Processor = method;
                invocation.Parms = parms;
                invocation.Resources = resources;
                invocations.Add(invocation);
                if (invocations.Any())
                {
                    Logger.Trace($"Adding {invocations.Count} Invocations for {plant.PlantCode}");
                    Logger.Trace(SerializeInvocations(plant, invocations));
                    dict.Add(plant, invocations);
                }
            }
            return parallel ? DownloadParallel(dict) : DownloadSynchronous(dict);
        }

        /********************************************************************************
        * PARALLEL
        ********************************************************************************/
        public static ProcessResponse DownloadParallel(Dictionary<Plant, List<InvocationObject>> plantJobs)
        {
            var parallelResponse = new ProcessResponse("Parallel Processor", true);
            parallelResponse.IsParallel = true;
            var parallelProcessResponses = new ConcurrentQueue<ProcessResponse>();
            var exceptions = new ConcurrentQueue<Exception>();
            try
            {
                Parallel.ForEach(plantJobs, kvp =>
                {
                    Logger.Trace($"Parallel Processor");
                    var response = new ProcessResponse($"Parallel Processor", kvp.Key);
                    var session = new BusinessSystemSession();
                    var sqlCns = new Dictionary<string, SqlConnection>();
                    try
                    {
                        //var step1 = new ProcessStep($"Resolve Plant {kvp.Key.PlantCode}");
                        //try
                        //{
                        //    response.Plant = kvp.Key;
                        //}
                        //catch (Exception e)
                        //{
                        //    //Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        //    step1.Errors.Add(new Error(e));
                        //}
                        //step1.Done();
                        //response.AddStep(step1);

                        var step2 = new ProcessStep($"Get {kvp.Key.PlantCode} Database Connections");
                        try
                        {
                            // Biz Session
                            if (kvp.Value.Any(x => x.Resources.Contains("E1.PD")))
                            {
                                session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["PrimaryEnvironment"]);
                                if (kvp.Value.Any(x => x.Resources.Contains("E1.HA")))
                                {
                                    //Throw warning for mixed resources
                                }
                            }
                            else
                            {
                                session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["ReplicationEnvironment"]);
                            }
                            // Plant Integration Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.INTEGRATION")))
                            {
                                var cnPlantInt = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".INTEGRATION");
                                cnPlantInt.Open();
                                sqlCns.Add("PLANT.INTEGRATION", cnPlantInt);
                            }
                            // Plant Shop Floor Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.SHOPFLOOR")))
                            {
                                var cnPlantSf = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOOR", false);
                                cnPlantSf.Open();
                                sqlCns.Add("PLANT.SHOPFLOOR", cnPlantSf);
                            }
                            // Plant Shop Floor Scheduling Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.SHOPFLOORSCHEDULING")))
                            {
                                var cnPlantSfSched = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOORSCHEDULING", false);
                                cnPlantSfSched.Open();
                                sqlCns.Add("PLANT.SHOPFLOORSCHEDULING", cnPlantSfSched);
                            }
                            // Plant Clear Path Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.CLEARPATH")))
                            {
                                var cnPlantCp = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".CLEARPATH", false);
                                cnPlantCp.Open();
                                sqlCns.Add("PLANT.CLEARPATH", cnPlantCp);
                            }
                            // Corporate Integration Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("CORP.INTEGRATION")))
                            {
                                var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                                cnCorpInt.Open();
                                sqlCns.Add("CORP.INTEGRATION", cnCorpInt);
                            }
                            // Corporate Data Warehouse Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("CORP.WAREHOUSE")))
                            {
                                var cnCorpDW = ConnectionManager.GetSqlConnectionFromKey("CORP.WAREHOUSE");
                                cnCorpDW.Open();
                                sqlCns.Add("CORP.WAREHOUSE", cnCorpDW);
                            }
                        }
                        catch (Exception e)
                        {
                            step2.Errors.Add(new Error($"Plant: {kvp.Key.PlantCode} Error: {e.Message}"));
                        }
                        step2.Done();
                        response.AddStep(step2);

                        if (!response.HasErrors())
                        {
                            foreach (var invocation in kvp.Value)
                            {
                                var connectionState = true;
                                foreach (var sqlCn in sqlCns)
                                {
                                    if (sqlCn.Value.State != ConnectionState.Open)
                                    {
                                        connectionState = false;
                                        Logger.Trace($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open");
                                        parallelResponse.Errors.Add(new Error($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open"));
                                    }
                                }
                                if (connectionState)
                                {
                                    Logger.Debug($"kvp.Key.PlantCode = {kvp.Key.PlantCode}");
                                    Logger.Debug($"invocation = {invocation.Resources}");
                                    Logger.Debug($"invocation.Processor.Method.Name = {invocation?.Processor?.Method?.Name}");
                                    if (invocation.Processor != null)
                                    {
                                        Logger.Debug($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
                                        var step3 = new ProcessStep($"Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
                                        try
                                        {
                                            step3.ProcessResponses.Add(invocation.Processor(kvp.Key, session, sqlCns, invocation.Parms));
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                                            step3.Errors.Add(new Error(e));
                                        }
                                        finally
                                        {
                                            step3.Done();
                                            response.AddStep(step3);
                                        }
                                    }
                                    else
                                    {
                                        Logger.Error("Invocation.Processor is NULL");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        response.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        session.Close();
                        foreach (var sqlCn in sqlCns)
                        {
                            sqlCn.Value.Close();
                        }
                        response.Close(true);
                    }
                    response.Close();
                    parallelProcessResponses.Enqueue(response);
                });
            }
            catch (AggregateException e)
            {
                foreach (var ex in e.InnerExceptions)
                {
                    parallelResponse.Errors.Add(new Error(ex));
                }
            }

            foreach (var pr in parallelProcessResponses)
            {
                parallelResponse.ProcessResponses.Add(pr);
            }

            parallelResponse.Close();
            return parallelResponse;
        }

        /********************************************************************************
        * SYNCHRONOUS
        ********************************************************************************/
        public static ProcessResponse DownloadSynchronous(Dictionary<Plant, List<InvocationObject>> invocations)
        {
            var synchResponse = new ProcessResponse("Synchronous Processor", true);
            synchResponse.IsParallel = false;
            try
            {
                foreach (var kvp in invocations)
                {
                    Logger.Trace($"Synchronous Processor");
                    var sqlCns = new Dictionary<string, SqlConnection>();
                    var response = new ProcessResponse($"Synchronous Processor", kvp.Key);
                    var session = new BusinessSystemSession();
                    try
                    {
                        //var step1 = new ProcessStep($"Resolve Plant {kvp.Key.PlantCode}");
                        //try
                        //{
                        //    response.Plant = kvp.Key;
                        //}
                        //catch (Exception e)
                        //{
                        //    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        //    step1.Errors.Add(new Error(e));
                        //}
                        //step1.Done();
                        //response.AddStep(step1);

                        var step2 = new ProcessStep($"Get {kvp.Key.PlantCode} Database Connections");
                        try
                        {
                            // Biz Session
                            if (kvp.Value[0].Resources.Contains("E1.HA"))
                            {
                                session = BusinessSystemSessionManager.GetBusinessSession(
                                    ConfigurationManager.AppSettings["ReplicationEnvironment"]);
                            }
                            else
                            {
                                session = BusinessSystemSessionManager.GetBusinessSession(
                                    ConfigurationManager.AppSettings["PrimaryEnvironment"]);
                            }

                            // Plant Integration Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.INTEGRATION")))
                            {
                                var cnPlantInt = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".INTEGRATION");
                                cnPlantInt.Open();
                                sqlCns.Add("PLANT.INTEGRATION", cnPlantInt);
                            }
                            // Plant Shop Floor Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.SHOPFLOOR")))
                            {
                                var cnPlantSf = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOOR", false);
                                cnPlantSf.Open();
                                sqlCns.Add("PLANT.SHOPFLOOR", cnPlantSf);
                            }
                            // Plant Shop Floor Scheduling Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.SHOPFLOORSCHEDULING")))
                            {
                                var cnPlantSfSched = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOORSCHEDULING", false);
                                cnPlantSfSched.Open();
                                sqlCns.Add("PLANT.SHOPFLOORSCHEDULING", cnPlantSfSched);
                            }
                            // Plant Clear Path Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.CLEARPATH")))
                            {
                                var cnPlantCp = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".CLEARPATH", false);
                                cnPlantCp.Open();
                                sqlCns.Add("PLANT.CLEARPATH", cnPlantCp);
                            }
                            // Corporate Integration Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("CORP.INTEGRATION")))
                            {
                                var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                                cnCorpInt.Open();
                                sqlCns.Add("CORP.INTEGRATION", cnCorpInt);
                            }
                            // Corporate Data Warehouse Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("CORP.WAREHOUSE")))
                            {
                                var cnCorpDW = ConnectionManager.GetSqlConnectionFromKey("CORP.WAREHOUSE");
                                cnCorpDW.Open();
                                sqlCns.Add("CORP.WAREHOUSE", cnCorpDW);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                            step2.Errors.Add(new Error($"Plant: {kvp.Key.PlantCode} Error: {e.Message}"));
                        }
                        step2.Done();
                        response.AddStep(step2);
                        if (!response.HasErrors())
                        {
                            foreach (var invocation in kvp.Value)
                            {
                                var connectionState = true;
                                foreach (var sqlCn in sqlCns)
                                {
                                    if (sqlCn.Value.State != ConnectionState.Open)
                                    {
                                        connectionState = false;
                                        Logger.Trace($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open");
                                        synchResponse.Errors.Add(new Error($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open"));
                                    }
                                }
                                if (connectionState)
                                {
                                    Logger.Trace($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
                                    var step3 = new ProcessStep($"Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
                                    try
                                    {
                                        step3.ProcessResponses.Add(invocation.Processor(kvp.Key, session, sqlCns, invocation.Parms));
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
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
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        response.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        session.Close();
                        foreach (var sqlCn in sqlCns)
                        {
                            sqlCn.Value.Close();
                        }
                        response.Close(true);
                    }
                    synchResponse.ProcessResponses.Add(response);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                synchResponse.Errors.Add(new Error(e));
            }
            synchResponse.Close();
            return synchResponse;
        }
    }
}