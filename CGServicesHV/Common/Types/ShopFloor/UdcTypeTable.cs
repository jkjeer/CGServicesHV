using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class UdcTypeTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public UdcTypeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[UdcTypeLedger]");
            Columns.Add("ProductCode", typeof(string));
            Columns.Add("UserCode", typeof(string));
            Columns.Add("Description", typeof(string));
            Columns.Add("CodeLength", typeof(decimal));
            Columns.Add("NumericYN", typeof(char));
            Columns.Add("UpdateDate", typeof(DateTime));
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
                        dr["ProductCode"] = Data.CastDbNull.To(reader["DTSY"], string.Empty).Trim();
                        dr["UserCode"] = Data.CastDbNull.To(reader["DTRT"], string.Empty).Trim();
                        dr["Description"] = Data.CastDbNull.To(reader["DTDL01"], string.Empty).Trim();
                        dr["CodeLength"] = Data.CastDbNull.To<decimal>(reader["DTCDL"], 0);
                        dr["NumericYN"] = Data.CastDbNull.To(reader["DTCNUM"], string.Empty).Trim();
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["DTUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["DTUPMT"], 0);
                        dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
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
