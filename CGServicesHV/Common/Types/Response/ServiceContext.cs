using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using NLog;

namespace CGServicesHV.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class ServiceContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ServiceContext()
        {
            var operationContext = OperationContext.Current;
            var securityContext = ServiceSecurityContext.Current;
            try
            {
                UserName = securityContext == null ? "NULL" : ( securityContext.IsAnonymous ? "Anonymous" : securityContext.PrimaryIdentity.Name );
                if (operationContext != null)
                {
                    ContractName = operationContext.EndpointDispatcher.ContractName;
                    HostName = operationContext.InstanceContext.Host.Description.Name;
                    Scheme = operationContext.EndpointDispatcher.EndpointAddress.Uri.Scheme;
                    Port = operationContext.EndpointDispatcher.EndpointAddress.Uri.Port;
                    ServerAddress = operationContext.Channel.LocalAddress.Uri.ToString();
                    SessionId = operationContext.SessionId;
                }
                else
                {
                    ContractName = "NULL";
                    HostName = "NULL";
                    Scheme = "NULL";
                    Port = 0;
                    ServerAddress = "NULL";
                    SessionId = "NULL";
                }
                TimeStamp = DateTime.Now;
                Logger.Info("UserName: " + UserName);
                Logger.Info("SessionId: " + SessionId);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e.Message + ": " + e.StackTrace);
            }
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string UserName { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string ContractName { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string QueryString { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string HostName { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string Scheme { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public int Port { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public DateTime TimeStamp { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string ServerAddress { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string SessionId { get; set; }
    }
}
