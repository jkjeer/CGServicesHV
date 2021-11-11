using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class RackAssetTransactionItems : List<RackAssetTransactionItem>
    {
    }

    public class RackAssetTransactionItem
    {

        [DataMember(Order = 1)]
        public int RackAssetTransactionId { get; set; }

        [DataMember(Order = 2)]
        public string PlantNum { get; set; }

        [DataMember(Order = 10)]
        public decimal RackId { get; set; }

        [DataMember(Order = 20)]
        public string AssetId { get; set; }

        [DataMember(Order = 30)]
        public DateTime TransactionDate { get; set; }

        [DataMember(Order = 35)]
        public string TransactionType { get; set; }

        [DataMember(Order = 40)]
        public string UserId { get; set; }

        [DataMember(Order = 50)]
        public string WorkstationId { get; set; }

        [DataMember(Order = 60)]
        public bool DoNotTransfer { get; set; }

        [DataMember(Order = 100)]
        public decimal TransferId { get; set; }

        [DataMember(Order = 110)]
        public string BatchNum { get; set; }

        [DataMember(Order = 120)]
        public string TransferStatus { get; set; }

        [DataMember(Order = 130)]
        public DateTime? TransferDate { get; set; }

        [DataMember(Order = 140)]
        public string TransferMessage { get; set; }

    }
}
