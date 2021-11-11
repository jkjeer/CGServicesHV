using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class SpeedRecvItems : List<SpeedRecvItem>
    {
    }

    [DataContract(Namespace = "")]
    public class SpeedRecvItem
    {
        public SpeedRecvItem()
        {
            SRZ58ASN = "";
            SREDLN = 0;
            SREDSP = "";
            SRDSE1 = "";
            SREDOC = 0;
            SREDCT = "";
            SRMCU = "";
            SRZ58RID = 0;
            SRZ58SN = "";
            SRLOT1 = "";
            SRZ58RT = "";
            SRITM = 0;
            SRLITM = "";
            SRDSC1 = "";
            SRDSC2 = "";
            SRPQOH = 0;
            SRUOM = "";
            SRDOCO = 0;
            SRDCTO = "";
            SRKCOO = "";
            SRSFXO = "";
            SRLNID = 0;
            SRLOTN = "";
            SRLOCN = "";
            SRZ58SD = "";
            SRZ58PS = 0;
            SRZ58STK = 0;
            SRZ58ROW = 0;
            SRZ58SL = 0;
            SRMMCU = "";
            SRLODN = 0;
            SRBOLN = 0;
            SRRECDATE = 0;
            SRURCD = "";
            SRURDT = 0;
            SRURAT = 0;
            SRURAB = 0;
            SRURRF = "";
            SRUSER = "";
            SRPID = "";
            SRJOBN = "";
            SRUPMJ = 0;
            SRUPMT = 0;
        }

        [DataMember(Order = 1)]
        public string SRZ58ASN { get; set; }
        [DataMember(Order = 2)]
        public decimal SREDLN { get; set; }
        [DataMember(Order = 3)]
        public string SREDSP { get; set; }
        [DataMember(Order = 4)]
        public string SRDSE1 { get; set; }
        [DataMember(Order = 5)]
        public decimal SREDOC { get; set; }
        [DataMember(Order = 6)]
        public string SREDCT { get; set; }
        [DataMember(Order = 7)]
        public string SRMCU { get; set; }
        [DataMember(Order = 8)]
        public decimal SRZ58RID { get; set; }
        [DataMember(Order = 9)]
        public string SRZ58SN { get; set; }
        [DataMember(Order = 10)]
        public string SRLOT1 { get; set; }
        [DataMember(Order = 11)]
        public string SRZ58RT { get; set; }
        [DataMember(Order = 12)]
        public decimal SRITM { get; set; }
        [DataMember(Order = 13)]
        public string SRLITM { get; set; }
        [DataMember(Order = 14)]
        public string SRDSC1 { get; set; }
        [DataMember(Order = 15)]
        public string SRDSC2 { get; set; }
        [DataMember(Order = 16)]
        public decimal SRPQOH { get; set; }
        [DataMember(Order = 17)]
        public string SRUOM { get; set; }
        [DataMember(Order = 18)]
        public decimal SRDOCO { get; set; }
        [DataMember(Order = 19)]
        public string SRDCTO { get; set; }
        [DataMember(Order = 20)]
        public string SRKCOO { get; set; }
        [DataMember(Order = 21)]
        public string SRSFXO { get; set; }
        [DataMember(Order = 22)]
        public decimal SRLNID { get; set; }
        [DataMember(Order = 23)]
        public string SRLOTN { get; set; }
        [DataMember(Order = 24)]
        public string SRLOCN { get; set; }
        [DataMember(Order = 25)]
        public string SRZ58SD { get; set; }
        [DataMember(Order = 26)]
        public decimal SRZ58PS { get; set; }
        [DataMember(Order = 27)]
        public decimal SRZ58STK { get; set; }
        [DataMember(Order = 28)]
        public decimal SRZ58ROW { get; set; }
        [DataMember(Order = 29)]
        public decimal SRZ58SL { get; set; }
        [DataMember(Order = 30)]
        public string SRMMCU { get; set; }
        [DataMember(Order = 31)]
        public decimal SRLODN { get; set; }
        [DataMember(Order = 32)]
        public decimal SRBOLN { get; set; }
        [DataMember(Order = 33)]
        public decimal SRRECDATE { get; set; }
        [DataMember(Order = 34)]
        public string SRURCD { get; set; }
        [DataMember(Order = 35)]
        public decimal SRURDT { get; set; }
        [DataMember(Order = 36)]
        public decimal SRURAT { get; set; }
        [DataMember(Order = 37)]
        public decimal SRURAB { get; set; }
        [DataMember(Order = 38)]
        public string SRURRF { get; set; }
        [DataMember(Order = 39)]
        public string SRUSER { get; set; }
        [DataMember(Order = 40)]
        public string SRPID { get; set; }
        [DataMember(Order = 41)]
        public string SRJOBN { get; set; }
        [DataMember(Order = 42)]
        public decimal SRUPMJ { get; set; }
        [DataMember(Order = 43)]
        public decimal SRUPMT { get; set; }
    }
}
