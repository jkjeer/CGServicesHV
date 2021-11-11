using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ItemRoutings : List<ItemRouting>
    {
    }

    [DataContract(Namespace = "")]
    public class ItemRouting
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string CostCenter { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string Description { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal OperationSeqNum { get; set; }
    }
}
