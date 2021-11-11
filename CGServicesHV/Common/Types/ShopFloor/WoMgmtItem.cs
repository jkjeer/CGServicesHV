using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class WoMgmtItems : List<WoMgmtItem>
    {
    }

    public class WoMgmtItem
    {
        [DataMember(Order = 10)]
        public int WOManagementID { get; set; }

        [DataMember(Order = 20)]
        public string WOStatus { get; set; }

        [DataMember(Order = 30)]
        public DateTime? ActionDateTime { get; set; }

        [DataMember(Order = 40)]
        public decimal WONumber { get; set; }

        [DataMember(Order = 42)]
        public string WODocType { get; set; }

        [DataMember(Order = 50)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 50)]
        public DateTime ScheduleDate { get; set; }

        [DataMember(Order = 60)]
        public string E1BatchID { get; set; }

        [DataMember(Order = 70)]
        public string UserID { get; set; }

        [DataMember(Order = 80)]
        public string Workstation { get; set; }

        [DataMember(Order = 82)]
        public string Process { get; set; }

        [DataMember(Order = 90)]
        public bool DoNotTransfer { get; set; }

        [DataMember(Order = 100)]
        public bool Transferred { get; set; }

        [DataMember(Order = 110)]
        public DateTime? TransferredDate { get; set; }

        [DataMember(Order = 120)]
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
