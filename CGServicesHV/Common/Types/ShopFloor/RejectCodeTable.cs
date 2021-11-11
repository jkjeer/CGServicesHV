using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class RejectCodeTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public RejectCodeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RejectCodeLedger]");
            Columns.Add("RejectCodeLedgerID", typeof(decimal));
            Columns.Add("ProductCode", typeof(string));
            Columns.Add("UserCode", typeof(string));
            Columns.Add("RejectCode", typeof(string));
            Columns.Add("RejectType", typeof(string));
            Columns.Add("Workcenter", typeof(string));
            Columns.Add("PlantNum", typeof(string));
            Columns.Add("Description", typeof(string));
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
                        dr["RejectCode"] = CastDbNull.To(reader["RCKY"], string.Empty).Trim();
                        dr["RejectType"] = CastDbNull.To(reader["RCDL01"], string.Empty).Trim();
                        dr["Description"] = CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        dr["ProductCode"] = CastDbNull.To(reader["RCSY"], string.Empty).Trim();
                        dr["UserCode"] = CastDbNull.To(reader["RCRT"], string.Empty).Trim();
                        dr["Workcenter"] = CastDbNull.To(reader["RCMCU"], string.Empty).Trim();
                        dr["PlantNum"] = CastDbNull.To(reader["MCRP01"], string.Empty).Trim();
                        var updateDateId = CastDbNull.To<decimal>(reader["DRUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<decimal>(reader["DRUPMT"], 0);
                        dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new FaultException(e.Message);
            }
        }
    }
}
