using System;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class InvoiceBatch
    {
        [DataMember(Order = 10)]
        public int BatchNum { get; set; }
        [DataMember(Order = 20)]
        public DateTime BatchDate { get; set; }
        [DataMember(Order = 30)]
        public int Lines { get; set; }
    }
}
