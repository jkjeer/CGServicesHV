using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class RackTransactionItems : List<RackTransactionItem>
    {
    }

    public class RackTransactionItem
    {
        [DataMember(Order = 1)]
        public int E1RackingTransactionId { get; set; }

        [DataMember(Order = 2)]
        public string DocumentType { get; set; }

        [DataMember(Order = 3)]
        public string SerialNumber { get; set; }

        [DataMember(Order = 4)]
        public decimal RackId { get; set; }

        [DataMember(Order = 5)]
        public string Location { get; set; }

        [DataMember(Order = 6)]
        public decimal QtyOnHand { get; set; }

        [DataMember(Order = 7)]
        public string RackStatus { get; set; }

        [DataMember(Order = 8)]
        public DateTime? CreateDate { get; set; }

        [DataMember(Order = 9)]
        public bool Transferred { get; set; }

        [DataMember(Order = 10)]
        public DateTime? TransferredDate { get; set; }

        [DataMember(Order = 11)]
        public string PCName { get; set; }

        [DataMember(Order = 12)]
        public string ItemNum { get; set; }

        [DataMember(Order = 13)]
        public string DestinationSerialNumber { get; set; }

        [DataMember(Order = 14)]
        public string ReasonCode { get; set; }

        [DataMember(Order = 15)]
        public string RackType { get; set; }

        [DataMember(Order = 16)]
        public string E1WorkCenter { get; set; }

        [DataMember(Order = 17)]
        public bool DefaultSN { get; set; }

        [DataMember(Order = 18)]
        public int HVTransactionHistoryID { get; set; }

        [DataMember(Order = 19)]
        public string FromRackStatus { get; set; }

        [DataMember(Order = 20)]
        public string FromReasonCode { get; set; }

        [DataMember(Order = 100)]
        public string PlantID { get; set; }

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
