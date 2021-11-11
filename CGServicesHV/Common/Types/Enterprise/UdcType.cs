using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class UdcTypes : List<UdcType>
    {
    }

    [DataContract(Namespace = "")]
    public class UdcType
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string ProductCode { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string UserCode { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string Description { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public decimal CodeLength { get; set; }
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string Numeric { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public DateTime UpdateDateTime { get; set; }
    }
}
