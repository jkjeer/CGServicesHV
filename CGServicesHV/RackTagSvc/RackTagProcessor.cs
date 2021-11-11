using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Enums;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.RackTagSvc
{
    public class RackTagProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackTagItems GetPlacard(
            string plantCode,
            Decimal rackId,
            string groupStrategy)
        {
            BusinessSystemSession e1Session = new BusinessSystemSession();
            SqlConnection sqlCn = new SqlConnection();
            RackTagItems placard;
            try
            {
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                Plant plant = PlantManager.GetPlant(plantCode);
                sqlCn = ConnectionManager.GetSqlConnectionFromKey(plantCode + ".SHOPFLOOR", true);
                sqlCn.Open();
                int num = (int)EnumValidator.GetEnum<RackTag.GroupStrategyEnum>(groupStrategy, true, false);
                placard = new RackTagManager(e1Session).GetPlacard(e1Session, sqlCn, plant, rackId, groupStrategy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                e1Session.Close();
                sqlCn.Close();
            }
            return placard;
        }

        public GenericGetResponse<RackStatusCode> GetRackStatusCodes()
        {
            var e1Session = GetE1Session();
            var rackStatusResponse = new GenericGetResponse<RackStatusCode>();
            RackStatusCodes rackStatusCodes;
            try
            {
                var manager = new RackStatusCodeManager(e1Session);
                rackStatusCodes = manager.GetRackStatusCodes(e1Session);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            rackStatusResponse.Results = rackStatusCodes;
            return rackStatusResponse;
        }


        public List<decimal> GetRackIdListByStatus(string plantCode, string statusCode, TimeSpan timeSpan)
        {
            var e1Session = GetE1Session();
            List<decimal> rackIds;
            try
            {
                var manager = new RackIdListManager(e1Session);
                var plant = PlantManager.GetPlant(plantCode);
                rackIds = manager.GetRackIdListByStatus(e1Session, plant, statusCode, timeSpan);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            return rackIds;
        }

        public List<decimal> GetRackIdListBySchedule(string plantCode, decimal scheduleNum)
        {
            var e1Session = GetE1Session();
            List<decimal> rackIds;
            try
            {
                var manager = new RackIdListManager(e1Session);
                rackIds = manager.GetRackIdList(e1Session, plantCode, scheduleNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            return rackIds;
        }

        public RackTagItems GetRackTagItems(decimal rackId, bool byUnitId)
        {
            var e1Session = GetE1Session();
            RackTagItems items;
            try
            {
                var itemMgr = new RackTagItemManager(e1Session);
                items = itemMgr.GetRackTagItems(e1Session, rackId, byUnitId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            return items;
        }

        public RackTagItems GetRackTagItemsBySchedule(string plantCode, decimal scheduleNum, bool byUnitId = true)
        {
            var e1Session = GetE1Session();
            var items = new RackTagItems();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                var itemMgr = new RackTagItemManager(e1Session);
                var rackIds = GetRackIdListBySchedule(plantCode, scheduleNum);
                foreach (var rackId in rackIds)
                {
                    var rackItems = itemMgr.GetRackTagItems(e1Session, rackId, byUnitId);
                    items.AddRange(rackItems);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            return items;
        }

        public RackTagItems FilterRackTags(RackTagItems rackTags, string filter)
        {
            var results = rackTags.AsQueryable().Where(filter);
            var returnTags = new RackTagItems();
            foreach (var result in results)
            {
                returnTags.Add(result);
            }
            return returnTags;
        }

        public List<decimal> GetLockedRacks(string plantCode, decimal minutesLatency)
        {
            var e1Session = GetE1Session();
            List<decimal> rackIds;
            try
            {
                var mgr = new RackIdListManager(e1Session);
                rackIds = mgr.GetLockedRacks(e1Session, plantCode, minutesLatency);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            return rackIds;
        }

        private static BusinessSystemSession GetE1Session(ConnectionStrings connectionStrings = null)
        {
            var e1Session = BusinessSystemSessionManager.GetBusinessSession();
            ConnectionStringManager.AddConnectionString(connectionStrings, "cnE1", e1Session.DbConnection.ConnectionString);
            return e1Session;
        }


        // NEW
        public RackTagItems GetRackTags(string plantCode, decimal rackId, string groupStrategy)
        {
            var e1Session = GetE1Session();
            SqlConnection sqlCn = new SqlConnection();
            RackTagItems items;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                sqlCn = ConnectionManager.GetSqlConnectionFromKey(plantCode + ".SHOPFLOOR", true);
                EnumValidator.GetEnum<RackTag.GroupStrategyEnum>(groupStrategy);
                var itemMgr = new RackTagManager(e1Session);
                items = itemMgr.GetRackTagItems(e1Session, sqlCn, plant, rackId, groupStrategy);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
                sqlCn.Close();
            }
            return items;
        }

        public HVRackTagItem GetHVRackTagMfg(string plantCode, string serialNum)
        {
            var e1Session = GetE1Session();
            HVRackTagItem item;
            try
            {
                var itemMgr = new HVRackTagManager(e1Session);
                item = itemMgr.GetHVRackTag(e1Session, plantCode, serialNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            return item;
        }

        public HVRackTagItems GetHVRackTagsMfgByWorkOrder(string plantCode, decimal workOrder)
        {
            var e1Session = GetE1Session();
            var items = new HVRackTagItems();
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            try
            {
                var serialNums = F580055Manager.GetSerialNumsByWorkOrder(e1Session, plantNum, workOrder);
                var itemMgr = new HVRackTagManager(e1Session);
                foreach (var serialNum in serialNums)
                {
                    items.Add(itemMgr.GetHVRackTag(e1Session, plantNum, serialNum));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            return items;
        }

        public HVRackTagItems GetHVRackTagsMfgBySchedule(string plantCode, decimal scheduleNum)
        {
            var e1Session = GetE1Session();
            var items = new HVRackTagItems();
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            try
            {
                var serialNums = F580055Manager.GetSerialNumsBySchedule(e1Session, plantNum, scheduleNum);
                var itemMgr = new HVRackTagManager(e1Session);
                foreach (var serialNum in serialNums)
                {
                    items.Add(itemMgr.GetHVRackTag(e1Session, plantNum, serialNum));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
            }
            return items;
        }

        public RackTagItems GetRackTagsShipped(string plantCode, decimal rackId, string groupStrategy)
        {
            var e1Session = GetE1Session();
            SqlConnection sqlCn = new SqlConnection();
            RackTagItems items;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                sqlCn = ConnectionManager.GetSqlConnectionFromKey(plantCode + ".SHOPFLOOR", true);
                EnumValidator.GetEnum<RackTag.GroupStrategyEnum>(groupStrategy);
                var itemMgr = new RackTagManager(e1Session);
                items = itemMgr.GetRackTagItems(e1Session, sqlCn, plant, rackId, groupStrategy);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
                sqlCn.Close();
            }
            return items;
        }

        public RackTagItems GetRackTagsBySchedule(string plantCode, decimal scheduleNum, string groupStrategy)
        {
            var e1Session = GetE1Session();
            SqlConnection sqlCn = new SqlConnection();
            var items = new RackTagItems();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                sqlCn = ConnectionManager.GetSqlConnectionFromKey(plantCode + ".SHOPFLOOR", true);
                EnumValidator.GetEnum<RackTag.GroupStrategyEnum>(groupStrategy);
                var itemMgr = new RackTagManager(e1Session);
                var rackIds = GetRackIdListBySchedule(plantCode, scheduleNum);
                foreach (var rackId in rackIds)
                {
                    var rackItems = itemMgr.GetRackTagItems(e1Session, sqlCn, plant, rackId, groupStrategy);
                    items.AddRange(rackItems);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                e1Session.Close();
                sqlCn.Close();
            }
            return items;
        }
    }
}
