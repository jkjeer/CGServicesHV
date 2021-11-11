using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F580030s : List<F580030>
    {
    }

    [DataContract(Namespace = "")]
    public class F580030
    {
        public F580030()
        {
            SFUKID = 0;
            SFEDUS = "";
            SFEDBT = "";
            SFEDTN = "";
            SFEDLN = 0;
            SFEDCT = "";
            SFTYTN = "";
            SFEDFT = "";
            SFEDDT = 0;
            SFDRIN = "";
            SFEDDL = 0;
            SFEDSP = "";
            SFPNID = "";
            SFTNAC = "";
            SFMMCU = "";
            SFACDT = 0;
            SFSTTM = "";
            SFDTMS = "";
            SFLITM = "";
            SFLOTN = "";
            SFZ58SN = "";
            SFDOCO = 0;
            SFDCTO = "";
            SFMCU = "";
            SFZ58SCN = 0;
            SFZ58SCSN = 0;
            SFANP = 0;
            SFANPA = 0;
            SFSFT = "";
            SFZ58PCR = "";
            SFZ58RCD = "";
            SFUORG = 0;
            SFSOCN = 0;
            SFLOCN = "";
            SFZ58RID = 0;
            SFZ58RT = "";
            SFZ58GWT = 0;
            SFZ58GWTT = 0;
            SFZ58IGLN = "";
            SFZ58UNID = "";
            SFZ58UID = "";
            SFTKNR = "";
            SFZ58PT = "";
            SFZ58L1CLN = 0;
            SFZ58BSF = 0;
            SFZ5850000 = 0;
            SFLOCATION = "";
            SFZ58LDOCO = 0;
            SFZ58LNID = 0;
            SFZ58FLAG = "";
            SFZ58LWSTN = "";
            SFJFLG = "";
            SFFLAG = "";
            SFUSER = "";
            SFUPMJ = 0;
            SFUPMT = 0;
            SFPID = "";
            SFZ58RCP = "";
            SFZ58RCC = "";
            SFZ58IP01 = "";
            SFZ58GT = "";
            SFZ58HST = "";
            SFZ58AID = "";
            SFUM = "";
            SFZ58SFSTS = "";
            SFGWID = 0;
            SFGDEP = 0;
            SFGHET = 0;
            SFZ58RTWT = 0;
        }

        [DataMember(Order = 1)]
        public decimal SFUKID { get; set; }
        [DataMember(Order = 2)]
        public string SFEDUS { get; set; }
        [DataMember(Order = 3)]
        public string SFEDBT { get; set; }
        [DataMember(Order = 4)]
        public string SFEDTN { get; set; }
        [DataMember(Order = 5)]
        public decimal SFEDLN { get; set; }
        [DataMember(Order = 6)]
        public string SFEDCT { get; set; }
        [DataMember(Order = 7)]
        public string SFTYTN { get; set; }
        [DataMember(Order = 8)]
        public string SFEDFT { get; set; }
        [DataMember(Order = 9)]
        public decimal SFEDDT { get; set; }
        [DataMember(Order = 10)]
        public string SFDRIN { get; set; }
        [DataMember(Order = 11)]
        public decimal SFEDDL { get; set; }
        [DataMember(Order = 12)]
        public string SFEDSP { get; set; }
        [DataMember(Order = 13)]
        public string SFPNID { get; set; }
        [DataMember(Order = 14)]
        public string SFTNAC { get; set; }
        [DataMember(Order = 15)]
        public string SFMMCU { get; set; }
        [DataMember(Order = 16)]
        public decimal SFACDT { get; set; }
        [DataMember(Order = 17)]
        public string SFSTTM { get; set; }
        [DataMember(Order = 18)]
        public string SFDTMS { get; set; }
        [DataMember(Order = 19)]
        public string SFLITM { get; set; }
        [DataMember(Order = 20)]
        public string SFLOTN { get; set; }
        [DataMember(Order = 21)]
        public string SFZ58SN { get; set; }
        [DataMember(Order = 22)]
        public decimal SFDOCO { get; set; }
        [DataMember(Order = 23)]
        public string SFDCTO { get; set; }
        [DataMember(Order = 24)]
        public string SFMCU { get; set; }
        [DataMember(Order = 25)]
        public decimal SFZ58SCN { get; set; }
        [DataMember(Order = 26)]
        public decimal SFZ58SCSN { get; set; }
        [DataMember(Order = 27)]
        public decimal SFANP { get; set; }
        [DataMember(Order = 28)]
        public decimal SFANPA { get; set; }
        [DataMember(Order = 29)]
        public string SFSFT { get; set; }
        [DataMember(Order = 30)]
        public string SFZ58PCR { get; set; }
        [DataMember(Order = 31)]
        public string SFZ58RCD { get; set; }
        [DataMember(Order = 32)]
        public decimal SFUORG { get; set; }
        [DataMember(Order = 33)]
        public decimal SFSOCN { get; set; }
        [DataMember(Order = 34)]
        public string SFLOCN { get; set; }
        [DataMember(Order = 35)]
        public decimal SFZ58RID { get; set; }
        [DataMember(Order = 36)]
        public string SFZ58RT { get; set; }
        [DataMember(Order = 37)]
        public decimal SFZ58GWT { get; set; }
        [DataMember(Order = 38)]
        public decimal SFZ58GWTT { get; set; }
        [DataMember(Order = 39)]
        public string SFZ58IGLN { get; set; }
        [DataMember(Order = 40)]
        public string SFZ58UNID { get; set; }
        [DataMember(Order = 41)]
        public string SFZ58UID { get; set; }
        [DataMember(Order = 42)]
        public string SFTKNR { get; set; }
        [DataMember(Order = 43)]
        public string SFZ58PT { get; set; }
        [DataMember(Order = 44)]
        public decimal SFZ58L1CLN { get; set; }
        [DataMember(Order = 45)]
        public decimal SFZ58BSF { get; set; }
        [DataMember(Order = 46)]
        public decimal SFZ5850000 { get; set; }
        [DataMember(Order = 47)]
        public string SFLOCATION { get; set; }
        [DataMember(Order = 48)]
        public decimal SFZ58LDOCO { get; set; }
        [DataMember(Order = 49)]
        public decimal SFZ58LNID { get; set; }
        [DataMember(Order = 50)]
        public string SFZ58FLAG { get; set; }
        [DataMember(Order = 51)]
        public string SFZ58LWSTN { get; set; }
        [DataMember(Order = 52)]
        public string SFJFLG { get; set; }
        [DataMember(Order = 53)]
        public string SFFLAG { get; set; }
        [DataMember(Order = 54)]
        public string SFUSER { get; set; }
        [DataMember(Order = 55)]
        public decimal SFUPMJ { get; set; }
        [DataMember(Order = 56)]
        public decimal SFUPMT { get; set; }
        [DataMember(Order = 57)]
        public string SFPID { get; set; }
        [DataMember(Order = 58)]
        public string SFZ58RCP { get; set; }
        [DataMember(Order = 59)]
        public string SFZ58RCC { get; set; }
        [DataMember(Order = 60)]
        public string SFZ58IP01 { get; set; }
        [DataMember(Order = 61)]
        public string SFZ58GT { get; set; }
        [DataMember(Order = 62)]
        public string SFZ58HST { get; set; }
        [DataMember(Order = 63)]
        public string SFZ58AID { get; set; }
        [DataMember(Order = 64)]
        public string SFUM { get; set; }
        [DataMember(Order = 66)]
        public string SFZ58SFSTS { get; set; }
        [DataMember(Order = 68)]
        public decimal SFGWID { get; set; }
        [DataMember(Order = 70)]
        public decimal SFGDEP { get; set; }
        [DataMember(Order = 72)]
        public decimal SFGHET { get; set; }
        [DataMember(Order = 74)]
        public decimal SFZ58RTWT { get; set; }
    }
}
