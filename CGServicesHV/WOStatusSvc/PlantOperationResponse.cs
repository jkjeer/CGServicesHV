using System;
using System.Runtime.Serialization;
using CGServicesHV.Common.Types;
using CGServicesHV.Common.Types.Response;

namespace CGServicesHV.WOStatusSvc
{
    [DataContract(Namespace = "")]
    public class PlantOperationResponse
    {

        [DataMember(Order = 10, EmitDefaultValue = true)]
        public string PlantNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = true)]
        public bool IsActive { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = true)]
        public bool IsSuspended { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = true)]
        public int PendingUpdates { get; set; }

        [DataMember(Order = 42, EmitDefaultValue = false)]
        public SqlBulkCopyResponse DestinationPersistence { get; set; }

        [DataMember(Order = 44, EmitDefaultValue = false)]
        public SqlDmlResponse SourcePersistence { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public DateTime StartDate { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = false)]
        public DateTime EndDate { get; set; }

    }
}
