using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F584715s : List<F584715>
    {
    }

    [DataContract(Namespace = "")]
    public class F584715
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string ZDEDBT { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal ZDEDLN { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string ZDMCU { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal ZDZ58SCN { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal ZDZ58SCSN { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string ZDEDTY { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string ZDKCOO { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public decimal ZDDOCO { get; set; }

        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string ZDDCTO { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal ZDLNID { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string ZDZ58TX01 { get; set; }

        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string ZDZ58TX02 { get; set; }

        [DataMember(Order = 130, EmitDefaultValue = false)]
        public string ZDZ58TX03 { get; set; }

        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string ZDZ58TX04 { get; set; }

        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string ZDZ58TX05 { get; set; }

        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string ZDUSER { get; set; }
    
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public string ZDPID { get; set; }
    
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public string ZDJOBN { get; set; }
    
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public decimal ZDUPMJ { get; set; }
    
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal ZDTDAY { get; set; }
    }
}
      
      
      
      
      
      
      
      
      
      
      