using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class CardinalProductTypeTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public CardinalProductTypeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[CardinalProductTypeMapLedger]");
            Columns.Add("CustomerShipToNum", typeof(decimal));
            Columns.Add("ProductType1", typeof(string));
            Columns.Add("ProductType2", typeof(string));
            Columns.Add("CardinalProductType", typeof(string));
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
                    dr["CustomerShipToNum"] = CastDbNull.To<decimal>(reader["PTSHAN"], 0);
                    dr["ProductType1"] = CastDbNull.To<string>(reader["PTZ5895154"], String.Empty).Trim();
                    dr["ProductType2"] = CastDbNull.To<string>(reader["PTZ58PT"], String.Empty).Trim();
                    dr["CardinalProductType"] = CastDbNull.To<string>(reader["PTZ5840310"], String.Empty).Trim();
                    dr["UserId"] = CastDbNull.To<string>(reader["PTUSER"], String.Empty).Trim();
                    dr["ProgramId"] = CastDbNull.To<string>(reader["PTPID"], String.Empty).Trim();
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["PTUPMJ"], 0), CastDbNull.To<decimal>(reader["PTUPMT"], 0)).AddMinutes(plant.BusinessSystemOffset);
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
