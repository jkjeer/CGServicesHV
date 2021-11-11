using System;
using System.Runtime.Serialization;
using System.Text;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class Dimension
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal DecimalValue { get; set; }

        [DataMember(Order = 15, EmitDefaultValue = false)]
        public decimal MMValue { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal Base { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal Numerator { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal Denominator { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string Description { get; set; }

        public string GetDescription()
        {
            string result = String.Empty;
            if (DecimalValue > 0)
            {
                var sb = new StringBuilder();
                sb.Append(Base.ToString());
                if (Numerator > 0)
                {
                    sb.Append(" ");
                    sb.Append(Numerator.ToString());
                    sb.Append("/");
                    sb.Append(Denominator.ToString());
                }
                result = sb.ToString();
            }
            return result;
        }
    }
}