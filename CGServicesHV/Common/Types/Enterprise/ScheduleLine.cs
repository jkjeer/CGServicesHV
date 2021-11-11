using System.Collections.Generic;
using System.Runtime.Serialization;
using CGServicesHV.Common.Types.E1;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ScheduleLines : List<ScheduleLine>
    {
    }

    [DataContract(Namespace = "")]
    public class ScheduleLine
    {
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public F5834A11 F5834A11 { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public F580021 F580021 { get; set; }

        [DataMember(Order = 112, EmitDefaultValue = false)]
        public string SoldToCustName { get; set; }

        [DataMember(Order = 114, EmitDefaultValue = false)]
        public string OrigSoldToCustName { get; set; }

        [DataMember(Order = 114, EmitDefaultValue = false)]
        public string OrigShipToCustName { get; set; }

        [DataMember(Order = 120, EmitDefaultValue = false)]
        public F580024 F580024 { get; set; }

        [DataMember(Order = 150, EmitDefaultValue = false)]
        public List<F580220> F580220s { get; set; }

        [DataMember(Order = 160, EmitDefaultValue = false)]
        public List<F4715> F4715s { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ItemAttributes ItemAttributes { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public List<string> FlexItemDescriptions { get; set; }

        [DataMember(Order = 400, EmitDefaultValue = false)]
        public long WorkOrderId { get; set; }

        [DataMember(Order = 402, EmitDefaultValue = false)]
        public long SpecUnitId { get; set; }
    }
}
