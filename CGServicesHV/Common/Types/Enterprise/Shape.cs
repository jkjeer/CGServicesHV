using System;
using System.Runtime.Serialization;
using System.Text;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class Shape
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal ShapeNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public Dimension BaseDimension { get; set; }

        [DataMember(Order = 21, EmitDefaultValue = false)]
        public Dimension LeftDimension { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public Dimension RightDimension { get; set; }

        [DataMember(Order = 23, EmitDefaultValue = false)]
        public Dimension TopDimension { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public Dimension Side1Dimension { get; set; }

        [DataMember(Order = 25, EmitDefaultValue = false)]
        public Dimension Side2Dimension { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal ActualSqft { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal BillableSqft { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal BlockSizePerimeter { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal BlockSizeHeight { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal BlockSizeWidth { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal BlockSizeShort { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal BlockSizeLong { get; set; }

        public string GetDimensionDesc()
        {
            string code = string.Empty;
            try
            {
                var sb = new StringBuilder();
                sb.Append(BaseDimension.Description);
                sb.Append(" X ");
                sb.Append(LeftDimension.Description);
                if (RightDimension.DecimalValue > 0)
                {
                    sb.Append(" X ");
                    sb.Append(RightDimension.Description);
                }
                if (TopDimension.DecimalValue > 0)
                {
                    sb.Append(" X ");
                    sb.Append(TopDimension.Description);
                }
                if (Side1Dimension.DecimalValue > 0)
                {
                    sb.Append(" X ");
                    sb.Append(Side1Dimension.Description);
                }
                if (Side2Dimension.DecimalValue > 0)
                {
                    sb.Append(" X ");
                    sb.Append(Side2Dimension.Description);
                }
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