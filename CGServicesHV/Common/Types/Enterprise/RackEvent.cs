using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackEvents : List<RackEvent>
    {
    }

    [DataContract(Namespace = "")]
    public class RackEvent
    {
        [DataMember(Order = 10)]
        public decimal EventId { get; set; }
        [DataMember(Order = 20)]
        public decimal RackId { get; set; }
        [DataMember(Order = 22)]
        public string EventCode { get; set; }
        [DataMember(Order = 24)]
        public DateTime EventDate { get; set; }

        [DataMember(Order = 30)]
        public string Status { get; set; }
        [DataMember(Order = 32)]
        public string Location { get; set; }
        [DataMember(Order = 34)]
        public string RackType { get; set; }
        [DataMember(Order = 36)]
        public string ReasonCode { get; set; }

        [DataMember(Order = 40)]
        public decimal CustomerNum { get; set; }
        [DataMember(Order = 41)]
        public decimal CustomerShipNum { get; set; }
        [DataMember(Order = 42)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 44)]
        public decimal BoxNum { get; set; }
        [DataMember(Order = 46)]
        public decimal LoadNum { get; set; }

        [DataMember(Order = 50)]
        public decimal GlassLbs { get; set; }
        [DataMember(Order = 52)]
        public decimal RackLbs { get; set; }
    }
}
