using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.WOStatusSvc
{
    public class WorkOrderStatusProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SqlDmlResponse SetPendingUpdates()
        {
            var session = new BusinessSystemSession();
            try
            {
                session = BusinessSystemSessionManager.GetBusinessSession();
                var manager = new WorkOrderStatusManager(session);
                return manager.SetPendingUpdates(session);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
        }

        public PendingUpdatesResults GetPlantsWithPendingUpdates()
        {
            var session = new BusinessSystemSession();
            PendingUpdatesResults results;
            try
            {
                session = BusinessSystemSessionManager.GetBusinessSession();
                var manager = new WorkOrderStatusManager(session);
                results = manager.GetPlantsWithPendingUpdates(session);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
            return results;
        }

        public WorkOrderStatuses GetPendingUpdatesByPlant(string plantCode)
        {
            var session = new BusinessSystemSession();
            WorkOrderStatuses workOrderStatuses;
            try
            {
                session = BusinessSystemSessionManager.GetBusinessSession();
                var manager = new WorkOrderStatusManager(session);
                workOrderStatuses = manager.GetPendingUpdatesByPlant(session, plantCode);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
            return workOrderStatuses;
        }

        public PlantOperationResponses ProcessAll()
        {
            var responses = new PlantOperationResponses();
            var session = new BusinessSystemSession();
            var activePlants = ConfigurationManager.AppSettings["ActivePlants"].Split(',');
            var suspendedPlants = ConfigurationManager.AppSettings["SuspendedPlants"].Split(',');
            try
            {
                session = BusinessSystemSessionManager.GetBusinessSession();
                var manager = new WorkOrderStatusManager(session);
                var results = GetPlantsWithPendingUpdates();
                foreach (PendingUpdatesResult result in results)
                {
                    var bulkErrors = false;

                    var operationResponse = new PlantOperationResponse();
                    operationResponse.StartDate = DateTime.Now;
                    operationResponse.PlantNum = result.PlantNum;

                    var isSuspended = suspendedPlants.Contains(result.PlantNum);
                    operationResponse.IsSuspended = isSuspended;
                    var isActive = activePlants.Contains(result.PlantNum);
                    operationResponse.IsActive = isActive;
                    operationResponse.PendingUpdates = result.Updates;
 
                    if (!isSuspended)
                    {
                        operationResponse.IsSuspended = false;
                        if (isActive)
                        {
                            operationResponse.IsActive = true;
                            var updates = GetPendingUpdatesByPlant(result.PlantNum);
                            var bulkResponse = BulkToPlant(result.PlantNum, updates);
                            operationResponse.DestinationPersistence = bulkResponse;
                            bulkErrors = bulkResponse.HasErrors();
                        }
                        if (!bulkErrors)
                        {
                            var response = manager.SetCompletedUpdatesByPlant(session, result.PlantNum);
                            operationResponse.SourcePersistence = response;
                        }              
                    }
                    operationResponse.EndDate = DateTime.Now;
                    responses.Add(operationResponse);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
            return responses;
        }

        public SqlBulkCopyResponse BulkToPlant(string plantNum, WorkOrderStatuses updates)
        {
            var response = new SqlBulkCopyResponse();
            var bulkCopy = new SqlBulkCopyManager();
            var destCn = ConnectionManager.GetSqlConnectionFromKey(plantNum);
            try
            {
                ConnectionManager.IsPingable(destCn.DataSource);
                bulkCopy.DbConnectionString = destCn.ConnectionString;
                response = bulkCopy.BulkTable("E1WorkOrderStatusLedger", updates.SerializeToDataTable());  
            }
            catch (Exception e)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(e));
                response.Message = e.ToString();
                Logger.Error("ERROR: " + destCn.ConnectionString + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(destCn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return response;
        }
    }
}
