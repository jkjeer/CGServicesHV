using System.Runtime.Serialization;
using CGServicesHV.Common.Types.Enterprise;

namespace CGServicesHV.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class InvoiceBatchResponse
    {
        [DataMember(Order = 10)]
        public InvoiceBatch InvoiceBatch { get; set; }

        [DataMember(Order = 22)]
        public Invoices Invoices { get; set; }
        [DataMember(Order = 20)]
        public Invoices ErrorInvoices { get; set; }

        [DataMember(Order = 30)]
        public JournalEntrys JournalEntrys { get; set; }
    }
}
