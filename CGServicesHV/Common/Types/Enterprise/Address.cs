using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Addresses : List<Address>
    {
    }

    [DataContract(Namespace = "")]
    public class Address
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal AddressNum { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string Address1 { get; set; }

        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string Address2 { get; set; }

        [DataMember(Order = 18, EmitDefaultValue = false)]
        public string Address3 { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Address4 { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string Address5 { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string City { get; set; }

        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string State { get; set; }

        [DataMember(Order = 28, EmitDefaultValue = false)]
        public string PostalCode { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Country { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string AddressType { get; set; }

        [DataMember(Order = 51, EmitDefaultValue = false)]
        public decimal Company { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public string NationalAccountGroup { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDateTime { get; set; }
        public decimal UpdateDateId { get; set; }
        public decimal UpdateTimeId { get; set; }
    }
}
