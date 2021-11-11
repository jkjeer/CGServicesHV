using System;
using System.Data;
using System.ServiceModel;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;

namespace CGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitGridTable : DataTable
    {

        public SpecUnitGridTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_GRID_LEDGER]");
            Columns.Add("SPEC_UNIT_ID", typeof(long));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("GRID_STYLE_CD", typeof(string));
            Columns.Add("SUB_GRID_STYLE_CD", typeof(string));
            Columns.Add("HORZ_BARS_QTY", typeof(decimal));
            Columns.Add("VERT_BARS_QTY", typeof(decimal));
            Columns.Add("HIGH_OPEN_QTY", typeof(decimal));
            Columns.Add("WIDE_OPEN_QTY", typeof(decimal));
            Columns.Add("ODD_OPEN_QTY", typeof(decimal));
            Columns.Add("RECT_OPEN_QTY", typeof(decimal));
            Columns.Add("DRAWING_CD", typeof(string));
            Columns.Add("HORZ_SASH_INTRUSION", typeof(decimal));
            Columns.Add("VERT_SASH_INTRUSION", typeof(decimal));
            Columns.Add("HORZ_PRAIRIE_OFFSET", typeof(decimal));
            Columns.Add("VERT_PRAIRIE_OFFSET", typeof(decimal));
            Columns.Add("PLACEMENT_TYPE_CD", typeof(string));
            Columns.Add("HORZ_POS_01", typeof(decimal));
            Columns.Add("HORZ_POS_02", typeof(decimal));
            Columns.Add("HORZ_POS_03", typeof(decimal));
            Columns.Add("HORZ_POS_04", typeof(decimal));
            Columns.Add("HORZ_POS_05", typeof(decimal));
            Columns.Add("HORZ_POS_06", typeof(decimal));
            Columns.Add("HORZ_POS_07", typeof(decimal));
            Columns.Add("HORZ_POS_08", typeof(decimal));
            Columns.Add("HORZ_POS_09", typeof(decimal));
            Columns.Add("HORZ_POS_10", typeof(decimal));
            Columns.Add("HORZ_POS_11", typeof(decimal));
            Columns.Add("HORZ_POS_12", typeof(decimal));
            Columns.Add("HORZ_POS_13", typeof(decimal));
            Columns.Add("HORZ_POS_14", typeof(decimal));
            Columns.Add("HORZ_POS_15", typeof(decimal));
            Columns.Add("HORZ_POS_16", typeof(decimal));
            Columns.Add("HORZ_POS_17", typeof(decimal));
            Columns.Add("HORZ_POS_18", typeof(decimal));
            Columns.Add("HORZ_POS_19", typeof(decimal));
            Columns.Add("HORZ_POS_20", typeof(decimal));
            Columns.Add("VERT_POS_01", typeof(decimal));
            Columns.Add("VERT_POS_02", typeof(decimal));
            Columns.Add("VERT_POS_03", typeof(decimal));
            Columns.Add("VERT_POS_04", typeof(decimal));
            Columns.Add("VERT_POS_05", typeof(decimal));
            Columns.Add("VERT_POS_06", typeof(decimal));
            Columns.Add("VERT_POS_07", typeof(decimal));
            Columns.Add("VERT_POS_08", typeof(decimal));
            Columns.Add("VERT_POS_09", typeof(decimal));
            Columns.Add("VERT_POS_10", typeof(decimal));
            Columns.Add("VERT_POS_11", typeof(decimal));
            Columns.Add("VERT_POS_12", typeof(decimal));
            Columns.Add("VERT_POS_13", typeof(decimal));
            Columns.Add("VERT_POS_14", typeof(decimal));
            Columns.Add("VERT_POS_15", typeof(decimal));
            Columns.Add("VERT_POS_16", typeof(decimal));
            Columns.Add("VERT_POS_17", typeof(decimal));
            Columns.Add("VERT_POS_18", typeof(decimal));
            Columns.Add("VERT_POS_19", typeof(decimal));
            Columns.Add("VERT_POS_20", typeof(decimal));
            Columns.Add("HORZ_ANGLE_01", typeof(decimal));
            Columns.Add("HORZ_ANGLE_02", typeof(decimal));
            Columns.Add("HORZ_ANGLE_03", typeof(decimal));
            Columns.Add("HORZ_ANGLE_04", typeof(decimal));
            Columns.Add("HORZ_ANGLE_05", typeof(decimal));
            Columns.Add("HORZ_ANGLE_06", typeof(decimal));
            Columns.Add("HORZ_ANGLE_07", typeof(decimal));
            Columns.Add("HORZ_ANGLE_08", typeof(decimal));
            Columns.Add("HORZ_ANGLE_09", typeof(decimal));
            Columns.Add("HORZ_ANGLE_10", typeof(decimal));
            Columns.Add("HORZ_ANGLE_11", typeof(decimal));
            Columns.Add("HORZ_ANGLE_12", typeof(decimal));
            Columns.Add("HORZ_ANGLE_13", typeof(decimal));
            Columns.Add("HORZ_ANGLE_14", typeof(decimal));
            Columns.Add("HORZ_ANGLE_15", typeof(decimal));
            Columns.Add("HORZ_ANGLE_16", typeof(decimal));
            Columns.Add("HORZ_ANGLE_17", typeof(decimal));
            Columns.Add("HORZ_ANGLE_18", typeof(decimal));
            Columns.Add("HORZ_ANGLE_19", typeof(decimal));
            Columns.Add("HORZ_ANGLE_20", typeof(decimal));
            Columns.Add("VERT_ANGLE_01", typeof(decimal));
            Columns.Add("VERT_ANGLE_02", typeof(decimal));
            Columns.Add("VERT_ANGLE_03", typeof(decimal));
            Columns.Add("VERT_ANGLE_04", typeof(decimal));
            Columns.Add("VERT_ANGLE_05", typeof(decimal));
            Columns.Add("VERT_ANGLE_06", typeof(decimal));
            Columns.Add("VERT_ANGLE_07", typeof(decimal));
            Columns.Add("VERT_ANGLE_08", typeof(decimal));
            Columns.Add("VERT_ANGLE_09", typeof(decimal));
            Columns.Add("VERT_ANGLE_10", typeof(decimal));
            Columns.Add("VERT_ANGLE_11", typeof(decimal));
            Columns.Add("VERT_ANGLE_12", typeof(decimal));
            Columns.Add("VERT_ANGLE_13", typeof(decimal));
            Columns.Add("VERT_ANGLE_14", typeof(decimal));
            Columns.Add("VERT_ANGLE_15", typeof(decimal));
            Columns.Add("VERT_ANGLE_16", typeof(decimal));
            Columns.Add("VERT_ANGLE_17", typeof(decimal));
            Columns.Add("VERT_ANGLE_18", typeof(decimal));
            Columns.Add("VERT_ANGLE_19", typeof(decimal));
            Columns.Add("VERT_ANGLE_20", typeof(decimal));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                var g = item.GlassItem.Grid;
                if (!String.IsNullOrEmpty(g.GridStyle))
                {
                    DataRow dr = NewRow();
                    dr["ITEM_NUM"] = item.ItemNum;
                    dr["CONFIGURATION_ID"] = item.ConfigurationId;
                    dr["GRID_STYLE_CD"] = (string)g.GridStyle;
                    dr["SUB_GRID_STYLE_CD"] = g.SubGridStyle;
                    dr["HORZ_BARS_QTY"] = g.NumHorizontal;
                    dr["VERT_BARS_QTY"] = g.NumVertical;
                    dr["HIGH_OPEN_QTY"] = g.NumOpeningsHigh;
                    dr["WIDE_OPEN_QTY"] = g.NumOpeningsWide;
                    dr["ODD_OPEN_QTY"] = g.NumOpeningsOdd;
                    dr["RECT_OPEN_QTY"] = g.NumOpeningsRectangle;
                    dr["DRAWING_CD"] = g.Drawing;
                    dr["HORZ_SASH_INTRUSION"] = item.GlassItem.HorizontalSashIntrusion;
                    dr["VERT_SASH_INTRUSION"] = item.GlassItem.VerticalSashIntrusion;
                    dr["HORZ_PRAIRIE_OFFSET"] = g.HorizontalPrairieOffset;
                    dr["VERT_PRAIRIE_OFFSET"] = g.VerticalPrairieOffset;
                    dr["PLACEMENT_TYPE_CD"] = g.PlacementType;
                    var hpcount = g.HorizontalBarPlacements.Count;
                    var vpcount = g.VerticalBarPlacements.Count;

                    // HORIZONTAL
                    if (hpcount > 0)
                    {
                        dr["HORZ_POS_01"] = g.HorizontalBarPlacements[0].Position;
                        dr["HORZ_ANGLE_01"] = g.HorizontalBarPlacements[0].Angle;
                    }
                    if (hpcount > 1)
                    {
                        dr["HORZ_POS_02"] = g.HorizontalBarPlacements[1].Position;
                        dr["HORZ_ANGLE_02"] = g.HorizontalBarPlacements[1].Angle;
                    }
                    if (hpcount > 2)
                    {
                        dr["HORZ_POS_03"] = g.HorizontalBarPlacements[2].Position;
                        dr["HORZ_ANGLE_03"] = g.HorizontalBarPlacements[2].Angle;
                    }
                    if (hpcount > 3)
                    {
                        dr["HORZ_POS_04"] = g.HorizontalBarPlacements[3].Position;
                        dr["HORZ_ANGLE_04"] = g.HorizontalBarPlacements[3].Angle;
                    }
                    if (hpcount > 4)
                    {
                        dr["HORZ_POS_05"] = g.HorizontalBarPlacements[4].Position;
                        dr["HORZ_ANGLE_05"] = g.HorizontalBarPlacements[4].Angle;
                    }
                    if (hpcount > 5)
                    {
                        dr["HORZ_POS_06"] = g.HorizontalBarPlacements[5].Position;
                        dr["HORZ_ANGLE_06"] = g.HorizontalBarPlacements[5].Angle;
                    }
                    if (hpcount > 6)
                    {
                        dr["HORZ_POS_07"] = g.HorizontalBarPlacements[6].Position;
                        dr["HORZ_ANGLE_07"] = g.HorizontalBarPlacements[6].Angle;
                    }
                    if (hpcount > 7)
                    {
                        dr["HORZ_POS_08"] = g.HorizontalBarPlacements[7].Position;
                        dr["HORZ_ANGLE_08"] = g.HorizontalBarPlacements[7].Angle;
                    }
                    if (hpcount > 8)
                    {
                        dr["HORZ_POS_09"] = g.HorizontalBarPlacements[8].Position;
                        dr["HORZ_ANGLE_09"] = g.HorizontalBarPlacements[8].Angle;
                    }
                    if (hpcount > 9)
                    {
                        dr["HORZ_POS_10"] = g.HorizontalBarPlacements[9].Position;
                        dr["HORZ_ANGLE_10"] = g.HorizontalBarPlacements[9].Angle;
                    }
                    if (hpcount > 10)
                    {
                        dr["HORZ_POS_11"] = g.HorizontalBarPlacements[10].Position;
                        dr["HORZ_ANGLE_11"] = g.HorizontalBarPlacements[10].Angle;
                    }
                    if (hpcount > 11)
                    {
                        dr["HORZ_POS_12"] = g.HorizontalBarPlacements[11].Position;
                        dr["HORZ_ANGLE_12"] = g.HorizontalBarPlacements[11].Angle;
                    }
                    if (hpcount > 12)
                    {
                        dr["HORZ_POS_13"] = g.HorizontalBarPlacements[12].Position;
                        dr["HORZ_ANGLE_13"] = g.HorizontalBarPlacements[12].Angle;
                    }
                    if (hpcount > 13)
                    {
                        dr["HORZ_POS_14"] = g.HorizontalBarPlacements[13].Position;
                        dr["HORZ_ANGLE_14"] = g.HorizontalBarPlacements[13].Angle;
                    }
                    if (hpcount > 14)
                    {
                        dr["HORZ_POS_15"] = g.HorizontalBarPlacements[14].Position;
                        dr["HORZ_ANGLE_15"] = g.HorizontalBarPlacements[14].Angle;
                    }
                    if (hpcount > 15)
                    {
                        dr["HORZ_POS_16"] = g.HorizontalBarPlacements[15].Position;
                        dr["HORZ_ANGLE_16"] = g.HorizontalBarPlacements[15].Angle;
                    }
                    if (hpcount > 16)
                    {
                        dr["HORZ_POS_17"] = g.HorizontalBarPlacements[16].Position;
                        dr["HORZ_ANGLE_17"] = g.HorizontalBarPlacements[16].Angle;
                    }
                    if (hpcount > 17)
                    {
                        dr["HORZ_POS_18"] = g.HorizontalBarPlacements[17].Position;
                        dr["HORZ_ANGLE_18"] = g.HorizontalBarPlacements[17].Angle;
                    }
                    if (hpcount > 18)
                    {
                        dr["HORZ_POS_19"] = g.HorizontalBarPlacements[18].Position;
                        dr["HORZ_ANGLE_19"] = g.HorizontalBarPlacements[18].Angle;
                    }
                    if (hpcount > 19)
                    {
                        dr["HORZ_POS_20"] = g.HorizontalBarPlacements[19].Position;
                        dr["HORZ_ANGLE_20"] = g.HorizontalBarPlacements[19].Angle;
                    }

                    // VERTICAL
                    if (vpcount > 0)
                    {
                        dr["VERT_POS_01"] = g.VerticalBarPlacements[0].Position;
                        dr["VERT_ANGLE_01"] = g.VerticalBarPlacements[0].Angle;
                    }
                    if (vpcount > 1)
                    {
                        dr["VERT_POS_02"] = g.VerticalBarPlacements[1].Position;
                        dr["VERT_ANGLE_02"] = g.VerticalBarPlacements[1].Angle;
                    }
                    if (vpcount > 2)
                    {
                        dr["VERT_POS_03"] = g.VerticalBarPlacements[2].Position;
                        dr["VERT_ANGLE_03"] = g.VerticalBarPlacements[2].Angle;
                    }
                    if (vpcount > 3)
                    {
                        dr["VERT_POS_04"] = g.VerticalBarPlacements[3].Position;
                        dr["VERT_ANGLE_04"] = g.VerticalBarPlacements[3].Angle;
                    }
                    if (vpcount > 4)
                    {
                        dr["VERT_POS_05"] = g.VerticalBarPlacements[4].Position;
                        dr["VERT_ANGLE_05"] = g.VerticalBarPlacements[4].Angle;
                    }
                    if (vpcount > 5)
                    {
                        dr["VERT_POS_06"] = g.VerticalBarPlacements[5].Position;
                        dr["VERT_ANGLE_06"] = g.VerticalBarPlacements[5].Angle;
                    }
                    if (vpcount > 6)
                    {
                        dr["VERT_POS_07"] = g.VerticalBarPlacements[6].Position;
                        dr["VERT_ANGLE_07"] = g.VerticalBarPlacements[6].Angle;
                    }
                    if (vpcount > 7)
                    {
                        dr["VERT_POS_08"] = g.VerticalBarPlacements[7].Position;
                        dr["VERT_ANGLE_08"] = g.VerticalBarPlacements[7].Angle;
                    }
                    if (vpcount > 8)
                    {
                        dr["VERT_POS_09"] = g.VerticalBarPlacements[8].Position;
                        dr["VERT_ANGLE_09"] = g.VerticalBarPlacements[8].Angle;
                    }
                    if (vpcount > 9)
                    {
                        dr["VERT_POS_10"] = g.VerticalBarPlacements[9].Position;
                        dr["VERT_ANGLE_10"] = g.VerticalBarPlacements[9].Angle;
                    }
                    if (vpcount > 10)
                    {
                        dr["VERT_POS_11"] = g.VerticalBarPlacements[10].Position;
                        dr["VERT_ANGLE_11"] = g.VerticalBarPlacements[10].Angle;
                    }
                    if (vpcount > 11)
                    {
                        dr["VERT_POS_12"] = g.VerticalBarPlacements[11].Position;
                        dr["VERT_ANGLE_12"] = g.VerticalBarPlacements[11].Angle;
                    }
                    if (vpcount > 12)
                    {
                        dr["VERT_POS_13"] = g.VerticalBarPlacements[12].Position;
                        dr["VERT_ANGLE_13"] = g.VerticalBarPlacements[12].Angle;
                    }
                    if (vpcount > 13)
                    {
                        dr["VERT_POS_14"] = g.VerticalBarPlacements[13].Position;
                        dr["VERT_ANGLE_14"] = g.VerticalBarPlacements[13].Angle;
                    }
                    if (vpcount > 14)
                    {
                        dr["VERT_POS_15"] = g.VerticalBarPlacements[14].Position;
                        dr["VERT_ANGLE_15"] = g.VerticalBarPlacements[14].Angle;
                    }
                    if (vpcount > 15)
                    {
                        dr["VERT_POS_16"] = g.VerticalBarPlacements[15].Position;
                        dr["VERT_ANGLE_16"] = g.VerticalBarPlacements[15].Angle;
                    }
                    if (vpcount > 16)
                    {
                        dr["VERT_POS_17"] = g.VerticalBarPlacements[16].Position;
                        dr["VERT_ANGLE_17"] = g.VerticalBarPlacements[16].Angle;
                    }
                    if (vpcount > 17)
                    {
                        dr["VERT_POS_18"] = g.VerticalBarPlacements[17].Position;
                        dr["VERT_ANGLE_18"] = g.VerticalBarPlacements[17].Angle;
                    }
                    if (vpcount > 18)
                    {
                        dr["VERT_POS_19"] = g.VerticalBarPlacements[18].Position;
                        dr["VERT_ANGLE_19"] = g.VerticalBarPlacements[18].Angle;
                    }
                    if (vpcount > 19)
                    {
                        dr["VERT_POS_20"] = g.VerticalBarPlacements[19].Position;
                        dr["VERT_ANGLE_20"] = g.VerticalBarPlacements[19].Angle;
                    }

                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    var f580021 = line.F580021;
                    var f580024 = line.F580024;
                    var atts = line.ItemAttributes;

                    if (!String.IsNullOrEmpty(f580024.S8Z58GSS1))
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;

                        dr["ITEM_NUM"] = f580021.S7ITM;
                        dr["CONFIG_ID"] = (int)f580021.CMCFGID;

                        dr["GRID_STYLE_CD"] = f580024.S8Z58GSS1;
                        dr["SUB_GRID_STYLE_CD"] = f580024.S8Z5805570;
                        dr["HORZ_BARS_QTY"] = f580024.S8Z58GHA1;
                        dr["VERT_BARS_QTY"] = f580024.S8Z58GVA1;
                        dr["HIGH_OPEN_QTY"] = f580024.S8Z58OHA1;
                        dr["WIDE_OPEN_QTY"] = f580024.S8Z58OWA1;
                        dr["ODD_OPEN_QTY"] = f580024.S8Z58OOA1;

                        dr["RECT_OPEN_QTY"] = atts.GetDecimalAttributeValue("Z5805380");
                        dr["DRAWING_CD"] = f580021.S7Z58DRAW;
                        dr["HORZ_SASH_INTRUSION"] = f580024.S8Z58IGSIH;
                        dr["VERT_SASH_INTRUSION"] = f580024.S8Z58IGSIV;
                        dr["HORZ_PRAIRIE_OFFSET"] = f580024.S8Z5805560;
                        dr["VERT_PRAIRIE_OFFSET"] = f580024.S8Z585555;
                        dr["PLACEMENT_TYPE_CD"] = f580024.S8Z58IPC;

                        dr["HORZ_POS_01"] = f580024.S8Z58IGHP1;
                        dr["HORZ_POS_02"] = f580024.S8Z58IGHP2;
                        dr["HORZ_POS_03"] = f580024.S8Z58IGHP3;
                        dr["HORZ_POS_04"] = f580024.S8Z58IGHP4;
                        dr["HORZ_POS_05"] = f580024.S8Z58IGHP5;
                        dr["HORZ_POS_06"] = f580024.S8Z58IGHP6;
                        dr["HORZ_POS_07"] = f580024.S8Z58IGHP7;
                        dr["HORZ_POS_08"] = f580024.S8Z58IGHP8;
                        dr["HORZ_POS_09"] = f580024.S8Z58IGHP9;
                        dr["HORZ_POS_10"] = f580024.S8Z58IGH10;
                        dr["HORZ_POS_11"] = f580024.S8Z58IGH11;
                        dr["HORZ_POS_12"] = f580024.S8Z58IGH12;
                        dr["HORZ_POS_13"] = f580024.S8Z58IGH13;
                        dr["HORZ_POS_14"] = f580024.S8Z58IGH14;
                        dr["HORZ_POS_15"] = f580024.S8Z58IGH15;
                        dr["HORZ_POS_16"] = f580024.S8Z58IGH16;
                        dr["HORZ_POS_17"] = f580024.S8Z58IGH17;
                        dr["HORZ_POS_18"] = f580024.S8Z58IGH18;
                        dr["HORZ_POS_19"] = f580024.S8Z58IGH19;
                        dr["HORZ_POS_20"] = f580024.S8Z58IGH20;

                        //dr["HORZ_ANGLE_01"] = f580024.S8Z586820;
                        //dr["HORZ_ANGLE_02"] = f580024.S8Z586850;
                        //dr["HORZ_ANGLE_03"] = f580024.S8Z586880;
                        //dr["HORZ_ANGLE_04"] = f580024.S8Z586910;
                        //dr["HORZ_ANGLE_05"] = f580024.S8Z586940;
                        //dr["HORZ_ANGLE_06"] = f580024.S8Z586970;
                        //dr["HORZ_ANGLE_07"] = f580024.S8Z587000;
                        //dr["HORZ_ANGLE_08"] = f580024.S8Z587030;
                        //dr["HORZ_ANGLE_09"] = f580024.S8Z587060;
                        //dr["HORZ_ANGLE_10"] = f580024.S8Z587090;
                        //dr["HORZ_ANGLE_11"] = f580024.S8Z587120;
                        //dr["HORZ_ANGLE_12"] = f580024.S8Z587150;
                        //dr["HORZ_ANGLE_13"] = f580024.S8Z587180;
                        //dr["HORZ_ANGLE_14"] = f580024.S8Z587210;
                        //dr["HORZ_ANGLE_15"] = f580024.S8Z587240;
                        //dr["HORZ_ANGLE_16"] = f580024.S8Z587270;
                        //dr["HORZ_ANGLE_17"] = f580024.S8Z587300;
                        //dr["HORZ_ANGLE_18"] = f580024.S8Z587330;
                        //dr["HORZ_ANGLE_19"] = f580024.S8Z587360;
                        //dr["HORZ_ANGLE_20"] = f580024.S8Z587390;

                        dr["VERT_POS_01"] = f580024.S8Z58IGVP1;
                        dr["VERT_POS_02"] = f580024.S8Z58IGVP2;
                        dr["VERT_POS_03"] = f580024.S8Z58IGVP3;
                        dr["VERT_POS_04"] = f580024.S8Z58IGVP4;
                        dr["VERT_POS_05"] = f580024.S8Z58IGVP5;
                        dr["VERT_POS_06"] = f580024.S8Z58IGVP6;
                        dr["VERT_POS_07"] = f580024.S8Z58IGVP7;
                        dr["VERT_POS_08"] = f580024.S8Z58IGVP8;
                        dr["VERT_POS_09"] = f580024.S8Z58IGVP9;
                        dr["VERT_POS_10"] = f580024.S8Z58IGV10;
                        dr["VERT_POS_11"] = f580024.S8Z58IGV11;
                        dr["VERT_POS_12"] = f580024.S8Z58IGV12;
                        dr["VERT_POS_13"] = f580024.S8Z58IGV13;
                        dr["VERT_POS_14"] = f580024.S8Z58IGV14;
                        dr["VERT_POS_15"] = f580024.S8Z58IGV15;
                        dr["VERT_POS_16"] = f580024.S8Z58IGV16;
                        dr["VERT_POS_17"] = f580024.S8Z58IGV17;
                        dr["VERT_POS_18"] = f580024.S8Z58IGV18;
                        dr["VERT_POS_19"] = f580024.S8Z58IGV19;
                        dr["VERT_POS_20"] = f580024.S8Z58IGV20;

                        //dr["VERT_ANGLE_01"] = f580024.S8Z586220;
                        //dr["VERT_ANGLE_02"] = f580024.S8Z586250;
                        //dr["VERT_ANGLE_03"] = f580024.S8Z586280;
                        //dr["VERT_ANGLE_04"] = f580024.S8Z586310;
                        //dr["VERT_ANGLE_05"] = f580024.S8Z586340;
                        //dr["VERT_ANGLE_06"] = f580024.S8Z586370;
                        //dr["VERT_ANGLE_07"] = f580024.S8Z586400;
                        //dr["VERT_ANGLE_08"] = f580024.S8Z586430;
                        //dr["VERT_ANGLE_09"] = f580024.S8Z586460;
                        //dr["VERT_ANGLE_10"] = f580024.S8Z586490;
                        //dr["VERT_ANGLE_11"] = f580024.S8Z586520;
                        //dr["VERT_ANGLE_12"] = f580024.S8Z586550;
                        //dr["VERT_ANGLE_13"] = f580024.S8Z586580;
                        //dr["VERT_ANGLE_14"] = f580024.S8Z586610;
                        //dr["VERT_ANGLE_15"] = f580024.S8Z586640;
                        //dr["VERT_ANGLE_16"] = f580024.S8Z586670;
                        //dr["VERT_ANGLE_17"] = f580024.S8Z586700;
                        //dr["VERT_ANGLE_18"] = f580024.S8Z586730;
                        //dr["VERT_ANGLE_19"] = f580024.S8Z586760;
                        //dr["VERT_ANGLE_20"] = f580024.S8Z586790;

                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitGridEbsTable : DataTable
    {

        public SpecUnitGridEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitGrid]");
            Columns.Add("SpecUnitId", typeof(int));
            Columns.Add("GridStyle", typeof(string));
            Columns.Add("SubGridStyle", typeof(string));
            Columns.Add("HorzBarQty", typeof(decimal));
            Columns.Add("VertBarQty", typeof(decimal));
            Columns.Add("HighOpenQty", typeof(decimal));
            Columns.Add("WideOpenQty", typeof(decimal));
            Columns.Add("OddOpenQty", typeof(decimal));
            Columns.Add("RectOpenQty", typeof(decimal));
            Columns.Add("DrawingCode", typeof(string));
            Columns.Add("HorzSashIntrusion", typeof(decimal));
            Columns.Add("VertSashIntrusion", typeof(decimal));
            Columns.Add("HorzPrairieOffset", typeof(decimal));
            Columns.Add("VertPrairieOffset", typeof(decimal));
            Columns.Add("PlacementType", typeof(string));
            Columns.Add("HorzPos01", typeof(decimal));
            Columns.Add("HorzPos02", typeof(decimal));
            Columns.Add("HorzPos03", typeof(decimal));
            Columns.Add("HorzPos04", typeof(decimal));
            Columns.Add("HorzPos05", typeof(decimal));
            Columns.Add("HorzPos06", typeof(decimal));
            Columns.Add("HorzPos07", typeof(decimal));
            Columns.Add("HorzPos08", typeof(decimal));
            Columns.Add("HorzPos09", typeof(decimal));
            Columns.Add("HorzPos10", typeof(decimal));
            Columns.Add("HorzPos11", typeof(decimal));
            Columns.Add("HorzPos12", typeof(decimal));
            Columns.Add("HorzPos13", typeof(decimal));
            Columns.Add("HorzPos14", typeof(decimal));
            Columns.Add("HorzPos15", typeof(decimal));
            Columns.Add("HorzPos16", typeof(decimal));
            Columns.Add("HorzPos17", typeof(decimal));
            Columns.Add("HorzPos18", typeof(decimal));
            Columns.Add("HorzPos19", typeof(decimal));
            Columns.Add("HorzPos20", typeof(decimal));
            Columns.Add("VertPos01", typeof(decimal));
            Columns.Add("VertPos02", typeof(decimal));
            Columns.Add("VertPos03", typeof(decimal));
            Columns.Add("VertPos04", typeof(decimal));
            Columns.Add("VertPos05", typeof(decimal));
            Columns.Add("VertPos06", typeof(decimal));
            Columns.Add("VertPos07", typeof(decimal));
            Columns.Add("VertPos08", typeof(decimal));
            Columns.Add("VertPos09", typeof(decimal));
            Columns.Add("VertPos10", typeof(decimal));
            Columns.Add("VertPos11", typeof(decimal));
            Columns.Add("VertPos12", typeof(decimal));
            Columns.Add("VertPos13", typeof(decimal));
            Columns.Add("VertPos14", typeof(decimal));
            Columns.Add("VertPos15", typeof(decimal));
            Columns.Add("VertPos16", typeof(decimal));
            Columns.Add("VertPos17", typeof(decimal));
            Columns.Add("VertPos18", typeof(decimal));
            Columns.Add("VertPos19", typeof(decimal));
            Columns.Add("VertPos20", typeof(decimal));
            Columns.Add("HorzAngle01", typeof(decimal));
            Columns.Add("HorzAngle02", typeof(decimal));
            Columns.Add("HorzAngle03", typeof(decimal));
            Columns.Add("HorzAngle04", typeof(decimal));
            Columns.Add("HorzAngle05", typeof(decimal));
            Columns.Add("HorzAngle06", typeof(decimal));
            Columns.Add("HorzAngle07", typeof(decimal));
            Columns.Add("HorzAngle08", typeof(decimal));
            Columns.Add("HorzAngle09", typeof(decimal));
            Columns.Add("HorzAngle10", typeof(decimal));
            Columns.Add("HorzAngle11", typeof(decimal));
            Columns.Add("HorzAngle12", typeof(decimal));
            Columns.Add("HorzAngle13", typeof(decimal));
            Columns.Add("HorzAngle14", typeof(decimal));
            Columns.Add("HorzAngle15", typeof(decimal));
            Columns.Add("HorzAngle16", typeof(decimal));
            Columns.Add("HorzAngle17", typeof(decimal));
            Columns.Add("HorzAngle18", typeof(decimal));
            Columns.Add("HorzAngle19", typeof(decimal));
            Columns.Add("HorzAngle20", typeof(decimal));
            Columns.Add("VertAngle01", typeof(decimal));
            Columns.Add("VertAngle02", typeof(decimal));
            Columns.Add("VertAngle03", typeof(decimal));
            Columns.Add("VertAngle04", typeof(decimal));
            Columns.Add("VertAngle05", typeof(decimal));
            Columns.Add("VertAngle06", typeof(decimal));
            Columns.Add("VertAngle07", typeof(decimal));
            Columns.Add("VertAngle08", typeof(decimal));
            Columns.Add("VertAngle09", typeof(decimal));
            Columns.Add("VertAngle10", typeof(decimal));
            Columns.Add("VertAngle11", typeof(decimal));
            Columns.Add("VertAngle12", typeof(decimal));
            Columns.Add("VertAngle13", typeof(decimal));
            Columns.Add("VertAngle14", typeof(decimal));
            Columns.Add("VertAngle15", typeof(decimal));
            Columns.Add("VertAngle16", typeof(decimal));
            Columns.Add("VertAngle17", typeof(decimal));
            Columns.Add("VertAngle18", typeof(decimal));
            Columns.Add("VertAngle19", typeof(decimal));
            Columns.Add("VertAngle20", typeof(decimal));
        }

  

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    var f580021 = line.F580021;
                    var f580024 = line.F580024;
                    var atts = line.ItemAttributes;

                    if (!String.IsNullOrEmpty(f580024.S8Z58GSS1))
                    {
                        DataRow dr = NewRow();

                        dr["SpecUnitId"] = line.SpecUnitId;

                        dr["GridStyle"] = f580024.S8Z58GSS1;
                        dr["SubGridStyle"] = f580024.S8Z5805570;
                        dr["HorzBarQty"] = f580024.S8Z58GHA1;
                        dr["VertBarQty"] = f580024.S8Z58GVA1;
                        dr["HighOpenQty"] = f580024.S8Z58OHA1;
                        dr["WideOpenQty"] = f580024.S8Z58OWA1;
                        dr["OddOpenQty"] = f580024.S8Z58OOA1;

                        dr["RectOpenQty"] = atts.GetDecimalAttributeValue("Z5805380");
                        dr["DrawingCode"] = f580021.S7Z58DRAW;
                        dr["HorzSashIntrusion"] = f580024.S8Z58IGSIH;
                        dr["VertSashIntrusion"] = f580024.S8Z58IGSIV;
                        dr["HorzPrairieOffset"] = f580024.S8Z5805560;
                        dr["VertPrairieOffset"] = f580024.S8Z585555;
                        dr["PlacementType"] = f580024.S8Z58IPC;

                        dr["HorzPos01"] = f580024.S8Z58IGHP1;
                        dr["HorzPos02"] = f580024.S8Z58IGHP2;
                        dr["HorzPos03"] = f580024.S8Z58IGHP3;
                        dr["HorzPos04"] = f580024.S8Z58IGHP4;
                        dr["HorzPos05"] = f580024.S8Z58IGHP5;
                        dr["HorzPos06"] = f580024.S8Z58IGHP6;
                        dr["HorzPos07"] = f580024.S8Z58IGHP7;
                        dr["HorzPos08"] = f580024.S8Z58IGHP8;
                        dr["HorzPos09"] = f580024.S8Z58IGHP9;
                        dr["HorzPos10"] = f580024.S8Z58IGH10;
                        dr["HorzPos11"] = f580024.S8Z58IGH11;
                        dr["HorzPos12"] = f580024.S8Z58IGH12;
                        dr["HorzPos13"] = f580024.S8Z58IGH13;
                        dr["HorzPos14"] = f580024.S8Z58IGH14;
                        dr["HorzPos15"] = f580024.S8Z58IGH15;
                        dr["HorzPos16"] = f580024.S8Z58IGH16;
                        dr["HorzPos17"] = f580024.S8Z58IGH17;
                        dr["HorzPos18"] = f580024.S8Z58IGH18;
                        dr["HorzPos19"] = f580024.S8Z58IGH19;
                        dr["HorzPos20"] = f580024.S8Z58IGH20;

                        //dr["HORZ_ANGLE_01"] = f580024.S8Z586820;
                        //dr["HORZ_ANGLE_02"] = f580024.S8Z586850;
                        //dr["HORZ_ANGLE_03"] = f580024.S8Z586880;
                        //dr["HORZ_ANGLE_04"] = f580024.S8Z586910;
                        //dr["HORZ_ANGLE_05"] = f580024.S8Z586940;
                        //dr["HORZ_ANGLE_06"] = f580024.S8Z586970;
                        //dr["HORZ_ANGLE_07"] = f580024.S8Z587000;
                        //dr["HORZ_ANGLE_08"] = f580024.S8Z587030;
                        //dr["HORZ_ANGLE_09"] = f580024.S8Z587060;
                        //dr["HORZ_ANGLE_10"] = f580024.S8Z587090;
                        //dr["HORZ_ANGLE_11"] = f580024.S8Z587120;
                        //dr["HORZ_ANGLE_12"] = f580024.S8Z587150;
                        //dr["HORZ_ANGLE_13"] = f580024.S8Z587180;
                        //dr["HORZ_ANGLE_14"] = f580024.S8Z587210;
                        //dr["HORZ_ANGLE_15"] = f580024.S8Z587240;
                        //dr["HORZ_ANGLE_16"] = f580024.S8Z587270;
                        //dr["HORZ_ANGLE_17"] = f580024.S8Z587300;
                        //dr["HORZ_ANGLE_18"] = f580024.S8Z587330;
                        //dr["HORZ_ANGLE_19"] = f580024.S8Z587360;
                        //dr["HORZ_ANGLE_20"] = f580024.S8Z587390;

                        dr["VertPos01"] = f580024.S8Z58IGVP1;
                        dr["VertPos02"] = f580024.S8Z58IGVP2;
                        dr["VertPos03"] = f580024.S8Z58IGVP3;
                        dr["VertPos04"] = f580024.S8Z58IGVP4;
                        dr["VertPos05"] = f580024.S8Z58IGVP5;
                        dr["VertPos06"] = f580024.S8Z58IGVP6;
                        dr["VertPos07"] = f580024.S8Z58IGVP7;
                        dr["VertPos08"] = f580024.S8Z58IGVP8;
                        dr["VertPos09"] = f580024.S8Z58IGVP9;
                        dr["VertPos10"] = f580024.S8Z58IGV10;
                        dr["VertPos11"] = f580024.S8Z58IGV11;
                        dr["VertPos12"] = f580024.S8Z58IGV12;
                        dr["VertPos13"] = f580024.S8Z58IGV13;
                        dr["VertPos14"] = f580024.S8Z58IGV14;
                        dr["VertPos15"] = f580024.S8Z58IGV15;
                        dr["VertPos16"] = f580024.S8Z58IGV16;
                        dr["VertPos17"] = f580024.S8Z58IGV17;
                        dr["VertPos18"] = f580024.S8Z58IGV18;
                        dr["VertPos19"] = f580024.S8Z58IGV19;
                        dr["VertPos20"] = f580024.S8Z58IGV20;

                        //dr["VERT_ANGLE_01"] = f580024.S8Z586220;
                        //dr["VERT_ANGLE_02"] = f580024.S8Z586250;
                        //dr["VERT_ANGLE_03"] = f580024.S8Z586280;
                        //dr["VERT_ANGLE_04"] = f580024.S8Z586310;
                        //dr["VERT_ANGLE_05"] = f580024.S8Z586340;
                        //dr["VERT_ANGLE_06"] = f580024.S8Z586370;
                        //dr["VERT_ANGLE_07"] = f580024.S8Z586400;
                        //dr["VERT_ANGLE_08"] = f580024.S8Z586430;
                        //dr["VERT_ANGLE_09"] = f580024.S8Z586460;
                        //dr["VERT_ANGLE_10"] = f580024.S8Z586490;
                        //dr["VERT_ANGLE_11"] = f580024.S8Z586520;
                        //dr["VERT_ANGLE_12"] = f580024.S8Z586550;
                        //dr["VERT_ANGLE_13"] = f580024.S8Z586580;
                        //dr["VERT_ANGLE_14"] = f580024.S8Z586610;
                        //dr["VERT_ANGLE_15"] = f580024.S8Z586640;
                        //dr["VERT_ANGLE_16"] = f580024.S8Z586670;
                        //dr["VERT_ANGLE_17"] = f580024.S8Z586700;
                        //dr["VERT_ANGLE_18"] = f580024.S8Z586730;
                        //dr["VERT_ANGLE_19"] = f580024.S8Z586760;
                        //dr["VERT_ANGLE_20"] = f580024.S8Z586790;

                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

