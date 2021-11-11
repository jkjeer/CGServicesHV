using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RelatedSalesOrderLines : List<RelatedSalesOrderLine>
    {
    }

    [DataContract(Namespace = "")]
    public class RelatedSalesOrderLine
    {
        public RelatedSalesOrderLine()
        {
            Crate = new Crate();
            EdiMessages = new EdiMessages();
            ProductionTransactions = new ProductionTransactions();
        }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string OrderCompany { get; set; }
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal OrderNum { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string OrderType { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public decimal OrderLineNum { get; set; }

        [DataMember(Order = 18, EmitDefaultValue = false)]
        public decimal WorkOrderNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal SoldToCustNum { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public Customer SoldToCustomer { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public decimal ShipToCustNum { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public Customer ShipToCustomer { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public decimal OriginalSoldToCustNum { get; set; }
        [DataMember(Order = 25, EmitDefaultValue = false)]
        public Customer OriginalSoldToCustomer { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public decimal OriginalShipToCustNum { get; set; }
        [DataMember(Order = 27, EmitDefaultValue = false)]
        public Customer OriginalShipToCustomer { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string VolumeIndicator { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 41, EmitDefaultValue = false)]
        public string UnitType { get; set; }
        [DataMember(Order = 42, EmitDefaultValue = false)]
        public string UnitTypeDesc { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal CustomerLiteNum { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public string TransactionType { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string CustomerSequenceNum { get; set; }
        [DataMember(Order = 101, EmitDefaultValue = false)]
        public string GenericSequenceNum { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public string ProductType { get; set; }
        [DataMember(Order = 104, EmitDefaultValue = false)]
        public string CustomerPart2 { get; set; }
        [DataMember(Order = 106, EmitDefaultValue = false)]
        public string CustomerPart3 { get; set; }
        [DataMember(Order = 108, EmitDefaultValue = false)]
        public string CustomerPart4 { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string ShopFloorMsg1 { get; set; }
        [DataMember(Order = 112, EmitDefaultValue = false)]
        public string ShopFloorMsg2 { get; set; }
        [DataMember(Order = 114, EmitDefaultValue = false)]
        public string PickNotes { get; set; }
        [DataMember(Order = 116, EmitDefaultValue = false)]
        public string PackNotes { get; set; }
        [DataMember(Order = 118, EmitDefaultValue = false)]
        public string CustomerPO { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string Department { get; set; }
        [DataMember(Order = 122, EmitDefaultValue = false)]
        public string ReasonCode { get; set; }
        [DataMember(Order = 123, EmitDefaultValue = false)]
        public string PriorityCode { get; set; }
        [DataMember(Order = 124, EmitDefaultValue = false)]
        public string Reference2 { get; set; }

        [DataMember(Order = 150, EmitDefaultValue = false)]
        public DateTime OrderDate { get; set; }
        [DataMember(Order = 152, EmitDefaultValue = false)]
        public DateTime RequestedDate { get; set; }
        [DataMember(Order = 153, EmitDefaultValue = false)]
        public int RequestedTime { get; set; }
        [DataMember(Order = 156, EmitDefaultValue = false)]
        public decimal OrderQty { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public string Crating { get; set; }
        [DataMember(Order = 202, EmitDefaultValue = false)]
        public string CratingDesc { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public Crate Crate { get; set; }

        [DataMember(Order = 220, EmitDefaultValue = false)]
        public bool TubesAdded { get; set; }
        [DataMember(Order = 222, EmitDefaultValue = false)]
        public bool ScoringAdded { get; set; }
        [DataMember(Order = 224, EmitDefaultValue = false)]
        public bool PreserveAdded { get; set; }
        [DataMember(Order = 226, EmitDefaultValue = false)]
        public bool Flippable { get; set; }
        [DataMember(Order = 228, EmitDefaultValue = false)]
        public bool CrateAdded { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public EdiMessages EdiMessages { get; set; }

        [DataMember(Order = 500, EmitDefaultValue = false)]
        public ProductionTransactions ProductionTransactions { get; set; }
    }
}
