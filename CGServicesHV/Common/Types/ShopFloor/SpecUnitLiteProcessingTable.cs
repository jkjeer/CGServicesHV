using System;
using System.Data;
using System.ServiceModel;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;

namespace CGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitLiteProcessingTable : DataTable
    {

        public SpecUnitLiteProcessingTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_LITE_PROCESSING_LEDGER]");
            Columns.Add("SPEC_UNIT_ID", typeof (long));
            Columns.Add("ITEM_NUM", typeof (decimal));
            Columns.Add("CONFIGURATION_ID", typeof (decimal));
            Columns.Add("LITE_NO", typeof (int));
            Columns.Add("LITE_CD", typeof (string));
            Columns.Add("ROUTING_CD", typeof (string));
            Columns.Add("EDGEWORK_CD", typeof (string));
            Columns.Add("VGROOVE_CD", typeof (string));
            Columns.Add("VGROOVE_SURFACE_CD", typeof (string));
            Columns.Add("VGROOVE_STYLE_CD", typeof (string));
            Columns.Add("VGROOVE_HIGH", typeof (decimal));
            Columns.Add("VGROOVE_WIDE", typeof (decimal));
            Columns.Add("VGROOVE_VERT_OFFSET", typeof (decimal));
            Columns.Add("VGROOVE_HORZ_OFFSET", typeof (decimal));
            Columns.Add("VGROOVE_VERT_START", typeof (string));
            Columns.Add("VGROOVE_HORZ_START", typeof (string));
            Columns.Add("HOLES_DRAWING", typeof (string));
            Columns.Add("HOLES_NO", typeof (decimal));
            Columns.Add("NOTCH_LOCATION_CD", typeof (string));
            Columns.Add("NOTCH_WIDTH", typeof (decimal));
            Columns.Add("NOTCH_HEIGHT", typeof (decimal));
            Columns.Add("ANSI_LOGO_CD", typeof (string));
            Columns.Add("TEMP_LOGO", typeof (string));
            Columns.Add("LOGO_OFFSET_X", typeof (decimal));
            Columns.Add("LOGO_OFFSET_Y", typeof (decimal));
        }

        public void Add(SpecUnits specUnits)
        {
            try
            {
                foreach (var specUnit in specUnits)
                {
                    var atts = specUnit.ItemAttributes;
                    if (atts.GetDecimalAttributeValue("Z58L1TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = specUnit.ItemNum;
                        dr["CONFIGURATION_ID"] = specUnit.ConfigurationId;
                        dr["LITE_NO"] = 1;
                        dr["LITE_CD"] = "EXT";
                        dr["ROUTING_CD"] = atts.GetStringAttributeValue("Z58L1RG");
                        dr["EDGEWORK_CD"] = atts.GetStringAttributeValue("Z58L1EW");
                        dr["VGROOVE_CD"] = atts.GetStringAttributeValue("Z58L1VGT");
                        dr["VGROOVE_SURFACE_CD"] = atts.GetStringAttributeValue("Z58L1VGS");
                        dr["VGROOVE_HIGH"] = atts.GetDecimalAttributeValue("Z58L1VGH");
                        dr["VGROOVE_WIDE"] = atts.GetDecimalAttributeValue("Z58L1VGW");
                        dr["VGROOVE_VERT_OFFSET"] = atts.GetDecimalAttributeValue("Z5801295");
                        dr["VGROOVE_HORZ_OFFSET"] = atts.GetDecimalAttributeValue("Z5801300");
                        dr["VGROOVE_VERT_START"] = atts.GetStringAttributeValue("Z5801296");
                        dr["VGROOVE_HORZ_START"] = atts.GetStringAttributeValue("Z5801301");
                        dr["HOLES_DRAWING"] = atts.GetStringAttributeValue("Z58L1HL");
                        dr["HOLES_NO"] = atts.GetDecimalAttributeValue("Z581365");
                        dr["NOTCH_LOCATION_CD"] = atts.GetStringAttributeValue("Z5801480");
                        dr["NOTCH_HEIGHT"] = atts.GetDecimalAttributeValue("Z5801481");
                        dr["NOTCH_WIDTH"] = atts.GetDecimalAttributeValue("Z5801482");
                        dr["ANSI_LOGO_CD"] = atts.GetStringAttributeValue("Z58L1ALT");
                        dr["LOGO_OFFSET_X"] = atts.GetDecimalAttributeValue("Z58L1LX");
                        dr["LOGO_OFFSET_Y"] = atts.GetDecimalAttributeValue("Z58L1LY");
                        Rows.Add(dr);
                    }
                    if (atts.GetDecimalAttributeValue("Z58L2TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = specUnit.ItemNum;
                        dr["CONFIGURATION_ID"] = specUnit.ConfigurationId;
                        dr["LITE_NO"] = 2;
                        dr["LITE_CD"] = (atts.GetDecimalAttributeValue("Z58L3TK") > 0) ? "MID" : "INT";
                        dr["ROUTING_CD"] = atts.GetStringAttributeValue("Z58L2RG");
                        dr["EDGEWORK_CD"] = atts.GetStringAttributeValue("Z58L2EW");
                        dr["VGROOVE_CD"] = atts.GetStringAttributeValue("Z58L2VGT");
                        dr["VGROOVE_SURFACE_CD"] = atts.GetStringAttributeValue("Z58L2VGS");
                        dr["VGROOVE_HIGH"] = atts.GetDecimalAttributeValue("Z58L2VGH");
                        dr["VGROOVE_WIDE"] = atts.GetDecimalAttributeValue("Z58L2VGW");
                        dr["VGROOVE_VERT_OFFSET"] = atts.GetDecimalAttributeValue("Z5802960");
                        dr["VGROOVE_HORZ_OFFSET"] = atts.GetDecimalAttributeValue("Z5802965");
                        dr["VGROOVE_VERT_START"] = atts.GetStringAttributeValue("Z5802961");
                        dr["VGROOVE_HORZ_START"] = atts.GetStringAttributeValue("Z5802966");
                        dr["HOLES_DRAWING"] = atts.GetStringAttributeValue("Z58L2HL");
                        dr["HOLES_NO"] = atts.GetDecimalAttributeValue("Z581365");
                        dr["NOTCH_LOCATION_CD"] = atts.GetStringAttributeValue("Z5801480");
                        dr["NOTCH_HEIGHT"] = atts.GetDecimalAttributeValue("Z5801481");
                        dr["NOTCH_WIDTH"] = atts.GetDecimalAttributeValue("Z5801482");
                        dr["ANSI_LOGO_CD"] = atts.GetStringAttributeValue("Z58L1ALT");
                        dr["LOGO_OFFSET_X"] = atts.GetDecimalAttributeValue("Z58L1LX");
                        dr["LOGO_OFFSET_Y"] = atts.GetDecimalAttributeValue("Z58L1LY");
                        Rows.Add(dr);
                    }
                    if (atts.GetDecimalAttributeValue("Z58L3TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = specUnit.ItemNum;
                        dr["CONFIGURATION_ID"] = specUnit.ConfigurationId;
                        dr["LITE_NO"] = 3;
                        dr["LITE_CD"] = "INT";
                        dr["ROUTING_CD"] = atts.GetStringAttributeValue("Z58L3RG");
                        dr["EDGEWORK_CD"] = atts.GetStringAttributeValue("Z58L3EW");
                        dr["VGROOVE_CD"] = atts.GetStringAttributeValue("Z58L3VGT");
                        dr["VGROOVE_SURFACE_CD"] = atts.GetStringAttributeValue("Z58L3VGS");
                        dr["VGROOVE_HIGH"] = atts.GetDecimalAttributeValue("Z58L3VGH");
                        dr["VGROOVE_WIDE"] = atts.GetDecimalAttributeValue("Z58L3VGW");
                        dr["VGROOVE_VERT_OFFSET"] = atts.GetDecimalAttributeValue("Z5804545");
                        dr["VGROOVE_HORZ_OFFSET"] = atts.GetDecimalAttributeValue("Z5804550");
                        dr["VGROOVE_VERT_START"] = atts.GetStringAttributeValue("Z5804546");
                        dr["VGROOVE_HORZ_START"] = atts.GetStringAttributeValue("Z5804551");
                        dr["HOLES_DRAWING"] = atts.GetStringAttributeValue("Z58L3HL");
                        dr["HOLES_NO"] = atts.GetDecimalAttributeValue("Z581365");
                        dr["NOTCH_LOCATION_CD"] = atts.GetStringAttributeValue("Z5801480");
                        dr["NOTCH_HEIGHT"] = atts.GetDecimalAttributeValue("Z5801481");
                        dr["NOTCH_WIDTH"] = atts.GetDecimalAttributeValue("Z5801482");
                        dr["ANSI_LOGO_CD"] = atts.GetStringAttributeValue("Z58L1ALT");
                        dr["LOGO_OFFSET_X"] = atts.GetDecimalAttributeValue("Z58L1LX");
                        dr["LOGO_OFFSET_Y"] = atts.GetDecimalAttributeValue("Z58L1LY");
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add(SpecUnit item)
        {
            try
            {
                foreach (Lite lt in item.GlassItem.Lites)
                {
                    DataRow dr = NewRow();
                    dr["ITEM_NUM"] = item.ItemNum;
                    dr["CONFIGURATION_ID"] = item.ConfigurationId;
                    dr["LITE_NO"] = lt.Position;
                    dr["LITE_CD"] = lt.PositionName;
                    dr["ROUTING_CD"] = lt.RoutingCode;
                    dr["EDGEWORK_CD"] = lt.EdgeWork;
                    dr["VGROOVE_CD"] = lt.VGrooveStyle;
                    dr["VGROOVE_SURFACE_CD"] = lt.VGrooveSurface.ToString();
                    dr["VGROOVE_HIGH"] = lt.VGrooveHighOpen;
                    dr["VGROOVE_WIDE"] = lt.VGrooveWideOpen;
                    dr["VGROOVE_VERT_OFFSET"] = lt.VGrooveVertOffset;
                    dr["VGROOVE_HORZ_OFFSET"] = lt.VGrooveHorzOffset;
                    dr["VGROOVE_VERT_START"] = lt.VGrooveVertStartStop;
                    dr["VGROOVE_HORZ_START"] = lt.VGrooveHorzStartStop;
                    dr["HOLES_DRAWING"] = lt.HolesDrawing;
                    dr["HOLES_NO"] = lt.HolesNum;
                    dr["NOTCH_LOCATION_CD"] = lt.NotchLocation;
                    dr["NOTCH_HEIGHT"] = lt.NotchHeight;
                    dr["NOTCH_WIDTH"] = lt.NotchWidth;
                    dr["ANSI_LOGO_CD"] = item.GlassItem.Logo.AnsiLogo;
                    dr["LOGO_OFFSET_X"] = item.GlassItem.Logo.XOffset;
                    dr["LOGO_OFFSET_Y"] = item.GlassItem.Logo.YOffset;
                    Rows.Add(dr);
                }
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
                    if (line.F580021.S7Z58L1TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                        dr["ITEM_NUM"] = line.F580021.S7ITM;
                        dr["CONFIGURATION_ID"] = (int) line.F580021.CMCFGID;
                        dr["LITE_NO"] = 1;
                        dr["LITE_CD"] = "EXT";
                        dr["ROUTING_CD"] = line.F580021.S7Z58L1RG;
                        dr["EDGEWORK_CD"] = line.F580021.S7Z58L1EW;
                        dr["VGROOVE_CD"] = line.F580021.S7Z58L1VGT;
                        dr["VGROOVE_SURFACE_CD"] = line.F580021.S7Z58L1VGS;
                        dr["VGROOVE_HIGH"] = line.F580021.S7Z58L1VGH;
                        dr["VGROOVE_WIDE"] = line.F580021.S7Z58L1VGW;
                        dr["VGROOVE_VERT_OFFSET"] = line.F580024.S8Z5801295;
                        dr["VGROOVE_HORZ_OFFSET"] = line.F580024.S8Z5801300;
                        dr["VGROOVE_VERT_START"] = line.F580024.S8Z5801296;
                        dr["VGROOVE_HORZ_START"] = line.F580024.S8Z5801301;
                        dr["HOLES_DRAWING"] = line.F580021.S7Z58L1HL;
                        dr["HOLES_NO"] = line.F580021.S7Z581365;
                        dr["NOTCH_LOCATION_CD"] = line.F580024.S8Z5801480;
                        dr["NOTCH_HEIGHT"] = line.F580024.S8Z5801481;
                        dr["NOTCH_WIDTH"] = line.F580024.S8Z5801482;
                        dr["ANSI_LOGO_CD"] = line.F580021.S7Z58L1ALT;
                        dr["LOGO_OFFSET_X"] = line.F580021.S7Z58L1LX;
                        dr["LOGO_OFFSET_Y"] = line.F580021.S7Z58L1LY;
                        Rows.Add(dr);
                    }
                    if (line.F580021.S7Z58L2TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                        dr["ITEM_NUM"] = line.F580021.S7ITM;
                        dr["CONFIGURATION_ID"] = (int) line.F580021.CMCFGID;
                        dr["LITE_NO"] = 2;
                        dr["LITE_CD"] = (line.F580021.S7Z58L3TK > 0) ? "MID" : "INT";
                        dr["ROUTING_CD"] = line.F580021.S7Z58L1RG;
                        dr["EDGEWORK_CD"] = line.F580021.S7Z58L1EW;
                        dr["VGROOVE_CD"] = line.F580021.S7Z58L1VGT;
                        dr["VGROOVE_SURFACE_CD"] = line.F580021.S7Z58L1VGS;
                        dr["VGROOVE_HIGH"] = line.F580021.S7Z58L1VGH;
                        dr["VGROOVE_WIDE"] = line.F580021.S7Z58L1VGW;
                        dr["VGROOVE_VERT_OFFSET"] = line.F580024.S8Z5801295;
                        dr["VGROOVE_HORZ_OFFSET"] = line.F580024.S8Z5801300;
                        dr["VGROOVE_VERT_START"] = line.F580024.S8Z5801296;
                        dr["VGROOVE_HORZ_START"] = line.F580024.S8Z5801301;
                        dr["HOLES_DRAWING"] = line.F580021.S7Z58L2HL;
                        dr["HOLES_NO"] = line.F580021.S7Z581365;
                        dr["NOTCH_LOCATION_CD"] = line.F580024.S8Z5801480;
                        dr["NOTCH_HEIGHT"] = line.F580024.S8Z5801481;
                        dr["NOTCH_WIDTH"] = line.F580024.S8Z5801482;
                        dr["ANSI_LOGO_CD"] = line.F580021.S7Z58L1ALT;
                        dr["LOGO_OFFSET_X"] = line.F580021.S7Z58L1LX;
                        dr["LOGO_OFFSET_Y"] = line.F580021.S7Z58L1LY;
                        Rows.Add(dr);
                    }
                    if (line.F580021.S7Z58L3TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                        dr["ITEM_NUM"] = line.F580021.S7ITM;
                        dr["CONFIGURATION_ID"] = (int) line.F580021.CMCFGID;
                        dr["LITE_NO"] = 3;
                        dr["LITE_CD"] = "INT";
                        dr["ROUTING_CD"] = line.F580021.S7Z58L3RG;
                        dr["EDGEWORK_CD"] = line.F580024.S8Z58L3EW;
                        dr["VGROOVE_CD"] = line.F580021.S7Z58L3VGT;
                        dr["VGROOVE_SURFACE_CD"] = line.F580021.S7Z58L3VGS;
                        dr["VGROOVE_HIGH"] = line.F580021.S7Z58L3VGH;
                        dr["VGROOVE_WIDE"] = line.F580021.S7Z58L3VGW;
                        dr["VGROOVE_VERT_OFFSET"] = line.F580024.S8Z5804545;
                        dr["VGROOVE_HORZ_OFFSET"] = line.F580024.S8Z5804550;
                        dr["VGROOVE_VERT_START"] = line.F580024.S8Z5804546;
                        dr["VGROOVE_HORZ_START"] = line.F580024.S8Z5804551;
                        dr["HOLES_DRAWING"] = line.F580021.S7Z58L3HL;
                        dr["HOLES_NO"] = line.F580021.S7Z581365;
                        dr["NOTCH_LOCATION_CD"] = line.F580024.S8Z5801480;
                        dr["NOTCH_HEIGHT"] = line.F580024.S8Z5801481;
                        dr["NOTCH_WIDTH"] = line.F580024.S8Z5801482;
                        dr["ANSI_LOGO_CD"] = line.F580021.S7Z58L1ALT;
                        dr["LOGO_OFFSET_X"] = line.F580021.S7Z58L1LX;
                        dr["LOGO_OFFSET_Y"] = line.F580021.S7Z58L1LY;
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
    public class SpecGlassProcessingEbsTable : DataTable
    {

        public SpecGlassProcessingEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitLiteProcessing]");
            Columns.Add("SpecUnitId", typeof(int));
            Columns.Add("LiteNum", typeof(int));
            Columns.Add("LiteCode", typeof(string));
            Columns.Add("RoutingCode", typeof(string));
            Columns.Add("EdgeWork", typeof(string));
            Columns.Add("Bevel", typeof(string));
            Columns.Add("VGrooveCode", typeof(string));
            Columns.Add("VGrooveSurface", typeof(string));
            Columns.Add("VGrooveStyle", typeof(string));
            Columns.Add("VGrooveHigh", typeof(decimal));
            Columns.Add("VGrooveWide", typeof(decimal));
            Columns.Add("VGrooveOpen", typeof(decimal));
            Columns.Add("VGrooveVertOffset", typeof(decimal));
            Columns.Add("VGrooveHorzOffset", typeof(decimal));
            Columns.Add("VGrooveVertStart", typeof(string));
            Columns.Add("VGrooveHorzStart", typeof(string));
            Columns.Add("VGrooveDrawing", typeof(string));
            Columns.Add("HolesDrawing", typeof(string));
            Columns.Add("HolesQty", typeof(decimal));
            Columns.Add("NotchLocation", typeof(string));
            Columns.Add("NotchWidth", typeof(decimal));
            Columns.Add("NotchHeight", typeof(decimal));
            Columns.Add("SilkSurface", typeof(decimal));
            Columns.Add("SilkDrawing", typeof(string));
            Columns.Add("AnsiLogo", typeof(string));
            Columns.Add("TempLogo", typeof(string));
            Columns.Add("LogoOffsetX", typeof(decimal));
            Columns.Add("LogoOffsetY", typeof(decimal));
            Columns.Add("EdgeWorkSides", typeof(string));
            Columns.Add("FabricationDrawing", typeof(string));
            Columns.Add("FabricationDesc", typeof(string));
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    if (line.F580021.S7Z58L1TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SpecUnitId"] = line.SpecUnitId;
                        dr["LiteNum"] = 1;
                        dr["LiteCode"] = "EXT";
                        dr["RoutingCode"] = line.F580021.S7Z58L1RG;
                        dr["EdgeWork"] = line.F580021.S7Z58L1EW;
                        dr["Bevel"] = line.F580021.S7Z58L1BL;
                        dr["VGrooveCode"] = line.F580021.S7Z58L1VGT;
                        dr["VGrooveSurface"] = line.F580021.S7Z58L1VGS;
                        dr["VGrooveHigh"] = line.F580021.S7Z58L1VGH;
                        dr["VGrooveWide"] = line.F580021.S7Z58L1VGW;
                        dr["VGrooveOpen"] = line.F580021.S7Z58L1VGO;
                        dr["VGrooveVertOffset"] = line.F580024.S8Z5801295;
                        dr["VGrooveHorzOffset"] = line.F580024.S8Z5801300;
                        dr["VGrooveVertStart"] = line.F580024.S8Z5801296;
                        dr["VGrooveHorzStart"] = line.F580024.S8Z5801301;
                        dr["VGrooveDrawing"] = line.F580021.S7Z58L1VGD;
                        dr["HolesDrawing"] = line.F580021.S7Z58L1HL;
                        dr["HolesQty"] = line.F580021.S7Z581365;
                        dr["NotchLocation"] = line.F580024.S8Z5801480;
                        dr["NotchHeight"] = line.F580024.S8Z5801481;
                        dr["NotchWidth"] = line.F580024.S8Z5801482;
                        dr["SilkSurface"] = line.F580021.S7Z58L1SF;
                        dr["SilkDrawing"] = line.F580021.S7Z58L1SD;
                        dr["AnsiLogo"] = line.F580021.S7Z58L1ALT;
                        dr["LogoOffsetX"] = line.F580021.S7Z58L1LX;
                        dr["LogoOffsetY"] = line.F580021.S7Z58L1LY;
                        dr["EdgeWorkSides"] = line.F580024.S8Z5853002;
                        dr["FabricationDrawing"] = line.F580024.S8Z5888100;
                        dr["FabricationDesc"] = line.F580024.S8Z58FABDE;
                        Rows.Add(dr);
                    }
                    if (line.F580021.S7Z58L2TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SpecUnitId"] = line.SpecUnitId;
                        dr["LiteNum"] = 2;
                        dr["LiteCode"] = (line.F580021.S7Z58L3TK > 0) ? "MID" : "INT";
                        dr["RoutingCode"] = line.F580021.S7Z58L2RG;
                        dr["EdgeWork"] = line.F580021.S7Z58L2EW;
                        dr["Bevel"] = line.F580021.S7Z58L2BL;
                        dr["VGrooveCode"] = line.F580021.S7Z58L2VGT;
                        dr["VGrooveSurface"] = line.F580021.S7Z58L2VGS;
                        dr["VGrooveHigh"] = line.F580021.S7Z58L2VGH;
                        dr["VGrooveWide"] = line.F580021.S7Z58L2VGW;
                        dr["VGrooveOpen"] = line.F580021.S7Z58L2VGO;
                        dr["VGrooveVertOffset"] = line.F580024.S8Z5801295;
                        dr["VGrooveHorzOffset"] = line.F580024.S8Z5801300;
                        dr["VGrooveVertStart"] = line.F580024.S8Z5801296;
                        dr["VGrooveHorzStart"] = line.F580024.S8Z5801301;
                        dr["VGrooveDrawing"] = line.F580021.S7Z58L2VGD;
                        dr["HolesDrawing"] = line.F580021.S7Z58L2HL;
                        dr["HolesQty"] = line.F580021.S7Z581365;
                        dr["NotchLocation"] = line.F580024.S8Z5801480;
                        dr["NotchHeight"] = line.F580024.S8Z5801481;
                        dr["NotchWidth"] = line.F580024.S8Z5801482;
                        dr["SilkSurface"] = line.F580021.S7Z58L2SF;
                        dr["SilkDrawing"] = line.F580021.S7Z58L2SD;
                        dr["AnsiLogo"] = line.F580021.S7Z58L1ALT;
                        dr["LogoOffsetX"] = line.F580021.S7Z58L1LX;
                        dr["LogoOffsetY"] = line.F580021.S7Z58L1LY;
                        Rows.Add(dr);
                    }
                    if (line.F580021.S7Z58L3TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SpecUnitId"] = line.SpecUnitId;
                        dr["LiteNum"] = 3;
                        dr["LiteCode"] = "INT";
                        dr["RoutingCode"] = line.F580021.S7Z58L3RG;
                        dr["EdgeWork"] = line.F580024.S8Z58L3EW;
                        dr["Bevel"] = line.F580021.S7Z58L3BL;
                        dr["VGrooveCode"] = line.F580021.S7Z58L3VGT;
                        dr["VGrooveSurface"] = line.F580021.S7Z58L3VGS;
                        dr["VGrooveHigh"] = line.F580021.S7Z58L3VGH;
                        dr["VGrooveWide"] = line.F580021.S7Z58L3VGW;
                        dr["VGrooveOpen"] = line.F580021.S7Z58L3VGO;
                        dr["VGrooveVertOffset"] = line.F580024.S8Z5804545;
                        dr["VGrooveHorzOffset"] = line.F580024.S8Z5804550;
                        dr["VGrooveVertStart"] = line.F580024.S8Z5804546;
                        dr["VGrooveHorzStart"] = line.F580024.S8Z5804551;
                        dr["VGrooveDrawing"] = line.F580021.S7Z58L3VGD;
                        dr["HolesDrawing"] = line.F580021.S7Z58L3HL;
                        dr["HolesQty"] = line.F580021.S7Z581365;
                        dr["NotchLocation"] = line.F580024.S8Z5801480;
                        dr["NotchHeight"] = line.F580024.S8Z5801481;
                        dr["NotchWidth"] = line.F580024.S8Z5801482;
                        dr["SilkSurface"] = line.F580021.S7Z58L3SF;
                        dr["SilkDrawing"] = line.F580021.S7Z58L3SD;
                        dr["AnsiLogo"] = line.F580021.S7Z58L1ALT;
                        dr["LogoOffsetX"] = line.F580021.S7Z58L1LX;
                        dr["LogoOffsetY"] = line.F580021.S7Z58L1LY;
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
