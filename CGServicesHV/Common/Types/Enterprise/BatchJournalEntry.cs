using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class BatchJournalEntrys : List<BatchJournalEntry>
    {
    }

    [DataContract(Namespace = "")]
    public class BatchJournalEntry
    {
        public BatchJournalEntry()
        {
            VNEDUS = "";
            VNEDTY = "";
            VNEDSQ = 0;
            VNEDTN = "";
            VNEDCT = "";
            VNEDLN = 0;
            VNEDTS = "";
            VNEDFT = "";
            VNEDDT = 0;
            VNEDER = "";
            VNEDDL = 0;
            VNEDSP = "";
            VNEDTC = "";
            VNEDTR = "";
            VNEDBT = "";
            VNEDGL = "";
            VNEDAN = 0;
            VNKCO = "";
            VNDCT = "";
            VNDOC = 0;
            VNDGJ = 0;
            VNJELN = 0;
            VNEXTL = "";
            VNPOST = "";
            VNICU = 0;
            VNICUT = "";
            VNDICJ = 0;
            VNDSYJ = 0;
            VNTICU = 0;
            VNCO = "";
            VNANI = "";
            VNAM = "";
            VNAID = "";
            VNMCU = "";
            VNOBJ = "";
            VNSUB = "";
            VNSBL = "";
            VNSBLT = "";
            VNLT = "";
            VNPN = 0;
            VNCTRY = 0;
            VNFY = 0;
            VNFQ = "";
            VNCRCD = "";
            VNCRR = 0;
            VNHCRR = 0;
            VNHDGJ = 0;
            VNAA = 0;
            VNU = 0;
            VNUM = "";
            VNGLC = "";
            VNRE = "";
            VNEXA = "";
            VNEXR = "";
            VNR1 = "";
            VNR2 = "";
            VNR3 = "";
            VNSFX = "";
            VNODOC = 0;
            VNODCT = "";
            VNOSFX = "";
            VNPKCO = "";
            VNOKCO = "";
            VNPDCT = "";
            VNAN8 = 0;
            VNCN = "";
            VNDKJ = 0;
            VNDKC = 0;
            VNASID = "";
            VNBRE = "";
            VNRCND = "";
            VNSUMM = "";
            VNPRGE = "";
            VNTNN = "";
            VNALT1 = "";
            VNALT2 = "";
            VNALT3 = "";
            VNALT4 = "";
            VNALT5 = "";
            VNALT6 = "";
            VNALT7 = "";
            VNALT8 = "";
            VNALT9 = "";
            VNALT0 = "";
            VNALTT = "";
            VNALTU = "";
            VNALTV = "";
            VNALTW = "";
            VNALTX = "";
            VNALTZ = "";
            VNDLNA = "";
            VNCFF1 = "";
            VNCFF2 = "";
            VNASM = "";
            VNBC = "";
            VNVINV = "";
            VNIVD = 0;
            VNWR01 = "";
            VNPO = "";
            VNPSFX = "";
            VNDCTO = "";
            VNLNID = 0;
            VNWY = 0;
            VNWN = 0;
            VNFNLP = "";
            VNOPSQ = 0;
            VNJBCD = "";
            VNJBST = "";
            VNHMCU = "";
            VNDOI = 0;
            VNALID = "";
            VNALTY = "";
            VNDSVJ = 0;
            VNTORG = "";
            VNREG_PS = 0;
            VNPYID = 0;
            VNUSER = "";
            VNPID = "";
            VNJOBN = "";
            VNUPMJ = 0;
            VNUPMT = 0;
            VNCRRM = "";
            VNACR = 0;
            VNDGM = 0;
            VNDGD = 0;
            VNDGY = 0;
            VNDG_PS = 0;
            VNDICM = 0;
            VNDICD = 0;
            VNDICY = 0;
            VNDIC_PS = 0;
            VNDSYM = 0;
            VNDSYD = 0;
            VNDSYY = 0;
            VNDSY_PS = 0;
            VNDKM = 0;
            VNDKD = 0;
            VNDKY = 0;
            VNDK_PS = 0;
            VNDSVM = 0;
            VNDSVD = 0;
            VNDSVY = 0;
            VNDSV_PS = 0;
            VNHDGM = 0;
            VNHDGD = 0;
            VNHDGY = 0;
            VNHDG_PS = 0;
            VNDKCM = 0;
            VNDKCD = 0;
            VNDKCY = 0;
            VNDKC_PS = 0;
            VNIVDM = 0;
            VNIVDD = 0;
            VNIVDY = 0;
            VNIVD_PS = 0;
            VNABR1 = "";
            VNABR2 = "";
            VNABR3 = "";
            VNABR4 = "";
            VNABT1 = "";
            VNABT2 = "";
            VNABT3 = "";
            VNABT4 = "";
            VNITM = 0;
            VNPM01 = "";
            VNPM02 = "";
            VNPM03 = "";
            VNPM04 = "";
            VNPM05 = "";
            VNPM06 = "";
            VNPM07 = "";
            VNPM08 = "";
            VNPM09 = "";
            VNPM10 = "";
            VNBCRC = "";
            VNEXR1 = "";
            VNTXA1 = "";
            VNTXITM = 0;
            VNACTB = "";
            VNSTAM = 0;
            VNCTAM = 0;
            VNAG = 0;
            VNAGF = 0;
            VNTKTX = "";
            VNDLNID = 0;
            VNCKNU = "";
            VNBUPC = "";
            VNAHBU = "";
            VNEPGC = "";
            VNJPGC = "";
            VNRC5 = 0;
            VNSFXE = 0;
            VNOFM = "";
        }

        [DataMember(Order = 1)]
        public string VNEDUS { get; set; }
        [DataMember(Order = 2)]
        public string VNEDTY { get; set; }
        [DataMember(Order = 3)]
        public decimal VNEDSQ { get; set; }
        [DataMember(Order = 4)]
        public string VNEDTN { get; set; }
        [DataMember(Order = 5)]
        public string VNEDCT { get; set; }
        [DataMember(Order = 6)]
        public decimal VNEDLN { get; set; }
        [DataMember(Order = 7)]
        public string VNEDTS { get; set; }
        [DataMember(Order = 8)]
        public string VNEDFT { get; set; }
        [DataMember(Order = 9)]
        public decimal VNEDDT { get; set; }
        [DataMember(Order = 10)]
        public string VNEDER { get; set; }
        [DataMember(Order = 11)]
        public decimal VNEDDL { get; set; }
        [DataMember(Order = 12)]
        public string VNEDSP { get; set; }
        [DataMember(Order = 13)]
        public string VNEDTC { get; set; }
        [DataMember(Order = 14)]
        public string VNEDTR { get; set; }
        [DataMember(Order = 15)]
        public string VNEDBT { get; set; }
        [DataMember(Order = 16)]
        public string VNEDGL { get; set; }
        [DataMember(Order = 17)]
        public decimal VNEDAN { get; set; }
        [DataMember(Order = 18)]
        public string VNKCO { get; set; }
        [DataMember(Order = 19)]
        public string VNDCT { get; set; }
        [DataMember(Order = 20)]
        public decimal VNDOC { get; set; }
        [DataMember(Order = 21)]
        public decimal VNDGJ { get; set; }
        [DataMember(Order = 22)]
        public decimal VNJELN { get; set; }
        [DataMember(Order = 23)]
        public string VNEXTL { get; set; }
        [DataMember(Order = 24)]
        public string VNPOST { get; set; }
        [DataMember(Order = 25)]
        public decimal VNICU { get; set; }
        [DataMember(Order = 26)]
        public string VNICUT { get; set; }
        [DataMember(Order = 27)]
        public decimal VNDICJ { get; set; }
        [DataMember(Order = 28)]
        public decimal VNDSYJ { get; set; }
        [DataMember(Order = 29)]
        public decimal VNTICU { get; set; }
        [DataMember(Order = 30)]
        public string VNCO { get; set; }
        [DataMember(Order = 31)]
        public string VNANI { get; set; }
        [DataMember(Order = 32)]
        public string VNAM { get; set; }
        [DataMember(Order = 33)]
        public string VNAID { get; set; }
        [DataMember(Order = 34)]
        public string VNMCU { get; set; }
        [DataMember(Order = 35)]
        public string VNOBJ { get; set; }
        [DataMember(Order = 36)]
        public string VNSUB { get; set; }
        [DataMember(Order = 37)]
        public string VNSBL { get; set; }
        [DataMember(Order = 38)]
        public string VNSBLT { get; set; }
        [DataMember(Order = 39)]
        public string VNLT { get; set; }
        [DataMember(Order = 40)]
        public decimal VNPN { get; set; }
        [DataMember(Order = 41)]
        public decimal VNCTRY { get; set; }
        [DataMember(Order = 42)]
        public decimal VNFY { get; set; }
        [DataMember(Order = 43)]
        public string VNFQ { get; set; }
        [DataMember(Order = 44)]
        public string VNCRCD { get; set; }
        [DataMember(Order = 45)]
        public decimal VNCRR { get; set; }
        [DataMember(Order = 46)]
        public decimal VNHCRR { get; set; }
        [DataMember(Order = 47)]
        public decimal VNHDGJ { get; set; }
        [DataMember(Order = 48)]
        public decimal VNAA { get; set; }
        [DataMember(Order = 49)]
        public decimal VNU { get; set; }
        [DataMember(Order = 50)]
        public string VNUM { get; set; }
        [DataMember(Order = 51)]
        public string VNGLC { get; set; }
        [DataMember(Order = 52)]
        public string VNRE { get; set; }
        [DataMember(Order = 53)]
        public string VNEXA { get; set; }
        [DataMember(Order = 54)]
        public string VNEXR { get; set; }
        [DataMember(Order = 55)]
        public string VNR1 { get; set; }
        [DataMember(Order = 56)]
        public string VNR2 { get; set; }
        [DataMember(Order = 57)]
        public string VNR3 { get; set; }
        [DataMember(Order = 58)]
        public string VNSFX { get; set; }
        [DataMember(Order = 59)]
        public decimal VNODOC { get; set; }
        [DataMember(Order = 60)]
        public string VNODCT { get; set; }
        [DataMember(Order = 61)]
        public string VNOSFX { get; set; }
        [DataMember(Order = 62)]
        public string VNPKCO { get; set; }
        [DataMember(Order = 63)]
        public string VNOKCO { get; set; }
        [DataMember(Order = 64)]
        public string VNPDCT { get; set; }
        [DataMember(Order = 65)]
        public decimal VNAN8 { get; set; }
        [DataMember(Order = 66)]
        public string VNCN { get; set; }
        [DataMember(Order = 67)]
        public decimal VNDKJ { get; set; }
        [DataMember(Order = 68)]
        public decimal VNDKC { get; set; }
        [DataMember(Order = 69)]
        public string VNASID { get; set; }
        [DataMember(Order = 70)]
        public string VNBRE { get; set; }
        [DataMember(Order = 71)]
        public string VNRCND { get; set; }
        [DataMember(Order = 72)]
        public string VNSUMM { get; set; }
        [DataMember(Order = 73)]
        public string VNPRGE { get; set; }
        [DataMember(Order = 74)]
        public string VNTNN { get; set; }
        [DataMember(Order = 75)]
        public string VNALT1 { get; set; }
        [DataMember(Order = 76)]
        public string VNALT2 { get; set; }
        [DataMember(Order = 77)]
        public string VNALT3 { get; set; }
        [DataMember(Order = 78)]
        public string VNALT4 { get; set; }
        [DataMember(Order = 79)]
        public string VNALT5 { get; set; }
        [DataMember(Order = 80)]
        public string VNALT6 { get; set; }
        [DataMember(Order = 81)]
        public string VNALT7 { get; set; }
        [DataMember(Order = 82)]
        public string VNALT8 { get; set; }
        [DataMember(Order = 83)]
        public string VNALT9 { get; set; }
        [DataMember(Order = 84)]
        public string VNALT0 { get; set; }
        [DataMember(Order = 85)]
        public string VNALTT { get; set; }
        [DataMember(Order = 86)]
        public string VNALTU { get; set; }
        [DataMember(Order = 87)]
        public string VNALTV { get; set; }
        [DataMember(Order = 88)]
        public string VNALTW { get; set; }
        [DataMember(Order = 89)]
        public string VNALTX { get; set; }
        [DataMember(Order = 90)]
        public string VNALTZ { get; set; }
        [DataMember(Order = 91)]
        public string VNDLNA { get; set; }
        [DataMember(Order = 92)]
        public string VNCFF1 { get; set; }
        [DataMember(Order = 93)]
        public string VNCFF2 { get; set; }
        [DataMember(Order = 94)]
        public string VNASM { get; set; }
        [DataMember(Order = 95)]
        public string VNBC { get; set; }
        [DataMember(Order = 96)]
        public string VNVINV { get; set; }
        [DataMember(Order = 97)]
        public decimal VNIVD { get; set; }
        [DataMember(Order = 98)]
        public string VNWR01 { get; set; }
        [DataMember(Order = 99)]
        public string VNPO { get; set; }
        [DataMember(Order = 100)]
        public string VNPSFX { get; set; }
        [DataMember(Order = 101)]
        public string VNDCTO { get; set; }
        [DataMember(Order = 102)]
        public decimal VNLNID { get; set; }
        [DataMember(Order = 103)]
        public decimal VNWY { get; set; }
        [DataMember(Order = 104)]
        public decimal VNWN { get; set; }
        [DataMember(Order = 105)]
        public string VNFNLP { get; set; }
        [DataMember(Order = 106)]
        public decimal VNOPSQ { get; set; }
        [DataMember(Order = 107)]
        public string VNJBCD { get; set; }
        [DataMember(Order = 108)]
        public string VNJBST { get; set; }
        [DataMember(Order = 109)]
        public string VNHMCU { get; set; }
        [DataMember(Order = 110)]
        public decimal VNDOI { get; set; }
        [DataMember(Order = 111)]
        public string VNALID { get; set; }
        [DataMember(Order = 112)]
        public string VNALTY { get; set; }
        [DataMember(Order = 113)]
        public decimal VNDSVJ { get; set; }
        [DataMember(Order = 114)]
        public string VNTORG { get; set; }
        [DataMember(Order = 115)]
        public decimal VNREG_PS { get; set; }
        [DataMember(Order = 116)]
        public decimal VNPYID { get; set; }
        [DataMember(Order = 117)]
        public string VNUSER { get; set; }
        [DataMember(Order = 118)]
        public string VNPID { get; set; }
        [DataMember(Order = 119)]
        public string VNJOBN { get; set; }
        [DataMember(Order = 120)]
        public decimal VNUPMJ { get; set; }
        [DataMember(Order = 121)]
        public decimal VNUPMT { get; set; }
        [DataMember(Order = 122)]
        public string VNCRRM { get; set; }
        [DataMember(Order = 123)]
        public decimal VNACR { get; set; }
        [DataMember(Order = 124)]
        public decimal VNDGM { get; set; }
        [DataMember(Order = 125)]
        public decimal VNDGD { get; set; }
        [DataMember(Order = 126)]
        public decimal VNDGY { get; set; }
        [DataMember(Order = 127)]
        public decimal VNDG_PS { get; set; }
        [DataMember(Order = 128)]
        public decimal VNDICM { get; set; }
        [DataMember(Order = 129)]
        public decimal VNDICD { get; set; }
        [DataMember(Order = 130)]
        public decimal VNDICY { get; set; }
        [DataMember(Order = 131)]
        public decimal VNDIC_PS { get; set; }
        [DataMember(Order = 132)]
        public decimal VNDSYM { get; set; }
        [DataMember(Order = 133)]
        public decimal VNDSYD { get; set; }
        [DataMember(Order = 134)]
        public decimal VNDSYY { get; set; }
        [DataMember(Order = 135)]
        public decimal VNDSY_PS { get; set; }
        [DataMember(Order = 136)]
        public decimal VNDKM { get; set; }
        [DataMember(Order = 137)]
        public decimal VNDKD { get; set; }
        [DataMember(Order = 138)]
        public decimal VNDKY { get; set; }
        [DataMember(Order = 139)]
        public decimal VNDK_PS { get; set; }
        [DataMember(Order = 140)]
        public decimal VNDSVM { get; set; }
        [DataMember(Order = 141)]
        public decimal VNDSVD { get; set; }
        [DataMember(Order = 142)]
        public decimal VNDSVY { get; set; }
        [DataMember(Order = 143)]
        public decimal VNDSV_PS { get; set; }
        [DataMember(Order = 144)]
        public decimal VNHDGM { get; set; }
        [DataMember(Order = 145)]
        public decimal VNHDGD { get; set; }
        [DataMember(Order = 146)]
        public decimal VNHDGY { get; set; }
        [DataMember(Order = 147)]
        public decimal VNHDG_PS { get; set; }
        [DataMember(Order = 148)]
        public decimal VNDKCM { get; set; }
        [DataMember(Order = 149)]
        public decimal VNDKCD { get; set; }
        [DataMember(Order = 150)]
        public decimal VNDKCY { get; set; }
        [DataMember(Order = 151)]
        public decimal VNDKC_PS { get; set; }
        [DataMember(Order = 152)]
        public decimal VNIVDM { get; set; }
        [DataMember(Order = 153)]
        public decimal VNIVDD { get; set; }
        [DataMember(Order = 154)]
        public decimal VNIVDY { get; set; }
        [DataMember(Order = 155)]
        public decimal VNIVD_PS { get; set; }
        [DataMember(Order = 156)]
        public string VNABR1 { get; set; }
        [DataMember(Order = 157)]
        public string VNABR2 { get; set; }
        [DataMember(Order = 158)]
        public string VNABR3 { get; set; }
        [DataMember(Order = 159)]
        public string VNABR4 { get; set; }
        [DataMember(Order = 160)]
        public string VNABT1 { get; set; }
        [DataMember(Order = 161)]
        public string VNABT2 { get; set; }
        [DataMember(Order = 162)]
        public string VNABT3 { get; set; }
        [DataMember(Order = 163)]
        public string VNABT4 { get; set; }
        [DataMember(Order = 164)]
        public decimal VNITM { get; set; }
        [DataMember(Order = 165)]
        public string VNPM01 { get; set; }
        [DataMember(Order = 166)]
        public string VNPM02 { get; set; }
        [DataMember(Order = 167)]
        public string VNPM03 { get; set; }
        [DataMember(Order = 168)]
        public string VNPM04 { get; set; }
        [DataMember(Order = 169)]
        public string VNPM05 { get; set; }
        [DataMember(Order = 170)]
        public string VNPM06 { get; set; }
        [DataMember(Order = 171)]
        public string VNPM07 { get; set; }
        [DataMember(Order = 172)]
        public string VNPM08 { get; set; }
        [DataMember(Order = 173)]
        public string VNPM09 { get; set; }
        [DataMember(Order = 174)]
        public string VNPM10 { get; set; }
        [DataMember(Order = 175)]
        public string VNBCRC { get; set; }
        [DataMember(Order = 176)]
        public string VNEXR1 { get; set; }
        [DataMember(Order = 177)]
        public string VNTXA1 { get; set; }
        [DataMember(Order = 178)]
        public decimal VNTXITM { get; set; }
        [DataMember(Order = 179)]
        public string VNACTB { get; set; }
        [DataMember(Order = 180)]
        public decimal VNSTAM { get; set; }
        [DataMember(Order = 181)]
        public decimal VNCTAM { get; set; }
        [DataMember(Order = 182)]
        public decimal VNAG { get; set; }
        [DataMember(Order = 183)]
        public decimal VNAGF { get; set; }
        [DataMember(Order = 184)]
        public string VNTKTX { get; set; }
        [DataMember(Order = 185)]
        public decimal VNDLNID { get; set; }
        [DataMember(Order = 186)]
        public string VNCKNU { get; set; }
        [DataMember(Order = 187)]
        public string VNBUPC { get; set; }
        [DataMember(Order = 188)]
        public string VNAHBU { get; set; }
        [DataMember(Order = 189)]
        public string VNEPGC { get; set; }
        [DataMember(Order = 190)]
        public string VNJPGC { get; set; }
        [DataMember(Order = 191)]
        public decimal VNRC5 { get; set; }
        [DataMember(Order = 192)]
        public decimal VNSFXE { get; set; }
        [DataMember(Order = 193)]
        public string VNOFM { get; set; }


    }
}
