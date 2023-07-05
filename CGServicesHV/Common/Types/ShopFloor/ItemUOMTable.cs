using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ItemUOMTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemUOMTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemUOMLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("Numerator", typeof(string));
            Columns.Add("Denominator", typeof(string));
            Columns.Add("Factor", typeof(decimal));
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
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["ITM"], 0);
                    dr["Numerator"] = CastDbNull.To<string>(reader["RUM"], String.Empty).Trim();
                    dr["Denominator"] = CastDbNull.To<string>(reader["UM"], String.Empty).Trim();
                    dr["Factor"] = CastDbNull.To<decimal>(reader["CONV"], 0) / 10000000;
                    dr["UserId"] = CastDbNull.To<string>(reader["USER"], String.Empty).Trim();
                    dr["ProgramId"] = CastDbNull.To<string>(reader["PID"], String.Empty).Trim();
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["UPMJ"], 0), CastDbNull.To<decimal>(reader["TDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
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
