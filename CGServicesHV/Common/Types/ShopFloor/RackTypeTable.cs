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

    public class RackTypeTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public RackTypeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackTypeLedger]");
            Columns.Add("RackTypeLedgerID", typeof(decimal));
            Columns.Add("RackTypeCode", typeof(string));
            Columns.Add("RackTypeDesc", typeof(string));
            Columns.Add("RackTypeStatus", typeof(string));
            Columns.Add("CustomerNumber", typeof(decimal));
            Columns.Add("ShipTo", typeof(decimal));
            Columns.Add("CorporateRackTypeFlag", typeof(string));
            Columns.Add("RackTypeShippableFlag", typeof(string));
            Columns.Add("RackTypeTareWeight", typeof(decimal));
            Columns.Add("MinTotalWeight", typeof(decimal));
            Columns.Add("MaxTotalWeight", typeof(decimal));
            Columns.Add("WeightUOM", typeof(string));
            Columns.Add("GrossHeight", typeof(decimal));
            Columns.Add("GrossWidth", typeof(decimal));
            Columns.Add("GrossDepth", typeof(decimal));
            Columns.Add("GlassMinLength", typeof(decimal));
            Columns.Add("GlassMaxLength", typeof(decimal));
            Columns.Add("GlassMinHeight", typeof(decimal));
            Columns.Add("GlassMaxHeight", typeof(decimal));
            Columns.Add("GlassMinWidth", typeof(decimal));
            Columns.Add("GlassMaxWidth", typeof(decimal));
            Columns.Add("DimensionUOM", typeof(string));
            Columns.Add("RackSides", typeof(decimal));
            Columns.Add("RackPositions", typeof(decimal));
            Columns.Add("RackStacks", typeof(decimal));
            Columns.Add("RackRows", typeof(decimal));
            Columns.Add("RackSlots", typeof(decimal));
            Columns.Add("RackShapeFlag", typeof(string));
            Columns.Add("RackAnnealedFlag", typeof(string));
            Columns.Add("ItemNumber", typeof(decimal));
            Columns.Add("GrossCubes", typeof(decimal));
            Columns.Add("VolumeUOM", typeof(string));
            Columns.Add("UpdateDate", typeof(DateTime));
            Columns.Add("PlantID", typeof(string));
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
                        dr["RackTypeCode"] = Data.CastDbNull.To(reader["RTZ58RT"], string.Empty).Trim();
                        dr["RackTypeDesc"] = Data.CastDbNull.To(reader["RTZ58RTD"], string.Empty).Trim();
                        dr["RackTypeStatus"] = Data.CastDbNull.To(reader["RTZ58RTST"], string.Empty).Trim();
                        dr["CustomerNumber"] = Data.CastDbNull.To<decimal>(reader["RTAN8"], 0);
                        dr["ShipTo"] = Data.CastDbNull.To<decimal>(reader["RTSHAN"], 0);
                        dr["CorporateRackTypeFlag"] = Data.CastDbNull.To(reader["RTZ58CRT"], string.Empty).Trim();
                        dr["RackTypeShippableFlag"] = Data.CastDbNull.To(reader["RTZ58SHP"], string.Empty).Trim();
                        dr["RackTypeTareWeight"] = Data.CastDbNull.To<decimal>(reader["RTZ58RTWT"], 0) / 100;
                        dr["MinTotalWeight"] = Data.CastDbNull.To<decimal>(reader["RTZ58WTMN"], 0);
                        dr["MaxTotalWeight"] = Data.CastDbNull.To<decimal>(reader["RTZ58WTMX"], 0);
                        dr["WeightUom"] = Data.CastDbNull.To(reader["RTZ58WTUM"], string.Empty).Trim();
                        dr["GrossHeight"] = Data.CastDbNull.To<decimal>(reader["RTGHET"], 0) / 100;
                        dr["GrossWidth"] = Data.CastDbNull.To<decimal>(reader["RTGWID"], 0) / 100;
                        dr["GrossDepth"] = Data.CastDbNull.To<decimal>(reader["RTGDEP"], 0) / 100;
                        dr["GlassMinLength"] = Data.CastDbNull.To<decimal>(reader["RTZ58LMDC"], 0) / 1000000;
                        dr["GlassMaxLength"] = Data.CastDbNull.To<decimal>(reader["RTZ58LXDC"], 0) / 1000000;
                        dr["GlassMinHeight"] = Data.CastDbNull.To<decimal>(reader["RTZ58HMDC"], 0) / 1000000;
                        dr["GlassMaxHeight"] = Data.CastDbNull.To<decimal>(reader["RTZ58HXDC"], 0) / 1000000;
                        dr["GlassMinWidth"] = Data.CastDbNull.To<decimal>(reader["RTZ58WMDC"], 0) / 1000000;
                        dr["GlassMaxWidth"] = Data.CastDbNull.To<decimal>(reader["RTZ58WXDC"], 0) / 1000000;
                        dr["DimensionUom"] = Data.CastDbNull.To(reader["RTWIUM"], string.Empty).Trim();
                        dr["RackSides"] = Data.CastDbNull.To<decimal>(reader["RTZ58RSD"], 0);
                        dr["RackPositions"] = Data.CastDbNull.To<decimal>(reader["RTZ58RPS"], 0);
                        dr["RackStacks"] = Data.CastDbNull.To<decimal>(reader["RTZ58RXT"], 0);
                        dr["RackRows"] = Data.CastDbNull.To<decimal>(reader["RTZ58RXR"], 0);
                        dr["RackSlots"] = Data.CastDbNull.To<decimal>(reader["RTZ58RSL"], 0);
                        dr["RackShapeFlag"] = Data.CastDbNull.To(reader["RTZ58RSH"], string.Empty).Trim();
                        dr["RackAnnealedFlag"] = Data.CastDbNull.To(reader["RTZ58RAN"], string.Empty).Trim();
                        dr["GrossCubes"] = Data.CastDbNull.To<decimal>(reader["RTGCUB"], 0) / 100;
                        dr["VolumeUom"] = Data.CastDbNull.To(reader["RTVUMD"], string.Empty).Trim();
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["RTUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["RTTDAY"], 0);
                        dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        dr["PlantID"] = Data.CastDbNull.To(reader["RTMCU"], string.Empty).Trim();
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
