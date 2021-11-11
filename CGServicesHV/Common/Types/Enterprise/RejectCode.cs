using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RejectCodes : List<RejectCode>
    {
    }

    [DataContract(Namespace = "")]
    public class RejectCode
    {
        public RejectCode()
        {
            Reject = "";
            RejectType = "";
            RejectDesc = "";
            ProductCode = "";
            UserCode = "";
            Workcenter = "";
            PlantNum = "";
            UpdateDate = DateTime.Now.ToLocalTime();
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Reject { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string RejectType { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string RejectDesc { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string ProductCode { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string UserCode { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Workcenter { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 100)]
        public DateTime UpdateDate { get; set; }
    }
}
