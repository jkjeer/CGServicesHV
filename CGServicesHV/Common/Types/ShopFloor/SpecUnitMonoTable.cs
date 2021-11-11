using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServices.Common.Managers.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Utilities;

namespace CGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitMonoTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }


        public SpecUnitMonoTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_MONO_LEDGER]");

            // ID
            Columns.Add("SPEC_UNIT_ID", typeof(int));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("UNIT_TYPE_CD", typeof(string));

            // CUSTOMER
            Columns.Add("PRODUCT_TYPE", typeof(string));
            Columns.Add("CARDINAL_PRODUCT_TYPE", typeof(string));
            Columns.Add("CUSTOMER_SOLD_NUM", typeof(decimal));
            Columns.Add("CUSTOMER_SHIP_NUM", typeof(decimal));
            Columns.Add("CUSTOMER_PART_NUM1", typeof(string));
            Columns.Add("CUSTOMER_PART_NUM2", typeof(string));
            Columns.Add("CUSTOMER_PART_NUM3", typeof(string));
            Columns.Add("ORIG_CUSTOMER_SOLD_NUM", typeof(decimal));
            Columns.Add("ORIG_CUSTOMER_SHIP_NUM", typeof(decimal));
            Columns.Add("ORIG_BRANCH_PLANT", typeof(string));

            // SHAPE
            Columns.Add("SHAPE_NO", typeof(decimal));
            Columns.Add("BASE_DIMENSION", typeof(decimal));
            Columns.Add("BASE_DIMENSION_MM", typeof(decimal));
            Columns.Add("BASE_DIMENSION_INT", typeof(int));
            Columns.Add("BASE_DIMENSION_NUM", typeof(int));
            Columns.Add("BASE_DIMENSION_DEN", typeof(int));
            Columns.Add("BASE_DIMENSION_DESC", typeof(string));
            Columns.Add("LEFT_DIMENSION", typeof(decimal));
            Columns.Add("LEFT_DIMENSION_MM", typeof(decimal));
            Columns.Add("LEFT_DIMENSION_INT", typeof(int));
            Columns.Add("LEFT_DIMENSION_NUM", typeof(int));
            Columns.Add("LEFT_DIMENSION_DEN", typeof(int));
            Columns.Add("LEFT_DIMENSION_DESC", typeof(string));
            Columns.Add("RIGHT_DIMENSION", typeof(decimal));
            Columns.Add("RIGHT_DIMENSION_MM", typeof(decimal));
            Columns.Add("RIGHT_DIMENSION_INT", typeof(int));
            Columns.Add("RIGHT_DIMENSION_NUM", typeof(int));
            Columns.Add("RIGHT_DIMENSION_DEN", typeof(int));
            Columns.Add("RIGHT_DIMENSION_DESC", typeof(string));
            Columns.Add("TOP_DIMENSION", typeof(decimal));
            Columns.Add("TOP_DIMENSION_MM", typeof(decimal));
            Columns.Add("TOP_DIMENSION_INT", typeof(int));
            Columns.Add("TOP_DIMENSION_NUM", typeof(int));
            Columns.Add("TOP_DIMENSION_DEN", typeof(int));
            Columns.Add("TOP_DIMENSION_DESC", typeof(string));
            Columns.Add("SIDE1_DIMENSION", typeof(decimal));
            Columns.Add("SIDE1_DIMENSION_MM", typeof(decimal));
            Columns.Add("SIDE1_DIMENSION_INT", typeof(int));
            Columns.Add("SIDE1_DIMENSION_NUM", typeof(int));
            Columns.Add("SIDE1_DIMENSION_DEN", typeof(int));
            Columns.Add("SIDE1_DIMENSION_DESC", typeof(string));
            Columns.Add("SIDE2_DIMENSION", typeof(decimal));
            Columns.Add("SIDE2_DIMENSION_MM", typeof(decimal));
            Columns.Add("SIDE2_DIMENSION_INT", typeof(int));
            Columns.Add("SIDE2_DIMENSION_NUM", typeof(int));
            Columns.Add("SIDE2_DIMENSION_DEN", typeof(int));
            Columns.Add("SIDE2_DIMENSION_DESC", typeof(string));
            Columns.Add("ACTUAL_SQFT", typeof(decimal));
            Columns.Add("BILLABLE_SQFT", typeof(decimal));
            Columns.Add("PERIMETER_BLOCK_SIZE", typeof(decimal));
            Columns.Add("GLASS_UNIT_LBS", typeof(decimal));

            // LITE 1 GLASS TYPE
            Columns.Add("LITE1_THICKNESS_NO", typeof(decimal));
            Columns.Add("LITE1_HEAT_TREATMENT_CD", typeof(string));
            Columns.Add("LITE1_COLOR_CD", typeof(string));
            Columns.Add("LITE1_SURFACE1_COAT_CD", typeof(string));
            Columns.Add("LITE1_SURFACE2_COAT_CD", typeof(string));
            Columns.Add("LITE1_PATTERN_TEXTURE_CD", typeof(string));
            Columns.Add("LITE1_PATTERN_SURFACE_CD", typeof(string));
            Columns.Add("LITE1_LAMI_TYPE_CD", typeof(string));
            Columns.Add("LITE1_LAMI_COLOR_SURFACE_CD", typeof(string));
            Columns.Add("LITE1_LAMI_PATTERN_SURFACE_CD", typeof(string));
            Columns.Add("LITE1_LAMI_COATING_SURFACE_CD", typeof(string));
            Columns.Add("LITE1_QUALITY_CD", typeof(string));
            Columns.Add("LITE1_IS_CUSTOMER_SUPPLIED", typeof(bool));
            Columns.Add("LITE1_CUSTOMER_LITE_NUM", typeof(string));

            // LITE 1 PROCESSING
            Columns.Add("LITE1_ROUTING_CD", typeof(string));
            Columns.Add("LITE1_EDGEWORK_CD", typeof(string));
            Columns.Add("LITE1_BEVEL_CD", typeof(string));
            Columns.Add("LITE1_VGROOVE_CD", typeof(string));
            Columns.Add("LITE1_VGROOVE_SURFACE_CD", typeof(string));
            Columns.Add("LITE1_VGROOVE_STYLE_CD", typeof(string));
            Columns.Add("LITE1_VGROOVE_HIGH", typeof(decimal));
            Columns.Add("LITE1_VGROOVE_WIDE", typeof(decimal));
            Columns.Add("LITE1_VGROOVE_VERT_OFFSET", typeof(decimal));
            Columns.Add("LITE1_VGROOVE_HORZ_OFFSET", typeof(decimal));
            Columns.Add("LITE1_VGROOVE_VERT_START", typeof(string));
            Columns.Add("LITE1_VGROOVE_HORZ_START", typeof(string));
            Columns.Add("LITE1_HOLES_DRAWING", typeof(string));
            Columns.Add("LITE1_HOLES_NO", typeof(decimal));
            Columns.Add("LITE1_NOTCH_LOCATION_CD", typeof(string));
            Columns.Add("LITE1_NOTCH_WIDTH", typeof(decimal));
            Columns.Add("LITE1_NOTCH_HEIGHT", typeof(decimal));
            Columns.Add("SILK_SURFACE", typeof(decimal));
            Columns.Add("SILK_DRAWING", typeof(string));
            Columns.Add("EDGE_WORK_SIDES", typeof(string));
            Columns.Add("FABRICATION_DRAWING", typeof(string));
            Columns.Add("FABRICATION_MAKEUP", typeof(string));
            Columns.Add("PL_SIDE", typeof(string));
            Columns.Add("QC_INSPECT_FLAG", typeof(string));
            Columns.Add("EDGE_DELETE_WIDTH", typeof(string));

            Columns.Add("LITE1_ANSI_LOGO_CD", typeof(string));
            Columns.Add("LITE1_TEMP_LOGO_CD", typeof(string));
            Columns.Add("LITE1_LOGO_OFFSET_X", typeof(decimal));
            Columns.Add("LITE1_LOGO_OFFSET_Y", typeof(decimal));
            Columns.Add("IG_LOGO", typeof(string));
            Columns.Add("NFRC_LOGO_1", typeof(string));
            Columns.Add("NFRC_LOGO_2", typeof(string));
            Columns.Add("NFRC_LOGO_VERT_OFFSET", typeof(decimal));
            Columns.Add("NFRC_LOGO_HORZ_OFFSET", typeof(decimal));

            // SURFACE
            Columns.Add("EXT_PRESERVE_COLOR", typeof(string));
            Columns.Add("EXT_PRESERVE_CUTBACK", typeof(string));
            Columns.Add("EXT_SCORING_WIDTH", typeof(decimal));
            Columns.Add("INT_PRESERVE_COLOR", typeof(string));
            Columns.Add("INT_PRESERVE_CUTBACK", typeof(string));
            Columns.Add("INT_SCORING_WIDTH", typeof(decimal));
            Columns.Add("OPACIFIER", typeof(string));
            Columns.Add("OVERLAY_BAR", typeof(string));
            Columns.Add("PRESERVE_PROCESSING_CD", typeof(string));

            // GRID
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

            // RACK
            Columns.Add("RACK_ORIENT_ROWS", typeof(decimal));
            Columns.Add("RACK_ORIENT_VH", typeof(string));
            Columns.Add("RACK_ORIENT_IO", typeof(string));
            Columns.Add("RACK_ORIENT_TIERS", typeof(decimal));
            Columns.Add("RACK_TYPE", typeof(string));
            Columns.Add("LITES_PER_RACK", typeof(decimal));
            Columns.Add("CRATE_CD", typeof(string));


            // META
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }


        public void Add(SpecUnits specUnits)
        {
            try
            {
                foreach (var specUnit in specUnits)
                {
                    DataRow dr = NewRow();

                    // ID
                    dr["SPEC_UNIT_ID"] = specUnit.SpecUnitId;
                    dr["ITEM_NUM"] = specUnit.ItemNum;
                    dr["CONFIGURATION_ID"] = specUnit.ConfigurationId;
                    dr["UNIT_TYPE_CD"] = specUnit.UnitType;

                    // CUSTOMER
                    dr["PRODUCT_TYPE"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58PT", String.Empty);
                    dr["CARDINAL_PRODUCT_TYPE"] = specUnit.ItemAttributes.GetStringAttributeValue("Z5840310", String.Empty);
                    dr["CUSTOMER_PART_NUM1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58CP2", String.Empty);
                    dr["CUSTOMER_PART_NUM2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58CP3", String.Empty);
                    dr["CUSTOMER_PART_NUM3"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58CP4", String.Empty);
                    dr["CUSTOMER_SOLD_NUM"] = specUnit.ItemAttributes.GetDecimalAttributeValue("AN8DR");
                    dr["CUSTOMER_SHIP_NUM"] = specUnit.ItemAttributes.GetDecimalAttributeValue("SHAN");
                    dr["ORIG_CUSTOMER_SOLD_NUM"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58CTL");
                    dr["ORIG_CUSTOMER_SHIP_NUM"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5840425");
                    dr["ORIG_BRANCH_PLANT"] = specUnit.ItemAttributes.GetStringAttributeValue("Z5840415", String.Empty);

                    // SHAPE
                    var atts = specUnit.ItemAttributes;

                    dr["SHAPE_NO"] = atts.GetDecimalAttributeValue("Z58SHAPE");
                    dr["ACTUAL_SQFT"] = atts.GetDecimalAttributeValue("Z58USF");
                    dr["BILLABLE_SQFT"] = atts.GetDecimalAttributeValue("Z58BSF");
                    dr["PERIMETER_BLOCK_SIZE"] = atts.GetDecimalAttributeValue("Z5850970");
                    dr["GLASS_UNIT_LBS"] = (specUnit.GlassWeightLbs == 0) ? atts.GetDecimalAttributeValue("Z58GLWT") /
                                   ((atts.GetDecimalAttributeValue("Z5840360") == 0)
                                        ? 1
                                        : atts.GetDecimalAttributeValue("Z5840360")) : specUnit.GlassWeightLbs;

                    var baseDimension = new Dimension
                    {
                        DecimalValue = atts.GetDecimalAttributeValue("Z581BDEC"),
                        MMValue = atts.GetDecimalAttributeValue("Z581BMIM"),
                        Base = atts.GetDecimalAttributeValue("Z58L1BDM"),
                        Numerator = atts.GetDecimalAttributeValue("Z58L1BNM"),
                        Denominator = atts.GetDecimalAttributeValue("Z58L1BDN")
                    };
                    dr["BASE_DIMENSION"] = baseDimension.DecimalValue;
                    dr["BASE_DIMENSION_MM"] = baseDimension.MMValue;
                    dr["BASE_DIMENSION_INT"] = baseDimension.Base;
                    dr["BASE_DIMENSION_NUM"] = baseDimension.Numerator;
                    dr["BASE_DIMENSION_DEN"] = baseDimension.Denominator;
                    dr["BASE_DIMENSION_DESC"] = baseDimension.GetDescription();

                    var leftDimension = new Dimension
                    {
                        DecimalValue = atts.GetDecimalAttributeValue("Z581LDEC"),
                        MMValue = atts.GetDecimalAttributeValue("Z581LMIM"),
                        Base = atts.GetDecimalAttributeValue("Z58L1LDM"),
                        Numerator = atts.GetDecimalAttributeValue("Z58L1LNM"),
                        Denominator = atts.GetDecimalAttributeValue("Z58L1LDN")
                    };
                    dr["LEFT_DIMENSION"] = leftDimension.DecimalValue;
                    dr["LEFT_DIMENSION_MM"] = leftDimension.MMValue;
                    dr["LEFT_DIMENSION_INT"] = leftDimension.Base;
                    dr["LEFT_DIMENSION_NUM"] = leftDimension.Numerator;
                    dr["LEFT_DIMENSION_DEN"] = leftDimension.Denominator;
                    dr["LEFT_DIMENSION_DESC"] = leftDimension.GetDescription();

                    var rightDimension = new Dimension
                    {
                        DecimalValue = atts.GetDecimalAttributeValue("Z581RDEC"),
                        MMValue = atts.GetDecimalAttributeValue("Z581RMIM"),
                        Base = atts.GetDecimalAttributeValue("Z58L1RDM"),
                        Numerator = atts.GetDecimalAttributeValue("Z58L1RNM"),
                        Denominator = atts.GetDecimalAttributeValue("Z58L1RDN")
                    };
                    dr["RIGHT_DIMENSION"] = rightDimension.DecimalValue;
                    dr["RIGHT_DIMENSION_MM"] = rightDimension.MMValue;
                    dr["RIGHT_DIMENSION_INT"] = rightDimension.Base;
                    dr["RIGHT_DIMENSION_NUM"] = rightDimension.Numerator;
                    dr["RIGHT_DIMENSION_DEN"] = rightDimension.Denominator;
                    dr["RIGHT_DIMENSION_DESC"] = rightDimension.GetDescription();

                    var topDimension = new Dimension
                    {
                        DecimalValue = atts.GetDecimalAttributeValue("Z581TDEC"),
                        MMValue = atts.GetDecimalAttributeValue("Z581TMIM"),
                        Base = atts.GetDecimalAttributeValue("Z58L1TDM"),
                        Numerator = atts.GetDecimalAttributeValue("Z58L1TNM"),
                        Denominator = atts.GetDecimalAttributeValue("Z58L1TDN")
                    };
                    dr["TOP_DIMENSION"] = topDimension.DecimalValue;
                    dr["TOP_DIMENSION_MM"] = topDimension.MMValue;
                    dr["TOP_DIMENSION_INT"] = topDimension.Base;
                    dr["TOP_DIMENSION_NUM"] = topDimension.Numerator;
                    dr["TOP_DIMENSION_DEN"] = topDimension.Denominator;
                    dr["TOP_DIMENSION_DESC"] = topDimension.GetDescription();

                    var side1Dimension = new Dimension
                    {
                        DecimalValue = atts.GetDecimalAttributeValue("Z5811DEC"),
                        MMValue = atts.GetDecimalAttributeValue("Z5811MIM"),
                        Base = atts.GetDecimalAttributeValue("Z58L11DM"),
                        Numerator = atts.GetDecimalAttributeValue("Z58L11NM"),
                        Denominator = atts.GetDecimalAttributeValue("Z58L11DN")
                    };
                    dr["SIDE1_DIMENSION"] = side1Dimension.DecimalValue;
                    dr["SIDE1_DIMENSION_MM"] = side1Dimension.MMValue;
                    dr["SIDE1_DIMENSION_INT"] = side1Dimension.Base;
                    dr["SIDE1_DIMENSION_NUM"] = side1Dimension.Numerator;
                    dr["SIDE1_DIMENSION_DEN"] = side1Dimension.Denominator;
                    dr["SIDE1_DIMENSION_DESC"] = side1Dimension.GetDescription();

                    var side2Dimension = new Dimension
                    {
                        DecimalValue = atts.GetDecimalAttributeValue("Z5812DEC"),
                        MMValue = atts.GetDecimalAttributeValue("Z5812MIM"),
                        Base = atts.GetDecimalAttributeValue("Z58L12DM"),
                        Numerator = atts.GetDecimalAttributeValue("Z58L12NM"),
                        Denominator = atts.GetDecimalAttributeValue("Z58L12DN")
                    };
                    dr["SIDE2_DIMENSION"] = side2Dimension.DecimalValue;
                    dr["SIDE2_DIMENSION_MM"] = side2Dimension.MMValue;
                    dr["SIDE2_DIMENSION_INT"] = side2Dimension.Base;
                    dr["SIDE2_DIMENSION_NUM"] = side2Dimension.Numerator;
                    dr["SIDE2_DIMENSION_DEN"] = side2Dimension.Denominator;
                    dr["SIDE2_DIMENSION_DESC"] = side2Dimension.GetDescription();

                    // LITE 1 GLASS TYPE
                    dr["LITE1_THICKNESS_NO"] = atts.GetDecimalAttributeValue("Z58L1TK");
                    dr["LITE1_HEAT_TREATMENT_CD"] = atts.GetStringAttributeValue("Z58L1HT", String.Empty);
                    dr["LITE1_COLOR_CD"] = atts.GetStringAttributeDesc3("Z58L1CL", "CL");
                    dr["LITE1_SURFACE1_COAT_CD"] = atts.GetStringAttributeValue("Z58L11C", String.Empty);
                    dr["LITE1_SURFACE2_COAT_CD"] = atts.GetStringAttributeValue("Z58L12C", String.Empty);
                    dr["LITE1_PATTERN_TEXTURE_CD"] = atts.GetStringAttributeValue("Z58L1PX", String.Empty);
                    dr["LITE1_PATTERN_SURFACE_CD"] = atts.GetStringAttributeValue("Z58L1PTS", String.Empty);
                    dr["LITE1_LAMI_TYPE_CD"] = atts.GetStringAttributeValue("Z58L1LPT", String.Empty);
                    dr["LITE1_LAMI_COLOR_SURFACE_CD"] = atts.GetStringAttributeValue("Z5801890", String.Empty);
                    dr["LITE1_LAMI_PATTERN_SURFACE_CD"] = atts.GetStringAttributeValue("Z58L1LPS", String.Empty);
                    dr["LITE1_LAMI_COATING_SURFACE_CD"] = atts.GetStringAttributeValue("Z58L1LXS", String.Empty);
                    dr["LITE1_QUALITY_CD"] = atts.GetStringAttributeValue("Z58L1PQL", String.Empty);
                    dr["LITE1_IS_CUSTOMER_SUPPLIED"] = (atts.GetStringAttributeValue("Z5840450", String.Empty) != null) &&
                                                 atts.GetStringAttributeValue("Z5840450", String.Empty).Contains("1");
                    dr["LITE1_CUSTOMER_LITE_NUM"] = atts.GetStringAttributeValue("Z58L1CLN", String.Empty);

                    // LITE 1 PROCESSING
                    dr["LITE1_ROUTING_CD"] = atts.GetStringAttributeValue("Z58L1RG", String.Empty);
                    dr["LITE1_BEVEL_CD"] = atts.GetStringAttributeValue("Z58L1BL", String.Empty);
                    dr["LITE1_EDGEWORK_CD"] = atts.GetStringAttributeValue("Z58L1EW", String.Empty);
                    dr["LITE1_VGROOVE_CD"] = atts.GetStringAttributeValue("Z58L1VG", String.Empty);
                    dr["LITE1_VGROOVE_SURFACE_CD"] = atts.GetStringAttributeValue("Z58L1VGS", String.Empty);
                    dr["LITE1_VGROOVE_STYLE_CD"] = atts.GetStringAttributeValue("Z58L1VGT", String.Empty);
                    dr["LITE1_VGROOVE_HIGH"] = atts.GetDecimalAttributeValue("Z58L1VGH");
                    dr["LITE1_VGROOVE_WIDE"] = atts.GetDecimalAttributeValue("Z58L1VGW");
                    dr["LITE1_VGROOVE_VERT_OFFSET"] = atts.GetDecimalAttributeValue("Z5801295");
                    dr["LITE1_VGROOVE_HORZ_OFFSET"] = atts.GetDecimalAttributeValue("Z5801300");
                    dr["LITE1_VGROOVE_VERT_START"] = atts.GetStringAttributeValue("Z5801296", String.Empty);
                    dr["LITE1_VGROOVE_HORZ_START"] = atts.GetStringAttributeValue("Z5801301", String.Empty);
                    dr["LITE1_HOLES_DRAWING"] = atts.GetStringAttributeValue("Z58L1HL", String.Empty);
                    dr["LITE1_HOLES_NO"] = atts.GetDecimalAttributeValue("Z581365");
                    dr["LITE1_NOTCH_LOCATION_CD"] = atts.GetStringAttributeValue("Z5801480", String.Empty);
                    dr["LITE1_NOTCH_HEIGHT"] = atts.GetDecimalAttributeValue("Z5801481");
                    dr["LITE1_NOTCH_WIDTH"] = atts.GetDecimalAttributeValue("Z5801482");
                    dr["SILK_SURFACE"] = atts.GetDecimalAttributeValue("Z58L1SF");
                    dr["SILK_DRAWING"] = atts.GetStringAttributeValue("Z58L1SD", String.Empty);
                    dr["FABRICATION_DRAWING"] = atts.GetStringAttributeValue("Z5888100", String.Empty);
                    dr["FABRICATION_MAKEUP"] = atts.GetStringAttributeValue("Z58FABDE", String.Empty);
                    dr["EDGE_WORK_SIDES"] = atts.GetStringAttributeValue("Z5853002", String.Empty);
                    dr["PL_SIDE"] = atts.GetStringAttributeValue("Z5853001", String.Empty);
                    dr["QC_INSPECT_FLAG"] = atts.GetStringAttributeValue("Z5801840", String.Empty);
                    dr["EDGE_DELETE_WIDTH"] = atts.GetStringAttributeValue("Z5805680", String.Empty);

                    dr["LITE1_ANSI_LOGO_CD"] = atts.GetStringAttributeValue("Z58L1ALT", String.Empty);
                    dr["LITE1_TEMP_LOGO_CD"] = atts.GetStringAttributeValue("Z58L1TL", String.Empty);
                    dr["LITE1_LOGO_OFFSET_X"] = atts.GetDecimalAttributeValue("Z58L1LX");
                    dr["LITE1_LOGO_OFFSET_Y"] = atts.GetDecimalAttributeValue("Z58L1LY");
                    dr["IG_LOGO"] = specUnit.ItemAttributes.GetStringAttributeValue("Z5840430", String.Empty);
                    dr["NFRC_LOGO_1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58NFRC", String.Empty);
                    dr["NFRC_LOGO_2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58NFRC2", String.Empty);
                    dr["NFRC_LOGO_HORZ_OFFSET"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58NFRCH");
                    dr["NFRC_LOGO_VERT_OFFSET"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58NFRCV");

                    // SURFACE
                    dr["EXT_PRESERVE_COLOR"] = atts.GetStringAttributeValue("Z587566", String.Empty); 
                    dr["EXT_PRESERVE_CUTBACK"] = atts.GetStringAttributeValue("Z587563", String.Empty);  
                    dr["EXT_SCORING_WIDTH"] = atts.GetDecimalAttributeValue("Z587569");
                    dr["INT_PRESERVE_COLOR"] = atts.GetStringAttributeValue("Z587578", String.Empty);
                    dr["INT_PRESERVE_CUTBACK"] = atts.GetStringAttributeValue("Z587575", String.Empty);
                    dr["INT_SCORING_WIDTH"] = atts.GetDecimalAttributeValue("Z587581");
                    dr["OPACIFIER"] = atts.GetStringAttributeValue("Z587584", String.Empty);
                    dr["OVERLAY_BAR"] = atts.GetStringAttributeValue("Z5805735", String.Empty);
                    dr["PRESERVE_PROCESSING_CD"] = atts.GetStringAttributeValue("Z5800520", String.Empty);

                    //GRID
                    dr["GRID_STYLE_CD"] = atts.GetStringAttributeValue("Z58GSS1", String.Empty);
                    dr["SUB_GRID_STYLE_CD"] = atts.GetStringAttributeValue("Z5805570", String.Empty);
                    dr["HORZ_BARS_QTY"] = atts.GetDecimalAttributeValue("Z58GHA1");  
                    dr["VERT_BARS_QTY"] = atts.GetDecimalAttributeValue("Z58GVA1"); 
                    dr["HIGH_OPEN_QTY"] = atts.GetDecimalAttributeValue("Z58OHA1"); 
                    dr["WIDE_OPEN_QTY"] = atts.GetDecimalAttributeValue("Z58OWA1"); 
                    dr["ODD_OPEN_QTY"] = atts.GetDecimalAttributeValue("Z58OOA1");
                    dr["RECT_OPEN_QTY"] = atts.GetDecimalAttributeValue("Z5805380");
                    dr["DRAWING_CD"] = atts.GetStringAttributeValue("Z58DRAW", String.Empty);
                    dr["HORZ_SASH_INTRUSION"] = atts.GetDecimalAttributeValue("Z58IGSIH");
                    dr["VERT_SASH_INTRUSION"] = atts.GetDecimalAttributeValue("Z58IGSIV");
                    dr["HORZ_PRAIRIE_OFFSET"] = atts.GetDecimalAttributeValue("Z5805560");
                    dr["VERT_PRAIRIE_OFFSET"] = atts.GetDecimalAttributeValue("Z585555");
                    dr["PLACEMENT_TYPE_CD"] = atts.GetStringAttributeValue("Z58IPC", String.Empty);
                    dr["HORZ_POS_01"] = atts.GetDecimalAttributeValue("Z58IGHP1");
                    dr["HORZ_POS_02"] = atts.GetDecimalAttributeValue("Z58IGHP2");
                    dr["HORZ_POS_03"] = atts.GetDecimalAttributeValue("Z58IGHP3");
                    dr["HORZ_POS_04"] = atts.GetDecimalAttributeValue("Z58IGHP4");
                    dr["HORZ_POS_05"] = atts.GetDecimalAttributeValue("Z58IGHP5");
                    dr["HORZ_POS_06"] = atts.GetDecimalAttributeValue("Z58IGHP6");
                    dr["HORZ_POS_07"] = atts.GetDecimalAttributeValue("Z58IGHP7");
                    dr["HORZ_POS_08"] = atts.GetDecimalAttributeValue("Z58IGHP8");
                    dr["HORZ_POS_09"] = atts.GetDecimalAttributeValue("Z58IGHP9");
                    dr["HORZ_POS_10"] = atts.GetDecimalAttributeValue("Z58IGH10");
                    dr["HORZ_POS_11"] = atts.GetDecimalAttributeValue("Z58IGH11");
                    dr["HORZ_POS_12"] = atts.GetDecimalAttributeValue("Z58IGH12");
                    dr["HORZ_POS_13"] = atts.GetDecimalAttributeValue("Z58IGH13");
                    dr["HORZ_POS_14"] = atts.GetDecimalAttributeValue("Z58IGH14");
                    dr["HORZ_POS_15"] = atts.GetDecimalAttributeValue("Z58IGH15");
                    dr["HORZ_POS_16"] = atts.GetDecimalAttributeValue("Z58IGH16");
                    dr["HORZ_POS_17"] = atts.GetDecimalAttributeValue("Z58IGH17");
                    dr["HORZ_POS_18"] = atts.GetDecimalAttributeValue("Z58IGH18");
                    dr["HORZ_POS_19"] = atts.GetDecimalAttributeValue("Z58IGH19");
                    dr["HORZ_POS_20"] = atts.GetDecimalAttributeValue("Z58IGH20");
                    dr["VERT_POS_01"] = atts.GetDecimalAttributeValue("Z58IGVP1");
                    dr["VERT_POS_02"] = atts.GetDecimalAttributeValue("Z58IGVP2");
                    dr["VERT_POS_03"] = atts.GetDecimalAttributeValue("Z58IGVP3");
                    dr["VERT_POS_04"] = atts.GetDecimalAttributeValue("Z58IGVP4");
                    dr["VERT_POS_05"] = atts.GetDecimalAttributeValue("Z58IGVP5");
                    dr["VERT_POS_06"] = atts.GetDecimalAttributeValue("Z58IGVP6");
                    dr["VERT_POS_07"] = atts.GetDecimalAttributeValue("Z58IGVP7");
                    dr["VERT_POS_08"] = atts.GetDecimalAttributeValue("Z58IGVP8");
                    dr["VERT_POS_09"] = atts.GetDecimalAttributeValue("Z58IGVP9");
                    dr["VERT_POS_10"] = atts.GetDecimalAttributeValue("Z58IGV10");
                    dr["VERT_POS_11"] = atts.GetDecimalAttributeValue("Z58IGV11");
                    dr["VERT_POS_12"] = atts.GetDecimalAttributeValue("Z58IGV12");
                    dr["VERT_POS_13"] = atts.GetDecimalAttributeValue("Z58IGV13");
                    dr["VERT_POS_14"] = atts.GetDecimalAttributeValue("Z58IGV14");
                    dr["VERT_POS_15"] = atts.GetDecimalAttributeValue("Z58IGV15");
                    dr["VERT_POS_16"] = atts.GetDecimalAttributeValue("Z58IGV16");
                    dr["VERT_POS_17"] = atts.GetDecimalAttributeValue("Z58IGV17");
                    dr["VERT_POS_18"] = atts.GetDecimalAttributeValue("Z58IGV18");
                    dr["VERT_POS_19"] = atts.GetDecimalAttributeValue("Z58IGV19");
                    dr["VERT_POS_20"] = atts.GetDecimalAttributeValue("Z58IGV20");

                    // RACK
                    dr["LITES_PER_RACK"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58LPR");
                    dr["RACK_ORIENT_ROWS"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58ROW");
                    dr["RACK_ORIENT_VH"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RPRVH", String.Empty);
                    dr["RACK_ORIENT_IO"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58RTNIO");
                    dr["RACK_ORIENT_TIERS"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58STK");
                    dr["RACK_TYPE"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RT", String.Empty);
                    dr["CRATE_CD"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58CRATE", String.Empty);

                    // META
                    dr["UPDATE_DATE"] = specUnit.TransactionDate;

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
                    DataRow dr = NewRow();

                    // ID
                    dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                    dr["ITEM_NUM"] = line.F580021.S7ITM;
                    dr["CONFIGURATION_ID"] = line.F580021.CMCFGID;
                    dr["UNIT_TYPE_CD"] = line.ItemAttributes.GetStringAttributeValue("Z5840275");

                    // CUSTOMER
                    dr["PRODUCT_TYPE"] = line.F580021.S7Z58PT;
                    dr["CARDINAL_PRODUCT_TYPE"] = line.F580024.S8Z5840310;
                    dr["CUSTOMER_PART_NUM1"] = line.F580021.S7Z58CP2;
                    dr["CUSTOMER_PART_NUM2"] = line.F580021.S7Z58CP3;
                    dr["CUSTOMER_PART_NUM3"] = line.F580021.S7Z58CP4;
                    dr["CUSTOMER_SOLD_NUM"] = line.F580021.S7AN8DR;
                    dr["CUSTOMER_SHIP_NUM"] = line.F580021.S7SHAN;
                    dr["ORIG_CUSTOMER_SOLD_NUM"] = line.F580021.S7Z58CTL;
                    dr["ORIG_CUSTOMER_SHIP_NUM"] = line.ItemAttributes.GetDecimalAttributeValue("Z5840425");
                    dr["ORIG_BRANCH_PLANT"] = line.F580024.S8Z5840415;

                    // SHAPE
                    dr["SHAPE_NO"] = Data.ParseStringToDecimal(line.F580021.S7Z58SHAPE);
                    dr["ACTUAL_SQFT"] = line.F580024.S8Z58USF / 10000;
                    dr["BILLABLE_SQFT"] = line.F580024.S8Z58BSF / 10000;
                    dr["PERIMETER_BLOCK_SIZE"] = line.ItemAttributes.GetDecimalAttributeValue("Z5800970");
                    dr["GLASS_UNIT_LBS"] = line.F580024.S8Z58GLWT / 1000  / line.F580021.S7Z58QTY;

                    var baseDimension = new Dimension
                    {
                        DecimalValue = line.F580021.S7Z581BDEC / 1000,
                        MMValue = line.F580021.S7Z581BMIM / 10,
                        Base = (int)line.F580021.S7Z58L1BDM,
                        Numerator = (int)line.F580021.S7Z58L1BNM,
                        Denominator = (int)line.F580021.S7Z58L1BDN
                    };
                    dr["BASE_DIMENSION"] = baseDimension.DecimalValue;
                    dr["BASE_DIMENSION_MM"] = baseDimension.MMValue;
                    dr["BASE_DIMENSION_INT"] = baseDimension.Base;
                    dr["BASE_DIMENSION_NUM"] = baseDimension.Numerator;
                    dr["BASE_DIMENSION_DEN"] = baseDimension.Denominator;
                    dr["BASE_DIMENSION_DESC"] = baseDimension.GetDescription();

                    var leftDimension = new Dimension
                    {
                        DecimalValue = line.F580021.S7Z581LDEC / 1000,
                        MMValue = line.F580021.S7Z581LMIM / 10,
                        Base = (int)line.F580021.S7Z58L1LDM,
                        Numerator = (int)line.F580021.S7Z58L1LNM,
                        Denominator = (int)line.F580021.S7Z58L1LDN
                    };
                    dr["LEFT_DIMENSION"] = leftDimension.DecimalValue;
                    dr["LEFT_DIMENSION_MM"] = leftDimension.MMValue;
                    dr["LEFT_DIMENSION_INT"] = leftDimension.Base;
                    dr["LEFT_DIMENSION_NUM"] = leftDimension.Numerator;
                    dr["LEFT_DIMENSION_DEN"] = leftDimension.Denominator;
                    dr["LEFT_DIMENSION_DESC"] = leftDimension.GetDescription();

                    var rightDimension = new Dimension
                    {
                        DecimalValue = line.F580021.S7Z581RDEC / 1000,
                        MMValue = line.F580021.S7Z581RMIM / 10,
                        Base = (int)line.F580021.S7Z58L1RDM,
                        Numerator = (int)line.F580021.S7Z58L1RNM,
                        Denominator = (int)line.F580021.S7Z58L1RDN
                    };
                    dr["RIGHT_DIMENSION"] = rightDimension.DecimalValue;
                    dr["RIGHT_DIMENSION_MM"] = rightDimension.MMValue;
                    dr["RIGHT_DIMENSION_INT"] = rightDimension.Base;
                    dr["RIGHT_DIMENSION_NUM"] = rightDimension.Numerator;
                    dr["RIGHT_DIMENSION_DEN"] = rightDimension.Denominator;
                    dr["RIGHT_DIMENSION_DESC"] = rightDimension.GetDescription();

                    var topDimension = new Dimension
                    {
                        DecimalValue = line.F580021.S7Z581TDEC / 1000,
                        MMValue = line.F580021.S7Z581TMIM / 10,
                        Base = (int)line.F580021.S7Z58L1TDM,
                        Numerator = (int)line.F580021.S7Z58L1TNM,
                        Denominator = (int)line.F580021.S7Z58L1TDN
                    };
                    dr["TOP_DIMENSION"] = topDimension.DecimalValue;
                    dr["TOP_DIMENSION_MM"] = topDimension.MMValue;
                    dr["TOP_DIMENSION_INT"] = topDimension.Base;
                    dr["TOP_DIMENSION_NUM"] = topDimension.Numerator;
                    dr["TOP_DIMENSION_DEN"] = topDimension.Denominator;
                    dr["TOP_DIMENSION_DESC"] = topDimension.GetDescription();

                    var side1Dimension = new Dimension
                    {
                        DecimalValue = line.F580021.S7Z5811DEC / 1000,
                        MMValue = line.F580021.S7Z5811MIM / 10,
                        Base = (int)line.F580021.S7Z58L11DM,
                        Numerator = (int)line.F580021.S7Z58L11NM,
                        Denominator = (int)line.F580021.S7Z58L11DN
                    };
                    dr["SIDE1_DIMENSION"] = side1Dimension.DecimalValue;
                    dr["SIDE1_DIMENSION_MM"] = side1Dimension.MMValue;
                    dr["SIDE1_DIMENSION_INT"] = side1Dimension.Base;
                    dr["SIDE1_DIMENSION_NUM"] = side1Dimension.Numerator;
                    dr["SIDE1_DIMENSION_DEN"] = side1Dimension.Denominator;
                    dr["SIDE1_DIMENSION_DESC"] = side1Dimension.GetDescription();

                    var side2Dimension = new Dimension
                    {
                        DecimalValue = line.F580021.S7Z5812DEC / 1000,
                        MMValue = line.F580021.S7Z5812MIM / 10,
                        Base = (int)line.F580021.S7Z58L12DM,
                        Numerator = (int)line.F580021.S7Z58L12NM,
                        Denominator = (int)line.F580021.S7Z58L12DN
                    };
                    dr["SIDE2_DIMENSION"] = side2Dimension.DecimalValue;
                    dr["SIDE2_DIMENSION_MM"] = side2Dimension.MMValue;
                    dr["SIDE2_DIMENSION_INT"] = side2Dimension.Base;
                    dr["SIDE2_DIMENSION_NUM"] = side2Dimension.Numerator;
                    dr["SIDE2_DIMENSION_DEN"] = side2Dimension.Denominator;
                    dr["SIDE2_DIMENSION_DESC"] = side2Dimension.GetDescription();

                    // LITE 1 GLASS TYPE
                    dr["LITE1_THICKNESS_NO"] = line.F580021.S7Z58L1TK / 10;
                    dr["LITE1_HEAT_TREATMENT_CD"] = line.F580021.S7Z58L1HT;
                    dr["LITE1_COLOR_CD"] = line.F580021.S7Z58L1CL;
                    dr["LITE1_SURFACE1_COAT_CD"] = line.F580021.S7Z58L11C;
                    dr["LITE1_SURFACE2_COAT_CD"] = line.F580021.S7Z58L12C;
                    dr["LITE1_PATTERN_TEXTURE_CD"] = line.F580021.S7Z58L1PX;
                    dr["LITE1_PATTERN_SURFACE_CD"] = line.F580021.S7Z58L1PTS;
                    dr["LITE1_LAMI_TYPE_CD"] = line.F580021.S7Z58L1LPT;
                    dr["LITE1_LAMI_COLOR_SURFACE_CD"] = line.F580024.S8Z5801890;
                    dr["LITE1_LAMI_PATTERN_SURFACE_CD"] = line.F580024.S8Z58L1LPS;
                    dr["LITE1_LAMI_COATING_SURFACE_CD"] = line.F580024.S8Z58L1LXS;
                    dr["LITE1_QUALITY_CD"] = line.F580021.S7Z58L1PQL;
                    dr["LITE1_IS_CUSTOMER_SUPPLIED"] = line.F580024.S8Z5840450 != null && line.F580024.S8Z5840450.Contains("1");
                    dr["LITE1_CUSTOMER_LITE_NUM"] = line.F580021.S7Z58L1CLN;

                    // LITE 1 PROCESSING
                    dr["LITE1_ROUTING_CD"] = line.F580021.S7Z58L1RG;
                    dr["LITE1_BEVEL_CD"] = line.F580021.S7Z58L1BL;
                    dr["LITE1_EDGEWORK_CD"] = line.F580021.S7Z58L1EW;
                    dr["LITE1_VGROOVE_CD"] = line.F580021.S7Z58L1VG;
                    dr["LITE1_VGROOVE_SURFACE_CD"] = line.F580021.S7Z58L1VGS;
                    dr["LITE1_VGROOVE_STYLE_CD"] = line.F580021.S7Z58L1VGT;
                    dr["LITE1_VGROOVE_HIGH"] = line.F580021.S7Z58L1VGH;
                    dr["LITE1_VGROOVE_WIDE"] = line.F580021.S7Z58L1VGW;
                    dr["LITE1_VGROOVE_VERT_OFFSET"] = line.F580024.S8Z5801295;
                    dr["LITE1_VGROOVE_HORZ_OFFSET"] = line.F580024.S8Z5801300;
                    dr["LITE1_VGROOVE_VERT_START"] = line.F580024.S8Z5801296;
                    dr["LITE1_VGROOVE_HORZ_START"] = line.F580024.S8Z5801301;
                    dr["LITE1_HOLES_DRAWING"] = line.F580021.S7Z58L1HL;
                    dr["LITE1_HOLES_NO"] = line.F580021.S7Z581365;
                    dr["LITE1_NOTCH_LOCATION_CD"] = line.F580024.S8Z5801480;
                    dr["LITE1_NOTCH_HEIGHT"] = line.F580024.S8Z5801481;
                    dr["LITE1_NOTCH_WIDTH"] = line.F580024.S8Z5801482;
                    dr["SILK_SURFACE"] = line.ItemAttributes.GetDecimalAttributeValue("Z58L1SF");
                    dr["SILK_DRAWING"] = line.ItemAttributes.GetStringAttributeValue("Z58L1SD", String.Empty);
                    dr["FABRICATION_DRAWING"] = line.F580024.S8Z5888100;
                    dr["FABRICATION_MAKEUP"] = line.F580024.S8Z58FABDE;
                    dr["EDGE_WORK_SIDES"] = line.F580024.S8Z5853002;
                    dr["PL_SIDE"] = line.F580024.S8Z5853001;
                    dr["QC_INSPECT_FLAG"] = line.F580024.S8Z5801840 ?? "";
                    dr["EDGE_DELETE_WIDTH"] = line.F580024.S8Z5805680;

                    dr["LITE1_ANSI_LOGO_CD"] = line.F580021.S7Z58L1ALT;
                    dr["LITE1_TEMP_LOGO_CD"] = line.F580021.S7Z58L1TL;
                    dr["LITE1_LOGO_OFFSET_X"] = line.F580021.S7Z58L1LX;
                    dr["LITE1_LOGO_OFFSET_Y"] = line.F580021.S7Z58L1LY;
                    dr["IG_LOGO"] = line.F580024.S8Z5840430;
                    dr["NFRC_LOGO_1"] = line.F580024.S8Z58NFRC;
                    dr["NFRC_LOGO_2"] = line.F580024.S8Z58NFRC2;
                    dr["NFRC_LOGO_HORZ_OFFSET"] = line.F580024.S8Z58NFRCH / 1000;
                    dr["NFRC_LOGO_VERT_OFFSET"] = line.F580024.S8Z58NFRCV / 1000;

                    // SURFACE
                    dr["EXT_PRESERVE_COLOR"] = line.F580024.S8Z587566;
                    dr["EXT_PRESERVE_CUTBACK"] = line.F580024.S8Z587563;
                    dr["EXT_SCORING_WIDTH"] = line.F580024.S8Z587569;
                    dr["INT_PRESERVE_COLOR"] = line.F580024.S8Z587578;
                    dr["INT_PRESERVE_CUTBACK"] = line.F580024.S8Z587575;
                    dr["INT_SCORING_WIDTH"] = line.F580024.S8Z587581;
                    dr["OPACIFIER"] = line.F580024.S8Z587584;
                    dr["OVERLAY_BAR"] = line.F580024.S8Z5805735;
                    dr["PRESERVE_PROCESSING_CD"] = line.ItemAttributes.GetStringAttributeValue("Z5800520", String.Empty);

                    //GRID
                    dr["GRID_STYLE_CD"] = line.F580024.S8Z58GSS1;
                    dr["SUB_GRID_STYLE_CD"] = line.F580024.S8Z5805570;
                    dr["HORZ_BARS_QTY"] = line.F580024.S8Z58GHA1;
                    dr["VERT_BARS_QTY"] = line.F580024.S8Z58GVA1;
                    dr["HIGH_OPEN_QTY"] = line.F580024.S8Z58OHA1;
                    dr["WIDE_OPEN_QTY"] = line.F580024.S8Z58OWA1;
                    dr["ODD_OPEN_QTY"] = line.F580024.S8Z58OOA1;
                    dr["RECT_OPEN_QTY"] = line.ItemAttributes.GetDecimalAttributeValue("Z5805380");
                    dr["DRAWING_CD"] = line.F580021.S7Z58DRAW;
                    dr["HORZ_SASH_INTRUSION"] = line.F580024.S8Z58IGSIH;
                    dr["VERT_SASH_INTRUSION"] = line.F580024.S8Z58IGSIV;
                    dr["HORZ_PRAIRIE_OFFSET"] = line.F580024.S8Z5805560;
                    dr["VERT_PRAIRIE_OFFSET"] = line.F580024.S8Z585555;
                    dr["PLACEMENT_TYPE_CD"] = line.F580024.S8Z58IPC;
                    dr["HORZ_POS_01"] = line.F580024.S8Z58IGHP1;
                    dr["HORZ_POS_02"] = line.F580024.S8Z58IGHP2;
                    dr["HORZ_POS_03"] = line.F580024.S8Z58IGHP3;
                    dr["HORZ_POS_04"] = line.F580024.S8Z58IGHP4;
                    dr["HORZ_POS_05"] = line.F580024.S8Z58IGHP5;
                    dr["HORZ_POS_06"] = line.F580024.S8Z58IGHP6;
                    dr["HORZ_POS_07"] = line.F580024.S8Z58IGHP7;
                    dr["HORZ_POS_08"] = line.F580024.S8Z58IGHP8;
                    dr["HORZ_POS_09"] = line.F580024.S8Z58IGHP9;
                    dr["HORZ_POS_10"] = line.F580024.S8Z58IGH10;
                    dr["HORZ_POS_11"] = line.F580024.S8Z58IGH11;
                    dr["HORZ_POS_12"] = line.F580024.S8Z58IGH12;
                    dr["HORZ_POS_13"] = line.F580024.S8Z58IGH13;
                    dr["HORZ_POS_14"] = line.F580024.S8Z58IGH14;
                    dr["HORZ_POS_15"] = line.F580024.S8Z58IGH15;
                    dr["HORZ_POS_16"] = line.F580024.S8Z58IGH16;
                    dr["HORZ_POS_17"] = line.F580024.S8Z58IGH17;
                    dr["HORZ_POS_18"] = line.F580024.S8Z58IGH18;
                    dr["HORZ_POS_19"] = line.F580024.S8Z58IGH19;
                    dr["HORZ_POS_20"] = line.F580024.S8Z58IGH20;
                    dr["VERT_POS_01"] = line.F580024.S8Z58IGVP1;
                    dr["VERT_POS_02"] = line.F580024.S8Z58IGVP2;
                    dr["VERT_POS_03"] = line.F580024.S8Z58IGVP3;
                    dr["VERT_POS_04"] = line.F580024.S8Z58IGVP4;
                    dr["VERT_POS_05"] = line.F580024.S8Z58IGVP5;
                    dr["VERT_POS_06"] = line.F580024.S8Z58IGVP6;
                    dr["VERT_POS_07"] = line.F580024.S8Z58IGVP7;
                    dr["VERT_POS_08"] = line.F580024.S8Z58IGVP8;
                    dr["VERT_POS_09"] = line.F580024.S8Z58IGVP9;
                    dr["VERT_POS_10"] = line.F580024.S8Z58IGV10;
                    dr["VERT_POS_11"] = line.F580024.S8Z58IGV11;
                    dr["VERT_POS_12"] = line.F580024.S8Z58IGV12;
                    dr["VERT_POS_13"] = line.F580024.S8Z58IGV13;
                    dr["VERT_POS_14"] = line.F580024.S8Z58IGV14;
                    dr["VERT_POS_15"] = line.F580024.S8Z58IGV15;
                    dr["VERT_POS_16"] = line.F580024.S8Z58IGV16;
                    dr["VERT_POS_17"] = line.F580024.S8Z58IGV17;
                    dr["VERT_POS_18"] = line.F580024.S8Z58IGV18;
                    dr["VERT_POS_19"] = line.F580024.S8Z58IGV19;
                    dr["VERT_POS_20"] = line.F580024.S8Z58IGV20;

                    // RACK
                    dr["LITES_PER_RACK"] = line.F580021.S7Z58LPR;
                    dr["RACK_ORIENT_ROWS"] = line.F580024.S8Z58ROW;
                    dr["RACK_ORIENT_VH"] = line.F580024.S8Z58RPRVH;
                    dr["RACK_ORIENT_IO"] = line.ItemAttributes.GetDecimalAttributeValue("Z58RTNIO");
                    dr["RACK_ORIENT_TIERS"] = line.F580024.S8Z58STK;
                    dr["RACK_TYPE"] = line.ItemAttributes.GetStringAttributeValue("Z58RT");
                    dr["CRATE_CD"] = line.ItemAttributes.GetStringAttributeValue("Z58CRATE", String.Empty);

                    // META
                    dr["UPDATE_DATE"] = line.F580021.CMDateTime;

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
