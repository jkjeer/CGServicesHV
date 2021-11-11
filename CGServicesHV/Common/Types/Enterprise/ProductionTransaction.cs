using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ProductionTransactions : List<ProductionTransaction>
    {
    }

    [DataContract(Namespace = "")]
    public class ProductionTransaction
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal TransactionId { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public DateTime TransactionDate { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string WorkCenter { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string WorkCenterDesc { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal OrderNum { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string OrderType { get; set; }
        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string UnitId { get; set; }
        [DataMember(Order = 36, EmitDefaultValue = true)]
        public decimal LiteNum { get; set; }
        [DataMember(Order = 38, EmitDefaultValue = false)]
        public decimal RackId { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string DocumentType { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public string TransactionType { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public String RejectReason { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public Decimal RejectQuantity { get; set; }
    }
}
