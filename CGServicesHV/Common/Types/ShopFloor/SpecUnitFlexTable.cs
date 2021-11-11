using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using CGServices.Common.Managers.E1;
using CGServices.Common.Types.Enterprise;

namespace CGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitFlexTable : DataTable
    {

        public SpecUnitFlexTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_FLEX_LEDGER]");
            Columns.Add("SPEC_UNIT_ID", typeof(long));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("CONSTRUCTION_CD", typeof(string));
            Columns.Add("SHAPE_CD", typeof(string));
            Columns.Add("AIRSPACE_CD", typeof(string));
            Columns.Add("PRESERVE_CD", typeof(string));
            Columns.Add("PLACEMENTS_CD", typeof(string));
            Columns.Add("GLASS_PROCESSING_CD", typeof(string));
            Columns.Add("VGROOVE_CD", typeof(string));
            Columns.Add("ROUTING_CD", typeof(string));
            Columns.Add("RELATED_ORDERS", typeof(string));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                DataRow dr = NewRow();
                dr["ITEM_NUM"] = item.ItemNum;
                dr["CONFIGURATION_ID"] = item.ConfigurationId;
                dr["CONSTRUCTION_CD"] = item.FlexDescriptions[0];
                dr["SHAPE_CD"] = item.FlexDescriptions[1];
                dr["AIRSPACE_CD"] = item.FlexDescriptions[2];
                dr["PRESERVE_CD"] = item.FlexDescriptions[3];
                dr["PLACEMENTS_CD"] = item.FlexDescriptions[4];
                dr["GLASS_PROCESSING_CD"] = item.FlexDescriptions[5];
                dr["VGROOVE_CD"] = item.FlexDescriptions[6];
                dr["ROUTING_CD"] = item.FlexDescriptions[7];
                //var orderIdList = new List<String>();
                //foreach (OrderId orderId in item.OrderIds)
                //{
                //    orderIdList.Add(orderId.Description);
                //}
                //dr["RELATED_ORDERS"] = string.Join(" | ", Array.ConvertAll(orderIdList.ToArray(), i => i.ToString()));
                Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    DataRow dr = NewRow();
                    dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                    dr["ITEM_NUM"] = line.F580021.S7ITM;
                    dr["CONFIGURATION_ID"] = line.F580021.CMCFGID;
                    dr["CONSTRUCTION_CD"] = line.FlexItemDescriptions[0];
                    dr["SHAPE_CD"] = line.FlexItemDescriptions[1];
                    dr["AIRSPACE_CD"] = line.FlexItemDescriptions[2];
                    dr["PRESERVE_CD"] = line.FlexItemDescriptions[3];
                    dr["PLACEMENTS_CD"] = line.FlexItemDescriptions[4];
                    dr["GLASS_PROCESSING_CD"] = line.FlexItemDescriptions[5];
                    dr["VGROOVE_CD"] = line.FlexItemDescriptions[6];
                    dr["ROUTING_CD"] = line.FlexItemDescriptions[7];
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }


    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitFlexEbsTable : DataTable
    {

        public SpecUnitFlexEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitDescription]");
            Columns.Add("SpecUnitId", typeof(int));
            Columns.Add("Construction", typeof(string));
            Columns.Add("Shape", typeof(string));
            Columns.Add("Airspace", typeof(string));
            Columns.Add("Preserve", typeof(string));
            Columns.Add("BarPlacement", typeof(string));
            Columns.Add("GlassProcessing", typeof(string));
            Columns.Add("Vgroove", typeof(string));
            Columns.Add("GlassRouting", typeof(string));
            Columns.Add("RelatedOrder", typeof(string));
        }



        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    DataRow dr = NewRow();
                    dr["SpecUnitId"] = line.SpecUnitId;
                    dr["Construction"] = line.FlexItemDescriptions[0];
                    dr["Shape"] = line.FlexItemDescriptions[1];
                    dr["Airspace"] = line.FlexItemDescriptions[2];
                    dr["Preserve"] = line.FlexItemDescriptions[3];
                    dr["BarPlacement"] = line.FlexItemDescriptions[4];
                    dr["GlassProcessing"] = line.FlexItemDescriptions[5];
                    dr["Vgroove"] = line.FlexItemDescriptions[6];
                    dr["GlassRouting"] = line.FlexItemDescriptions[7];
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
