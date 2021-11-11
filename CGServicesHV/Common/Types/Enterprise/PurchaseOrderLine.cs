using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class PurchaseOrderLines : List<PurchaseOrderLine>
    {
    }

    [DataContract(Namespace = "")]
    public class PurchaseOrderLine : BaseItem
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public Decimal OrderLineNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public String LotNum { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public String Description { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public String LineType { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public String NextStatusCode { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public String LastStatusCode { get; set; }
        [DataMember(Order = 25, EmitDefaultValue = false)]
        public String UnitOfMeasure { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public Decimal UnitsQuantity { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public Decimal ExtendedCost { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = true)]
        public String CustomerSequenceNum { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public String GenericSequenceNum { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public String ProductType { get; set; }
        [DataMember(Order = 56, EmitDefaultValue = false)]
        public String CustomerPart1 { get; set; }
        [DataMember(Order = 56, EmitDefaultValue = false)]
        public String CustomerPart2 { get; set; }
        [DataMember(Order = 58, EmitDefaultValue = false)]
        public String CustomerPart3 { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public String CustomerPart4 { get; set; }
        [DataMember(Order = 62, EmitDefaultValue = false)]
        public String ShopFloorMsg1 { get; set; }
        [DataMember(Order = 64, EmitDefaultValue = false)]
        public String ShopFloorMsg2 { get; set; }
        [DataMember(Order = 66, EmitDefaultValue = false)]
        public String PickNotes { get; set; }
        [DataMember(Order = 68, EmitDefaultValue = false)]
        public String PackNotes { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public Decimal ItemNum { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public Item Item { get; set; }

        [DataMember(Order = 150, EmitDefaultValue = false)]
        public RelatedSalesOrderLine RelatedSalesOrderLine { get; set; }

    }
}
