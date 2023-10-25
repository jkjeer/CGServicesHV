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
        [DataMember(Order = 2)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 10)]
        public string Name { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public decimal Pel { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string NameDesc { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string DataType { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public decimal Precision { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string Scale { get; set; }

        [DataMember(Order = 36, EmitDefaultValue = false)]
        public string Numeric { get; set; }

        [DataMember(Order = 50)]
        public string Value { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string Udc1 { get; set; }

        [DataMember(Order = 61, EmitDefaultValue = false)]
        public string Udc2 { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string Desc1 { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = false)]
        public string Desc2 { get; set; }

        [DataMember(Order = 64, EmitDefaultValue = false)]
        public string Desc3 { get; set; }

        [DataMember(Order = 70)]
        public DateTime TransactionDate { get; set; }
    }
}