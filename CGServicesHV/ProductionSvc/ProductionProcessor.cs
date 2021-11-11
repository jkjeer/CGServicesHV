using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.ProductionSvc
{
    public class ProductionProcessor
    {
        public static ProcessResponse ProductionUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse("Production Upload", plant);
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);

                var mgr1 = new ConsumedStoceUploadManager(session);
                var response1 = mgr1.Process(session, sqlCns, plant, statusCode);
                response.ProcessResponses.Add(response1);

                var mgr2 = new CustPackUploadManager(session);
                var response2 = mgr2.Process(session, sqlCns, plant, statusCode);
                response.ProcessResponses.Add(response2);

                var mgr3 = new HvCompletionUploadManager(session);
                var response3 = mgr3.Process(session, sqlCns, plant, statusCode);
                response.ProcessResponses.Add(response3);

                var mgr4 = new HvReceiptUploadManager(session);
                var response4 = mgr4.Process(session, sqlCns, plant, statusCode);
                response.ProcessResponses.Add(response4);

                //var mgr5 = new HvTrimUploadManager(session);
                //var response5 = mgr5.Process(session, sqlCns, plant, statusCode);
                //response.ProcessResponses.Add(response5);

                var mgr6 = new HvIssueUploadManager(session);
                var response6 = mgr6.Process(session, sqlCns, plant, statusCode);
                response.ProcessResponses.Add(response6);

                var mgr7 = new HvWoIssueUploadManager(session);
                var response7 = mgr7.Process(session, sqlCns, plant, statusCode);
                response.ProcessResponses.Add(response7);

                //var mgr8 = new WoMgmtUploadManager(session);
                //var response8 = mgr8.Process(session, sqlCns, plant, statusCode);
                //response.ProcessResponses.Add(response8);

                var mgr9 = new RackTransactionUploadManager(session);
                var response9 = mgr9.Process(session, sqlCns, plant, statusCode);
                response.ProcessResponses.Add(response9);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse ConsumedStoceUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Consumed Stoce Upload Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new ConsumedStoceUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse CustPackUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Cust Pack Upload Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new CustPackUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse HvCompletionUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} HV Completion Upload Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new HvCompletionUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse HvReceiptUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} HV Receipt Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new HvReceiptUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse HvTrimUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} HV Trim Upload Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new HvTrimUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse HvIssueUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} HV Issue Upload Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new HvIssueUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse HvWoIssueUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} HV WO Issue Upload Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new HvWoIssueUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse WoMgmtUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var responseByWO = new ProcessResponse($"{plant.PlantCode} WO Mgmt Upload Manager");
            var responseBySched = new ProcessResponse($"{plant.PlantCode} WO Mgmt Upload By Schedule Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new WoMgmtUploadManager(session);
                responseByWO = mgr.ProcessByWO(session, sqlCns, plant, statusCode);
                responseBySched = mgr.ProcessBySched(session, sqlCns, plant, statusCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            responseByWO.Close();
            responseBySched.Close();
            return responseBySched.RowsAffected > 0 ? responseBySched : responseByWO;
        }

        public static ProcessResponse RackTransactionUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Rack Transaction Upload Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new RackTransactionUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            response.Close();
            return response;
        }

        public static ProcessResponse RackAssetTransactionUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Rack Asset Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new RackAssetTransactionUpload(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }
    }
}
