using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.Response
{
    [CollectionDataContract(Namespace = "")]
    public class SerializedObjects : List<SerializedObject>
    {
        public void Add<T>(string name, T obj)
        {
            var x = new SerializedObject(name, XmlSerializationUtility.SerializeObject(Encoding.UTF8, obj));
            x.Value = XmlSerializationUtility.SerializeObject(Encoding.UTF8, obj);
            Add(x);
        }

        public void Add(string name, object obj, string collectionName = null)
        {
            var x = new SerializedObject(name);
            x.Value = XmlSerializationUtility.SerializeAnonymousType(obj, name, collectionName);
            Add(x);
        }

        public void Add(string name, object obj, string collectionName, string itemName)
        {
            var x = new SerializedObject(name);
            x.Value = XmlSerializationUtility.SerializeAnonymousType(obj, itemName, collectionName);
            Add(x);
        }

    }

    [DataContract(Namespace = "")]
    public class SerializedObject
    {
        public SerializedObject()
        {
        }

        public SerializedObject(string name)
        {
            Name = name;
        }

        public SerializedObject(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [DataMember(Order = 10)]
        public string Name { get; set; }
        [DataMember(Order = 20)]
        public string Value { get; set; }
    }
}
