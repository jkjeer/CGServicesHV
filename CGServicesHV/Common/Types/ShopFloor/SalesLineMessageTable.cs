using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SalesLineMessageTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public SalesLineMessageTable()
        {
            ExtendedProperties.Add("Target", "EBS.SalesLineMessageLedger");
            Columns.Add("SOrderNum", typeof(decimal));
            Columns.Add("SOrderType", typeof(string));
            Columns.Add("SOrderLineNum", typeof(decimal));
            Columns.Add("EdiSeqNum", typeof(decimal));
            Columns.Add("EdiLineNum", typeof(decimal));
            Columns.Add("EdiType", typeof(string));
            Columns.Add("EdiMsg", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["SOrderNum"] = reader.Get<decimal>("SDDOCO");
                    dr["SOrderType"] = reader.Get<string>("SDDCTO");
                    dr["SOrderLineNum"] = reader.Get<decimal>("SDLNID") / 1000;
                    dr["EdiSeqNum"] = reader.Get<decimal>("ZDEDSQ");
                    dr["EdiLineNum"] = reader.Get<decimal>("ZDLINS") / 100;
                    dr["EdiType"] = reader.Get<string>("ZDEDTY");
                    dr["EdiMsg"] = reader.Get<string>("ZDTXLN");
                    dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ZDUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["ZDTDAY"], 0)).AddMinutes(plant.BusinessSystemOffset); ;
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
