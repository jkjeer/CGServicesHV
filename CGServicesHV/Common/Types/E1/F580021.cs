using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

// ReSharper disable once InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F580021s : List<F580021>
    {
    }

    [DataContract(Namespace = "")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class F580021
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string S7EDBT { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal S7EDLN { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string S7MCU { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal S7Z58SCN { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal S7Z58SCSN { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string S7Z58SCSTS { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string S7EDSP { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string S7RORN { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string S7RCTO { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal S7LNID { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal S7DOCO { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string S7DCTO { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal S7SHAN { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string S7ALPH { get; set; }
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string S7VR01 { get; set; }
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string S7VR02 { get; set; }
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public string S7VR03 { get; set; }
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public decimal S7Z58SCD { get; set; }
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public string S7Z58SCTYP { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal S7Z58SCDDJ { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public string S7Z58SAREA { get; set; }
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public decimal S7ITM { get; set; }
        [DataMember(Order = 230, EmitDefaultValue = false)]
        public string S7LITM { get; set; }
        [DataMember(Order = 240, EmitDefaultValue = false)]
        public string S7DSC1 { get; set; }
        [DataMember(Order = 250, EmitDefaultValue = false)]
        public string S7DSC2 { get; set; }
        [DataMember(Order = 260, EmitDefaultValue = false)]
        public string S7CITM { get; set; }
        [DataMember(Order = 270, EmitDefaultValue = false)]
        public string S7Z58CP2 { get; set; }
        [DataMember(Order = 280, EmitDefaultValue = false)]
        public string S7Z58CP3 { get; set; }
        [DataMember(Order = 290, EmitDefaultValue = false)]
        public string S7Z58CP4 { get; set; }
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public decimal S7Z58QTY { get; set; }
        [DataMember(Order = 310, EmitDefaultValue = false)]
        public string S7UOM { get; set; }
        [DataMember(Order = 320, EmitDefaultValue = false)]
        public decimal S7RSDJ { get; set; }
        [DataMember(Order = 330, EmitDefaultValue = false)]
        public decimal S7RSDT { get; set; }
        [DataMember(Order = 340, EmitDefaultValue = false)]
        public string S7PRIO { get; set; }
        [DataMember(Order = 350, EmitDefaultValue = false)]
        public string S7Z58GAL1 { get; set; }
        [DataMember(Order = 360, EmitDefaultValue = false)]
        public string S7Z58GAL2 { get; set; }
        [DataMember(Order = 370, EmitDefaultValue = false)]
        public decimal S7Z58ML { get; set; }
        [DataMember(Order = 380, EmitDefaultValue = false)]
        public string S7Z58CSQNO { get; set; }
        [DataMember(Order = 390, EmitDefaultValue = false)]
        public string S7Z58GSQNO { get; set; }
        [DataMember(Order = 400, EmitDefaultValue = false)]
        public string S7Z58PT { get; set; }
        [DataMember(Order = 410, EmitDefaultValue = false)]
        public string S7Z58DRAW { get; set; }
        [DataMember(Order = 420, EmitDefaultValue = false)]
        public string S7Z58ZPO { get; set; }
        [DataMember(Order = 430, EmitDefaultValue = false)]
        public decimal S7AN8DR { get; set; }
        [DataMember(Order = 440, EmitDefaultValue = false)]
        public string S7Z58CRATE { get; set; }
        [DataMember(Order = 450, EmitDefaultValue = false)]
        public decimal S7Z58CTL { get; set; }
        [DataMember(Order = 460, EmitDefaultValue = false)]
        public string S7Z58FLIP { get; set; }
        [DataMember(Order = 470, EmitDefaultValue = false)]
        public string S7Z58TBAD { get; set; }
        [DataMember(Order = 480, EmitDefaultValue = false)]
        public string S7Z58PVAD { get; set; }
        [DataMember(Order = 490, EmitDefaultValue = false)]
        public string S7Z58SCAD { get; set; }
        [DataMember(Order = 500, EmitDefaultValue = false)]
        public string S7Z58IGLN { get; set; }
        [DataMember(Order = 510, EmitDefaultValue = false)]
        public string S7Z58BOXP { get; set; }
        [DataMember(Order = 520, EmitDefaultValue = false)]
        public string S7Z58BGEE { get; set; }
        [DataMember(Order = 530, EmitDefaultValue = false)]
        public decimal S7Z58BGSZ { get; set; }
        [DataMember(Order = 540, EmitDefaultValue = false)]
        public decimal S7Z58IGCLN { get; set; }
        [DataMember(Order = 550, EmitDefaultValue = false)]
        public string S7Z58CSCNO { get; set; }
        [DataMember(Order = 560, EmitDefaultValue = false)]
        public decimal S7LINN { get; set; }
        [DataMember(Order = 570, EmitDefaultValue = false)]
        public string S7Z58L1RG { get; set; }
        [DataMember(Order = 580, EmitDefaultValue = false)]
        public string S7Z58L2RG { get; set; }
        [DataMember(Order = 590, EmitDefaultValue = false)]
        public string S7Z58L3RG { get; set; }
        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string S7Z58MSG1 { get; set; }
        [DataMember(Order = 610, EmitDefaultValue = false)]
        public string S7Z58MSG2 { get; set; }
        [DataMember(Order = 620, EmitDefaultValue = false)]
        public decimal S7Z58L1QOH { get; set; }
        [DataMember(Order = 630, EmitDefaultValue = false)]
        public decimal S7Z58L2QOH { get; set; }
        [DataMember(Order = 640, EmitDefaultValue = false)]
        public decimal S7Z58L3QOH { get; set; }
        [DataMember(Order = 650, EmitDefaultValue = false)]
        public decimal S7Z58L4QOH { get; set; }
        [DataMember(Order = 660, EmitDefaultValue = false)]
        public string S7Z58FLRN { get; set; }
        [DataMember(Order = 670, EmitDefaultValue = false)]
        public string S7Z58FLTN { get; set; }
        [DataMember(Order = 680, EmitDefaultValue = false)]
        public string S7Z58SLRN { get; set; }
        [DataMember(Order = 690, EmitDefaultValue = false)]
        public string S7Z58SLTN { get; set; }
        [DataMember(Order = 700, EmitDefaultValue = false)]
        public string S7Z58TLRN { get; set; }
        [DataMember(Order = 710, EmitDefaultValue = false)]
        public string S7Z58TLTN { get; set; }
        [DataMember(Order = 720, EmitDefaultValue = false)]
        public string S7Z58CHGN { get; set; }
        [DataMember(Order = 730, EmitDefaultValue = false)]
        public string S7Z58RT { get; set; }
        [DataMember(Order = 740, EmitDefaultValue = false)]
        public string S7Z58ROVR { get; set; }
        [DataMember(Order = 750, EmitDefaultValue = false)]
        public decimal S7Z58RKS { get; set; }
        [DataMember(Order = 760, EmitDefaultValue = false)]
        public decimal S7Z58CRMQ { get; set; }
        [DataMember(Order = 770, EmitDefaultValue = false)]
        public decimal S7Z58CRXQ { get; set; }
        [DataMember(Order = 780, EmitDefaultValue = false)]
        public decimal S7Z58PPR { get; set; }
        [DataMember(Order = 790, EmitDefaultValue = false)]
        public decimal S7Z58LPP { get; set; }
        [DataMember(Order = 800, EmitDefaultValue = false)]
        public decimal S7Z58PMQ { get; set; }
        [DataMember(Order = 810, EmitDefaultValue = false)]
        public decimal S7Z58PXQ { get; set; }
        [DataMember(Order = 820, EmitDefaultValue = false)]
        public decimal S7Z58MTRW { get; set; }
        [DataMember(Order = 830, EmitDefaultValue = false)]
        public string S7UWUM { get; set; }
        [DataMember(Order = 840, EmitDefaultValue = false)]
        public string S7GPUS { get; set; }
        [DataMember(Order = 850, EmitDefaultValue = false)]
        public decimal S7Z58LPR { get; set; }
        [DataMember(Order = 860, EmitDefaultValue = false)]
        public string S7Z58MC { get; set; }
        [DataMember(Order = 870, EmitDefaultValue = false)]
        public string S7Z58CC { get; set; }
        [DataMember(Order = 880, EmitDefaultValue = false)]
        public string S7Z58TKY { get; set; }
        [DataMember(Order = 890, EmitDefaultValue = false)]
        public string S7Z58CMT1 { get; set; }
        [DataMember(Order = 900, EmitDefaultValue = false)]
        public string S7Z58CMT2 { get; set; }
        [DataMember(Order = 910, EmitDefaultValue = false)]
        public string S7Z58CMT3 { get; set; }
        [DataMember(Order = 920, EmitDefaultValue = false)]
        public decimal S7Z58L1TK { get; set; }
        [DataMember(Order = 930, EmitDefaultValue = false)]
        public string S7Z58L11C { get; set; }
        [DataMember(Order = 940, EmitDefaultValue = false)]
        public string S7Z58L1CL { get; set; }
        [DataMember(Order = 950, EmitDefaultValue = false)]
        public string S7Z58L1HT { get; set; }
        [DataMember(Order = 960, EmitDefaultValue = false)]
        public string S7Z58L12C { get; set; }
        [DataMember(Order = 970, EmitDefaultValue = false)]
        public string S7Z58L1PX { get; set; }
        [DataMember(Order = 980, EmitDefaultValue = false)]
        public string S7Z58L1LPT { get; set; }
        [DataMember(Order = 990, EmitDefaultValue = false)]
        public string S7Z58SHAPE { get; set; }
        [DataMember(Order = 1000, EmitDefaultValue = false)]
        public decimal S7Z58L1BDM { get; set; }
        [DataMember(Order = 1010, EmitDefaultValue = false)]
        public decimal S7Z58L1BNM { get; set; }
        [DataMember(Order = 1020, EmitDefaultValue = false)]
        public decimal S7Z58L1BDN { get; set; }
        [DataMember(Order = 1030, EmitDefaultValue = false)]
        public decimal S7Z58L1LDM { get; set; }
        [DataMember(Order = 1040, EmitDefaultValue = false)]
        public decimal S7Z58L1LNM { get; set; }
        [DataMember(Order = 1050, EmitDefaultValue = false)]
        public decimal S7Z58L1LDN { get; set; }
        [DataMember(Order = 1060, EmitDefaultValue = false)]
        public decimal S7Z58L1RDM { get; set; }
        [DataMember(Order = 1070, EmitDefaultValue = false)]
        public decimal S7Z58L1RNM { get; set; }
        [DataMember(Order = 1080, EmitDefaultValue = false)]
        public decimal S7Z58L1RDN { get; set; }
        [DataMember(Order = 1090, EmitDefaultValue = false)]
        public decimal S7Z58L1TDM { get; set; }
        [DataMember(Order = 1100, EmitDefaultValue = false)]
        public decimal S7Z58L1TNM { get; set; }
        [DataMember(Order = 1110, EmitDefaultValue = false)]
        public decimal S7Z58L1TDN { get; set; }
        [DataMember(Order = 1120, EmitDefaultValue = false)]
        public decimal S7Z58L11DM { get; set; }
        [DataMember(Order = 1130, EmitDefaultValue = false)]
        public decimal S7Z58L11NM { get; set; }
        [DataMember(Order = 1140, EmitDefaultValue = false)]
        public decimal S7Z58L11DN { get; set; }
        [DataMember(Order = 1150, EmitDefaultValue = false)]
        public decimal S7Z58L12DM { get; set; }
        [DataMember(Order = 1160, EmitDefaultValue = false)]
        public decimal S7Z58L12NM { get; set; }
        [DataMember(Order = 1170, EmitDefaultValue = false)]
        public decimal S7Z58L12DN { get; set; }
        [DataMember(Order = 1180, EmitDefaultValue = false)]
        public decimal S7Z581BDEC { get; set; }
        [DataMember(Order = 1190, EmitDefaultValue = false)]
        public decimal S7Z581LDEC { get; set; }
        [DataMember(Order = 1200, EmitDefaultValue = false)]
        public decimal S7Z581RDEC { get; set; }
        [DataMember(Order = 1210, EmitDefaultValue = false)]
        public decimal S7Z581TDEC { get; set; }
        [DataMember(Order = 1220, EmitDefaultValue = false)]
        public decimal S7Z5811DEC { get; set; }
        [DataMember(Order = 1230, EmitDefaultValue = false)]
        public decimal S7Z5812DEC { get; set; }
        [DataMember(Order = 1240, EmitDefaultValue = false)]
        public decimal S7Z581BMIM { get; set; }
        [DataMember(Order = 1250, EmitDefaultValue = false)]
        public decimal S7Z581LMIM { get; set; }
        [DataMember(Order = 1260, EmitDefaultValue = false)]
        public decimal S7Z581RMIM { get; set; }
        [DataMember(Order = 1270, EmitDefaultValue = false)]
        public decimal S7Z581TMIM { get; set; }
        [DataMember(Order = 1280, EmitDefaultValue = false)]
        public decimal S7Z5811MIM { get; set; }
        [DataMember(Order = 1290, EmitDefaultValue = false)]
        public decimal S7Z5812MIM { get; set; }
        [DataMember(Order = 1300, EmitDefaultValue = false)]
        public string S7Z58L1VG { get; set; }
        [DataMember(Order = 1310, EmitDefaultValue = false)]
        public decimal S7Z58L1VGS { get; set; }
        [DataMember(Order = 1320, EmitDefaultValue = false)]
        public string S7Z58L1VGT { get; set; }
        [DataMember(Order = 1330, EmitDefaultValue = false)]
        public decimal S7Z58L1VGW { get; set; }
        [DataMember(Order = 1340, EmitDefaultValue = false)]
        public decimal S7Z58L1VGH { get; set; }
        [DataMember(Order = 1350, EmitDefaultValue = false)]
        public string S7Z58L1VGD { get; set; }
        [DataMember(Order = 1360, EmitDefaultValue = false)]
        public decimal S7Z58L1VGO { get; set; }
        [DataMember(Order = 1370, EmitDefaultValue = false)]
        public string S7Z58L1BL { get; set; }
        [DataMember(Order = 1380, EmitDefaultValue = false)]
        public string S7Z58L1HL { get; set; }
        [DataMember(Order = 1390, EmitDefaultValue = false)]
        public decimal S7Z581365 { get; set; }
        [DataMember(Order = 1400, EmitDefaultValue = false)]
        public decimal S7Z58L1SF { get; set; }
        [DataMember(Order = 1410, EmitDefaultValue = false)]
        public string S7Z58L1SD { get; set; }
        [DataMember(Order = 1420, EmitDefaultValue = false)]
        public string S7Z58L1EW { get; set; }
        [DataMember(Order = 1430, EmitDefaultValue = false)]
        public string S7Z58L1PQL { get; set; }
        [DataMember(Order = 1440, EmitDefaultValue = false)]
        public string S7Z58L1TL { get; set; }
        [DataMember(Order = 1450, EmitDefaultValue = false)]
        public string S7Z58L1ALT { get; set; }
        [DataMember(Order = 1460, EmitDefaultValue = false)]
        public decimal S7Z58L1LX { get; set; }
        [DataMember(Order = 1470, EmitDefaultValue = false)]
        public decimal S7Z58L1LY { get; set; }
        [DataMember(Order = 1480, EmitDefaultValue = false)]
        public decimal S7Z58L1CLN { get; set; }
        [DataMember(Order = 1490, EmitDefaultValue = false)]
        public decimal S7Z58L1PTS { get; set; }
        [DataMember(Order = 1500, EmitDefaultValue = false)]
        public decimal S7Z58L1LCS { get; set; }
        [DataMember(Order = 1510, EmitDefaultValue = false)]
        public decimal S7Z58L2TK { get; set; }
        [DataMember(Order = 1520, EmitDefaultValue = false)]
        public string S7Z58L23C { get; set; }
        [DataMember(Order = 1530, EmitDefaultValue = false)]
        public string S7Z58L2CL { get; set; }
        [DataMember(Order = 1540, EmitDefaultValue = false)]
        public string S7Z58L2HT { get; set; }
        [DataMember(Order = 1550, EmitDefaultValue = false)]
        public string S7Z58L24C { get; set; }
        [DataMember(Order = 1560, EmitDefaultValue = false)]
        public string S7Z58L2PX { get; set; }
        [DataMember(Order = 1570, EmitDefaultValue = false)]
        public string S7Z58L2LPT { get; set; }
        [DataMember(Order = 1580, EmitDefaultValue = false)]
        public decimal S7Z58L2BDM { get; set; }
        [DataMember(Order = 1590, EmitDefaultValue = false)]
        public decimal S7Z58L2BNM { get; set; }
        [DataMember(Order = 1600, EmitDefaultValue = false)]
        public decimal S7Z58L2BDN { get; set; }
        [DataMember(Order = 1610, EmitDefaultValue = false)]
        public decimal S7Z58L2LDM { get; set; }
        [DataMember(Order = 1620, EmitDefaultValue = false)]
        public decimal S7Z58L2LNM { get; set; }
        [DataMember(Order = 1630, EmitDefaultValue = false)]
        public decimal S7Z58L2LDN { get; set; }
        [DataMember(Order = 1640, EmitDefaultValue = false)]
        public decimal S7Z58L2RDM { get; set; }
        [DataMember(Order = 1650, EmitDefaultValue = false)]
        public decimal S7Z58L2RNM { get; set; }
        [DataMember(Order = 1660, EmitDefaultValue = false)]
        public decimal S7Z58L2RDN { get; set; }
        [DataMember(Order = 1670, EmitDefaultValue = false)]
        public decimal S7Z58L2TDM { get; set; }
        [DataMember(Order = 1680, EmitDefaultValue = false)]
        public decimal S7Z58L2TNM { get; set; }
        [DataMember(Order = 1690, EmitDefaultValue = false)]
        public decimal S7Z58L2TDN { get; set; }
        [DataMember(Order = 1700, EmitDefaultValue = false)]
        public decimal S7Z58L21DM { get; set; }
        [DataMember(Order = 1710, EmitDefaultValue = false)]
        public decimal S7Z58L21NM { get; set; }
        [DataMember(Order = 1720, EmitDefaultValue = false)]
        public decimal S7Z58L21DN { get; set; }
        [DataMember(Order = 1730, EmitDefaultValue = false)]
        public decimal S7Z58L22DM { get; set; }
        [DataMember(Order = 1740, EmitDefaultValue = false)]
        public decimal S7Z58L22NM { get; set; }
        [DataMember(Order = 1750, EmitDefaultValue = false)]
        public decimal S7Z58L22DN { get; set; }
        [DataMember(Order = 1760, EmitDefaultValue = false)]
        public decimal S7Z582BDEC { get; set; }
        [DataMember(Order = 1770, EmitDefaultValue = false)]
        public decimal S7Z582LDEC { get; set; }
        [DataMember(Order = 1780, EmitDefaultValue = false)]
        public decimal S7Z582RDEC { get; set; }
        [DataMember(Order = 1790, EmitDefaultValue = false)]
        public decimal S7Z582TDEC { get; set; }
        [DataMember(Order = 1800, EmitDefaultValue = false)]
        public decimal S7Z5821DEC { get; set; }
        [DataMember(Order = 1810, EmitDefaultValue = false)]
        public decimal S7Z5822DEC { get; set; }
        [DataMember(Order = 1820, EmitDefaultValue = false)]
        public string S7Z58L2VG { get; set; }
        [DataMember(Order = 1830, EmitDefaultValue = false)]
        public decimal S7Z58L2VGS { get; set; }
        [DataMember(Order = 1840, EmitDefaultValue = false)]
        public string S7Z58L2VGT { get; set; }
        [DataMember(Order = 1850, EmitDefaultValue = false)]
        public decimal S7Z58L2VGW { get; set; }
        [DataMember(Order = 1860, EmitDefaultValue = false)]
        public decimal S7Z58L2VGH { get; set; }
        [DataMember(Order = 1870, EmitDefaultValue = false)]
        public string S7Z58L2VGD { get; set; }
        [DataMember(Order = 1880, EmitDefaultValue = false)]
        public decimal S7Z58L2VGO { get; set; }
        [DataMember(Order = 1890, EmitDefaultValue = false)]
        public string S7Z58L2BL { get; set; }
        [DataMember(Order = 1900, EmitDefaultValue = false)]
        public string S7Z58L2HL { get; set; }
        [DataMember(Order = 1910, EmitDefaultValue = false)]
        public decimal S7Z58L2SF { get; set; }
        [DataMember(Order = 1920, EmitDefaultValue = false)]
        public string S7Z58L2SD { get; set; }
        [DataMember(Order = 1930, EmitDefaultValue = false)]
        public string S7Z58L2EW { get; set; }
        [DataMember(Order = 1940, EmitDefaultValue = false)]
        public string S7Z58L2TL { get; set; }
        [DataMember(Order = 1950, EmitDefaultValue = false)]
        public decimal S7Z58L2PTS { get; set; }
        [DataMember(Order = 1960, EmitDefaultValue = false)]
        public decimal S7Z58L2LCS { get; set; }
        [DataMember(Order = 1970, EmitDefaultValue = false)]
        public decimal S7Z58L3TK { get; set; }
        [DataMember(Order = 1980, EmitDefaultValue = false)]
        public string S7Z58L35C { get; set; }
        [DataMember(Order = 1990, EmitDefaultValue = false)]
        public string S7Z58L3CL { get; set; }
        [DataMember(Order = 2000, EmitDefaultValue = false)]
        public string S7Z58L3HT { get; set; }
        [DataMember(Order = 2010, EmitDefaultValue = false)]
        public string S7Z58L36C { get; set; }
        [DataMember(Order = 2020, EmitDefaultValue = false)]
        public string S7Z58L3PX { get; set; }
        [DataMember(Order = 2030, EmitDefaultValue = false)]
        public string S7Z58L3LPT { get; set; }
        [DataMember(Order = 2040, EmitDefaultValue = false)]
        public decimal S7Z58L3BDM { get; set; }
        [DataMember(Order = 2050, EmitDefaultValue = false)]
        public decimal S7Z58L3BNM { get; set; }
        [DataMember(Order = 2060, EmitDefaultValue = false)]
        public decimal S7Z58L3BDN { get; set; }
        [DataMember(Order = 2070, EmitDefaultValue = false)]
        public decimal S7Z58L3LDM { get; set; }
        [DataMember(Order = 2080, EmitDefaultValue = false)]
        public decimal S7Z58L3LNM { get; set; }
        [DataMember(Order = 2090, EmitDefaultValue = false)]
        public decimal S7Z58L3LDN { get; set; }
        [DataMember(Order = 2100, EmitDefaultValue = false)]
        public decimal S7Z58L3RDM { get; set; }
        [DataMember(Order = 2110, EmitDefaultValue = false)]
        public decimal S7Z58L3RNM { get; set; }
        [DataMember(Order = 2120, EmitDefaultValue = false)]
        public decimal S7Z58L3RDN { get; set; }
        [DataMember(Order = 2130, EmitDefaultValue = false)]
        public decimal S7Z58L3TDM { get; set; }
        [DataMember(Order = 2140, EmitDefaultValue = false)]
        public decimal S7Z58L3TNM { get; set; }
        [DataMember(Order = 2150, EmitDefaultValue = false)]
        public decimal S7Z58L3TDN { get; set; }
        [DataMember(Order = 2160, EmitDefaultValue = false)]
        public decimal S7Z58L31DM { get; set; }
        [DataMember(Order = 2170, EmitDefaultValue = false)]
        public decimal S7Z58L31NM { get; set; }
        [DataMember(Order = 2180, EmitDefaultValue = false)]
        public decimal S7Z58L31DN { get; set; }
        [DataMember(Order = 2190, EmitDefaultValue = false)]
        public decimal S7Z58L32DM { get; set; }
        [DataMember(Order = 2200, EmitDefaultValue = false)]
        public decimal S7Z58L32NM { get; set; }
        [DataMember(Order = 2210, EmitDefaultValue = false)]
        public decimal S7Z58L32DN { get; set; }
        [DataMember(Order = 2220, EmitDefaultValue = false)]
        public decimal S7Z583BDEC { get; set; }
        [DataMember(Order = 2230, EmitDefaultValue = false)]
        public decimal S7Z583LDEC { get; set; }
        [DataMember(Order = 2240, EmitDefaultValue = false)]
        public decimal S7Z583RDEC { get; set; }
        [DataMember(Order = 2250, EmitDefaultValue = false)]
        public decimal S7Z583TDEC { get; set; }
        [DataMember(Order = 2260, EmitDefaultValue = false)]
        public decimal S7Z5831DEC { get; set; }
        [DataMember(Order = 2270, EmitDefaultValue = false)]
        public decimal S7Z5832DEC { get; set; }
        [DataMember(Order = 2280, EmitDefaultValue = false)]
        public string S7Z58L3VG { get; set; }
        [DataMember(Order = 2290, EmitDefaultValue = false)]
        public decimal S7Z58L3VGS { get; set; }
        [DataMember(Order = 2300, EmitDefaultValue = false)]
        public string S7Z58L3VGT { get; set; }
        [DataMember(Order = 2310, EmitDefaultValue = false)]
        public decimal S7Z58L3VGW { get; set; }
        [DataMember(Order = 2320, EmitDefaultValue = false)]
        public decimal S7Z58L3VGH { get; set; }
        [DataMember(Order = 2330, EmitDefaultValue = false)]
        public string S7Z58L3VGD { get; set; }
        [DataMember(Order = 2340, EmitDefaultValue = false)]
        public decimal S7Z58L3VGO { get; set; }
        [DataMember(Order = 2350, EmitDefaultValue = false)]
        public string S7Z58L3BL { get; set; }
        [DataMember(Order = 2360, EmitDefaultValue = false)]
        public string S7Z58L3HL { get; set; }
        [DataMember(Order = 2370, EmitDefaultValue = false)]
        public decimal S7Z58L3SF { get; set; }
        [DataMember(Order = 2380, EmitDefaultValue = false)]
        public string S7Z58L3SD { get; set; }
        [DataMember(Order = 2390, EmitDefaultValue = false)]
        public string S7Z58SFS { get; set; }
        [DataMember(Order = 2400, EmitDefaultValue = false)]
        public string S7USER { get; set; }
        [DataMember(Order = 2410, EmitDefaultValue = false)]
        public string S7PID { get; set; }
        [DataMember(Order = 2420, EmitDefaultValue = false)]
        public string S7JOBN { get; set; }
        [DataMember(Order = 2430, EmitDefaultValue = false)]
        public decimal S7UPMJ { get; set; }
        [DataMember(Order = 2440, EmitDefaultValue = false)]
        public decimal S7TDAY { get; set; }

        [DataMember(Order = 245, EmitDefaultValue = false)]
        public decimal S7Z5803005 { get; set; }
        [DataMember(Order = 245, EmitDefaultValue = false)]
        public decimal S7Z5804595 { get; set; }
        [DataMember(Order = 250, EmitDefaultValue = false)]
        public decimal CMCFGID { get; set; }
        [DataMember(Order = 252, EmitDefaultValue = false)]
        public DateTime CMDateTime { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string WASRST { get; set; }

        [DataMember(Order = 2490, EmitDefaultValue = false)]
        public decimal IBANPL { get; set; }
        [DataMember(Order = 2500, EmitDefaultValue = false)]
        public string ABALPH { get; set; }
        [DataMember(Order = 2510, EmitDefaultValue = false)]
        public string IBGLPT { get; set; }
        [DataMember(Order = 2520, EmitDefaultValue = false)]
        public decimal IBUPMJ { get; set; }
        [DataMember(Order = 2530, EmitDefaultValue = false)]
        public decimal IBTDAY { get; set; }
    }
}
