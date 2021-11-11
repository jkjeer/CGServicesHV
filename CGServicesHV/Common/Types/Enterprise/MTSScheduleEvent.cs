using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class MTSScheduleEvents : List<MTSScheduleEvent>
    {
    }

    [DataContract(Namespace = "")]
    public class MTSScheduleEvent
    {
        [DataMember(Order = 10)]
        public string PlantNum { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string SchedulePlantNum { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string MfgPlantNum { get; set; }

        [DataMember(Order = 20)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 30)]
        public string BatchNum { get; set; }

        [DataMember(Order = 32)]
        public string CostCenter { get; set; }

        [DataMember(Order = 34)]
        public DateTime ScheduleDate { get; set; }
        [DataMember(Order = 35)]
        public decimal ScheduleDateE1 { get; set; }

        [DataMember(Order = 36)]
        public string ShopFloorSystem { get; set; }

        [DataMember(Order = 38)]
        public string StatusCode { get; set; }

        [DataMember(Order = 40)]
        public string ProcessFlag { get; set; }

        [DataMember(Order = 42)]
        public decimal MathNumeric06 { get; set; }

        [DataMember(Order = 44)]
        public string OperationStatus { get; set; }

        [DataMember(Order = 46)]
        public string Description { get; set; }
    }
}
