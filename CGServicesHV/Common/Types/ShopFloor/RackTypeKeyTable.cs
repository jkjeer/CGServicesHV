using System;
using System.Data;
using System.ServiceModel;
using CGServices.Common.Utilities;

namespace CGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class RackTypeKeyTable : DataTable
    {
        public RackTypeKeyTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RACK_TYPE_KEY_LEDGER]");
            Columns.Add("Plant", typeof(string));
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
        }

        public void Add(IDataReader reader)
        {
            try
            {
                using (reader)
                {

                    while (reader.Read())
                    {
                        var dr = NewRow();
                        dr["Plant"] = CastDbNull.To(reader["RTMCU"], string.Empty).Trim();
                        dr["RackTypeCode"] = CastDbNull.To(reader["RTZ58RT"], string.Empty).Trim();
                        dr["RackTypeDesc"] = CastDbNull.To(reader["RTZ58RTD"], string.Empty).Trim();
                        dr["RackTypeStatus"] = CastDbNull.To(reader["RTZ58RTST"], string.Empty).Trim();
                        dr["CustomerNumber"] = CastDbNull.To<decimal>(reader["RTAN8"], 0);
                        dr["ShipTo"] = CastDbNull.To<decimal>(reader["RTSHAN"], 0);
                        dr["CorporateRackTypeFlag"] = CastDbNull.To(reader["RTZ58CRT"], string.Empty).Trim();
                        dr["RackTypeShippableFlag"] = CastDbNull.To(reader["RTZ58SHP"], string.Empty).Trim();
                        dr["RackTypeTareWeight"] = CastDbNull.To<decimal>(reader["RTZ58RTWT"], 0) / 100;
                        dr["MinTotalWeight"] = CastDbNull.To<decimal>(reader["RTZ58WTMN"], 0);
                        dr["MaxTotalWeight"] = CastDbNull.To<decimal>(reader["RTZ58WTMX"], 0);
                        dr["WeightUom"] = CastDbNull.To(reader["RTZ58WTUM"], string.Empty).Trim();
                        dr["GrossHeight"] = CastDbNull.To<decimal>(reader["RTGHET"], 0) / 100;
                        dr["GrossWidth"] = CastDbNull.To<decimal>(reader["RTGWID"], 0) / 100;
                        dr["GrossDepth"] = CastDbNull.To<decimal>(reader["RTGDEP"], 0) / 100;
                        dr["GlassMinLength"] = CastDbNull.To<decimal>(reader["RTZ58LMDC"], 0) / 1000000;
                        dr["GlassMaxLength"] = CastDbNull.To<decimal>(reader["RTZ58LXDC"], 0) / 1000000;
                        dr["GlassMinHeight"] = CastDbNull.To<decimal>(reader["RTZ58HMDC"], 0) / 1000000;
                        dr["GlassMaxHeight"] = CastDbNull.To<decimal>(reader["RTZ58HXDC"], 0) / 1000000;
                        dr["GlassMinWidth"] = CastDbNull.To<decimal>(reader["RTZ58WMDC"], 0) / 1000000;
                        dr["GlassMaxWidth"] = CastDbNull.To<decimal>(reader["RTZ58WXDC"], 0) / 1000000;
                        dr["DimensionUom"] = CastDbNull.To(reader["RTWIUM"], string.Empty).Trim();
                        dr["RackSides"] = CastDbNull.To<decimal>(reader["RTZ58RSD"], 0);
                        dr["RackPositions"] = CastDbNull.To<decimal>(reader["RTZ58RPS"], 0);
                        dr["RackStacks"] = CastDbNull.To<decimal>(reader["RTZ58RXT"], 0);
                        dr["RackRows"] = CastDbNull.To<decimal>(reader["RTZ58RXR"], 0);
                        dr["RackSlots"] = CastDbNull.To<decimal>(reader["RTZ58RSL"], 0);
                        dr["RackShapeFlag"] = CastDbNull.To(reader["RTZ58RSH"], string.Empty).Trim();
                        dr["RackAnnealedFlag"] = CastDbNull.To(reader["RTZ58RAN"], string.Empty).Trim();
                        dr["GrossCubes"] = CastDbNull.To<decimal>(reader["RTGCUB"], 0) / 100;
                        dr["VolumeUom"] = CastDbNull.To(reader["RTVUMD"], string.Empty).Trim();
                        var updateDateId = CastDbNull.To<decimal>(reader["RTUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<decimal>(reader["RTTDAY"], 0);
                        dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                        Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
