using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class MachinePartConsumedItems : List<MachinePartConsumedItem>
    {
    }

    public class MachinePartConsumedItem
    {

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public Int64 ConsumedId { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string ItemDesc { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public int Quantity { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string UnitMeasure { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string Location { get; set; }

        [DataMember(Order = 25, EmitDefaultValue = false)]
        public string E1Location { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public Int64 TransactionId { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public DateTime TransactionDate { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime WrittenDate { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string WrittenBy { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }

        [DataMember(Order = 202, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }

        [DataMember(Order = 204, EmitDefaultValue = false)]
        public string TransferMessage { get; set; }

        [DataMember(Order = 206, EmitDefaultValue = false)]
        public long TransferBatchNum { get; set; }

        [DataMember(Order = 208, EmitDefaultValue = false)]
        public long TransferId { get; set; }

    }
}
