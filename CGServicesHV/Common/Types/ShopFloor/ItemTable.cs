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
            Columns.Add("StockType", typeof(string));
            Columns.Add("GlCategory", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("Description2", typeof(string));
            Columns.Add("PrimaryUOM", typeof(string));
            Columns.Add("SecondaryUOM", typeof(string));
            Columns.Add("SalesCode1", typeof(string));
            Columns.Add("SalesCode2", typeof(string));
            Columns.Add("SalesCode3", typeof(string));
            Columns.Add("SalesCode4", typeof(string));
            Columns.Add("PurchCode1", typeof(string));
            Columns.Add("PurchCode2", typeof(string));
            Columns.Add("PurchCode3", typeof(string));
            Columns.Add("PurchCode4", typeof(string));
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
                    dr["StockType"] = CastDbNull.To<string>(reader["IMSTKT"], String.Empty).Trim();
                    dr["GlCategory"] = CastDbNull.To<string>(reader["IMGLPT"], String.Empty).Trim();
                    dr["Description1"] = CastDbNull.To<string>(reader["IMDSC1"], String.Empty).Trim();
                    dr["Description2"] = CastDbNull.To<string>(reader["IMDSC2"], String.Empty).Trim();
                    dr["PrimaryUOM"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
                    dr["SecondaryUOM"] = CastDbNull.To<string>(reader["IMUOM2"], String.Empty).Trim();
                    dr["SalesCode1"] = CastDbNull.To<string>(reader["IMSRP1"], String.Empty).Trim();
                    dr["SalesCode2"] = CastDbNull.To<string>(reader["IMSRP2"], String.Empty).Trim();
                    dr["SalesCode3"] = CastDbNull.To<string>(reader["IMSRP3"], String.Empty).Trim();
                    dr["SalesCode4"] = CastDbNull.To<string>(reader["IMSRP4"], String.Empty).Trim();
                    dr["PurchCode1"] = CastDbNull.To<string>(reader["IMPRP1"], String.Empty).Trim();
                    dr["PurchCode2"] = CastDbNull.To<string>(reader["IMPRP2"], String.Empty).Trim();
                    dr["PurchCode3"] = CastDbNull.To<string>(reader["IMPRP3"], String.Empty).Trim();
                    dr["PurchCode4"] = CastDbNull.To<string>(reader["IMPRP4"], String.Empty).Trim();
                    var tranDateId = Data.CastDbNull.To<decimal>(reader["IMUPMJ"], 0);
                    var tranTimeId = Data.CastDbNull.To<decimal>(reader["IMTDAY"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(tranDateId, tranTimeId).AddMinutes(plant.BusinessSystemOffset);
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
