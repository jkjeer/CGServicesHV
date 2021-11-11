using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class InvoiceLines : List<InvoiceLine>
    {
        public decimal GrossAmount
        {
            get
            {
                decimal amt = 0;
                foreach (var invoice in this)
                {
                    amt = amt + invoice.GrossAmt;
                }
                return amt;
            }
        }

        public decimal OpenAmount
        {
            get
            {
                decimal amt = 0;
                foreach (var invoice in this)
                {
                    amt = amt + invoice.OpenAmt;
                }
                return amt;
            }
        }

        public decimal TaxAmount
        {
            get
            {
                decimal amt = 0;
                foreach (var invoice in this)
                {
                    if (invoice.GlCategory == "TAX")
                    {
                        amt = amt + invoice.OpenAmt;
                    }
                }
                return amt;
            }
        }

        //public decimal GrossAmount()
        //{
        //    decimal amt = 0;
        //    foreach (InvoiceLine invoice in this)
        //    {
        //        amt = amt + invoice.GrossAmt;
        //    }
        //    return amt;
        //}

        //public decimal OpenAmount()
        //{
        //    decimal amt = 0;
        //    foreach (InvoiceLine invoice in this)
        //    {
        //        amt = amt + invoice.OpenAmt;
        //    }
        //    return amt;
        //}
    }

    [DataContract(Namespace = "")]
    public class InvoiceLine : ICloneable
    {
        [DataMember(Order = 10)]
        public int InvoiceId { get; set; }

        [DataMember(Order = 20)]
        public decimal OrderNum { get; set; }
        [DataMember(Order = 22)]
        public string OrderType { get; set; }
        [DataMember(Order = 24)]
        public decimal OrderLineNum { get; set; }

        [DataMember(Order = 30)]
        public decimal OrderQty { get; set; }
        [DataMember(Order = 34)]
        public decimal GrossAmt { get; set; }
        [DataMember(Order = 36)]
        public decimal OpenAmt { get; set; }

        [DataMember(Order = 40)]
        public int ProductId { get; set; }
        [DataMember(Order = 42)]
        public string ProductCode { get; set; }
        [DataMember(Order = 44)]
        public string ProductDesc { get; set; }

        [DataMember(Order = 50)]
        public string GlCategory { get; set; }
        [DataMember(Order = 52)]
        public string GlAccount { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
