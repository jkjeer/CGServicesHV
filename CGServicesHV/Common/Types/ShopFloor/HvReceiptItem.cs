using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class HvReceiptItems : List<HvReceiptItem>
    {
    }

    public class HvReceiptItem
    {
        [DataMember(Order = 1)]
        public int HVReceiptsID { get; set; }

        [DataMember(Order = 2)]
        public string PlantID { get; set; }

        [DataMember(Order = 3)]
        public decimal WOrderNum { get; set; }

        [DataMember(Order = 4)]
        public string WOrderType { get; set; }

        [DataMember(Order = 5)]
        public decimal E1Schedule { get; set; }

        [DataMember(Order = 6)]
        public decimal FirmSchedule { get; set; }

        [DataMember(Order = 7)]
        public DateTime? TransactionDate { get; set; }

        [DataMember(Order = 8)]
        public DateTime? ProductionDate { get; set; }

        [DataMember(Order = 9)]
        public string SerialNumber { get; set; }

        [DataMember(Order = 10)]
        public string Location { get; set; }

        [DataMember(Order = 11)]
        public string ProductionLineCode { get; set; }

        [DataMember(Order = 12)]
        public string LineOperationCode { get; set; }

        [DataMember(Order = 13)]
        public string E1Workcenter { get; set; }

        [DataMember(Order = 14)]
        public string E1Operation { get; set; }

        [DataMember(Order = 15)]
        public string Workstation { get; set; }

        [DataMember(Order = 16)]
        public string LineID { get; set; }

        [DataMember(Order = 17)]
        public string Shift { get; set; }

        [DataMember(Order = 18)]
        public decimal GlassHandler1 { get; set; }

        [DataMember(Order = 19)]
        public decimal GlassHandler2 { get; set; }

        [DataMember(Order = 20)]
        public bool WIP { get; set; }

        [DataMember(Order = 21)]
        public decimal Quantity { get; set; }

        [DataMember(Order = 22)]
        public decimal SqftActual { get; set; }

        [DataMember(Order = 23)]
        public decimal SqftBillable { get; set; }

        [DataMember(Order = 24)]
        public bool Transferred { get; set; }

        [DataMember(Order = 25)]
        public DateTime? TransferredDate { get; set; }

        [DataMember(Order = 26)]
        public bool Upload { get; set; }

        [DataMember(Order = 27)]
        public string ProdType { get; set; }

        [DataMember(Order = 28)]
        public string Crew { get; set; }

        [DataMember(Order = 29)]
        public string Lite { get; set; }

        [DataMember(Order = 30)]
        public bool ShiftOverride { get; set; }

        [DataMember(Order = 31)]
        public string IPAddress { get; set; }

        [DataMember(Order = 32)]
        public string Comment { get; set; }

        [DataMember(Order = 33)]
        public string RackType { get; set; }

        [DataMember(Order = 34)]
        public bool ReportingOnly { get; set; }

        [DataMember(Order = 35)]
        public bool WIP_PAR { get; set; }

        [DataMember(Order = 36)]
        public decimal PartialQty { get; set; }

        [DataMember(Order = 37)]
        public string PartialSN { get; set; }

        [DataMember(Order = 38)]
        public string RackStatus { get; set; }

        [DataMember(Order = 39)]
        public string RackReasonStatus { get; set; }

        [DataMember(Order = 40)]
        public int WIP_TYPE { get; set; }

        [DataMember(Order = 41)]
        public decimal ProducedQuantity { get; set; }

        [DataMember(Order = 42)]
        public int HVTransactionHistoryID { get; set; }

        [DataMember(Order = 43)]
        public decimal FreeLiteQuantity { get; set; }

        [DataMember(Order = 44)]
        public string AssetID { get; set; }

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
