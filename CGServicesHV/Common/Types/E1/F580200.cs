using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{

    [CollectionDataContract(Namespace = "")]
    public class F580200s : List<F580200>
    {
    }

    [DataContract(Namespace = "")]
    public class F580200
    {
        public F580200()
        {
            BIEDUS = "";
            BIEDBT = "";
            BIEDTN = "";
            BIEDLN = 0;
            BIEDDT = 0;
            BIEDSP = "";
            BIDOCO = 0;
            BIDCTO = "";
            BILNID = 0;
            BIZ58UNID = "";
            BIZ58BXCR = 0;
            BIZ58SNUM = "";
            BIZ58RT = "";
            BIZ58BOX = 0;
            BIZ58BXRO = "";
            BIZ58SD = "";
            BIZ58STK = 0;
            BIZ58ROW = 0;
            BIZ58SL = 0;
            BIZ58IGLN = "";
            BIZ58SCN = 0;
            BIZ58RNSQ = 0;
            BIZ58ONSK = "";
            BIZ58L1R = "";
            BIZ58L1SLN = 0;
            BIZ58L2R = "";
            BIZ58L2SLN = 0;
            BIZ58L3SLN = 0;
            BIZ58L3R = "";
            BIURCD = "";
            BIURDT = 0;
            BIURAT = 0;
            BIURAB = 0;
            BIURRF = "";
            BIUSER = "";
            BIPID = "";
            BIJOBN = "";
            BIUPMJ = 0;
            BITDAY = 0;
            BIZ58RID = 0;
            BIZ58SCHDJ = 0;
            BIMCU = "";
            BIKCOO = "";
            BIAN8 = 0;
            BISHAN = 0;
            BIITM = 0;
            BILITM = "";
            BIAITM = "";
            BIRKCO = "";
            BITRDJ = 0;
            BIRORN = "";
            BIRCTO = "";
            BIRLLN = 0;
            BIITWT = 0;
            BIWTUM = "";
            BITRQTY = 0;
            BIUM = "";
            BICFGID = 0;
            BIUKID = 0;
        }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string BIEDUS { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string BIEDBT { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string BIEDTN { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public decimal BIEDLN { get; set; }
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public decimal BIEDDT { get; set; }
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string BIEDSP { get; set; }
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public decimal BIDOCO { get; set; }
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public string BIDCTO { get; set; }
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public decimal BILNID { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string BIZ58UNID { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public decimal BIZ58BXCR { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string BIZ58SNUM { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string BIZ58RT { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public decimal BIZ58BOX { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string BIZ58BXRO { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string BIZ58SD { get; set; }
        [DataMember(Order = 17, EmitDefaultValue = false)]
        public decimal BIZ58STK { get; set; }
        [DataMember(Order = 18, EmitDefaultValue = false)]
        public decimal BIZ58ROW { get; set; }
        [DataMember(Order = 19, EmitDefaultValue = false)]
        public decimal BIZ58SL { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string BIZ58IGLN { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public decimal BIZ58SCN { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public decimal BIZ58RNSQ { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public string BIZ58ONSK { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string BIZ58L1R { get; set; }
        [DataMember(Order = 25, EmitDefaultValue = false)]
        public decimal BIZ58L1SLN { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string BIZ58L2R { get; set; }
        [DataMember(Order = 27, EmitDefaultValue = false)]
        public decimal BIZ58L2SLN { get; set; }
        [DataMember(Order = 28, EmitDefaultValue = false)]
        public decimal BIZ58L3SLN { get; set; }
        [DataMember(Order = 29, EmitDefaultValue = false)]
        public string BIZ58L3R { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string BIURCD { get; set; }
        [DataMember(Order = 31, EmitDefaultValue = false)]
        public decimal BIURDT { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public decimal BIURAT { get; set; }
        [DataMember(Order = 33, EmitDefaultValue = false)]
        public decimal BIURAB { get; set; }
        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string BIURRF { get; set; }
        [DataMember(Order = 35, EmitDefaultValue = false)]
        public string BIUSER { get; set; }
        [DataMember(Order = 36, EmitDefaultValue = false)]
        public string BIPID { get; set; }
        [DataMember(Order = 37, EmitDefaultValue = false)]
        public string BIJOBN { get; set; }
        [DataMember(Order = 38, EmitDefaultValue = false)]
        public decimal BIUPMJ { get; set; }
        [DataMember(Order = 39, EmitDefaultValue = false)]
        public decimal BITDAY { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal BIZ58RID { get; set; }
        [DataMember(Order = 41, EmitDefaultValue = false)]
        public decimal BIZ58SCHDJ { get; set; }
        [DataMember(Order = 42, EmitDefaultValue = false)]
        public string BIMCU { get; set; }
        [DataMember(Order = 43, EmitDefaultValue = false)]
        public string BIKCOO { get; set; }
        [DataMember(Order = 44, EmitDefaultValue = false)]
        public decimal BIAN8 { get; set; }
        [DataMember(Order = 45, EmitDefaultValue = false)]
        public decimal BISHAN { get; set; }
        [DataMember(Order = 46, EmitDefaultValue = false)]
        public decimal BIITM { get; set; }
        [DataMember(Order = 47, EmitDefaultValue = false)]
        public string BILITM { get; set; }
        [DataMember(Order = 48, EmitDefaultValue = false)]
        public string BIAITM { get; set; }
        [DataMember(Order = 49, EmitDefaultValue = false)]
        public string BIRKCO { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal BITRDJ { get; set; }
        [DataMember(Order = 51, EmitDefaultValue = false)]
        public string BIRORN { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public string BIRCTO { get; set; }
        [DataMember(Order = 53, EmitDefaultValue = false)]
        public decimal BIRLLN { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public decimal BIITWT { get; set; }
        [DataMember(Order = 55, EmitDefaultValue = false)]
        public string BIWTUM { get; set; }
        [DataMember(Order = 56, EmitDefaultValue = false)]
        public decimal BITRQTY { get; set; }
        [DataMember(Order = 57, EmitDefaultValue = false)]
        public string BIUM { get; set; }
        [DataMember(Order = 58, EmitDefaultValue = false)]
        public decimal BICFGID { get; set; }
        [DataMember(Order = 59, EmitDefaultValue = false)]
        public decimal BIUKID { get; set; }

    }

}
