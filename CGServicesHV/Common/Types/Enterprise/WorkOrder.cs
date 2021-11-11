using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class WorkOrders : List<WorkOrder>
    {
    }

    [DataContract(Namespace = "")]
    public class WorkOrder
    {
        [DataMember(Order = 10)]
        public decimal WorkOrderNum { get; set; }

        [DataMember(Order = 20)]
        public string PlantNum { get; set; }
        [DataMember(Order = 22)]
        public string PlantCode { get; set; }
        [DataMember(Order = 24)]
        public string StatusCode { get; set; }
        [DataMember(Order = 26)]
        public string StatusDesc { get; set; }
        [DataMember(Order = 28)]
        public DateTime OrderDate { get; set; }
        [DataMember(Order = 30)]
        public DateTime RequestedDate { get; set; }
        [DataMember(Order = 32)]
        public decimal Quantity { get; set; }

        [DataMember(Order = 40)]
        public decimal RelatedOrderNum { get; set; }
        [DataMember(Order = 42)]
        public string RelatedOrderType { get; set; }
        [DataMember(Order = 44)]
        public decimal RelatedOrderLineNum { get; set; }
        [DataMember(Order = 45)]
        public string RelatedOrderCompany { get; set; }
        [DataMember(Order = 46)]
        public string StockingType { get; set; }
        [DataMember(Order = 48)]
        public RelatedSalesOrderLine RelatedSalesOrderLine { get; set; }
        //[DataMember(Order = 49)]
        //public RelatedSalesOrderLines RelatedSalesOrderLines { get; set; }

        [DataMember(Order = 60)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 61)]
        public string UnitType { get; set; }
        [DataMember(Order = 62)]
        //public Item Item { get; set; }
        public GlassItem GlassItem { get; set; }

        [DataMember(Order = 100)]
        public Routings Routings { get; set; }

        [DataMember(Order = 110)]
        public WorkOrderComponents WorkOrderComponents { get; set; }

        [DataMember(Order = 200)]
        public List<decimal> UnitIds { get; set; }
    }
}
