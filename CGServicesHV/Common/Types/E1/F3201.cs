using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable once InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    public class F3201s : List<F3201>
    {
    }

    [DataContract(Namespace = "")]
    public class F3201
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal CMCFGID { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string CMKCOO { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal CMDOCO { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string CMDCTO { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string CMSFXO { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal CMLNID { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal CMCFGCID { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string CMSY { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public decimal CMAN8 { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal CMSHAN { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string CMEMCU { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string CMCO { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public string CMCRRM { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string CMCRCD { get; set; }
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string CMCRDC { get; set; }
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string CMURCD { get; set; }
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public decimal CMURDT { get; set; }
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public decimal CMURAT { get; set; }
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public decimal CMURAB { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public string CMURRF { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public string CMUSER { get; set; }
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public string CMPID { get; set; }
        [DataMember(Order = 230, EmitDefaultValue = false)]
        public string CMJOBN { get; set; }
        [DataMember(Order = 240, EmitDefaultValue = false)]
        public decimal CMUPMJ { get; set; }
        [DataMember(Order = 250, EmitDefaultValue = false)]
        public decimal CMTDAY { get; set; }
    }
}
