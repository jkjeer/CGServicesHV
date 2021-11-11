using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CGServicesHV.Common.Types.E1;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Schedules : List<Schedule>
    {
    }

    [DataContract(Namespace = "")]
    public class Schedule
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public Plant Plant { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public long ScheduleId { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public decimal ScheduleDateE1 { get; set; }

        [DataMember(Order = 15, EmitDefaultValue = false)]
        public DateTime? ScheduleDate { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public F580020 f580020 { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public F5834A01 f5834A01 { get; set; }


        [DataMember(Order = 100, EmitDefaultValue = false)]
        public ScheduleLines ScheduleLines { get; set; }
    }
}
