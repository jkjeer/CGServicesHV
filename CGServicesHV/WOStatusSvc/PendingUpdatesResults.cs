using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.WOStatusSvc
{
    [CollectionDataContract(Namespace = "")]
    public class PendingUpdatesResults : List<PendingUpdatesResult>
    {
    }
}
