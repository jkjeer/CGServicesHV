using System;

namespace CGServicesHV.Common.Utilities
{
    public static class EnumValidator
    {
        public static TEnum GetEnum<TEnum>(string value, bool caseInsensitive = true, bool useDefaultValue = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            TEnum lResult;
            if (Enum.TryParse(value, caseInsensitive, out lResult))
            {
                return lResult;
            }
            else
            {
                if (useDefaultValue)
                {
                    return default(TEnum);
                }
                else
                {
                    throw new Exception(
                        $"Enum [{typeof(TEnum).FullName}] Value [{value}] NOT Defined ... Valid Values are {string.Join(",", Enum.GetNames(typeof(TEnum)))} ");
                }
            }
        }
    }
}



