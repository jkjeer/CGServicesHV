using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.Enterprise;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ItemTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("LongItem", typeof(string));
            Columns.Add("ItemNum3", typeof(string));
            Columns.Add("StockType", typeof(string));
            Columns.Add("GlCategory", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("Description2", typeof(string));
            Columns.Add("PrimaryUOM", typeof(string));
            Columns.Add("SecondaryUOM", typeof(string));
            Columns.Add("UOM3", typeof(string));
            Columns.Add("SalesCode1", typeof(string));
            Columns.Add("SalesCode2", typeof(string));
            Columns.Add("SalesCode3", typeof(string));
            Columns.Add("SalesCode4", typeof(string));
            Columns.Add("PurchCode1", typeof(string));
            Columns.Add("PurchCode2", typeof(string));
            Columns.Add("PurchCode3", typeof(string));
            Columns.Add("PurchCode4", typeof(string));
            Columns.Add("ReorderPoint", typeof(decimal));
            Columns.Add("ReorderQuantity", typeof(decimal));
            Columns.Add("VendorNum", typeof(decimal));
            Columns.Add("PlannerNum", typeof(decimal));
            Columns.Add("BuyerNum", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["IMITM"], 0);
                    dr["LongItem"] = Data.CastDbNull.To<string>(reader["IBLITM"], "");
                    dr["ItemNum3"] = Data.CastDbNull.To<string>(reader["IBAITM"], "");
                    dr["StockType"] = CastDbNull.To<string>(reader["IMSTKT"], String.Empty).Trim();
                    dr["GlCategory"] = CastDbNull.To<string>(reader["IMGLPT"], String.Empty).Trim();
                    dr["Description1"] = CastDbNull.To<string>(reader["IMDSC1"], String.Empty).Trim();
                    dr["Description2"] = CastDbNull.To<string>(reader["IMDSC2"], String.Empty).Trim();
                    dr["PrimaryUOM"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
                    dr["SecondaryUOM"] = CastDbNull.To<string>(reader["IMUOM2"], String.Empty).Trim();
                    dr["UOM3"] = reader.Get<string>("IMUOM3");
                    dr["SalesCode1"] = CastDbNull.To<string>(reader["IMSRP1"], String.Empty).Trim();
                    dr["SalesCode2"] = CastDbNull.To<string>(reader["IMSRP2"], String.Empty).Trim();
                    dr["SalesCode3"] = CastDbNull.To<string>(reader["IMSRP3"], String.Empty).Trim();
                    dr["SalesCode4"] = CastDbNull.To<string>(reader["IMSRP4"], String.Empty).Trim();
                    dr["PurchCode1"] = CastDbNull.To<string>(reader["IMPRP1"], String.Empty).Trim();
                    dr["PurchCode2"] = CastDbNull.To<string>(reader["IMPRP2"], String.Empty).Trim();
                    dr["PurchCode3"] = CastDbNull.To<string>(reader["IMPRP3"], String.Empty).Trim();
                    dr["PurchCode4"] = CastDbNull.To<string>(reader["IMPRP4"], String.Empty).Trim();
                    dr["ReorderPoint"] = reader.Get<decimal>("IBROPI") / 1000;
                    dr["ReorderQuantity"] = reader.Get<decimal>("IBROQI") / 1000;
                    dr["VendorNum"] = reader.Get<decimal>("IBVEND");
                    dr["PlannerNum"] = reader.Get<decimal>("IBANPL");
                    dr["BuyerNum"] = reader.Get<decimal>("IBBUYR");
                    var updateDateId1 = CastDbNull.To<decimal>(reader["IBUPMJ"], 0);
                    var updateTimeId1 = CastDbNull.To<decimal>(reader["IBTDAY"], 0);
                    var updateDateId2 = CastDbNull.To<decimal>(reader["IMUPMJ"], 0);
                    var updateTimeId2 = CastDbNull.To<decimal>(reader["IMTDAY"], 0);
                    var updateDate1 = Utilities.Common.CastToDateTime(updateDateId1, updateTimeId1);
                    var updateDate2 = Utilities.Common.CastToDateTime(updateDateId2, updateTimeId2);
                    var updateDate = new[] { updateDate1, updateDate2 }.Max();
                    dr["TransactionDate"] = updateDate.AddHours(plant.TimeOffset);
                    this.Rows.Add(dr);
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
