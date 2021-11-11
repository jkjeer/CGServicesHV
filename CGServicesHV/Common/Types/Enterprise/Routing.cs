using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Routings : List<Routing>
    {
    }

    [DataContract(Namespace = "")]
    public class Routing
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal OrderNum { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string OrderType { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string RoutingType { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string WorkCenter { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Description { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public decimal SequenceNum { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string ShopFloorCode { get; set; }
    }
}
