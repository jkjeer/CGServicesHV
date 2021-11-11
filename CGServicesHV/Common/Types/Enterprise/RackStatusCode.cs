using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackStatusCodes : List<RackStatusCode>
    {
    }

    [DataContract(Namespace = "")]
    public class RackStatusCode : UdcItem
    {
    }
}
