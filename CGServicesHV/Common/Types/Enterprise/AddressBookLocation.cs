using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class AddressBookLocations : List<AddressBookLocation>
    {
    }

    [DataContract(Namespace = "")]
    public class AddressBookLocation
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public decimal ParentAddressNum { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public decimal Company { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string AddressType { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string AddressTypeName { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string Address1 { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public string Address2 { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string Address3 { get; set; }
        [DataMember(Order = 25, EmitDefaultValue = false)]
        public string Address4 { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string PostalCode { get; set; }
        [DataMember(Order = 27, EmitDefaultValue = false)]
        public string City { get; set; }
        [DataMember(Order = 28, EmitDefaultValue = false)]
        public string State { get; set; }
        [DataMember(Order = 29, EmitDefaultValue = false)]
        public string Country { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string AreaCode { get; set; }
        [DataMember(Order = 41, EmitDefaultValue = false)]
        public string Phone { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string NationalAccountGroup { get; set; }
        [DataMember(Order = 51, EmitDefaultValue = false)]
        public string NationalAccountGroupName { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDateTime { get; set; }
        public decimal UpdateDateId { get; set; }
        public decimal UpdateTimeId { get; set; }
    }
}
