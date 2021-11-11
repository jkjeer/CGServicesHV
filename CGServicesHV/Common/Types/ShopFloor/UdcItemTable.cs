using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class UdcItemTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public UdcItemTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[UdcItemLedger]");
            Columns.Add("ProductCode", typeof(string));
            Columns.Add("UserCode", typeof(string));
            Columns.Add("UserValue", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("Description2", typeof(string));
            Columns.Add("HandlingCode", typeof(string));
            Columns.Add("SpecDescription1", typeof(string));
            Columns.Add("SpecDescription2", typeof(string));
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
                        dr["ProductCode"] = CastDbNull.To(reader["DRSY"], string.Empty).Trim();
                        dr["UserCode"] = CastDbNull.To(reader["DRRT"], string.Empty).Trim();
                        //dr["UserCodeType"] = CastDbNull.To(reader["DTDL01"], string.Empty).Trim();
                        dr["UserValue"] = CastDbNull.To(reader["DRKY"], string.Empty).Trim();
                        dr["Description1"] = CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        dr["Description2"] = CastDbNull.To(reader["DRDL02"], string.Empty).Trim();
                        dr["HandlingCode"] = CastDbNull.To(reader["DRSPHD"], string.Empty).Trim();
                        var updateDateId = CastDbNull.To<decimal>(reader["DRUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<decimal>(reader["DRUPMT"], 0);
                        dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        dr["SpecDescription1"] = CastDbNull.To(reader["SPECDESC1"], string.Empty).Trim();
                        dr["SpecDescription2"] = CastDbNull.To(reader["SPECDESC2"], string.Empty).Trim();
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
