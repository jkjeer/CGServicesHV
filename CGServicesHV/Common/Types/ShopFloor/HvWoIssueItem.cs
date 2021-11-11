using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class HvWoIssueItems : List<HvWoIssueItem>
    {
    }

    public class HvWoIssueItem
    {
        [DataMember(Order = 1)]
        public int HVWOIssuesID { get; set; }

        [DataMember(Order = 2)]
        public string PlantID { get; set; }

        [DataMember(Order = 3)]
        public decimal WOrderNum { get; set; }

        [DataMember(Order = 4)]
        public string WOrderType { get; set; }

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
        public decimal Quantity { get; set; }

        [DataMember(Order = 17)]
        public decimal QuantityCanceled { get; set; }

        [DataMember(Order = 18)]
        public decimal SqftActual { get; set; }

        [DataMember(Order = 19)]
        public decimal SqftBillable { get; set; }

        [DataMember(Order = 20)]
        public string RejectCode { get; set; }

        [DataMember(Order = 21)]
        public bool Transferred { get; set; }

        [DataMember(Order = 22)]
        public DateTime? TransferredDate { get; set; }

        [DataMember(Order = 23)]
        public bool Upload { get; set; }

        [DataMember(Order = 24)]
        public string Crew { get; set; }

        [DataMember(Order = 25)]
        public string LegacyWorkStation { get; set; }

        [DataMember(Order = 26)]
        public string ProdType { get; set; }

        [DataMember(Order = 27)]
        public bool ShiftOverride { get; set; }

        [DataMember(Order = 28)]
        public string RejectReason { get; set; }

        [DataMember(Order = 29)]
        public double DimBase { get; set; }

        [DataMember(Order = 30)]
        public double DimLeft { get; set; }

        [DataMember(Order = 31)]
        public decimal Thickness { get; set; }

        [DataMember(Order = 32)]
        public string Color { get; set; }

        [DataMember(Order = 33)]
        public string HeatTreatment { get; set; }

        [DataMember(Order = 34)]
        public string FirstSrfCoating { get; set; }

        [DataMember(Order = 35)]
        public string SecondSrfCoating { get; set; }

        [DataMember(Order = 36)]
        public string Pattern { get; set; }

        [DataMember(Order = 37)]
        public string RawCTCFCode { get; set; }

        [DataMember(Order = 38)]
        public bool DefaultSN { get; set; }

        [DataMember(Order = 39)]
        public bool DefaultSNNotification { get; set; }

        [DataMember(Order = 40)]
        public DateTime? DefaultSNNotificationTS { get; set; }

        [DataMember(Order = 41)]
        public string SNUpdateUID { get; set; }

        [DataMember(Order = 42)]
        public bool DoNotUpload { get; set; }

        [DataMember(Order = 43)]
        public int LiteNum { get; set; }

        [DataMember(Order = 44)]
        public string FGSerialNumber { get; set; }

        [DataMember(Order = 45)]
        public string LoaderID { get; set; }

        [DataMember(Order = 46)]
        public bool ReportingOnly { get; set; }

        [DataMember(Order = 47)]
        public DateTime? SNUpdateUTS { get; set; }

        [DataMember(Order = 48)]
        public int HVTransactionHistoryID { get; set; }


        [DataMember(Order = 100)]
        public string BenchmarkReasonCode { get; set; }

        [DataMember(Order = 102)]
        public string RejectReasonDesc { get; set; }

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
