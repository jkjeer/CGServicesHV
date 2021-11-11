using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class WorkOrderComponents : List<WorkOrderComponent>
    {
    }

    [DataContract(Namespace = "")]
    public class WorkOrderComponent : BaseItem
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal SequenceNum { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string Location { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 71, EmitDefaultValue = false)]
        public decimal InventoryQuantity { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string UnitOfMeasure { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public decimal ComponentNum { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public Item Item { get; set; }
    }
}
