using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class Surface
    {
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string PreserveCutback { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string PreserveColor { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string Opacifier { get; set; }

        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string OpacifierDesc { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal ScoringWidth { get; set; }

    }
}