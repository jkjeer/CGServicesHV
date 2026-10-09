using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ItemCostTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemCostTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemCostLedger]");
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
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["COITM"], 0);
                    dr["Location"] = Data.CastDbNull.To<string>(reader["COLOCN"], String.Empty).Trim();
                    dr["Lot"] = Data.CastDbNull.To<string>(reader["COLOTN"], String.Empty).Trim();
                    dr["LedgerTypeCode"] = Data.CastDbNull.To<string>(reader["COLEDG"], String.Empty).Trim();
                    dr["UnitCostAmt"] = Data.CastDbNull.To<decimal>(reader["COUNCS"], 0) / 10000;
                    dr["PurchasingSelectionCode"] = Data.CastDbNull.To<string>(reader["COCSPO"], String.Empty).Trim();
                    dr["InventorySelectionCode"] = Data.CastDbNull.To<string>(reader["COCSIN"], String.Empty).Trim();
                    dr["CostChangedCode"] = Data.CastDbNull.To<string>(reader["COCCFL"], String.Empty).Trim();
                    dr["UserId"] = Data.CastDbNull.To<string>(reader["COUSER"], String.Empty).Trim();
                    dr["ProgramId"] = Data.CastDbNull.To<string>(reader["COPID"], String.Empty).Trim();
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(Data.CastDbNull.To<decimal>(reader["COUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["COTDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
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
