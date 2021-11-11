using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class InventoryLocations : List<InventoryLocation>
    {
    }

    [DataContract(Namespace = "")]
    public class InventoryLocation
    {
        [DataMember(Order = 1)]
        public string LocationCode { get; set; }
        [DataMember(Order = 2)]
        public string PlantNum { get; set; }

        [DataMember(Order = 10)]
        public string Aisle { get; set; }
        [DataMember(Order = 11)]
        public string Bin { get; set; }
        [DataMember(Order = 12)]
        public string DetailLevel { get; set; }
        [DataMember(Order = 13)]
        public string PutawayZone { get; set; }
        [DataMember(Order = 14)]
        public string Category03 { get; set; }
        [DataMember(Order = 15)]
        public string Category04 { get; set; }
        [DataMember(Order = 16)]
        public string StagingFlag { get; set; }
        [DataMember(Order = 100)]
        public DateTime UpdateDateTime { get; set; }

        public decimal UpdateDateId { get; set; }
        public decimal UpdateTimeId { get; set; }
    }
}

