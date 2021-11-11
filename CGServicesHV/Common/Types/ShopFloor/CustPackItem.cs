using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class CustPackItems : List<CustPackItem> 
    {
    }

    public class CustPackItem
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public decimal CustPackId { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal WorkOrderNum { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string SerialNum { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public int ScheduleNum { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Location { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string ProductionLineCode { get; set; }

        [DataMember(Order = 42, EmitDefaultValue = false)]
        public string WorkCenterCode { get; set; }

        [DataMember(Order = 43, EmitDefaultValue = false)]
        public int LineID { get; set; }

        [DataMember(Order = 44, EmitDefaultValue = false)]
        public string ProductionType { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public int Quantity { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public string RackType { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public string BoxSlot { get; set; }

        [DataMember(Order = 56, EmitDefaultValue = false)]
        public bool Backout { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime TransactionDate { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal TransferId { get; set; }

        [DataMember(Order = 201, EmitDefaultValue = false)]
        public decimal TransferBatchId { get; set; }

        [DataMember(Order = 202, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }

        [DataMember(Order = 204, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }

        [DataMember(Order = 206, EmitDefaultValue = false)]
        public string TransferComment { get; set; }

    }
}
