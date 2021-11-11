using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class MTSScheduleLines : List<MTSScheduleLine>
    {
    }

    [DataContract(Namespace = "")]
    public class MTSScheduleLine
    {
        [DataMember(Order = 10)]
        public decimal ScheduleSeqNum { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string SchedulePlantNum { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string MfgPlantNum { get; set; }


        [DataMember(Order = 50)]
        public DateTime ReleaseDate { get; set; }
        [DataMember(Order = 51)]
        public DateTime RequestedDate { get; set; }

        [DataMember(Order = 60)]
        public decimal SoldToCustNum { get; set; }
        [DataMember(Order = 62)]
        public Customer SoldToCustomer { get; set; }
        [DataMember(Order = 70)]
        public decimal ShipToCustNum { get; set; }
        [DataMember(Order = 72)]
        public Customer ShipToCustomer { get; set; }

        [DataMember(Order = 100)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 110)]
        public string IgLine { get; set; }
        [DataMember(Order = 120)]
        public string BoxPriority { get; set; }
        [DataMember(Order = 130)]
        public string Bungee { get; set; }
        [DataMember(Order = 140)]
        public decimal BungeeSize { get; set; }
        [DataMember(Order = 150)]
        public string SchedulerMsg1 { get; set; }
        [DataMember(Order = 160)]
        public string SchedulerMsg2 { get; set; }
        [DataMember(Order = 170)]
        public string Comment1 { get; set; }
        [DataMember(Order = 180)]
        public string Comment2 { get; set; }
        [DataMember(Order = 190)]
        public string Comment3 { get; set; }

        [DataMember(Order = 200)]
        public string Value01 { get; set; }
        [DataMember(Order = 202)]
        public string Value02 { get; set; }
        [DataMember(Order = 204)]
        public string Value03 { get; set; }

        //[DataMember(Order = 500)]
        //public RelatedSalesOrderLine RelatedSalesOrderLine { get; set; }

        [DataMember(Order = 600)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(Order = 602)]
        public WorkOrder WorkOrder { get; set; }

        //[DataMember(Order = 700)]
        //public EdiMessages EdiMessages { get; set; }
    }
}
