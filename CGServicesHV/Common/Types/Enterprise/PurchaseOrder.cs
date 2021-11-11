using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class PurchaseOrder
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public String Company { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public String PlantNum { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public Decimal DocumentNum { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public Decimal OrderNum { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public String OrderType { get; set; }
        [DataMember(Order = 18, EmitDefaultValue = false)]
        public Decimal NumLines { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public Customer SoldToCustomer { get; set; }
        [DataMember(Order = 36, EmitDefaultValue = false)]
        public Customer ShipToCustomer { get; set; }
        [DataMember(Order = 38, EmitDefaultValue = false)]
        public Customer Vendor { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public DateTime RequestedDate { get; set; }
        [DataMember(Order = 42, EmitDefaultValue = false)]
        public DateTime TransactionDate { get; set; }
        [DataMember(Order = 44, EmitDefaultValue = false)]
        public DateTime ScheduledPickDate { get; set; }
        [DataMember(Order = 46, EmitDefaultValue = false)]
        public DateTime OriginalPromisedDate { get; set; }
        [DataMember(Order = 48, EmitDefaultValue = false)]
        public DateTime ActualShipDate { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public DateTime CancelDate { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public Decimal AttachedLines { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public PurchaseOrderLines PurchaseOrderLines { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public List<String> DelimitedLines { get; set; }
    }
}
