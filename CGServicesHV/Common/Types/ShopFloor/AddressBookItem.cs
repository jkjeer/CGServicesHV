using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class AddressBookItems : List<AddressBookItem>
    {
    }

    [DataContract(Namespace = "")]
    public class AddressBookItem
    {
        public AddressBookItem()
        {
            AddressNum = 0;
            AddressType = "";
            AddressTypeName = "";
            Name = "";
            Address1 = "";
            Address2 = "";
            Address3 = "";
            Address4 = "";
            PostalCode = "";
            City = "";
            State = "";
            AreaCode = "";
            Phone = "";
            ParentAddressNum = 0;
            Company = 0;
        }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string AddressType { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string AddressTypeName { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string Address1 { get; set; }
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string Address2 { get; set; }
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public string Address3 { get; set; }
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public string Address4 { get; set; }
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string PostalCode { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string City { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string State { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string AreaCode { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string Phone { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public decimal ParentAddressNum { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public decimal Company { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string NationalAccountGroup { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDate { get; set; }
    }
}
