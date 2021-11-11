using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class HvTrimItems : List<HvTrimItem>
    {
    }

    public class HvTrimItem
    {
        [DataMember(Order = 1)]
        public int HVTrimID { get; set; }

        [DataMember(Order = 2)]
        public string PlantID { get; set; }

        [DataMember(Order = 3)]
        public decimal E1Schedule { get; set; }

        [DataMember(Order = 4)]
        public decimal FirmSchedule { get; set; }

        [DataMember(Order = 5)]
        public DateTime? TransactionDate { get; set; }

        [DataMember(Order = 6)]
        public DateTime? ProductionDate { get; set; }

        [DataMember(Order = 7)]
        public string ItemNum { get; set; }

        [DataMember(Order = 8)]
        public string RawSerialNumber { get; set; }

        [DataMember(Order = 9)]
        public string ProductionLineCode { get; set; }

        [DataMember(Order = 10)]
        public string LineOperationCode { get; set; }

        [DataMember(Order = 11)]
        public string E1Workcenter { get; set; }

        [DataMember(Order = 12)]
        public string E1Operation { get; set; }

        [DataMember(Order = 13)]
        public string Workstation { get; set; }

        [DataMember(Order = 14)]
        public string LineID { get; set; }

        [DataMember(Order = 15)]
        public string Shift { get; set; }

        [DataMember(Order = 16)]
        public string Crew { get; set; }

        [DataMember(Order = 17)]
        public string TrimSide { get; set; }

        [DataMember(Order = 18)]
        public string RejectCode { get; set; }

        [DataMember(Order = 19)]
        public decimal SqftTrim { get; set; }

        [DataMember(Order = 20)]
        public bool Transferred { get; set; }

        [DataMember(Order = 21)]
        public DateTime? TransferredDate { get; set; }

        [DataMember(Order = 22)]
        public decimal RackID { get; set; }

        [DataMember(Order = 23)]
        public bool ShiftOverride { get; set; }

        [DataMember(Order = 200)]
        public decimal TransferId { get; set; }

        [DataMember(Order = 202)]
        public decimal TransferBatchId { get; set; }

        [DataMember(Order = 204)]
        public string TransferStatus { get; set; }

        [DataMember(Order = 206)]
        public string TransferMessage { get; set; }
    }
}
