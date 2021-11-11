using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class GlassItems : List<GlassItem>
    {
    }

    [DataContract(Namespace = "", Name = "GlassItem")]
    public class GlassItem : Item
    {
        // Unit Definitions
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string UnitType { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string UnitTypeDesc { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string UnitTreatment { get; set; }

        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string UnitTreatmentDesc { get; set; }

        [DataMember(Order = 18, EmitDefaultValue = false)]
        public string VolumeIndicator { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal OverallThickness { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public decimal TotalWeight { get; set; }

        // Metadata
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string ProductType { get; set; }

        [DataMember(Order = 101, EmitDefaultValue = false)]
        public string CardinalProductType { get; set; }

        // Unit Processing    
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string CornerClipping { get; set; }

        [DataMember(Order = 152, EmitDefaultValue = false)]
        public string IGCaps { get; set; }

        [DataMember(Order = 154, EmitDefaultValue = false)]
        public string IGCapsDesc { get; set; }

        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string BasePickLocation { get; set; }

        [DataMember(Order = 162, EmitDefaultValue = false)]
        public decimal EndPickItemNum { get; set; }

        [DataMember(Order = 164, EmitDefaultValue = true)]
        public string BatchShiftLotNum { get; set; }

        // Unit Config
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal VerticalSashIntrusion { get; set; }

        [DataMember(Order = 202, EmitDefaultValue = false)]
        public decimal HorizontalSashIntrusion { get; set; }

        [DataMember(Order = 210, EmitDefaultValue = false)]
        public string NarrowMarginCode { get; set; }

        [DataMember(Order = 212, EmitDefaultValue = false)]
        public string NarrowMarginDesc { get; set; }

        [DataMember(Order = 220, EmitDefaultValue = false)]
        public string EnergyPanel { get; set; }

        [DataMember(Order = 222, EmitDefaultValue = false)]
        public string EnergyPanelDesc { get; set; }

        [DataMember(Order = 224, EmitDefaultValue = false)]
        public string PreserveProcessing { get; set; }


        // Customer
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public decimal SoldToCustomerNum { get; set; }

        [DataMember(Order = 302, EmitDefaultValue = false)]
        public decimal ShipToCustomerNum { get; set; }

        [DataMember(Order = 304, EmitDefaultValue = false)]
        public decimal OriginalSoldToCustomerNum { get; set; }

        [DataMember(Order = 306, EmitDefaultValue = false)]
        public decimal OriginalShipToCustomerNum { get; set; }

        [DataMember(Order = 307, EmitDefaultValue = false)]
        public decimal PlannerNum { get; set; }

        [DataMember(Order = 307, EmitDefaultValue = false)]
        public decimal VendorNum { get; set; }

        [DataMember(Order = 310, EmitDefaultValue = false)]
        public Customer Customer { get; set; }

        [DataMember(Order = 312, EmitDefaultValue = false)]
        public Customer ShipToCustomer { get; set; }

        [DataMember(Order = 314, EmitDefaultValue = false)]
        public Customer OriginalSoldToCustomer { get; set; }

        [DataMember(Order = 316, EmitDefaultValue = false)]
        public Customer OriginalShipToCustomer { get; set; }

        [DataMember(Order = 320, EmitDefaultValue = false)]
        public DiscreteItemReference CustomerItemReference { get; set; }

        [DataMember(Order = 330, EmitDefaultValue = false)]
        public string CustomerPartNum1 { get; set; }

        [DataMember(Order = 332, EmitDefaultValue = false)]
        public string CustomerPartNum2 { get; set; }

        [DataMember(Order = 334, EmitDefaultValue = false)]
        public string CustomerPartNum3 { get; set; }

        // Racking
        [DataMember(Order = 400, EmitDefaultValue = false)]
        public string RackType { get; set; }

        [DataMember(Order = 402, EmitDefaultValue = false)]
        public string RackTypeDesc { get; set; }

        [DataMember(Order = 404, EmitDefaultValue = false)]
        public decimal LitesPerRack { get; set; }

        [DataMember(Order = 406, EmitDefaultValue = false)]
        public decimal RackRows { get; set; }

        [DataMember(Order = 408, EmitDefaultValue = false)]
        public decimal RackTiers { get; set; }

        [DataMember(Order = 410, EmitDefaultValue = false)]
        public string RackOrientationVH { get; set; }

        [DataMember(Order = 412, EmitDefaultValue = false)]
        public string RackOrientationIO { get; set; }

        // Shape
        [DataMember(Order = 500, EmitDefaultValue = false)]
        public Shape Shape { get; set; }

        // Lites
        [DataMember(Order = 550, EmitDefaultValue = false)]
        public int NumLites { get; set; }

        [DataMember(Order = 552, EmitDefaultValue = false)]
        public Lites Lites { get; set; }

        // Surfaces
        [DataMember(Order = 700, EmitDefaultValue = false)]
        public Surface ExteriorSurface { get; set; }

        [DataMember(Order = 702, EmitDefaultValue = false)]
        public Surface InteriorSurface { get; set; }

        // Logos
        [DataMember(Order = 800, EmitDefaultValue = false)]
        public Logo Logo { get; set; }

        // Flex Item Desc
        [DataMember(Order = 850, EmitDefaultValue = false)]
        public string FlexItemDesc1 { get; set; }
        [DataMember(Order = 852, EmitDefaultValue = false)]
        public string FlexItemDesc2 { get; set; }
        [DataMember(Order = 854, EmitDefaultValue = false)]
        public string FlexItemDesc3 { get; set; }
        [DataMember(Order = 856, EmitDefaultValue = false)]
        public string FlexItemDesc4 { get; set; }
        [DataMember(Order = 858, EmitDefaultValue = false)]
        public string FlexItemDesc5 { get; set; }
        [DataMember(Order = 860, EmitDefaultValue = false)]
        public string FlexItemDesc6 { get; set; }
        [DataMember(Order = 862, EmitDefaultValue = false)]
        public string FlexItemDesc7 { get; set; }
        [DataMember(Order = 864, EmitDefaultValue = false)]
        public string FlexItemDesc8 { get; set; }

        // Miscellaneous
        [DataMember(Order = 900, EmitDefaultValue = false)]
        public Uoms Uoms { get; set; }

        [DataMember(Order = 910, EmitDefaultValue = false)]
        public ItemComponents ItemComponents { get; set; }

        [DataMember(Order = 920, EmitDefaultValue = false)]
        public ItemRoutings ItemRoutings { get; set; }

        [DataMember(Order = 930, EmitDefaultValue = false)]
        public ItemCostComponents ItemCostComponents { get; set; }

        [DataMember(Order = 940, EmitDefaultValue = false)]
        public bool HasAttributes { get; set; }

        //[DataMember(Order = 1002, EmitDefaultValue = false)]
        public ItemAttributes ItemAttributes { get; set; }
    }
}