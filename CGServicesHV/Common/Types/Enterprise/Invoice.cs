using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Invoices : List<Invoice>
    {
    }

    [DataContract(Namespace = "")]
    public class Invoice
    {
        [DataMember(Order = 10)]
        public int InvoiceId { get; set; }
        [DataMember(Order = 12)]
        public decimal InvoiceNum { get; set; }
        [DataMember(Order = 14)]
        public string InvoiceType { get; set; }
        public bool IsCredit
        {
            get
            {
                var creditTypes = new string[] { "CO", "CT" };
                return (creditTypes.Contains(this.InvoiceType));
            }
        }
        public bool IsExternal
        {
            get
            {
                var externalTypes = new string[] { "CO", "SO" };
                return (externalTypes.Contains(this.InvoiceType));
            }
        }
        [DataMember(Order = 16)]
        public DateTime InvoiceDate { get; set; }
        [DataMember(Order = 16)]
        public DateTime OriginalInvoiceDate { get; set; }
        [DataMember(Order = 17)]
        public DateTime DiscountDate { get; set; }
        [DataMember(Order = 18)]
        public DateTime DueDate { get; set; }
        [DataMember(Order = 19)]
        public int InvoiceStatus { get; set; }

        //[DataMember(Order = 20)]
        //public string PlantNum { get; set; }
        [DataMember(Order = 22)]
        public Plant Plant { get; set; }

        //[DataMember(Order = 30)]
        //public decimal SoldToCustomerNum { get; set; }
        [DataMember(Order = 32)]
        public Customer SoldToCustomer { get; set; }
        [DataMember(Order = 34)]
        //public decimal ShipToCustomerNum { get; set; }
        //[DataMember(Order = 36)]
        public Customer ShipToCustomer { get; set; }
        [DataMember(Order = 38)]
        public bool IsInternalCustomer { get; set; }

        [DataMember(Order = 50)]
        public string Currency { get; set; }
        [DataMember(Order = 52)]
        public Decimal GrossAmt { get; set; }
        [DataMember(Order = 54)]
        public Decimal DiscountAmt { get; set; }
        [DataMember(Order = 56)]
        public Decimal ReceivedAmt { get; set; }
        [DataMember(Order = 58)]
        public Decimal OpenAmt { get; set; }

        [DataMember(Order = 60)]
        public string Terms { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string TransferBatchNum { get; set; }
        [DataMember(Order = 72, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 74, EmitDefaultValue = false)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 76, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }

        [DataMember(Order = 100)]
        public InvoiceLines InvoiceLines { get; set; }
        [DataMember(Order = 200)]
        public JournalEntrys JournalEntrys { get; set; }

        [DataMember(Order = 300)]
        public bool IsBatchInvoice { get; set; }
        [DataMember(Order = 302)]
        public BatchInvoices BatchInvoices { get; set; }
        [DataMember(Order = 400)]
        public BatchJournalEntrys BatchJournalEntrys { get; set; }
    }
}