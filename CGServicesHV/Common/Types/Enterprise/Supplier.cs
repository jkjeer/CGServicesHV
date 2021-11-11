using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Suppliers : List<Supplier>
    {
    }

    public class Supplier
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal SupplierNum { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string SupplierType { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string LegacyCustomerNum { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string SupplierName { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public bool IsPlant { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public string PlantCode { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string ShortCompanyNum { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string CompanyNum { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public int CTSupplierNum { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string TaxRateCode { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public Address Address { get; set; }
    }
}