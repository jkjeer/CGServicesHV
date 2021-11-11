using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable once InconsistentNaming

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F4715s : List<F4715>
    {
    }

    [DataContract(Namespace = "")]
    public class F4715
    {

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string ZDEDTY { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal ZDEDSQ { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string ZDEKCO { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal ZDEDOC { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string ZDEDCT { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal ZDEDLN { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string ZDEDSP { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string ZDEDBT { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string ZDFILE { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string ZDKCOO { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal ZDDOCO { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string ZDDCTO { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal ZDLNID { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string ZDPNTC { get; set; }
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public decimal ZDLINS { get; set; }
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string ZDTXLN { get; set; }
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public string ZDTORG { get; set; }
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public string ZDUSER { get; set; }
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public string ZDPID { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public string ZDJOBN { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public decimal ZDUPMJ { get; set; }
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public decimal ZDTDAY { get; set; }
    }

    [CollectionDataContract(Namespace = "")]
    public class F4715Pivots : List<F4715Pivot>
    {
    }

    [DataContract(Namespace = "")]
    public class F4715Pivot
    {
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string ZDKCOO { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal ZDDOCO { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string ZDDCTO { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal ZDLNID { get; set; }

        [DataMember(Order = 201, EmitDefaultValue = false)]
        public string L1 { get; set; }
        [DataMember(Order = 202, EmitDefaultValue = false)]
        public string L2 { get; set; }
        [DataMember(Order = 203, EmitDefaultValue = false)]
        public string L3 { get; set; }
        [DataMember(Order = 204, EmitDefaultValue = false)]
        public string L4 { get; set; }
        [DataMember(Order = 205, EmitDefaultValue = false)]
        public string L5 { get; set; }
        [DataMember(Order = 206, EmitDefaultValue = false)]
        public string L6 { get; set; }
        [DataMember(Order = 207, EmitDefaultValue = false)]
        public string L7 { get; set; }
        [DataMember(Order = 208, EmitDefaultValue = false)]
        public string L8 { get; set; }
        [DataMember(Order = 209, EmitDefaultValue = false)]
        public string L9 { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public string L10 { get; set; }
        [DataMember(Order = 211, EmitDefaultValue = false)]
        public string L11 { get; set; }
        [DataMember(Order = 212, EmitDefaultValue = false)]
        public string L12 { get; set; }
        [DataMember(Order = 213, EmitDefaultValue = false)]
        public string L13 { get; set; }
        [DataMember(Order = 214, EmitDefaultValue = false)]
        public string L14 { get; set; }
        [DataMember(Order = 215, EmitDefaultValue = false)]
        public string L15 { get; set; }
        [DataMember(Order = 216, EmitDefaultValue = false)]
        public string L16 { get; set; }
        [DataMember(Order = 217, EmitDefaultValue = false)]
        public string L17 { get; set; }
        [DataMember(Order = 218, EmitDefaultValue = false)]
        public string L18 { get; set; }
        [DataMember(Order = 219, EmitDefaultValue = false)]
        public string L19 { get; set; }
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public string L20 { get; set; }
        [DataMember(Order = 221, EmitDefaultValue = false)]
        public string L21 { get; set; }
        [DataMember(Order = 222, EmitDefaultValue = false)]
        public string L22 { get; set; }
        [DataMember(Order = 223, EmitDefaultValue = false)]
        public string L23 { get; set; }
        [DataMember(Order = 224, EmitDefaultValue = false)]
        public string L24 { get; set; }
        [DataMember(Order = 225, EmitDefaultValue = false)]
        public string L25 { get; set; }
        [DataMember(Order = 226, EmitDefaultValue = false)]
        public string L26 { get; set; }
        [DataMember(Order = 227, EmitDefaultValue = false)]
        public string L27 { get; set; }
        [DataMember(Order = 228, EmitDefaultValue = false)]
        public string L28 { get; set; }
        [DataMember(Order = 229, EmitDefaultValue = false)]
        public string L29 { get; set; }
        [DataMember(Order = 230, EmitDefaultValue = false)]
        public string L30 { get; set; }

        [DataMember(Order = 301, EmitDefaultValue = false)]
        public string E1 { get; set; }
        [DataMember(Order = 302, EmitDefaultValue = false)]
        public string E2 { get; set; }
        [DataMember(Order = 303, EmitDefaultValue = false)]
        public string E3 { get; set; }
        [DataMember(Order = 304, EmitDefaultValue = false)]
        public string E4 { get; set; }
        [DataMember(Order = 305, EmitDefaultValue = false)]
        public string E5 { get; set; }
        [DataMember(Order = 306, EmitDefaultValue = false)]
        public string E6 { get; set; }
        [DataMember(Order = 307, EmitDefaultValue = false)]
        public string E7 { get; set; }
        [DataMember(Order = 308, EmitDefaultValue = false)]
        public string E8 { get; set; }
        [DataMember(Order = 309, EmitDefaultValue = false)]
        public string E9 { get; set; }
        [DataMember(Order = 310, EmitDefaultValue = false)]
        public string E10 { get; set; }
        [DataMember(Order = 311, EmitDefaultValue = false)]
        public string E11 { get; set; }
        [DataMember(Order = 312, EmitDefaultValue = false)]
        public string E12 { get; set; }
        [DataMember(Order = 313, EmitDefaultValue = false)]
        public string E13 { get; set; }
        [DataMember(Order = 314, EmitDefaultValue = false)]
        public string E14 { get; set; }
        [DataMember(Order = 315, EmitDefaultValue = false)]
        public string E15 { get; set; }
        [DataMember(Order = 316, EmitDefaultValue = false)]
        public string E16 { get; set; }
        [DataMember(Order = 317, EmitDefaultValue = false)]
        public string E17 { get; set; }
        [DataMember(Order = 318, EmitDefaultValue = false)]
        public string E18 { get; set; }
        [DataMember(Order = 319, EmitDefaultValue = false)]
        public string E19 { get; set; }
        [DataMember(Order = 320, EmitDefaultValue = false)]
        public string E20 { get; set; }
        [DataMember(Order = 321, EmitDefaultValue = false)]
        public string E21 { get; set; }
        [DataMember(Order = 322, EmitDefaultValue = false)]
        public string E22 { get; set; }
        [DataMember(Order = 323, EmitDefaultValue = false)]
        public string E23 { get; set; }
        [DataMember(Order = 324, EmitDefaultValue = false)]
        public string E24 { get; set; }
        [DataMember(Order = 325, EmitDefaultValue = false)]
        public string E25 { get; set; }
        [DataMember(Order = 326, EmitDefaultValue = false)]
        public string E26 { get; set; }
        [DataMember(Order = 327, EmitDefaultValue = false)]
        public string E27 { get; set; }
        [DataMember(Order = 328, EmitDefaultValue = false)]
        public string E28 { get; set; }
        [DataMember(Order = 329, EmitDefaultValue = false)]
        public string E29 { get; set; }
        [DataMember(Order = 330, EmitDefaultValue = false)]
        public string E30 { get; set; }
    }
}

