using System;
using System.Collections.Generic;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using IBM.Data.DB2.iSeries;

namespace CGServicesHV.Common.Managers.E1
{
    public class SessionManager
    {
        public static BusinessSystemSession GetSession()
        {
            var session = new BusinessSystemSession();
            try
            {
                var dbConnectionString = ConnectionManager.GetDb2ConnectionString();
                session.DbConnection = ConnectionManager.GetDb2Connection(dbConnectionString);
                session.DbCommands = new Dictionary<string, iDB2Command>();
                session.DbConnection.Open();
                session.IsOpen = true;
            }
            catch (Exception e)
            {
                throw new ApplicationException(session.DbConnection.ConnectionString + ": " + e.Message + ": " + e.StackTrace);
            }
            return session;
        }
    }
}
