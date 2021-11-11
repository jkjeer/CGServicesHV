using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Response
{
    [CollectionDataContract(Namespace = "")]
    public class BusinessObjects : List<BusinessObject>
    {
    }

    [DataContract(Namespace = "")]
    public class BusinessObject
    {
        public BusinessObject()
        {
        }

        public BusinessObject(string name, object value, System.Type type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        [DataMember(Order = 10)]
        public string Name { get; set; }
        [DataMember(Order = 20)]
        public object Value { get; set; }
        [DataMember(Order = 30)]
        public System.Type Type { get; set; }
    }
}
