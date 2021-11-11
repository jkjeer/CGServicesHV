using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Response;

namespace CGServicesHV.ItemSvc
{
    public class MachinePartProcessor
    {
        public static ProcessResponse ConsumptionUpload(string plantCode, string status)
        {
            var response = new ProcessResponse("Consumption Upload");
            var session = new BusinessSystemSession();
            var cn = new SqlConnection();
            var cnInt = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
                cn.Open();

                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();

                // Process
                var mgr = new MachinePartUploadManager(session);
                //response = mgr.Process(session, cn, cnInt, plant, status);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
                cn.Close();
                cnInt.Close();
            }
            response.Close();
            return response;
        }
    }
}
