using System;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    [KnownType("GetKnownTypes")]
    public class BaseItem
    {
        private static Type[] GetKnownTypes()
        {
            return new[]
                       {
                           typeof (GlassItem)
                       };
        }
    }
}