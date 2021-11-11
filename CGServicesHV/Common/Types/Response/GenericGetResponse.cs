using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CGServicesHV.Common.Types.Enterprise;

namespace CGServicesHV.Common.Types.Response
{
    [DataContract(Namespace = "")]
    [KnownType("GetKnownTypes")]
    public class GenericGetResponse<T>
    {

        [DataMember(Order = 10)]
        public ServiceContext ServiceContext { get; set; }
        [DataMember(Order = 20)]
        public ConnectionStrings ConnectionStrings { get; set; }
        [DataMember(Order = 30)]
        public List<T> Results { get; set; }

        private static Type[] GetKnownTypes()
        {
            return new[]
                       {
                           typeof (UdcTypes),
                           typeof (UdcItems),
                           typeof (InventoryLocations),
                           typeof (RackTypes),
                           typeof (AddressBookLocations),
                           typeof (Employees),
                           typeof (RejectCodes),
                           typeof (ConfiguratorRules),
                           typeof (List<Employee>)
                       };
        }
    }
}
