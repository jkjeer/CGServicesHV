using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ConfiguratorPartTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }


        public ConfiguratorPartTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ConfiguratorPartLedger]");
            Columns.Add("ConfigurationId", typeof(decimal));
            Columns.Add("ComponentNum", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("SequenceNum", typeof(decimal));
            Columns.Add("PlannedQty", typeof(decimal));
            Columns.Add("UOM", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("Description", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }


        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ConfigurationId"] = CastDbNull.To<decimal>(reader["CCCFGID"], 0);
                    dr["ComponentNum"] = Convert.ToDecimal(CastDbNull.To<long>(reader["CCCPNB"], 0));
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["CCITM"], 0);
                    dr["SequenceNum"] = CastDbNull.To<decimal>(reader["CCOPSQ"], 0) / 100;
                    dr["PlannedQty"] = CastDbNull.To<decimal>(reader["CCQNTY"], 0) / 1000;
                    dr["UOM"] = CastDbNull.To(reader["CCUOM"], string.Empty).Trim();
                    dr["Location"] = CastDbNull.To(reader["CCLOCN"], string.Empty).Trim();
                    dr["Description"] = CastDbNull.To(reader["CCDSC1"], string.Empty).Trim();
                    var updateDateId = CastDbNull.To<decimal>(reader["CCUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["CCTDAY"], 0);
                    dr["TransactionDate"] = CGServicesHV.Common.Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddMinutes(plant.BusinessSystemOffset);
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
