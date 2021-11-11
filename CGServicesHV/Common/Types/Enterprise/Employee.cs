using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Employees : List<Employee> 
    {
    }

    [DataContract(Namespace = "")]
    public class Employee
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string HomeCompany { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string HomeCompanyDesc { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string Company { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string HomeCostCenter { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string CostCenter { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string UnionCode { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string PayStatus { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDateTime { get; set; }
    }
}
