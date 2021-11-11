using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F580220s : List<F580220>
    {
    }

    [DataContract(Namespace = "")]
    public class F580220
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal CRZ58CRID { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string CRZ58CRTY { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal CRDOCO { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string CRDCTO { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string CRKCOO { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal CRLNID { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal CRUORG { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public decimal CRZ58CRQ { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string CRZ58CRMSG { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal CRDOC1 { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string CRRCTO { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string CRRKCO { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal CRRLLN { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string CREV01 { get; set; }
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string CREV02 { get; set; }
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string CRUSER { get; set; }
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public string CRPID { get; set; }
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public string CRJOBN { get; set; }
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public decimal CRUPMJ { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal CRTDAY { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string DRDL01 { get; set; }
    }
}
