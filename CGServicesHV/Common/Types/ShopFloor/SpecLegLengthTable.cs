using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class SpecLegLengthTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("WrittenDate")).Max(); }
            set { }
        }

        public SpecLegLengthTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecLegLengthLedger]");
            Columns.Add("LegLengthId", typeof(int));
            Columns.Add("Increment", typeof(int));
            Columns.Add("Inch", typeof(decimal));
            Columns.Add("MinInch", typeof(decimal));
            Columns.Add("MaxInch", typeof(decimal));
            Columns.Add("Millimeter", typeof(decimal));
            Columns.Add("MinMillimeter", typeof(decimal));
            Columns.Add("MaxMillimeter", typeof(decimal));
            Columns.Add("FractionalBase", typeof(int));
            Columns.Add("FractionalNumerator", typeof(int));
            Columns.Add("FractionalDenominator", typeof(int));
            Columns.Add("FractionalDescription", typeof(string));
            //Columns.Add("WrittenDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                using (reader)
                {
                    while (reader.Read())
                    {
                        var dr = NewRow();
                        dr["LegLengthId"] = Data.CastDbNull.To(reader["LegLengthId"], 0);
                        dr["Increment"] = Data.CastDbNull.To(reader["Increment"], 0);
                        dr["Inch"] = Data.CastDbNull.To<decimal>(reader["Inch"], 0);
                        dr["MinInch"] = Data.CastDbNull.To<decimal>(reader["MinInch"], 0);
                        dr["MaxInch"] = Data.CastDbNull.To<decimal>(reader["MaxInch"], 0);
                        dr["Millimeter"] = Data.CastDbNull.To<decimal>(reader["Millimeter"], 0);
                        dr["MinMillimeter"] = Data.CastDbNull.To<decimal>(reader["MinMillimeter"], 0);
                        dr["MaxMillimeter"] = Data.CastDbNull.To<decimal>(reader["MaxMillimeter"], 0);
                        dr["FractionalBase"] = Data.CastDbNull.To(reader["FractionalBase"], 0);
                        dr["FractionalNumerator"] = Data.CastDbNull.To(reader["FractionalNumerator"], 0);
                        dr["FractionalDenominator"] = Data.CastDbNull.To(reader["FractionalDenominator"], 0);
                        dr["FractionalDescription"] = Data.CastDbNull.To(reader["FractionalDescription"], string.Empty).Trim();
                        //dr["WrittenDate"] = Data.CastDbNull.To(reader["WrittenDate"], DateTime.Parse("1900-01-01"));
                        Rows.Add(dr);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }
}
