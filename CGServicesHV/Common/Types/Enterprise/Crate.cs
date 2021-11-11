using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Crates : List<Crate>
    {
    }

    [DataContract(Namespace = "")]
    public class Crate
    {
        public Crate()
        {
            CrateId = 0;
            CrateType = "";
            OrderQuantity = 0;
            Description = "";
            Quantity = 0;
            Message = "";
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal CrateId { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string CrateType { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal OrderNum { get; set; }

        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string OrderType { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string OrderCompany { get; set; }

        [DataMember(Order = 33, EmitDefaultValue = false)]
        public decimal OrderLineNum { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public decimal OrderQuantity { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal Quantity { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string Message { get; set; }
    }
}
