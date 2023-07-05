using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.Enterprise;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class AccountBalanceTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public AccountBalanceTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[AccountBalanceLedger]");
            Columns.Add("FiscalYear", typeof(int));
            Columns.Add("FiscalPeriod", typeof(int));
            Columns.Add("AccountId", typeof(string));
            Columns.Add("Subledger", typeof(string));
            Columns.Add("LedgerType", typeof(string));
            Columns.Add("CostCenter", typeof(string));
            Columns.Add("ObjectAccount", typeof(string));
            Columns.Add("Subsidiary", typeof(string));
            Columns.Add("Description", typeof(string));
            Columns.Add("DocumentType", typeof(string));
            Columns.Add("Amount", typeof(decimal));
        }

        public void AddTransactionAggregate(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["FiscalYear"] = (int)(CastDbNull.To<decimal>(reader["GLCTRY"], 0) * 100) + CastDbNull.To<decimal>(reader["GLFY"], 0);
                    dr["FiscalPeriod"] = (int)(CastDbNull.To<decimal>(reader["GLPN"], 0));
                    dr["LedgerType"] = CastDbNull.To(reader["GLLT"], string.Empty).Trim();
                    dr["CostCenter"] = CastDbNull.To(reader["GMMCU"], string.Empty).Trim();
                    dr["AccountId"] = CastDbNull.To(reader["GMAID"], string.Empty).Trim();
                    dr["ObjectAccount"] = CastDbNull.To(reader["GMOBJ"], string.Empty).Trim();
                    dr["Subsidiary"] = CastDbNull.To(reader["GMSUB"], string.Empty).Trim();
                    dr["Description"] = CastDbNull.To(reader["GMDL01"], string.Empty).Trim();
                    dr["Subledger"] = CastDbNull.To(reader["GLSBL"], string.Empty).Trim();
                    dr["DocumentType"] = CastDbNull.To(reader["GLDCT"], string.Empty).Trim();
                    dr["Amount"] = Data.CastDbNull.To<decimal>(reader["GLAA"], 0) / 100;
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
