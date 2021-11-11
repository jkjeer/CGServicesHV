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
                            S7EDBT = CastDbNull.To(reader["S7EDBT"], string.Empty).Trim(),
                            S7EDLN = CastDbNull.To<decimal>(reader["S7EDLN"], 0),
                            S7MCU = CastDbNull.To(reader["S7MCU"], string.Empty).Trim(),
                            S7Z58SCN = CastDbNull.To<decimal>(reader["S7Z58SCN"], 0),
                            S7Z58SCSN = CastDbNull.To<decimal>(reader["S7Z58SCSN"], 0),
                            S7Z58SCSTS = CastDbNull.To(reader["S7Z58SCSTS"], string.Empty).Trim(),
                            S7EDSP = CastDbNull.To(reader["S7EDSP"], string.Empty).Trim(),
                            S7RORN = CastDbNull.To(reader["S7RORN"], string.Empty).Trim(),
                            S7RCTO = CastDbNull.To(reader["S7RCTO"], string.Empty).Trim(),
                            S7LNID = CastDbNull.To<decimal>(reader["S7LNID"], 0),
                            S7DOCO = CastDbNull.To<decimal>(reader["S7DOCO"], 0),
                            S7DCTO = CastDbNull.To(reader["S7DCTO"], string.Empty).Trim(),
                            S7SHAN = CastDbNull.To<decimal>(reader["S7SHAN"], 0),
                            S7ALPH = CastDbNull.To(reader["S7ALPH"], string.Empty).Trim(),
                            S7VR01 = CastDbNull.To(reader["S7VR01"], string.Empty).Trim(),
                            S7VR02 = CastDbNull.To(reader["S7VR02"], string.Empty).Trim(),
                            S7VR03 = CastDbNull.To(reader["S7VR03"], string.Empty).Trim(),
                            S7Z58SCD = CastDbNull.To<decimal>(reader["S7Z58SCD"], 0),
                            S7Z58SCTYP = CastDbNull.To(reader["S7Z58SCTYP"], string.Empty).Trim(),
                            S7Z58SCDDJ = CastDbNull.To<decimal>(reader["S7Z58SCDDJ"], 0),
                            S7Z58SAREA = CastDbNull.To(reader["S7Z58SAREA"], string.Empty).Trim(),
                            S7ITM = CastDbNull.To<decimal>(reader["S7ITM"], 0),
                            S7LITM = CastDbNull.To(reader["S7LITM"], string.Empty).Trim(),
                            S7DSC1 = CastDbNull.To(reader["S7DSC1"], string.Empty).Trim(),
                            S7DSC2 = CastDbNull.To(reader["S7DSC2"], string.Empty).Trim(),
                            S7CITM = CastDbNull.To(reader["S7CITM"], string.Empty).Trim(),
                            S7Z58CP2 = CastDbNull.To(reader["S7Z58CP2"], string.Empty).Trim(),
                            S7Z58CP3 = CastDbNull.To(reader["S7Z58CP3"], string.Empty).Trim(),
                            S7Z58CP4 = CastDbNull.To(reader["S7Z58CP4"], string.Empty).Trim(),
                            S7Z58QTY = CastDbNull.To<decimal>(reader["S7Z58QTY"], 0),
                            S7UOM = CastDbNull.To(reader["S7UOM"], string.Empty).Trim(),
                            S7RSDJ = CastDbNull.To<decimal>(reader["S7RSDJ"], 0),
                            S7RSDT = CastDbNull.To<decimal>(reader["S7RSDT"], 0),
                            S7PRIO = CastDbNull.To(reader["S7PRIO"], string.Empty).Trim(),
                            S7Z58GAL1 = CastDbNull.To(reader["S7Z58GAL1"], string.Empty).Trim(),
                            S7Z58GAL2 = CastDbNull.To(reader["S7Z58GAL2"], string.Empty).Trim(),
                            S7Z58ML = CastDbNull.To<decimal>(reader["S7Z58ML"], 0),
                            S7Z58CSQNO = CastDbNull.To(reader["S7Z58CSQNO"], string.Empty).Trim(),
                            S7Z58GSQNO = CastDbNull.To(reader["S7Z58GSQNO"], string.Empty).Trim(),
                            S7Z58PT = CastDbNull.To(reader["S7Z58PT"], string.Empty).Trim(),
                            S7Z58DRAW = CastDbNull.To(reader["S7Z58DRAW"], string.Empty).Trim(),
                            S7Z58ZPO = CastDbNull.To(reader["S7Z58ZPO"], string.Empty).Trim(),
                            S7AN8DR = CastDbNull.To<decimal>(reader["S7AN8DR"], 0),
                            S7Z58CRATE = CastDbNull.To(reader["S7Z58CRATE"], string.Empty).Trim(),
                            S7Z58CTL = CastDbNull.To<decimal>(reader["S7Z58CTL"], 0),
                            S7Z58FLIP = CastDbNull.To(reader["S7Z58FLIP"], string.Empty).Trim(),
                            S7Z58TBAD = CastDbNull.To(reader["S7Z58TBAD"], string.Empty).Trim(),
                            S7Z58PVAD = CastDbNull.To(reader["S7Z58PVAD"], string.Empty).Trim(),
                            S7Z58SCAD = CastDbNull.To(reader["S7Z58SCAD"], string.Empty).Trim(),
                            S7Z58IGLN = CastDbNull.To(reader["S7Z58IGLN"], string.Empty).Trim(),
                            S7Z58BOXP = CastDbNull.To(reader["S7Z58BOXP"], string.Empty).Trim(),
                            S7Z58BGEE = CastDbNull.To(reader["S7Z58BGEE"], string.Empty).Trim(),
                            S7Z58BGSZ = CastDbNull.To<decimal>(reader["S7Z58BGSZ"], 0),
                            S7Z58IGCLN = CastDbNull.To<decimal>(reader["S7Z58IGCLN"], 0),
                            S7Z58CSCNO = CastDbNull.To(reader["S7Z58CSCNO"], string.Empty).Trim(),
                            S7LINN = CastDbNull.To<decimal>(reader["S7LINN"], 0),
                            S7Z58L1RG = CastDbNull.To(reader["S7Z58L1RG"], string.Empty).Trim(),
                            S7Z58L2RG = CastDbNull.To(reader["S7Z58L2RG"], string.Empty).Trim(),
                            S7Z58L3RG = CastDbNull.To(reader["S7Z58L3RG"], string.Empty).Trim(),
                            S7Z58MSG1 = CastDbNull.To(reader["S7Z58MSG1"], string.Empty).Trim(),
                            S7Z58MSG2 = CastDbNull.To(reader["S7Z58MSG2"], string.Empty).Trim(),
                            S7Z58L1QOH = CastDbNull.To<decimal>(reader["S7Z58L1QOH"], 0),
                            S7Z58L2QOH = CastDbNull.To<decimal>(reader["S7Z58L2QOH"], 0),
                            S7Z58L3QOH = CastDbNull.To<decimal>(reader["S7Z58L3QOH"], 0),
                            S7Z58L4QOH = CastDbNull.To<decimal>(reader["S7Z58L4QOH"], 0),
                            S7Z58FLRN = CastDbNull.To(reader["S7Z58FLRN"], string.Empty).Trim(),
                            S7Z58FLTN = CastDbNull.To(reader["S7Z58FLTN"], string.Empty).Trim(),
                            S7Z58SLRN = CastDbNull.To(reader["S7Z58SLRN"], string.Empty).Trim(),
                            S7Z58SLTN = CastDbNull.To(reader["S7Z58SLTN"], string.Empty).Trim(),
                            S7Z58TLRN = CastDbNull.To(reader["S7Z58TLRN"], string.Empty).Trim(),
                            S7Z58TLTN = CastDbNull.To(reader["S7Z58TLTN"], string.Empty).Trim(),
                            S7Z58CHGN = CastDbNull.To(reader["S7Z58CHGN"], string.Empty).Trim(),
                            S7Z58RT = CastDbNull.To(reader["S7Z58RT"], string.Empty).Trim(),
                            S7Z58ROVR = CastDbNull.To(reader["S7Z58ROVR"], string.Empty).Trim(),
                            S7Z58RKS = CastDbNull.To<decimal>(reader["S7Z58RKS"], 0),
                            S7Z58CRMQ = CastDbNull.To<decimal>(reader["S7Z58CRMQ"], 0),
                            S7Z58CRXQ = CastDbNull.To<decimal>(reader["S7Z58CRXQ"], 0),
                            S7Z58PPR = CastDbNull.To<decimal>(reader["S7Z58PPR"], 0),
                            S7Z58LPP = CastDbNull.To<decimal>(reader["S7Z58LPP"], 0),
                            S7Z58PMQ = CastDbNull.To<decimal>(reader["S7Z58PMQ"], 0),
                            S7Z58PXQ = CastDbNull.To<decimal>(reader["S7Z58PXQ"], 0),
                            S7Z58MTRW = CastDbNull.To<decimal>(reader["S7Z58MTRW"], 0),
                            S7UWUM = CastDbNull.To(reader["S7UWUM"], string.Empty).Trim(),
                            S7GPUS = CastDbNull.To(reader["S7GPUS"], string.Empty).Trim(),
                            S7Z58LPR = CastDbNull.To<decimal>(reader["S7Z58LPR"], 0),
                            S7Z58MC = CastDbNull.To(reader["S7Z58MC"], string.Empty).Trim(),
                            S7Z58CC = CastDbNull.To(reader["S7Z58CC"], string.Empty).Trim(),
                            S7Z58TKY = CastDbNull.To(reader["S7Z58TKY"], string.Empty).Trim(),
                            S7Z58CMT1 = CastDbNull.To(reader["S7Z58CMT1"], string.Empty).Trim(),
                            S7Z58CMT2 = CastDbNull.To(reader["S7Z58CMT2"], string.Empty).Trim(),
                            S7Z58CMT3 = CastDbNull.To(reader["S7Z58CMT3"], string.Empty).Trim(),
                            S7Z58L1TK = CastDbNull.To<decimal>(reader["S7Z58L1TK"], 0),
                            S7Z58L11C = CastDbNull.To(reader["S7Z58L11C"], string.Empty).Trim(),
                            S7Z58L1CL = CastDbNull.To(reader["S7Z58L1CL"], string.Empty).Trim(),
                            S7Z58L1HT = CastDbNull.To(reader["S7Z58L1HT"], string.Empty).Trim(),
                            S7Z58L12C = CastDbNull.To(reader["S7Z58L12C"], string.Empty).Trim(),
                            S7Z58L1PX = CastDbNull.To(reader["S7Z58L1PX"], string.Empty).Trim(),
                            S7Z58L1LPT = CastDbNull.To(reader["S7Z58L1LPT"], string.Empty).Trim(),
                            S7Z58SHAPE = CastDbNull.To(reader["S7Z58SHAPE"], string.Empty).Trim(),
                            S7Z58L1BDM = CastDbNull.To<decimal>(reader["S7Z58L1BDM"], 0),
                            S7Z58L1BNM = CastDbNull.To<decimal>(reader["S7Z58L1BNM"], 0),
                            S7Z58L1BDN = CastDbNull.To<decimal>(reader["S7Z58L1BDN"], 0),
                            S7Z58L1LDM = CastDbNull.To<decimal>(reader["S7Z58L1LDM"], 0),
                            S7Z58L1LNM = CastDbNull.To<decimal>(reader["S7Z58L1LNM"], 0),
                            S7Z58L1LDN = CastDbNull.To<decimal>(reader["S7Z58L1LDN"], 0),
                            S7Z58L1RDM = CastDbNull.To<decimal>(reader["S7Z58L1RDM"], 0),
                            S7Z58L1RNM = CastDbNull.To<decimal>(reader["S7Z58L1RNM"], 0),
                            S7Z58L1RDN = CastDbNull.To<decimal>(reader["S7Z58L1RDN"], 0),
                            S7Z58L1TDM = CastDbNull.To<decimal>(reader["S7Z58L1TDM"], 0),
                            S7Z58L1TNM = CastDbNull.To<decimal>(reader["S7Z58L1TNM"], 0),
                            S7Z58L1TDN = CastDbNull.To<decimal>(reader["S7Z58L1TDN"], 0),
                            S7Z58L11DM = CastDbNull.To<decimal>(reader["S7Z58L11DM"], 0),
                            S7Z58L11NM = CastDbNull.To<decimal>(reader["S7Z58L11NM"], 0),
                            S7Z58L11DN = CastDbNull.To<decimal>(reader["S7Z58L11DN"], 0),
                            S7Z58L12DM = CastDbNull.To<decimal>(reader["S7Z58L12DM"], 0),
                            S7Z58L12NM = CastDbNull.To<decimal>(reader["S7Z58L12NM"], 0),
                            S7Z58L12DN = CastDbNull.To<decimal>(reader["S7Z58L12DN"], 0),
                            S7Z581BDEC = CastDbNull.To<decimal>(reader["S7Z581BDEC"], 0),
                            S7Z581LDEC = CastDbNull.To<decimal>(reader["S7Z581LDEC"], 0),
                            S7Z581RDEC = CastDbNull.To<decimal>(reader["S7Z581RDEC"], 0),
                            S7Z581TDEC = CastDbNull.To<decimal>(reader["S7Z581TDEC"], 0),
                            S7Z5811DEC = CastDbNull.To<decimal>(reader["S7Z5811DEC"], 0),
                            S7Z5812DEC = CastDbNull.To<decimal>(reader["S7Z5812DEC"], 0),
                            S7Z581BMIM = CastDbNull.To<decimal>(reader["S7Z581BMIM"], 0),
                            S7Z581LMIM = CastDbNull.To<decimal>(reader["S7Z581LMIM"], 0),
                            S7Z581RMIM = CastDbNull.To<decimal>(reader["S7Z581RMIM"], 0),
                            S7Z581TMIM = CastDbNull.To<decimal>(reader["S7Z581TMIM"], 0),
                            S7Z5811MIM = CastDbNull.To<decimal>(reader["S7Z5811MIM"], 0),
                            S7Z5812MIM = CastDbNull.To<decimal>(reader["S7Z5812MIM"], 0),
                            S7Z58L1VG = CastDbNull.To(reader["S7Z58L1VG"], string.Empty).Trim(),
                            S7Z58L1VGS = CastDbNull.To<decimal>(reader["S7Z58L1VGS"], 0),
                            S7Z58L1VGT = CastDbNull.To(reader["S7Z58L1VGT"], string.Empty).Trim(),
                            S7Z58L1VGW = CastDbNull.To<decimal>(reader["S7Z58L1VGW"], 0),
                            S7Z58L1VGH = CastDbNull.To<decimal>(reader["S7Z58L1VGH"], 0),
                            S7Z58L1VGD = CastDbNull.To(reader["S7Z58L1VGD"], string.Empty).Trim(),
                            S7Z58L1VGO = CastDbNull.To<decimal>(reader["S7Z58L1VGO"], 0),
                            S7Z58L1BL = CastDbNull.To(reader["S7Z58L1BL"], string.Empty).Trim(),
                            S7Z58L1HL = CastDbNull.To(reader["S7Z58L1HL"], string.Empty).Trim(),
                            S7Z581365 = CastDbNull.To<decimal>(reader["S7Z581365"], 0),
                            S7Z58L1SF = CastDbNull.To<decimal>(reader["S7Z58L1SF"], 0),
                            S7Z58L1SD = CastDbNull.To(reader["S7Z58L1SD"], string.Empty).Trim(),
                            S7Z58L1EW = CastDbNull.To(reader["S7Z58L1EW"], string.Empty).Trim(),
                            S7Z58L1PQL = CastDbNull.To(reader["S7Z58L1PQL"], string.Empty).Trim(),
                            S7Z58L1TL = CastDbNull.To(reader["S7Z58L1TL"], string.Empty).Trim(),
                            S7Z58L1ALT = CastDbNull.To(reader["S7Z58L1ALT"], string.Empty).Trim(),
                            S7Z58L1LX = CastDbNull.To<decimal>(reader["S7Z58L1LX"], 0),
                            S7Z58L1LY = CastDbNull.To<decimal>(reader["S7Z58L1LY"], 0),
                            S7Z58L1CLN = CastDbNull.To<decimal>(reader["S7Z58L1CLN"], 0),
                            S7Z58L1PTS = CastDbNull.To<decimal>(reader["S7Z58L1PTS"], 0),
                            S7Z58L1LCS = CastDbNull.To<decimal>(reader["S7Z58L1LCS"], 0),
                            S7Z58L2TK = CastDbNull.To<decimal>(reader["S7Z58L2TK"], 0),
                            S7Z58L23C = CastDbNull.To(reader["S7Z58L23C"], string.Empty).Trim(),
                            S7Z58L2CL = CastDbNull.To(reader["S7Z58L2CL"], string.Empty).Trim(),
                            S7Z58L2HT = CastDbNull.To(reader["S7Z58L2HT"], string.Empty).Trim(),
                            S7Z58L24C = CastDbNull.To(reader["S7Z58L24C"], string.Empty).Trim(),
                            S7Z58L2PX = CastDbNull.To(reader["S7Z58L2PX"], string.Empty).Trim(),
                            S7Z58L2LPT = CastDbNull.To(reader["S7Z58L2LPT"], string.Empty).Trim(),
                            S7Z58L2BDM = CastDbNull.To<decimal>(reader["S7Z58L2BDM"], 0),
                            S7Z58L2BNM = CastDbNull.To<decimal>(reader["S7Z58L2BNM"], 0),
                            S7Z58L2BDN = CastDbNull.To<decimal>(reader["S7Z58L2BDN"], 0),
                            S7Z58L2LDM = CastDbNull.To<decimal>(reader["S7Z58L2LDM"], 0),
                            S7Z58L2LNM = CastDbNull.To<decimal>(reader["S7Z58L2LNM"], 0),
                            S7Z58L2LDN = CastDbNull.To<decimal>(reader["S7Z58L2LDN"], 0),
                            S7Z58L2RDM = CastDbNull.To<decimal>(reader["S7Z58L2RDM"], 0),
                            S7Z58L2RNM = CastDbNull.To<decimal>(reader["S7Z58L2RNM"], 0),
                            S7Z58L2RDN = CastDbNull.To<decimal>(reader["S7Z58L2RDN"], 0),
                            S7Z58L2TDM = CastDbNull.To<decimal>(reader["S7Z58L2TDM"], 0),
                            S7Z58L2TNM = CastDbNull.To<decimal>(reader["S7Z58L2TNM"], 0),
                            S7Z58L2TDN = CastDbNull.To<decimal>(reader["S7Z58L2TDN"], 0),
                            S7Z58L21DM = CastDbNull.To<decimal>(reader["S7Z58L21DM"], 0),
                            S7Z58L21NM = CastDbNull.To<decimal>(reader["S7Z58L21NM"], 0),
                            S7Z58L21DN = CastDbNull.To<decimal>(reader["S7Z58L21DN"], 0),
                            S7Z58L22DM = CastDbNull.To<decimal>(reader["S7Z58L22DM"], 0),
                            S7Z58L22NM = CastDbNull.To<decimal>(reader["S7Z58L22NM"], 0),
                            S7Z58L22DN = CastDbNull.To<decimal>(reader["S7Z58L22DN"], 0),
                            S7Z582BDEC = CastDbNull.To<decimal>(reader["S7Z582BDEC"], 0),
                            S7Z582LDEC = CastDbNull.To<decimal>(reader["S7Z582LDEC"], 0),
                            S7Z582RDEC = CastDbNull.To<decimal>(reader["S7Z582RDEC"], 0),
                            S7Z582TDEC = CastDbNull.To<decimal>(reader["S7Z582TDEC"], 0),
                            S7Z5821DEC = CastDbNull.To<decimal>(reader["S7Z5821DEC"], 0),
                            S7Z5822DEC = CastDbNull.To<decimal>(reader["S7Z5822DEC"], 0),
                            S7Z58L2VG = CastDbNull.To(reader["S7Z58L2VG"], string.Empty).Trim(),
                            S7Z58L2VGS = CastDbNull.To<decimal>(reader["S7Z58L2VGS"], 0),
                            S7Z58L2VGT = CastDbNull.To(reader["S7Z58L2VGT"], string.Empty).Trim(),
                            S7Z58L2VGW = CastDbNull.To<decimal>(reader["S7Z58L2VGW"], 0),
                            S7Z58L2VGH = CastDbNull.To<decimal>(reader["S7Z58L2VGH"], 0),
                            S7Z58L2VGD = CastDbNull.To(reader["S7Z58L2VGD"], string.Empty).Trim(),
                            S7Z58L2VGO = CastDbNull.To<decimal>(reader["S7Z58L2VGO"], 0),
                            S7Z58L2BL = CastDbNull.To(reader["S7Z58L2BL"], string.Empty).Trim(),
                            S7Z58L2HL = CastDbNull.To(reader["S7Z58L2HL"], string.Empty).Trim(),
                            S7Z58L2SF = CastDbNull.To<decimal>(reader["S7Z58L2SF"], 0),
                            S7Z58L2SD = CastDbNull.To(reader["S7Z58L2SD"], string.Empty).Trim(),
                            S7Z58L2EW = CastDbNull.To(reader["S7Z58L2EW"], string.Empty).Trim(),
                            S7Z58L2TL = CastDbNull.To(reader["S7Z58L2TL"], string.Empty).Trim(),
                            S7Z58L2PTS = CastDbNull.To<decimal>(reader["S7Z58L2PTS"], 0),
                            S7Z58L2LCS = CastDbNull.To<decimal>(reader["S7Z58L2LCS"], 0),
                            S7Z58L3TK = CastDbNull.To<decimal>(reader["S7Z58L3TK"], 0),
                            S7Z58L35C = CastDbNull.To(reader["S7Z58L35C"], string.Empty).Trim(),
                            S7Z58L3CL = CastDbNull.To(reader["S7Z58L3CL"], string.Empty).Trim(),
                            S7Z58L3HT = CastDbNull.To(reader["S7Z58L3HT"], string.Empty).Trim(),
                            S7Z58L36C = CastDbNull.To(reader["S7Z58L36C"], string.Empty).Trim(),
                            S7Z58L3PX = CastDbNull.To(reader["S7Z58L3PX"], string.Empty).Trim(),
                            S7Z58L3LPT = CastDbNull.To(reader["S7Z58L3LPT"], string.Empty).Trim(),
                            S7Z58L3BDM = CastDbNull.To<decimal>(reader["S7Z58L3BDM"], 0),
                            S7Z58L3BNM = CastDbNull.To<decimal>(reader["S7Z58L3BNM"], 0),
                            S7Z58L3BDN = CastDbNull.To<decimal>(reader["S7Z58L3BDN"], 0),
                            S7Z58L3LDM = CastDbNull.To<decimal>(reader["S7Z58L3LDM"], 0),
                            S7Z58L3LNM = CastDbNull.To<decimal>(reader["S7Z58L3LNM"], 0),
                            S7Z58L3LDN = CastDbNull.To<decimal>(reader["S7Z58L3LDN"], 0),
                            S7Z58L3RDM = CastDbNull.To<decimal>(reader["S7Z58L3RDM"], 0),
                            S7Z58L3RNM = CastDbNull.To<decimal>(reader["S7Z58L3RNM"], 0),
                            S7Z58L3RDN = CastDbNull.To<decimal>(reader["S7Z58L3RDN"], 0),
                            S7Z58L3TDM = CastDbNull.To<decimal>(reader["S7Z58L3TDM"], 0),
                            S7Z58L3TNM = CastDbNull.To<decimal>(reader["S7Z58L3TNM"], 0),
                            S7Z58L3TDN = CastDbNull.To<decimal>(reader["S7Z58L3TDN"], 0),
                            S7Z58L31DM = CastDbNull.To<decimal>(reader["S7Z58L31DM"], 0),
                            S7Z58L31NM = CastDbNull.To<decimal>(reader["S7Z58L31NM"], 0),
                            S7Z58L31DN = CastDbNull.To<decimal>(reader["S7Z58L31DN"], 0),
                            S7Z58L32DM = CastDbNull.To<decimal>(reader["S7Z58L32DM"], 0),
                            S7Z58L32NM = CastDbNull.To<decimal>(reader["S7Z58L32NM"], 0),
                            S7Z58L32DN = CastDbNull.To<decimal>(reader["S7Z58L32DN"], 0),
                            S7Z583BDEC = CastDbNull.To<decimal>(reader["S7Z583BDEC"], 0),
                            S7Z583LDEC = CastDbNull.To<decimal>(reader["S7Z583LDEC"], 0),
                            S7Z583RDEC = CastDbNull.To<decimal>(reader["S7Z583RDEC"], 0),
                            S7Z583TDEC = CastDbNull.To<decimal>(reader["S7Z583TDEC"], 0),
                            S7Z5831DEC = CastDbNull.To<decimal>(reader["S7Z5831DEC"], 0),
                            S7Z5832DEC = CastDbNull.To<decimal>(reader["S7Z5832DEC"], 0),
                            S7Z58L3VG = CastDbNull.To(reader["S7Z58L3VG"], string.Empty).Trim(),
                            S7Z58L3VGS = CastDbNull.To<decimal>(reader["S7Z58L3VGS"], 0),
                            S7Z58L3VGT = CastDbNull.To(reader["S7Z58L3VGT"], string.Empty).Trim(),
                            S7Z58L3VGW = CastDbNull.To<decimal>(reader["S7Z58L3VGW"], 0),
                            S7Z58L3VGH = CastDbNull.To<decimal>(reader["S7Z58L3VGH"], 0),
                            S7Z58L3VGD = CastDbNull.To(reader["S7Z58L3VGD"], string.Empty).Trim(),
                            S7Z58L3VGO = CastDbNull.To<decimal>(reader["S7Z58L3VGO"], 0),
                            S7Z58L3BL = CastDbNull.To(reader["S7Z58L3BL"], string.Empty).Trim(),
                            S7Z58L3HL = CastDbNull.To(reader["S7Z58L3HL"], string.Empty).Trim(),
                            S7Z58L3SF = CastDbNull.To<decimal>(reader["S7Z58L3SF"], 0),
                            S7Z58L3SD = CastDbNull.To(reader["S7Z58L3SD"], string.Empty).Trim(),
                            S7Z58SFS = CastDbNull.To(reader["S7Z58SFS"], string.Empty).Trim(),
                            S7USER = CastDbNull.To(reader["S7USER"], string.Empty).Trim(),
                            S7PID = CastDbNull.To(reader["S7PID"], string.Empty).Trim(),
                            S7JOBN = CastDbNull.To(reader["S7JOBN"], string.Empty).Trim(),
                            S7UPMJ = CastDbNull.To<decimal>(reader["S7UPMJ"], 0),
                            S7TDAY = CastDbNull.To<decimal>(reader["S7TDAY"], 0),
                            CMCFGID = CastDbNull.To<decimal>(reader["CMCFGID"], 0),
                            CMDateTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["CMUPMJ"], 0), CastDbNull.To<decimal>(reader["CMTDAY"], 0)),
                            WASRST = CastDbNull.To(reader["WASRST"], string.Empty).Trim(),
                            IBANPL = CastDbNull.To<decimal>(reader["IBANPL"], 0),
                            ABALPH = CastDbNull.To(reader["ABALPH"], ""),
                            IBGLPT = CastDbNull.To(reader["IBGLPT"], ""),
                            IBUPMJ = CastDbNull.To<decimal>(reader["IBUPMJ"], 0),
                            IBTDAY = CastDbNull.To<decimal>(reader["IBTDAY"], 0)
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
