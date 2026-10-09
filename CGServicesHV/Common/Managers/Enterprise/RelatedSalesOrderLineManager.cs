using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class RelatedSalesOrderLineManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CustomerManager _custMgr;
        private readonly EdiMessageManager _ediMessageMgr;
        private readonly CrateManager _crateMgr;

        public RelatedSalesOrderLineManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GetRelatedSalesOrderLine", GetRelatedSalesOrderLine()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
            //_itemMgr = new ItemManager(e1Session);
            _custMgr = new CustomerManager(e1Session);
            _ediMessageMgr = new EdiMessageManager(e1Session);
            _crateMgr = new CrateManager(e1Session);
        }

        public iDB2Command GetRelatedSalesOrderLine()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = string.Format(@"
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
                        UD1.DRDL01 AS SRP4DL01   
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
                        LEFT OUTER JOIN {0}.F0005 UD1
                        ON  UD1.DRSY = '41' AND
                            UD1.DRRT = 'S4' AND
                            TRIM(UD1.DRKY) = TRIM(IBSRP4)
                        LEFT OUTER JOIN F580032
                        ON RODOCO = ATDOCO AND
                            RODCTO = ATDCTO AND
                            ROLNID = ATLNID AND
                            ROKCOO = ATKCOO
                        LEFT OUTER JOIN {0}.F0005 UD2
                        ON  UD2.DRSY = '42' AND
                            UD2.DRRT = 'RC' AND
                            TRIM(UD2.DRKY) = SDRCD
                    WHERE
                        RODOCO > 0 AND
                        RORKCO = ? AND 
                        RODOC1 = ? AND
                        RORCTO = ? AND
                        RORLLN = ?",
                ConfigurationManager.AppSettings["ControlLib"])
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

        public RelatedSalesOrderLine GetRelatedSalesOrderLine(BusinessSystemSession e1Session, string orderCompany, decimal orderNum, string orderType, decimal orderLine)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var line = new RelatedSalesOrderLine();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetRelatedSalesOrderLine")) ? e1Session.DbCommands["GetRelatedSalesOrderLine"] : GetRelatedSalesOrderLine();
                cmd.Parameters[0].Value = orderCompany;
                cmd.Parameters[1].Value = orderNum;
                cmd.Parameters[2].Value = orderType;
                cmd.Parameters[3].Value = orderLine;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        line.OrderCompany = Data.CastDbNull.To(reader["ROKCOO"], string.Empty).Trim();
                        line.PlantNum = Data.CastDbNull.To(reader["ROMCU"], string.Empty).Trim();
                        line.OrderNum = Data.CastDbNull.To<decimal>(reader["RODOCO"], 0);
                        line.OrderType = Data.CastDbNull.To(reader["RODCTO"], string.Empty).Trim();
                        line.OrderLineNum = Data.CastDbNull.To<decimal>(reader["ROLNID"], 0) / 1000;
                        line.WorkOrderNum = Data.CastDbNull.To<decimal>(reader["ATZ58WODC"], 0);
                        line.CustomerLiteNum = Data.CastDbNull.To<decimal>(reader["ROZ58L1CLN"], 0);
                        line.TransactionType = Data.CastDbNull.To(reader["ROEDTR"], string.Empty).Trim();
                        line.ItemNum = Data.CastDbNull.To<decimal>(reader["SDITM"], 0);
                        line.CustomerPO = Data.CastDbNull.To(reader["SDVR01"], string.Empty).Trim();
                        line.UnitType = Data.CastDbNull.To(reader["IBSRP4"], string.Empty).Trim();
                        line.UnitTypeDesc = Data.CastDbNull.To(reader["SRP4DL01"], string.Empty).Trim();
                        var requestedDateId = Data.CastDbNull.To<decimal>(reader["SDDRQJ"], 0);
                        line.RequestedDate = Utilities.Common.CastToDateTime(requestedDateId);
                        //line.UnitsQuantity = Data.CastDbNull.To<Decimal>(reader["ROUORG"], 0) / 1000;
                        //line.ReasonCode = Data.CastDbNull.To<String>(reader["SDRCD"], String.Empty).Trim();
                        //line.ReasonCodeDesc = Data.CastDbNull.To<String>(reader["DRDL01"], String.Empty).Trim();
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
                        var productionTranMgr = new ProductionTransactionManager();
                        line.ProductionTransactions = productionTranMgr.GetProductionTransactionRejectsByWorkOrder(e1Session, line.PlantNum, line.WorkOrderNum);
                        //var customerMgr = new CustomerManager();
                        var customerNum = Data.CastDbNull.To<decimal>(reader["SDAN8"], 0);
                        line.SoldToCustomer = _custMgr.GetCustomer(e1Session, customerNum, true);
                        var customerShipToNum = Data.CastDbNull.To<decimal>(reader["SDSHAN"], 0);
                        line.ShipToCustomer = _custMgr.GetCustomer(e1Session, customerShipToNum, true);
                        line.TubesAdded = (Data.CastDbNull.To(reader["ATZ58TBAD"], string.Empty).Trim() == "Y");
                        line.ScoringAdded = (Data.CastDbNull.To(reader["ATZ58SCAD"], string.Empty).Trim() == "Y");
                        line.PreserveAdded = (Data.CastDbNull.To(reader["ATZ58PVAD"], string.Empty).Trim() == "Y");
                        line.CrateAdded = (Data.CastDbNull.To(reader["ATZ58CRATE"], string.Empty).Trim() != "N");
                        line.Flippable = (Data.CastDbNull.To(reader["ATZ58FLIP"], string.Empty).Trim() == "Y");
                        //var itemNum = Data.CastDbNull.To<Decimal>(reader["SDITM"], 0);
                        //line.CustomerItemReference = customerMgr.GetCustomerItemReference(e1Session, customerNum, itemNum );
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
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
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return line;
        }
    }
}


