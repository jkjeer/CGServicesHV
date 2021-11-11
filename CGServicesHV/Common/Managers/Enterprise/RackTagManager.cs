using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Enums;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class RackTagManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ItemAttributeManager _itemAttributeMgr;

        public RackTagManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"RackTagManager.GetRackPlant", GetRackPlant()},
                {"RackTagManager.GetRackTagItems", GetRackTagItems()},
                {"RackTagManager.GetRackTagItemsArchive", GetRackTagItemsArchive()},
                {"RackTagManager.GetRelatedOrderInfo", GetRelatedOrderInfo()},
                {"RackTagManager.GetCoatedWorkCenter", GetCoatedWorkCenter()},
                {"RackTagManager.GetEdiMessages", GetEdiMessages()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }

            _itemAttributeMgr = new ItemAttributeManager(e1Session);
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
                        AB1.ABAC19,
                        RHSHAN,
                        AB2.ABALPH AS SHANALPH,  
                        RHZ58RT,
                        UD1.DRDL01 AS RHZ58RT_DESC,
						RTZ58RSL,
                        RHZ58BOX,
                        RHZ58GWTT,
                        RHZ58RTWT,
                        RHZ58SCN,
                        RHLOCN,
                        RHURAB,
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
                        COALESCE(F4211.SDUORG,F42119.SDUORG) AS SDUORG,
                        COALESCE(F4211.SDVR01,F42119.SDVR01) AS SDVR01,
                        COALESCE(F4211.SDRCD,F42119.SDRCD) AS SDRCD,
                        COALESCE(F4211.SDURCD,F42119.SDURCD) AS SDURCD,
                        COALESCE(F4211.SDDRQJ,F42119.SDDRQJ) AS SDDRQJ,
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
                        RDLOTN,
                        RDZ58DST,
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
                        BIZ58L4R,
                        BIZ58L4SLN,
                        1 AS RDPQOH,
                        COALESCE(CMCFGID, 0) AS CMCFGID
                    FROM
                        F580055
                        INNER JOIN F580056
                        ON RDZ58RID = RHZ58RID
                        LEFT OUTER JOIN F4201
                        ON  SHDOCO = RDDOCO AND
                            SHDCTO = RDDCTO AND
                            SHKCOO = RDKCOO
                        LEFT OUTER JOIN F4211
                        ON  F4211.SDDOCO = RDDOCO AND
                            F4211.SDLNID = RDLNID AND
                            F4211.SDDCTO = RDDCTO AND
                            F4211.SDKCOO = RDKCOO
                        LEFT OUTER JOIN F42119
                        ON  F42119.SDDOCO = RDDOCO AND
                            F42119.SDLNID = RDLNID AND
                            F42119.SDDCTO = RDDCTO AND
                            F42119.SDKCOO = RDKCOO
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
                        LEFT OUTER JOIN F3201
                        ON  CMDOCO = RDDOCO AND
                            CMDCTO = RDDCTO AND
                            CMLNID = RDLNID AND
                            CMKCOO = RDKCOO AND
                            CMDOCO > 0
						LEFT OUTER JOIN F580061
						ON RTZ58RT = RHZ58RT AND
						    RTMCU = RHMCU AND
                            RTAN8 = 0
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

        public iDB2Command GetRackTagItemsArchive()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT DISTINCT
                    HLZ58RID AS RHZ58RID,
                    HLMCU AS RHMCU,
                    HLAN8 AS RHAN8,
                    HLZ58HST AS RHZ58HST,
                    HLZ58RSDT AS RHZ58RSDT,
                    AB1.ABALPH AS AN8ALPH,
                    AB1.ABAC19,
                    HLSHAN AS RHSHAN,
                    AB2.ABALPH AS SHANALPH,  
                    HLZ58RT AS RHZ58RT,
                    UD1.DRDL01 AS RHZ58RT_DESC,
					RTZ58RSL,
                    HLZ58BOX AS RHZ58BOX,
                    HLZ58GWTT AS RHZ58GWTT,
                    HLZ58RTWT AS RHZ58RTWT,
                    HLZ58SCN AS RHZ58SCN,
                    HLLOCN AS RHLOCN,
                    HLURAB AS RHURAB,
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
                    COALESCE(F4211.SDUORG,F42119.SDUORG) AS SDUORG,
                    COALESCE(F4211.SDVR01,F42119.SDVR01) AS SDVR01,
                    COALESCE(F4211.SDRCD,F42119.SDRCD) AS SDRCD,
                        COALESCE(F4211.SDURCD,F42119.SDURCD) AS SDURCD,
                        COALESCE(F4211.SDDRQJ,F42119.SDDRQJ) AS SDDRQJ,
                    S1Z58SCDDJ,
                    S1Z58SCTYP,
                    COALESCE(BIZ58IGLN,'0') AS BIZ58IGLN,
                    DLKCOO AS RDKCOO,
                    DLDOCO AS RDDOCO,
                    DLDCTO AS RDDCTO,
                    DLLNID AS RDLNID,
                    DLDOC1 AS RDDOC1,
                    DLXDCTO AS RDXDCTO,
                    DLDOC2 AS RDDOC2,
                    DLPKCO AS RDPKCO,
                    DLXCTO AS RDXCTO,
                    DLLNIC AS RDLNIC,
                    DLZ58SD AS RDZ58SD,
                    DLZ58ROW AS RDZ58ROW,
                    DLZ58STK AS RDZ58STK,
                    DLLOTN AS RDLOTN,
                    DLZ58DST as RDZ58DST,
                    DLZ58SN AS MIN_RDZ58SN,
                    DLZ58SN AS MAX_RDZ58SN,
                    DLZ58BXRO AS RDZ58BXRO,
                    DLZ58PS AS RDZ58PS,
                    DLZ58SL AS RDZ58SL,
                    DLITM AS RDITM,
                    BIZ58L1R,
                    BIZ58L1SLN,
                    BIZ58L2R,
                    BIZ58L2SLN,
                    BIZ58L3R,
                    BIZ58L3SLN,               
                    BIZ58L4R,
                    BIZ58L4SLN,               
                    1 AS RDPQOH,
                    COALESCE(CMCFGID, 0) AS CMCFGID
                FROM
                    F580559
                    INNER JOIN F580569
                    ON	DLZ58RID = HLZ58RID AND
						DLZ58DST = HLZ58HST AND
						DLMCU = HLMCU
                    LEFT OUTER JOIN F4201
                    ON  SHDOCO = DLDOCO AND
                        SHDCTO = DLDCTO AND
                        SHKCOO = DLKCOO
                    LEFT OUTER JOIN F4211
                    ON  F4211.SDDOCO = DLDOCO AND
                        F4211.SDLNID = DLLNID AND
                        F4211.SDDCTO = DLDCTO AND
                        F4211.SDKCOO = DLKCOO
                    LEFT OUTER JOIN F42119
                    ON  F42119.SDDOCO = DLDOCO AND
                        F42119.SDLNID = DLLNID AND
                        F42119.SDDCTO = DLDCTO AND
                        F42119.SDKCOO = DLKCOO
                    LEFT OUTER JOIN F580032
                    ON  ATDOCO = DLDOCO AND
                        ATLNID = DLLNID AND
                        ATDCTO = DLDCTO AND
                        ATKCOO = DLKCOO AND
                        ATDOCO > 0
                    LEFT OUTER JOIN F5834A11
                    ON  S5MCU = DLMCU AND
                        S5DOCO = DLDOC1
                    LEFT OUTER JOIN F5834A01
                    ON  S1MCU = S5MCU AND
                        S1Z58SCN = S5Z58SCN AND
                        S1Z58SCD = S5Z58SCD
                    LEFT OUTER JOIN F0101 AB1
                    ON  AB1.ABAN8 = HLAN8
                    LEFT OUTER JOIN F0101 AB2
                    ON  AB2.ABAN8 = HLSHAN
                    LEFT OUTER JOIN F580200 
                    ON  BIMCU = DLMCU AND
                        BIDOCO = DLDOC1 AND
                        BIZ58UNID = DLZ58SN AND
                        BIEDSP != 'D'
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                    ON  UD1.DRSY = '57' AND
                        UD1.DRRT = 'RK' AND
                        TRIM(UD1.DRKY) = TRIM(HLZ58RT)
                    LEFT OUTER JOIN F3201
                    ON  CMDOCO = DLDOCO AND
                        CMDCTO = DLDCTO AND
                        CMLNID = DLLNID AND
                        CMKCOO = DLKCOO AND
                        CMDOCO > 0
					LEFT OUTER JOIN F580061
					ON RTZ58RT = HLZ58RT AND
						RTMCU = HLMCU AND 
                        RTAN8 = 0
                WHERE
                    HLZ58RID = ? AND
					HLMCU = ? AND
					HLZ58HST = '8'
                ORDER BY
                    DLZ58SD,
                    DLZ58ROW,
                    DLZ58STK,
                    DLZ58SL,
                    DLZ58PS"
            };
            cmd.Parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RID"].Precision = 15;
            cmd.Parameters["RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
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

        /**************************************************************
         *  Original Customer PO Line
         * ************************************************************/
        public iDB2Command GetRelatedOrderInfo()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	                SD2.SDVR01,
                    RO.ROEDTR
                FROM 
	                F4211 SD1 
	                INNER JOIN F580400 RO 
	                ON	RO.RODOC1 = SD1.SDRORN AND 
		                RO.RORCTO = SD1.SDRCTO AND 
		                RO.RORLLN = SD1.SDRLLN AND 
		                RO.RORKCO = SD1.SDRKCO
	                INNER JOIN F4211 SD2 
	                ON	RO.RODOCO = SD2.SDDOCO AND 
		                RO.RODCTO = SD2.SDDCTO AND 
		                RO.ROLNID = SD2.SDLNID AND 
		                RO.ROKCOO = SD2.SDKCOO
                WHERE	                
                    SD1.SDKCOO = ? AND
	                SD1.SDDOCO = ? AND
	                SD1.SDDCTO = ? AND
	                SD1.SDLNID = ? "
            };
            cmd.Parameters.Add(new iDB2Parameter("KCOO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("LNID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LNID"].Precision = 6;
            cmd.Parameters["LNID"].Scale = 0;
            return cmd;
        }

        /**************************************************************
         *  Original Customer PO Line
         * ************************************************************/
        public iDB2Command GetCoatedWorkCenter()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	               COUNT(*) AS RECS
                FROM
	                F3112 
                WHERE	                
                    WLDOCO = ? AND
	                WLOPSQ IN (2700, 4200)"
            };
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            return cmd;
        }

        /**************************************************************
         *  Current Plant
         * ************************************************************/
        public iDB2Command GetRackPlant()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        RHMCU
                    FROM
                        F580055 
                    WHERE
                        RHZ58RID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RID"].Precision = 15;
            cmd.Parameters["RID"].Scale = 0;
            return cmd;
        }

        //public RackTagItems GetRackTagItems(BusinessSystemSession e1Session, Plant plant, decimal rackId, string strategy)
        public RackTagItems GetRackTagItems(BusinessSystemSession e1Session, SqlConnection sqlCn, Plant plant, decimal rackId, string strategy)
        {
            var rackTags = new RackTagItems();
            var rackTagItems = new RackTagItems();
            var custNameMgr = new CustomerNameManager(e1Session);
            try
            {
                // Current Plant
                var cmd0 = e1Session.DbCommands["RackTagManager.GetRackPlant"];
                cmd0.Parameters[0].Value = rackId;
                cmd0.Connection = e1Session.DbConnection;
                string currentPlant = "";
                Logger.Info(Logging.DbCmdString(cmd0));
                using (IDataReader reader0 = cmd0.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader0.Read())
                    {
                        currentPlant = CastDbNull.To(reader0["RHMCU"], string.Empty).Trim();
                    }

                    reader0.Close();
                }

                // Current or Archive
                var cmd = new iDB2Command();
                if (currentPlant.In("32", "04", "05", "08") )
                {
                    cmd = e1Session.DbCommands["RackTagManager.GetRackTagItems"];
                    cmd.Parameters[0].Value = rackId;
                }
                else
                {
                    cmd = e1Session.DbCommands["RackTagManager.GetRackTagItemsArchive"];
                    cmd.Parameters[0].Value = rackId;
                    cmd.Parameters[1].Value = plant.PlantNum.PadLeft(12);
                }

                cmd.Connection = e1Session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new RackTagItem
                        {
                            RackId = CastDbNull.To<decimal>(reader["RHZ58RID"], 0),
                            CompanyNum = CastDbNull.To(reader["RDKCOO"], string.Empty).Trim(),
                            PlantNum = CastDbNull.To(reader["RHMCU"], string.Empty).Trim(),
                            SalesOrderNum = CastDbNull.To<decimal>(reader["RDDOCO"], 0),
                            SalesOrderLineNum = Math.Round(CastDbNull.To<decimal>(reader["RDLNID"], 0) / 1000, 3),
                            SalesOrderType = CastDbNull.To(reader["RDDCTO"], string.Empty).Trim(),
                            WorkOrderNum = CastDbNull.To<decimal>(reader["RDDOC1"], 0),
                            LotNum = CastDbNull.To<string>(reader["RDLOTN"], string.Empty).Trim(),
                            BeginSerialNum = CastDbNull.To(reader["MIN_RDZ58SN"], string.Empty).Trim(),
                            EndSerialNum = CastDbNull.To(reader["MAX_RDZ58SN"], string.Empty).Trim(),
                            ItemNum = CastDbNull.To<decimal>(reader["RDITM"], 0),
                            SoldToNum = CastDbNull.To<decimal>(reader["RHAN8"], 0),
                            SoldToName = CastDbNull.To(reader["AN8ALPH"], string.Empty).Trim(),
                            ShipToNum = CastDbNull.To<decimal>(reader["RHSHAN"], 0),
                            ShipToName = CastDbNull.To(reader["SHANALPH"], string.Empty).Trim(),
                            CustPartNum1 = CastDbNull.To(reader["ATZ58CP2"], string.Empty).Trim(),
                            CustPartNum2 = CastDbNull.To(reader["ATZ58CP3"], string.Empty).Trim(),
                            CustPartNum3 = CastDbNull.To(reader["ATZ58CP4"], string.Empty).Trim(),
                            CustSeqNum1 = CastDbNull.To(reader["ATZ58CSQNO"], string.Empty).Trim(),
                            GenericSeqNum = CastDbNull.To(reader["ATZ58GSQNO"], string.Empty).Trim(),
                            //IGLine = CastDbNull.To(reader["BIZ58IGLN"], string.Empty).Trim(),
                            IGLine = "0",
                            RackType = CastDbNull.To(reader["RHZ58RT"], string.Empty).Trim(),
                            RackTypeDesc = CastDbNull.To(reader["RHZ58RT_DESC"], string.Empty).Trim(),

                            RackSlotsTotal = CastDbNull.To<decimal>(reader["RTZ58RSL"], 0),
                            IsEmptySlot = false,

                            Location = CastDbNull.To(reader["RHLOCN"], string.Empty).Trim(),
                            Status = CastDbNull.To(reader["RHZ58HST"], string.Empty).Trim(),
                            RackStatus = CastDbNull.To<string>(reader["RHZ58HST"], string.Empty).Trim(),
                            RackItemStatus = CastDbNull.To<string>(reader["RDZ58DST"], string.Empty).Trim(),
                            StatusDate = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["RHZ58RSDT"], 0)),
                            BoxNum = CastDbNull.To<decimal>(reader["RHZ58BOX"], 0),
                            RackGlassWeight = CastDbNull.To<decimal>(reader["RHZ58GWTT"], 0) / 100,
                            RackTareWeight = CastDbNull.To<decimal>(reader["RHZ58RTWT"], 0) / 100,
                            ScheduleNum = CastDbNull.To<decimal>(reader["RHZ58SCN"], 0),
                            ScheduleType = CastDbNull.To(reader["S1Z58SCTYP"], string.Empty).Trim(),
                            Quantity = CastDbNull.To(reader["RDPQOH"], 0),
                            CustomerPO = CastDbNull.To(reader["SDVR01"], string.Empty).Trim(),
                            ReasonCode = CastDbNull.To(reader["SDRCD"], string.Empty).Trim(),
                            ShopFloorMessage1 = CastDbNull.To(reader["ATZ5899009"], string.Empty).Trim(),
                            ShopFloorMessage2 = CastDbNull.To(reader["ATZ5899010"], string.Empty).Trim(),
                            PickNotes = CastDbNull.To(reader["ATZ5899011"], string.Empty).Trim(),
                            PackNotes = CastDbNull.To(reader["ATZ5899012"], string.Empty).Trim(),
                            ProductType = CastDbNull.To(reader["ATZ58PT"], string.Empty).Trim(),
                            CustomerDate = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["SDDRQJ"], 0)),
                            DueDate = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0)),
                            DueDateString = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0)).ToString("MM/dd/yyyy"),
                            RackSide = CastDbNull.To(reader["RDZ58SD"], string.Empty).Trim(),
                            RackPosition = CastDbNull.To<decimal>(reader["RDZ58PS"], 0),
                            RackStack = CastDbNull.To<decimal>(reader["RDZ58STK"], 0),
                            RackRow = CastDbNull.To<decimal>(reader["RDZ58ROW"], 0),
                            RackSlot = CastDbNull.To<decimal>(reader["RDZ58SL"], 0),
                            RackSlotPosition = (CastDbNull.To<decimal>(reader["RDZ58PS"], 0) > 0)
                                ? CastDbNull.To<decimal>(reader["RDZ58PS"], 0)
                                : CastDbNull.To<decimal>(reader["RDZ58SL"], 0),
                            BoxRotated = CastDbNull.To(reader["RDZ58BXRO"], string.Empty).Trim(),
                            Lite1Rack = CastDbNull.To(reader["BIZ58L1R"], string.Empty).Trim(),
                            Lite1Slot = CastDbNull.To<decimal>(reader["BIZ58L1SLN"], 0),
                            Lite2Rack = CastDbNull.To(reader["BIZ58L2R"], string.Empty).Trim(),
                            Lite2Slot = CastDbNull.To<decimal>(reader["BIZ58L2SLN"], 0),
                            Lite3Rack = CastDbNull.To(reader["BIZ58L3R"], string.Empty).Trim(),
                            Lite3Slot = CastDbNull.To<decimal>(reader["BIZ58L3SLN"], 0),
                            Lite4Rack = CastDbNull.To<string>(reader["BIZ58L4R"], string.Empty).Trim(),
                            Lite4Slot = CastDbNull.To<Decimal>(reader["BIZ58L4SLN"], Decimal.Zero),
                            ConfigId = CastDbNull.To<decimal>(reader["CMCFGID"], 0),
                            PrinterNum = CastDbNull.To<Decimal>(reader["RHURAB"], Decimal.Zero),
                            BlindDocumentFlag = CastDbNull.To<string>(reader["ABAC19"], string.Empty).Trim()
                        };
                        rackTags.Add(rackTag);
                    }

                    reader.Close();
                }

                Logger.Info($"Rack Ids [{string.Join(",", rackTags.GroupBy(x => x.RackId).Select(x => x.Key).ToArray())}]");
                //this.GetScheduleNums(sqlCn, rackTags);

                // Grouping Key
                RackTag.GroupStrategyEnum strategyEnum;
                Enum.TryParse(strategy, true, out strategyEnum);

                //Add empty slots
                if (rackTags.Any() && strategyEnum == RackTag.GroupStrategyEnum.UnitWithEmptySlots)
                {
                    decimal rackSlotsTotal = rackTags.First().RackSlotsTotal;
                    if (rackSlotsTotal > 0)
                    {
                        decimal rackSlotsUsed = rackTags.Max(x => x.RackSlotPosition);
                        for (int i = 1; i <= rackSlotsTotal; i++)
                        {
                            var tag = rackTags.FirstOrDefault(t => t.RackSlotPosition == i);
                            if (tag == null)
                            {
                                //Make blank tag
                                var firstTag = i == 1 ? rackTags.First() : rackTags.Find(item => item.RackSlotPosition == i - 1);
                                var blankTag = new RackTagItem
                                {
                                    RackSlot = i,
                                    RackPosition = i,
                                    RackSlotPosition = i,
                                    RackSlotsTotal = firstTag.RackSlotsTotal,
                                    IsEmptySlot = true,
                                    RackId = firstTag.RackId,
                                    RackSide = firstTag.RackSide,
                                    RackRow = firstTag.RackRow,
                                    RackStack = firstTag.RackStack,
                                    RackTypeDesc = firstTag.RackTypeDesc,
                                    RackType = firstTag.RackType,
                                    RackTareWeight = firstTag.RackTareWeight,
                                    BeginSerialNum = "0", //Convert.ToString(i), //Convert.ToString(Convert.ToDecimal(firstTag.BeginSerialNum) + 1, CultureInfo.InvariantCulture),
                                    BoxNum = firstTag.BoxNum,
                                    Location = firstTag.Location,
                                    CrateType = firstTag.CrateType,
                                    RackGlassWeight = firstTag.RackGlassWeight,
                                    PlantNum = firstTag.PlantNum,
                                    CompanyNum = firstTag.CompanyNum,
                                    ScheduleNum = firstTag.ScheduleNum,
                                    SalesOrderNum = 0,
                                    SalesOrderType = "",
                                    SalesOrderLineNum = 0,
                                    ItemNum = 0,
                                    WorkOrderNum = 0,
                                    Quantity = 0,
                                    DueDate = firstTag.DueDate,
                                    DueDateString = firstTag.DueDateString
                                };

                                //Insert blank tag
                                if (i <= rackSlotsUsed)
                                {
                                    //Insert into middle of list
                                    rackTags.Insert(i, blankTag);
                                }
                                else
                                {
                                    //Add to end of list
                                    rackTags.Add(blankTag);
                                }
                            }
                        }
                    }
                }

                if (rackTags.Any(x => x.BoxNum == 0) && strategyEnum == RackTag.GroupStrategyEnum.WorkOrderInterleaved)
                {
                    strategyEnum = RackTag.GroupStrategyEnum.Unit;
                    Logger.Info("Box Zero");
                }

                if (rackTags.Count > 1 && rackTags.Max(x => x.RackSlotPosition) <= 1 && strategyEnum == RackTag.GroupStrategyEnum.WorkOrderInterleaved)
                {
                    strategyEnum = RackTag.GroupStrategyEnum.Unit;
                    Logger.Info("No RackSlotPosition");
                }

                switch (strategyEnum)
                {
                    case RackTag.GroupStrategyEnum.Unit:
                        Logger.Info("Enum Unit");
                        SetGroupByUnit(rackTags);
                        break;
                    case RackTag.GroupStrategyEnum.WorkOrder:
                        Logger.Info("Enum Work Order");
                        SetGroupByWorkOrder(rackTags);
                        break;
                    case RackTag.GroupStrategyEnum.WorkOrderInterleaved:
                        Logger.Info("Enum Work Order Interleaved");
                        SetGroupByWorkOrderInterleaved(rackTags);
                        break;
                    default:
                        Logger.Info("Enum Default Unit");
                        SetGroupByUnit(rackTags);
                        break;
                }

                // Group
                var rackTagGroups = rackTags.GroupBy(x => x.GroupingKey);
                foreach (var rackTagGroup in rackTagGroups)
                {
                    var rackTagItem = new RackTagItem();

                    // First Item
                    var rackTagGroupFirstItem = rackTagGroup.First(); //item => item.WorkOrderNum > 0 && item.ItemNum > 0);
                    rackTagItem.RackId = rackTagGroupFirstItem.RackId;
                    rackTagItem.CompanyNum = rackTagGroupFirstItem.CompanyNum;
                    rackTagItem.PlantNum = rackTagGroupFirstItem.PlantNum;
                    rackTagItem.SalesOrderNum = rackTagGroupFirstItem.SalesOrderNum;
                    rackTagItem.SalesOrderType = rackTagGroupFirstItem.SalesOrderType;
                    rackTagItem.SalesOrderLineNum = rackTagGroupFirstItem.SalesOrderLineNum;
                    rackTagItem.WorkOrderNum = rackTagGroupFirstItem.WorkOrderNum;
                    rackTagItem.LotNum = rackTagGroupFirstItem.LotNum;
                    rackTagItem.BeginSerialNum = rackTagGroupFirstItem.BeginSerialNum;
                    rackTagItem.EndSerialNum = rackTagGroup.Select(x => x.BeginSerialNum).Last();
                    rackTagItem.ItemNum = rackTagGroupFirstItem.ItemNum;
                    rackTagItem.SoldToNum = rackTagGroupFirstItem.SoldToNum;
                    rackTagItem.SoldToName = rackTagGroupFirstItem.SoldToName;
                    rackTagItem.BlindDocumentFlag = rackTagGroupFirstItem.BlindDocumentFlag;
                    rackTagItem.ShipToNum = rackTagGroupFirstItem.ShipToNum;
                    rackTagItem.ShipToName = rackTagGroupFirstItem.ShipToName;
                    rackTagItem.CustPartNum1 = rackTagGroupFirstItem.CustPartNum1;
                    rackTagItem.CustPartNum2 = rackTagGroupFirstItem.CustPartNum2;
                    rackTagItem.CustPartNum3 = rackTagGroupFirstItem.CustPartNum3;
                    rackTagItem.CustSeqNum1 = rackTagGroupFirstItem.CustSeqNum1;
                    rackTagItem.GenericSeqNum = rackTagGroupFirstItem.GenericSeqNum;
                    rackTagItem.IGLine = rackTagGroupFirstItem.IGLine;
                    rackTagItem.RackType = rackTagGroupFirstItem.RackType;
                    rackTagItem.RackTypeDesc = rackTagGroupFirstItem.RackTypeDesc;
                    rackTagItem.Location = rackTagGroupFirstItem.Location;
                    rackTagItem.Status = rackTagGroupFirstItem.Status;
                    rackTagItem.RackStatus = rackTagGroupFirstItem.RackStatus;
                    rackTagItem.RackItemStatus = rackTagGroupFirstItem.RackItemStatus;
                    rackTagItem.StatusDate = rackTagGroupFirstItem.StatusDate;
                    rackTagItem.BoxNum = rackTagGroupFirstItem.BoxNum;
                    rackTagItem.RackGlassWeight = rackTagGroupFirstItem.RackGlassWeight;
                    rackTagItem.RackTareWeight = rackTagGroupFirstItem.RackTareWeight;
                    rackTagItem.ScheduleNum = rackTagGroupFirstItem.ScheduleNum;
                    rackTagItem.ScheduleType = rackTagGroupFirstItem.ScheduleType;
                    rackTagItem.CustomerPO = rackTagGroupFirstItem.CustomerPO;
                    rackTagItem.ReasonCode = rackTagGroupFirstItem.ReasonCode;
                    rackTagItem.DockLocal = rackTagGroupFirstItem.DockLocal;
                    rackTagItem.Dock = rackTagGroupFirstItem.Dock;
                    rackTagItem.BoxingMessage = rackTagGroupFirstItem.BoxingMessage;
                    rackTagItem.ShopFloorMessage1 = rackTagGroupFirstItem.ShopFloorMessage1;
                    rackTagItem.ShopFloorMessage2 = rackTagGroupFirstItem.ShopFloorMessage2;
                    rackTagItem.PickNotes = rackTagGroupFirstItem.PickNotes;
                    rackTagItem.PackNotes = rackTagGroupFirstItem.PackNotes;
                    rackTagItem.CustomerDate = rackTagGroupFirstItem.CustomerDate;
                    rackTagItem.DueDate = rackTagGroupFirstItem.DueDate;
                    rackTagItem.DueDateString = rackTagGroupFirstItem.DueDateString;
                    rackTagItem.BoxRotated = rackTagGroupFirstItem.BoxRotated;
                    rackTagItem.RackSide = rackTagGroupFirstItem.RackSide;
                    rackTagItem.RackRow = rackTagGroupFirstItem.RackRow;
                    rackTagItem.RackStack = rackTagGroupFirstItem.RackStack;
                    rackTagItem.RackPosition = rackTagGroupFirstItem.RackPosition;
                    rackTagItem.RackSlot = rackTagGroupFirstItem.RackSlot;
                    rackTagItem.RackSlotPosition = rackTagGroupFirstItem.RackSlotPosition;
                    rackTagItem.GroupingKey = rackTagGroupFirstItem.GroupingKey;
                    rackTagItem.Lite1Rack = rackTagGroupFirstItem.Lite1Rack;
                    rackTagItem.Lite1Slot = rackTagGroupFirstItem.Lite1Slot;
                    rackTagItem.Lite2Rack = rackTagGroupFirstItem.Lite2Rack;
                    rackTagItem.Lite2Slot = rackTagGroupFirstItem.Lite2Slot;
                    rackTagItem.Lite3Rack = rackTagGroupFirstItem.Lite3Rack;
                    rackTagItem.Lite3Slot = rackTagGroupFirstItem.Lite3Slot;
                    rackTagItem.Lite4Rack = rackTagGroupFirstItem.Lite4Rack;
                    rackTagItem.Lite4Slot = rackTagGroupFirstItem.Lite4Slot;
                    rackTagItem.RemakeRack = rackTagGroupFirstItem.RemakeRack;
                    rackTagItem.RemakeSlot = rackTagGroupFirstItem.RemakeSlot;
                    rackTagItem.ConfigId = rackTagGroupFirstItem.ConfigId;
                    rackTagItem.ProductType = rackTagGroupFirstItem.ProductType;
                    rackTagItem.PrinterNum = rackTagGroupFirstItem.PrinterNum;

                    rackTagItem.RackSlotsTotal = rackTagGroupFirstItem.RackSlotsTotal;
                    rackTagItem.IsEmptySlot = rackTagGroupFirstItem.IsEmptySlot;
                    //if (rackTagItem.IsEmptySlot)
                    //{
                    //    rackTagItem.BeginSerialNum = "";
                    //    rackTagItem.EndSerialNum = "";
                    //}

                    // Sum
                    rackTagItem.Quantity = rackTagGroup.Sum(x => x.Quantity);
                    rackTagItems.Add(rackTagItem);
                }

                // Item Attributes
                foreach (var rackTagItem in rackTagItems)
                {
                    if (rackTagItem.WorkOrderNum > 0 && rackTagItem.ItemNum > 0)
                    {
                        ItemAttributes segments;
                        if (rackTagItem.ConfigId > 0)
                        {
                            segments = _itemAttributeMgr.GetConfiguredItemAttributes(e1Session, rackTagItem.ConfigId);
                        }
                        else
                        {
                            segments = _itemAttributeMgr.GetDiscreteItemAttributes(e1Session, rackTagItem.ItemNum);
                        }

                        // Glass Type
                        rackTagItem.Lite1Thickness = segments.GetDecimalAttributeValue("Z58L1TK");
                        rackTagItem.Lite1Heat = segments.GetStringAttributeValue("Z58L1HT");
                        rackTagItem.Lite1FirstSrfCoat = segments.GetStringAttributeValue("Z58L11C");
                        rackTagItem.Lite1SecondSrfCoat = segments.GetStringAttributeValue("Z58L12C");
                        rackTagItem.Lite1Color = segments.GetStringAttributeDesc3("Z58L1CL", "CL");
                        rackTagItem.Lite1Pattern = segments.GetStringAttributeValue("Z58L1PX");
                        rackTagItem.Lite1LamiType = segments.GetStringAttributeValue("Z58L1LPT");
                        rackTagItem.Lite1LamiDesc = segments.GetStringAttributeDesc1("Z58L1LPT");
                        rackTagItem.Lite1LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L1LPT");
                        rackTagItem.Lite1VGroove = segments.GetStringAttributeDesc1("Z58L1VG");
                        rackTagItem.Lite1TempLogo = segments.GetStringAttributeValue("Z58L1TL");
                        rackTagItem.Lite1Routing = segments.GetStringAttributeDesc1("Z58L1RTG");
                        rackTagItem.Lite1EdgeWork = segments.GetStringAttributeDesc1("Z58L1EW");
                        rackTagItem.Lite1AnsiLogo = segments.GetStringAttributeValue("Z58L1ALT");

                        rackTagItem.Lite2Thickness = segments.GetDecimalAttributeValue("Z58L2TK");
                        rackTagItem.Lite2Heat = segments.GetStringAttributeValue("Z58L2HT");
                        rackTagItem.Lite2ThirdSrfCoat = segments.GetStringAttributeValue("Z58L23C");
                        rackTagItem.Lite2FourthSrfCoat = segments.GetStringAttributeValue("Z58L24C");
                        rackTagItem.Lite2Color = segments.GetStringAttributeDesc3("Z58L2CL", "CL");
                        rackTagItem.Lite2Pattern = segments.GetStringAttributeValue("Z58L2PX");
                        rackTagItem.Lite2LamiType = segments.GetStringAttributeValue("Z58L2LPT");
                        rackTagItem.Lite2LamiDesc = segments.GetStringAttributeDesc1("Z58L2LPT");
                        rackTagItem.Lite2LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L2LPT");
                        rackTagItem.Lite2VGroove = segments.GetStringAttributeDesc2("Z58L2VG");
                        rackTagItem.Lite2Routing = segments.GetStringAttributeDesc1("Z58L2RTG");

                        rackTagItem.Lite3Thickness = segments.GetDecimalAttributeValue("Z58L3TK");
                        rackTagItem.Lite3Heat = segments.GetStringAttributeValue("Z58L3HT");
                        rackTagItem.Lite3FifthSrfCoat = segments.GetStringAttributeValue("Z58L35C");
                        rackTagItem.Lite3SixthSrfCoat = segments.GetStringAttributeValue("Z58L36C");
                        rackTagItem.Lite3Color = segments.GetStringAttributeDesc3("Z58L3CL", "CL");
                        rackTagItem.Lite3Pattern = segments.GetStringAttributeValue("Z58L3PX");
                        rackTagItem.Lite3LamiType = segments.GetStringAttributeValue("Z58L3LPT");
                        rackTagItem.Lite3LamiDesc = segments.GetStringAttributeDesc1("Z58L3LPT");
                        rackTagItem.Lite3LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L3LPT");
                        rackTagItem.Lite3VGroove = segments.GetStringAttributeDesc3("Z58L3VG");
                        rackTagItem.Lite3Routing = segments.GetStringAttributeDesc1("Z58L3RTG");
                        rackTagItem.Lite1PatTextSrf = segments.GetStringAttributeValue("Z58L1PTS");
                        if (rackTagItem.Lite1PatTextSrf != null) rackTagItem.Lite1PatTextSrf = rackTagItem.Lite1PatTextSrf.Length > 2 ? rackTagItem.Lite1PatTextSrf.Substring(0, 1) : rackTagItem.Lite1PatTextSrf;

                        // Shapes
                        rackTagItem.ShapeNum = segments.GetDecimalAttributeValue("Z58SHAPE");
                        rackTagItem.Base = segments.GetDecimalAttributeValue("Z581BDEC");
                        rackTagItem.Left = segments.GetDecimalAttributeValue("Z581LDEC");
                        rackTagItem.Right = segments.GetDecimalAttributeValue("Z581RDEC");
                        rackTagItem.Top = segments.GetDecimalAttributeValue("Z581TDEC");
                        rackTagItem.S1 = segments.GetDecimalAttributeValue("Z5811DEC");
                        rackTagItem.S2 = segments.GetDecimalAttributeValue("Z5812DEC");

                        // Edge Delete Width
                        rackTagItem.EdgeDeleteWidth = segments.GetStringAttributeDesc2("Z5805680");

                        // Unit Weight
                        var qty = segments.GetDecimalAttributeValue("Z5840360");
                        var totalWeight = segments.GetDecimalAttributeValue("Z58GLWT");
                        rackTagItem.UnitWeight = (qty == 0) ? 0 : Math.Round(totalWeight / qty, 3);

                        // Sqft
                        rackTagItem.UnitActualSqft = segments.GetDecimalAttributeValue("Z58USF");
                        rackTagItem.UnitBillableSqft = segments.GetDecimalAttributeValue("Z58BSF");

                        // Spacer
                        rackTagItem.Spacer1Size = segments.GetDecimalAttributeValue("Z58L1SSZ");
                        rackTagItem.Spacer1Type = segments.GetStringAttributeValue("Z58L1STP");
                        rackTagItem.Spacer2Size = segments.GetDecimalAttributeValue("Z58L2SSZ");
                        rackTagItem.Spacer2Type = segments.GetStringAttributeValue("Z58L2STP");

                        // Preserve / Scoring
                        rackTagItem.ExteriorPreserveColor = segments.GetStringAttributeValue("Z587566");
                        rackTagItem.ExteriorPreserveCutback = segments.GetStringAttributeValue("Z587563");
                        rackTagItem.InteriorPreserveColor = segments.GetStringAttributeValue("Z587578");
                        rackTagItem.InteriorPreserveCutback = segments.GetStringAttributeValue("Z587575");
                        rackTagItem.InteriorScoring = segments.GetStringAttributeValue("Z587581");
                        rackTagItem.ExteriorScoring = segments.GetStringAttributeValue("Z587569");
                        rackTagItem.BreatherTube1 = segments.GetStringAttributeValue("Z58TA1");
                        rackTagItem.BreatherTubeUnitsCount = (rackTagItem.BreatherTube1 == "Y") ? rackTagItem.Quantity : 0;
                        rackTagItem.ScoredUnitsCount = (string.IsNullOrEmpty(rackTagItem.InteriorScoring) & string.IsNullOrEmpty(rackTagItem.ExteriorScoring)) ? 0 : rackTagItem.Quantity;

                        // Other
                        rackTagItem.CardinalProductType = segments.GetStringAttributeValue("Z5840310");
                        rackTagItem.CrateType = segments.GetStringAttributeValue("Z58CRATE");
                        rackTagItem.CustomerSupplied = segments.GetStringAttributeValue("Z5840450");
                        rackTagItem.CustomerLiteNumber = segments.GetDecimalAttributeValue("Z58L1CLN");

                        // Original Customer
                        rackTagItem.OrigSoldToNum = segments.GetDecimalAttributeValue("Z58CTL");
                        rackTagItem.OrigShipToNum = segments.GetDecimalAttributeValue("Z5840425");
                        rackTagItem.OrigSoldToName = custNameMgr.GetName(e1Session, rackTagItem.OrigSoldToNum);
                        rackTagItem.OrigShipToName = custNameMgr.GetName(e1Session, rackTagItem.OrigSoldToNum);

                        // Picked Units
                        rackTagItem.PickItemNum = segments.GetDecimalAttributeValue("Z5880230");
                        rackTagItem.PickedUnitsCount = rackTagItem.PickItemNum != 0 ? rackTagItem.Quantity : 0;
                    }
                }

                // Orig Cust PO
                foreach (var rackTag in rackTagItems)
                {
                    if (rackTag.WorkOrderNum > 0 && rackTag.ItemNum > 0)
                    {
                        if (rackTag.SalesOrderType == "ST")
                        {
                            var cmdPOLine = e1Session.DbCommands["RackTagManager.GetRelatedOrderInfo"];
                            cmdPOLine.Parameters[0].Value = rackTag.CompanyNum.PadLeft(5);
                            cmdPOLine.Parameters[1].Value = rackTag.SalesOrderNum;
                            cmdPOLine.Parameters[2].Value = rackTag.SalesOrderType.PadLeft(2);
                            cmdPOLine.Parameters[3].Value = rackTag.SalesOrderLineNum * 1000;
                            cmdPOLine.Connection = e1Session.DbConnection;
                            using (IDataReader readerPOLine = cmdPOLine.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                while (readerPOLine.Read())
                                {
                                    var vr01 = CastDbNull.To(readerPOLine["SDVR01"], string.Empty).Trim();
                                    rackTag.OrigCustomerPO = vr01;
                                    var edtr = CastDbNull.To(readerPOLine["ROEDTR"], string.Empty).Trim();
                                    rackTag.RemakeFlag = (edtr == "R") ? "R" : "";
                                }

                                readerPOLine.Close();
                            }
                        }
                    }
                }

                // Is Coated Workcenter
                foreach (var rackTag in rackTagItems)
                {
                    if (rackTag.WorkOrderNum > 0 && rackTag.ItemNum > 0)
                    {
                        var cmdWc = e1Session.DbCommands["RackTagManager.GetCoatedWorkCenter"];
                        cmdWc.Parameters[0].Value = rackTag.WorkOrderNum;
                        cmdWc.Connection = e1Session.DbConnection;
                        using (IDataReader rdrWc = cmdWc.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            while (rdrWc.Read())
                            {
                                var val = CastDbNull.To(rdrWc["RECS"], 0);
                                rackTag.IsCoated = (val > 0);
                            }

                            rdrWc.Close();
                        }
                    }
                }

                //if (rackTagItems.Any())
                //{
                //    var firstOrDefault = rackTagItems.FirstOrDefault();
                //    var plantNum = firstOrDefault?.PlantNum;
                //    if (plantNum != null)
                //    {
                //        var plantCode = PlantManager.GetPlant(plantNum).PlantCode;
                //        var workOrders = rackTagItems.Select(o => o.WorkOrderNum).ToList().Distinct().ToList();
                //        var dockLocal = RackTagItemManager.GetDockLocal(plantCode, workOrders);
                //        foreach (var rackTagItem in rackTagItems)
                //        {
                //            string dock;
                //            dockLocal.TryGetValue(rackTagItem.WorkOrderNum, out dock);
                //            rackTagItem.DockLocal = dock ?? "";
                //        }

                //        if (plantCode == "MVCG")
                //        {
                //            var hasMate = RackTagItemManager.GetHasMate(plantCode, workOrders);
                //            foreach (var rackTagItem in rackTagItems)
                //            {
                //                string mate;
                //                hasMate.TryGetValue(rackTagItem.BeginSerialNum, out mate);
                //                rackTagItem.HasMate = mate ?? "";
                //            }
                //        }
                //    }
                //}

                // Edi Messages
                foreach (var rackTag in rackTagItems)
                {
                    if (rackTag.WorkOrderNum > 0 && rackTag.ItemNum > 0)
                    {
                        var cmd1 = e1Session.DbCommands["RackTagManager.GetEdiMessages"];
                        cmd1.Parameters[0].Value = rackTag.CompanyNum.PadLeft(5);
                        cmd1.Parameters[1].Value = rackTag.SalesOrderNum;
                        cmd1.Parameters[2].Value = rackTag.SalesOrderType.PadLeft(2);
                        cmd1.Parameters[3].Value = rackTag.SalesOrderLineNum * 1000;
                        cmd1.Connection = e1Session.DbConnection;
                        using (IDataReader reader2 = cmd1.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            while (reader2.Read())
                            {
                                var msgType = CastDbNull.To(reader2["ZDEDTY"], string.Empty).Trim();
                                if (msgType == "R")
                                {
                                    switch (Convert.ToInt32(CastDbNull.To<decimal>(reader2["ZDEDSQ"], 0)))
                                    {
                                        case 1:
                                            rackTag.FooterMsg1 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 2:
                                            rackTag.FooterMsg2 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 3:
                                            rackTag.FooterMsg3 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 4:
                                            rackTag.FooterMsg4 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 5:
                                            rackTag.FooterMsg5 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 6:
                                            rackTag.FooterMsg6 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 7:
                                            rackTag.FooterMsg7 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 8:
                                            rackTag.FooterMsg8 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 9:
                                            rackTag.FooterMsg9 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 10:
                                            rackTag.FooterMsg10 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 11:
                                            rackTag.FooterMsg11 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 12:
                                            rackTag.FooterMsg12 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 13:
                                            rackTag.FooterMsg13 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 14:
                                            rackTag.FooterMsg14 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 15:
                                            rackTag.FooterMsg15 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 16:
                                            rackTag.FooterMsg16 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 17:
                                            rackTag.FooterMsg17 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 18:
                                            rackTag.FooterMsg18 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 19:
                                            rackTag.FooterMsg19 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 20:
                                            rackTag.FooterMsg20 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 21:
                                            rackTag.FooterMsg21 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 22:
                                            rackTag.FooterMsg22 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 23:
                                            rackTag.FooterMsg23 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 24:
                                            rackTag.FooterMsg24 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 25:
                                            rackTag.FooterMsg25 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 26:
                                            rackTag.FooterMsg26 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 27:
                                            rackTag.FooterMsg27 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 28:
                                            rackTag.FooterMsg28 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 29:
                                            rackTag.FooterMsg29 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 30:
                                            rackTag.FooterMsg30 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                    }
                                }
                                else
                                {
                                    if (msgType == "L")
                                    {
                                        switch (Convert.ToInt32(CastDbNull.To<decimal>(reader2["ZDEDSQ"], 0)))
                                        {
                                            case 1:
                                                rackTag.LabelMsg1 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 2:
                                                rackTag.LabelMsg2 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 3:
                                                rackTag.LabelMsg3 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 4:
                                                rackTag.LabelMsg4 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 5:
                                                rackTag.LabelMsg5 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 6:
                                                rackTag.LabelMsg6 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 7:
                                                rackTag.LabelMsg7 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 8:
                                                rackTag.LabelMsg8 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 9:
                                                rackTag.LabelMsg9 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 10:
                                                rackTag.LabelMsg10 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 11:
                                                rackTag.LabelMsg11 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 12:
                                                rackTag.LabelMsg12 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 13:
                                                rackTag.LabelMsg13 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 14:
                                                rackTag.LabelMsg14 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 15:
                                                rackTag.LabelMsg15 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 16:
                                                rackTag.LabelMsg16 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 17:
                                                rackTag.LabelMsg17 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 18:
                                                rackTag.LabelMsg18 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 19:
                                                rackTag.LabelMsg19 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 20:
                                                rackTag.LabelMsg20 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 21:
                                                rackTag.LabelMsg21 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 22:
                                                rackTag.LabelMsg22 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 23:
                                                rackTag.LabelMsg23 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 24:
                                                rackTag.LabelMsg24 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 25:
                                                rackTag.LabelMsg25 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 26:
                                                rackTag.LabelMsg26 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 27:
                                                rackTag.LabelMsg27 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 28:
                                                rackTag.LabelMsg28 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 29:
                                                rackTag.LabelMsg29 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                            case 30:
                                                rackTag.LabelMsg30 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                                break;
                                        }
                                    }
                                }
                            }

                            reader2.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            var sortedItems = rackTagItems.OrderBy(x => x.GroupingKey).ToList();
            var rackTagItems1 = new RackTagItems();
            foreach (var z in sortedItems)
            {
                rackTagItems1.Add(z);
            }

            return rackTagItems1;
        }

        public RackTagItems GetPlacard(
          BusinessSystemSession e1Session,
          SqlConnection sqlCn,
          Plant plant,
          Decimal rackId,
          string strategy)
        {
            RackTagItems rackTagItems1 = new RackTagItems();
            try
            {
                RackTagItems rackTagItems2 = this.GetRackTagItems(e1Session, sqlCn, plant, rackId, strategy);
                RackTagItem rackTagItem1 = new RackTagItem();
                if (rackTagItems2.Any<RackTagItem>())
                {
                    rackTagItem1.RackId = rackTagItems2.FirstOrDefault<RackTagItem>().RackId;
                    rackTagItem1.CompanyNum = rackTagItems2.FirstOrDefault<RackTagItem>().CompanyNum;
                    rackTagItem1.PlantNum = rackTagItems2.FirstOrDefault<RackTagItem>().PlantNum;
                    rackTagItem1.SalesOrderNum = rackTagItems2.FirstOrDefault<RackTagItem>().SalesOrderNum;
                    rackTagItem1.SalesOrderType = rackTagItems2.FirstOrDefault<RackTagItem>().SalesOrderType;
                    rackTagItem1.SalesOrderLineNum = rackTagItems2.FirstOrDefault<RackTagItem>().SalesOrderLineNum;
                    rackTagItem1.WorkOrderNum = rackTagItems2.FirstOrDefault<RackTagItem>().WorkOrderNum;
                    rackTagItem1.LotNum = rackTagItems2.FirstOrDefault<RackTagItem>().LotNum;
                    rackTagItem1.BeginSerialNum = rackTagItems2.FirstOrDefault<RackTagItem>().BeginSerialNum;
                    rackTagItem1.EndSerialNum = rackTagItems2.Select<RackTagItem, string>((Func<RackTagItem, string>)(x => x.BeginSerialNum)).Last<string>();
                    rackTagItem1.ItemNum = rackTagItems2.FirstOrDefault<RackTagItem>().ItemNum;
                    rackTagItem1.SoldToNum = rackTagItems2.FirstOrDefault<RackTagItem>().SoldToNum;
                    rackTagItem1.SoldToName = rackTagItems2.FirstOrDefault<RackTagItem>().SoldToName;
                    rackTagItem1.BlindDocumentFlag = rackTagItems2.FirstOrDefault<RackTagItem>().BlindDocumentFlag;
                    rackTagItem1.ShipToNum = rackTagItems2.FirstOrDefault<RackTagItem>().ShipToNum;
                    rackTagItem1.ShipToName = rackTagItems2.FirstOrDefault<RackTagItem>().ShipToName;
                    rackTagItem1.CustPartNum1 = rackTagItems2.FirstOrDefault<RackTagItem>().CustPartNum1;
                    rackTagItem1.CustPartNum2 = rackTagItems2.FirstOrDefault<RackTagItem>().CustPartNum2;
                    rackTagItem1.CustPartNum3 = rackTagItems2.FirstOrDefault<RackTagItem>().CustPartNum3;
                    rackTagItem1.CustSeqNum1 = rackTagItems2.FirstOrDefault<RackTagItem>().CustSeqNum1;
                    rackTagItem1.GenericSeqNum = rackTagItems2.FirstOrDefault<RackTagItem>().GenericSeqNum;
                    rackTagItem1.RackType = rackTagItems2.FirstOrDefault<RackTagItem>().RackType;
                    rackTagItem1.RackTypeDesc = rackTagItems2.FirstOrDefault<RackTagItem>().RackTypeDesc;
                    rackTagItem1.Location = rackTagItems2.FirstOrDefault<RackTagItem>().Location;
                    rackTagItem1.Status = rackTagItems2.FirstOrDefault<RackTagItem>().Status;
                    rackTagItem1.RackStatus = rackTagItems2.FirstOrDefault<RackTagItem>().RackStatus;
                    rackTagItem1.RackItemStatus = rackTagItems2.FirstOrDefault<RackTagItem>().RackItemStatus;
                    rackTagItem1.StatusDate = rackTagItems2.FirstOrDefault<RackTagItem>().StatusDate;
                    rackTagItem1.BoxNum = rackTagItems2.FirstOrDefault<RackTagItem>().BoxNum;
                    rackTagItem1.RackGlassWeight = rackTagItems2.FirstOrDefault<RackTagItem>().RackGlassWeight;
                    rackTagItem1.RackTareWeight = rackTagItems2.FirstOrDefault<RackTagItem>().RackTareWeight;
                    rackTagItem1.ScheduleNum = rackTagItems2.FirstOrDefault<RackTagItem>().ScheduleNum;
                    rackTagItem1.OriginalScheduleNum = rackTagItems2.FirstOrDefault<RackTagItem>().OriginalScheduleNum;
                    rackTagItem1.ScheduleType = rackTagItems2.FirstOrDefault<RackTagItem>().ScheduleType;
                    rackTagItem1.CustomerPO = rackTagItems2.FirstOrDefault<RackTagItem>().CustomerPO;
                    rackTagItem1.ReasonCode = rackTagItems2.FirstOrDefault<RackTagItem>().ReasonCode;
                    rackTagItem1.PriorityCode = rackTagItems2.FirstOrDefault<RackTagItem>().PriorityCode;
                    rackTagItem1.DockLocal = rackTagItems2.FirstOrDefault<RackTagItem>().DockLocal;
                    rackTagItem1.Dock = rackTagItems2.FirstOrDefault<RackTagItem>().Dock;
                    rackTagItem1.BoxingMessage = rackTagItems2.FirstOrDefault<RackTagItem>().BoxingMessage;
                    rackTagItem1.ShopFloorMessage1 = rackTagItems2.FirstOrDefault<RackTagItem>().ShopFloorMessage1;
                    rackTagItem1.ShopFloorMessage2 = rackTagItems2.FirstOrDefault<RackTagItem>().ShopFloorMessage2;
                    rackTagItem1.PickNotes = rackTagItems2.FirstOrDefault<RackTagItem>().PickNotes;
                    rackTagItem1.PackNotes = rackTagItems2.FirstOrDefault<RackTagItem>().PackNotes;
                    rackTagItem1.DueDate = rackTagItems2.Min<RackTagItem, DateTime>((Func<RackTagItem, DateTime>)(x => x.DueDate));
                    rackTagItem1.BoxRotated = rackTagItems2.FirstOrDefault<RackTagItem>().BoxRotated;
                    rackTagItem1.RackSide = rackTagItems2.FirstOrDefault<RackTagItem>().RackSide;
                    rackTagItem1.RackRow = rackTagItems2.FirstOrDefault<RackTagItem>().RackRow;
                    rackTagItem1.RackStack = rackTagItems2.FirstOrDefault<RackTagItem>().RackStack;
                    rackTagItem1.RackPosition = rackTagItems2.FirstOrDefault<RackTagItem>().RackPosition;
                    rackTagItem1.RackSlot = rackTagItems2.FirstOrDefault<RackTagItem>().RackSlot;
                    rackTagItem1.RackSlotPosition = rackTagItems2.FirstOrDefault<RackTagItem>().RackSlotPosition;
                    rackTagItem1.GroupingKey = rackTagItems2.FirstOrDefault<RackTagItem>().GroupingKey;
                    rackTagItem1.Lite1Rack = rackTagItems2.FirstOrDefault<RackTagItem>().Lite1Rack;
                    rackTagItem1.Lite1Slot = rackTagItems2.FirstOrDefault<RackTagItem>().Lite1Slot;
                    rackTagItem1.Lite2Rack = rackTagItems2.FirstOrDefault<RackTagItem>().Lite2Rack;
                    rackTagItem1.Lite2Slot = rackTagItems2.FirstOrDefault<RackTagItem>().Lite2Slot;
                    rackTagItem1.Lite3Rack = rackTagItems2.FirstOrDefault<RackTagItem>().Lite3Rack;
                    rackTagItem1.Lite3Slot = rackTagItems2.FirstOrDefault<RackTagItem>().Lite3Slot;
                    rackTagItem1.Lite4Rack = rackTagItems2.FirstOrDefault<RackTagItem>().Lite4Rack;
                    rackTagItem1.Lite4Slot = rackTagItems2.FirstOrDefault<RackTagItem>().Lite4Slot;
                    rackTagItem1.RemakeRack = rackTagItems2.FirstOrDefault<RackTagItem>().RemakeRack;
                    rackTagItem1.RemakeSlot = rackTagItems2.FirstOrDefault<RackTagItem>().RemakeSlot;
                    rackTagItem1.ConfigId = rackTagItems2.FirstOrDefault<RackTagItem>().ConfigId;
                    rackTagItem1.ProductType = rackTagItems2.FirstOrDefault<RackTagItem>().ProductType;
                    rackTagItem1.PrinterNum = rackTagItems2.FirstOrDefault<RackTagItem>().PrinterNum;
                    rackTagItem1.Quantity = rackTagItems2.Sum<RackTagItem>((Func<RackTagItem, int>)(x => x.Quantity));
                    if (rackTagItem1.SalesOrderType == "ST")
                    {
                        foreach (RackTagItem rackTagItem2 in (List<RackTagItem>)rackTagItems2)
                        {
                            string[] strArray = rackTagItem2.CustSeqNum1.Split(',');
                            if (strArray.Length > 1)
                                rackTagItem2.Dock = strArray[0];
                        }
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (IGrouping<string, RackTagItem> source in rackTagItems2.GroupBy<RackTagItem, string>((Func<RackTagItem, string>)(x => x.Dock)))
                            stringBuilder.Append(string.Format("{0} [{1}] ", (object)source.Key, (object)source.Sum<RackTagItem>((Func<RackTagItem, int>)(x => x.Quantity))));
                        rackTagItem1.DockLocal = stringBuilder.ToString();
                    }
                    rackTagItems1.Add(rackTagItem1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rackTagItems1;
        }

        /******************************************************************
        * Grouping Strategies
        ******************************************************************/
        private void SetGroupByWorkOrderInterleaved(RackTagItems inputItems)
        {
            try
            {
                var g = inputItems.GroupBy(x => new
                {
                    x.WorkOrderNum,
                    x.RackSide
                });
                foreach (var g1 in g)
                {
                    decimal assignSlot = 0;
                    decimal prevSlot = 0;
                    foreach (var g2 in g1.OrderBy(x => x.RackSide).ThenBy(x1 => x1.RackRow).ThenBy(x2 => x2.RackStack).ThenBy(x3 => x3.RackSlotPosition))
                    {
                        // first item
                        if (assignSlot == 0)
                        {
                            assignSlot = g2.RackSlotPosition;
                            prevSlot = g2.RackSlotPosition;
                            g2.GroupingKey = $"{g2.RackSide.PadLeft(2, '0')}:{g2.RackRow.ToString().PadLeft(2, '0')}:{g2.RackStack.ToString().PadLeft(2, '0')}:{assignSlot.ToString().PadLeft(3, '0')}:{g2.WorkOrderNum}";
                        }
                        else
                        {
                            if (g2.RackSlotPosition == prevSlot + 1)
                            {
                                g2.GroupingKey = $"{g2.RackSide.PadLeft(2, '0')}:{g2.RackRow.ToString().PadLeft(2, '0')}:{g2.RackStack.ToString().PadLeft(2, '0')}:{assignSlot.ToString().PadLeft(3, '0')}:{g2.WorkOrderNum}";
                                prevSlot = g2.RackSlotPosition;
                            }
                            else
                            {
                                g2.GroupingKey = $"{g2.RackSide.PadLeft(2, '0')}:{g2.RackRow.ToString().PadLeft(2, '0')}:{g2.RackStack.ToString().PadLeft(2, '0')}:{g2.RackSlotPosition.ToString().PadLeft(3, '0')}:{g2.WorkOrderNum}:{g2.BeginSerialNum}";
                                prevSlot = g2.RackSlotPosition;
                                assignSlot = g2.RackSlotPosition;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //private void SetGroupByWorkOrderInterleaved(RackTagItems inputItems)
        //{
        //    try
        //    {
        //        var g = inputItems.GroupBy(x => new {x.WorkOrderNum, x.RackSide});
        //        foreach (var g1 in g)
        //        {
        //            decimal assignSlot = 0;
        //            decimal prevSlot = 0;
        //            //string prevSide = "";
        //            //foreach (var g2 in g1.OrderBy(x => x.RackSlotPosition))
        //            //foreach (var g2 in g1.OrderBy(x => new {x.RackSide, x.RackRow, x.RackStack, x.RackSlotPosition}))
        //            foreach (var g2 in g1.OrderBy(x => x.RackSide).ThenBy(x1 => x1.RackRow).ThenBy(x2 => x2.RackStack).ThenBy(x3 => x3.RackSlotPosition))
        //            {
        //                // first item
        //                if (assignSlot == 0)
        //                {
        //                    assignSlot = g2.RackSlotPosition;
        //                    prevSlot = g2.RackSlotPosition;
        //                    //prevSide = g2.RackSide;
        //                    g2.GroupingKey = $"{g2.RackSide.PadLeft(2, '0')}:{g2.RackRow.ToString().PadLeft(2, '0')}:{g2.RackStack.ToString().PadLeft(2, '0')}:{assignSlot.ToString().PadLeft(3, '0')}";
        //                }
        //                else
        //                {
        //                    if ( g2.RackSlotPosition == prevSlot + 1)
        //                       // if (g2.RackSide == prevSide && g2.RackSlotPosition == prevSlot + 1)
        //                        {
        //                        g2.GroupingKey = $"{g2.RackSide.PadLeft(2, '0')}:{g2.RackRow.ToString().PadLeft(2, '0')}:{g2.RackStack.ToString().PadLeft(2, '0')}:{assignSlot.ToString().PadLeft(3, '0')}";
        //                        prevSlot = g2.RackSlotPosition;
        //                        //prevSide = g2.RackSide;
        //                    }
        //                    else
        //                    {
        //                        g2.GroupingKey = $"{g2.RackSide.PadLeft(2, '0')}:{g2.RackRow.ToString().PadLeft(2, '0')}:{g2.RackStack.ToString().PadLeft(2, '0')}:{g2.RackSlotPosition.ToString().PadLeft(3, '0')}:{g2.BeginSerialNum}";
        //                        prevSlot = g2.RackSlotPosition;
        //                        //prevSide = g2.RackSide;
        //                        assignSlot = g2.RackSlotPosition;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        private void SetGroupByWorkOrder(RackTagItems inputItems)
        {
            try
            {
                var g = inputItems.GroupBy(x => x.WorkOrderNum);
                foreach (var g1 in g)
                {
                    var firstUnit = g1.First();
                    foreach (var g2 in g1)
                    {
                        g2.GroupingKey = $"{firstUnit.RackSide.PadLeft(2, '0')}:{firstUnit.RackRow.ToString().PadLeft(2, '0')}:{firstUnit.RackStack.ToString().PadLeft(2, '0')}:{firstUnit.RackSlotPosition.ToString().PadLeft(3, '0')}:{firstUnit.BeginSerialNum}";
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void SetGroupByUnit(RackTagItems inputItems)
        {
            try
            {
                var g = inputItems.GroupBy(x => new
                {
                    x.WorkOrderNum,
                    x.BeginSerialNum,
                    x.RackSlotPosition
                });
                foreach (var g1 in g)
                {
                    var firstUnit = g1.First();
                    foreach (var g2 in g1)
                    {
                        g2.GroupingKey = $"{firstUnit.RackSide.PadLeft(2, '0')}:{firstUnit.RackRow.ToString().PadLeft(2, '0')}:{firstUnit.RackStack.ToString().PadLeft(2, '0')}:{firstUnit.RackSlotPosition.ToString().PadLeft(3, '0')}:{firstUnit.BeginSerialNum}";
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
        * Grouping Strategies Old
        ******************************************************************/
        private void SetGroupByWorkOrderInterleavedOld(RackTagItems inputItems)
        {
            try
            {
                var g = inputItems.GroupBy(x => x.WorkOrderNum);
                foreach (var g1 in g)
                {
                    decimal assignSlot = 0;
                    decimal prevSlot = 0;
                    foreach (var g2 in g1)
                    {
                        // first item
                        if (assignSlot == 0)
                        {
                            assignSlot = g2.RackSlotPosition;
                            prevSlot = g2.RackSlotPosition;
                            g2.GroupingKey = $"{g2.RackSide}:{g2.RackRow}:{g2.RackStack}:{assignSlot}";
                        }
                        else
                        {
                            if (g2.RackSlotPosition == prevSlot + 1)
                            {
                                g2.GroupingKey = $"{g2.RackSide}:{g2.RackRow}:{g2.RackStack}:{assignSlot}";
                                prevSlot = g2.RackSlotPosition;
                            }
                            else
                            {
                                g2.GroupingKey = $"{g2.RackSide}:{g2.RackRow}:{g2.RackStack}:{g2.RackSlotPosition}";
                                prevSlot = g2.RackSlotPosition;
                                assignSlot = g2.RackSlotPosition;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void SetGroupByWorkOrderOld(RackTagItems inputItems)
        {
            try
            {
                var g = inputItems.GroupBy(x => x.WorkOrderNum);
                foreach (var g1 in g)
                {
                    var firstUnit = g1.First();
                    foreach (var g2 in g1)
                    {
                        g2.GroupingKey = $"{firstUnit.RackSide}:{firstUnit.RackRow}:{firstUnit.RackStack}:{firstUnit.RackSlotPosition}:{firstUnit.WorkOrderNum}";
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void SetGroupByUnitOld(RackTagItems inputItems)
        {
            try
            {
                var g = inputItems.GroupBy(x => new
                {
                    x.WorkOrderNum,
                    x.BeginSerialNum
                });
                foreach (var g1 in g)
                {
                    var firstUnit = g1.First();
                    foreach (var g2 in g1)
                    {
                        //g2.GroupingKey = $"{firstUnit.RackSide}:{firstUnit.RackRow}:{firstUnit.RackStack}:{firstUnit.RackSlotPosition}";
                        g2.GroupingKey = $"{firstUnit.RackSide}:{firstUnit.RackRow}:{firstUnit.RackStack}:{firstUnit.RackSlotPosition}:{firstUnit.BeginSerialNum}";
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}