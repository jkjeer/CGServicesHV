using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Lites : List<Lite>
    {
    }

    [DataContract(Namespace = "")]
    public class Lite
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public int Position { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string PositionName { get; set; }

        [DataMember(Order = 6, EmitDefaultValue = false)]
        public String RoutingCode { get; set; }

        [DataMember(Order = 7, EmitDefaultValue = false)]
        public String RoutingDesc { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public GlassType GlassType { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public Shape Shape { get; set; }

        // Quality Level
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public String QualityLevel { get; set; }

        [DataMember(Order = 41, EmitDefaultValue = false)]
        public String QualityLevelDesc { get; set; }

        // VGroove 
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public String VGrooveType { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public String VGrooveTypeDesc { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public String VGrooveStyle { get; set; }

        [DataMember(Order = 56, EmitDefaultValue = false)]
        public String VGrooveStyleDesc { get; set; }

        [DataMember(Order = 58, EmitDefaultValue = false)]
        public Decimal VGrooveSurface { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public String VGrooveDrawing { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = false)]
        public Decimal VGrooveHorzBars { get; set; }

        [DataMember(Order = 64, EmitDefaultValue = false)]
        public Decimal VGrooveVertBars { get; set; }

        [DataMember(Order = 66, EmitDefaultValue = false)]
        public Decimal VGrooveHighOpen { get; set; }

        [DataMember(Order = 68, EmitDefaultValue = false)]
        public Decimal VGrooveWideOpen { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public Decimal VGrooveTotalOpen { get; set; }

        [DataMember(Order = 72, EmitDefaultValue = false)]
        public String VGrooveBevel { get; set; }

        [DataMember(Order = 74, EmitDefaultValue = false)]
        public String VGrooveVertStartStop { get; set; }

        [DataMember(Order = 76, EmitDefaultValue = false)]
        public String VGrooveHorzStartStop { get; set; }

        [DataMember(Order = 78, EmitDefaultValue = false)]
        public Decimal VGrooveVertOffset { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public Decimal VGrooveHorzOffset { get; set; }

        // Notching 
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public String NotchLocation { get; set; }

        [DataMember(Order = 161, EmitDefaultValue = false)]
        public String NotchLocationDesc { get; set; }

        [DataMember(Order = 162, EmitDefaultValue = false)]
        public Decimal NotchHeight { get; set; }

        [DataMember(Order = 163, EmitDefaultValue = false)]
        public Decimal NotchWidth { get; set; }

        // Holes
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public Decimal HolesNum { get; set; }

        [DataMember(Order = 172, EmitDefaultValue = false)]
        public String HolesDrawing { get; set; }

        // Polish
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public String PolishType { get; set; }

        // Edge Work
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public String EdgeWork { get; set; }

        [DataMember(Order = 192, EmitDefaultValue = false)]
        public String EdgeWorkDesc { get; set; }

        // Edge Delete Width
        [DataMember(Order = 194, EmitDefaultValue = false)]
        public String EdgeDeleteWidthCode { get; set; }

        [DataMember(Order = 196, EmitDefaultValue = false)]
        public Decimal EdgeDeleteWidthValue { get; set; }

        // Silk Screen
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public Decimal SilkScreenSurface { get; set; }

        [DataMember(Order = 202, EmitDefaultValue = false)]
        public String SilkScreenDrawing { get; set; }

        // Etching
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public String EtchingDrawing { get; set; }

        // Lami
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public String LamiMixedTreatment { get; set; }
        [DataMember(Order = 222, EmitDefaultValue = false)]
        public String LamiColorSrfc { get; set; }
        [DataMember(Order = 224, EmitDefaultValue = false)]
        public Decimal LamiThickness1 { get; set; }
        [DataMember(Order = 226, EmitDefaultValue = false)]
        public Decimal LamiInnerThickness { get; set; }
        [DataMember(Order = 228, EmitDefaultValue = false)]
        public Decimal LamiThickness2 { get; set; }
        [DataMember(Order = 230, EmitDefaultValue = false)]
        public String LamiPatternSrfc { get; set; }
        [DataMember(Order = 232, EmitDefaultValue = false)]
        public String LamiCoatingSrfc { get; set; }

        // Raw Picks
        [DataMember(Order = 250, EmitDefaultValue = false)]
        public String RawPickLocation { get; set; }

        [DataMember(Order = 252, EmitDefaultValue = false)]
        public Decimal RawPickItemNum { get; set; }

        [DataMember(Order = 260, EmitDefaultValue = false)]
        public Decimal DiscretePartNum { get; set; }
    }
}