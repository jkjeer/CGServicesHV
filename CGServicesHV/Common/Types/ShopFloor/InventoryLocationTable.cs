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

    public class InventoryLocationTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public InventoryLocationTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[InventoryLocationLedger]");
            Columns.Add("PlantID", typeof(string));
            Columns.Add("LocationCode", typeof(string));
            Columns.Add("LocationCodeE1", typeof(string));
            Columns.Add("Aisle", typeof(string));
            Columns.Add("Bin", typeof(string));
            Columns.Add("CategoryCode", typeof(string));
            Columns.Add("Category3", typeof(string));
            Columns.Add("Category4", typeof(string));
            Columns.Add("DetailLevel", typeof(string));
            Columns.Add("PutawayZone", typeof(string));
            Columns.Add("StagingFlag", typeof(string));
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
                        dr["PlantID"] = CastDbNull.To(reader["LMMCU"], string.Empty).Trim();
                        var detailLevel = CastDbNull.To(reader["LMLLDL"], string.Empty).Trim();
                        dr["DetailLevel"] = detailLevel;
                        //var plantId = CastDbNull.To(reader["LMMCU"], string.Empty).Trim();
                        //dr["PlantID"] = (detailLevel == "2" || detailLevel == "9") ? null : plantId;
                        dr["LocationCode"] = CastDbNull.To(reader["LMLOCN"], string.Empty).Trim();
                        dr["LocationCodeE1"] = CastDbNull.To(reader["LMLOCN"], string.Empty);
                        dr["Aisle"] = CastDbNull.To(reader["LMAISL"], string.Empty).Trim();
                        dr["Bin"] = CastDbNull.To(reader["LMBIN"], string.Empty).Trim();
                        dr["CategoryCode"] = "";
                        dr["Category3"] = CastDbNull.To(reader["LMLA03"], string.Empty).Trim();
                        dr["Category4"] = CastDbNull.To(reader["LMLA04"], string.Empty).Trim();
                        var putawayZone = CastDbNull.To(reader["LMPZON"], string.Empty).Trim();
                        dr["PutawayZone"] = putawayZone;
                        dr["StagingFlag"] = putawayZone == "1" ? "Y" : "N";
                        var updateDateId = CastDbNull.To<decimal>(reader["LMUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<decimal>(reader["LMTDAY"], 0);
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
