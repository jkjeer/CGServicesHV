using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class LaborTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select new DateTime(a.Field<int>("ProductionYear"), a.Field<int>("ProductionMonth"), 1)).Max(); }
            set { }
        }

        public LaborTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[LaborLedger]");
            Columns.Add("ProductionYear", typeof(int));
            Columns.Add("ProductionMonth", typeof(int));
            Columns.Add("Workgroup1", typeof(string));
            Columns.Add("Workgroup2", typeof(string));
            Columns.Add("Workgroup3", typeof(string));
            Columns.Add("PaylineTypeCode", typeof(string));
            Columns.Add("PayTypeDesc", typeof(string));
            Columns.Add("Amount", typeof(decimal));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ProductionYear"] = Data.CastDbNull.To<int>(reader["ProductionYear"], 0);
                    dr["ProductionMonth"] = Data.CastDbNull.To<int>(reader["ProductionMonth"], 0);
                    dr["Workgroup1"] = Data.CastDbNull.To(reader["Workgroup1"], string.Empty).Trim();
                    dr["Workgroup2"] = Data.CastDbNull.To(reader["Workgroup2"], string.Empty).Trim();
                    dr["Workgroup3"] = Data.CastDbNull.To(reader["Workgroup3"], string.Empty).Trim();
                    dr["PaylineTypeCode"] = Data.CastDbNull.To(reader["PaylineTypeCode"], string.Empty).Trim();
                    dr["PayTypeDesc"] = Data.CastDbNull.To(reader["PayTypeDesc"], string.Empty).Trim();
                    dr["Amount"] = Data.CastDbNull.To<decimal>(reader["Amount"], 0);
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }
}
