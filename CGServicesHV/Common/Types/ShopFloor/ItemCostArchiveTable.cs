using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ItemCostArchiveTable : DataTable
    {

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("TransactionId")).Max(); }
            set { }
        }

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("LedgerDate")).Max(); }
            set { }
        }

        public ItemCostArchiveTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemCostArchiveLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("Location", typeof(string));
            Columns.Add("Lot", typeof(string));
            Columns.Add("LedgerTypeCode", typeof(string));
            Columns.Add("UnitCostAmt", typeof(decimal));
            Columns.Add("PurchasingSelectionCode", typeof(string));
            Columns.Add("InventorySelectionCode", typeof(string));
            Columns.Add("CostChangedCode", typeof(string));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("LedgerDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ItemNum"] = reader.Get<decimal>("CLITM");
                    dr["Location"] = reader.Get<string>("CLLOCN");
                    dr["Lot"] = reader.Get<string>("CLLOTN");
                    dr["LedgerTypeCode"] = reader.Get<string>("CLLEDG");
                    dr["UnitCostAmt"] = reader.Get<decimal>("CLUNCS") / 10000;
                    dr["PurchasingSelectionCode"] = reader.Get<string>("CLCSPO");
                    dr["InventorySelectionCode"] = reader.Get<string>("CLCSIN");
                    dr["CostChangedCode"] = reader.Get<string>("CLCCFL");
                    dr["UserId"] = reader.Get<string>("CLUSER");
                    dr["ProgramId"] = reader.Get<string>("CLPID");
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(reader.Get<decimal>("CLUPMJ"), reader.Get<decimal>("CLTDAY")).AddMinutes(plant.BusinessSystemOffset);
                    dr["TransactionId"] = reader.Get<decimal>("CLUKID");
                    dr["LedgerDate"] = Utilities.Common.CastToDateTime(reader.Get<decimal>("CLMUPM"), reader.Get<decimal>("CLUPMT")).AddMinutes(plant.BusinessSystemOffset);
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
