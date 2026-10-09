using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class WorkOrderManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly GlassItemManager _glassItemMgr;
        private readonly CustomerManager _customerMgr;
        private readonly EdiMessageManager _ediMessageMgr;
        private readonly CrateManager _crateMgr;
        private readonly UdcItemManager _udcMgr;

        public WorkOrderManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"WorkOrderManager.GetWorkOrder", GetWorkOrder()},
                {"WorkOrderManager.GetWorkOrderComponents", GetWorkOrderComponents()},
                {"WorkOrderManager.GetWorkOrderRoutings", GetWorkOrderRoutings()},
                {"WorkOrderManager.GetCancelledOrdersBySchedule", GetCancelledOrdersBySchedule()},
                {"WorkOrderManager.GetUnitIds", GetUnitIds()},
                {"WorkOrderManager.GetWORelatedSalesOrderLine", GetWORelatedSalesOrderLine()},
                {"WorkOrderManager.GetSalesOrderLine", GetSalesOrderLine()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
            _glassItemMgr = new GlassItemManager(e1Session);
            _ediMessageMgr = new EdiMessageManager(e1Session);
            _customerMgr = new CustomerManager(e1Session);
            _crateMgr = new CrateManager(e1Session);
            _udcMgr = new UdcItemManager(e1Session);
        }

        /*********************************************************************************
         * Prepared Commands
         *******************************************************************************/
        public iDB2Command GetWorkOrder()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT
                        WADOCO,
                        WAMCU,
                        WASRST,
                        WATRDJ,
                        WADRQJ,
                        DRDL01,
                        WAITM,
                        WAUORG,
                        WARORN,
                        WARCTO,
                        WALNID,
                        WACO,
                        IBSTKT
                    FROM
                        F4801
                        INNER JOIN F4102
                        ON IBMCU = WAMCU AND IBITM = WAITM
                        INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                        ON  DRSY = '00' AND
                            DRRT = 'SS' AND
                            TRIM(DRKY) = WASRST
                    WHERE
                        WADOCO = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("WorkOrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["WorkOrderNum"].Precision = 8;
            cmd.Parameters["WorkOrderNum"].Scale = 0;
            return cmd;
        }

        public WorkOrder GetWorkOrder(BusinessSystemSession e1Session, decimal orderNum, ItemAttributeCache cache)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var sqlResponse = new SqlDmlResponse();
            var order = new WorkOrder();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetWorkOrder")) ? e1Session.DbCommands["WorkOrderManager.GetWorkOrder"] : GetWorkOrder();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = orderNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                sqlResponse.Init(cmd);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var rowCount = 0;
                    while (reader.Read())
                    {
                        order.WorkOrderNum = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                        order.PlantNum = Data.CastDbNull.To(reader["WAMCU"], string.Empty).Trim();
                        order.PlantCode = PlantManager.GetPlant(order.PlantNum).PlantCode;
                        order.StatusCode = Data.CastDbNull.To(reader["WASRST"], string.Empty).Trim();
                        order.StatusDesc = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        order.OrderDate = Utilities.Common.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WATRDJ"], 0));
                        order.RequestedDate = Utilities.Common.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WADRQJ"], 0));
                        order.Quantity = Data.CastDbNull.To<decimal>(reader["WAUORG"], 0) / 1000;
                        order.ItemNum = Data.CastDbNull.To<decimal>(reader["WAITM"], 0);

                        var documentNum = Utilities.Common.CastStringToDecimal(Data.CastDbNull.To(reader["WARORN"], string.Empty).Trim());
                        var documentType = Data.CastDbNull.To(reader["WARCTO"], string.Empty).Trim();
                        var documentLineNum = Data.CastDbNull.To<decimal>(reader["WALNID"], 0);
                        var companyNum = Data.CastDbNull.To(reader["WACO"], string.Empty).Trim();
                        var stockingType = Data.CastDbNull.To(reader["IBSTKT"], string.Empty).Trim();
                        order.RelatedSalesOrderLine = GetRelatedSalesOrderLine(e1Session, documentNum, documentType, companyNum, documentLineNum);

                        var glassItem = stockingType == "C" ? _glassItemMgr.GetGlassItem(e1Session, order.ItemNum, order.PlantCode, order.WorkOrderNum, "WO", 0, "*", cache) : _glassItemMgr.GetGlassItem(e1Session, order.ItemNum, order.PlantCode);
                        order.GlassItem = glassItem;

                        // Original Customer Order Attributes
                        order.RelatedSalesOrderLine.OriginalSoldToCustomer = _customerMgr.GetCustomer(e1Session, glassItem.OriginalSoldToCustomerNum);
                        order.RelatedSalesOrderLine.OriginalShipToCustomer = _customerMgr.GetCustomer(e1Session, glassItem.OriginalShipToCustomerNum);
                        order.RelatedSalesOrderLine.CustomerLiteNum = glassItem.ItemAttributes.GetDecimalAttributeValue("Z58L1CLN");

                        order.Routings = GetWorkOrderRoutings(e1Session, orderNum);
                        rowCount++;
                    }

                    reader.Close();
                    sqlResponse.Done(rowCount);
                    Logger.Trace("ROWS: " + rowCount);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return order;
        }

        public WorkOrder GetWorkOrder(BusinessSystemSession e1Session, decimal orderNum, bool includeRoutings = false, bool includeComponents = false, bool includeUnitIds = false)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var sqlResponse = new SqlDmlResponse();
            var order = new WorkOrder();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetWorkOrder")) ? e1Session.DbCommands["WorkOrderManager.GetWorkOrder"] : GetWorkOrder();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = orderNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                sqlResponse.Init(cmd);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var rowCount = 0;
                    while (reader.Read())
                    {
                        order.WorkOrderNum = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                        order.PlantNum = Data.CastDbNull.To(reader["WAMCU"], string.Empty).Trim();
                        order.PlantCode = PlantManager.GetPlant(order.PlantNum).PlantCode;
                        order.StatusCode = Data.CastDbNull.To(reader["WASRST"], string.Empty).Trim();
                        order.StatusDesc = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        order.OrderDate = Utilities.Common.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WATRDJ"], 0));
                        order.RequestedDate = Utilities.Common.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WADRQJ"], 0));
                        order.Quantity = Data.CastDbNull.To<decimal>(reader["WAUORG"], 0) / 1000;
                        order.ItemNum = Data.CastDbNull.To<decimal>(reader["WAITM"], 0);

                        var documentNum = Utilities.Common.CastStringToDecimal(Data.CastDbNull.To(reader["WARORN"], string.Empty).Trim());
                        var documentType = Data.CastDbNull.To(reader["WARCTO"], string.Empty).Trim();
                        var documentLineNum = Data.CastDbNull.To<decimal>(reader["WALNID"], 0);
                        var companyNum = Data.CastDbNull.To(reader["WACO"], string.Empty).Trim();
                        order.RelatedSalesOrderLine = GetWORelatedSalesOrderLine(e1Session, documentNum, documentType, companyNum, documentLineNum);

                        var stockingType = Data.CastDbNull.To(reader["IBSTKT"], string.Empty).Trim();
                        var glassItem = stockingType == "C" ? _glassItemMgr.GetGlassItem(e1Session, order.ItemNum, order.PlantCode, order.WorkOrderNum, "WO", 0, "*") : _glassItemMgr.GetGlassItem(e1Session, order.ItemNum, order.PlantCode);
                        order.GlassItem = glassItem;

                        // Original Customer Order Attributes
                        order.RelatedSalesOrderLine.OriginalSoldToCustomer = _customerMgr.GetCustomer(e1Session, glassItem.OriginalSoldToCustomerNum);
                        order.RelatedSalesOrderLine.OriginalShipToCustomer = _customerMgr.GetCustomer(e1Session, glassItem.OriginalShipToCustomerNum);
                        order.RelatedSalesOrderLine.CustomerLiteNum = glassItem.ItemAttributes.GetDecimalAttributeValue("Z58L1CLN");
                        rowCount++;
                    }

                    reader.Close();
                    sqlResponse.Done(rowCount);
                    Logger.Trace("ROWS: " + rowCount);
                }

                if (includeRoutings) { order.Routings = GetWorkOrderRoutings(e1Session, orderNum); }
                if (includeComponents) { order.WorkOrderComponents = GetWorkOrderComponents(e1Session, orderNum); }
                if (includeUnitIds) { order.UnitIds = GetUnitIds(e1Session, orderNum); }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return order;
        }

        public WorkOrder BuildWorkOrder(BusinessSystemSession e1Session, decimal orderNum, ItemAttributeCache cache)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var order = new WorkOrder();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("F4801Manager.GetWorkOrder")) ? e1Session.DbCommands["F4801Manager.GetWorkOrder"] : GetWorkOrder();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = orderNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        order.WorkOrderNum = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                        order.PlantNum = Data.CastDbNull.To(reader["WAMCU"], string.Empty).Trim();
                        order.PlantCode = PlantManager.GetPlant(order.PlantNum).PlantCode;
                        order.StatusCode = Data.CastDbNull.To(reader["WASRST"], string.Empty).Trim();
                        order.StatusDesc = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        order.OrderDate = Utilities.Common.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WATRDJ"], 0));
                        order.RequestedDate = Utilities.Common.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WADRQJ"], 0));
                        order.Quantity = Data.CastDbNull.To<decimal>(reader["WAUORG"], 0) / 1000;
                        order.ItemNum = Data.CastDbNull.To<decimal>(reader["WAITM"], 0);

                        order.RelatedOrderNum = Utilities.Common.CastStringToDecimal(Data.CastDbNull.To(reader["WARORN"], string.Empty).Trim());
                        order.RelatedOrderType = Data.CastDbNull.To(reader["WARCTO"], string.Empty).Trim();
                        order.RelatedOrderLineNum = Data.CastDbNull.To<decimal>(reader["WALNID"], 0);
                        order.RelatedOrderCompany = Data.CastDbNull.To(reader["WACO"], string.Empty).Trim();
                        order.StockingType = Data.CastDbNull.To(reader["IBSTKT"], string.Empty).Trim();
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return order;
        }

        /*********************************************************************************
         * Cancelled Orders
         *******************************************************************************/
        public iDB2Command GetCancelledOrdersBySchedule()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    WADOCO
                    FROM 
	                    F5834A11
	                    INNER JOIN ( SELECT
					                    S1MCU ,
					                    S1Z58SCN ,
					                    MAX(S1Z58SCD) AS S1Z58SCD
				                    FROM
					                    F5834A01
				                    GROUP BY
					                    S1MCU ,
					                    S1Z58SCN
				                    ) S1
	                    ON	S1.S1MCU = S5MCU AND
		                    S1.S1Z58SCN = S5Z58SCN AND
		                    S1.S1Z58SCD = S5Z58SCD
	                    INNER JOIN F4801
	                    ON  WAMCU = S5MCU AND
		                    WADOCO = S5DOCO
                    WHERE 
                        WASRST = '98' AND
	                    S1MCU = ? AND 
	                    S1Z58SCN = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ScheduleNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleNum"].Precision = 5;
            cmd.Parameters["ScheduleNum"].Scale = 0;
            return cmd;
        }

        public List<decimal> GetCancelledOrdersBySchedule(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var orderNums = new List<decimal>();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetCancelledOrdersBySchedule")) ? e1Session.DbCommands["WorkOrderManager.GetCancelledOrdersBySchedule"] : GetCancelledOrdersBySchedule();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        orderNums.Add(Data.CastDbNull.To<decimal>(reader["WADOCO"], 0));
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + orderNums.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return orderNums;
        }

        /*********************************************************************************
         * Routings
         *******************************************************************************/
        public iDB2Command GetWorkOrderRoutings()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        WLDOCO, 
                        WLDCTO, 
                        WLTRT, 
                        WLMMCU, 
                        WLMCU, 
                        WLDSC1, 
                        WLOPSQ,
                        DRDL02          
                    FROM 
	                    F3112
                        INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                        ON  DRSY = '58' AND
                            DRRT = 'RT' AND
                            TRIM(DRKY) = CAST(CAST((WLOPSQ / 100.00) AS DECIMAL(9,2)) AS VARCHAR(10))
                    WHERE 
                        WLOPSQ < 98000 AND
                        WLDOCO = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("WorkOrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["WorkOrderNum"].Precision = 8;
            cmd.Parameters["WorkOrderNum"].Scale = 0;
            return cmd;
        }

        public Routings GetWorkOrderRoutings(BusinessSystemSession e1Session, decimal orderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var routings = new Routings();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetWorkOrderRoutings")) ? e1Session.DbCommands["WorkOrderManager.GetWorkOrderRoutings"] : GetWorkOrderRoutings();
                cmd.Parameters[0].Value = orderNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var routing = new Routing
                        {
                            RoutingType = Data.CastDbNull.To(reader["WLTRT"], string.Empty).Trim(),
                            WorkCenter = Data.CastDbNull.To(reader["WLMCU"], string.Empty).Trim(),
                            Description = Data.CastDbNull.To(reader["WLDSC1"], string.Empty).Trim(),
                            SequenceNum = Data.CastDbNull.To<decimal>(reader["WLOPSQ"], 0) / 100,
                            ShopFloorCode = Data.CastDbNull.To(reader["DRDL02"], string.Empty).Trim()
                        };
                        routings.Add(routing);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + routings.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return routings;
        }

        /*********************************************************************************
         * Components
         *******************************************************************************/
        public iDB2Command GetWorkOrderComponents()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        WMDOCO,
                        WMOPSQ,
                        WMCPIT,
                        WMCMCU,
                        WMDSC1,
                        WMDSC2,
                        WMLOCN,
                        WMUORG,
                        WMTRQT,
                        WMUM,
                        WMCPNB,
                        WMMCU,
                        IBSTKT,
                        IBGLPT
                    FROM
                        F3111
                        LEFT OUTER JOIN F4102 
                        ON  IBMCU = WMCMCU AND
                            IBITM = WMCPIT
                    WHERE
                        WMDOCO = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("WorkOrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["WorkOrderNum"].Precision = 8;
            cmd.Parameters["WorkOrderNum"].Scale = 0;
            return cmd;
        }

        public WorkOrderComponents GetWorkOrderComponents(BusinessSystemSession e1Session, decimal orderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var components = new WorkOrderComponents();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetWorkOrderComponents")) ? e1Session.DbCommands["WorkOrderManager.GetWorkOrderComponents"] : GetWorkOrderComponents();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = orderNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var component = new WorkOrderComponent
                        {
                            WorkOrderNum = Data.CastDbNull.To<decimal>(reader["WMDOCO"], 0),
                            SequenceNum = Data.CastDbNull.To<decimal>(reader["WMOPSQ"], 0) / 100,
                            Quantity = Data.CastDbNull.To<decimal>(reader["WMUORG"], 0) / 1000,
                            InventoryQuantity = Data.CastDbNull.To<decimal>(reader["WMTRQT"], 0) / 1000,
                            ComponentNum = Data.CastDbNull.To<decimal>(reader["WMCPNB"], 0) / 100,
                            Location = Data.CastDbNull.To(reader["WMLOCN"], string.Empty).Trim(),
                            UnitOfMeasure = Data.CastDbNull.To(reader["WMUM"], string.Empty).Trim()
                        };
                        //var itemNum = Data.CastDbNull.To<decimal>(reader["WMCPIT"], 0);
                        //var plantNum = Data.CastDbNull.To(reader["WMMCU"], string.Empty).Trim();
                        //var glCategory = Data.CastDbNull.To(reader["IBGLPT"], string.Empty).Trim();
                        //component.Item = _itemMgr.GetTypedItem(e1Session, plantNum, itemNum, glCategory);
                        components.Add(component);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + components.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return components;
        }

        /*********************************************************************************
         * Unit Ids
         *******************************************************************************/
        public iDB2Command GetUnitIds()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                   BIZ58UNID
                    FROM 
	                    F580200
                    WHERE 
                        BIDOCO = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            return cmd;
        }

        public List<decimal> GetUnitIds(BusinessSystemSession e1Session, decimal orderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var unitIds = new List<decimal>();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetUnitIds")) ? e1Session.DbCommands["WorkOrderManager.GetUnitIds"] : GetUnitIds();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = orderNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        unitIds.Add(Convert.ToDecimal(Data.CastDbNull.To(reader["BIZ58UNID"], string.Empty).Trim()));
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + unitIds.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return unitIds;
        }

        /*********************************************************************************
         * Related Sales Order Line
         *******************************************************************************/
        public iDB2Command GetWORelatedSalesOrderLine()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        SDDOCO,
                        SDDCTO,
                        SDLNID,
                        SDKCOO,
                        SDVR01,
                        SDVR02,
                        SDRCD,
                        SDURCD,
	                    SHAN8,
                        SHSHAN,
                        ATZ58CSQNO,                        
                        ATZ58GSQNO,
                        ATZ58PT,
                        ATZ58CP2,
                        ATZ58CP3,
                        ATZ58CP4,
                        ATZ5899009,
                        ATZ5899010,
                        ATZ5899011,
                        ATZ5899012,
                        ATZ5840460,
                        ATZ58WODC,
                        ATZ58WOTY,
                        ATZ58CRATE 
                    FROM 
	                    F4211
                        INNER JOIN F4201
                        ON  SHDOCO = SDDOCO AND
                            SHDCTO = SDDCTO AND
                            SHKCOO = SDKCOO
                        INNER JOIN F580032
                        ON  ATDOCO = SDDOCO AND
                            ATDCTO = SDDCTO AND
                            ATLNID = SDLNID AND
                            ATKCOO = SDKCOO
                    WHERE 
                        SDDOCO = ? AND
                        SDDCTO = ? AND
                        SDKCOO = ? AND
                        SDLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("Company", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public RelatedSalesOrderLine GetWORelatedSalesOrderLine(BusinessSystemSession e1Session, decimal orderNum, string orderType, string company, decimal orderLineNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var line = new RelatedSalesOrderLine();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetWORelatedSalesOrderLine")) ? e1Session.DbCommands["WorkOrderManager.GetWORelatedSalesOrderLine"] : GetWORelatedSalesOrderLine();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = orderNum;
                cmd.Parameters[1].Value = orderType;
                cmd.Parameters[2].Value = company;
                cmd.Parameters[3].Value = orderLineNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        line.OrderNum = Data.CastDbNull.To<decimal>(reader["SDDOCO"], 0);
                        line.OrderType = Data.CastDbNull.To(reader["SDDCTO"], string.Empty).Trim();
                        line.OrderLineNum = Data.CastDbNull.To<decimal>(reader["SDLNID"], 0) / 1000;
                        line.OrderCompany = Data.CastDbNull.To(reader["SDKCOO"], string.Empty).Trim();
                        var soldToCustomerNum = Data.CastDbNull.To<decimal>(reader["SHAN8"], 0);
                        line.CustomerPO = Data.CastDbNull.To(reader["SDVR01"], string.Empty).Trim();
                        line.Reference2 = Data.CastDbNull.To(reader["SDVR02"], string.Empty).Trim();
                        line.ReasonCode = Data.CastDbNull.To(reader["SDRCD"], string.Empty).Trim();
                        line.PriorityCode = Data.CastDbNull.To(reader["SDURCD"], string.Empty).Trim();
                        line.SoldToCustomer = _customerMgr.GetCustomer(e1Session, soldToCustomerNum);
                        var shipToCustomerNum = Data.CastDbNull.To<decimal>(reader["SHSHAN"], 0);
                        line.ShipToCustomer = _customerMgr.GetCustomer(e1Session, shipToCustomerNum);
                        line.CustomerSequenceNum = Data.CastDbNull.To(reader["ATZ58CSQNO"], string.Empty).Trim();
                        line.GenericSequenceNum = Data.CastDbNull.To(reader["ATZ58GSQNO"], string.Empty).Trim();
                        line.ProductType = Data.CastDbNull.To(reader["ATZ58PT"], string.Empty).Trim();
                        line.CustomerPart2 = Data.CastDbNull.To(reader["ATZ58CP2"], string.Empty).Trim();
                        line.CustomerPart3 = Data.CastDbNull.To(reader["ATZ58CP3"], string.Empty).Trim();
                        line.CustomerPart4 = Data.CastDbNull.To(reader["ATZ58CP4"], string.Empty).Trim();
                        line.ShopFloorMsg1 = Data.CastDbNull.To(reader["ATZ5899009"], string.Empty).Trim();
                        line.ShopFloorMsg2 = Data.CastDbNull.To(reader["ATZ5899010"], string.Empty).Trim();
                        line.PickNotes = Data.CastDbNull.To(reader["ATZ5899011"], string.Empty).Trim();
                        line.PackNotes = Data.CastDbNull.To(reader["ATZ5899012"], string.Empty).Trim();
                        line.VolumeIndicator = Data.CastDbNull.To(reader["ATZ5840460"], string.Empty).Trim();
                        line.Crating = Data.CastDbNull.To(reader["ATZ58CRATE"], string.Empty).Trim();
                        line.CratingDesc = _udcMgr.GetUdcItem(e1Session, "57", "CR", line.Crating).Description1;
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }

                var ediMessages = _ediMessageMgr.GetEdiMessages(e1Session, line.OrderCompany, line.OrderNum, line.OrderType, line.OrderLineNum * 1000);
                if (ediMessages.Count > 0)
                {
                    line.EdiMessages = ediMessages;
                }
                var crate = _crateMgr.GetCrate(e1Session, line.OrderNum, line.OrderType, line.OrderCompany, line.OrderLineNum * 1000);
                if (crate != null)
                {
                    line.Crate = crate;
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return line;
        }

        public iDB2Command GetSalesOrderLine()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        SDDOCO,
                        SDDCTO,
                        SDLNID,
                        SDKCOO,
                        SDVR01,
                        SDVR02,
                        SDRCD,
                        SDURCD,
	                    SHAN8,
                        SHSHAN,
                        SHDRQT,
                        ATZ58CSQNO,                        
                        ATZ58GSQNO,
                        ATZ58PT,
                        ATZ58CP2,
                        ATZ58CP3,
                        ATZ58CP4,
                        ATZ5899009,
                        ATZ5899010,
                        ATZ5899011,
                        ATZ5899012,
                        ATZ5840460,
                        ATZ58WODC,
                        ATZ58WOTY,
                        ATZ58CRATE,
                        DRDL01
                    FROM 
	                    F4211
                        INNER JOIN F4201
                            ON  SHDOCO = SDDOCO AND
                                SHDCTO = SDDCTO AND
                                SHKCOO = SDKCOO
                        INNER JOIN F580032
                            ON  ATDOCO = SDDOCO AND
                                ATDCTO = SDDCTO AND
                                ATLNID = SDLNID AND
                                ATKCOO = SDKCOO
                        INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                            ON LEFT(DRSY,2) = '57' AND
                                DRRT = 'CR' AND
                                TRIM(DRKY) = ATZ58CRATE
                    WHERE 
                        SDDOCO = ? AND
                        SDDCTO = ? AND
                        SDKCOO = ? AND
                        SDLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("Company", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public RelatedSalesOrderLine BuildSalesOrderLine(BusinessSystemSession e1Session, decimal orderNum, string orderType, string company, decimal orderLineNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var line = new RelatedSalesOrderLine();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetSalesOrderLine")) ? e1Session.DbCommands["WorkOrderManager.GetSalesOrderLine"] : GetSalesOrderLine();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["OrderNum"].Value = orderNum;
                cmd.Parameters["OrderType"].Value = orderType;
                cmd.Parameters["Company"].Value = company;
                cmd.Parameters["OrderLineNum"].Value = orderLineNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        line.OrderNum = Data.CastDbNull.To<decimal>(reader["SDDOCO"], 0);
                        line.OrderType = Data.CastDbNull.To(reader["SDDCTO"], string.Empty).Trim();
                        line.OrderLineNum = Data.CastDbNull.To<decimal>(reader["SDLNID"], 0) / 1000;
                        line.OrderCompany = Data.CastDbNull.To(reader["SDKCOO"], string.Empty).Trim();
                        line.SoldToCustNum = Data.CastDbNull.To<decimal>(reader["SHAN8"], 0);
                        line.RequestedTime = Data.CastDbNull.To(reader["SHAN8"], 0);
                        line.CustomerPO = Data.CastDbNull.To(reader["SDVR01"], string.Empty).Trim();
                        line.Reference2 = Data.CastDbNull.To(reader["SDVR02"], string.Empty).Trim();
                        line.ReasonCode = Data.CastDbNull.To(reader["SDRCD"], string.Empty).Trim();
                        line.PriorityCode = Data.CastDbNull.To(reader["SDURCD"], string.Empty).Trim();
                        line.ShipToCustNum = Data.CastDbNull.To<decimal>(reader["SHSHAN"], 0);
                        line.CustomerSequenceNum = Data.CastDbNull.To(reader["ATZ58CSQNO"], string.Empty).Trim();
                        line.GenericSequenceNum = Data.CastDbNull.To(reader["ATZ58GSQNO"], string.Empty).Trim();
                        line.ProductType = Data.CastDbNull.To(reader["ATZ58PT"], string.Empty).Trim();
                        line.CustomerPart2 = Data.CastDbNull.To(reader["ATZ58CP2"], string.Empty).Trim();
                        line.CustomerPart3 = Data.CastDbNull.To(reader["ATZ58CP3"], string.Empty).Trim();
                        line.CustomerPart4 = Data.CastDbNull.To(reader["ATZ58CP4"], string.Empty).Trim();
                        line.ShopFloorMsg1 = Data.CastDbNull.To(reader["ATZ5899009"], string.Empty).Trim();
                        line.ShopFloorMsg2 = Data.CastDbNull.To(reader["ATZ5899010"], string.Empty).Trim();
                        line.PickNotes = Data.CastDbNull.To(reader["ATZ5899011"], string.Empty).Trim();
                        line.PackNotes = Data.CastDbNull.To(reader["ATZ5899012"], string.Empty).Trim();
                        line.VolumeIndicator = Data.CastDbNull.To(reader["ATZ5840460"], string.Empty).Trim();
                        line.Crating = Data.CastDbNull.To(reader["ATZ58CRATE"], string.Empty).Trim();
                        line.CratingDesc = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return line;
        }

        /*********************************************************************************
         * Related Sales Order Line
         *******************************************************************************/
        public iDB2Command GetRelatedSalesOrderLine()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        RODOCO ,
                        RODCTO ,
                        ROKCOO ,
                        ROLNID ,
                        ROZ58L1CLN ,
                        ROMCU ,
                        ROEDTR,
                        ROUORG,
                        SDRCD,
                        SDURCD,
                        UD2.DRDL01,
                        SDAN8,
                        SDSHAN,
                        SDITM,
                        SDDRQJ,
                        SDVR01,
                        SDVR02,
                        ATZ58CSQNO,                        
                        ATZ58GSQNO,
                        ATZ58PT,
                        ATZ58CP2,
                        ATZ58CP3,
                        ATZ58CP4,
                        ATZ5899009,
                        ATZ5899010,
                        ATZ5899011,
                        ATZ5899012,
                        ATZ58WODC,
                        ATZ58WOTY,
                        ATZ5840460,
                        ATZ58CRATE,
                        ATZ58FLIP,
                        ATZ58TBAD,
                        ATZ58SCAD,
                        ATZ58PVAD,
                        IBSRP4,
                        UD1.DRDL01 AS SRP4DL01,   
                        UD2.DRDL01 AS DRDL0142RC,   
                        UD3.DRDL01 AS DRDL0157CR
                    FROM 
	                    F580400
                        LEFT OUTER JOIN F4211
                        ON RODOCO = SDDOCO AND
                            RODCTO = SDDCTO AND
                            ROLNID = SDLNID AND
                            ROKCOO = SDKCOO
                        LEFT OUTER JOIN F4102
                        ON  IBMCU = SDMCU AND
                            IBITM = SDITM
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                        ON  UD1.DRSY = '41' AND
                            UD1.DRRT = 'S4' AND
                            TRIM(UD1.DRKY) = TRIM(IBSRP4)
                        LEFT OUTER JOIN F580032
                        ON RODOCO = ATDOCO AND
                            RODCTO = ATDCTO AND
                            ROLNID = ATLNID AND
                            ROKCOO = ATKCOO
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD2
                        ON  UD2.DRSY = '42' AND
                            UD2.DRRT = 'RC' AND
                            TRIM(UD2.DRKY) = SDRCD
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD3
                        ON LEFT(UD3.DRSY,2) = '57' AND
                            UD3.DRRT = 'CR' AND
                            TRIM(UD3.DRKY) = ATZ58CRATE
                    WHERE
                        RORKCO = ? AND 
                        RODOC1 = ? AND
                        RORCTO = ? AND
                        RORLLN = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("Company", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 7;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public RelatedSalesOrderLine GetRelatedSalesOrderLine(BusinessSystemSession e1Session, decimal orderNum, string orderType, string company, decimal orderLineNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var line = new RelatedSalesOrderLine();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("WorkOrderManager.GetRelatedSalesOrderLine")) ? e1Session.DbCommands["WorkOrderManager.GetRelatedSalesOrderLine"] : GetRelatedSalesOrderLine();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["OrderNum"].Value = orderNum;
                cmd.Parameters["OrderType"].Value = orderType;
                cmd.Parameters["Company"].Value = company;
                cmd.Parameters["OrderLineNum"].Value = orderLineNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        line.OrderNum = Data.CastDbNull.To<decimal>(reader["SDDOCO"], 0);
                        line.OrderType = Data.CastDbNull.To(reader["SDDCTO"], string.Empty).Trim();
                        line.OrderLineNum = Data.CastDbNull.To<decimal>(reader["SDLNID"], 0) / 1000;
                        line.OrderCompany = Data.CastDbNull.To(reader["SDKCOO"], string.Empty).Trim();
                        var soldToCustomerNum = Data.CastDbNull.To<decimal>(reader["SHAN8"], 0);
                        line.CustomerPO = Data.CastDbNull.To(reader["SDVR01"], string.Empty).Trim();
                        line.Reference2 = Data.CastDbNull.To(reader["SDVR02"], string.Empty).Trim();
                        line.ReasonCode = Data.CastDbNull.To(reader["SDRCD"], string.Empty).Trim();
                        line.PriorityCode = Data.CastDbNull.To(reader["SDURCD"], string.Empty).Trim();
                        line.SoldToCustomer = _customerMgr.GetCustomer(e1Session, soldToCustomerNum);
                        var shipToCustomerNum = Data.CastDbNull.To<decimal>(reader["SHSHAN"], 0);
                        line.ShipToCustomer = _customerMgr.GetCustomer(e1Session, shipToCustomerNum);
                        line.CustomerSequenceNum = Data.CastDbNull.To(reader["ATZ58CSQNO"], string.Empty).Trim();
                        line.GenericSequenceNum = Data.CastDbNull.To(reader["ATZ58GSQNO"], string.Empty).Trim();
                        line.ProductType = Data.CastDbNull.To(reader["ATZ58PT"], string.Empty).Trim();
                        line.CustomerPart2 = Data.CastDbNull.To(reader["ATZ58CP2"], string.Empty).Trim();
                        line.CustomerPart3 = Data.CastDbNull.To(reader["ATZ58CP3"], string.Empty).Trim();
                        line.CustomerPart4 = Data.CastDbNull.To(reader["ATZ58CP4"], string.Empty).Trim();
                        line.ShopFloorMsg1 = Data.CastDbNull.To(reader["ATZ5899009"], string.Empty).Trim();
                        line.ShopFloorMsg2 = Data.CastDbNull.To(reader["ATZ5899010"], string.Empty).Trim();
                        line.PickNotes = Data.CastDbNull.To(reader["ATZ5899011"], string.Empty).Trim();
                        line.PackNotes = Data.CastDbNull.To(reader["ATZ5899012"], string.Empty).Trim();
                        line.VolumeIndicator = Data.CastDbNull.To(reader["ATZ5840460"], string.Empty).Trim();
                        line.Crating = Data.CastDbNull.To(reader["ATZ58CRATE"], string.Empty).Trim();
                        line.CratingDesc = Data.CastDbNull.To(reader["DRDL0157CR"], string.Empty).Trim(); // _udcMgr.GetUdcItem(e1Session, "57", "CR", line.Crating).Description1;
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }

                var ediMessages = _ediMessageMgr.GetEdiMessages(e1Session, line.OrderCompany, line.OrderNum, line.OrderType, line.OrderLineNum * 1000);
                if (ediMessages.Count > 0)
                {
                    line.EdiMessages = ediMessages;
                }
                var crate = _crateMgr.GetCrate(e1Session, line.OrderNum, line.OrderType, line.OrderCompany, line.OrderLineNum * 1000);
                if (crate != null)
                {
                    line.Crate = crate;
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return line;
        }

        public static iDB2Command GetE1CompletedWorkOrders(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        COUNT(WADOCO) AS RECS
                    FROM 
                        F4801
                    WHERE 
                        WASRST > '92' AND                         
                    {filterString}"
            };
            return cmd;
        }

    }
}
