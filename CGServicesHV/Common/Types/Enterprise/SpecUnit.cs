using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class SpecUnits : List<SpecUnit>
    {
    }

    [DataContract(Namespace = "")]
    public class SpecUnit
    {
        [DataMember(Order = 10)]
        public decimal Id { get; set; }

        [DataMember(Order = 12)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 14)]
        public string StockType { get; set; }

        [DataMember(Order = 16)]
        public string UnitType { get; set; }

        [DataMember(Order = 20)]
        public decimal GlassWeightLbs { get; set; }

        [DataMember(Order = 50)]
        public DateTime TransactionDate { get; set; }

        [DataMember(Order = 100)]
        public ItemAttributes ItemAttributes { get; set; }

        [DataMember(Order = 200)]
        public List<string> FlexDescriptions { get; set; }

        [DataMember(Order = 202)]
        public string MakeupDesc { get; set; }

        [DataMember(Order = 204)]
        public string DimensionDesc { get; set; }

        [DataMember(Order = 206)]
        public string Lite1GlassTypeDesc { get; set; }

        [DataMember(Order = 208)]
        public string Lite2GlassTypeDesc { get; set; }

        [DataMember(Order = 210)]
        public string Lite3GlassTypeDesc { get; set; }

        [DataMember(Order = 212)]
        public string LiteRoutingDesc { get; set; }

        [DataMember(Order = 214)]
        public string AirspaceDesc { get; set; }

        [DataMember(Order = 216)]
        public string SpacerDesc { get; set; }

        [DataMember(Order = 218)]
        public string BarAssemblyDesc { get; set; }

        [DataMember(Order = 220)]
        public string GridDesc { get; set; }

        [DataMember(Order = 222)]
        public string GridIndexDesc { get; set; }

        [DataMember(Order = 224)]
        public string LogoDesc { get; set; }

        [DataMember(Order = 226)]
        public string PreserveDesc { get; set; }

        [DataMember(Order = 228)]
        public string EdgeProcessingDesc { get; set; }

        [DataMember(Order = 230)]
        public string HoleProcessingDesc { get; set; }

        [DataMember(Order = 240)]
        public DateTime IADate { get; set; }

        [DataMember(Order = 250)]
        public DateTime IBDate { get; set; }

        [DataMember(Order = 250)]
        public DateTime UMDate { get; set; }

        //[DataMember(Order = 12, EmitDefaultValue = false)]
        //public OrderIds OrderIds { get; set; }

        //[DataMember(Order = 14, EmitDefaultValue = false)]
        //public List<string> Orders { get; set; }

        //[DataMember(Order = 20)]
        //public List<string> FlexDescriptions { get; set; }

        //[DataMember(Order = 30)]
        //public GlassItem GlassItem { get; set; }

    }
}
