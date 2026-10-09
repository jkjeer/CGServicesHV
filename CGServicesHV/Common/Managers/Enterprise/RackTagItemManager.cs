using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class RackTagItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ItemAttributeManager _itemAttributeMgr;
        private RelatedSalesOrderLineManager _relatedSalesOrderLineMgr;


        public RackTagItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackTagItemManager.GetRackTagItems", GetRackTagItems()},
                {"RackTagItemManager.GetRackTagItemsByUnitId", GetRackTagItemsByUnitId()},
                {"RackTagItemManager.GetEdiMessages", GetEdiMessages()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
            _itemAttributeMgr = new ItemAttributeManager(e1Session);
            _relatedSalesOrderLineMgr = new RelatedSalesOrderLineManager(e1Session);
        }

        public iDB2Command GetRackTagItems()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT
                        RHZ58RID,
                        RHMCU,
                        RHAN8,
                        RHZ58HST,
                        RHZ58RSDT,
                        AB1.ABALPH AS AN8ALPH,
                        RHSHAN,
                        AB2.ABALPH AS SHANALPH,  
                        RHZ58RT,
                        UD1.DRDL01 AS RHZ58RT_DESC,
                        RHZ58BOX,
                        RHZ58GWTT,
                        RHZ58RTWT,
                        RHZ58SCN,
                        RHLOCN,
                        ATZ5899003,
                        ATZ58CP2,
                        ATZ58CP3,
                        ATZ58CP4,
                        ATZ58CSQNO,
                        ATZ58GSQNO,
                        ATZ5899009,
                        ATZ5899010,
                        ATZ5899011,
                        ATZ5899012,
                        ATZ58PT,
                        SHDRQJ,
                        SDDRQJ,
                        SDUORG,
                        SDVR01,
                        S1Z58SCDDJ,
                        S1Z58SCTYP,
                        COALESCE(BIZ58IGLN,'0') AS BIZ58IGLN,
                        RDKCOO,
                        RDDOCO,
                        RDDCTO,
                        RDLNID,
                        RDDOC1,
                        RDXDCTO,
                        RDDOC2,
                        RDPKCO,
                        RDXCTO,
                        RDLNIC,
                        RDZ58SD,
                        RDZ58ROW,
                        RDZ58STK,                
                        MIN(BIZ58L1R) AS BIZ58L1R,
                        MIN(BIZ58L1SLN) AS BIZ58L1SLN,
                        MIN(BIZ58L2R) AS BIZ58L2R,
                        MIN(BIZ58L2SLN) AS BIZ58L2SLN,
                        MIN(BIZ58L3R) AS BIZ58L3R,
                        MIN(BIZ58L3SLN) AS BIZ58L3SLN, 
                        MIN(RDZ58SN) AS MIN_RDZ58SN,
                        MAX(RDZ58SN) AS MAX_RDZ58SN,
                        MAX(RDZ58BXRO) AS RDZ58BXRO,
                        MIN(RDZ58PS) AS RDZ58PS,
                        MIN(RDZ58SL) AS RDZ58SL,
                        MAX(RDITM) AS RDITM,
                        COUNT(*) AS RDPQOH            
                    FROM
                        F580055
                        INNER JOIN F580056
                        ON RDZ58RID = RHZ58RID
                        LEFT OUTER JOIN F4201
                        ON  SHDOCO = RDDOCO AND
                            SHDCTO = RDDCTO AND
                            SHKCOO = RDKCOO
                        LEFT OUTER JOIN F4211
                        ON  SDDOCO = RDDOCO AND
                            SDLNID = RDLNID AND
                            SDDCTO = RDDCTO AND
                            SDKCOO = RDKCOO
                        LEFT OUTER JOIN F580032
                        ON  ATDOCO = RDDOCO AND
                            ATLNID = RDLNID AND
                            ATDCTO = RDDCTO AND
                            ATKCOO = RDKCOO AND
                            ATDOCO > 0
                        LEFT OUTER JOIN F5834A11
                        ON  S5MCU = RDMCU AND
                            S5DOCO = RDDOC1
                        LEFT OUTER JOIN F5834A01
                        ON  S1MCU = S5MCU AND
                            S1Z58SCN = S5Z58SCN AND
                            S1Z58SCD = S5Z58SCD
                        LEFT OUTER JOIN F0101 AB1
                        ON  AB1.ABAN8 = RHAN8
                        LEFT OUTER JOIN F0101 AB2
                        ON  AB2.ABAN8 = RHSHAN
                        LEFT OUTER JOIN F580200 
                        ON  BIMCU = RDMCU AND
                            BIDOCO = RDDOC1 AND
                            BIZ58UNID = RDZ58SN AND
                            BIEDSP != 'D'
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                        ON  UD1.DRSY = '57' AND
                            UD1.DRRT = 'RK' AND
                            TRIM(UD1.DRKY) = TRIM(RHZ58RT)
                    WHERE
                        RHZ58RID = ? 
                        AND BIEDSP = 'Y'
                    GROUP BY
                        RHZ58RID,
                        RHMCU,
                        RHAN8,
                        RHZ58HST,
                        RHZ58RSDT,
                        AB1.ABALPH,
                        RHSHAN,
                        AB2.ABALPH,  
                        RHZ58RT,
                        UD1.DRDL01,
                        RHZ58BOX,
                        RHZ58GWTT,
                        RHZ58RTWT,
                        RHZ58SCN,
                        RHLOCN,
                        ATZ5899003,
                        ATZ58CP2,
                        ATZ58CP3,
                        ATZ58CP4,
                        ATZ58CSQNO,
                        ATZ58GSQNO,
                        ATZ5899009,
                        ATZ5899010,
                        ATZ5899011,
                        ATZ5899012,
                        ATZ58PT,
                        SHDRQJ,
                        SDDRQJ,
                        SDUORG,
                        SDVR01,
                        S1Z58SCDDJ,
                        S1Z58SCTYP,
                        COALESCE(BIZ58IGLN,'0'),
                        RDKCOO,
                        RDDOCO,
                        RDDCTO,
                        RDLNID,
                        RDKCOO,
                        RDDOC1,
                        RDXDCTO,
                        RDDOC2,
                        RDPKCO,
                        RDXCTO,
                        RDLNIC,
                        RDZ58SD,
                        RDZ58ROW,
                        RDZ58STK
                    ORDER BY
                        RDZ58SD,
                        RDZ58ROW,
                        RDZ58STK,
                        RDZ58SL,
                        RDZ58PS"
            };
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 15;
            cmd.Parameters["RackId"].Scale = 0;
            return cmd;
        }

    public iDB2Command GetRackTagItemsByUnitId()
    {
        var cmd = new iDB2Command
        {
            CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT DISTINCT
                        RHZ58RID,
                        RHMCU,
                        RHAN8,
                        RHZ58HST,
                        RHZ58RSDT,
                        AB1.ABALPH AS AN8ALPH,
                        RHSHAN,
                        AB2.ABALPH AS SHANALPH,  
                        RHZ58RT,
                        UD1.DRDL01 AS RHZ58RT_DESC,
                        RHZ58BOX,
                        RHZ58GWTT,
                        RHZ58RTWT,
                        RHZ58SCN,
                        RHLOCN,
                        ATZ5899003,
                        ATZ58CP2,
                        ATZ58CP3,
                        ATZ58CP4,
                        ATZ58CSQNO,
                        ATZ58GSQNO,
                        ATZ5899009,
                        ATZ5899010,
                        ATZ5899011,
                        ATZ5899012,
                        ATZ58PT,
                        SHDRQJ,
                        SDDRQJ,
                        SDUORG,
                        SDVR01,
                        S1Z58SCDDJ,
                        S1Z58SCTYP,
                        COALESCE(BIZ58IGLN,'0') AS BIZ58IGLN,
                        RDKCOO,
                        RDDOCO,
                        RDDCTO,
                        RDLNID,
                        RDDOC1,
                        RDXDCTO,
                        RDDOC2,
                        RDPKCO,
                        RDXCTO,
                        RDLNIC,
                        RDZ58SD,
                        RDZ58ROW,
                        RDZ58STK,
                        RDZ58SN AS MIN_RDZ58SN,
                        RDZ58SN AS MAX_RDZ58SN,
                        RDZ58BXRO AS RDZ58BXRO,
                        RDZ58PS AS RDZ58PS,
                        RDZ58SL AS RDZ58SL,
                        RDITM AS RDITM,
                        BIZ58L1R,
                        BIZ58L1SLN,
                        BIZ58L2R,
                        BIZ58L2SLN,
                        BIZ58L3R,
                        BIZ58L3SLN,               
                        1 AS RDPQOH            
                    FROM
                        F580055
                        INNER JOIN F580056
                        ON RDZ58RID = RHZ58RID
                        LEFT OUTER JOIN F4201
                        ON  SHDOCO = RDDOCO AND
                            SHDCTO = RDDCTO AND
                            SHKCOO = RDKCOO
                        LEFT OUTER JOIN F4211
                        ON  SDDOCO = RDDOCO AND
                            SDLNID = RDLNID AND
                            SDDCTO = RDDCTO AND
                            SDKCOO = RDKCOO
                        LEFT OUTER JOIN F580032
                        ON  ATDOCO = RDDOCO AND
                            ATLNID = RDLNID AND
                            ATDCTO = RDDCTO AND
                            ATKCOO = RDKCOO AND
                            ATDOCO > 0
                        LEFT OUTER JOIN F5834A11
                        ON  S5MCU = RDMCU AND
                            S5DOCO = RDDOC1
                        LEFT OUTER JOIN F5834A01
                        ON  S1MCU = S5MCU AND
                            S1Z58SCN = S5Z58SCN AND
                            S1Z58SCD = S5Z58SCD
                        LEFT OUTER JOIN F0101 AB1
                        ON  AB1.ABAN8 = RHAN8
                        LEFT OUTER JOIN F0101 AB2
                        ON  AB2.ABAN8 = RHSHAN
                        LEFT OUTER JOIN F580200 
                        ON  BIMCU = RDMCU AND
                            BIDOCO = RDDOC1 AND
                            BIZ58UNID = RDZ58SN AND
                            BIEDSP != 'D'
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                        ON  UD1.DRSY = '57' AND
                            UD1.DRRT = 'RK' AND
                            TRIM(UD1.DRKY) = TRIM(RHZ58RT)
                    WHERE
                        RHZ58RID = ?
                    ORDER BY
                        RDZ58SD,
                        RDZ58ROW,
                        RDZ58STK,
                        RDZ58SL,
                        RDZ58PS"
            };
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 15;
            cmd.Parameters["RackId"].Scale = 0;
            return cmd;
        }

        /**************************************************************
         *  EDI Messages
         * ************************************************************/
        public iDB2Command GetEdiMessages()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        ZDEDTY,
                        ZDEDSQ,
                        ZDLINS,
                        ZDTXLN
                    FROM
                        F4715 
                    WHERE
                        ZDKCOO = ? AND
                        ZDDOCO = ? AND
                        ZDDCTO = ? AND
                        ZDLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 9;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public RackTagItems GetRackTagItems(BusinessSystemSession e1Session, decimal rackId, bool byUnitId)
        {
            var rackTags = new RackTagItems();
            var custNameMgr = new CustomerNameManager(e1Session);
            try
            {
                iDB2Command cmd;
                if (byUnitId)
                {
                    cmd = (e1Session.DbCommands.ContainsKey("RackTagItemManager.GetRackTagItemsByUnitId")) ? e1Session.DbCommands["RackTagItemManager.GetRackTagItemsByUnitId"] : GetRackTagItemsByUnitId(); 
                } else
                {
                    cmd = (e1Session.DbCommands.ContainsKey("RackTagItemManager.GetRackTagItems")) ? e1Session.DbCommands["RackTagItemManager.GetRackTagItems"] : GetRackTagItems();
                }
                cmd.Parameters[0].Value = rackId;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new RackTagItem
                        {
                            RackId = Data.CastDbNull.To<decimal>(reader["RHZ58RID"], 0),
                            CompanyNum = Data.CastDbNull.To(reader["RDKCOO"], string.Empty).Trim(),
                            PlantNum = Data.CastDbNull.To(reader["RHMCU"], string.Empty).Trim(),
                            SalesOrderNum = Data.CastDbNull.To<decimal>(reader["RDDOCO"], 0),
                            SalesOrderLineNum = Math.Round(Data.CastDbNull.To<decimal>(reader["RDLNID"], 0) / 1000, 3),
                            SalesOrderType = Data.CastDbNull.To(reader["RDDCTO"], string.Empty).Trim(),
                            WorkOrderNum = Data.CastDbNull.To<decimal>(reader["RDDOC1"], 0),
                            BeginSerialNum = Data.CastDbNull.To(reader["MIN_RDZ58SN"], string.Empty).Trim(),
                            EndSerialNum = Data.CastDbNull.To(reader["MAX_RDZ58SN"], string.Empty).Trim(),
                            ItemNum = Data.CastDbNull.To<decimal>(reader["RDITM"], 0),
                            SoldToNum = Data.CastDbNull.To<decimal>(reader["RHAN8"], 0),
                            SoldToName = Data.CastDbNull.To(reader["AN8ALPH"], string.Empty).Trim(),
                            ShipToNum = Data.CastDbNull.To<decimal>(reader["RHSHAN"], 0),
                            ShipToName = Data.CastDbNull.To(reader["SHANALPH"], string.Empty).Trim(),
                            CustPartNum1 = Data.CastDbNull.To(reader["ATZ58CP2"], string.Empty).Trim(),
                            CustPartNum2 = Data.CastDbNull.To(reader["ATZ58CP3"], string.Empty).Trim(),
                            CustPartNum3 = Data.CastDbNull.To(reader["ATZ58CP4"], string.Empty).Trim(),
                            CustSeqNum1 = Data.CastDbNull.To(reader["ATZ58CSQNO"], string.Empty).Trim(),
                            GenericSeqNum = Data.CastDbNull.To(reader["ATZ58GSQNO"], string.Empty).Trim(),
                            //IGLine = Data.CastDbNull.To(reader["BIZ58IGLN"], string.Empty).Trim(),
                            IGLine = "0",
                            RackType = Data.CastDbNull.To(reader["RHZ58RT"], string.Empty).Trim(),
                            RackTypeDesc = Data.CastDbNull.To(reader["RHZ58RT_DESC"], string.Empty).Trim(),
                            Location = Data.CastDbNull.To(reader["RHLOCN"], string.Empty).Trim(),
                            Status = Data.CastDbNull.To(reader["RHZ58HST"], string.Empty).Trim(),
                            StatusDate = Utilities.Common.ConvertE1Date(Data.CastDbNull.To<decimal>(reader["RHZ58RSDT"], 0)),
                            BoxNum = Data.CastDbNull.To<decimal>(reader["RHZ58BOX"], 0),
                            RackGlassWeight = Data.CastDbNull.To<decimal>(reader["RHZ58GWTT"], 0) / 100,
                            RackTareWeight = Data.CastDbNull.To<decimal>(reader["RHZ58RTWT"], 0) / 100,
                            ScheduleNum = Data.CastDbNull.To<decimal>(reader["RHZ58SCN"], 0),
                            ScheduleType = Data.CastDbNull.To(reader["S1Z58SCTYP"], string.Empty).Trim(),
                            Quantity = Data.CastDbNull.To(reader["RDPQOH"], 0),
                            CustomerPO = Data.CastDbNull.To(reader["SDVR01"], string.Empty).Trim(),
                            ShopFloorMessage1 = Data.CastDbNull.To(reader["ATZ5899009"], string.Empty).Trim(),
                            ShopFloorMessage2 = Data.CastDbNull.To(reader["ATZ5899010"], string.Empty).Trim(),
                            PickNotes = Data.CastDbNull.To(reader["ATZ5899011"], string.Empty).Trim(),
                            PackNotes = Data.CastDbNull.To(reader["ATZ5899012"], string.Empty).Trim(),
                            ProductType = Data.CastDbNull.To(reader["ATZ58PT"], string.Empty).Trim(),
                            CustomerDate = Utilities.Common.ConvertE1Date(Data.CastDbNull.To<decimal>(reader["SDDRQJ"], 0)),
                            DueDate = Utilities.Common.ConvertE1Date(Data.CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0)),
                            DueDateString = Utilities.Common.ConvertE1Date(Data.CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0)).ToString("MM/dd/yyyy"),
                            RackSide = Data.CastDbNull.To(reader["RDZ58SD"], string.Empty).Trim(),
                            RackPosition = Data.CastDbNull.To<decimal>(reader["RDZ58PS"], 0),
                            RackStack = Data.CastDbNull.To<decimal>(reader["RDZ58STK"], 0),
                            RackRow = Data.CastDbNull.To<decimal>(reader["RDZ58ROW"], 0),
                            RackSlot = Data.CastDbNull.To<decimal>(reader["RDZ58SL"], 0),
                            RackSlotPosition = (Data.CastDbNull.To<decimal>(reader["RDZ58PS"], 0) > 0) ? Data.CastDbNull.To<decimal>(reader["RDZ58PS"], 0) : Data.CastDbNull.To<decimal>(reader["RDZ58SL"], 0),
                            BoxRotated = Data.CastDbNull.To(reader["RDZ58BXRO"], string.Empty).Trim(),
                            Lite1Rack = Data.CastDbNull.To(reader["BIZ58L1R"], string.Empty).Trim(),
                            Lite1Slot = Data.CastDbNull.To<decimal>(reader["BIZ58L1SLN"], 0),
                            Lite2Rack = Data.CastDbNull.To(reader["BIZ58L2R"], string.Empty).Trim(),
                            Lite2Slot = Data.CastDbNull.To<decimal>(reader["BIZ58L2SLN"], 0),
                            Lite3Rack = Data.CastDbNull.To(reader["BIZ58L3R"], string.Empty).Trim(),
                            Lite3Slot = Data.CastDbNull.To<decimal>(reader["BIZ58L3SLN"], 0)
                        };
                        var rddoco = Data.CastDbNull.To<decimal>(reader["RDDOCO"], 0);
                        var rddoc1 = Data.CastDbNull.To<decimal>(reader["RDDOC1"], 0);
                        var rddoc2 = Data.CastDbNull.To<decimal>(reader["RDDOC2"], 0);
                        ItemAttributes segments;
                        // Item by Work
                        if (rddoc1 > 0)
                        {
                            segments = _itemAttributeMgr.GetItemAttributes(e1Session, 0, rddoc1, "WO", 0, "*");
                        }
                        else
                        {
                            // Item by Sale
                            if (rddoco > 0)
                            {
                                var rdkcoo = Data.CastDbNull.To(reader["RDKCOO"], string.Empty).Trim();
                                var rddcto = Data.CastDbNull.To(reader["RDDCTO"], string.Empty).Trim();
                                var rdlnid = Data.CastDbNull.To<decimal>(reader["RDLNID"], 0);
                                segments = _itemAttributeMgr.GetItemAttributes(e1Session, 0, rddoco, rddcto, rdlnid, rdkcoo);
                            }
                            else
                            {
                                // Item by Purchase    
                                if (rddoc2 > 0)
                                {
                                    var rdpkco = Data.CastDbNull.To(reader["RDPKCO"], string.Empty).Trim();
                                    var rdxcto = Data.CastDbNull.To(reader["RDXCTO"], string.Empty).Trim();
                                    var rdlnic = Math.Round(Data.CastDbNull.To<decimal>(reader["RDLNIC"], 0) / 1000, 3);
                                    segments = _itemAttributeMgr.GetItemAttributes(e1Session, 0, rddoc2, rdxcto, rdlnic, rdpkco);
                                }
                                else
                                {
                                    // Item by Item
                                    {
                                        segments = _itemAttributeMgr.GetItemAttributes(e1Session, rackTag.ItemNum);
                                    }
                                }
                            }
                        }

                        // Glass Type
                        rackTag.Lite1Thickness = segments.GetDecimalAttributeValue("Z58L1TK");
                        rackTag.Lite1Heat = segments.GetStringAttributeValue("Z58L1HT");
                        rackTag.Lite1FirstSrfCoat = segments.GetStringAttributeValue("Z58L11C");
                        rackTag.Lite1SecondSrfCoat = segments.GetStringAttributeValue("Z58L12C");
                        rackTag.Lite1Color = segments.GetStringAttributeDesc3("Z58L1CL", "CL");
                        rackTag.Lite1Pattern = segments.GetStringAttributeValue("Z58L1PX");
                        rackTag.Lite1LamiType = segments.GetStringAttributeValue("Z58L1LPT");
                        rackTag.Lite1LamiDesc = segments.GetStringAttributeDesc1("Z58L1LPT");
                        rackTag.Lite1LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L1LPT");
                        rackTag.Lite1VGroove = segments.GetStringAttributeDesc1("Z58L1VG");
                        rackTag.Lite1TempLogo = segments.GetStringAttributeValue("Z58L1TL");
                        rackTag.Lite1Routing = segments.GetStringAttributeDesc1("Z58L1RTG");
                        rackTag.Lite1EdgeWork = segments.GetStringAttributeDesc1("Z58L1EW");
                        rackTag.Lite1AnsiLogo = segments.GetStringAttributeValue("Z58L1ALT");

                        rackTag.Lite2Thickness = segments.GetDecimalAttributeValue("Z58L2TK");
                        rackTag.Lite2Heat = segments.GetStringAttributeValue("Z58L2HT");
                        rackTag.Lite2ThirdSrfCoat = segments.GetStringAttributeValue("Z58L23C");
                        rackTag.Lite2FourthSrfCoat = segments.GetStringAttributeValue("Z58L24C");
                        rackTag.Lite2Color = segments.GetStringAttributeDesc3("Z58L2CL", "CL");
                        rackTag.Lite2Pattern = segments.GetStringAttributeValue("Z58L2PX");
                        rackTag.Lite2LamiType = segments.GetStringAttributeValue("Z58L2LPT");
                        rackTag.Lite2LamiDesc = segments.GetStringAttributeDesc1("Z58L2LPT");
                        rackTag.Lite2LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L2LPT");
                        rackTag.Lite2VGroove = segments.GetStringAttributeDesc2("Z58L2VG");
                        rackTag.Lite2Routing = segments.GetStringAttributeDesc1("Z58L2RTG");

                        rackTag.Lite3Thickness = segments.GetDecimalAttributeValue("Z58L3TK");
                        rackTag.Lite3Heat = segments.GetStringAttributeValue("Z58L3HT");
                        rackTag.Lite3FifthSrfCoat = segments.GetStringAttributeValue("Z58L35C");
                        rackTag.Lite3SixthSrfCoat = segments.GetStringAttributeValue("Z58L36C");
                        rackTag.Lite3Color = segments.GetStringAttributeDesc3("Z58L3CL", "CL");
                        rackTag.Lite3Pattern = segments.GetStringAttributeValue("Z58L3PX");
                        rackTag.Lite3LamiType = segments.GetStringAttributeValue("Z58L3LPT");
                        rackTag.Lite3LamiDesc = segments.GetStringAttributeDesc1("Z58L3LPT");
                        rackTag.Lite3LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L3LPT");
                        rackTag.Lite3VGroove = segments.GetStringAttributeDesc3("Z58L3VG");
                        rackTag.Lite3Routing = segments.GetStringAttributeDesc1("Z58L3RTG");

                        // Shapes
                        rackTag.ShapeNum = segments.GetDecimalAttributeValue("Z58SHAPE");
                        rackTag.Base = segments.GetDecimalAttributeValue("Z581BDEC");
                        rackTag.Left = segments.GetDecimalAttributeValue("Z581LDEC");
                        rackTag.Right = segments.GetDecimalAttributeValue("Z581RDEC");
                        rackTag.Top = segments.GetDecimalAttributeValue("Z581TDEC");
                        rackTag.S1 = segments.GetDecimalAttributeValue("Z5811DEC");
                        rackTag.S2 = segments.GetDecimalAttributeValue("Z5812DEC");
                        // Unit Weight
                        var qty = segments.GetDecimalAttributeValue("Z5840360");
                        var totalWeight = segments.GetDecimalAttributeValue("Z58GLWT");
                        rackTag.UnitWeight = (qty == 0) ? 0 : Math.Round(totalWeight / qty, 3);
                        // Sqft
                        rackTag.UnitActualSqft = segments.GetDecimalAttributeValue("Z58USF");
                        rackTag.UnitBillableSqft = segments.GetDecimalAttributeValue("Z58BSF");
                        // Spacer
                        rackTag.Spacer1Size = segments.GetDecimalAttributeValue("Z58L1SSZ");
                        rackTag.Spacer1Type = segments.GetStringAttributeValue("Z58L1STP");
                        rackTag.Spacer2Size = segments.GetDecimalAttributeValue("Z58L2SSZ");
                        rackTag.Spacer2Type = segments.GetStringAttributeValue("Z58L2STP");
                        // Preserve / Scoring
                        rackTag.ExteriorPreserveColor = segments.GetStringAttributeValue("Z587566");
                        rackTag.ExteriorPreserveCutback = segments.GetStringAttributeValue("Z587563");
                        rackTag.InteriorPreserveColor = segments.GetStringAttributeValue("Z587578");
                        rackTag.InteriorPreserveCutback = segments.GetStringAttributeValue("Z587575");
                        rackTag.InteriorScoring = segments.GetStringAttributeValue("Z587581");
                        rackTag.ExteriorScoring = segments.GetStringAttributeValue("Z587569");
                        rackTag.BreatherTube1 = segments.GetStringAttributeValue("Z58TA1");
                        rackTag.BreatherTubeUnitsCount = (rackTag.BreatherTube1 == "Y") ? rackTag.Quantity : 0;
                        rackTag.ScoredUnitsCount = (string.IsNullOrEmpty(rackTag.InteriorScoring) & string.IsNullOrEmpty(rackTag.ExteriorScoring)) ? 0 : rackTag.Quantity;
                        // Other
                        rackTag.CardinalProductType = segments.GetStringAttributeValue("Z5840310");
                        rackTag.CrateType = segments.GetStringAttributeValue("Z58CRATE");
                        rackTag.CustomerSupplied = segments.GetStringAttributeValue("Z5840450");
                        rackTag.CustomerLiteNumber = segments.GetDecimalAttributeValue("Z58L1CLN");

                        // Original Customer
                        rackTag.OrigSoldToNum = segments.GetDecimalAttributeValue("Z58CTL");
                        rackTag.OrigShipToNum = segments.GetDecimalAttributeValue("Z5840425");
                        rackTag.OrigSoldToName = custNameMgr.GetName(e1Session, rackTag.OrigSoldToNum);
                        rackTag.OrigShipToName = custNameMgr.GetName(e1Session, rackTag.OrigSoldToNum);

                        // Picked Units
                        rackTag.PickItemNum = segments.GetDecimalAttributeValue("Z5880230");
                        rackTag.PickedUnitsCount = rackTag.PickItemNum != 0 ? rackTag.Quantity : 0;

                        // Related Sales Line
                        //var salesLine = _relatedSalesOrderLineMgr.GetRelatedSalesOrderLine(e1Session, rackTag.CompanyNum, rackTag.SalesOrderNum, rackTag.SalesOrderType, rackTag.SalesOrderLineNum * 1000);
                        //rackTag.ProductType = salesLine.ProductType;

                        // Treat CustPacked schedule as scheduling for filter purposes.
                        if (rackTag.Status == "P") rackTag.Status = "1";

                        rackTags.Add(rackTag);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rackTags.Count);
                    Logger.Trace($"Rack Ids [{string.Join(",", rackTags.GroupBy(x => x.RackId).Select(x => x.Key).ToArray())}]");
                }

                // Edi Messages
                foreach (var rackTag in rackTags)
                {
                    var cmd1 = (e1Session.DbCommands.ContainsKey("RackTagItemManager.GetEdiMessages")) ? e1Session.DbCommands["RackTagItemManager.GetEdiMessages"] : GetEdiMessages();
                    cmd1.Parameters[0].Value = rackTag.CompanyNum.PadLeft(5);
                    cmd1.Parameters[1].Value = rackTag.SalesOrderNum;
                    cmd1.Parameters[2].Value = rackTag.SalesOrderType.PadLeft(2);
                    cmd1.Parameters[3].Value = rackTag.SalesOrderLineNum * 1000;
                    cmd1.Connection = e1Session.DbConnection;
                    using (IDataReader reader2 = cmd1.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader2.Read())
                        {
                            var msgType = Data.CastDbNull.To(reader2["ZDEDTY"], string.Empty).Trim();
                            if (msgType == "R")
                            {
                                switch (Convert.ToInt32(Data.CastDbNull.To<decimal>(reader2["ZDEDSQ"], 0)))
                                {
                                    case 1:
                                        rackTag.FooterMsg1 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 2:
                                        rackTag.FooterMsg2 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 3:
                                        rackTag.FooterMsg3 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 4:
                                        rackTag.FooterMsg4 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 5:
                                        rackTag.FooterMsg5 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 6:
                                        rackTag.FooterMsg6 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                }
                            }
                            else
                            {
                                if (msgType == "L")
                                {
                                    switch (Convert.ToInt32(Data.CastDbNull.To<decimal>(reader2["ZDEDSQ"], 0)))
                                    {
                                        case 1:
                                            rackTag.LabelMsg1 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 2:
                                            rackTag.LabelMsg2 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 3:
                                            rackTag.LabelMsg3 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 4:
                                            rackTag.LabelMsg4 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 5:
                                            rackTag.LabelMsg5 = Data.CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                    }
                                }
                            }
                        }

                        reader2.Close();
                    }
                }

                //if (rackTags.Any())
                //{
                //    var firstOrDefault = rackTags.FirstOrDefault();
                //    var plantNum = firstOrDefault?.PlantNum;
                //    if (plantNum != null)
                //    {
                //        var plantCode = PlantManager.GetPlant(plantNum).PlantCode;
                //        var workOrders = rackTags.Select(o => o.WorkOrderNum).ToList().Distinct().ToList();
                //        var dockLocal = GetDockLocal(plantCode, workOrders);
                //        foreach (var rackTag in rackTags)
                //        {
                //            string dock;
                //            dockLocal.TryGetValue(rackTag.WorkOrderNum, out dock);
                //            rackTag.DockLocal = dock ?? "";
                //        }
                //        if (plantCode == "MVCG")
                //        {
                //            var hasMate = GetHasMate(plantCode, workOrders);
                //            foreach (var rackTag in rackTags)
                //            {
                //                string mate;
                //                hasMate.TryGetValue(rackTag.BeginSerialNum, out mate);
                //                rackTag.HasMate = mate ?? "";
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return rackTags;
        }

        public static Dictionary<string, string> GetHasMate(string plantCode, List<decimal> workOrders = null)
        {
            var cnCP = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Shop Floor Connection
                cnCP = ConnectionManager.GetSqlConnectionFromKey($"{plant.PlantCode}.CLEARPATH");
                cnCP.Open();

                // Process
                return GetCPHasMate(cnCP, workOrders);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw new Exception(e.ToString());
            }
            finally
            {
                cnCP.Close();
            }
        }

        private static Dictionary<string, string> GetCPHasMate(SqlConnection cnSF, IEnumerable<decimal> workOrders)
        {
            var hash = new Dictionary<string, string>();
            try
            {
                // create temp table for wo List
                var cmdTemp = new SqlCommand();
                cmdTemp.CommandType = CommandType.Text;
                cmdTemp.CommandText = @"CREATE TABLE #TempWoList (WOrderNum decimal(8,0))";
                cmdTemp.Connection = cnSF;
                int rows = cmdTemp.ExecuteNonQuery();

                // Populate WO List
                Logger.Info("Load Work Order Data Table");
                var sb = new StringBuilder();
                sb.Append("INSERT #TempWoList");
                var i = 0;
                foreach (var workOrder in workOrders)
                {
                    sb.Append(i == 0 ? $" SELECT {workOrder}" : $" UNION SELECT {workOrder}");
                    i += 1;
                }

                var cmd2 = new SqlCommand();
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandText = sb.ToString();
                cmd2.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd2));
                int rows2 = cmd2.ExecuteNonQuery();

                // Get IncomingOrders Data
                var cmdHasMate = GetHasMate();
                cmdHasMate.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmdHasMate));
                using (IDataReader reader = cmdHasMate.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        //var workOrderNum = Data.CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                        var unitID = Data.CastDbNull.To(reader["UnitID"], "");
                        var hasMate = Data.CastDbNull.To(reader["HasMate"], "");
                        Logger.Trace($@"UnitID: {unitID}  D: {hasMate}");
                        hash.Add(unitID, hasMate);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }

            return hash;
        }

        //public static Dictionary<decimal, string> GetDockLocal(string plantCode, List<decimal> workOrders = null)
        //{
        //    var cnSf = new SqlConnection();
        //    try
        //    {
        //        // resolve Plant
        //        var plant = PlantManager.GetPlant(plantCode);

        //        // Shop Floor Connection
        //        cnSf = ConnectionManager.GetSqlConnectionFromKey($"{plant.PlantCode}.SHOPFLOOR");
        //        cnSf.Open();

        //        // Process
        //        return GetSFIncomingOrdersDockLocal(cnSf, workOrders);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e);
        //        throw new Exception(e.ToString());
        //    }
        //    finally
        //    {
        //        cnSf.Close();
        //    }
        //}

        public static SqlCommand GetHasMate()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT *
                    FROM   (   SELECT PS.ProdScheduleID
                                    , PUL.ProdUnitLiteID
                                    , PU.SerialNumber AS UnitID
                                    , CAST(WOI.WorkOrderNum AS DECIMAL(8, 0)) AS WOrderNum
                                    , CAST(PS.ScheduleNum AS NUMERIC(6, 0)) AS ScheduleNum
                                    , CASE WHEN SRT.Description LIKE '%BUNGEE%' -- in a bungee
                                                AND PUB.BoxSlot % 2 <> 0 -- and odd slot number
                                                AND NBS.NextBoxSlot IS NULL -- and there is no slot after this one
                                    THEN       'N'
                                           ELSE 'Y'
                                      END AS HasMate
                               FROM   ClearPath.ProdSchedule PS WITH ( READUNCOMMITTED )
                                      JOIN ClearPath.ProdScheduleUnit PSU WITH ( READUNCOMMITTED ) ON PS.ProdScheduleID = PSU.ProdScheduleID
                                      JOIN ClearPath.ProdUnit PU WITH ( READUNCOMMITTED ) ON PSU.ProdUnitID = PU.ProdUnitID
                                      JOIN ClearPath.SpecUnit SU WITH ( READUNCOMMITTED ) ON PU.SpecUnitID = SU.SpecUnitID
                                      JOIN ClearPath.WorkOrderItem WOI WITH ( READUNCOMMITTED ) ON SU.WorkOrderItemID = WOI.WorkOrderItemID
                                      INNER JOIN #TempWoList A ON A.WOrderNum = CAST(WOI.WorkOrderNum AS DECIMAL(8, 0))
                                      JOIN ClearPath.Customer C WITH ( READUNCOMMITTED ) ON WOI.CustomerID = C.CustomerID
                                      JOIN ClearPath.ProdUnitLite PUL ON PU.ProdUnitID = PUL.ProdUnitID
                                      LEFT JOIN ClearPath.ProdUnitBox PUB WITH ( READUNCOMMITTED ) ON PU.ProdUnitID = PUB.ProdUnitID
                                      JOIN ClearPath.SpecRackType SRT WITH ( READUNCOMMITTED ) ON PUB.SpecRackTypeID = SRT.SpecRackTypeID
                                      OUTER APPLY (   SELECT MAX(PUBt.BoxSlot) AS NextBoxSlot
                                                      FROM   ClearPath.ProdScheduleUnit PSUt WITH ( READUNCOMMITTED )
                                                             JOIN ClearPath.ProdUnitBox PUBt WITH ( READUNCOMMITTED ) ON PSUt.ProdUnitID = PUBt.ProdUnitID
                                                      WHERE  PSUt.ProdScheduleID = PSU.ProdScheduleID
                                                             AND PUBt.BoxNum = PUB.BoxNum
                                                             AND PUBt.BoxSlot = PUB.BoxSlot + 1 ) AS NBS
                               WHERE  PS.SoftDeleted = 0
                                      AND PUL.SoftDeleted = 0 ) AS t
                    ";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

    }
}
