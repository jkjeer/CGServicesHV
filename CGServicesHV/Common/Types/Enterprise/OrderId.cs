using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class OrderIds : List<OrderId>
    {
    }

    [DataContract(Namespace = "")]
    public class OrderId
    {
        [DataMember(Order = 10)]
        public decimal DocumentNum { get; set; }

        [DataMember(Order = 20)]
        public string DocumentType { get; set; }

        [DataMember(Order = 30)]
        public decimal DocumentLineNum { get; set; }

        [DataMember(Order = 40)]
        public string Company { get; set; }

        [DataMember(Order = 50)]
        public string Plant { get; set; }

        [DataMember(Order = 60)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 62)]
        public string LotNum { get; set; }

        [DataMember(Order = 100)]
        public string Description { get; set; }
    }
}
