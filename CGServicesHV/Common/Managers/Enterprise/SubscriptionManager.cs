using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;
using Plant = CGServicesHV.Common.Types.Enterprise.Plant;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class SubscriptionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public SubscriptionManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"SubscriptionManager.GetSubscriptions", GetSubscriptions()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        public static ProcessSteps SetSubscription(SqlConnection cnInt, string plantString, string subscriptionTypeString, bool isActive, bool isEnabled)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Trace($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var steps = new ProcessSteps();
            var step = new ProcessStep($@"Step. Update Plant ({plantString})");
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnInt,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        UPDATE s
                        SET    s.[IS_ACTIVE] = {Convert.ToInt32(isActive)}
                             , s.[IS_ENABLED] = {Convert.ToInt32(isEnabled)}
                        FROM   [INTEGRATION].[SVCCG].[SUBSCRIPTION] s
                        WHERE  s.[PLANT_ALPHA_CD] IN ( {plantString} )
                               AND s.[SUBSCRIPTION_TYPE_CD] IN ( {subscriptionTypeString} ) 
                    "
                };
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace($"ROWS: {rowCount}");
                step.RowsAffected = rowCount;
                steps.Add(step);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Trace($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return steps;
        }

        /******************************************************************
        * Subscriptions
        ******************************************************************/
        public static SqlCommand GetSubscriptionList()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        sub.SUBSCRIPTION_TYPE_CD
                    FROM
                        SVCCG.SUBSCRIPTION sub
                        INNER JOIN SVCCG.SUBSCRIPTION_TYPE st
                        ON  st.SUBSCRIPTION_TYPE_CD = sub.SUBSCRIPTION_TYPE_CD
                        INNER JOIN SVCCG.PLANT pt
                        ON  pt.PLANT_ALPHA_CD = sub.PLANT_ALPHA_CD
                    WHERE
                        sub.IS_ACTIVE = 1 AND
                        st.IS_ACTIVE = 1 AND
                        pt.IS_ACTIVE = 1 AND
                        sub.PLANT_ALPHA_CD = @PLANT_ALPHA_CD"
            };
            cmd.Parameters.Add(new SqlParameter("@PLANT_ALPHA_CD", SqlDbType.VarChar, 12));
            return cmd;
        }

        public static SqlCommand GetMTOScheduleEnvironment()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        SUBSCRIPTION_TYPE_CD
                    FROM
                        [SVCCG].SUBSCRIPTION
                    WHERE
                        IS_ACTIVE = 1 AND
                        SUBSCRIPTION_TYPE_CD in ('MTO_SCHEDULE','FUSE_SCHEDULE','DUAL_MTO_SCHEDULE') AND
                        PLANT_ALPHA_CD = @PLANT_CODE;"
            };
            cmd.Parameters.Add(new SqlParameter("@PLANT_CODE", SqlDbType.VarChar, 12));
            return cmd;
        }

        public static string GetMTOScheduleEnvironment(SqlConnection cnInt, Types.Enterprise.Plant plant)
        {
            var environ = "";
            try
            {
                var cmd = GetMTOScheduleEnvironment();
                cmd.Parameters["@PLANT_CODE"].Value = plant.PlantCode;
                cmd.Connection = cnInt;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        environ = CastDbNull.To(reader["SUBSCRIPTION_TYPE_CD"], string.Empty).Trim();
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnInt.ConnectionString + " JobName: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cnInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return environ;
        }

        public static List<string> GetSubscriptionList(Types.Enterprise.Plant plant)
        {
            var subscriptions = new List<string>();
            SqlConnection cn = new SqlConnection();
            try
            {
                cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cn.Open();
                var cmd = GetSubscriptionList();
                cmd.Parameters["@PLANT_ALPHA_CD"].Value = plant.PlantCode;
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        subscriptions.Add(CastDbNull.To(reader["SUBSCRIPTION_TYPE_CD"], string.Empty).Trim());
                        Logger.Trace(CastDbNull.To(reader["SUBSCRIPTION_TYPE_CD"], string.Empty).Trim());
                    }

                    reader.Close();
                }
                Logger.Trace($@"Rows: {subscriptions.Count}");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + " JobName: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                cn.Close();
            }

            return subscriptions;
        }

        public static List<string> GetSubscriptionList(Types.Enterprise.Plant plant, SqlConnection cn)
        {
            var subscriptions = new List<string>();
            try
            {
                var cmd = GetSubscriptionList();
                cmd.Parameters["@PLANT_ALPHA_CD"].Value = plant.PlantCode;
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        subscriptions.Add(CastDbNull.To(reader["SUBSCRIPTION_TYPE_CD"], string.Empty).Trim());
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + " JobName: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return subscriptions;
        }

        /******************************************************************
         * Subscription Types Cache
         ******************************************************************/
        public static SubscriptionTypes GetSubscriptionTypes(string code)
        {
            var subscriptionTypes = new SubscriptionTypes();
            try
            {
                var subscriptionTypeItemCache = GetSubscriptionTypeCache();
                var subscriptionTypeList = code.Split(',').ToList();
                foreach (var subscriptionTypeItem in subscriptionTypeList)
                {
                    var subscriptionType = subscriptionTypeItemCache.Where(t => t.GroupCode == string.Empty).Where(s => string.Equals(s.SubscriptionType.SubscriptionTypeCode, subscriptionTypeItem, StringComparison.CurrentCultureIgnoreCase)).Select(z => z.SubscriptionType).FirstOrDefault();
                    if (subscriptionType != null)
                    {
                        subscriptionTypes.Add(subscriptionType);
                    }
                    else
                    {
                        var subscriptionTypeItems = subscriptionTypeItemCache.Where(t => string.Equals(t.GroupCode, subscriptionTypeItem, StringComparison.CurrentCultureIgnoreCase)).Select(x => x.SubscriptionType).ToList();
                        subscriptionTypes.AddRange(subscriptionTypeItems);
                    }
                    if (!subscriptionTypes.Any())
                    {
                        throw new Exception($"Error: Subscription Type {code} not found.");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception($"{e.Message}: {e.StackTrace}");
            }
            return subscriptionTypes;
        }

        public static List<string> GetSubscriptionResources(string code)
        {
            var subscriptionResources= new List<string>();
            try
            {
                var subscriptionTypeItemCache = GetSubscriptionTypeCache();
                var subscriptionTypeList = code.Split(',').ToList();
                foreach (var subscriptionTypeItem in subscriptionTypeList)
                {
                    var subscriptionType = subscriptionTypeItemCache.Where(t => t.GroupCode == string.Empty).Where(s => string.Equals(s.SubscriptionType.SubscriptionTypeCode, subscriptionTypeItem, StringComparison.CurrentCultureIgnoreCase)).Select(z => z.SubscriptionType).FirstOrDefault();
                    if (subscriptionType != null)
                    {
                        subscriptionResources = subscriptionType.ResourceList;
                    }
                    else
                    {
                        var subscriptionTypes = subscriptionTypeItemCache.Where(t => string.Equals(t.GroupCode, subscriptionTypeItem, StringComparison.CurrentCultureIgnoreCase)).Select(x => x.SubscriptionType).ToList();
                        foreach (var type in subscriptionTypes)
                        {
                            subscriptionResources.AddRange(type.ResourceList);
                        }
                    }
                    if (!subscriptionResources.Any())
                    {
                        throw new Exception($"Error: Subscription Type {code} has no resources.");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception($"{e.Message}: {e.StackTrace}");
            }
            return subscriptionResources.GroupBy(x => x).Where(g => g.Count() == 1).SelectMany(g => g).ToList(); 
        }

        public static List<SubscriptionTypeItem> GetSubscriptionTypeCache()
        {
            //ObjectCache cache = MemoryCache.Default;
            //if (cache.Contains("SubscriptionTypeItems")) return (List<SubscriptionTypeItem>) cache.Get("SubscriptionTypeItems");
            //else
            //{
                var cn = new SqlConnection();
                List<SubscriptionTypeItem> subscriptionTypeItems;
                try
                {
                    cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                    cn.Open();
                    subscriptionTypeItems = GetSubscriptionTypeItems(cn);
                    //// Store data in the cache
                    //CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                    ////cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
                    //cache.Add("SubscriptionTypeItems", subscriptionTypeItems, cacheItemPolicy);
                    //Logger.Trace("Subscription Type Cache Loaded");
                    //Logger.Trace(XmlSerializationUtility.SerializeObject(Encoding.UTF8, subscriptionTypeItems));
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cn.Close();
                }
                return subscriptionTypeItems;
            //}
        }

        public static SqlCommand GetSubscriptionTypeItems()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        SG.GROUP_CD,
	                    ST.SUBSCRIPTION_TYPE_CD,
                        --ST.NAMESPACE_NAME,
                        --ST.TYPE_NAME,
	                    ST.METHOD_NAME,
                        ST.CLASS_NAME,
                        ST.ARGUMENTS,
                        ST.RESOURCES
                    FROM
                        [SVCCG].SUBSCRIPTION_TYPE_GROUP SG
                        INNER JOIN [SVCCG].SUBSCRIPTION_TYPE_GROUP_XREF SGX
                        ON  SGX.GROUP_CD = SG.GROUP_CD
	                    INNER JOIN [SVCCG].SUBSCRIPTION_TYPE ST
	                    ON ST.SUBSCRIPTION_TYPE_CD = SGX.SUBSCRIPTION_TYPE_CD
					WHERE [ST].[METHOD_NAME] IS NOT NULL
						AND [ST].[CLASS_NAME] IS NOT NULL
                    UNION
                    SELECT
                        '' AS GROUP_CD,
	                    ST.SUBSCRIPTION_TYPE_CD,                        
                        --ST.NAMESPACE_NAME,
                        --ST.TYPE_NAME,
	                    ST.METHOD_NAME,
                        ST.CLASS_NAME,
                        ST.ARGUMENTS,
                        ST.RESOURCES
                    FROM
                        [SVCCG].SUBSCRIPTION_TYPE ST
					WHERE [ST].[METHOD_NAME] IS NOT NULL
						AND [ST].[CLASS_NAME] IS NOT NULL
                "
            };
            return cmd;
        }

        public static SubscriptionTypeItems GetSubscriptionTypeItems(SqlConnection cn)
        {
            var auxResponse = new ProcessResponse($"GetSubscriptionTypeItems");
            var subscriptionTypeItems = new SubscriptionTypeItems();
            try
            {
                var cmd = GetSubscriptionTypeItems();
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var subscriptionTypeItem = new SubscriptionTypeItem();
                        subscriptionTypeItem.GroupCode = CastDbNull.To(reader["GROUP_CD"], string.Empty).Trim();
                        var subscriptionType = new SubscriptionType();
                        subscriptionType.SubscriptionTypeCode = CastDbNull.To(reader["SUBSCRIPTION_TYPE_CD"], string.Empty).Trim();
                        //subscriptionType.NamespaceName = CastDbNull.To(reader["NAMESPACE_NAME"], string.Empty).Trim();
                        //subscriptionType.TypeName = CastDbNull.To(reader["TYPE_NAME"], string.Empty).Trim();
                        subscriptionType.MethodName = CastDbNull.To(reader["METHOD_NAME"], string.Empty).Trim();
                        subscriptionType.ClassName = CastDbNull.To(reader["CLASS_NAME"], string.Empty).Trim();
                        subscriptionType.Arguments = CastDbNull.To(reader["ARGUMENTS"], string.Empty).Trim();
                        subscriptionType.ArgumentsHash = GetArgumentsHash(subscriptionType.Arguments);
                        Logger.Trace($"SubscriptionTypeCode={subscriptionType.SubscriptionTypeCode} ");
                        subscriptionType.DelegateMethod = GetDownloadDelegate(subscriptionType.ClassName, subscriptionType.MethodName);
                        if (subscriptionType.DelegateMethod == null)
                        {
                            if (!subscriptionType.SubscriptionTypeCode.Equals("*ALL*"))
                            {
                                Logger.Trace(subscriptionType.MethodName);
                                //auxResponse.Errors.Add(new Error($"{subscriptionType.SubscriptionTypeCode} - {subscriptionType.ClassName}.{subscriptionType.MethodName} NOT Found in Delegate Lookup"));
                                //var x = new Error($"{subscriptionType.SubscriptionTypeCode} {subscriptionType.MethodName} {subscriptionType.ClassName} NOT Found in Delegate Lookup");
                            }
                        }

                        subscriptionType.Resources = CastDbNull.To(reader["RESOURCES"], string.Empty).Trim();
                        subscriptionType.ResourceList = subscriptionType.Resources.Split(',').Select(p => p.Trim()).ToList();
                        subscriptionTypeItem.SubscriptionType = subscriptionType;
                        subscriptionTypeItems.Add(subscriptionTypeItem);
                    }

                    reader.Close();
                }

                if (auxResponse.HasErrors())
                {
                    SummarizeGetSubscriptionTypeItems(auxResponse);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            return subscriptionTypeItems;
        }

        public static void SummarizeGetSubscriptionTypeItems(ProcessResponse response)
        {
            var stepSummarize = new ProcessStep("Summary Collections");
            try
            {
                //response.SerializedObjects.Add("ResultLine", configs, "ResultLines");

                var headerSummary = new
                {
                    HasErrors = response.HasErrors(),
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    response.EndDate,
                    EndDateString = response.EndDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.EndDate - response.StartDate).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSummarize.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarize.Done();
                response.ProcessSteps.Add(stepSummarize);
            }
        }

        public static SqlCommand GetSubscriptionTypesCmd()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT 
                SUBSCRIPTION_TYPE_CD,
                CLASS_NAME,
                NAMESPACE_NAME,
                TYPE_NAME,
                METHOD_NAME,
                ARGUMENTS,
                RESOURCES
            FROM 
	            [SVCCG].SUBSCRIPTION_TYPE";
            return cmd;
        }

        public static SubscriptionTypes GetSubscriptionTypes(SqlConnection cn)
        {
            var subscriptionTypes = new SubscriptionTypes();
            try
            {
                var cmd = GetSubscriptionTypesCmd();
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var subscriptionType = new SubscriptionType();
                        subscriptionType.SubscriptionTypeCode = CastDbNull.To(reader["SUBSCRIPTION_TYPE_CD"], string.Empty).Trim();
                        subscriptionType.ClassName = CastDbNull.To(reader["CLASS_NAME"], string.Empty).Trim();
                        //subscriptionType.NamespaceName = CastDbNull.To(reader["NAMESPACE_NAME"], string.Empty).Trim();
                        //subscriptionType.TypeName = CastDbNull.To(reader["TYPE_NAME"], string.Empty).Trim();
                        subscriptionType.MethodName = CastDbNull.To(reader["METHOD_NAME"], string.Empty).Trim();
                        subscriptionType.Arguments = CastDbNull.To(reader["ARGUMENTS"], string.Empty).Trim();
                        subscriptionType.Resources = CastDbNull.To(reader["RESOURCES"], string.Empty).Trim();
                        subscriptionType.ResourceList = subscriptionType.Resources.Split(',').Select(p => p.Trim()).ToList();
                        subscriptionTypes.Add(subscriptionType);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }

            return subscriptionTypes;
        }

        public static BusinessSystemTransferProcessor.ProcessDelegate GetDownloadDelegate(string className, string methodName)
        //public static BusinessSystemTransferProcessor.ProcessDelegate GetDownloadDelegate(string namespaceName, string typeName, string methodName)
        {
            try
            {
                Logger.Trace($"GetDelegateDynamic CGServicesHV.{className} {methodName}");
                Type t = Type.GetType($"CGServicesHV.{className}");
                //ReSharper disable once AssignNullToNotNullAttribute
                var d = (BusinessSystemTransferProcessor.ProcessDelegate)Delegate.CreateDelegate(typeof(BusinessSystemTransferProcessor.ProcessDelegate), t, methodName);
                Logger.Trace($"GetDelegateDynamic OK - {d.Method}");
                return d;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Dictionary<string, object> GetArgumentsHash(string argumentString)
        {
            var dict = new Dictionary<string, object>();
            try
            {
                var arguments = argumentString.Split('&').ToList();
                if (arguments.Count > 0 && arguments[0] != string.Empty)
                {
                    foreach (var argument in arguments)
                    {
                        // tokenize name/value
                        var token = argument.Split('=');
                        dict.Add(token[0], token[1]);
                    }
                }
                return dict;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static SubscriptionTypeItems RefreshSubscriptionTypeCache()
        {
            ObjectCache cache = MemoryCache.Default;
            var cn = new SqlConnection();
            SubscriptionTypeItems subscriptionTypeItems;
            try
            {
                cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cn.Open();
                subscriptionTypeItems = GetSubscriptionTypeItems(cn);

                // Remove existing cache
                cache.Remove("SubscriptionTypeItems");

                // Store data in the cache
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cache.Add("SubscriptionTypeItems", subscriptionTypeItems, cacheItemPolicy);
                Logger.Trace(XmlSerializationUtility.SerializeObject(Encoding.UTF8, (SubscriptionTypeItems)cache.Get("SubscriptionTypeItems"), true));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                cn.Close();
            }

            return subscriptionTypeItems;
        }

        public static void SummarizeRunSubscriptionType(ProcessResponse response, string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType)
        {
            var stepSummarize = new ProcessStep("Summary Collections");
            try
            {
                //response.SerializedObjects.Add("ResultLine", configs, "ResultLines");

                var headerSummary = new
                {
                    plants,
                    timeSpan,
                    useHighWaterMark,
                    parallel,
                    subscriptionType,
                    HasErrors = response.HasErrors(),
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    response.EndDate,
                    EndDateString = response.EndDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.EndDate - response.StartDate).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSummarize.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarize.Done();
                response.ProcessSteps.Add(stepSummarize);
            }
        }

        public static void NotifyRunSubscriptionType(ProcessResponse response)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailErrorToAddress"];
                var serializedXml = response.Serialize();
                Logger.Trace($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "CGServicesHV.Common.Xslt.RunSubscriptionType.xslt", Encoding.UTF8);
                Logger.Trace($"Report XML: {xslOutput}");
                var subject = $@"Run Subscription Type [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}]";
                if (response.HasErrors()) subject = $"Error - {subject}";
                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }
    }
}