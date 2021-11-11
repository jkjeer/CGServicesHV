using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ScheduleEvents : List<ScheduleEvent>
    {
    }

    [DataContract(Namespace = "")]
    public class ScheduleEvent
    {

        [DataMember(Order = 6)]
        public Plant Plant { get; set; }

        [DataMember(Order = 10)]
        public string PlantNum { get; set; }

        [DataMember(Order = 20)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 21)]
        public int ScheduleId { get; set; }

        [DataMember(Order = 22)]
        public Int32 BusinessScheduleId { get; set; }

        [DataMember(Order = 23)]
        public Int64 ShopFloorScheduleId { get; set; }



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

        [DataMember(Order = 100)]
        public string TransferBatchNum { get; set; }
        [DataMember(Order = 102)]
        public string TransferStatusCode { get; set; }
        [DataMember(Order = 104)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 106)]
        public decimal TransferTransactionId { get; set; }

        [DataMember(Order = 108)]
        public int TransferLogId { get; set; }
    }
}
