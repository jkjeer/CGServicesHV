using System;
using System.Data;
using System.ServiceModel;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;

namespace CGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitLiteGlassTypeTable : DataTable
    {

        public SpecUnitLiteGlassTypeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_LITE_GLASS_TYPE_LEDGER]");
            Columns.Add("SPEC_UNIT_ID", typeof(long));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("LITE_NO", typeof(decimal));
            Columns.Add("LITE_CD", typeof(string));
            Columns.Add("THICKNESS_NO", typeof(decimal));
            Columns.Add("HEAT_TREATMENT_CD", typeof(string));
            Columns.Add("COLOR_CD", typeof(string));
            Columns.Add("SURFACE1_COAT_CD", typeof(string));
            Columns.Add("SURFACE2_COAT_CD", typeof(string));
            Columns.Add("PATTERN_TEXTURE_CD", typeof(string));
            Columns.Add("PATTERN_SURFACE_CD", typeof(string));
            Columns.Add("LAMI_TYPE_CD", typeof(string));
            Columns.Add("LAMI_COLOR_SURFACE_CD", typeof(string));
            Columns.Add("LAMI_PATTERN_SURFACE_CD", typeof(string));
            Columns.Add("LAMI_COATING_SURFACE_CD", typeof(string));
            Columns.Add("QUALITY_CD", typeof(string));
            Columns.Add("IS_CUSTOMER_SUPPLIED", typeof(bool));
            Columns.Add("CUSTOMER_LITE_NUM", typeof(string));
        }

        /*********************************************************************************
         * Item Attributes - Configurator
         *******************************************************************************/
        public void Add(SpecUnits items)
        {
            try
            {
                foreach (SpecUnit item in items)
                {
                    var attributes = item.ItemAttributes;

                    if (attributes.GetDecimalAttributeValue("Z58L1TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIGURATION_ID"] = (int) item.ConfigurationId;
                        dr["LITE_NO"] = 1;
                        dr["LITE_CD"] = "EXT";
                        dr["THICKNESS_NO"] = attributes.GetDecimalAttributeValue("Z58L1TK");
                        dr["HEAT_TREATMENT_CD"] = attributes.GetStringAttributeValue("Z58L1HT");
                        dr["COLOR_CD"] = attributes.GetStringAttributeDesc3("Z58L1CL", "CL");
                        dr["SURFACE1_COAT_CD"] = attributes.GetStringAttributeValue("Z58L11C");
                        dr["SURFACE2_COAT_CD"] = attributes.GetStringAttributeValue("Z58L12C");
                        dr["PATTERN_TEXTURE_CD"] = attributes.GetStringAttributeValue("Z58L1PX");
                        dr["PATTERN_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L1PTS");
                        dr["LAMI_TYPE_CD"] = attributes.GetStringAttributeValue("Z58L1LPT");
                        dr["LAMI_COLOR_SURFACE_CD"] = attributes.GetStringAttributeValue("Z5801890");
                        dr["LAMI_PATTERN_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L1LPS");
                        dr["LAMI_COATING_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L1LXS");
                        dr["QUALITY_CD"] = attributes.GetStringAttributeValue("Z58L1PQL");
                        dr["IS_CUSTOMER_SUPPLIED"] = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                                     attributes.GetStringAttributeValue("Z5840450").Contains("1");
                        dr["CUSTOMER_LITE_NUM"] = attributes.GetStringAttributeValue("Z58L1CLN");
                        Rows.Add(dr);
                    }
                    if (attributes.GetDecimalAttributeValue("Z58L2TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIGURATION_ID"] = (int) item.ConfigurationId;
                        dr["LITE_NO"] = 2;
                        dr["LITE_CD"] = (attributes.GetDecimalAttributeValue("Z58L3TK") > 0) ? "MID" : "INT";
                        dr["THICKNESS_NO"] = attributes.GetDecimalAttributeValue("Z58L2TK");
                        dr["HEAT_TREATMENT_CD"] = attributes.GetStringAttributeValue("Z58L2HT");
                        dr["COLOR_CD"] = attributes.GetStringAttributeDesc3("Z58L2CL", "CL");
                        dr["SURFACE1_COAT_CD"] = attributes.GetStringAttributeValue("Z58L14C");
                        dr["SURFACE2_COAT_CD"] = attributes.GetStringAttributeValue("Z58L13C");
                        dr["PATTERN_TEXTURE_CD"] = attributes.GetStringAttributeValue("Z58L2PX");
                        dr["PATTERN_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L2PTS");
                        dr["LAMI_TYPE_CD"] = attributes.GetStringAttributeValue("Z58L2LPT");
                        dr["LAMI_COLOR_SURFACE_CD"] = attributes.GetStringAttributeValue("Z5803510");
                        dr["LAMI_PATTERN_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L2LPS");
                        dr["LAMI_COATING_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L2LXS");
                        dr["QUALITY_CD"] = attributes.GetStringAttributeValue("Z58L1PQL");
                        dr["IS_CUSTOMER_SUPPLIED"] = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                                     attributes.GetStringAttributeValue("Z5840450").Contains("2");
                        dr["CUSTOMER_LITE_NUM"] = attributes.GetStringAttributeValue("Z58L1CLN");
                        Rows.Add(dr);
                    }
                    if (attributes.GetDecimalAttributeValue("Z58L3TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIGURATION_ID"] = (int) item.ConfigurationId;
                        dr["LITE_NO"] = 3;
                        dr["LITE_CD"] = "INT";
                        dr["THICKNESS_NO"] = attributes.GetDecimalAttributeValue("Z58L3TK");
                        dr["HEAT_TREATMENT_CD"] = attributes.GetStringAttributeValue("Z58L3HT");
                        dr["COLOR_CD"] = attributes.GetStringAttributeDesc3("Z58L3CL", "CL");
                        dr["SURFACE1_COAT_CD"] = attributes.GetStringAttributeValue("Z58L15C");
                        dr["SURFACE2_COAT_CD"] = attributes.GetStringAttributeValue("Z58L16C");
                        dr["PATTERN_TEXTURE_CD"] = attributes.GetStringAttributeValue("Z58L3PX");
                        dr["PATTERN_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L3PTS");
                        dr["LAMI_TYPE_CD"] = attributes.GetStringAttributeValue("Z58L3LPT");
                        dr["LAMI_COLOR_SURFACE_CD"] = attributes.GetStringAttributeValue("Z5805100");
                        dr["LAMI_PATTERN_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L3LPS");
                        dr["LAMI_COATING_SURFACE_CD"] = attributes.GetStringAttributeValue("Z58L3LXS");
                        dr["QUALITY_CD"] = attributes.GetStringAttributeValue("Z58L1PQL");
                        dr["IS_CUSTOMER_SUPPLIED"] = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                                     attributes.GetStringAttributeValue("Z5840450").Contains("3");
                        dr["CUSTOMER_LITE_NUM"] = attributes.GetStringAttributeValue("Z58L1CLN");
                        Rows.Add(dr);
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /*********************************************************************************
         * E1 Scheduling Interface - F580021/24
         *******************************************************************************/
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
                        dr["CONFIGURATION_ID"] = (int)line.F580021.CMCFGID;
                        dr["LITE_NO"] = 1;
                        dr["LITE_CD"] = "EXT";
                        dr["THICKNESS_NO"] = line.F580021.S7Z58L1TK;
                        dr["HEAT_TREATMENT_CD"] = line.F580021.S7Z58L1HT;
                        dr["COLOR_CD"] = line.F580021.S7Z58L1CL;
                        dr["SURFACE1_COAT_CD"] = line.F580021.S7Z58L11C;
                        dr["SURFACE2_COAT_CD"] = line.F580021.S7Z58L12C;
                        dr["PATTERN_TEXTURE_CD"] = line.F580021.S7Z58L1PX;
                        dr["PATTERN_SURFACE_CD"] = line.F580021.S7Z58L1PTS;
                        dr["LAMI_TYPE_CD"] = line.F580021.S7Z58L1LPT;
                        dr["LAMI_COLOR_SURFACE_CD"] = line.F580024.S8Z5801890;
                        dr["LAMI_PATTERN_SURFACE_CD"] = line.F580024.S8Z58L1LPS;
                        dr["LAMI_COATING_SURFACE_CD"] = line.F580024.S8Z58L1LXS;
                        dr["QUALITY_CD"] = line.F580021.S7Z58L1PQL;
                        dr["IS_CUSTOMER_SUPPLIED"] = line.F580024.S8Z5840450 != null && line.F580024.S8Z5840450.Contains("1");
                        dr["CUSTOMER_LITE_NUM"] = line.F580021.S7Z58L1CLN;
                        Rows.Add(dr);
                    }
                    if (line.F580021.S7Z58L2TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                        dr["ITEM_NUM"] = line.F580021.S7ITM;
                        dr["CONFIGURATION_ID"] = (int)line.F580021.CMCFGID;
                        dr["LITE_NO"] = 2;
                        dr["LITE_CD"] = (line.F580021.S7Z58L3TK > 0) ? "MID" : "INT";
                        dr["THICKNESS_NO"] = line.F580021.S7Z58L2TK;
                        dr["HEAT_TREATMENT_CD"] = line.F580021.S7Z58L2HT;
                        dr["COLOR_CD"] = line.F580021.S7Z58L2CL;
                        dr["SURFACE1_COAT_CD"] = line.F580021.S7Z58L24C;
                        dr["SURFACE2_COAT_CD"] = line.F580021.S7Z58L23C;
                        dr["PATTERN_TEXTURE_CD"] = line.F580021.S7Z58L2PX;
                        dr["PATTERN_SURFACE_CD"] = line.F580021.S7Z58L2PTS;
                        dr["LAMI_TYPE_CD"] = line.F580021.S7Z58L2LPT;
                        dr["LAMI_COLOR_SURFACE_CD"] = line.F580024.S8Z5803510;
                        dr["LAMI_PATTERN_SURFACE_CD"] = line.F580024.S8Z58L2LPS;
                        dr["LAMI_COATING_SURFACE_CD"] = line.F580024.S8Z58L2LXS;
                        dr["QUALITY_CD"] = line.F580021.S7Z58L1PQL;
                        dr["IS_CUSTOMER_SUPPLIED"] = line.F580024.S8Z5840450 != null && line.F580024.S8Z5840450.Contains("2");
                        dr["CUSTOMER_LITE_NUM"] = line.F580021.S7Z58L1CLN;
                        Rows.Add(dr);
                    }
                    if (line.F580021.S7Z58L3TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                        dr["ITEM_NUM"] = line.F580021.S7ITM;
                        dr["CONFIGURATION_ID"] = (int)line.F580021.CMCFGID;
                        dr["LITE_NO"] = 3;
                        dr["LITE_CD"] = "INT";
                        dr["THICKNESS_NO"] = line.F580021.S7Z58L3TK;
                        dr["HEAT_TREATMENT_CD"] = line.F580021.S7Z58L3HT;
                        dr["COLOR_CD"] = line.F580021.S7Z58L3CL;
                        dr["SURFACE1_COAT_CD"] = line.F580021.S7Z58L35C;
                        dr["SURFACE2_COAT_CD"] = line.F580021.S7Z58L36C;
                        dr["PATTERN_TEXTURE_CD"] = line.F580021.S7Z58L3PX;
                        //dr["PATTERN_SURFACE_CD"] = line.F580021.S7Z58L3PTS;
                        dr["LAMI_TYPE_CD"] = line.F580021.S7Z58L3LPT;
                        dr["LAMI_COLOR_SURFACE_CD"] = line.F580024.S8Z5805100;
                        dr["LAMI_PATTERN_SURFACE_CD"] = line.F580024.S8Z58L3LPS;
                        dr["LAMI_COATING_SURFACE_CD"] = line.F580024.S8Z58L3LXS;
                        dr["QUALITY_CD"] = line.F580021.S7Z58L1PQL;
                        dr["IS_CUSTOMER_SUPPLIED"] = line.F580024.S8Z5840450 != null && line.F580024.S8Z5840450.Contains("3");
                        dr["CUSTOMER_LITE_NUM"] = line.F580021.S7Z58L1CLN;
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /*********************************************************************************
         * Enterprise Business Object - Glass Item
         *******************************************************************************/
        public void Add(SpecUnit item)
        {
            try
            {
                foreach (Lite lt in item.GlassItem.Lites)
                {
                    DataRow dr = NewRow();
                    var egt = lt.GlassType;
                    dr["ITEM_NUM"] = item.ItemNum;
                    dr["CONFIGURATION_ID"] = item.ConfigurationId;
                    dr["LITE_NO"] = lt.Position;
                    dr["LITE_CD"] = lt.PositionName;
                    dr["THICKNESS_NO"] = egt.Thickness;
                    dr["HEAT_TREATMENT_CD"] = egt.HeatTreatment;
                    dr["COLOR_CD"] = egt.ColorCode;
                    dr["SURFACE1_COAT_CD"] = egt.Surface1Coating;
                    dr["SURFACE2_COAT_CD"] = egt.Surface2Coating;
                    dr["PATTERN_TEXTURE_CD"] = egt.PatternTexture;
                    dr["PATTERN_SURFACE_CD"] = egt.PatternTextureSrf;
                    dr["LAMI_TYPE_CD"] = egt.LamiProductType;
                    dr["LAMI_COLOR_SURFACE_CD"] = egt.LamiColorSurface;
                    dr["LAMI_PATTERN_SURFACE_CD"] = egt.LamiPatternSurface;
                    dr["LAMI_COATING_SURFACE_CD"] = egt.LamiCoatingSurface;
                    dr["QUALITY_CD"] = lt.QualityLevel;
                    dr["IS_CUSTOMER_SUPPLIED"] = egt.CustomerSupplied;
                    //dr["CUSTOMER_LITE_NUM"] = egt.CustomerLiteNum;
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

    }


    /*********************************************************************************
     * EBS Interface
     *******************************************************************************/
    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecGlassTypeEbsTable : DataTable
    {

        public SpecGlassTypeEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitLiteGlassType]");
            Columns.Add("SpecUnitId", typeof (int));
            Columns.Add("LiteNum", typeof (int));
            Columns.Add("LiteCode", typeof (string));
            Columns.Add("Thickness", typeof (decimal));
            Columns.Add("HeatTreatment", typeof (string));
            Columns.Add("Color", typeof (string));
            Columns.Add("Surface1Coating", typeof (string));
            Columns.Add("Surface2Coating", typeof (string));
            Columns.Add("Pattern", typeof (string));
            Columns.Add("PatternSurface", typeof (string));
            Columns.Add("LamiType", typeof (string));
            Columns.Add("LamiColorSurface", typeof (string));
            Columns.Add("LamiPatternSurface", typeof (string));
            Columns.Add("LamiCoatingSurface", typeof (string));
            Columns.Add("Quality", typeof (string));
            Columns.Add("IsCustomerSupplied", typeof (bool));
            Columns.Add("CustomerLiteNum", typeof (string));
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
                        dr["Thickness"] = line.F580021.S7Z58L1TK / 10;
                        dr["HeatTreatment"] = line.F580021.S7Z58L1HT;
                        dr["Color"] = line.F580021.S7Z58L1CL;
                        dr["Surface1Coating"] = line.F580021.S7Z58L11C;
                        dr["Surface2Coating"] = line.F580021.S7Z58L12C;
                        dr["Pattern"] = line.F580021.S7Z58L1PX;
                        dr["PatternSurface"] = line.F580021.S7Z58L1PTS;
                        dr["LamiType"] = line.F580021.S7Z58L1LPT;
                        dr["LamiColorSurface"] = line.F580024.S8Z5801890;
                        dr["LamiPatternSurface"] = line.F580024.S8Z58L1LPS;
                        dr["LamiCoatingSurface"] = line.F580024.S8Z58L1LXS;
                        dr["Quality"] = line.F580021.S7Z58L1PQL;
                        dr["IsCustomerSupplied"] = line.F580024.S8Z5840450 != null &&
                                                     line.F580024.S8Z5840450.Contains("1");
                        dr["CustomerLiteNum"] = line.F580021.S7Z58L1CLN;
                        Rows.Add(dr);
                    }
                    if (line.F580021.S7Z58L2TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SpecUnitId"] = line.SpecUnitId;
                        dr["LiteNum"] = 2;
                        dr["LiteCode"] = (line.F580021.S7Z58L3TK > 0) ? "MID" : "INT";
                        dr["Thickness"] = line.F580021.S7Z58L2TK / 10;
                        dr["HeatTreatment"] = line.F580021.S7Z58L2HT;
                        dr["Color"] = line.F580021.S7Z58L2CL;
                        dr["Surface1Coating"] = line.F580021.S7Z58L24C;
                        dr["Surface2Coating"] = line.F580021.S7Z58L23C;
                        dr["Pattern"] = line.F580021.S7Z58L2PX;
                        dr["PatternSurface"] = line.F580021.S7Z58L2PTS;
                        dr["LamiType"] = line.F580021.S7Z58L2LPT;
                        dr["LamiColorSurface"] = line.F580024.S8Z5803510;
                        dr["LamiPatternSurface"] = line.F580024.S8Z58L2LPS;
                        dr["LamiCoatingSurface"] = line.F580024.S8Z58L2LXS;
                        dr["Quality"] = line.F580021.S7Z58L1PQL;
                        dr["IsCustomerSupplied"] = line.F580024.S8Z5840450 != null &&
                                                     line.F580024.S8Z5840450.Contains("2");
                        dr["CustomerLiteNum"] = line.F580021.S7Z58L1CLN;
                        Rows.Add(dr);
                    }
                    if (line.F580021.S7Z58L3TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SpecUnitId"] = line.SpecUnitId;
                        dr["LiteNum"] = 3;
                        dr["LiteCode"] = "INT";
                        dr["Thickness"] = line.F580021.S7Z58L3TK / 10;
                        dr["HeatTreatment"] = line.F580021.S7Z58L3HT;
                        dr["Color"] = line.F580021.S7Z58L3CL;
                        dr["Surface1Coating"] = line.F580021.S7Z58L35C;
                        dr["Surface2Coating"] = line.F580021.S7Z58L36C;
                        dr["Pattern"] = line.F580021.S7Z58L3PX;
                        //dr["PatternSurface"] = line.F580021.S7Z58L3PTS;
                        dr["LamiType"] = line.F580021.S7Z58L3LPT;
                        dr["LamiColorSurface"] = line.F580024.S8Z5805100;
                        dr["LamiPatternSurface"] = line.F580024.S8Z58L3LPS;
                        dr["LamiCoatingSurface"] = line.F580024.S8Z58L3LXS;
                        dr["Quality"] = line.F580021.S7Z58L1PQL;
                        dr["IsCustomerSupplied"] = line.F580024.S8Z5840450 != null &&
                                                     line.F580024.S8Z5840450.Contains("3");
                        dr["CustomerLiteNum"] = line.F580021.S7Z58L1CLN;
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
