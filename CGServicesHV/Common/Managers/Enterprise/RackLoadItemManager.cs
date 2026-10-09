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
    public class RackLoadItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackLoadItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackItemManager.GetFloatRackLoadItems", GetFloatRackLoadItems()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public RackLoadItems GetFloatRackLoadItems(BusinessSystemSession session, string plantCode)
        {
            var rackLoadItems = new RackLoadItems();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                var cmd = new iDB2Command();
                cmd = (session.DbCommands.ContainsKey("RackManager.GetFloatRackLoadItems")) ? session.DbCommands["RackManager.GetFloatRackLoadItems"] : GetFloatRackLoadItems();
                cmd.Parameters[0].Value = plant.AddressNum;
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackLoadItem = new RackLoadItem();
                        rackLoadItem.RackId = Data.CastDbNull.To<Decimal>(reader["RHZ58RID"], 0);
                        rackLoadItem.SerialNum = Data.CastDbNull.To<String>(reader["RDZ58SN"], String.Empty).Trim();
                        rackLoadItem.PlantNum = Data.CastDbNull.To<String>(reader["RHMCU"], String.Empty).Trim();
                        rackLoadItem.Location = Data.CastDbNull.To<String>(reader["RHMCU"], String.Empty).Trim();
                        rackLoadItem.RackType = Data.CastDbNull.To<String>(reader["RHZ58RT"], String.Empty).Trim();
                        rackLoadItem.RackTypeDesc = Data.CastDbNull.To<String>(reader["F0005_57_RK_DRDL01"], String.Empty).Trim();
                        rackLoadItem.RackStatus = Data.CastDbNull.To<String>(reader["RHZ58HST"], String.Empty).Trim();
                        rackLoadItem.RackStatusDesc = Data.CastDbNull.To<String>(reader["F0005_58_ST_DRDL01"], String.Empty).Trim();
                        var rackStatusDateId = Data.CastDbNull.To<Decimal>(reader["RHZ58RSDT"], 0);
                        rackLoadItem.RackStatusDate = Utilities.Common.CastToDateTime(rackStatusDateId, 0);
                        rackLoadItem.LoadNum = Data.CastDbNull.To<decimal>(reader["RHPSN"], 0);
                        rackLoadItem.SuggestedLoadNum = Data.CastDbNull.To<decimal>(reader["RHPSN"], 0);
                        rackLoadItem.RackUserReservedCode = Data.CastDbNull.To<String>(reader["RHURCD"], String.Empty).Trim();
                        rackLoadItem.AssetID = Data.CastDbNull.To<String>(reader["RHURRF"], String.Empty).Trim();
                        var rackUpdateDateId = Data.CastDbNull.To<Decimal>(reader["RHUPMJ"], 0);
                        var rackUpdateTimeId = Data.CastDbNull.To<Decimal>(reader["RHTDAY"], 0);
                        rackLoadItem.RackUpdateDate = Utilities.Common.CastToDateTime(rackUpdateDateId, rackUpdateTimeId);
                        rackLoadItem.ScheduleNum = Data.CastDbNull.To<Decimal>(reader["RHZ58SCN"], 0);
                        rackLoadItem.SoldToNum = Data.CastDbNull.To<Decimal>(reader["RHAN8"], 0);
                        rackLoadItem.ItemNum = Data.CastDbNull.To<Decimal>(reader["RDITM"], 0);
                        var transactionDateId = Data.CastDbNull.To<Decimal>(reader["RDTRDJ"], 0);
                        rackLoadItem.TransactionDate = Utilities.Common.CastToDateTime(transactionDateId, 0);
                        rackLoadItem.Quantity = Data.CastDbNull.To<decimal>(reader["RDPQOH"], 0) / 1000;
                        rackLoadItem.SalesOrderNum = Data.CastDbNull.To<Decimal>(reader["RDDOCO"], 0);
                        rackLoadItem.SalesOrderType = Data.CastDbNull.To<String>(reader["RDDCTO"], String.Empty).Trim();
                        rackLoadItem.SalesOrderLineNum = Data.CastDbNull.To<Decimal>(reader["RDLNID"], 0) / 1000;
                        rackLoadItem.CompanyNum = Data.CastDbNull.To<String>(reader["RDKCOO"], String.Empty).Trim();
                        rackLoadItem.LotNum = Data.CastDbNull.To<string>(reader["RDLOTN"], String.Empty).Trim();
                        rackLoadItem.WorkOrderNum = Data.CastDbNull.To<Decimal>(reader["RDDOC1"], 0);
                        rackLoadItem.PurchaseOrderNum = Data.CastDbNull.To<Decimal>(reader["RDDOC2"], 0);
                        rackLoadItem.PurchaseOrderType = Data.CastDbNull.To<String>(reader["RDXCTO"], String.Empty).Trim();
                        rackLoadItem.PurchaseOrderLineNum = Data.CastDbNull.To<Decimal>(reader["RDLNIC"], 0) / 1000;
                        rackLoadItem.CompletedQuantity = Data.CastDbNull.To<decimal>(reader["RDZ58CQTY"], 0) / 1000;
                        var manufacturedDateId = Data.CastDbNull.To<Decimal>(reader["RDZ58MDT"], 0);
                        rackLoadItem.ManufacturedDate = Utilities.Common.CastToDateTime(manufacturedDateId, 0);
                        var productionDateId = Data.CastDbNull.To<Decimal>(reader["RDZ58PDT"], 0);
                        rackLoadItem.ProductionDate = Utilities.Common.CastToDateTime(productionDateId, 0);
                        rackLoadItem.Description1 = Data.CastDbNull.To<String>(reader["RDDSC1"], String.Empty).Trim();
                        rackLoadItem.Description2 = Data.CastDbNull.To<String>(reader["RDDSC2"], String.Empty).Trim();
                        rackLoadItem.LoadStatus = Data.CastDbNull.To<String>(reader["LHZ58LDS"], String.Empty).Trim();
                        rackLoadItem.LoadStatusDesc = Data.CastDbNull.To<String>(reader["F0005_58_LS_DRDL01"], String.Empty).Trim();
                        var reqDateId = Data.CastDbNull.To<Decimal>(reader["LHDRQJ"], 0);
                        var reqTimeId = Data.CastDbNull.To<Decimal>(reader["LHDRQT"], 0);
                        rackLoadItem.RequestedDate = Utilities.Common.CastToDateTime(reqDateId, reqTimeId);
                        var loadedDateId = Data.CastDbNull.To<Decimal>(reader["LHLOAD"], 0);
                        var loadedTimeId = Data.CastDbNull.To<Decimal>(reader["LHTMLS"], 0);
                        rackLoadItem.LoadedDate = Utilities.Common.CastToDateTime(loadedDateId, loadedTimeId);
                        rackLoadItem.LoadComment1 = Data.CastDbNull.To<String>(reader["LHZ58CMT1"], String.Empty).Trim();
                        rackLoadItem.LoadComment2 = Data.CastDbNull.To<String>(reader["LHZ58CMT2"], String.Empty).Trim();
                        rackLoadItem.LoadComment3 = Data.CastDbNull.To<String>(reader["LHZ58CMT3"], String.Empty).Trim();
                        rackLoadItem.Carrier = Data.CastDbNull.To<decimal>(reader["LHCARS"], 0);
                        rackLoadItem.TrailerNumber = Data.CastDbNull.To<string>(reader["LHZ58TN"], String.Empty).Trim();
                        var loadConfirmDateId = Data.CastDbNull.To<Decimal>(reader["LHLDDT"], 0);
                        var loadConfirmTimeId = Data.CastDbNull.To<Decimal>(reader["LHZ58TMLC"], 0);
                        rackLoadItem.LoadConfirmDate = Utilities.Common.CastToDateTime(loadConfirmDateId, loadConfirmTimeId);
                        var actualShipDateId = Data.CastDbNull.To<Decimal>(reader["LHADDJ"], 0);
                        var actualShipTimeId = Data.CastDbNull.To<Decimal>(reader["LHADTM"], 0);
                        rackLoadItem.ActualShipDate = Utilities.Common.CastToDateTime(actualShipDateId, actualShipTimeId);
                        rackLoadItem.BillOfLadingNumber = Data.CastDbNull.To<decimal>(reader["LHBOLN"], 0);
                        rackLoadItem.MBOL = Data.CastDbNull.To<decimal>(reader["LHZ58MBOL"], 0);
                        var loadUpdateDateId = Data.CastDbNull.To<Decimal>(reader["LHUPMJ"], 0);
                        var loadUpdateTimeId = Data.CastDbNull.To<Decimal>(reader["LHTDAY"], 0);
                        rackLoadItem.LoadUpdateDate = Utilities.Common.CastToDateTime(rackUpdateDateId, rackUpdateTimeId);

                        rackLoadItems.Add(rackLoadItem);
                    }
                    reader.Close();
                    Logger.Trace($"Rows: {rackLoadItems.Count}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                //throw new RackLoadItemManager(e.ToString());
            }
            return rackLoadItems;
        }

        /******************************************************************
        * Rack Items - Prepared
        ******************************************************************/
        public iDB2Command GetFloatRackLoadItems()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
	                RHZ58RID,
	                RDZ58SN,
	                RHMCU,
	                RHLOCN,
	                RHZ58RT,
	                F0005_57_RK.DRDL01 F0005_57_RK_DRDL01,
	                RHZ58HST,
	                F0005_58_ST.DRDL01 F0005_58_ST_DRDL01,
	                RHZ58RSDT,
	                RHPSN,
	                RHZ58SLDN,
	                RHURCD,
	                RHURRF,
	                RHUPMJ,
	                RHTDAY,
	                RHZ58SCN,
	                RHAN8,
	                RDITM,
	                RDTRDJ,
	                RDPQOH,
	                RDDOCO,
	                RDDCTO,
	                RDLNID,
	                RDKCOO,
                    RDLOTN,
	                RDDOC1,
	                RDXDCTO,
	                RDDOC2,
	                RDPKCO,
	                RDXCTO,
                    RDLNIC,
                    RDZ58CQTY,
	                RDZ58MDT,
                    RDZ58PDT,
	                RDDSC1,
	                RDDSC2,
	                LHZ58LDS,
	                F0005_58_LS.DRDL01 F0005_58_LS_DRDL01,
	                LHDRQJ,
	                LHDRQT,
	                LHLOAD,
	                LHTMLS,
	                LHZ58CMT1,
	                LHZ58CMT2,
	                LHZ58CMT3,
	                LHCARS,
	                LHZ58TN,
	                LHLDDT,
	                LHZ58TMLC,
	                LHADDJ,
	                LHADTM,
	                LHBOLN,
	                LHZ58MBOL,
	                LHUPMJ,
	                LHTDAY
                FROM  F580055 
                LEFT OUTER JOIN F580056
	                ON RDZ58RID = RHZ58RID
                LEFT OUTER JOIN F580150
	                ON LHLDNM = RHPSN
                LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 F0005_57_RK
	                ON TRIM(F0005_57_RK.DRKY) = RHZ58RT
	                AND F0005_57_RK.DRSY = '57'
	                AND F0005_57_RK.DRRT = 'RK'
                LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 F0005_58_ST
	                ON TRIM(F0005_58_ST.DRKY) = RHZ58HST
	                AND F0005_58_ST.DRSY = '58'
	                AND F0005_58_ST.DRRT = 'ST'
                LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 F0005_58_LS
	                ON TRIM(F0005_58_LS.DRKY) = LHZ58LDS
	                AND F0005_58_LS.DRSY = '58'
	                AND F0005_58_LS.DRRT = 'LS'
                WHERE  
                    RHMCU IN ('          07','          09','          12','          18','          23','          27','          30','          47','          49','          93')
                    AND RHZ58HST IN ('1','2','3','4','5','6','7','8','9')
                    AND RHAN8 = ?
                ORDER BY RHUPMJ, RHTDAY
            ";
            cmd.Parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AN8"].Precision = 8;
            cmd.Parameters["AN8"].Scale = 0;
            return cmd;
        }
    }
}
