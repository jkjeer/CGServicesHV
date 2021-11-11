using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class RawGlassItems : List<RawGlassItem>
    {
    }

    [DataContract(Namespace = "")]
    public class RawGlassItem
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string WMCPIT { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string IBMCU { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string IBGLPT { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal WMDOCO { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string WMDCTO { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string WMDSC1 { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string WMDSC2 { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string WMLOCN { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public decimal WMUORG { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal WMTRQT { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal CRZ58L1TK { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string CRZ58L1HT { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public string CRZ58L1CL { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string CRZ58L11C { get; set; }
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string CRZ58L12C { get; set; }
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string CRZ58L1PX { get; set; }
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public decimal CRZ58L1BDM { get; set; }
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public decimal CRZ58L1BNM { get; set; }
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public decimal CRZ58L1BDN { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal CRZ58L1LDM { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public decimal CRZ58L1LNM { get; set; }
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public decimal CRZ58L1LDN { get; set; }
        [DataMember(Order = 230, EmitDefaultValue = false)]
        public decimal CRZ58L1RDM { get; set; }
        [DataMember(Order = 240, EmitDefaultValue = false)]
        public decimal CRZ58L1RNM { get; set; }
        [DataMember(Order = 250, EmitDefaultValue = false)]
        public decimal CRZ58L1RDN { get; set; }
        [DataMember(Order = 260, EmitDefaultValue = false)]
        public decimal CRZ58L1TDM { get; set; }
        [DataMember(Order = 270, EmitDefaultValue = false)]
        public decimal CRZ58L1TNM { get; set; }
        [DataMember(Order = 280, EmitDefaultValue = false)]
        public decimal CRZ58L1TDN { get; set; }

        [DataMember(Order = 290, EmitDefaultValue = false)]
        public string CRZ58SHAPE { get; set; }
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string S7EDBT { get; set; }
        [DataMember(Order = 310, EmitDefaultValue = false)]
        public decimal S7ITM { get; set; }
        [DataMember(Order = 320, EmitDefaultValue = false)]
        public decimal S7Z58SCN { get; set; }
        [DataMember(Order = 330, EmitDefaultValue = false)]
        public decimal WMOPSQ { get; set; }
    }
}
