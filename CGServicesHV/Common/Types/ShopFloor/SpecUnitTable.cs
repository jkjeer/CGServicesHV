using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.Enterprise;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("ConfigurationId")).Max(); }
            set { }
        }

        public SpecUnitTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitLedger]");
            Columns.Add("ConfigurationId", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("StockType", typeof(string));
            Columns.Add("UnitType", typeof(string));

            Columns.Add("Lite1_Thick", typeof(decimal));
            Columns.Add("Lite1_Color", typeof(string));
            Columns.Add("Lite1_Heat", typeof(string));
            Columns.Add("Lite1_1stSrfCoat", typeof(string));
            Columns.Add("Lite1_2ndSrfCoat", typeof(string));
            Columns.Add("Lite1_Pattern", typeof(string));
            Columns.Add("Lite1_Lami", typeof(string));
            Columns.Add("Lite1_Routing", typeof(string));

            Columns.Add("ShapeNum", typeof(decimal));
            Columns.Add("BaseDim", typeof(decimal));
            Columns.Add("LeftDim", typeof(decimal));
            Columns.Add("RightDim", typeof(decimal));
            Columns.Add("TopDim", typeof(decimal));
            Columns.Add("Side1Dim", typeof(decimal));
            Columns.Add("Side2Dim", typeof(decimal));

            Columns.Add("Dim_Base_Frac", typeof(string));
            Columns.Add("Dim_Left_Frac", typeof(string));
            Columns.Add("Dim_Right_Frac", typeof(string));
            Columns.Add("Dim_Top_Frac", typeof(string));
            Columns.Add("Dim_Side1_Frac", typeof(string));
            Columns.Add("Dim_Side2_Frac", typeof(string));

            Columns.Add("TotalWeight", typeof(decimal));
            Columns.Add("UnitWeight", typeof(decimal));
            Columns.Add("GlassWeightLbs", typeof(decimal));
            Columns.Add("ActualSqft", typeof(decimal));
            Columns.Add("BillableSqft", typeof(decimal));

            Columns.Add("LitesPerRack", typeof(decimal));
            Columns.Add("RackOrientRows", typeof(decimal));
            Columns.Add("RackOrientVH", typeof(string));
            Columns.Add("RackOrientIO", typeof(string));
            Columns.Add("RackOrientTiers", typeof(decimal));
            Columns.Add("RackType", typeof(string));

            Columns.Add("BaseL", typeof(decimal));
            Columns.Add("BaseN", typeof(decimal));
            Columns.Add("BaseD", typeof(decimal));
            Columns.Add("LeftL", typeof(decimal));
            Columns.Add("LeftN", typeof(decimal));
            Columns.Add("LeftD", typeof(decimal));
            Columns.Add("RightL", typeof(decimal));
            Columns.Add("RightN", typeof(decimal));
            Columns.Add("RightD", typeof(decimal));
            Columns.Add("TopL", typeof(decimal));
            Columns.Add("TopN", typeof(decimal));
            Columns.Add("TopD", typeof(decimal));

            Columns.Add("AnsiiLogoType", typeof(string));
            Columns.Add("Lite1_ProdQualityLevel", typeof(string));

            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("IADate", typeof(DateTime));
            //Columns.Add("IBDate", typeof(DateTime));
            Columns.Add("CMDate", typeof(DateTime));
        }

        public void SetTransactionDate(Plant plant)
        {
            try
            {
                foreach (DataRow dr in Rows)
                {
                    //var iaDate = (dr["IADate"].ToString() == "") ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dr["IADate"].ToString());
                    //var ibDate = (dr["IBDate"].ToString() == "") ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dr["IBDate"].ToString());
                    var cmDate = (dr["CMDate"].ToString() == "") ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dr["CMDate"].ToString());
                    //var tDate = new[] { iaDate, ibDate, cmDate }.Max();
                    //Logger.Debug($"ItemNum: [{dr["ItemNum"]}]");
                    //Logger.Debug($"iaDate: {iaDate}");
                    //Logger.Debug($"ibDate: {ibDate}");
                    //Logger.Debug($"tDate: {tDate}");
                    dr["TransactionDate"] = cmDate;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }

        }

        public void Add(SpecUnitItems specUnitItems, Plant plant)
        {
            try
            {
                foreach (var specUnit in specUnitItems)
                {
                    Logger.Info($"Plant: {plant.PlantCode}-{plant.PlantNum} ConfigId: {specUnit.ConfigId} Item: {specUnit.ItemNum}");
                    DataRow dr = NewRow();
                    dr["ConfigurationId"] = specUnit.ConfigId;
                    dr["ItemNum"] = specUnit.ItemNum;
                    dr["StockType"] = specUnit.StockType ?? "";
                    dr["UnitType"] = specUnit.UnitType ?? "";

                    dr["Lite1_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TK");
                    dr["Lite1_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L1CL", "CL");
                    dr["Lite1_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1HT");
                    dr["Lite1_1stSrfCoat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L11C");
                    dr["Lite1_2ndSrfCoat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L12C");
                    dr["Lite1_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1PX");
                    dr["Lite1_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1LPT");
                    dr["Lite1_Routing"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1RTG");

                    dr["ShapeNum"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58SHAPE");
                    decimal baseDim = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581BDEC");
                    dr["BaseDim"] = baseDim;
                    decimal leftDim = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581LDEC");
                    dr["LeftDim"] = leftDim;
                    decimal rightDim = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581RDEC");
                    dr["RightDim"] = rightDim;
                    decimal topDim = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581TDEC");
                    dr["TopDim"] = topDim;
                    decimal side1Dim = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5811DEC");
                    dr["Side1Dim"] = side1Dim;
                    decimal side2Dim = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5812DEC");
                    dr["Side2Dim"] = side2Dim;

                    dr["Dim_Base_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDN"));
                    dr["Dim_Left_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDN"));
                    dr["Dim_Right_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDN"));
                    dr["Dim_Top_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDN"));
                    dr["Dim_Side1_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11DM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11NM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11DN"));
                    dr["Dim_Side2_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12DM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12NM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12DN"));

                    decimal actualSqft = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58USF");
                    dr["ActualSqft"] = actualSqft;
                    decimal billableSqft = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58BSF");
                    dr["BillableSqft"] = billableSqft;

                    decimal totalWeight = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT");
                    dr["TotalWeight"] = totalWeight;
                    decimal glassWeightLbs;
                    try
                    {
                        glassWeightLbs = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT") / specUnit.ItemAttributes.GetDecimalAttributeValue("Z58RKS");
                    }
                    catch (Exception)
                    {
                        glassWeightLbs = 0;
                    }
                    dr["GlassWeightLbs"] = glassWeightLbs; 
                    decimal unitWeightLbs;
                    try
                    {
                        unitWeightLbs = glassWeightLbs / specUnit.ItemAttributes.GetDecimalAttributeValue("Z58LPR");
                    }
                    catch (Exception)
                    {
                        unitWeightLbs = 0;
                    }
                    dr["UnitWeight"] = unitWeightLbs;

                    decimal baseL = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDM");
                    dr["BaseL"] = baseL;
                    decimal baseN = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BNM");
                    dr["BaseN"] = baseN;
                    decimal baseD = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDN");
                    dr["BaseD"] = baseD;
                    decimal leftL = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDM");
                    dr["LeftL"] = leftL;
                    decimal leftN = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LNM");
                    dr["LeftN"] = leftN;
                    decimal leftD = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDN");
                    dr["LeftD"] = leftD;
                    decimal rightL = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDM");
                    dr["RightL"] = rightL;
                    decimal rightN = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RNM");
                    dr["RightN"] = rightN;
                    decimal rightD = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDN");
                    dr["RightD"] = rightD;
                    decimal topL = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDM");
                    dr["TopL"] = topL;
                    decimal topN = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TNM");
                    dr["TopN"] = topN;
                    decimal topD = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDN");
                    dr["TopD"] = topD;

                    dr["LitesPerRack"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58LPR");
                    dr["RackOrientVH"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RPRVH");
                    dr["RackOrientIO"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RTNIO");
                    dr["RackOrientRows"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58ROW");
                    dr["RackOrientTiers"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58STK");
                    dr["RackType"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RT");

                    dr["AnsiiLogoType"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1ALT");
                    dr["Lite1_ProdQualityLevel"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1PQL");

                    dr["CMDate"] = specUnit.CMDate;
                    dr["TransactionDate"] = specUnit.TransactionDate;

                    var isOverflow = false;
                    if (baseDim > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} BaseDim out of range: {baseDim}");
                    }
                    if (leftDim > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} LeftDim out of range: {leftDim}");
                    }
                    if (rightDim > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} RightDim out of range: {rightDim}");
                    }
                    if (topDim > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} TopDim out of range: {topDim}");
                    }
                    if (side1Dim > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} Side1Dim out of range: {side1Dim}");
                    }
                    if (side2Dim > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} Side2Dim out of range: {side2Dim}");
                    }

                    if (baseL > 100000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} BaseL out of range: {baseL}");
                    }
                    if (baseN > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} BaseN out of range: {baseN}");
                    }
                    if (baseD > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} BaseD out of range: {baseD}");
                    }
                    if (leftL > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} LeftL out of range: {leftL}");
                    }
                    if (leftN > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} LeftN out of range: {leftN}");
                    }
                    if (leftD > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} LeftD out of range: {leftD}");
                    }
                    if (rightL > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} RightL out of range: {rightL}");
                    }
                    if (rightN > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} RightN out of range: {rightN}");
                    }
                    if (rightD > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} RightD out of range: {rightD}");
                    }
                    if (topL > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} TopL out of range: {topL}");
                    }
                    if (topN > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} TopN out of range: {topN}");
                    }
                    if (topD > 10000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} TopD out of range: {topD}");
                    }

                    if (actualSqft > 100000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} ActualSqft out of range: {actualSqft}");
                    }
                    if (billableSqft > 100000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} BillableSqft out of range: {billableSqft}");
                    }
                    if (glassWeightLbs > 1000000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} GlassWeightLbs out of range: {glassWeightLbs}");
                    }
                    if (totalWeight > 1000000)
                    {
                        isOverflow = true;
                        Logger.Error($"ERROR: Plant: {plant.PlantCode}-{plant.PlantNum} Item: {specUnit.ItemNum} ConfigId: {specUnit.ConfigId} TotalWeight out of range: {totalWeight}");
                    }
                    if (!isOverflow)
                    {
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        public void Add(SpecUnits specUnits, Plant plant)
        {
            try
            {
                foreach (SpecUnit specUnit in specUnits)
                {
                    DataRow dr = NewRow();
                    dr["ConfigurationId"] = specUnit.Id;
                    dr["ItemNum"] = specUnit.ItemNum;
                    dr["StockType"] = specUnit.StockType ?? "";
                    dr["UnitType"] = specUnit.UnitType ?? "";

                    dr["Lite1_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TK");
                    dr["Lite1_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L1CL", "CL");
                    dr["Lite1_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1HT");
                    dr["Lite1_1stSrfCoat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L11C");
                    dr["Lite1_2ndSrfCoat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L12C");
                    dr["Lite1_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1PX");
                    dr["Lite1_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1LPT");
                    dr["Lite1_Routing"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1RTG");

                    dr["ShapeNum"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58SHAPE");
                    dr["BaseDim"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581BDEC");
                    dr["LeftDim"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581LDEC");
                    dr["RightDim"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581RDEC");
                    dr["TopDim"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581TDEC");
                    dr["Side1Dim"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5811DEC");
                    dr["Side2Dim"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5812DEC");

                    dr["Dim_Base_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDN"));
                    dr["Dim_Left_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDN"));
                    dr["Dim_Right_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDN"));
                    dr["Dim_Top_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDN"));
                    dr["Dim_Side1_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11DM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11NM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11DN"));
                    dr["Dim_Side2_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12DM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12NM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12DN"));

                    dr["ActualSqft"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58USF");
                    dr["BillableSqft"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58BSF");
                    decimal glassWeightLbs;
                    try
                    {
                        glassWeightLbs = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT") / specUnit.ItemAttributes.GetDecimalAttributeValue("Z58LPR");
                    }
                    catch (Exception)
                    {
                        glassWeightLbs = 0;
                    }
                    dr["GlassWeightLbs"] = glassWeightLbs;
                    dr["TotalWeight"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT");
                    dr["UnitWeight"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT");

                    dr["BaseL"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDM");
                    dr["BaseN"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BNM");
                    dr["BaseD"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDN");
                    dr["LeftL"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDM");
                    dr["LeftN"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LNM");
                    dr["LeftD"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDN");
                    dr["RightL"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDM");
                    dr["RightN"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RNM");
                    dr["RightD"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDN");
                    dr["TopL"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDM");
                    dr["TopN"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TNM");
                    dr["TopD"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDN");

                    dr["LitesPerRack"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58LPR");
                    dr["RackOrientVH"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RPRVH");
                    dr["RackOrientIO"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RTNIO");
                    dr["RackOrientRows"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58ROW");
                    dr["RackOrientTiers"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58STK");
                    dr["RackType"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RT");

                    dr["IADate"] = specUnit.IADate;
                    dr["TransactionDate"] = specUnit.TransactionDate;

                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        private string GetDimensionDescription(decimal baseDim, decimal numDim, decimal denDim)
        {
            string result = "";
            if (baseDim > 0 || numDim > 0)
            {
                var sb = new StringBuilder();
                sb.Append(baseDim.ToString(CultureInfo.CurrentCulture));
                if (numDim > 0)
                {
                    sb.Append(" ");
                    sb.Append(numDim.ToString(CultureInfo.CurrentCulture));
                    sb.Append("/");
                    sb.Append(denDim.ToString(CultureInfo.CurrentCulture));
                }
                result = sb.ToString();
            }
            return result;
        }

    }
}
