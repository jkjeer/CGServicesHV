using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class SpecUnitDocuments : List<SpecUnitDocument>
    {
    }

    [DataContract(Namespace = "")]
    public class SpecUnitDocument
    {
        [DataMember(Order = 10)]
        public decimal ConfigurationId { get; set; }

        [DataMember(Order = 12)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 14)]
        public string StockType { get; set; }

        [DataMember(Order = 16)]
        public string UnitType { get; set; }

        [DataMember(Order = 20)]
        public decimal DocumentNum { get; set; }

        [DataMember(Order = 30)]
        public string DocumentType { get; set; }

        [DataMember(Order = 40)]
        public decimal DocumentLineNum { get; set; }

        [DataMember(Order = 50)]
        public DateTime CMDate { get; set; }

        [DataMember(Order = 51)]
        public DateTime TransactionDate { get; set; }
    }
}