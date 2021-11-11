using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackTypes : List<RackType>
    {
    }

    [DataContract(Namespace = "")]
    public class RackType
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string RackTypeCode { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string RackTypeName { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string RackTypeStatus { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string CostCenter { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public decimal AddressShipToNum { get; set; }

        [DataMember(Order = 25, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal TareWeight { get; set; }
        [DataMember(Order = 31, EmitDefaultValue = false)]
        public decimal MinTotalWeight { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public decimal MaxTotalWeight { get; set; }
        [DataMember(Order = 33, EmitDefaultValue = false)]
        public string WeightUom { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string VolumeUom { get; set; }
        [DataMember(Order = 41, EmitDefaultValue = false)]
        public decimal GrossHeight { get; set; }
        [DataMember(Order = 42, EmitDefaultValue = false)]
        public decimal GrossWidth { get; set; }
        [DataMember(Order = 43, EmitDefaultValue = false)]
        public decimal GrossDepth { get; set; }
        [DataMember(Order = 44, EmitDefaultValue = false)]
        public decimal GlassMinLength { get; set; }
        [DataMember(Order = 45, EmitDefaultValue = false)]
        public decimal GlassMaxLength { get; set; }
        [DataMember(Order = 46, EmitDefaultValue = false)]
        public decimal GlassMinHeight { get; set; }
        [DataMember(Order = 47, EmitDefaultValue = false)]
        public decimal GlassMaxHeight { get; set; }
        [DataMember(Order = 48, EmitDefaultValue = false)]
        public decimal GlassMinWidth { get; set; }
        [DataMember(Order = 49, EmitDefaultValue = false)]
        public decimal GlassMaxWidth { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string DimensionUom { get; set; }
        [DataMember(Order = 51, EmitDefaultValue = false)]
        public decimal GrossCubicDimensions { get; set; }

        [DataMember(Order = 55, EmitDefaultValue = false)]
        public decimal RackSides { get; set; }
        [DataMember(Order = 56, EmitDefaultValue = false)]
        public decimal RackPositions { get; set; }
        [DataMember(Order = 57, EmitDefaultValue = false)]
        public decimal RackStacks { get; set; }
        [DataMember(Order = 58, EmitDefaultValue = false)]
        public decimal RackRows { get; set; }
        [DataMember(Order = 59, EmitDefaultValue = false)]
        public decimal RackSlots { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string ShapeFlag { get; set; }
        [DataMember(Order = 61, EmitDefaultValue = false)]
        public string AnnealedFlag { get; set; }
        [DataMember(Order = 62, EmitDefaultValue = false)]
        public string CorpFlag { get; set; }
        [DataMember(Order = 63, EmitDefaultValue = false)]
        public string ShippableFlag { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDateTime { get; set; }
    }
}
