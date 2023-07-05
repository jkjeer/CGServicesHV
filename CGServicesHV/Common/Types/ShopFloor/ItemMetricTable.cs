using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.Enterprise;
using System;
using System.Data;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ItemMetricTable : DataTable
    {

        public ItemMetricTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemMetricLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("CategoryCode", typeof(string));
            Columns.Add("DocumentTypeCode", typeof(string));
            Columns.Add("TransUom", typeof(string));
            Columns.Add("PrimaryUom", typeof(string));
            Columns.Add("MthTransAmt", typeof(decimal));
            Columns.Add("QtrTransAmt", typeof(decimal));
            Columns.Add("YrTransAmt", typeof(decimal));
            Columns.Add("MthPrimaryAmt", typeof(decimal));
            Columns.Add("QtrPrimaryAmt", typeof(decimal));
            Columns.Add("YrPrimaryAmt", typeof(decimal));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["ILITM"], 0);
                    dr["CategoryCode"] = CastDbNull.To<string>(reader["ILGLPT"], "");
                    dr["DocumentTypeCode"] = CastDbNull.To<string>(reader["ILDCT"], "");
                    dr["TransUom"] = CastDbNull.To<string>(reader["ILTRUM"], "");
                    dr["PrimaryUom"] = CastDbNull.To<string>(reader["IMUOM1"], "");
                    dr["MthTransAmt"] = CastDbNull.To<decimal>(reader["ILTRQT1"], 0);
                    dr["QtrTransAmt"] = CastDbNull.To<decimal>(reader["ILTRQT2"], 0);
                    dr["YrTransAmt"] = CastDbNull.To<decimal>(reader["ILTRQT3"], 0);
                    dr["MthPrimaryAmt"] = 0;
                    dr["QtrPrimaryAmt"] = 0;
                    dr["YrPrimaryAmt"] = 0;
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