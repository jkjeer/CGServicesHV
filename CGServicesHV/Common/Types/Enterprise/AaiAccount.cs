using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class AaiAccount
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal TableNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Company { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string OrderType { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string DocumentType { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string GlCategory { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string CostType { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string CostCenter { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string ObjectAcct { get; set; }

        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string Subsidiary { get; set; }
    }
}
