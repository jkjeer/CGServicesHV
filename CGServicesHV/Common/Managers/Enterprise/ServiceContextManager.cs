using System;
using System.ServiceModel;
using CGServicesHV.Common.Types.Response;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class ServiceContextManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ServiceContext GetServiceContext()
        {
            var context = new ServiceContext();
            var securityContext = ServiceSecurityContext.Current;
            var operationContext = OperationContext.Current;
            try
            {
                var isAnonymous = securityContext?.IsAnonymous ?? false;
                context.UserName = (isAnonymous) ? "Anonymous" : securityContext?.PrimaryIdentity?.Name ?? "";
                context.ContractName = operationContext?.EndpointDispatcher?.ContractName ?? "";
                context.HostName = operationContext?.InstanceContext?.Host?.Description.Name ?? "";
                context.Scheme = operationContext?.EndpointDispatcher?.EndpointAddress.Uri.Scheme ?? "";
                context.Port = operationContext?.EndpointDispatcher?.EndpointAddress.Uri.Port ?? 0;
                context.ServerAddress = operationContext?.Channel?.LocalAddress?.Uri.ToString() ?? "";
                context.SessionId = operationContext?.SessionId ?? "";
                context.TimeStamp = DateTime.Now;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(e.Message + ": " + e.StackTrace);
            }
            return context;
        }

    }
}