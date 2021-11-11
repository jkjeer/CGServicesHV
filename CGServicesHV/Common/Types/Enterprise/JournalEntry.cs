using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class JournalEntrys : List<JournalEntry>
    {
    }

    [DataContract(Namespace = "")]
    public class JournalEntry
    {
        [DataMember(Order = 10)]
        public string CostCenter { get; set; }
        [DataMember(Order = 11)]
        public string CompanyNum { get; set; }
        [DataMember(Order = 12)]
        public string ShortCompanyNum { get; set; }
        [DataMember(Order = 13)]
        public decimal DocumentNum { get; set; }
        [DataMember(Order = 14)]
        public string DocumentType { get; set; }
        [DataMember(Order = 15)]
        public decimal DocumentLineNum { get; set; }
        [DataMember(Order = 16)]
        public string TransactionType { get; set; }
        [DataMember(Order = 18)]
        public string BatchType { get; set; }
        [DataMember(Order = 18)]
        public string EdiUserId { get; set; }

        [DataMember(Order = 20)]
        public string OrderType { get; set; }
        [DataMember(Order = 22)]
        public string CategoryCode { get; set; }
        [DataMember(Order = 23)]
        public string TaxCode { get; set; }
        [DataMember(Order = 24)]
        public string Account { get; set; }
        [DataMember(Order = 25)]
        public AaiAccount AaiAccount { get; set; }
        [DataMember(Order = 26)]
        public string Subsidiary { get; set; }
        [DataMember(Order = 28)]
        public string Subledger { get; set; }
        [DataMember(Order = 30)]
        public string SubledgerType { get; set; }

        [DataMember(Order = 40)]
        public DateTime JournalDate { get; set; }
        [DataMember(Order = 42)]
        public decimal Amount { get; set; }

        [DataMember(Order = 50)]
        public string BatchNum { get; set; }
        [DataMember(Order = 60)]
        public decimal CustomerNum { get; set; }
    }
}
