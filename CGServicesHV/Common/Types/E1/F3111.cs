using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable once InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    public class F3111s : List<F3111>
    {
    }

    public class F3111
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal WMDOCO { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string WMFORQ { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal WMCPIT { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string WMCMCU { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string WMDSC1 { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string WMDSC2 { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal WMUORG { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string WMCPIL { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string WMCPIA { get; set; }
    }
}
