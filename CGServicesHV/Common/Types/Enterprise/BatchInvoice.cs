using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class BatchInvoices : List<BatchInvoice>
    {
    }

    [DataContract(Namespace = "")]
    public class BatchInvoice
    {
        public BatchInvoice()
        {
            VJEDUS = "";
            VJEDTY = "";
            VJEDSQ = 0;
            VJEDTN = "";
            VJEDCT = "";
            VJEDLN = 0;
            VJEDTS = "";
            VJEDFT = "";
            VJEDDT = 0;
            VJEDER = "";
            VJEDDL = 0;
            VJEDSP = "";
            VJEDTC = "";
            VJEDTR = "";
            VJEDBT = "";
            VJEDGL = "";
            VJEDDH = "";
            VJEDAN = 0;
            VJDOC = 0;
            VJDCT = "";
            VJKCO = "";
            VJSFX = "";
            VJAN8 = 0;
            VJDGJ = 0;
            VJDIVJ = 0;
            VJICUT = "";
            VJICU = 0;
            VJDICJ = 0;
            VJFY = 0;
            VJCTRY = 0;
            VJPN = 0;
            VJCO = "";
            VJGLC = "";
            VJAID = "";
            VJPA8 = 0;
            VJAN8J = 0;
            VJPYR = 0;
            VJPOST = "";
            VJISTR = "";
            VJBALJ = "";
            VJPST = "";
            VJAG = 0;
            VJAAP = 0;
            VJADSC = 0;
            VJADSA = 0;
            VJATXA = 0;
            VJATXN = 0;
            VJSTAM = 0;
            VJCRRM = "";
            VJCRCD = "";
            VJCRR = 0;
            VJDMCD = "";
            VJACR = 0;
            VJFAP = 0;
            VJCDS = 0;
            VJCDSA = 0;
            VJCTXA = 0;
            VJCTXN = 0;
            VJCTAM = 0;
            VJTXA1 = "";
            VJEXR1 = "";
            VJDSVJ = 0;
            VJGLBA = "";
            VJAM = "";
            VJAID2 = "";
            VJAM2 = "";
            VJMCU = "";
            VJOBJ = "";
            VJSUB = "";
            VJSBLT = "";
            VJSBL = "";
            VJPTC = "";
            VJDDJ = 0;
            VJDDNJ = 0;
            VJRDDJ = 0;
            VJRDSJ = 0;
            VJSMTJ = 0;
            VJNBRR = "";
            VJRDRL = "";
            VJRMDS = 0;
            VJCOLL = "";
            VJCORC = "";
            VJAFC = "";
            VJRSCO = "";
            VJCKNU = "";
            VJODOC = 0;
            VJODCT = "";
            VJOKCO = "";
            VJOSFX = "";
            VJVINV = "";
            VJPO = "";
            VJPDCT = "";
            VJPKCO = "";
            VJDCTO = "";
            VJLNID = 0;
            VJSDOC = 0;
            VJSDCT = "";
            VJSKCO = "";
            VJSFXO = "";
            VJVLDT = 0;
            VJCMC1 = 0;
            VJVR01 = "";
            VJUNIT = "";
            VJMCU2 = "";
            VJRMK = "";
            VJALPH = "";
            VJRF = "";
            VJDRF = 0;
            VJCTL = "";
            VJFNLP = "";
            VJITM = 0;
            VJU = 0;
            VJUM = "";
            VJALT6 = "";
            VJRYIN = "";
            VJVDGJ = 0;
            VJVRE = "";
            VJRP1 = "";
            VJRP2 = "";
            VJRP3 = "";
            VJAR01 = "";
            VJAR02 = "";
            VJAR03 = "";
            VJAR04 = "";
            VJAR05 = "";
            VJAR06 = "";
            VJAR07 = "";
            VJAR08 = "";
            VJAR09 = "";
            VJAR10 = "";
            VJURC1 = "";
            VJURDT = 0;
            VJURAT = 0;
            VJURAB = 0;
            VJURRF = "";
            VJTORG = "";
            VJUSER = "";
            VJPID = "";
            VJUPMJ = 0;
            VJUPMT = 0;
            VJJOBN = "";
            VJHCRR = 0;
            VJHDGJ = 0;
            VJDIM = 0;
            VJDID = 0;
            VJDIY = 0;
            VJDI_PS = 0;
            VJDGM = 0;
            VJDGD = 0;
            VJDGY = 0;
            VJDG_PS = 0;
            VJDICM = 0;
            VJDICD = 0;
            VJDICY = 0;
            VJDIC_PS = 0;
            VJDSVM = 0;
            VJDSVD = 0;
            VJDSVY = 0;
            VJDSV_PS = 0;
            VJDDM = 0;
            VJDDD = 0;
            VJDDY = 0;
            VJDD_PS = 0;
            VJDDNM = 0;
            VJDDND = 0;
            VJDDNY = 0;
            VJDDN_PS = 0;
            VJSMTM = 0;
            VJSMTD = 0;
            VJSMTY = 0;
            VJSMT_PS = 0;
            VJRDDM = 0;
            VJRDDD = 0;
            VJRDDY = 0;
            VJRDD_PS = 0;
            VJRDSM = 0;
            VJRDSD = 0;
            VJRDSY = 0;
            VJRDS_PS = 0;
            VJHDGM = 0;
            VJHDGD = 0;
            VJHDGY = 0;
            VJHDG_PS = 0;
            VJSHPN = 0;
            VJDTXS = "";
            VJOMOD = "";
            VJCLMG = "";
            VJCMGR = "";
            VJATAD = 0;
            VJCTAD = 0;
            VJNRTA = 0;
            VJFNRT = 0;
            VJPRGF = "";
            VJGFL1 = "";
            VJGFL2 = "";
            VJDOCO = 0;
            VJKCOO = "";
            VJSOTF = "";
            VJDTPB = 0;
            VJERDJ = 0;
            VJNETST = "";
            VJRMR1 = "";
        }

        [DataMember(Order = 1)]
        public string VJEDUS { get; set; }
        [DataMember(Order = 2)]
        public string VJEDTY { get; set; }
        [DataMember(Order = 3)]
        public decimal VJEDSQ { get; set; }
        [DataMember(Order = 4)]
        public string VJEDTN { get; set; }
        [DataMember(Order = 5)]
        public string VJEDCT { get; set; }
        [DataMember(Order = 6)]
        public decimal VJEDLN { get; set; }
        [DataMember(Order = 7)]
        public string VJEDTS { get; set; }
        [DataMember(Order = 8)]
        public string VJEDFT { get; set; }
        [DataMember(Order = 9)]
        public decimal VJEDDT { get; set; }
        [DataMember(Order = 10)]
        public string VJEDER { get; set; }
        [DataMember(Order = 11)]
        public decimal VJEDDL { get; set; }
        [DataMember(Order = 12)]
        public string VJEDSP { get; set; }
        [DataMember(Order = 13)]
        public string VJEDTC { get; set; }
        [DataMember(Order = 14)]
        public string VJEDTR { get; set; }
        [DataMember(Order = 15)]
        public string VJEDBT { get; set; }
        [DataMember(Order = 16)]
        public string VJEDGL { get; set; }
        [DataMember(Order = 17)]
        public string VJEDDH { get; set; }
        [DataMember(Order = 18)]
        public decimal VJEDAN { get; set; }
        [DataMember(Order = 19)]
        public decimal VJDOC { get; set; }
        [DataMember(Order = 20)]
        public string VJDCT { get; set; }
        [DataMember(Order = 21)]
        public string VJKCO { get; set; }
        [DataMember(Order = 22)]
        public string VJSFX { get; set; }
        [DataMember(Order = 23)]
        public decimal VJAN8 { get; set; }
        [DataMember(Order = 24)]
        public decimal VJDGJ { get; set; }
        [DataMember(Order = 25)]
        public decimal VJDIVJ { get; set; }
        [DataMember(Order = 26)]
        public string VJICUT { get; set; }
        [DataMember(Order = 27)]
        public decimal VJICU { get; set; }
        [DataMember(Order = 28)]
        public decimal VJDICJ { get; set; }
        [DataMember(Order = 29)]
        public decimal VJFY { get; set; }
        [DataMember(Order = 30)]
        public decimal VJCTRY { get; set; }
        [DataMember(Order = 31)]
        public decimal VJPN { get; set; }
        [DataMember(Order = 32)]
        public string VJCO { get; set; }
        [DataMember(Order = 33)]
        public string VJGLC { get; set; }
        [DataMember(Order = 34)]
        public string VJAID { get; set; }
        [DataMember(Order = 35)]
        public decimal VJPA8 { get; set; }
        [DataMember(Order = 36)]
        public decimal VJAN8J { get; set; }
        [DataMember(Order = 37)]
        public decimal VJPYR { get; set; }
        [DataMember(Order = 38)]
        public string VJPOST { get; set; }
        [DataMember(Order = 39)]
        public string VJISTR { get; set; }
        [DataMember(Order = 40)]
        public string VJBALJ { get; set; }
        [DataMember(Order = 41)]
        public string VJPST { get; set; }
        [DataMember(Order = 42)]
        public decimal VJAG { get; set; }
        [DataMember(Order = 43)]
        public decimal VJAAP { get; set; }
        [DataMember(Order = 44)]
        public decimal VJADSC { get; set; }
        [DataMember(Order = 45)]
        public decimal VJADSA { get; set; }
        [DataMember(Order = 46)]
        public decimal VJATXA { get; set; }
        [DataMember(Order = 47)]
        public decimal VJATXN { get; set; }
        [DataMember(Order = 48)]
        public decimal VJSTAM { get; set; }
        [DataMember(Order = 49)]
        public string VJCRRM { get; set; }
        [DataMember(Order = 50)]
        public string VJCRCD { get; set; }
        [DataMember(Order = 51)]
        public decimal VJCRR { get; set; }
        [DataMember(Order = 52)]
        public string VJDMCD { get; set; }
        [DataMember(Order = 53)]
        public decimal VJACR { get; set; }
        [DataMember(Order = 54)]
        public decimal VJFAP { get; set; }
        [DataMember(Order = 55)]
        public decimal VJCDS { get; set; }
        [DataMember(Order = 56)]
        public decimal VJCDSA { get; set; }
        [DataMember(Order = 57)]
        public decimal VJCTXA { get; set; }
        [DataMember(Order = 58)]
        public decimal VJCTXN { get; set; }
        [DataMember(Order = 59)]
        public decimal VJCTAM { get; set; }
        [DataMember(Order = 60)]
        public string VJTXA1 { get; set; }
        [DataMember(Order = 61)]
        public string VJEXR1 { get; set; }
        [DataMember(Order = 62)]
        public decimal VJDSVJ { get; set; }
        [DataMember(Order = 63)]
        public string VJGLBA { get; set; }
        [DataMember(Order = 64)]
        public string VJAM { get; set; }
        [DataMember(Order = 65)]
        public string VJAID2 { get; set; }
        [DataMember(Order = 66)]
        public string VJAM2 { get; set; }
        [DataMember(Order = 67)]
        public string VJMCU { get; set; }
        [DataMember(Order = 68)]
        public string VJOBJ { get; set; }
        [DataMember(Order = 69)]
        public string VJSUB { get; set; }
        [DataMember(Order = 70)]
        public string VJSBLT { get; set; }
        [DataMember(Order = 71)]
        public string VJSBL { get; set; }
        [DataMember(Order = 72)]
        public string VJPTC { get; set; }
        [DataMember(Order = 73)]
        public decimal VJDDJ { get; set; }
        [DataMember(Order = 74)]
        public decimal VJDDNJ { get; set; }
        [DataMember(Order = 75)]
        public decimal VJRDDJ { get; set; }
        [DataMember(Order = 76)]
        public decimal VJRDSJ { get; set; }
        [DataMember(Order = 77)]
        public decimal VJSMTJ { get; set; }
        [DataMember(Order = 78)]
        public string VJNBRR { get; set; }
        [DataMember(Order = 79)]
        public string VJRDRL { get; set; }
        [DataMember(Order = 80)]
        public decimal VJRMDS { get; set; }
        [DataMember(Order = 81)]
        public string VJCOLL { get; set; }
        [DataMember(Order = 82)]
        public string VJCORC { get; set; }
        [DataMember(Order = 83)]
        public string VJAFC { get; set; }
        [DataMember(Order = 84)]
        public string VJRSCO { get; set; }
        [DataMember(Order = 85)]
        public string VJCKNU { get; set; }
        [DataMember(Order = 86)]
        public decimal VJODOC { get; set; }
        [DataMember(Order = 87)]
        public string VJODCT { get; set; }
        [DataMember(Order = 88)]
        public string VJOKCO { get; set; }
        [DataMember(Order = 89)]
        public string VJOSFX { get; set; }
        [DataMember(Order = 90)]
        public string VJVINV { get; set; }
        [DataMember(Order = 91)]
        public string VJPO { get; set; }
        [DataMember(Order = 92)]
        public string VJPDCT { get; set; }
        [DataMember(Order = 93)]
        public string VJPKCO { get; set; }
        [DataMember(Order = 94)]
        public string VJDCTO { get; set; }
        [DataMember(Order = 95)]
        public decimal VJLNID { get; set; }
        [DataMember(Order = 96)]
        public decimal VJSDOC { get; set; }
        [DataMember(Order = 97)]
        public string VJSDCT { get; set; }
        [DataMember(Order = 98)]
        public string VJSKCO { get; set; }
        [DataMember(Order = 99)]
        public string VJSFXO { get; set; }
        [DataMember(Order = 100)]
        public decimal VJVLDT { get; set; }
        [DataMember(Order = 101)]
        public decimal VJCMC1 { get; set; }
        [DataMember(Order = 102)]
        public string VJVR01 { get; set; }
        [DataMember(Order = 103)]
        public string VJUNIT { get; set; }
        [DataMember(Order = 104)]
        public string VJMCU2 { get; set; }
        [DataMember(Order = 105)]
        public string VJRMK { get; set; }
        [DataMember(Order = 106)]
        public string VJALPH { get; set; }
        [DataMember(Order = 107)]
        public string VJRF { get; set; }
        [DataMember(Order = 108)]
        public decimal VJDRF { get; set; }
        [DataMember(Order = 109)]
        public string VJCTL { get; set; }
        [DataMember(Order = 110)]
        public string VJFNLP { get; set; }
        [DataMember(Order = 111)]
        public decimal VJITM { get; set; }
        [DataMember(Order = 112)]
        public decimal VJU { get; set; }
        [DataMember(Order = 113)]
        public string VJUM { get; set; }
        [DataMember(Order = 114)]
        public string VJALT6 { get; set; }
        [DataMember(Order = 115)]
        public string VJRYIN { get; set; }
        [DataMember(Order = 116)]
        public decimal VJVDGJ { get; set; }
        [DataMember(Order = 117)]
        public string VJVRE { get; set; }
        [DataMember(Order = 118)]
        public string VJRP1 { get; set; }
        [DataMember(Order = 119)]
        public string VJRP2 { get; set; }
        [DataMember(Order = 120)]
        public string VJRP3 { get; set; }
        [DataMember(Order = 121)]
        public string VJAR01 { get; set; }
        [DataMember(Order = 122)]
        public string VJAR02 { get; set; }
        [DataMember(Order = 123)]
        public string VJAR03 { get; set; }
        [DataMember(Order = 124)]
        public string VJAR04 { get; set; }
        [DataMember(Order = 125)]
        public string VJAR05 { get; set; }
        [DataMember(Order = 126)]
        public string VJAR06 { get; set; }
        [DataMember(Order = 127)]
        public string VJAR07 { get; set; }
        [DataMember(Order = 128)]
        public string VJAR08 { get; set; }
        [DataMember(Order = 129)]
        public string VJAR09 { get; set; }
        [DataMember(Order = 130)]
        public string VJAR10 { get; set; }
        [DataMember(Order = 131)]
        public string VJURC1 { get; set; }
        [DataMember(Order = 132)]
        public decimal VJURDT { get; set; }
        [DataMember(Order = 133)]
        public decimal VJURAT { get; set; }
        [DataMember(Order = 134)]
        public decimal VJURAB { get; set; }
        [DataMember(Order = 135)]
        public string VJURRF { get; set; }
        [DataMember(Order = 136)]
        public string VJTORG { get; set; }
        [DataMember(Order = 137)]
        public string VJUSER { get; set; }
        [DataMember(Order = 138)]
        public string VJPID { get; set; }
        [DataMember(Order = 139)]
        public decimal VJUPMJ { get; set; }
        [DataMember(Order = 140)]
        public decimal VJUPMT { get; set; }
        [DataMember(Order = 141)]
        public string VJJOBN { get; set; }
        [DataMember(Order = 142)]
        public decimal VJHCRR { get; set; }
        [DataMember(Order = 143)]
        public decimal VJHDGJ { get; set; }
        [DataMember(Order = 144)]
        public decimal VJDIM { get; set; }
        [DataMember(Order = 145)]
        public decimal VJDID { get; set; }
        [DataMember(Order = 146)]
        public decimal VJDIY { get; set; }
        [DataMember(Order = 147)]
        public decimal VJDI_PS { get; set; }
        [DataMember(Order = 148)]
        public decimal VJDGM { get; set; }
        [DataMember(Order = 149)]
        public decimal VJDGD { get; set; }
        [DataMember(Order = 150)]
        public decimal VJDGY { get; set; }
        [DataMember(Order = 151)]
        public decimal VJDG_PS { get; set; }
        [DataMember(Order = 152)]
        public decimal VJDICM { get; set; }
        [DataMember(Order = 153)]
        public decimal VJDICD { get; set; }
        [DataMember(Order = 154)]
        public decimal VJDICY { get; set; }
        [DataMember(Order = 155)]
        public decimal VJDIC_PS { get; set; }
        [DataMember(Order = 156)]
        public decimal VJDSVM { get; set; }
        [DataMember(Order = 157)]
        public decimal VJDSVD { get; set; }
        [DataMember(Order = 158)]
        public decimal VJDSVY { get; set; }
        [DataMember(Order = 159)]
        public decimal VJDSV_PS { get; set; }
        [DataMember(Order = 160)]
        public decimal VJDDM { get; set; }
        [DataMember(Order = 161)]
        public decimal VJDDD { get; set; }
        [DataMember(Order = 162)]
        public decimal VJDDY { get; set; }
        [DataMember(Order = 163)]
        public decimal VJDD_PS { get; set; }
        [DataMember(Order = 164)]
        public decimal VJDDNM { get; set; }
        [DataMember(Order = 165)]
        public decimal VJDDND { get; set; }
        [DataMember(Order = 166)]
        public decimal VJDDNY { get; set; }
        [DataMember(Order = 167)]
        public decimal VJDDN_PS { get; set; }
        [DataMember(Order = 168)]
        public decimal VJSMTM { get; set; }
        [DataMember(Order = 169)]
        public decimal VJSMTD { get; set; }
        [DataMember(Order = 170)]
        public decimal VJSMTY { get; set; }
        [DataMember(Order = 171)]
        public decimal VJSMT_PS { get; set; }
        [DataMember(Order = 172)]
        public decimal VJRDDM { get; set; }
        [DataMember(Order = 173)]
        public decimal VJRDDD { get; set; }
        [DataMember(Order = 174)]
        public decimal VJRDDY { get; set; }
        [DataMember(Order = 175)]
        public decimal VJRDD_PS { get; set; }
        [DataMember(Order = 176)]
        public decimal VJRDSM { get; set; }
        [DataMember(Order = 177)]
        public decimal VJRDSD { get; set; }
        [DataMember(Order = 178)]
        public decimal VJRDSY { get; set; }
        [DataMember(Order = 179)]
        public decimal VJRDS_PS { get; set; }
        [DataMember(Order = 180)]
        public decimal VJHDGM { get; set; }
        [DataMember(Order = 181)]
        public decimal VJHDGD { get; set; }
        [DataMember(Order = 182)]
        public decimal VJHDGY { get; set; }
        [DataMember(Order = 183)]
        public decimal VJHDG_PS { get; set; }
        [DataMember(Order = 184)]
        public decimal VJSHPN { get; set; }
        [DataMember(Order = 185)]
        public string VJDTXS { get; set; }
        [DataMember(Order = 186)]
        public string VJOMOD { get; set; }
        [DataMember(Order = 187)]
        public string VJCLMG { get; set; }
        [DataMember(Order = 188)]
        public string VJCMGR { get; set; }
        [DataMember(Order = 189)]
        public decimal VJATAD { get; set; }
        [DataMember(Order = 190)]
        public decimal VJCTAD { get; set; }
        [DataMember(Order = 191)]
        public decimal VJNRTA { get; set; }
        [DataMember(Order = 192)]
        public decimal VJFNRT { get; set; }
        [DataMember(Order = 193)]
        public string VJPRGF { get; set; }
        [DataMember(Order = 194)]
        public string VJGFL1 { get; set; }
        [DataMember(Order = 195)]
        public string VJGFL2 { get; set; }
        [DataMember(Order = 196)]
        public decimal VJDOCO { get; set; }
        [DataMember(Order = 197)]
        public string VJKCOO { get; set; }
        [DataMember(Order = 198)]
        public string VJSOTF { get; set; }
        [DataMember(Order = 199)]
        public decimal VJDTPB { get; set; }
        [DataMember(Order = 200)]
        public decimal VJERDJ { get; set; }
        [DataMember(Order = 201)]
        public string VJNETST { get; set; }
        [DataMember(Order = 202)]
        public string VJRMR1 { get; set; }
    }
}
