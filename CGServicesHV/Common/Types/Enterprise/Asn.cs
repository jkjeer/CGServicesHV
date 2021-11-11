using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CGServicesHV.Common.Types.Response;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Asns : List<Asn>
    {
    }

    [DataContract(Namespace = "")]
    public class Asn
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public int AsnId { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public decimal AsnNum { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string AsnCharNum { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string PlantCode { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string RecvPlantNum { get; set; }
        [DataMember(Order = 17, EmitDefaultValue = false)]
        public string RecvPlantCode { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public DateTime WrittenDate { get; set; }
        [DataMember(Order = 42, EmitDefaultValue = false)]
        public string WrittenBy { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public AsnLines AsnLines { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public Errors ErrorMessages { get; set; }
    }
}
