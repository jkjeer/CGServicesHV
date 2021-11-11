using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Uoms : List<Uom>
    {
    }

    [DataContract(Namespace = "")]
    public class Uom
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string UnitMeasure { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string UnitMeasureRelated { get; set; }
        [DataMember(Order = 25, EmitDefaultValue = false)]
        public decimal Factor { get; set; }
    }
}
