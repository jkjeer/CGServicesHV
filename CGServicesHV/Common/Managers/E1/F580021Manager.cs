using System;
using System.Data;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.E1
{
    public class F580021Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static iDB2Command GetF580021s()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT
                        *
                    FROM (
                        SELECT 
                            F580021.*,
                            COALESCE(CMCFGID, 0) AS CMCFGID,
                            COALESCE(CMUPMJ, 0) AS CMUPMJ,
                            COALESCE(CMTDAY, 0) AS CMTDAY,
                            WASRST,
                            IBANPL,
                            ABALPH,
                            IBGLPT,
                            IBUPMJ,
                            IBTDAY,
                            ROW_NUMBER() OVER (PARTITION BY S7EDLN, S7DOCO, CMDCTO, CMKCOO ORDER BY CMUPMJ DESC, CMTDAY DESC) AS ROWNUM
                        FROM
                            F580021
                            LEFT OUTER JOIN F3201 
                            ON  CMDOCO = S7DOCO AND
                                CMKCOO = '*' AND
                                CMDCTO = 'WO' AND
                                CMPID != 'P580150' AND 
                                CMUPMJ > 117000
                            LEFT OUTER JOIN F4801
                            ON  WADOCO = S7DOCO
                            LEFT OUTER JOIN F4102 ON IBMCU = S7MCU
                                                 AND IBITM = S7ITM
				            LEFT OUTER JOIN F0101 ON IBANPL = ABAN8 
                        WHERE                      
                            S7MCU = ? AND
                            S7Z58SCN = ? AND
                            S7EDBT = ?
                    ) A
                    WHERE A.ROWNUM = 1
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("S7MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("S7Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S7Z58SCN"].Precision = 5;
            cmd.Parameters["S7Z58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("S7EDBT", iDB2DbType.iDB2Graphic, 15));
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static F580021s GetF580021s(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var f580021s = new F580021s();
            try
            {
                var cmd = GetF580021s();
                cmd.Parameters["S7MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["S7Z58SCN"].Value = scheduleNum;
                cmd.Parameters["S7EDBT"].Value = batchNum;
                cmd.Connection = e1Session.DbConnection;
                cmd.CommandTimeout = 0;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var f580021 = new F580021
                        {
                            S7EDBT = Data.CastDbNull.To(reader["S7EDBT"], string.Empty).Trim(),
                            S7EDLN = Data.CastDbNull.To<decimal>(reader["S7EDLN"], 0),
                            S7MCU = Data.CastDbNull.To(reader["S7MCU"], string.Empty).Trim(),
                            S7Z58SCN = Data.CastDbNull.To<decimal>(reader["S7Z58SCN"], 0),
                            S7Z58SCSN = Data.CastDbNull.To<decimal>(reader["S7Z58SCSN"], 0),
                            S7Z58SCSTS = Data.CastDbNull.To(reader["S7Z58SCSTS"], string.Empty).Trim(),
                            S7EDSP = Data.CastDbNull.To(reader["S7EDSP"], string.Empty).Trim(),
                            S7RORN = Data.CastDbNull.To(reader["S7RORN"], string.Empty).Trim(),
                            S7RCTO = Data.CastDbNull.To(reader["S7RCTO"], string.Empty).Trim(),
                            S7LNID = Data.CastDbNull.To<decimal>(reader["S7LNID"], 0),
                            S7DOCO = Data.CastDbNull.To<decimal>(reader["S7DOCO"], 0),
                            S7DCTO = Data.CastDbNull.To(reader["S7DCTO"], string.Empty).Trim(),
                            S7SHAN = Data.CastDbNull.To<decimal>(reader["S7SHAN"], 0),
                            S7ALPH = Data.CastDbNull.To(reader["S7ALPH"], string.Empty).Trim(),
                            S7VR01 = Data.CastDbNull.To(reader["S7VR01"], string.Empty).Trim(),
                            S7VR02 = Data.CastDbNull.To(reader["S7VR02"], string.Empty).Trim(),
                            S7VR03 = Data.CastDbNull.To(reader["S7VR03"], string.Empty).Trim(),
                            S7Z58SCD = Data.CastDbNull.To<decimal>(reader["S7Z58SCD"], 0),
                            S7Z58SCTYP = Data.CastDbNull.To(reader["S7Z58SCTYP"], string.Empty).Trim(),
                            S7Z58SCDDJ = Data.CastDbNull.To<decimal>(reader["S7Z58SCDDJ"], 0),
                            S7Z58SAREA = Data.CastDbNull.To(reader["S7Z58SAREA"], string.Empty).Trim(),
                            S7ITM = Data.CastDbNull.To<decimal>(reader["S7ITM"], 0),
                            S7LITM = Data.CastDbNull.To(reader["S7LITM"], string.Empty).Trim(),
                            S7DSC1 = Data.CastDbNull.To(reader["S7DSC1"], string.Empty).Trim(),
                            S7DSC2 = Data.CastDbNull.To(reader["S7DSC2"], string.Empty).Trim(),
                            S7CITM = Data.CastDbNull.To(reader["S7CITM"], string.Empty).Trim(),
                            S7Z58CP2 = Data.CastDbNull.To(reader["S7Z58CP2"], string.Empty).Trim(),
                            S7Z58CP3 = Data.CastDbNull.To(reader["S7Z58CP3"], string.Empty).Trim(),
                            S7Z58CP4 = Data.CastDbNull.To(reader["S7Z58CP4"], string.Empty).Trim(),
                            S7Z58QTY = Data.CastDbNull.To<decimal>(reader["S7Z58QTY"], 0),
                            S7UOM = Data.CastDbNull.To(reader["S7UOM"], string.Empty).Trim(),
                            S7RSDJ = Data.CastDbNull.To<decimal>(reader["S7RSDJ"], 0),
                            S7RSDT = Data.CastDbNull.To<decimal>(reader["S7RSDT"], 0),
                            S7PRIO = Data.CastDbNull.To(reader["S7PRIO"], string.Empty).Trim(),
                            S7Z58GAL1 = Data.CastDbNull.To(reader["S7Z58GAL1"], string.Empty).Trim(),
                            S7Z58GAL2 = Data.CastDbNull.To(reader["S7Z58GAL2"], string.Empty).Trim(),
                            S7Z58ML = Data.CastDbNull.To<decimal>(reader["S7Z58ML"], 0),
                            S7Z58CSQNO = Data.CastDbNull.To(reader["S7Z58CSQNO"], string.Empty).Trim(),
                            S7Z58GSQNO = Data.CastDbNull.To(reader["S7Z58GSQNO"], string.Empty).Trim(),
                            S7Z58PT = Data.CastDbNull.To(reader["S7Z58PT"], string.Empty).Trim(),
                            S7Z58DRAW = Data.CastDbNull.To(reader["S7Z58DRAW"], string.Empty).Trim(),
                            S7Z58ZPO = Data.CastDbNull.To(reader["S7Z58ZPO"], string.Empty).Trim(),
                            S7AN8DR = Data.CastDbNull.To<decimal>(reader["S7AN8DR"], 0),
                            S7Z58CRATE = Data.CastDbNull.To(reader["S7Z58CRATE"], string.Empty).Trim(),
                            S7Z58CTL = Data.CastDbNull.To<decimal>(reader["S7Z58CTL"], 0),
                            S7Z58FLIP = Data.CastDbNull.To(reader["S7Z58FLIP"], string.Empty).Trim(),
                            S7Z58TBAD = Data.CastDbNull.To(reader["S7Z58TBAD"], string.Empty).Trim(),
                            S7Z58PVAD = Data.CastDbNull.To(reader["S7Z58PVAD"], string.Empty).Trim(),
                            S7Z58SCAD = Data.CastDbNull.To(reader["S7Z58SCAD"], string.Empty).Trim(),
                            S7Z58IGLN = Data.CastDbNull.To(reader["S7Z58IGLN"], string.Empty).Trim(),
                            S7Z58BOXP = Data.CastDbNull.To(reader["S7Z58BOXP"], string.Empty).Trim(),
                            S7Z58BGEE = Data.CastDbNull.To(reader["S7Z58BGEE"], string.Empty).Trim(),
                            S7Z58BGSZ = Data.CastDbNull.To<decimal>(reader["S7Z58BGSZ"], 0),
                            S7Z58IGCLN = Data.CastDbNull.To<decimal>(reader["S7Z58IGCLN"], 0),
                            S7Z58CSCNO = Data.CastDbNull.To(reader["S7Z58CSCNO"], string.Empty).Trim(),
                            S7LINN = Data.CastDbNull.To<decimal>(reader["S7LINN"], 0),
                            S7Z58L1RG = Data.CastDbNull.To(reader["S7Z58L1RG"], string.Empty).Trim(),
                            S7Z58L2RG = Data.CastDbNull.To(reader["S7Z58L2RG"], string.Empty).Trim(),
                            S7Z58L3RG = Data.CastDbNull.To(reader["S7Z58L3RG"], string.Empty).Trim(),
                            S7Z58MSG1 = Data.CastDbNull.To(reader["S7Z58MSG1"], string.Empty).Trim(),
                            S7Z58MSG2 = Data.CastDbNull.To(reader["S7Z58MSG2"], string.Empty).Trim(),
                            S7Z58L1QOH = Data.CastDbNull.To<decimal>(reader["S7Z58L1QOH"], 0),
                            S7Z58L2QOH = Data.CastDbNull.To<decimal>(reader["S7Z58L2QOH"], 0),
                            S7Z58L3QOH = Data.CastDbNull.To<decimal>(reader["S7Z58L3QOH"], 0),
                            S7Z58L4QOH = Data.CastDbNull.To<decimal>(reader["S7Z58L4QOH"], 0),
                            S7Z58FLRN = Data.CastDbNull.To(reader["S7Z58FLRN"], string.Empty).Trim(),
                            S7Z58FLTN = Data.CastDbNull.To(reader["S7Z58FLTN"], string.Empty).Trim(),
                            S7Z58SLRN = Data.CastDbNull.To(reader["S7Z58SLRN"], string.Empty).Trim(),
                            S7Z58SLTN = Data.CastDbNull.To(reader["S7Z58SLTN"], string.Empty).Trim(),
                            S7Z58TLRN = Data.CastDbNull.To(reader["S7Z58TLRN"], string.Empty).Trim(),
                            S7Z58TLTN = Data.CastDbNull.To(reader["S7Z58TLTN"], string.Empty).Trim(),
                            S7Z58CHGN = Data.CastDbNull.To(reader["S7Z58CHGN"], string.Empty).Trim(),
                            S7Z58RT = Data.CastDbNull.To(reader["S7Z58RT"], string.Empty).Trim(),
                            S7Z58ROVR = Data.CastDbNull.To(reader["S7Z58ROVR"], string.Empty).Trim(),
                            S7Z58RKS = Data.CastDbNull.To<decimal>(reader["S7Z58RKS"], 0),
                            S7Z58CRMQ = Data.CastDbNull.To<decimal>(reader["S7Z58CRMQ"], 0),
                            S7Z58CRXQ = Data.CastDbNull.To<decimal>(reader["S7Z58CRXQ"], 0),
                            S7Z58PPR = Data.CastDbNull.To<decimal>(reader["S7Z58PPR"], 0),
                            S7Z58LPP = Data.CastDbNull.To<decimal>(reader["S7Z58LPP"], 0),
                            S7Z58PMQ = Data.CastDbNull.To<decimal>(reader["S7Z58PMQ"], 0),
                            S7Z58PXQ = Data.CastDbNull.To<decimal>(reader["S7Z58PXQ"], 0),
                            S7Z58MTRW = Data.CastDbNull.To<decimal>(reader["S7Z58MTRW"], 0),
                            S7UWUM = Data.CastDbNull.To(reader["S7UWUM"], string.Empty).Trim(),
                            S7GPUS = Data.CastDbNull.To(reader["S7GPUS"], string.Empty).Trim(),
                            S7Z58LPR = Data.CastDbNull.To<decimal>(reader["S7Z58LPR"], 0),
                            S7Z58MC = Data.CastDbNull.To(reader["S7Z58MC"], string.Empty).Trim(),
                            S7Z58CC = Data.CastDbNull.To(reader["S7Z58CC"], string.Empty).Trim(),
                            S7Z58TKY = Data.CastDbNull.To(reader["S7Z58TKY"], string.Empty).Trim(),
                            S7Z58CMT1 = Data.CastDbNull.To(reader["S7Z58CMT1"], string.Empty).Trim(),
                            S7Z58CMT2 = Data.CastDbNull.To(reader["S7Z58CMT2"], string.Empty).Trim(),
                            S7Z58CMT3 = Data.CastDbNull.To(reader["S7Z58CMT3"], string.Empty).Trim(),
                            S7Z58L1TK = Data.CastDbNull.To<decimal>(reader["S7Z58L1TK"], 0),
                            S7Z58L11C = Data.CastDbNull.To(reader["S7Z58L11C"], string.Empty).Trim(),
                            S7Z58L1CL = Data.CastDbNull.To(reader["S7Z58L1CL"], string.Empty).Trim(),
                            S7Z58L1HT = Data.CastDbNull.To(reader["S7Z58L1HT"], string.Empty).Trim(),
                            S7Z58L12C = Data.CastDbNull.To(reader["S7Z58L12C"], string.Empty).Trim(),
                            S7Z58L1PX = Data.CastDbNull.To(reader["S7Z58L1PX"], string.Empty).Trim(),
                            S7Z58L1LPT = Data.CastDbNull.To(reader["S7Z58L1LPT"], string.Empty).Trim(),
                            S7Z58SHAPE = Data.CastDbNull.To(reader["S7Z58SHAPE"], string.Empty).Trim(),
                            S7Z58L1BDM = Data.CastDbNull.To<decimal>(reader["S7Z58L1BDM"], 0),
                            S7Z58L1BNM = Data.CastDbNull.To<decimal>(reader["S7Z58L1BNM"], 0),
                            S7Z58L1BDN = Data.CastDbNull.To<decimal>(reader["S7Z58L1BDN"], 0),
                            S7Z58L1LDM = Data.CastDbNull.To<decimal>(reader["S7Z58L1LDM"], 0),
                            S7Z58L1LNM = Data.CastDbNull.To<decimal>(reader["S7Z58L1LNM"], 0),
                            S7Z58L1LDN = Data.CastDbNull.To<decimal>(reader["S7Z58L1LDN"], 0),
                            S7Z58L1RDM = Data.CastDbNull.To<decimal>(reader["S7Z58L1RDM"], 0),
                            S7Z58L1RNM = Data.CastDbNull.To<decimal>(reader["S7Z58L1RNM"], 0),
                            S7Z58L1RDN = Data.CastDbNull.To<decimal>(reader["S7Z58L1RDN"], 0),
                            S7Z58L1TDM = Data.CastDbNull.To<decimal>(reader["S7Z58L1TDM"], 0),
                            S7Z58L1TNM = Data.CastDbNull.To<decimal>(reader["S7Z58L1TNM"], 0),
                            S7Z58L1TDN = Data.CastDbNull.To<decimal>(reader["S7Z58L1TDN"], 0),
                            S7Z58L11DM = Data.CastDbNull.To<decimal>(reader["S7Z58L11DM"], 0),
                            S7Z58L11NM = Data.CastDbNull.To<decimal>(reader["S7Z58L11NM"], 0),
                            S7Z58L11DN = Data.CastDbNull.To<decimal>(reader["S7Z58L11DN"], 0),
                            S7Z58L12DM = Data.CastDbNull.To<decimal>(reader["S7Z58L12DM"], 0),
                            S7Z58L12NM = Data.CastDbNull.To<decimal>(reader["S7Z58L12NM"], 0),
                            S7Z58L12DN = Data.CastDbNull.To<decimal>(reader["S7Z58L12DN"], 0),
                            S7Z581BDEC = Data.CastDbNull.To<decimal>(reader["S7Z581BDEC"], 0),
                            S7Z581LDEC = Data.CastDbNull.To<decimal>(reader["S7Z581LDEC"], 0),
                            S7Z581RDEC = Data.CastDbNull.To<decimal>(reader["S7Z581RDEC"], 0),
                            S7Z581TDEC = Data.CastDbNull.To<decimal>(reader["S7Z581TDEC"], 0),
                            S7Z5811DEC = Data.CastDbNull.To<decimal>(reader["S7Z5811DEC"], 0),
                            S7Z5812DEC = Data.CastDbNull.To<decimal>(reader["S7Z5812DEC"], 0),
                            S7Z581BMIM = Data.CastDbNull.To<decimal>(reader["S7Z581BMIM"], 0),
                            S7Z581LMIM = Data.CastDbNull.To<decimal>(reader["S7Z581LMIM"], 0),
                            S7Z581RMIM = Data.CastDbNull.To<decimal>(reader["S7Z581RMIM"], 0),
                            S7Z581TMIM = Data.CastDbNull.To<decimal>(reader["S7Z581TMIM"], 0),
                            S7Z5811MIM = Data.CastDbNull.To<decimal>(reader["S7Z5811MIM"], 0),
                            S7Z5812MIM = Data.CastDbNull.To<decimal>(reader["S7Z5812MIM"], 0),
                            S7Z58L1VG = Data.CastDbNull.To(reader["S7Z58L1VG"], string.Empty).Trim(),
                            S7Z58L1VGS = Data.CastDbNull.To<decimal>(reader["S7Z58L1VGS"], 0),
                            S7Z58L1VGT = Data.CastDbNull.To(reader["S7Z58L1VGT"], string.Empty).Trim(),
                            S7Z58L1VGW = Data.CastDbNull.To<decimal>(reader["S7Z58L1VGW"], 0),
                            S7Z58L1VGH = Data.CastDbNull.To<decimal>(reader["S7Z58L1VGH"], 0),
                            S7Z58L1VGD = Data.CastDbNull.To(reader["S7Z58L1VGD"], string.Empty).Trim(),
                            S7Z58L1VGO = Data.CastDbNull.To<decimal>(reader["S7Z58L1VGO"], 0),
                            S7Z58L1BL = Data.CastDbNull.To(reader["S7Z58L1BL"], string.Empty).Trim(),
                            S7Z58L1HL = Data.CastDbNull.To(reader["S7Z58L1HL"], string.Empty).Trim(),
                            S7Z581365 = Data.CastDbNull.To<decimal>(reader["S7Z581365"], 0),
                            S7Z58L1SF = Data.CastDbNull.To<decimal>(reader["S7Z58L1SF"], 0),
                            S7Z58L1SD = Data.CastDbNull.To(reader["S7Z58L1SD"], string.Empty).Trim(),
                            S7Z58L1EW = Data.CastDbNull.To(reader["S7Z58L1EW"], string.Empty).Trim(),
                            S7Z58L1PQL = Data.CastDbNull.To(reader["S7Z58L1PQL"], string.Empty).Trim(),
                            S7Z58L1TL = Data.CastDbNull.To(reader["S7Z58L1TL"], string.Empty).Trim(),
                            S7Z58L1ALT = Data.CastDbNull.To(reader["S7Z58L1ALT"], string.Empty).Trim(),
                            S7Z58L1LX = Data.CastDbNull.To<decimal>(reader["S7Z58L1LX"], 0),
                            S7Z58L1LY = Data.CastDbNull.To<decimal>(reader["S7Z58L1LY"], 0),
                            S7Z58L1CLN = Data.CastDbNull.To<decimal>(reader["S7Z58L1CLN"], 0),
                            S7Z58L1PTS = Data.CastDbNull.To<decimal>(reader["S7Z58L1PTS"], 0),
                            S7Z58L1LCS = Data.CastDbNull.To<decimal>(reader["S7Z58L1LCS"], 0),
                            S7Z58L2TK = Data.CastDbNull.To<decimal>(reader["S7Z58L2TK"], 0),
                            S7Z58L23C = Data.CastDbNull.To(reader["S7Z58L23C"], string.Empty).Trim(),
                            S7Z58L2CL = Data.CastDbNull.To(reader["S7Z58L2CL"], string.Empty).Trim(),
                            S7Z58L2HT = Data.CastDbNull.To(reader["S7Z58L2HT"], string.Empty).Trim(),
                            S7Z58L24C = Data.CastDbNull.To(reader["S7Z58L24C"], string.Empty).Trim(),
                            S7Z58L2PX = Data.CastDbNull.To(reader["S7Z58L2PX"], string.Empty).Trim(),
                            S7Z58L2LPT = Data.CastDbNull.To(reader["S7Z58L2LPT"], string.Empty).Trim(),
                            S7Z58L2BDM = Data.CastDbNull.To<decimal>(reader["S7Z58L2BDM"], 0),
                            S7Z58L2BNM = Data.CastDbNull.To<decimal>(reader["S7Z58L2BNM"], 0),
                            S7Z58L2BDN = Data.CastDbNull.To<decimal>(reader["S7Z58L2BDN"], 0),
                            S7Z58L2LDM = Data.CastDbNull.To<decimal>(reader["S7Z58L2LDM"], 0),
                            S7Z58L2LNM = Data.CastDbNull.To<decimal>(reader["S7Z58L2LNM"], 0),
                            S7Z58L2LDN = Data.CastDbNull.To<decimal>(reader["S7Z58L2LDN"], 0),
                            S7Z58L2RDM = Data.CastDbNull.To<decimal>(reader["S7Z58L2RDM"], 0),
                            S7Z58L2RNM = Data.CastDbNull.To<decimal>(reader["S7Z58L2RNM"], 0),
                            S7Z58L2RDN = Data.CastDbNull.To<decimal>(reader["S7Z58L2RDN"], 0),
                            S7Z58L2TDM = Data.CastDbNull.To<decimal>(reader["S7Z58L2TDM"], 0),
                            S7Z58L2TNM = Data.CastDbNull.To<decimal>(reader["S7Z58L2TNM"], 0),
                            S7Z58L2TDN = Data.CastDbNull.To<decimal>(reader["S7Z58L2TDN"], 0),
                            S7Z58L21DM = Data.CastDbNull.To<decimal>(reader["S7Z58L21DM"], 0),
                            S7Z58L21NM = Data.CastDbNull.To<decimal>(reader["S7Z58L21NM"], 0),
                            S7Z58L21DN = Data.CastDbNull.To<decimal>(reader["S7Z58L21DN"], 0),
                            S7Z58L22DM = Data.CastDbNull.To<decimal>(reader["S7Z58L22DM"], 0),
                            S7Z58L22NM = Data.CastDbNull.To<decimal>(reader["S7Z58L22NM"], 0),
                            S7Z58L22DN = Data.CastDbNull.To<decimal>(reader["S7Z58L22DN"], 0),
                            S7Z582BDEC = Data.CastDbNull.To<decimal>(reader["S7Z582BDEC"], 0),
                            S7Z582LDEC = Data.CastDbNull.To<decimal>(reader["S7Z582LDEC"], 0),
                            S7Z582RDEC = Data.CastDbNull.To<decimal>(reader["S7Z582RDEC"], 0),
                            S7Z582TDEC = Data.CastDbNull.To<decimal>(reader["S7Z582TDEC"], 0),
                            S7Z5821DEC = Data.CastDbNull.To<decimal>(reader["S7Z5821DEC"], 0),
                            S7Z5822DEC = Data.CastDbNull.To<decimal>(reader["S7Z5822DEC"], 0),
                            S7Z58L2VG = Data.CastDbNull.To(reader["S7Z58L2VG"], string.Empty).Trim(),
                            S7Z58L2VGS = Data.CastDbNull.To<decimal>(reader["S7Z58L2VGS"], 0),
                            S7Z58L2VGT = Data.CastDbNull.To(reader["S7Z58L2VGT"], string.Empty).Trim(),
                            S7Z58L2VGW = Data.CastDbNull.To<decimal>(reader["S7Z58L2VGW"], 0),
                            S7Z58L2VGH = Data.CastDbNull.To<decimal>(reader["S7Z58L2VGH"], 0),
                            S7Z58L2VGD = Data.CastDbNull.To(reader["S7Z58L2VGD"], string.Empty).Trim(),
                            S7Z58L2VGO = Data.CastDbNull.To<decimal>(reader["S7Z58L2VGO"], 0),
                            S7Z58L2BL = Data.CastDbNull.To(reader["S7Z58L2BL"], string.Empty).Trim(),
                            S7Z58L2HL = Data.CastDbNull.To(reader["S7Z58L2HL"], string.Empty).Trim(),
                            S7Z58L2SF = Data.CastDbNull.To<decimal>(reader["S7Z58L2SF"], 0),
                            S7Z58L2SD = Data.CastDbNull.To(reader["S7Z58L2SD"], string.Empty).Trim(),
                            S7Z58L2EW = Data.CastDbNull.To(reader["S7Z58L2EW"], string.Empty).Trim(),
                            S7Z58L2TL = Data.CastDbNull.To(reader["S7Z58L2TL"], string.Empty).Trim(),
                            S7Z58L2PTS = Data.CastDbNull.To<decimal>(reader["S7Z58L2PTS"], 0),
                            S7Z58L2LCS = Data.CastDbNull.To<decimal>(reader["S7Z58L2LCS"], 0),
                            S7Z58L3TK = Data.CastDbNull.To<decimal>(reader["S7Z58L3TK"], 0),
                            S7Z58L35C = Data.CastDbNull.To(reader["S7Z58L35C"], string.Empty).Trim(),
                            S7Z58L3CL = Data.CastDbNull.To(reader["S7Z58L3CL"], string.Empty).Trim(),
                            S7Z58L3HT = Data.CastDbNull.To(reader["S7Z58L3HT"], string.Empty).Trim(),
                            S7Z58L36C = Data.CastDbNull.To(reader["S7Z58L36C"], string.Empty).Trim(),
                            S7Z58L3PX = Data.CastDbNull.To(reader["S7Z58L3PX"], string.Empty).Trim(),
                            S7Z58L3LPT = Data.CastDbNull.To(reader["S7Z58L3LPT"], string.Empty).Trim(),
                            S7Z58L3BDM = Data.CastDbNull.To<decimal>(reader["S7Z58L3BDM"], 0),
                            S7Z58L3BNM = Data.CastDbNull.To<decimal>(reader["S7Z58L3BNM"], 0),
                            S7Z58L3BDN = Data.CastDbNull.To<decimal>(reader["S7Z58L3BDN"], 0),
                            S7Z58L3LDM = Data.CastDbNull.To<decimal>(reader["S7Z58L3LDM"], 0),
                            S7Z58L3LNM = Data.CastDbNull.To<decimal>(reader["S7Z58L3LNM"], 0),
                            S7Z58L3LDN = Data.CastDbNull.To<decimal>(reader["S7Z58L3LDN"], 0),
                            S7Z58L3RDM = Data.CastDbNull.To<decimal>(reader["S7Z58L3RDM"], 0),
                            S7Z58L3RNM = Data.CastDbNull.To<decimal>(reader["S7Z58L3RNM"], 0),
                            S7Z58L3RDN = Data.CastDbNull.To<decimal>(reader["S7Z58L3RDN"], 0),
                            S7Z58L3TDM = Data.CastDbNull.To<decimal>(reader["S7Z58L3TDM"], 0),
                            S7Z58L3TNM = Data.CastDbNull.To<decimal>(reader["S7Z58L3TNM"], 0),
                            S7Z58L3TDN = Data.CastDbNull.To<decimal>(reader["S7Z58L3TDN"], 0),
                            S7Z58L31DM = Data.CastDbNull.To<decimal>(reader["S7Z58L31DM"], 0),
                            S7Z58L31NM = Data.CastDbNull.To<decimal>(reader["S7Z58L31NM"], 0),
                            S7Z58L31DN = Data.CastDbNull.To<decimal>(reader["S7Z58L31DN"], 0),
                            S7Z58L32DM = Data.CastDbNull.To<decimal>(reader["S7Z58L32DM"], 0),
                            S7Z58L32NM = Data.CastDbNull.To<decimal>(reader["S7Z58L32NM"], 0),
                            S7Z58L32DN = Data.CastDbNull.To<decimal>(reader["S7Z58L32DN"], 0),
                            S7Z583BDEC = Data.CastDbNull.To<decimal>(reader["S7Z583BDEC"], 0),
                            S7Z583LDEC = Data.CastDbNull.To<decimal>(reader["S7Z583LDEC"], 0),
                            S7Z583RDEC = Data.CastDbNull.To<decimal>(reader["S7Z583RDEC"], 0),
                            S7Z583TDEC = Data.CastDbNull.To<decimal>(reader["S7Z583TDEC"], 0),
                            S7Z5831DEC = Data.CastDbNull.To<decimal>(reader["S7Z5831DEC"], 0),
                            S7Z5832DEC = Data.CastDbNull.To<decimal>(reader["S7Z5832DEC"], 0),
                            S7Z58L3VG = Data.CastDbNull.To(reader["S7Z58L3VG"], string.Empty).Trim(),
                            S7Z58L3VGS = Data.CastDbNull.To<decimal>(reader["S7Z58L3VGS"], 0),
                            S7Z58L3VGT = Data.CastDbNull.To(reader["S7Z58L3VGT"], string.Empty).Trim(),
                            S7Z58L3VGW = Data.CastDbNull.To<decimal>(reader["S7Z58L3VGW"], 0),
                            S7Z58L3VGH = Data.CastDbNull.To<decimal>(reader["S7Z58L3VGH"], 0),
                            S7Z58L3VGD = Data.CastDbNull.To(reader["S7Z58L3VGD"], string.Empty).Trim(),
                            S7Z58L3VGO = Data.CastDbNull.To<decimal>(reader["S7Z58L3VGO"], 0),
                            S7Z58L3BL = Data.CastDbNull.To(reader["S7Z58L3BL"], string.Empty).Trim(),
                            S7Z58L3HL = Data.CastDbNull.To(reader["S7Z58L3HL"], string.Empty).Trim(),
                            S7Z58L3SF = Data.CastDbNull.To<decimal>(reader["S7Z58L3SF"], 0),
                            S7Z58L3SD = Data.CastDbNull.To(reader["S7Z58L3SD"], string.Empty).Trim(),
                            S7Z58SFS = Data.CastDbNull.To(reader["S7Z58SFS"], string.Empty).Trim(),
                            S7USER = Data.CastDbNull.To(reader["S7USER"], string.Empty).Trim(),
                            S7PID = Data.CastDbNull.To(reader["S7PID"], string.Empty).Trim(),
                            S7JOBN = Data.CastDbNull.To(reader["S7JOBN"], string.Empty).Trim(),
                            S7UPMJ = Data.CastDbNull.To<decimal>(reader["S7UPMJ"], 0),
                            S7TDAY = Data.CastDbNull.To<decimal>(reader["S7TDAY"], 0),
                            CMCFGID = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0),
                            CMDateTime = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["CMUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["CMTDAY"], 0)),
                            WASRST = Data.CastDbNull.To(reader["WASRST"], string.Empty).Trim(),
                            IBANPL = Data.CastDbNull.To<decimal>(reader["IBANPL"], 0),
                            ABALPH = Data.CastDbNull.To(reader["ABALPH"], ""),
                            IBGLPT = Data.CastDbNull.To(reader["IBGLPT"], ""),
                            IBUPMJ = Data.CastDbNull.To<decimal>(reader["IBUPMJ"], 0),
                            IBTDAY = Data.CastDbNull.To<decimal>(reader["IBTDAY"], 0)
                        };
                        //f580021.CMCFGID = f3201Mgr.GetCMCFGID(e1Session, f580021.S7DOCO);
                        f580021s.Add(f580021);
                    }

                    reader.Close();
                    Logger.Trace($@"ROWS: {f580021s.Count}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return f580021s;
        }
    }
}
