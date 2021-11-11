using System.Runtime.Serialization;

namespace CGServicesHV.WOStatusSvc
{
    [DataContract(Namespace = "")]
    public class PendingUpdatesResult
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public int Updates { get; set; }
    }
}
