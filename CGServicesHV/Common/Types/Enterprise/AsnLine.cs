using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class AsnLines : List<AsnLine>
    {
    }

    [DataContract(Namespace = "")]
    public class AsnLine
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public int AsnLineId { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal OrderNum { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string OrderType { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public decimal OrderLineNum { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string OrderCompanyNum { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public string OrderStatus { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public int SerialNum { get; set; }
        [DataMember(Order = 28, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string ItemDesc1 { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string ItemDesc2 { get; set; }
        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string DimensionDesc { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public int ShapeNum { get; set; }
        [DataMember(Order = 42, EmitDefaultValue = false)]
        public decimal BaseDim { get; set; }
        [DataMember(Order = 43, EmitDefaultValue = false)]
        public decimal LeftDim { get; set; }
        [DataMember(Order = 44, EmitDefaultValue = false)]
        public decimal RightDim { get; set; }
        [DataMember(Order = 45, EmitDefaultValue = false)]
        public decimal TopDim { get; set; }
        [DataMember(Order = 46, EmitDefaultValue = false)]
        public decimal Side1Dim { get; set; }
        [DataMember(Order = 47, EmitDefaultValue = false)]
        public decimal Side2Dim { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public int Quantity { get; set; }
        [DataMember(Order = 62, EmitDefaultValue = false)]
        public decimal UnitActualSqft { get; set; }
        [DataMember(Order = 64, EmitDefaultValue = false)]
        public decimal TotalActualSqft { get; set; }
        [DataMember(Order = 66, EmitDefaultValue = false)]
        public decimal UnitBillableSqft { get; set; }
        [DataMember(Order = 68, EmitDefaultValue = false)]
        public decimal TotalBillableSqft { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal UnitAmount { get; set; }
        [DataMember(Order = 72, EmitDefaultValue = false)]
        public decimal TotalAmount { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime WrittenDate { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public string WrittenBy { get; set; }
    }
}
