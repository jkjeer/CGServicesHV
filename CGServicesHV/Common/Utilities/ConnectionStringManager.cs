using System;
using CGServicesHV.Common.Types.Response;

namespace CGServicesHV.Common.Utilities
{
    public static class ConnectionStringManager
    {
        public static ConnectionStrings AddConnectionString(ConnectionStrings connectionStrings, string name, string value)
        {
            try
            {
                if (connectionStrings == null)
                {
                    connectionStrings = new ConnectionStrings();
                }
                var connectionString = new ConnectionString
                {
                    Name = name,
                    Value = value
                };
                connectionStrings.Add(connectionString);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return connectionStrings;
        }
    }
}
