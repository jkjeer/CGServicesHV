using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.Enterprise;
using System;
using System.Data;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class GlassItemMetricTable : DataTable
    {

        public GlassItemMetricTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[GlassItemMetricLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("MetricName", typeof(string));
            Columns.Add("MonthAmt", typeof(decimal));
            Columns.Add("QuarterAmt", typeof(decimal));
            Columns.Add("YearAmt", typeof(decimal));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["ILITM"], 0);
                    dr["MetricName"] = Data.CastDbNull.To<string>(reader["METRICNAME"], "");
                    dr["MonthAmt"] = Data.CastDbNull.To<decimal>(reader["MONTHAMT"], 0);
                    dr["QuarterAmt"] = Data.CastDbNull.To<decimal>(reader["QUARTERAMT"], 0);
                    dr["YearAmt"] = Data.CastDbNull.To<decimal>(reader["YEARAMT"], 0);
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