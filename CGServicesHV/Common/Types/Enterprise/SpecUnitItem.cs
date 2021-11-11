using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class SpecUnitItems : List<SpecUnitItem>
    {
    }

    [DataContract(Namespace = "")]
    public class SpecUnitItem
    {
        [DataMember(Order = 10)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 14)]
        public string StockType { get; set; }

        [DataMember(Order = 16)]
        public string UnitType { get; set; }

        [DataMember(Order = 20)]
        public decimal ConfigId { get; set; }

        [DataMember(Order = 30)]
        public DateTime TransactionDate { get; set; }

        [DataMember(Order = 100)]
        public SpecUnit SpecUnit { get; set; }

        [DataMember(Order = 200)]
        public ItemAttributes ItemAttributes { get; set; }

        [DataMember(Order = 240)]
        public DateTime IADate { get; set; }

        [DataMember(Order = 250)]
        public DateTime IBDate { get; set; }

        [DataMember(Order = 250)]
        public DateTime CMDate { get; set; }

    }
}