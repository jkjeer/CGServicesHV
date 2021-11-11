using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class EdiMessages : List<EdiMessage>
    {
    }

    [DataContract(Namespace = "")]
    public class EdiMessage
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string EdiType { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal EdiSeqNum { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Message { get; set; }
    }
}
