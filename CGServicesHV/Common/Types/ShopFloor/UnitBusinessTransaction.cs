using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class UnitBusinessTransactions : List<UnitBusinessTransaction>
    {
    }

    public class UnitBusinessTransaction
    {

        public UnitBusinessTransaction()
        {
            ProcessingStatus = new ProcessingStatus();
        }

        [DataMember(Order = 10)]
        public int UnitBusinessTransactionId { get; set; }
        [DataMember(Order = 12)]
        public string TransactionType { get; set; }
        [DataMember(Order = 13)]
        public int TransactionId { get; set; }
        [DataMember(Order = 14)]
        public DateTime? TransactionDate { get; set; }
        [DataMember(Order = 15)]
        public string TransactionDateChar { get; set; }
        [DataMember(Order = 16)]
        public DateTime? ProductionDate { get; set; }
        [DataMember(Order = 17)]
        public string ProdType { get; set; }

        [DataMember(Order = 20)]
        public string PlantID { get; set; }
        [DataMember(Order = 22)]
        public decimal WOrderNum { get; set; }
        [DataMember(Order = 23)]
        public string LotNum { get; set; }
        [DataMember(Order = 24)]
        public string SerialNum { get; set; }
        [DataMember(Order = 25)]
        public decimal RackId { get; set; }
        [DataMember(Order = 26)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 27)]
        public string LocationCode { get; set; }
        [DataMember(Order = 28)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 29)]
        public decimal BoxNum { get; set; }
        [DataMember(Order = 30)]
        public string RoutingCode { get; set; }

        [DataMember(Order = 40)]
        public string LineTypeCode { get; set; }
        [DataMember(Order = 41)]
        public string ProductionLineCode { get; set; }
        [DataMember(Order = 42)]
        public string E1WorkcenterCode { get; set; }
        [DataMember(Order = 44)]
        public string LineOperationCode { get; set; }
        [DataMember(Order = 46)]
        public string E1OperationCode { get; set; }
        [DataMember(Order = 48)]
        public string LineTransactionCode { get; set; }
        [DataMember(Order = 49)]
        public string LineTransactionDesc { get; set; }
        [DataMember(Order = 50)]
        public int LineCurrentStatusCode { get; set; }
        [DataMember(Order = 52)]
        public bool IsFinalStation { get; set; }

        [DataMember(Order = 60)]
        public string BusinessRackStatusCode { get; set; }
        [DataMember(Order = 62)]
        public string BusinessRackStationCode { get; set; }
        [DataMember(Order = 64)]
        public decimal Quantity { get; set; }

        [DataMember(Order = 80)]
        public string RejectReason { get; set; }
        [DataMember(Order = 81)]
        public string RejectCode { get; set; }
        [DataMember(Order = 82)]
        public string Comments { get; set; }

        [DataMember(Order = 200)]
        public ProcessingStatus ProcessingStatus { get; set; }
    }
}
