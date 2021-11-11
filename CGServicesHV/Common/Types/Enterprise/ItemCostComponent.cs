using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ItemCostComponents : List<ItemCostComponent>
    {
    }

    [DataContract(Namespace = "")]
    public class ItemCostComponent
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string LedgerType { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string LedgerTypeDesc { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string CostType { get; set; }
        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string CostTypeDesc { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal StandardCostRollup { get; set; }
    }
}
