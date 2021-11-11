using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Racks : List<Rack>
    {
    }

    [DataContract(Namespace = "")]
    public class Rack
    {
        [DataMember(Order = 10)]
        public decimal RackId { get; set; }
        [DataMember(Order = 12)]
        public string CostCenter { get; set; }
        [DataMember(Order = 13)]
        public string RackType { get; set; }
        [DataMember(Order = 14)]
        public string Location { get; set; }
        [DataMember(Order = 16)]
        public string HeaderStatus { get; set; }
        [DataMember(Order = 18)]
        public DateTime HeaderStatusDate { get; set; }
        [DataMember(Order = 20)]
        public decimal GlassWeight { get; set; }
        [DataMember(Order = 21)]
        public decimal TareWeight { get; set; }
        [DataMember(Order = 22)]
        public decimal GrossWeight { get; set; }
        [DataMember(Order = 24)]
        public string ReasonCode { get; set; }

        [DataMember(Order = 40)]
        public Customer Customer { get; set; }
        [DataMember(Order = 42)]
        public Customer ShipToCustomer { get; set; }
        [DataMember(Order = 44)]
        public decimal SupplierNum { get; set; }

        [DataMember(Order = 50)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 51)]
        public decimal BoxNum { get; set; }
        [DataMember(Order = 52)]
        public decimal LoadNum { get; set; }

        [DataMember(Order = 90)]
        public DateTime? ReceivedDate { get; set; }
        [DataMember(Order = 92)]
        public decimal ReceivedQty { get; set; }

        [DataMember(Order = 94)]
        public string ProcessId { get; set; }

        [DataMember(Order = 100)]
        public DateTime UpdateDateTime { get; set; }

        [DataMember(Order = 110)]
        public RackEvents RackEvents { get; set; }

        [DataMember(Order = 200)]
        public RackItems RackItems { get; set; }


    }
}
