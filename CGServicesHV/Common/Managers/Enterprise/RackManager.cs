using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class RackManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private GlassItemManager _glassItemMgr;
        private CustomerManager _customerMgr;
        private ItemAttributeManager _itemAttMgr;
        private GlassUnitManager _glassUnitMgr;

        public RackManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("RackManager.GetRack", GetRack());
            hash.Add("RackManager.GetRackItem", GetRackItem());
            hash.Add("RackManager.GetRackItems", GetRackItems());
            hash.Add("RackManager.GetRackItemReceiving", GetRackItemReceiving());
            hash.Add("RackManager.GetRackedOrders2", GetRackedOrders2());
            hash.Add("RackManager.GetBoxing", GetBoxing());
            hash.Add("RackManager.GetRacking", GetRacking());
            foreach (string keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _glassItemMgr = new GlassItemManager(session);
            _customerMgr = new CustomerManager(session);
            _itemAttMgr = new ItemAttributeManager(session);
            _glassUnitMgr = new GlassUnitManager(session);

        }


        /******************************************************************
        * Racks - Dynamic
        ******************************************************************/
        public iDB2Command GetRacks(List<string> filters)
        {
            var filterListString = String.Join(" ", filters);
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
	        SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RHPID
	        FROM
		        F580055
	        WHERE 
		        {0} 1=1", filterListString);
            return cmd;
        }

        /******************************************************************
        * Racks - Prepared
        ******************************************************************/
        public iDB2Command GetRack()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
	        SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RHPID
	        FROM
		        F580055
            WHERE 
                RHZ58RID = ?");
            cmd.Parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58RID"].Precision = 15;
            cmd.Parameters["Z58RID"].Scale = 0;
            return cmd;
        }

        /******************************************************************
        * Rack Items - Prepared
        ******************************************************************/
        public iDB2Command GetRackItems()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT 
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDDOC2,
                RDXCTO,
                RDLNIC,
                RDPKCO,
                RDPID,
                IBSTKT,
                IBGLPT
            FROM 
	            F580056
                INNER JOIN F4102 
                ON  IBMCU = RDMCU AND 
                    IBITM = RDITM
            WHERE 
                RDZ58RID = ?");
            cmd.Parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58RID"].Precision = 15;
            cmd.Parameters["Z58RID"].Scale = 0;
            return cmd;
        }

        /******************************************************************
        * Rack Item - Prepared
        ******************************************************************/
        public iDB2Command GetRackItem()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT 
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDDOC2,
                RDXCTO,
                RDLNIC,
                RDPKCO,
                RDPID,
                IBSTKT,
                IBGLPT
            FROM 
	            F580056
                INNER JOIN F4102 
                ON  IBMCU = RDMCU AND 
                    IBITM = RDITM
            WHERE 
                RDZ58RID = ? AND
                RDZ58SN = ?");
            cmd.Parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58RID"].Precision = 15;
            cmd.Parameters["Z58RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Z58SN", iDB2DbType.iDB2Graphic, 20));
            return cmd;
        }


        public Racks GetRacks(BusinessSystemSession session, decimal rackId, string serialNum, string plantCode = null, bool attributes = false)
        {
            var racks = new Racks();
            var cmd = new iDB2Command();
            List<String> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {

                if (rackId > 0)
                {
                    filters.Add("RHZ58RID = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }

                // Plant Code
                if (!String.IsNullOrEmpty(plantCode))
                {
                    filters.Add("RHMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantCode.PadLeft(12);
                }

                // Command
                cmd = GetRacks(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {

                    // Reader
                    while (reader.Read())
                    {
                        var rack = new Rack();
                        rack.RackId = CastDbNull.To<Decimal>(reader["RHZ58RID"], 0);
                        rack.CostCenter = CastDbNull.To<String>(reader["RHMCU"], String.Empty).Trim();
                        rack.Location = CastDbNull.To<String>(reader["RHLOCN"], String.Empty).Trim();
                        rack.RackType = CastDbNull.To<String>(reader["RHZ58RT"], String.Empty).Trim();
                        rack.HeaderStatus = CastDbNull.To<String>(reader["RHZ58HST"], String.Empty).Trim();
                        var statusDateId = CastDbNull.To<Decimal>(reader["RHZ58RSDT"], 0);
                        rack.HeaderStatusDate = Utilities.Common.CastToDateTime(statusDateId, 0);
                        rack.GlassWeight = Math.Round(CastDbNull.To<Decimal>(reader["RHZ58GWTT"], 0), 4);
                        rack.TareWeight = Math.Round(CastDbNull.To<Decimal>(reader["RHZ58RTWT"], 0), 4);
                        rack.GrossWeight = CastDbNull.To<Decimal>(reader["RHGWEI"], 0);
                        rack.ReasonCode = CastDbNull.To<String>(reader["RHZ58RCD"], String.Empty).Trim();
                        rack.LoadNum = CastDbNull.To<Decimal>(reader["RHPSN"], 0);
                        rack.ScheduleNum = CastDbNull.To<Decimal>(reader["RHZ58SCN"], 0);
                        rack.BoxNum = CastDbNull.To<Decimal>(reader["RHZ58BOX"], 0);
                        var updateDateId = CastDbNull.To<Decimal>(reader["RHUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<Decimal>(reader["RHTDAY"], 0);
                        rack.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                        var customerNum = CastDbNull.To<Decimal>(reader["RHAN8"], 0);
                        rack.Customer = _customerMgr.GetCustomer(session, customerNum);
                        rack.RackItems = GetRackItems(session, rack.RackId, serialNum, attributes);
                        rack.SupplierNum = rack.RackItems[0].SupplierNum;
                        rack.ProcessId = CastDbNull.To<String>(reader["RHPID"], String.Empty).Trim();
                        racks.Add(rack);
                    }

                    reader.Close();
                }
                Logger.Trace($"Rows: {racks.Count}");
                AddRackReceiving(session, racks);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException(e.ToString());
            }
            return racks;
        }

        public Rack GetRack(BusinessSystemSession session, Decimal rackId, string serialNum, bool attributes = true)
        {
            var rack = new Rack();
            try
            {
                var cmd = (session.DbCommands.ContainsKey("RackManager.GetRack")) ? session.DbCommands["RackManager.GetRack"] : GetRack();
                cmd.Parameters[0].Value = rackId;
                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        rack.RackId = CastDbNull.To<Decimal>(reader["RHZ58RID"], 0);
                        rack.CostCenter = CastDbNull.To<String>(reader["RHMCU"], String.Empty).Trim();
                        rack.Location = CastDbNull.To<String>(reader["RHLOCN"], String.Empty).Trim();
                        rack.RackType = CastDbNull.To<String>(reader["RHZ58RT"], String.Empty).Trim();
                        rack.HeaderStatus = CastDbNull.To<String>(reader["RHZ58HST"], String.Empty).Trim();
                        var statusDateId = CastDbNull.To<Decimal>(reader["RHZ58RSDT"], 0);
                        rack.HeaderStatusDate = Utilities.Common.CastToDateTime(statusDateId, 0);
                        rack.GlassWeight = Math.Round(CastDbNull.To<Decimal>(reader["RHZ58GWTT"], 0), 4);
                        rack.TareWeight = Math.Round(CastDbNull.To<Decimal>(reader["RHZ58RTWT"], 0), 4);
                        rack.GrossWeight = CastDbNull.To<Decimal>(reader["RHGWEI"], 0);
                        rack.ReasonCode = CastDbNull.To<String>(reader["RHZ58RCD"], String.Empty).Trim();
                        rack.LoadNum = CastDbNull.To<Decimal>(reader["RHPSN"], 0);
                        rack.ScheduleNum = CastDbNull.To<Decimal>(reader["RHZ58SCN"], 0);
                        rack.BoxNum = CastDbNull.To<Decimal>(reader["RHZ58BOX"], 0);
                        var updateDateId = CastDbNull.To<Decimal>(reader["RHUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<Decimal>(reader["RHTDAY"], 0);
                        rack.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                        var customerNum = CastDbNull.To<Decimal>(reader["RHAN8"], 0);
                        rack.Customer = _customerMgr.GetCustomer(session, customerNum);
                        rack.RackItems = GetRackItems(session, rack.RackId, serialNum, attributes);
                        rack.SupplierNum = rack.RackItems[0].SupplierNum;
                    }

                    reader.Close();
                }

                var racks = new Racks();
                racks.Add(rack);
                Logger.Trace($"Rows: {racks.Count}");
                AddRackReceiving(session, racks);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException(e.ToString());
            }
            return rack;
        }

        public RackItems GetRackItems(BusinessSystemSession session, Decimal rackId, string serialNum = null, bool attributes = true)
        {
            var rackItems = new RackItems();
            try
            {
                var cmd = new iDB2Command();
                if (String.IsNullOrEmpty(serialNum))
                {
                    cmd = (session.DbCommands.ContainsKey("RackManager.GetRackItems")) ? session.DbCommands["RackManager.GetRackItems"] : GetRackItems();
                    cmd.Parameters[0].Value = rackId;
                }
                else
                {
                    cmd = (session.DbCommands.ContainsKey("RackManager.GetRackItem")) ? session.DbCommands["RackManager.GetRackItem"] : GetRackItem();
                    cmd.Parameters[0].Value = rackId;
                    cmd.Parameters[1].Value = serialNum;
                }

                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                { 
                    while (reader.Read())
                    {
                        var rackItem = new RackItem();
                        rackItem.RackId = CastDbNull.To<Decimal>(reader["RDZ58RID"], 0);
                        rackItem.SerialNum = CastDbNull.To<String>(reader["RDZ58SN"], String.Empty).Trim();
                        rackItem.ItemNum = CastDbNull.To<Decimal>(reader["RDITM"], 0);
                        rackItem.GLCategory = CastDbNull.To<String>(reader["IBGLPT"], String.Empty).Trim();
                        rackItem.PlantNum = CastDbNull.To<String>(reader["RDMCU"], String.Empty).Trim();
                        rackItem.Side = CastDbNull.To<String>(reader["RDZ58SD"], String.Empty).Trim();
                        rackItem.Position = CastDbNull.To<Decimal>(reader["RDZ58PS"], 0);
                        rackItem.StackTier = CastDbNull.To<Decimal>(reader["RDZ58STK"], 0);
                        rackItem.Row = CastDbNull.To<Decimal>(reader["RDZ58ROW"], 0);
                        rackItem.Slot = CastDbNull.To<Decimal>(reader["RDZ58SL"], 0);
                        rackItem.LotNum = CastDbNull.To<String>(reader["RDLOTN"], String.Empty).Trim();
                        rackItem.DetailStatus = CastDbNull.To<String>(reader["RDZ58DST"], String.Empty).Trim();
                        var transactionDateId = CastDbNull.To<Decimal>(reader["RDTRDJ"], 0);
                        rackItem.TransactionDate = Utilities.Common.CastToDateTime(transactionDateId, 0);
                        var manufacturedDateId = CastDbNull.To<Decimal>(reader["RDZ58MDT"], 0);
                        rackItem.ManufacturedDate = Utilities.Common.CastToDateTime(manufacturedDateId, 0);
                        //rackItem.ManufacturedPlant = CastDbNull.To<String>(reader["WAMCU"], String.Empty).Trim();
                        var productionDateId = CastDbNull.To<Decimal>(reader["RDZ58PDT"], 0);
                        rackItem.ProductionDate = Utilities.Common.CastToDateTime(productionDateId, 0);
                        var updateDateId = CastDbNull.To<Decimal>(reader["RDUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<Decimal>(reader["RDTDAY"], 0);
                        rackItem.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                        rackItem.Quantity = CastDbNull.To<Decimal>(reader["RDPQOH"], 0) / 1000;
                        rackItem.QuantityUom = CastDbNull.To<String>(reader["RDUOM1"], String.Empty).Trim();
                        rackItem.GlassWeight = CastDbNull.To<Decimal>(reader["RDZ58GWT"], 0) / 100;
                        rackItem.GlassWeightUom = CastDbNull.To<String>(reader["RDUWUM"], String.Empty).Trim();
                        rackItem.ProcessId = CastDbNull.To<String>(reader["RDPID"], String.Empty).Trim();
                        rackItem.WorkOrderNum = CastDbNull.To<Decimal>(reader["RDDOC1"], 0);
                        //rackItem.WorkOrderPlantNum = CastDbNull.To<String>(reader["WAMCU"], String.Empty).Trim();
                        rackItem.SalesOrderNum = CastDbNull.To<Decimal>(reader["RDDOCO"], 0);
                        rackItem.SalesOrderType = CastDbNull.To<String>(reader["RDDCTO"], String.Empty).Trim();
                        rackItem.SalesOrderLineNum = CastDbNull.To<Decimal>(reader["RDLNID"], 0) / 1000;
                        rackItem.PurchaseOrderNum = CastDbNull.To<Decimal>(reader["RDDOC2"], 0);
                        rackItem.PurchaseOrderType = CastDbNull.To<String>(reader["RDXCTO"], String.Empty).Trim();
                        rackItem.PurchaseOrderLineNum = CastDbNull.To<Decimal>(reader["RDLNIC"], 0) / 1000;
                        if (attributes)
                        {
                            //var plant = PlantManager.GetPlant(rackItem.PlantNum);
                            var stockingType = CastDbNull.To<String>(reader["IBSTKT"], String.Empty).Trim();
                            if (stockingType != "C")
                            {
                                var itemAtts = _itemAttMgr.GetDiscreteItemAttributes(session, rackItem.ItemNum);
                                rackItem.GlassItem = _glassUnitMgr.GetGlassUnit(session, itemAtts);
                                //rackItem.GlassItem = _glassItemMgr.GetGlassItem(session, rackItem.ItemNum, plant.PlantNum);
                            }
                            else
                            {
                                var rdkcoo = CastDbNull.To<String>(reader["RDKCOO"], String.Empty).Trim();
                                var rdpkco = CastDbNull.To<String>(reader["RDPKCO"], String.Empty).Trim();
                                var cfgId = _itemAttMgr.GetConfiguredId(session, rdkcoo, rackItem.SalesOrderNum, rackItem.SalesOrderType, rackItem.SalesOrderLineNum * 1000, rdpkco, rackItem.PurchaseOrderNum, rackItem.PurchaseOrderType, rackItem.PurchaseOrderLineNum * 1000, rackItem.WorkOrderNum);
                                var itemAtts = _itemAttMgr.GetConfiguredItemAttributes(session, cfgId);
                                rackItem.GlassItem = _glassUnitMgr.GetGlassUnit(session, itemAtts);
                            }

                            try
                            {
                                rackItem.HasGlassAttributes = (rackItem.GlassItem.ItemAttributes.Count > 0);
                            }
                            catch (Exception)
                            {
                                rackItem.HasGlassAttributes = false;
                            }
                        }
                        //AddRackItemReceiving(e1Session, rackItem);

                        rackItems.Add(rackItem);
                    }

                    reader.Close();
                }
                Logger.Trace($"Rows: {rackItems.Count}");
                AddRackItemReceipt(session, rackItems);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException(e.ToString());
            }
            return rackItems;
        }

        /******************************************************************
        * Rack Receipt - Prepared
        ******************************************************************/
        public iDB2Command GetRackItemReceipt()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                *
            FROM (
                SELECT 
                    DLUPMJ,
                    DLPQOH,
                    ROW_NUMBER() OVER (PARTITION BY DLZ58RID, DLZ58SN, DLMCU ORDER BY DLUKID DESC) AS ROWNUM   
                FROM 
	                F580569
                WHERE 
                    DLZ58RID = ? AND
                    DLZ58SN = ? AND
                    DLMCU = ? AND
                    DLPQOH > 0) A
            WHERE
                A.ROWNUM = 1";
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 15;
            cmd.Parameters["RackId"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SerialNum", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public void AddRackItemReceipt(BusinessSystemSession e1Session, RackItems items)
        {
            try
            {
                foreach (RackItem item in items)
                {

                    var cmd = (e1Session.DbCommands.ContainsKey("RackManager.GetRackItemReceipt")) ? e1Session.DbCommands["RackManager.GetRackItemReceipt"] : GetRackItemReceipt();
                    cmd.Parameters[0].Value = item.RackId;
                    cmd.Parameters[1].Value = item.SerialNum;
                    cmd.Parameters[2].Value = item.PlantNum.PadLeft(12);
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    var i = 0;
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            var recdDateId = CastDbNull.To<Decimal>(reader["DLUPMJ"], 0);
                            item.ReceivedDate = Utilities.Common.CastToDateTime(recdDateId, 0);
                            //rackItem.SupplierNum = CastDbNull.To<Decimal>(reader["PHAN8"], 0);
                            item.ReceivedQty = CastDbNull.To<Decimal>(reader["DLPQOH"], 0) / 1000;
                            i++;
                        }

                        reader.Close();
                    }
                    Logger.Trace($"Rows: {i}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException(e.ToString());
            }
        }

        public iDB2Command GetRackItemReceiving()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"SELECT 
                DLUPMJ,
                DLTDAY,
                DLPID,
                DLUSER,
                DLPQOH,
                DLDOC1,
                PHAN8        
            FROM 
	            F580569
                INNER JOIN F4301
                ON  PHDOCO = DLDOC2 AND
                    PHDCTO = DLXCTO AND
                    PHKCOO = DLPKCO AND
                    DLDOC2 > 0
            WHERE 
                DLZ58RID = ? AND
                DLZ58SN = ? AND
                DLMCU = ? AND
                (
                    (DLPID IN ('N580060') AND DLZ58DST = '8' AND DLPSN > 0) OR
                    (DLPID IN ('P4312') AND DLPSN = 0)
                )";
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 15;
            cmd.Parameters["RackId"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SerialNum", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public void AddRackReceiving(BusinessSystemSession e1Session, Racks racks)
        {
            try
            {
                foreach (Rack rack in racks)
                {
                    decimal tot = 0;
                    foreach (RackItem rackItem in rack.RackItems)
                    {
                        var cmd = (e1Session.DbCommands.ContainsKey("RackManager.GetRackItemReceiving"))
                                      ? e1Session.DbCommands["RackManager.GetRackItemReceiving"]
                                      : GetRackItemReceiving();
                        cmd.Parameters[0].Value = rack.RackId;
                        cmd.Parameters[1].Value = rackItem.SerialNum;
                        cmd.Parameters[2].Value = rackItem.PlantNum.PadLeft(12);
                        cmd.Connection = e1Session.DbConnection;
                        Logger.Trace(Logging.DbCmdString(cmd));
                        var i = 0;
                        using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            while (reader.Read())
                            {
                                var recdDateId = CastDbNull.To<Decimal>(reader["DLUPMJ"], 0);
                                rackItem.ReceivedDate = Utilities.Common.CastToDateTime(recdDateId, 0);
                                rackItem.SupplierNum = CastDbNull.To<Decimal>(reader["PHAN8"], 0);
                                rackItem.ReceivedQty = CastDbNull.To<Decimal>(reader["DLPQOH"], 0) / 1000;
                                tot = tot + rackItem.ReceivedQty;
                                i++;
                            }

                            reader.Close();
                        }
                        Logger.Trace($"Rows: {i}");

                        rack.ReceivedDate = rackItem.ReceivedDate;
                        rack.SupplierNum = rackItem.SupplierNum;
                        rack.ReceivedQty = tot;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException(e.ToString());
            }
        }

        /******************************************************************
        * Racked Orders - Prepared
        ******************************************************************/
        public iDB2Command GetRackedOrders1()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
	        SELECT 
		        BIDOCO,
		        BIZ58SCN,
		        MAX(RHZ58SCN) AS RHZ58SCN,
		        COUNT(*) AS BOXED,
		        SUM(CASE WHEN RDZ58RID IS NOT NULL THEN 1 ELSE 0 END) AS RACKED
	        FROM
		        F580200
		        LEFT OUTER JOIN F580056 
			        INNER JOIN F580055
			        ON RHZ58RID = RDZ58RID
		        ON	RDMCU = BIMCU AND
			        RDDOC1 = BIDOCO AND
			        RDZ58SN = BIZ58SNUM
	        WHERE
		        BIMCU = ? AND
		        BIZ58SCN = ?
	        GROUP BY
		        BIDOCO,
		        BIZ58SCN
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("DOC1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOC1"].Precision = 8;
            cmd.Parameters["DOC1"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetRackedOrders2()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
	        SELECT 
		        BIDOCO,
		        BIZ58SCN,
		        MAX(RHZ58SCN) AS RHZ58SCN,
		        COUNT(*) AS BOXED,
		        SUM(CASE WHEN RDZ58RID IS NOT NULL THEN 1 ELSE 0 END) AS RACKED
	        FROM
		        F580200
		        LEFT OUTER JOIN F580056 
			        INNER JOIN F580055
			        ON RHZ58RID = RDZ58RID
		        ON	RDMCU = BIMCU AND
			        RDDOC1 = BIDOCO AND
			        RDZ58SN = BIZ58SNUM
	        WHERE
		        BIMCU = ? AND
		        BIDOCO = ?
	        GROUP BY
		        BIDOCO,
		        BIZ58SCN
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("DOC1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOC1"].Precision = 8;
            cmd.Parameters["DOC1"].Scale = 0;
            return cmd;
        }

        public DataTable GetRackedOrders(BusinessSystemSession session, Plant plant, decimal scheduleNum, List<decimal> workOrderNums)
        {
            var dt = new DataTable("RackedOrders");
            dt.Columns.Add("WorkOrderNum", typeof(decimal));
            dt.Columns.Add("IsPosted", typeof(string));
            dt.Columns.Add("IsRacked", typeof(string));
            dt.Columns.Add("ScheduleNum", typeof(decimal));
            try
            {
                iDB2Command cmd;
                if (scheduleNum > 0 && (workOrderNums?.Count ?? 0) == 0)
                {
                    cmd = GetRackedOrders1();
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = scheduleNum;
                    cmd.Connection = session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            var dr = dt.NewRow();
                            dr["WorkOrderNum"] = CastDbNull.To<Decimal>(reader["BIDOCO"], 0);
                            dr["ScheduleNum"] = CastDbNull.To<Decimal>(reader["BIZ58SCN"], 0);
                            dr["IsPosted"] = CastDbNull.To<int>(reader["BOXED"], 0) > 0 ? "Y" : "N";
                            dr["IsRacked"] = CastDbNull.To<int>(reader["RACKED"], 0) > 0 ? "Y" : "N";
                            dt.Rows.Add(dr);
                        }

                        reader.Close();
                    }
                }
                else
                {
                    cmd = session.DbCommands["RackManager.GetRackedOrders2"];
                    foreach (var workOrderNum in workOrderNums)
                    {
                        cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                        cmd.Parameters[1].Value = workOrderNum;
                        cmd.Connection = session.DbConnection;
                        Logger.Trace(Logging.DbCmdString(cmd));
                        using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            while (reader.Read())
                            {
                                var dr = dt.NewRow();
                                dr["WorkOrderNum"] = CastDbNull.To<Decimal>(reader["BIDOCO"], 0);
                                dr["ScheduleNum"] = CastDbNull.To<Decimal>(reader["BIZ58SCN"], 0);
                                dr["IsPosted"] = CastDbNull.To<int>(reader["BOXED"], 0) > 0 ? "Y" : "N";
                                dr["IsRacked"] = CastDbNull.To<int>(reader["RACKED"], 0) > 0 ? "Y" : "N";
                                dt.Rows.Add(dr);
                            }

                            reader.Close();
                        }
                        Logger.Trace($"Rows: {dt.Rows.Count}");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{e.Message} {e.StackTrace}");
            }
            return dt;
        }

        /******************************************************************
        * Schedule - Rack Status
        ******************************************************************/
        public iDB2Command GetScheduleRackStatus()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
	            BIEDBT,
	            BIZ58SCN,
	            BIMCU,
	            CAST(MIN(BIUPMJ * 1000000 + BITDAY) / 1000000 AS DECIMAL(6,0)) AS BIUPMJ,
	            MOD(MIN(BIUPMJ * 1000000 + BITDAY), 1000000) AS BITDAY,
	            CAST(MAX(DLMUPM * 1000000 + DLUPMT) / 1000000 AS DECIMAL(6,0)) AS DLMUPM,
	            MOD(MAX(DLMUPM * 1000000 + DLUPMT), 1000000) AS DLUPMT,
	            COUNT(*) AS F580220,
	            COUNT(DLZ58RID) AS F580569
            FROM 
	            F580200 
	            LEFT OUTER JOIN F580569
	            ON	DLMCU = BIMCU AND 
		            DLDOC1 = BIDOCO AND 
		            DLXDCTO = BIDCTO AND
		            DLZ58SN = BIZ58UNID AND
		            DLIOPR = 'Insert'
            WHERE
	            BIMCU = ? AND 
	            BIZ58SCN = ?
            GROUP BY
	            BIEDBT,
	            BIZ58SCN,
	            BIMCU
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN1"].Precision = 5;
            cmd.Parameters["SCN1"].Scale = 0;
            return cmd;
        }

        public DataTable GetScheduleRackStatus(BusinessSystemSession session, Plant plant, decimal scheduleNum)
        {
            var dt = new DataTable("ScheduleRackStatus");
            dt.Columns.Add("BatchNum", typeof(string));
            dt.Columns.Add("ScheduleNum", typeof(decimal));
            dt.Columns.Add("BoxedDate", typeof(DateTime));
            dt.Columns.Add("RackedDate", typeof(DateTime));
            dt.Columns.Add("Boxed", typeof(int));
            dt.Columns.Add("Racked", typeof(int));
            try
            {
                var cmd = GetScheduleRackStatus();
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        var dr = dt.NewRow();
                        dr["BatchNum"] = CastDbNull.To<string>(reader["BIEDBT"], String.Empty).Trim();
                        dr["ScheduleNum"] = CastDbNull.To<Decimal>(reader["BIZ58SCN"], 0);
                        dr["BoxedDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<Decimal>(reader["BIUPMJ"], 0), CastDbNull.To<Decimal>(reader["BITDAY"], 0));
                        dr["RackedDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<Decimal>(reader["DLMUPM"], 0), CastDbNull.To<Decimal>(reader["DLUPMT"], 0));
                        dr["Boxed"] = CastDbNull.To<int>(reader["F580220"], 0);
                        dr["Racked"] = CastDbNull.To<int>(reader["F580569"], 0);
                        dt.Rows.Add(dr);
                    }

                    reader.Close();
                }
                Logger.Trace($"Rows: {dt.Rows.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{e.Message} {e.StackTrace}");
            }

            return dt;
        }

        /******************************************************************
        * Is Boxed
        ******************************************************************/
        public iDB2Command GetBoxing()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
	            BIZ58SCN
            FROM 
	            F580200 
            WHERE
	            BIMCU = ? AND 
	            BIDOCO = ?
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            return cmd;
        }

        public decimal GetBoxing(BusinessSystemSession session, Plant plant, decimal workOrderNum)
        {
            decimal scheduleNum = 0;
            try
            {
                var cmd = session.DbCommands["RackManager.GetBoxing"];
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = workOrderNum;
                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        scheduleNum = CastDbNull.To<decimal>(reader["BIZ58SCN"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message} {e.StackTrace}");
            }

            return scheduleNum;
        }

        /******************************************************************
        * Is Racked
        ******************************************************************/
        public iDB2Command GetRacking()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
	            RHZ58SCN,
                RHZ58RID
            FROM 
	            F580056 
                INNER JOIN F580055
                ON RHZ58RID = RDZ58RID
            WHERE
	            RDMCU = ? AND 
	            RDDOC1 = ? AND
                RDDOC1 > 0
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("DOC1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOC1"].Precision = 8;
            cmd.Parameters["DOC1"].Scale = 0;
            return cmd;
        }

        public Hashtable GetRacking(BusinessSystemSession session, Plant plant, decimal workOrderNum)
        {
            var hash = new Hashtable();
            try
            {
                var cmd = session.DbCommands["RackManager.GetRacking"];
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = workOrderNum;
                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    hash.Add("ScheduleNum", (decimal) 0);
                    hash.Add("RackId", (decimal) 0);
                    while (reader.Read())
                    {
                        hash["ScheduleNum"] = CastDbNull.To<decimal>(reader["RHZ58SCN"], 0);
                        hash["RackId"] = CastDbNull.To<decimal>(reader["RHZ58RID"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message} {e.StackTrace}");
            }

            return hash;
        }
    }
}





