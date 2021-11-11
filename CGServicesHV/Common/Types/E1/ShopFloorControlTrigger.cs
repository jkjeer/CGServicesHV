using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.E1
{
    [DataContract(Namespace = "")]
    public class ShopFloorControlTrigger
    {
        public ShopFloorControlTrigger()
        {
            QAUKID = 0;
            QAEDSP = "";
            QAEDCT = "";
            QAMCU = "";
            QAEDBT = "";
            QAZ58SCN = 0;
            QACRTU = "";
            QACPGM = "";
            QAJOBN = "";
            QACRTDT = 0;
            QACRTM = 0;
            QAUPMB = "";
            QAUPGM = "";
            QAMJOB = "";
            QAUPMJ = 0;
            QAUPMT = 0;
        }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public decimal QAUKID { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string QAEDSP { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string QAEDCT { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string QAMCU { get; set; }
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string QAEDBT { get; set; }
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public decimal QAZ58SCN { get; set; }
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public string QACRTU { get; set; }
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public string QACPGM { get; set; }
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string QAJOBN { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal QACRTDT { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public decimal QACRTM { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string QAUPMB { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string QAUPGM { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string QAMJOB { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public decimal QAUPMJ { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public decimal QAUPMT { get; set; }

    }
}
