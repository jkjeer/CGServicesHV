using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;


namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitDocumentTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("ConfigurationId")).Max(); }
            set { }
        }


        public SpecUnitDocumentTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitDocumentLedger]");
            Columns.Add("ConfigurationId", typeof(decimal));
            Columns.Add("DocumentNum", typeof(decimal));
            Columns.Add("DocumentType", typeof(string));
            Columns.Add("DocumentLineNum", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(SpecUnitDocuments specUnitDocuments, Plant plant)
        {
            try
            {
                foreach (var specUnitDocument in specUnitDocuments)
                {
                    DataRow dr = NewRow();
                    dr["ConfigurationId"] = specUnitDocument.ConfigurationId;
                    dr["DocumentNum"] = specUnitDocument.DocumentNum;
                    dr["DocumentType"] = specUnitDocument.DocumentType;
                    dr["DocumentLineNum"] = specUnitDocument.DocumentLineNum;
                    dr["TransactionDate"] = specUnitDocument.TransactionDate;
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


    }
}