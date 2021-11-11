using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    public class E1ScheduleData
    {
        public E1ScheduleData()
        {
            F580020 = new F580020();
            F580021s = new F580021s();
            F580024s = new F580024s();
            F584715Ls = new F584715s();
            F584715Es = new F584715s();
            F580220s = new F580220s();
            F4715s = new F4715s();
            F4715Pivots = new F4715Pivots();
        }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public F580020 F580020 { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public F580021s F580021s { get; set; }

        [DataMember(Order = 120, EmitDefaultValue = false)]
        public F580024s F580024s { get; set; }

        [DataMember(Order = 130, EmitDefaultValue = false)]
        public F584715s F584715Ls { get; set; }

        [DataMember(Order = 140, EmitDefaultValue = false)]
        public F584715s F584715Es { get; set; }

        [DataMember(Order = 150, EmitDefaultValue = false)]
        public F580220s F580220s { get; set; }

        [DataMember(Order = 160, EmitDefaultValue = false)]
        public F4715s F4715s { get; set; }

        [DataMember(Order = 170, EmitDefaultValue = false)]
        public F4715Pivots F4715Pivots { get; set; }

        //[DataMember(Order = 180, EmitDefaultValue = false)]
        //public F3201s F3201s { get; set; }
    }
}
