using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class Item : BaseItem
    {
        [DataMember(Order = 10)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 12)]
        public string PlantNum { get; set; }

        [DataMember(Order = 14)]
        public string Description { get; set; }

        [DataMember(Order = 16)]
        public string Construction { get; set; }

        [DataMember(Order = 18)]
        public string StockingType { get; set; }

        [DataMember(Order = 20)]
        public string GLCategory { get; set; }

        [DataMember(Order = 22)]
        public string PrimaryUom { get; set; }
    }
}