using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F580024s : List<F580024>
    {
    }

    [DataContract(Namespace = "")]
    public class F580024
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string S8EDBT { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal S8EDLN { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string S8MCU { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal S8Z58SCN { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal S8Z58SCSN { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string S8Z58SCSTS { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string S8EDSP { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public decimal S8Z58L1SSZ { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string S8Z58L1STP { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string S8Z58S1GM { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal S8Z58L2SSZ { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string S8Z58L2STP { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal S8Z58M1A1 { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string S8Z58M1A1O { get; set; }
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public decimal S8Z58M2A1 { get; set; }
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string S8Z58M2A1O { get; set; }
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public decimal S8Z58GHA1 { get; set; }
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public decimal S8Z58GVA1 { get; set; }
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public decimal S8Z58OWA1 { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal S8Z58OHA1 { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public decimal S8Z58OOA1 { get; set; }
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public decimal S8Z58CCA1 { get; set; }
        [DataMember(Order = 230, EmitDefaultValue = false)]
        public string S8Z58BA1 { get; set; }
        [DataMember(Order = 240, EmitDefaultValue = false)]
        public decimal S8Z58NA1 { get; set; }
        [DataMember(Order = 250, EmitDefaultValue = false)]
        public string S8Z58SA1 { get; set; }
        [DataMember(Order = 260, EmitDefaultValue = false)]
        public string S8Z58TA1 { get; set; }
        [DataMember(Order = 270, EmitDefaultValue = false)]
        public string S8Z58GSS1 { get; set; }
        [DataMember(Order = 280, EmitDefaultValue = false)]
        public decimal S8Z585555 { get; set; }
        [DataMember(Order = 290, EmitDefaultValue = false)]
        public string S8Z58NFRC { get; set; }
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public decimal S8Z58NFRCV { get; set; }
        [DataMember(Order = 310, EmitDefaultValue = false)]
        public decimal S8Z58NFRCH { get; set; }
        [DataMember(Order = 320, EmitDefaultValue = false)]
        public string S8Z58IGNMC { get; set; }
        [DataMember(Order = 330, EmitDefaultValue = false)]
        public decimal S8Z58IGSIV { get; set; }
        [DataMember(Order = 340, EmitDefaultValue = false)]
        public decimal S8Z58IGSIH { get; set; }
        [DataMember(Order = 350, EmitDefaultValue = false)]
        public decimal S8Z58M1A2 { get; set; }
        [DataMember(Order = 360, EmitDefaultValue = false)]
        public string S8Z58M1A2O { get; set; }
        [DataMember(Order = 370, EmitDefaultValue = false)]
        public decimal S8Z58M2A2 { get; set; }
        [DataMember(Order = 380, EmitDefaultValue = false)]
        public string S8Z58M2A2O { get; set; }
        [DataMember(Order = 390, EmitDefaultValue = false)]
        public decimal S8Z58GHA2 { get; set; }
        [DataMember(Order = 400, EmitDefaultValue = false)]
        public decimal S8Z58GVA2 { get; set; }
        [DataMember(Order = 410, EmitDefaultValue = false)]
        public decimal S8Z58OWA2 { get; set; }
        [DataMember(Order = 420, EmitDefaultValue = false)]
        public decimal S8Z58OHA2 { get; set; }
        [DataMember(Order = 430, EmitDefaultValue = false)]
        public decimal S8Z58OOA2 { get; set; }
        [DataMember(Order = 440, EmitDefaultValue = false)]
        public decimal S8Z58CCA2 { get; set; }
        [DataMember(Order = 450, EmitDefaultValue = false)]
        public string S8Z58BA2 { get; set; }
        [DataMember(Order = 460, EmitDefaultValue = false)]
        public decimal S8Z58NA2 { get; set; }
        [DataMember(Order = 470, EmitDefaultValue = false)]
        public string S8Z58SA2 { get; set; }
        [DataMember(Order = 480, EmitDefaultValue = false)]
        public string S8Z58TA2 { get; set; }
        [DataMember(Order = 490, EmitDefaultValue = false)]
        public string S8Z58IPC { get; set; }
        [DataMember(Order = 500, EmitDefaultValue = false)]
        public decimal S8Z58IGVP1 { get; set; }
        [DataMember(Order = 510, EmitDefaultValue = false)]
        public decimal S8Z58IGVP2 { get; set; }
        [DataMember(Order = 520, EmitDefaultValue = false)]
        public decimal S8Z58IGVP3 { get; set; }
        [DataMember(Order = 530, EmitDefaultValue = false)]
        public decimal S8Z58IGVP4 { get; set; }
        [DataMember(Order = 540, EmitDefaultValue = false)]
        public decimal S8Z58IGVP5 { get; set; }
        [DataMember(Order = 550, EmitDefaultValue = false)]
        public decimal S8Z58IGVP6 { get; set; }
        [DataMember(Order = 560, EmitDefaultValue = false)]
        public decimal S8Z58IGVP7 { get; set; }
        [DataMember(Order = 570, EmitDefaultValue = false)]
        public decimal S8Z58IGVP8 { get; set; }
        [DataMember(Order = 580, EmitDefaultValue = false)]
        public decimal S8Z58IGVP9 { get; set; }
        [DataMember(Order = 590, EmitDefaultValue = false)]
        public decimal S8Z58IGV10 { get; set; }
        [DataMember(Order = 600, EmitDefaultValue = false)]
        public decimal S8Z58IGV11 { get; set; }
        [DataMember(Order = 610, EmitDefaultValue = false)]
        public decimal S8Z58IGV12 { get; set; }
        [DataMember(Order = 620, EmitDefaultValue = false)]
        public decimal S8Z58IGV13 { get; set; }
        [DataMember(Order = 630, EmitDefaultValue = false)]
        public decimal S8Z58IGV14 { get; set; }
        [DataMember(Order = 640, EmitDefaultValue = false)]
        public decimal S8Z58IGV15 { get; set; }
        [DataMember(Order = 650, EmitDefaultValue = false)]
        public decimal S8Z58IGV16 { get; set; }
        [DataMember(Order = 660, EmitDefaultValue = false)]
        public decimal S8Z58IGV17 { get; set; }
        [DataMember(Order = 670, EmitDefaultValue = false)]
        public decimal S8Z58IGV18 { get; set; }
        [DataMember(Order = 680, EmitDefaultValue = false)]
        public decimal S8Z58IGV19 { get; set; }
        [DataMember(Order = 690, EmitDefaultValue = false)]
        public decimal S8Z58IGV20 { get; set; }
        [DataMember(Order = 700, EmitDefaultValue = false)]
        public decimal S8Z58IGHP1 { get; set; }
        [DataMember(Order = 710, EmitDefaultValue = false)]
        public decimal S8Z58IGHP2 { get; set; }
        [DataMember(Order = 720, EmitDefaultValue = false)]
        public decimal S8Z58IGHP3 { get; set; }
        [DataMember(Order = 730, EmitDefaultValue = false)]
        public decimal S8Z58IGHP4 { get; set; }
        [DataMember(Order = 740, EmitDefaultValue = false)]
        public decimal S8Z58IGHP5 { get; set; }
        [DataMember(Order = 750, EmitDefaultValue = false)]
        public decimal S8Z58IGHP6 { get; set; }
        [DataMember(Order = 760, EmitDefaultValue = false)]
        public decimal S8Z58IGHP7 { get; set; }
        [DataMember(Order = 770, EmitDefaultValue = false)]
        public decimal S8Z58IGHP8 { get; set; }
        [DataMember(Order = 780, EmitDefaultValue = false)]
        public decimal S8Z58IGHP9 { get; set; }
        [DataMember(Order = 790, EmitDefaultValue = false)]
        public decimal S8Z58IGH10 { get; set; }
        [DataMember(Order = 800, EmitDefaultValue = false)]
        public decimal S8Z58IGH11 { get; set; }
        [DataMember(Order = 810, EmitDefaultValue = false)]
        public decimal S8Z58IGH12 { get; set; }
        [DataMember(Order = 820, EmitDefaultValue = false)]
        public decimal S8Z58IGH13 { get; set; }
        [DataMember(Order = 830, EmitDefaultValue = false)]
        public decimal S8Z58IGH14 { get; set; }
        [DataMember(Order = 840, EmitDefaultValue = false)]
        public decimal S8Z58IGH15 { get; set; }
        [DataMember(Order = 850, EmitDefaultValue = false)]
        public decimal S8Z58IGH16 { get; set; }
        [DataMember(Order = 860, EmitDefaultValue = false)]
        public decimal S8Z58IGH17 { get; set; }
        [DataMember(Order = 870, EmitDefaultValue = false)]
        public decimal S8Z58IGH18 { get; set; }
        [DataMember(Order = 880, EmitDefaultValue = false)]
        public decimal S8Z58IGH19 { get; set; }
        [DataMember(Order = 890, EmitDefaultValue = false)]
        public decimal S8Z58IGH20 { get; set; }
        [DataMember(Order = 900, EmitDefaultValue = false)]
        public string S8Z587563 { get; set; }
        [DataMember(Order = 910, EmitDefaultValue = false)]
        public string S8Z587566 { get; set; }
        [DataMember(Order = 920, EmitDefaultValue = false)]
        public decimal S8Z587569 { get; set; }
        [DataMember(Order = 930, EmitDefaultValue = false)]
        public string S8Z587572 { get; set; }
        [DataMember(Order = 940, EmitDefaultValue = false)]
        public string S8Z587575 { get; set; }
        [DataMember(Order = 950, EmitDefaultValue = false)]
        public string S8Z587578 { get; set; }
        [DataMember(Order = 960, EmitDefaultValue = false)]
        public decimal S8Z587581 { get; set; }
        [DataMember(Order = 970, EmitDefaultValue = false)]
        public string S8Z587584 { get; set; }
        [DataMember(Order = 980, EmitDefaultValue = false)]
        public string S8Z58RPRVH { get; set; }
        [DataMember(Order = 990, EmitDefaultValue = false)]
        public string S8Z58RTNIO { get; set; }
        [DataMember(Order = 1000, EmitDefaultValue = false)]
        public decimal S8Z58RPRR { get; set; }
        [DataMember(Order = 1010, EmitDefaultValue = false)]
        public decimal S8Z58RPRT { get; set; }
        [DataMember(Order = 1020, EmitDefaultValue = false)]
        public string S8Z58PKGIL { get; set; }
        [DataMember(Order = 1030, EmitDefaultValue = false)]
        public string S8Z58L3EW { get; set; }
        [DataMember(Order = 1040, EmitDefaultValue = false)]
        public string S8Z58L3TL { get; set; }
        [DataMember(Order = 1050, EmitDefaultValue = false)]
        public decimal S8Z58L3PTS { get; set; }
        [DataMember(Order = 1060, EmitDefaultValue = false)]
        public decimal S8Z58L3LCS { get; set; }
        [DataMember(Order = 1070, EmitDefaultValue = false)]
        public string S8Z5840430 { get; set; }
        [DataMember(Order = 1080, EmitDefaultValue = false)]
        public string S8Z5840450 { get; set; }
        [DataMember(Order = 1090, EmitDefaultValue = false)]
        public string S8Z58SFS { get; set; }
        [DataMember(Order = 1100, EmitDefaultValue = false)]
        public decimal S8Z58STK { get; set; }
        [DataMember(Order = 1110, EmitDefaultValue = false)]
        public decimal S8Z58ROW { get; set; }
        [DataMember(Order = 1120, EmitDefaultValue = false)]
        public string S8URCD { get; set; }
        [DataMember(Order = 1130, EmitDefaultValue = false)]
        public decimal S8URDT { get; set; }
        [DataMember(Order = 1140, EmitDefaultValue = false)]
        public decimal S8URAB { get; set; }
        [DataMember(Order = 1150, EmitDefaultValue = false)]
        public string S8URRF { get; set; }
        [DataMember(Order = 1160, EmitDefaultValue = false)]
        public string S8USER { get; set; }
        [DataMember(Order = 1170, EmitDefaultValue = false)]
        public string S8PID { get; set; }
        [DataMember(Order = 1180, EmitDefaultValue = false)]
        public string S8JOBN { get; set; }
        [DataMember(Order = 1190, EmitDefaultValue = false)]
        public decimal S8UPMJ { get; set; }
        [DataMember(Order = 1200, EmitDefaultValue = false)]
        public decimal S8TDAY { get; set; }
        [DataMember(Order = 1210, EmitDefaultValue = false)]
        public string S8SRP6 { get; set; }
        [DataMember(Order = 1220, EmitDefaultValue = false)]
        public string S8MMCU { get; set; }
        [DataMember(Order = 1230, EmitDefaultValue = false)]
        public decimal S8Z58BSF { get; set; }
        [DataMember(Order = 1240, EmitDefaultValue = false)]
        public decimal S8Z58USF { get; set; }
        [DataMember(Order = 1250, EmitDefaultValue = false)]
        public string S8Z5800495 { get; set; }
        [DataMember(Order = 1260, EmitDefaultValue = false)]
        public decimal S8Z5803005 { get; set; }
        [DataMember(Order = 1270, EmitDefaultValue = false)]
        public decimal S8Z5804595 { get; set; }
        [DataMember(Order = 1280, EmitDefaultValue = false)]
        public string S8Z5805735 { get; set; }
        [DataMember(Order = 1290, EmitDefaultValue = false)]
        public string S8Z5840470 { get; set; }
        [DataMember(Order = 1300, EmitDefaultValue = false)]
        public string S8Z5840480 { get; set; }
        [DataMember(Order = 1310, EmitDefaultValue = false)]
        public decimal S8Z58SLT { get; set; }
        [DataMember(Order = 1320, EmitDefaultValue = false)]
        public string S8Z58SLF { get; set; }
        [DataMember(Order = 1330, EmitDefaultValue = false)]
        public decimal S8Z58SUT { get; set; }
        [DataMember(Order = 1340, EmitDefaultValue = false)]
        public string S8Z58SUF { get; set; }
        [DataMember(Order = 1350, EmitDefaultValue = false)]
        public decimal S8Z5805560 { get; set; }
        [DataMember(Order = 1360, EmitDefaultValue = false)]
        public string S8Z5805570 { get; set; }
        [DataMember(Order = 1370, EmitDefaultValue = false)]
        public string S8Z5805470 { get; set; }
        [DataMember(Order = 1380, EmitDefaultValue = false)]
        public string S8Z5806070 { get; set; }
        [DataMember(Order = 1390, EmitDefaultValue = false)]
        public decimal S8Z5801295 { get; set; }
        [DataMember(Order = 1400, EmitDefaultValue = false)]
        public decimal S8Z5801300 { get; set; }
        [DataMember(Order = 1410, EmitDefaultValue = false)]
        public string S8Z5801296 { get; set; }
        [DataMember(Order = 1420, EmitDefaultValue = false)]
        public string S8Z5801301 { get; set; }
        [DataMember(Order = 1430, EmitDefaultValue = false)]
        public decimal S8Z5802960 { get; set; }
        [DataMember(Order = 1440, EmitDefaultValue = false)]
        public decimal S8Z5802965 { get; set; }
        [DataMember(Order = 1450, EmitDefaultValue = false)]
        public string S8Z5802961 { get; set; }
        [DataMember(Order = 1460, EmitDefaultValue = false)]
        public string S8Z5802966 { get; set; }
        [DataMember(Order = 1470, EmitDefaultValue = false)]
        public decimal S8Z5804545 { get; set; }
        [DataMember(Order = 1480, EmitDefaultValue = false)]
        public decimal S8Z5804550 { get; set; }
        [DataMember(Order = 1490, EmitDefaultValue = false)]
        public string S8Z5804546 { get; set; }
        [DataMember(Order = 1500, EmitDefaultValue = false)]
        public string S8Z5804551 { get; set; }
        [DataMember(Order = 1510, EmitDefaultValue = false)]
        public string S8Z5801512 { get; set; }
        [DataMember(Order = 1520, EmitDefaultValue = false)]
        public string S8Z5801480 { get; set; }
        [DataMember(Order = 1530, EmitDefaultValue = false)]
        public decimal S8Z5801481 { get; set; }
        [DataMember(Order = 1540, EmitDefaultValue = false)]
        public decimal S8Z5801482 { get; set; }
        [DataMember(Order = 1550, EmitDefaultValue = false)]
        public string S8Z58NFRC2 { get; set; }
        [DataMember(Order = 1560, EmitDefaultValue = false)]
        public string S8Z58SWA2 { get; set; }
        [DataMember(Order = 1570, EmitDefaultValue = false)]
        public string S8Z58SHA2 { get; set; }
        [DataMember(Order = 1580, EmitDefaultValue = false)]
        public string S8Z58SOA2 { get; set; }
        [DataMember(Order = 1590, EmitDefaultValue = false)]
        public string S8Z5840455 { get; set; }
        [DataMember(Order = 1600, EmitDefaultValue = false)]
        public string S8Z5801900 { get; set; }
        [DataMember(Order = 1610, EmitDefaultValue = false)]
        public string S8Z5803520 { get; set; }
        [DataMember(Order = 1620, EmitDefaultValue = false)]
        public string S8Z5805110 { get; set; }
        [DataMember(Order = 1630, EmitDefaultValue = false)]
        public string S8Z5801840 { get; set; }
        [DataMember(Order = 1640, EmitDefaultValue = false)]
        public decimal S8ITM { get; set; }
        [DataMember(Order = 1650, EmitDefaultValue = false)]
        public string S8PPIK { get; set; }
        [DataMember(Order = 1660, EmitDefaultValue = false)]
        public string S8Z5801630 { get; set; }
        [DataMember(Order = 1670, EmitDefaultValue = false)]
        public string S8Z5851965 { get; set; }
        [DataMember(Order = 1680, EmitDefaultValue = false)]
        public string S8Z5851966 { get; set; }
        [DataMember(Order = 1690, EmitDefaultValue = false)]
        public string S8Z5801865 { get; set; }
        [DataMember(Order = 1700, EmitDefaultValue = false)]
        public string S8Z5801890 { get; set; }
        [DataMember(Order = 1710, EmitDefaultValue = false)]
        public string S8Z58L1LPS { get; set; }
        [DataMember(Order = 1720, EmitDefaultValue = false)]
        public string S8Z58L1LXS { get; set; }
        [DataMember(Order = 1730, EmitDefaultValue = false)]
        public decimal S8Z5801905 { get; set; }
        [DataMember(Order = 1740, EmitDefaultValue = false)]
        public decimal S8Z5801906 { get; set; }
        [DataMember(Order = 1750, EmitDefaultValue = false)]
        public decimal S8Z5801907 { get; set; }
        [DataMember(Order = 1760, EmitDefaultValue = false)]
        public string S8Z5803485 { get; set; }
        [DataMember(Order = 1770, EmitDefaultValue = false)]
        public string S8Z5803510 { get; set; }
        [DataMember(Order = 1780, EmitDefaultValue = false)]
        public string S8Z58L2LPS { get; set; }
        [DataMember(Order = 1790, EmitDefaultValue = false)]
        public string S8Z58L2LXS { get; set; }
        [DataMember(Order = 1800, EmitDefaultValue = false)]
        public decimal S8Z5803525 { get; set; }
        [DataMember(Order = 1810, EmitDefaultValue = false)]
        public decimal S8Z5803526 { get; set; }
        [DataMember(Order = 1820, EmitDefaultValue = false)]
        public decimal S8Z5803527 { get; set; }
        [DataMember(Order = 1830, EmitDefaultValue = false)]
        public string S8Z5805075 { get; set; }
        [DataMember(Order = 1840, EmitDefaultValue = false)]
        public string S8Z5805100 { get; set; }
        [DataMember(Order = 1850, EmitDefaultValue = false)]
        public string S8Z58L3LPS { get; set; }
        [DataMember(Order = 1860, EmitDefaultValue = false)]
        public string S8Z58L3LXS { get; set; }
        [DataMember(Order = 1870, EmitDefaultValue = false)]
        public decimal S8Z5805115 { get; set; }
        [DataMember(Order = 1880, EmitDefaultValue = false)]
        public decimal S8Z5805116 { get; set; }
        [DataMember(Order = 1890, EmitDefaultValue = false)]
        public decimal S8Z5805117 { get; set; }
        [DataMember(Order = 1900, EmitDefaultValue = false)]
        public string S8Z5801990 { get; set; }
        [DataMember(Order = 1910, EmitDefaultValue = false)]
        public string S8Z5802000 { get; set; }
        [DataMember(Order = 1920, EmitDefaultValue = false)]
        public string S8Z5840435 { get; set; }
        [DataMember(Order = 1930, EmitDefaultValue = false)]
        public decimal S8Z58GLWT { get; set; }
        [DataMember(Order = 1940, EmitDefaultValue = false)]
        public string S8Z5801635 { get; set; }
        [DataMember(Order = 1950, EmitDefaultValue = false)]
        public string S8Z5800550 { get; set; }
        [DataMember(Order = 1960, EmitDefaultValue = false)]
        public decimal S8Z5880230 { get; set; }
        [DataMember(Order = 1970, EmitDefaultValue = false)]
        public string S8Z5840310 { get; set; }
        [DataMember(Order = 1980, EmitDefaultValue = false)]
        public decimal S8Z5851551 { get; set; }
        [DataMember(Order = 1990, EmitDefaultValue = false)]
        public decimal S8Z5851552 { get; set; }
        [DataMember(Order = 2000, EmitDefaultValue = false)]
        public decimal S8Z5851553 { get; set; }
        [DataMember(Order = 2010, EmitDefaultValue = false)]
        public string S8Z58L1RLO { get; set; }
        [DataMember(Order = 2020, EmitDefaultValue = false)]
        public string S8Z58L2RLO { get; set; }
        [DataMember(Order = 2030, EmitDefaultValue = false)]
        public string S8Z58L3RLO { get; set; }
        [DataMember(Order = 2040, EmitDefaultValue = false)]
        public string S8RCD { get; set; }
        [DataMember(Order = 2050, EmitDefaultValue = false)]
        public decimal S8DRQT { get; set; }
        [DataMember(Order = 2060, EmitDefaultValue = false)]
        public string S8Z58BDES { get; set; }
        [DataMember(Order = 2070, EmitDefaultValue = false)]
        public string S8Z58CCV01 { get; set; }
        [DataMember(Order = 2080, EmitDefaultValue = false)]
        public string S8Z58CCV02 { get; set; }
        [DataMember(Order = 2090, EmitDefaultValue = false)]
        public string S8Z58CCV03 { get; set; }
        [DataMember(Order = 2100, EmitDefaultValue = false)]
        public string S8Z58CCV04 { get; set; }
        [DataMember(Order = 2110, EmitDefaultValue = false)]
        public string S8Z58CCV05 { get; set; }
        [DataMember(Order = 2120, EmitDefaultValue = false)]
        public string S8Z58CCV06 { get; set; }
        [DataMember(Order = 2130, EmitDefaultValue = false)]
        public string S8Z58CCV07 { get; set; }
        [DataMember(Order = 2140, EmitDefaultValue = false)]
        public string S8Z58CCV08 { get; set; }
        [DataMember(Order = 2150, EmitDefaultValue = false)]
        public string S8Z58CCV09 { get; set; }
        [DataMember(Order = 2160, EmitDefaultValue = false)]
        public string S8Z58CCV10 { get; set; }
        [DataMember(Order = 2170, EmitDefaultValue = false)]
        public string S8MLNM { get; set; }
        [DataMember(Order = 2180, EmitDefaultValue = false)]
        public string S8Z5899011 { get; set; }
        [DataMember(Order = 2190, EmitDefaultValue = false)]
        public string S8Z5899012 { get; set; }
        [DataMember(Order = 2200, EmitDefaultValue = false)]
        public string S8Z5840415 { get; set; }
        [DataMember(Order = 2210, EmitDefaultValue = false)]
        public string S8Z5805680 { get; set; }
        [DataMember(Order = 2211, EmitDefaultValue = false)]
        public string S8Z5888100 { get; set; }
        [DataMember(Order = 2212, EmitDefaultValue = false)]
        public string S8Z5853001 { get; set; }
        [DataMember(Order = 2213, EmitDefaultValue = false)]
        public string S8Z5853002 { get; set; }
        [DataMember(Order = 2214, EmitDefaultValue = false)]
        public string S8Z58FABDE { get; set; }
        [DataMember(Order = 2290, EmitDefaultValue = false)]
        public string S8Z5801621 { get; set; }
        [DataMember(Order = 2300, EmitDefaultValue = false)]
        public string S8TRNT { get; set; }

        [DataMember(Order = 2330, EmitDefaultValue = false)]
        public string S8Z5852060 { get; set; }
        [DataMember(Order = 2340, EmitDefaultValue = false)]
        public string S8Z5852061 { get; set; }
        [DataMember(Order = 2350, EmitDefaultValue = false)]
        public string S8Z5852062 { get; set; }

        [DataMember(Order = 2350, EmitDefaultValue = false)]
        public string S8Z58IR01 { get; set; }
        [DataMember(Order = 2350, EmitDefaultValue = false)]
        public string S8Z5899074 { get; set; }
        [DataMember(Order = 2350, EmitDefaultValue = false)]
        public decimal S8Z5899075 { get; set; }

    }
}
