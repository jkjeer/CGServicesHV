using System;
using System.Data;
using System.ServiceModel;
using System.Text;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Utilities;

namespace CGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitShapeTable : DataTable
    {

        public SpecUnitShapeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_SHAPE_LEDGER]");
            Columns.Add("SPEC_UNIT_ID", typeof(long));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
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
            Columns.Add("GLASS_WEIGHT_LBS", typeof(decimal));
        }

        public void Add(SpecUnits specUnits)
        {
            try
            {
                foreach (var specUnit in specUnits)
                {

                    DataRow dr = NewRow();
                    dr["ITEM_NUM"] = specUnit.ItemNum;
                    dr["CONFIGURATION_ID"] = specUnit.ConfigurationId;

                    var atts = specUnit.ItemAttributes;

                    dr["SHAPE_NO"] = atts.GetDecimalAttributeValue("Z58SHAPE");
                    dr["ACTUAL_SQFT"] = atts.GetDecimalAttributeValue("Z58USF");
                    dr["BILLABLE_SQFT"] = atts.GetDecimalAttributeValue("Z58BSF");
                    dr["PERIMETER_BLOCK_SIZE"] = atts.GetDecimalAttributeValue("Z5850970");
                    dr["GLASS_WEIGHT_LBS"] = specUnit.GlassWeightLbs;

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

                    Rows.Add(dr);
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
                DataRow dr = NewRow();
                var gs = item.GlassItem.Shape;
                dr["ITEM_NUM"] = item.ItemNum;
                dr["CONFIGURATION_ID"] = item.ConfigurationId;
                dr["SHAPE_NO"] = (int)gs.ShapeNum;

                var leg = new Dimension();

                leg = gs.BaseDimension;
                dr["BASE_DIMENSION"] = leg.DecimalValue;
                dr["BASE_DIMENSION_MM"] = leg.MMValue;
                dr["BASE_DIMENSION_INT"] = (int)leg.Base;
                dr["BASE_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["BASE_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["BASE_DIMENSION_DESC"] = leg.Description;

                leg = gs.LeftDimension;
                dr["LEFT_DIMENSION"] = leg.DecimalValue;
                dr["LEFT_DIMENSION_MM"] = leg.MMValue;
                dr["LEFT_DIMENSION_INT"] = (int)leg.Base;
                dr["LEFT_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["LEFT_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["LEFT_DIMENSION_DESC"] = leg.Description;

                leg = gs.RightDimension;
                dr["RIGHT_DIMENSION"] = leg.DecimalValue;
                dr["RIGHT_DIMENSION_MM"] = leg.MMValue;
                dr["RIGHT_DIMENSION_INT"] = (int)leg.Base;
                dr["RIGHT_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["RIGHT_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["RIGHT_DIMENSION_DESC"] = leg.Description;

                leg = gs.TopDimension;
                dr["TOP_DIMENSION"] = leg.DecimalValue;
                dr["TOP_DIMENSION_MM"] = leg.MMValue;
                dr["TOP_DIMENSION_INT"] = (int)leg.Base;
                dr["TOP_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["TOP_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["TOP_DIMENSION_DESC"] = leg.Description;

                leg = gs.Side1Dimension;
                dr["SIDE1_DIMENSION"] = leg.DecimalValue;
                dr["SIDE1_DIMENSION_MM"] = leg.MMValue;
                dr["SIDE1_DIMENSION_INT"] = (int)leg.Base;
                dr["SIDE1_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["SIDE1_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["SIDE1_DIMENSION_DESC"] = leg.Description;

                leg = gs.Side2Dimension;
                dr["SIDE2_DIMENSION"] = leg.DecimalValue;
                dr["SIDE2_DIMENSION_MM"] = leg.MMValue;
                dr["SIDE2_DIMENSION_INT"] = (int)leg.Base;
                dr["SIDE2_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["SIDE2_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["SIDE2_DIMENSION_DESC"] = leg.Description;

                dr["ACTUAL_SQFT"] = gs.ActualSqft;
                dr["BILLABLE_SQFT"] = gs.BillableSqft;
                dr["PERIMETER_BLOCK_SIZE"] = gs.BlockSizePerimeter;
                dr["GLASS_WEIGHT_LBS"] = item.GlassItem.TotalWeight;

                Rows.Add(dr);
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

                    DataRow dr = NewRow();

                    dr["SPEC_UNIT_ID"] = line.SpecUnitId;

                    dr["ITEM_NUM"] = line.F580021.S7ITM;
                    dr["CONFIGURATION_ID"] = (int)line.F580021.CMCFGID;
                    dr["SHAPE_NO"] = Data.ParseStringToDecimal(line.F580021.S7Z58SHAPE);

                    dr["BASE_DIMENSION"] = line.F580021.S7Z581BDEC/1000;
                    dr["BASE_DIMENSION_MM"] = line.F580021.S7Z581BMIM/10;
                    dr["BASE_DIMENSION_INT"] = (int)line.F580021.S7Z58L1BDM;
                    dr["BASE_DIMENSION_NUM"] = (int)line.F580021.S7Z58L1BNM;
                    dr["BASE_DIMENSION_DEN"] = (int)line.F580021.S7Z58L1BDN;
                    dr["BASE_DIMENSION_DESC"] = GetDescription(line.F580021.S7Z581BDEC/1000, (int) line.F580021.S7Z58L1BDM,
                        (int) line.F580021.S7Z58L1BNM, (int) line.F580021.S7Z58L1BDN);

                    dr["LEFT_DIMENSION"] = line.F580021.S7Z581LDEC/1000;
                    dr["LEFT_DIMENSION_MM"] = line.F580021.S7Z581LMIM/10;
                    dr["LEFT_DIMENSION_INT"] = (int)line.F580021.S7Z58L1LDM;
                    dr["LEFT_DIMENSION_NUM"] = (int)line.F580021.S7Z58L1LNM;
                    dr["LEFT_DIMENSION_DEN"] = (int)line.F580021.S7Z58L1LDN;
                    dr["LEFT_DIMENSION_DESC"] = GetDescription(line.F580021.S7Z581LDEC/1000, (int)line.F580021.S7Z58L1LDM,
                        (int)line.F580021.S7Z58L1LNM, (int)line.F580021.S7Z58L1LDN);

                    dr["RIGHT_DIMENSION"] = line.F580021.S7Z581RDEC/1000;
                    dr["RIGHT_DIMENSION_MM"] = line.F580021.S7Z581RMIM/10;
                    dr["RIGHT_DIMENSION_INT"] = (int)line.F580021.S7Z58L1RDM;
                    dr["RIGHT_DIMENSION_NUM"] = (int)line.F580021.S7Z58L1RNM;
                    dr["RIGHT_DIMENSION_DEN"] = (int)line.F580021.S7Z58L1RDN;
                    dr["RIGHT_DIMENSION_DESC"] = GetDescription(line.F580021.S7Z581RDEC/1000, (int)line.F580021.S7Z58L1RDM,
                        (int)line.F580021.S7Z58L1RNM, (int)line.F580021.S7Z58L1RDN);

                    dr["TOP_DIMENSION"] = line.F580021.S7Z581TDEC/1000;
                    dr["TOP_DIMENSION_MM"] = line.F580021.S7Z581TMIM/10;
                    dr["TOP_DIMENSION_INT"] = (int)line.F580021.S7Z58L1TDM;
                    dr["TOP_DIMENSION_NUM"] = (int)line.F580021.S7Z58L1TNM;
                    dr["TOP_DIMENSION_DEN"] = (int)line.F580021.S7Z58L1TDN;
                    dr["TOP_DIMENSION_DESC"] = GetDescription(line.F580021.S7Z581TDEC/1000, (int)line.F580021.S7Z58L1TDM,
                        (int)line.F580021.S7Z58L1TNM, (int)line.F580021.S7Z58L1TDN);

                    dr["SIDE1_DIMENSION"] = line.F580021.S7Z5811DEC/1000;
                    dr["SIDE1_DIMENSION_MM"] = line.F580021.S7Z5811MIM/10;
                    dr["SIDE1_DIMENSION_INT"] = (int)line.F580021.S7Z58L11DM;
                    dr["SIDE1_DIMENSION_NUM"] = (int)line.F580021.S7Z58L11NM;
                    dr["SIDE1_DIMENSION_DEN"] = (int)line.F580021.S7Z58L11DN;
                    dr["SIDE1_DIMENSION_DESC"] = GetDescription(line.F580021.S7Z5811DEC/1000, (int)line.F580021.S7Z58L11DM,
                        (int)line.F580021.S7Z58L11NM, (int)line.F580021.S7Z58L11DN);

                    dr["SIDE2_DIMENSION"] = line.F580021.S7Z5812DEC / 1000;
                    dr["SIDE2_DIMENSION_MM"] = line.F580021.S7Z5812MIM / 10;
                    dr["SIDE2_DIMENSION_INT"] = (int)line.F580021.S7Z58L12DM;
                    dr["SIDE2_DIMENSION_NUM"] = (int)line.F580021.S7Z58L12NM;
                    dr["SIDE2_DIMENSION_DEN"] = (int)line.F580021.S7Z58L12DN;
                    dr["SIDE2_DIMENSION_DESC"] = GetDescription(line.F580021.S7Z5812DEC / 1000, (int)line.F580021.S7Z58L12DM,
                        (int)line.F580021.S7Z58L12NM, (int)line.F580021.S7Z58L12DN);

                    dr["ACTUAL_SQFT"] = line.F580024.S8Z58USF / 10000;
                    dr["BILLABLE_SQFT"] = line.F580024.S8Z58BSF / 10000;
                    //dr["PERIMETER_BLOCK_SIZE"] = line.F580021.S7Z5850970;
                    //dr["GLASS_WEIGHT_LBS"] = item.GlassItem.TotalWeight;

                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        public string GetDescription(decimal val, int bas, int num, int den)
        {
            string result = null;
            if (val > 0)
            {
                var sb = new StringBuilder();
                sb.Append(bas.ToString());
                if (num > 0)
                {
                    sb.Append(" ");
                    sb.Append(num.ToString());
                    sb.Append("/");
                    sb.Append(den.ToString());
                }
                result = sb.ToString();
            }
            return result;
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitShapeEbsTable : DataTable
    {

        public SpecUnitShapeEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitShape]");
            Columns.Add("SpecUnitId", typeof(int));
            Columns.Add("ShapeNum", typeof(int));
            Columns.Add("DimBase", typeof(decimal));
            Columns.Add("DimBaseMM", typeof(decimal));
            Columns.Add("DimBaseIn", typeof(int));
            Columns.Add("DimBaseNum", typeof(int));
            Columns.Add("DimBaseDen", typeof(int));
            Columns.Add("DimBaseDesc", typeof(string));
            Columns.Add("DimLeft", typeof(decimal));
            Columns.Add("DimLeftMM", typeof(decimal));
            Columns.Add("DimLeftIn", typeof(int));
            Columns.Add("DimLeftNum", typeof(int));
            Columns.Add("DimLeftDen", typeof(int));
            Columns.Add("DimLeftDesc", typeof(string));
            Columns.Add("DimRight", typeof(decimal));
            Columns.Add("DimRightMM", typeof(decimal));
            Columns.Add("DimRightIn", typeof(int));
            Columns.Add("DimRightNum", typeof(int));
            Columns.Add("DimRightDen", typeof(int));
            Columns.Add("DimRightDesc", typeof(string));
            Columns.Add("DimTop", typeof(decimal));
            Columns.Add("DimTopMM", typeof(decimal));
            Columns.Add("DimTopIn", typeof(int));
            Columns.Add("DimTopNum", typeof(int));
            Columns.Add("DimTopDen", typeof(int));
            Columns.Add("DimTopDesc", typeof(string));
            Columns.Add("DimS1", typeof(decimal));
            Columns.Add("DimS1MM", typeof(decimal));
            Columns.Add("DimS1In", typeof(int));
            Columns.Add("DimS1Num", typeof(int));
            Columns.Add("DimS1Den", typeof(int));
            Columns.Add("DimS1Desc", typeof(string));
            Columns.Add("DimS2", typeof(decimal));
            Columns.Add("DimS2MM", typeof(decimal));
            Columns.Add("DimS2In", typeof(int));
            Columns.Add("DimS2Num", typeof(int));
            Columns.Add("DimS2Den", typeof(int));
            Columns.Add("DimS2Desc", typeof(string));
            Columns.Add("ActualSqft", typeof(decimal));
            Columns.Add("BillableSqft", typeof(decimal));
            Columns.Add("UnitWeight", typeof(decimal));
            Columns.Add("BlockSizeHeight", typeof(decimal));
            Columns.Add("BlockSizeWidth", typeof(decimal));
            Columns.Add("BlockSizeShort", typeof(decimal));
            Columns.Add("BlockSizeLong", typeof(decimal));
            Columns.Add("BlockSizeSqft", typeof(decimal));
            Columns.Add("PerimeterBlockSize", typeof(decimal));
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {

                    DataRow dr = NewRow();

                    dr["SpecUnitId"] = line.SpecUnitId;

                    dr["ShapeNum"] = Data.ParseStringToDecimal(line.F580021.S7Z58SHAPE);

                    dr["DimBase"] = line.F580021.S7Z581BDEC / 1000;
                    dr["DimBaseMM"] = line.F580021.S7Z581BMIM / 10;
                    dr["DimBaseIn"] = (int)line.F580021.S7Z58L1BDM;
                    dr["DimBaseNum"] = (int)line.F580021.S7Z58L1BNM;
                    dr["DimBaseDen"] = (int)line.F580021.S7Z58L1BDN;
                    dr["DimBaseDesc"] = GetDescription(line.F580021.S7Z581BDEC / 1000, (int)line.F580021.S7Z58L1BDM,
                        (int)line.F580021.S7Z58L1BNM, (int)line.F580021.S7Z58L1BDN);

                    dr["DimLeft"] = line.F580021.S7Z581LDEC / 1000;
                    dr["DimLeftMM"] = line.F580021.S7Z581LMIM / 10;
                    dr["DimLeftIn"] = (int)line.F580021.S7Z58L1LDM;
                    dr["DimLeftNum"] = (int)line.F580021.S7Z58L1LNM;
                    dr["DimLeftDen"] = (int)line.F580021.S7Z58L1LDN;
                    dr["DimLeftDesc"] = GetDescription(line.F580021.S7Z581LDEC / 1000, (int)line.F580021.S7Z58L1LDM,
                        (int)line.F580021.S7Z58L1LNM, (int)line.F580021.S7Z58L1LDN);

                    dr["DimRight"] = line.F580021.S7Z581RDEC / 1000;
                    dr["DimRightMM"] = line.F580021.S7Z581RMIM / 10;
                    dr["DimRightIn"] = (int)line.F580021.S7Z58L1RDM;
                    dr["DimRightNum"] = (int)line.F580021.S7Z58L1RNM;
                    dr["DimRightDen"] = (int)line.F580021.S7Z58L1RDN;
                    dr["DimRightDesc"] = GetDescription(line.F580021.S7Z581RDEC / 1000, (int)line.F580021.S7Z58L1RDM,
                        (int)line.F580021.S7Z58L1RNM, (int)line.F580021.S7Z58L1RDN);

                    dr["DimTop"] = line.F580021.S7Z581TDEC / 1000;
                    dr["DimTopMM"] = line.F580021.S7Z581TMIM / 10;
                    dr["DimTopIn"] = (int)line.F580021.S7Z58L1TDM;
                    dr["DimTopNum"] = (int)line.F580021.S7Z58L1TNM;
                    dr["DimTopDen"] = (int)line.F580021.S7Z58L1TDN;
                    dr["DimTopDesc"] = GetDescription(line.F580021.S7Z581TDEC / 1000, (int)line.F580021.S7Z58L1TDM,
                        (int)line.F580021.S7Z58L1TNM, (int)line.F580021.S7Z58L1TDN);

                    dr["DimS1"] = line.F580021.S7Z5811DEC / 1000;
                    dr["DimS1MM"] = line.F580021.S7Z5811MIM / 10;
                    dr["DimS1In"] = (int)line.F580021.S7Z58L11DM;
                    dr["DimS1Num"] = (int)line.F580021.S7Z58L11NM;
                    dr["DimS1Den"] = (int)line.F580021.S7Z58L11DN;
                    dr["DimS1Desc"] = GetDescription(line.F580021.S7Z5811DEC / 1000, (int)line.F580021.S7Z58L11DM,
                        (int)line.F580021.S7Z58L11NM, (int)line.F580021.S7Z58L11DN);

                    dr["DimS2"] = line.F580021.S7Z5812DEC / 1000;
                    dr["DimS2MM"] = line.F580021.S7Z5812MIM / 10;
                    dr["DimS2In"] = (int)line.F580021.S7Z58L12DM;
                    dr["DimS2Num"] = (int)line.F580021.S7Z58L12NM;
                    dr["DimS2Den"] = (int)line.F580021.S7Z58L12DN;
                    dr["DimS2Desc"] = GetDescription(line.F580021.S7Z5812DEC / 1000, (int)line.F580021.S7Z58L12DM,
                        (int)line.F580021.S7Z58L12NM, (int)line.F580021.S7Z58L12DN);

                    dr["ActualSqft"] = line.F580024.S8Z58USF / 10000;
                    dr["BillableSqft"] = line.F580024.S8Z58BSF / 10000;
                    dr["UnitWeight"] = line.F580024.S8Z58GLWT / 1000 / line.F580021.S7Z58QTY;
                    dr["BlockSizeHeight"] = line.ItemAttributes.GetDecimalAttributeValue("Z5800536");
                    dr["BlockSizeWidth"] = line.ItemAttributes.GetDecimalAttributeValue("Z5800537");
                    dr["BlockSizeShort"] = line.ItemAttributes.GetDecimalAttributeValue("Z5800538");
                    dr["BlockSizeLong"] = line.ItemAttributes.GetDecimalAttributeValue("Z5800539");
                    dr["BlockSizeSqft"] = line.ItemAttributes.GetDecimalAttributeValue("Z5800960");
                    dr["PerimeterBlockSize"] = line.ItemAttributes.GetDecimalAttributeValue("Z5800970");

                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string GetDescription(decimal val, int bas, int num, int den)
        {
            string result = "";
            if (val > 0)
            {
                var sb = new StringBuilder();
                sb.Append(bas.ToString());
                if (num > 0)
                {
                    sb.Append(" ");
                    sb.Append(num.ToString());
                    sb.Append("/");
                    sb.Append(den.ToString());
                }
                result = sb.ToString();
            }
            return result;
        }
    }
}
