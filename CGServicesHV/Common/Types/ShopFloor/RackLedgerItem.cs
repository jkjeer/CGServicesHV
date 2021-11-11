using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class RackLedgerItems : List<RackLedgerItem>
    {
    }

    [DataContract(Namespace = "")]
    public class RackLedgerItem
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantCode { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = true)]
        public decimal RackId { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = true)]
        public string SerialNum { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string StockingType { get; set; }
        [DataMember(Order = 28, EmitDefaultValue = false)]
        public string LotNum { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = true)]
        public string Location { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = true)]
        public string Status { get; set; }
        [DataMember(Order = 34, EmitDefaultValue = true)]
        public decimal QtyOnHand { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal ReceivedQty { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public DateTime StatusDate { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public DateTime ReceivedDate { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public DateTime ExpirationDate { get; set; }
        [DataMember(Order = 56, EmitDefaultValue = false)]
        public DateTime ProductionDate { get; set; }
        [DataMember(Order = 58, EmitDefaultValue = false)]
        public DateTime UpdateDateTime { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal CustomerNum { get; set; }
        [DataMember(Order = 62, EmitDefaultValue = false)]
        public string CustomerName { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal Lite1Thickness { get; set; }
        [DataMember(Order = 72, EmitDefaultValue = false)]
        public string Lite1Color { get; set; }
        [DataMember(Order = 74, EmitDefaultValue = false)]
        public string Lite1HeatTreatment { get; set; }
        [DataMember(Order = 76, EmitDefaultValue = false)]
        public string Lite1FirstSrfCoating { get; set; }
        [DataMember(Order = 78, EmitDefaultValue = false)]
        public string Lite1SecondSrfCoating { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string Lite1PatternTexture { get; set; }
        [DataMember(Order = 82, EmitDefaultValue = false)]
        public string Lite1LamiProdType { get; set; }
        [DataMember(Order = 84, EmitDefaultValue = false)]
        public string Lite1ProdQualityLevel { get; set; }
        [DataMember(Order = 86, EmitDefaultValue = false)]
        public string AnsiiLogoType { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal ShapeNum { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal BaseLeg { get; set; }
        [DataMember(Order = 112, EmitDefaultValue = false)]
        public decimal BaseNum { get; set; }
        [DataMember(Order = 114, EmitDefaultValue = false)]
        public decimal BaseDen { get; set; }
        [DataMember(Order = 116, EmitDefaultValue = false)]
        public decimal LeftLeg { get; set; }
        [DataMember(Order = 118, EmitDefaultValue = false)]
        public decimal LeftNum { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public decimal LeftDen { get; set; }
        [DataMember(Order = 122, EmitDefaultValue = false)]
        public decimal RightLeg { get; set; }
        [DataMember(Order = 124, EmitDefaultValue = false)]
        public decimal RightNum { get; set; }
        [DataMember(Order = 126, EmitDefaultValue = false)]
        public decimal RightDen { get; set; }
        [DataMember(Order = 128, EmitDefaultValue = false)]
        public decimal TopLeg { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal TopNum { get; set; }
        [DataMember(Order = 132, EmitDefaultValue = false)]
        public decimal TopDen { get; set; }
        [DataMember(Order = 134, EmitDefaultValue = false)]
        public decimal Side1Leg { get; set; }
        [DataMember(Order = 136, EmitDefaultValue = false)]
        public decimal Side1Num { get; set; }
        [DataMember(Order = 138, EmitDefaultValue = false)]
        public decimal Side1Den { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public decimal Side2Leg { get; set; }
        [DataMember(Order = 142, EmitDefaultValue = false)]
        public decimal Side2Num { get; set; }
        [DataMember(Order = 144, EmitDefaultValue = false)]
        public decimal Side2Den { get; set; }
        [DataMember(Order = 146, EmitDefaultValue = false)]
        public decimal SqftActual { get; set; }
        [DataMember(Order = 148, EmitDefaultValue = false)]
        public decimal SqftBillable { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public string RackType { get; set; }
        [DataMember(Order = 202, EmitDefaultValue = false)]
        public decimal LitesPerRack { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public string ProcessId { get; set; }
        [DataMember(Order = 212, EmitDefaultValue = false)]
        public string GlCategory { get; set; }
        [DataMember(Order = 214, EmitDefaultValue = false)]
        public string ReasonCode { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public decimal SequenceNum { get; set; }

        [DataMember(Order = 400, EmitDefaultValue = true)]
        public bool HasGlassAttributes { get; set; }

    }
}
