using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackItems : List<RackItem>
    {
    }

    [DataContract(Namespace = "")]
    public class RackItem
    {
        [DataMember(Order = 1)]
        public decimal RackId { get; set; }
        [DataMember(Order = 2)]
        public string SerialNum { get; set; }
        [DataMember(Order = 3)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 4)]
        public string GLCategory { get; set; }
        [DataMember(Order = 5)]
        public string PlantNum { get; set; }
        [DataMember(Order = 6)]
        public decimal ConfigId { get; set; }
        [DataMember(Order = 4)]
        public string StockCode { get; set; }

        [DataMember(Order = 10)]
        public string Side { get; set; }
        [DataMember(Order = 12)]
        public decimal Position { get; set; }
        [DataMember(Order = 14)]
        public decimal StackTier { get; set; }
        [DataMember(Order = 16)]
        public decimal Row { get; set; }
        [DataMember(Order = 18)]
        public decimal Slot { get; set; }
        [DataMember(Order = 20)]
        public string BoxRotated { get; set; }

        [DataMember(Order = 30)]
        public Item Item { get; set; }
        [DataMember(Order = 32)]
        public string LotNum { get; set; }
        [DataMember(Order = 33)]
        public string Location { get; set; }
        [DataMember(Order = 34)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 40)]
        public string StatusCode { get; set; }
        [DataMember(Order = 41)]
        public string DetailStatus { get; set; }
        [DataMember(Order = 42)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 44)]
        public string QuantityUom { get; set; }
        [DataMember(Order = 46)]
        public decimal GlassWeight { get; set; }
        [DataMember(Order = 48)]
        public string GlassWeightUom { get; set; }
        [DataMember(Order = 50)]
        public DateTime TransactionDate { get; set; }
        [DataMember(Order = 52)]
        public DateTime ProductionDate { get; set; }

        [DataMember(Order = 60)]
        public DateTime ManufacturedDate { get; set; }
        [DataMember(Order = 62)]
        public String ManufacturedPlant { get; set; }

        [DataMember(Order = 70)]
        public DateTime? ReceivedDate { get; set; }
        [DataMember(Order = 74)]
        public decimal ReceivedQty { get; set; }

        [DataMember(Order = 80)]
        public decimal SalesOrderNum { get; set; }
        [DataMember(Order = 82)]
        public String SalesOrderType { get; set; }
        [DataMember(Order = 84)]
        public decimal SalesOrderLineNum { get; set; }
        [DataMember(Order = 85)]
        public string SalesOrderCompany { get; set; }
        [DataMember(Order = 86)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(Order = 88)]
        public string WorkOrderPlantNum { get; set; }

        [DataMember(Order = 90)]
        public decimal PurchaseOrderNum { get; set; }
        [DataMember(Order = 92)]
        public String PurchaseOrderType { get; set; }
        [DataMember(Order = 94)]
        public decimal PurchaseOrderLineNum { get; set; }
        [DataMember(Order = 95)]
        public string PurchaseOrderCompany { get; set; }
        [DataMember(Order = 96)]
        public decimal SupplierNum { get; set; }

        [DataMember(Order = 100)]
        public DateTime UpdateDateTime { get; set; }
        [DataMember(Order = 110)]
        public String ProcessId { get; set; }

        [DataMember(Order = 200)]
        public bool HasGlassAttributes { get; set; }
        [DataMember(Order = 210)]
        public GlassItem GlassItem { get; set; }



    }
}
