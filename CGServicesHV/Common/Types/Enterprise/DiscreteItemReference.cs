using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class DiscreteItemReference
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string ReferenceType { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string CustomerItemNum { get; set; }
        [DataMember(Order = 25, EmitDefaultValue = false)]
        public string Description1 { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Description2 { get; set; }
    }
}
