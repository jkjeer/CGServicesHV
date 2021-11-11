using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ItemAttributes : KeyedCollection<string, ItemAttribute>
    {
        protected override string GetKeyForItem(ItemAttribute attribute)
        {
            return attribute.Name;
        }

        public decimal GetIntAttributeValue(string attributeName)
        {
            if (Contains(attributeName))
            {
                return Convert.ToInt32(this[attributeName].Value.Trim());
            }
            return 0;
        }

        public decimal GetDecimalAttributeValue(string attributeName)
        {
            if (Contains(attributeName))
            {
                try
                {
                    // return Convert.ToDecimal(this[attributeName].Value.Trim());
                    //return Utilities.Data.ParseStringToDecimal(this[attributeName].Value.Trim());
                    decimal.TryParse(this[attributeName].Value.Trim(), out decimal decVal);
                    return decVal;
                }
                catch (Exception e)
                {
                    throw new FaultException("EXCEPTION " + attributeName + " " + this[attributeName].Value + " " + e.Message);
                }

            }
            return 0;
        }

        public string GetStringAttributeValue(string attributeName, string defaultValue = null)
        {
            if (Contains(attributeName))
            {
                return this[attributeName].Value.Trim();
            }
            return defaultValue;
        }

        public string GetStringAttributeDesc1(string attributeName)
        {
            if (Contains(attributeName))
            {
                return this[attributeName].Desc1.Trim();
            }
            return null;
        }

        public string GetStringAttributeDesc2(string attributeName)
        {
            if (Contains(attributeName))
            {
                return this[attributeName].Desc2.Trim();
            }
            return null;
        }

        public string GetStringAttributeDesc3(string attributeName, string defaultValue = null)
        {
            if (Contains(attributeName))
            {
                return this[attributeName].Desc3.Trim();
            }
            return defaultValue;
        }

    }

    [DataContract(Namespace = "")]
    public class ItemAttribute
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Value { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Desc1 { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string Desc2 { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string Desc3 { get; set; }
    }
}