using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ItemComponents : List<ItemComponent>
    {
    }

    [DataContract(Namespace = "")]
    public class ItemComponent
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public Item Item { get; set; }
        //[DataMember(Order = 10, EmitDefaultValue = false)]
        //public decimal ItemNum { get; set; }
        //[DataMember(Order = 15, EmitDefaultValue = false)]
        //public string ItemDesc { get; set; }
        //[DataMember(Order = 16, EmitDefaultValue = false)]
        //public string ItemCategory { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string UnitMeasure { get; set; }
        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string UnitMeasureInput { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal OperationSeqNum { get; set; }
    }
}
