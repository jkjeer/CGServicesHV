using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class Plants : List<Plant>
    {
    }

    [DataContract(Namespace = "")]
    public class Plant
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string PlantCode { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public int NumericValue { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string ShortCompanyNum { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string CompanyNum { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string AddressName { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string LegacyAddressNum { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public Address Address { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string TimeZone { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public int TimeOffset { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public string TimeZoneName { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public double BusinessSystemOffset { get; set; }
    }

    [DataContract(Namespace = "")]
    public class PlantGroupItem
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string GroupCode { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string PlantNumericCode { get; set; }
    }

    [CollectionDataContract(Namespace = "")]
    public class PlantGroups : List<PlantGroup>
    {
    }

    [DataContract(Namespace = "")]
    public class PlantGroup
    {

        public PlantGroup()
        {
            Plants = new Plants();
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string GroupCode { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public Plants Plants { get; set; }
    }

    [CollectionDataContract(Namespace = "")]
    public class PlantItems : List<PlantItem>
    {
    }

    [DataContract(Namespace = "")]
    public class PlantItem
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string GroupCode { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public Plant Plant { get; set; }
    }
}
