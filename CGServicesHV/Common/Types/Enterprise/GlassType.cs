using System;
using System.Runtime.Serialization;
using System.Text;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class GlassType
    {
        public GlassType()
        {
            Thickness = 0;
            Color = "";
            ColorCode = "";
            HeatTreatment = "";
            HeatTreatmentCode = "";
            Surface1Coating = "";
            Surface1CoatingCode = "";
            Surface2Coating = "";
            Surface2CoatingCode = "";
            PatternTexture = "";
            PatternTextureCode = "";
            LamiProductTypeCode = "";
            LamiProductTypeDesc = "";
        }

        [DataMember(Order = 10, EmitDefaultValue = true)]
        public decimal Thickness { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = true)]
        public string Color { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = true)]
        public string ColorCode { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string ColorDesc { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = true)]
        public string HeatTreatment { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = true)]
        public string HeatTreatmentCode { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string HeatTreatmentDesc { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = true)]
        public string Surface1Coating { get; set; }

        [DataMember(Order = 42, EmitDefaultValue = true)]
        public string Surface1CoatingCode { get; set; }

        [DataMember(Order = 44, EmitDefaultValue = false)]
        public string Surface1CoatingDesc { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = true)]
        public string Surface2Coating { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = true)]
        public string Surface2CoatingCode { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public string Surface2CoatingDesc { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = true)]
        public string PatternTexture { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = true)]
        public string PatternTextureCode { get; set; }

        [DataMember(Order = 64, EmitDefaultValue = false)]
        public string PatternTextureDesc { get; set; }

        [DataMember(Order = 66, EmitDefaultValue = false)]
        public decimal PatternTextureSrf { get; set; }

        [DataMember(Order = 68, EmitDefaultValue = false)]
        public string PatternTextureSrfDesc { get; set; }

        // Lami Attributes
        [DataMember(Order = 70, EmitDefaultValue = true)]
        public string LamiProductType { get; set; }

        [DataMember(Order = 72, EmitDefaultValue = true)]
        public string LamiProductTypeCode { get; set; }

        [DataMember(Order = 74, EmitDefaultValue = false)]
        public string LamiProductTypeDesc { get; set; }

        [DataMember(Order = 76, EmitDefaultValue = false)]
        public decimal LamiColorSrf { get; set; }

        [DataMember(Order = 78, EmitDefaultValue = false)]
        public string LamiColorSrfDesc { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public String LamiMixedTreatment { get; set; }

        [DataMember(Order = 82, EmitDefaultValue = false)]
        public String LamiPatternSurface { get; set; }

        [DataMember(Order = 84, EmitDefaultValue = false)]
        public String LamiCoatingSurface { get; set; }

        [DataMember(Order = 86, EmitDefaultValue = false)]
        public String LamiColorSurface { get; set; }

        [DataMember(Order = 88, EmitDefaultValue = false)]
        public Decimal LamiThickness1 { get; set; }

        [DataMember(Order = 90, EmitDefaultValue = false)]
        public Decimal LamiInnerLayerThickness { get; set; }

        [DataMember(Order = 92, EmitDefaultValue = false)]
        public Decimal LamiThickness2 { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public string TempLogo { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public bool CustomerSupplied { get; set; }

        //[DataMember(Order = 90, EmitDefaultValue = false)]
        //public string MakeupCode { get; set; }

        public string GetMakeupCode()
        {
            string code = string.Empty;
            try
            {
                var sb = new StringBuilder();
                sb.Append(Thickness.ToString());
                sb.Append(":");
                sb.Append(Surface1Coating);
                sb.Append(":");
                sb.Append((String.IsNullOrEmpty(Color)) ? "CL" : Color);
                sb.Append(":");
                sb.Append(HeatTreatment);
                sb.Append(":");
                sb.Append(PatternTexture);
                sb.Append(":");
                sb.Append(LamiProductType);
                sb.Append(":");
                sb.Append(Surface2Coating);
                code = sb.ToString();
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(e.Message);
            }
            return code;
        }
    }
}