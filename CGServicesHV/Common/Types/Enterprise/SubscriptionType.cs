using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using CGServicesHV.Common.Managers.Process;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class SubscriptionTypes : List<SubscriptionType>
    {
    }

    [DataContract(Namespace = "")]
    public class SubscriptionType
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string SubscriptionTypeCode { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string ClassName { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string NamespaceName { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string TypeName { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string MethodName { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string Arguments { get; set; }
        [XmlIgnore]
        public Dictionary<string, object> ArgumentsHash { get; set; }
        [XmlIgnore]
        public BusinessSystemTransferProcessor.ProcessDelegate DelegateMethod { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string Resources { get; set; }
        [XmlIgnore]
        public List<string> ResourceList { get; set; }
    }

    [CollectionDataContract(Namespace = "")]
    public class SubscriptionTypeItems : List<SubscriptionTypeItem>
    {
    }

    [DataContract(Namespace = "")]
    public class SubscriptionTypeItem
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string GroupCode { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public SubscriptionType SubscriptionType { get; set; }
    }
}
