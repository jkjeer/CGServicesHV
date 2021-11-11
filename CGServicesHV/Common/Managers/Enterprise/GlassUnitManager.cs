using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using IBM.Data.DB2.iSeries;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class GlassUnitManager
    {

        public GlassUnitManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            foreach (var keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /********************************************************************************
        * Segments
        *******************************************************************************/
        public GlassItem GetGlassUnit(BusinessSystemSession session, ItemAttributes attributes)
        {
            var item = new GlassItem();
            try
            {
                item.OverallThickness = attributes.GetDecimalAttributeValue("Z5850865");
                item.LitesPerRack = attributes.GetDecimalAttributeValue("Z58LPR");
                item.UnitTreatment = attributes.GetStringAttributeValue("Z5800513");
                item.UnitTreatmentDesc = attributes.GetStringAttributeDesc1("Z5800513");
                //item.UnitType = attributes.GetStringAttributeValue("Z5840275");
                //item.UnitTypeDesc = attributes.GetStringAttributeDesc1("Z5840275");
                item.RackRows = attributes.GetDecimalAttributeValue("Z58ROW");
                item.RackTiers = attributes.GetDecimalAttributeValue("Z58STK");
                item.RackOrientationVH = attributes.GetStringAttributeValue("Z58RPRVH");
                item.RackOrientationIO = attributes.GetStringAttributeValue("Z58RTNIO");
                item.ProductType = attributes.GetStringAttributeDesc1("Z58PT");
                item.CardinalProductType = attributes.GetStringAttributeDesc1("Z5840310");
                item.CornerClipping = attributes.GetStringAttributeDesc1("Z5800550");
                item.VolumeIndicator = attributes.GetStringAttributeValue("Z5840460");
                item.BatchShiftLotNum = attributes.GetStringAttributeValue("Z5895856");
                item.VerticalSashIntrusion = attributes.GetDecimalAttributeValue("Z58IGSIV");
                item.HorizontalSashIntrusion = attributes.GetDecimalAttributeValue("Z58IGSIH");
                item.NarrowMarginCode = attributes.GetStringAttributeValue("Z58IGNMC");
                item.NarrowMarginDesc = attributes.GetStringAttributeDesc1("Z58IGNMC");
                item.EnergyPanel = attributes.GetStringAttributeValue("Z5801630");
                item.EnergyPanelDesc = attributes.GetStringAttributeDesc1("Z5801630");
                item.PreserveProcessing = attributes.GetStringAttributeDesc1("Z5800520");
                item.IGCaps = attributes.GetStringAttributeValue("Z5801635");
                item.IGCapsDesc = attributes.GetStringAttributeDesc1("Z5801635");
                item.TotalWeight = attributes.GetDecimalAttributeValue("Z58GLWT") /
                                   ((attributes.GetDecimalAttributeValue("Z5840360") == 0)
                                        ? 1
                                        : attributes.GetDecimalAttributeValue("Z5840360"));

                item.CustomerPartNum1 = attributes.GetStringAttributeValue("Z58CP2");
                item.CustomerPartNum2 = attributes.GetStringAttributeValue("Z58CP3");
                item.CustomerPartNum3 = attributes.GetStringAttributeValue("Z58CP4");
                item.EndPickItemNum = attributes.GetDecimalAttributeValue("Z5880230");
                //if (item.EndPickItemNum > 0)
                //{
                //    item.BasePickLocation = GetPickLocation(session, item.PlantNum, item.EndPickItemNum, "P");
                //}

                // Customer
                item.SoldToCustomerNum = attributes.GetDecimalAttributeValue("AN8DR");
                item.OriginalSoldToCustomerNum = attributes.GetDecimalAttributeValue("Z58CTL");
                item.ShipToCustomerNum = attributes.GetDecimalAttributeValue("SHAN");
                item.OriginalShipToCustomerNum = attributes.GetDecimalAttributeValue("Z5840425");

                // Shape
                var unitShape = new Shape();
                unitShape.ShapeNum = attributes.GetDecimalAttributeValue("Z58SHAPE");
                unitShape.ActualSqft = attributes.GetDecimalAttributeValue("Z58USF");
                unitShape.BillableSqft = attributes.GetDecimalAttributeValue("Z58BSF");
                unitShape.BlockSizePerimeter = attributes.GetDecimalAttributeValue("Z5850970");
                unitShape.BlockSizeHeight = attributes.GetDecimalAttributeValue("Z5800536");
                unitShape.BlockSizeWidth = attributes.GetDecimalAttributeValue("Z5800537");
                unitShape.BlockSizeShort = attributes.GetDecimalAttributeValue("Z5800538");
                unitShape.BlockSizeLong = attributes.GetDecimalAttributeValue("Z5800539");

                var baseDimension = new Dimension();
                baseDimension.DecimalValue = attributes.GetDecimalAttributeValue("Z581BDEC");
                baseDimension.MMValue = attributes.GetDecimalAttributeValue("Z581BMIM");
                baseDimension.Base = attributes.GetDecimalAttributeValue("Z58L1BDM");
                baseDimension.Numerator = attributes.GetDecimalAttributeValue("Z58L1BNM");
                baseDimension.Denominator = attributes.GetDecimalAttributeValue("Z58L1BDN");
                baseDimension.Description = baseDimension.GetDescription();
                unitShape.BaseDimension = baseDimension;

                var leftDimension = new Dimension();
                leftDimension.DecimalValue = attributes.GetDecimalAttributeValue("Z581LDEC");
                leftDimension.MMValue = attributes.GetDecimalAttributeValue("Z581LMIM");
                leftDimension.Base = attributes.GetDecimalAttributeValue("Z58L1LDM");
                leftDimension.Numerator = attributes.GetDecimalAttributeValue("Z58L1LNM");
                leftDimension.Denominator = attributes.GetDecimalAttributeValue("Z58L1LDN");
                leftDimension.Description = leftDimension.GetDescription();
                unitShape.LeftDimension = leftDimension;

                var rightDimension = new Dimension();
                rightDimension.DecimalValue = attributes.GetDecimalAttributeValue("Z581RDEC");
                rightDimension.MMValue = attributes.GetDecimalAttributeValue("Z581RMIM");
                rightDimension.Base = attributes.GetDecimalAttributeValue("Z58L1RDM");
                rightDimension.Numerator = attributes.GetDecimalAttributeValue("Z58L1RNM");
                rightDimension.Denominator = attributes.GetDecimalAttributeValue("Z58L1RDN");
                rightDimension.Description = rightDimension.GetDescription();
                unitShape.RightDimension = rightDimension;

                var topDimension = new Dimension();
                topDimension.DecimalValue = attributes.GetDecimalAttributeValue("Z581TDEC");
                topDimension.MMValue = attributes.GetDecimalAttributeValue("Z581TMIM");
                topDimension.Base = attributes.GetDecimalAttributeValue("Z58L1TDM");
                topDimension.Numerator = attributes.GetDecimalAttributeValue("Z58L1TNM");
                topDimension.Denominator = attributes.GetDecimalAttributeValue("Z58L1TDN");
                topDimension.Description = topDimension.GetDescription();
                unitShape.TopDimension = topDimension;

                var s1Dimension = new Dimension();
                s1Dimension.DecimalValue = attributes.GetDecimalAttributeValue("Z5811DEC");
                s1Dimension.MMValue = attributes.GetDecimalAttributeValue("Z5811MIM");
                s1Dimension.Base = attributes.GetDecimalAttributeValue("Z58L11DM");
                s1Dimension.Numerator = attributes.GetDecimalAttributeValue("Z58L11NM");
                s1Dimension.Denominator = attributes.GetDecimalAttributeValue("Z58L11DN");
                s1Dimension.Description = s1Dimension.GetDescription();
                unitShape.Side1Dimension = s1Dimension;

                var s2Dimension = new Dimension();
                s2Dimension.DecimalValue = attributes.GetDecimalAttributeValue("Z5812DEC");
                s2Dimension.MMValue = attributes.GetDecimalAttributeValue("Z5812MIM");
                s2Dimension.Base = attributes.GetDecimalAttributeValue("Z58L12DM");
                s2Dimension.Numerator = attributes.GetDecimalAttributeValue("Z58L12NM");
                s2Dimension.Denominator = attributes.GetDecimalAttributeValue("Z58L12DN");
                s2Dimension.Description = s2Dimension.GetDescription();
                unitShape.Side2Dimension = s2Dimension;

                item.Shape = unitShape;
                item.UnitType = attributes.GetStringAttributeValue("Z5840275");

                var numLites = 0;
                if (attributes.GetDecimalAttributeValue("Z58L3TK") > 0)
                {
                    numLites = 3;
                }
                else
                {
                    numLites = attributes.GetDecimalAttributeValue("Z58L2TK") > 0 ? 2 : 1;
                }

                // Lites
                //switch (item.UnitType)
                //{
                //    case "1":
                //        numLites = 1;
                //        break;
                //    case "2":
                //        numLites = 2;
                //        break;
                //    case "3":
                //        numLites = 3;
                //        break;
                //    default:
                //        numLites = 1;
                //        break;
                //}
                var lites = new Lites();
                item.NumLites = numLites;

                // Lite 1
                var lite1 = new Lite();
                lite1.Position = 1;
                lite1.PositionName = GetLitePositionName(1, numLites);
                lite1.RoutingCode = attributes.GetStringAttributeValue("Z58L1RTG");
                lite1.RoutingDesc = attributes.GetStringAttributeDesc1("Z58L1RTG");
                lite1.RawPickItemNum = attributes.GetDecimalAttributeValue("Z5851551");
                //if (lite1.RawPickItemNum > 0)
                //{
                //    lite1.RawPickLocation = GetPickLocation(session, item.PlantNum, lite1.RawPickItemNum, "R");
                //}

                var gt1 = new GlassType();
                gt1.Thickness = attributes.GetDecimalAttributeValue("Z58L1TK");

                gt1.HeatTreatment = attributes.GetStringAttributeValue("Z58L1HT", "A");
                gt1.HeatTreatmentCode = attributes.GetStringAttributeDesc3("Z58L1HT");
                gt1.Color = attributes.GetStringAttributeValue("Z58L1CL", "");
                gt1.ColorCode = attributes.GetStringAttributeDesc3("Z58L1CL", "CL");
                //gt1.ColorCode = _udcItemManager.GetUdcItem(session, "57", "CL", gt1.Color).SpecialHandlingCode;
                gt1.Surface1Coating = attributes.GetStringAttributeValue("Z58L11C", string.Empty);
                gt1.Surface1CoatingCode = attributes.GetStringAttributeDesc3("Z58L11C");
                gt1.Surface2Coating = attributes.GetStringAttributeValue("Z58L12C", string.Empty);
                gt1.Surface2CoatingCode = attributes.GetStringAttributeDesc3("Z58L12C");
                gt1.PatternTexture = attributes.GetStringAttributeValue("Z58L1PX", string.Empty);
                gt1.PatternTextureCode = attributes.GetStringAttributeDesc3("Z58L1PX");
                gt1.PatternTextureSrf = attributes.GetDecimalAttributeValue("Z58L1PTS");
                gt1.LamiProductType = attributes.GetStringAttributeValue("Z58L1LPT", string.Empty);
                gt1.LamiProductTypeCode = attributes.GetStringAttributeDesc3("Z58L1LPT");
                //gt1.LamiProductTypeDesc = attributes.GetStringAttributeDesc1("Z58L1LPT");
                //gt1.LamiColorSrf = attributes.GetDecimalAttributeValue("Z58L1LCS");
                //gt1.LamiColorSrfDesc = attributes.GetStringAttributeDesc1("Z58L1LCS");

                gt1.LamiMixedTreatment = attributes.GetStringAttributeValue("Z5801865");
                gt1.LamiPatternSurface = attributes.GetStringAttributeValue("Z58L1LPS");
                gt1.LamiCoatingSurface = attributes.GetStringAttributeValue("Z58L1LXS");
                gt1.LamiColorSurface = attributes.GetStringAttributeValue("Z5801890");
                gt1.LamiThickness1 = attributes.GetDecimalAttributeValue("Z5801905");
                gt1.LamiInnerLayerThickness = attributes.GetDecimalAttributeValue("Z5801906");
                gt1.LamiThickness2 = attributes.GetDecimalAttributeValue("Z5801907");

                gt1.TempLogo = attributes.GetStringAttributeValue("Z58L1TL");
                gt1.CustomerSupplied = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                       attributes.GetStringAttributeValue("Z5840450").Contains("1");
                lite1.GlassType = gt1;

                lite1.QualityLevel = attributes.GetStringAttributeValue("Z58L1PQL");
                lite1.QualityLevelDesc = attributes.GetStringAttributeDesc1("Z58L1PQL");

                lite1.VGrooveType = attributes.GetStringAttributeValue("Z58L1VG");
                lite1.VGrooveTypeDesc = attributes.GetStringAttributeDesc1("Z58L1VG");
                lite1.VGrooveStyle = attributes.GetStringAttributeValue("Z58L1VGT");
                lite1.VGrooveStyleDesc = attributes.GetStringAttributeDesc1("Z58L1VGT");
                lite1.VGrooveSurface = attributes.GetDecimalAttributeValue("Z58L1VGS");
                lite1.VGrooveDrawing = attributes.GetStringAttributeValue("Z58L1VGD");
                lite1.VGrooveHorzBars = attributes.GetDecimalAttributeValue("Z5801180");
                lite1.VGrooveVertBars = attributes.GetDecimalAttributeValue("Z5801190");
                lite1.VGrooveHighOpen = attributes.GetDecimalAttributeValue("Z58L1VGH");
                lite1.VGrooveWideOpen = attributes.GetDecimalAttributeValue("Z58L1VGW");
                lite1.VGrooveTotalOpen = attributes.GetDecimalAttributeValue("Z58L1VGO");
                lite1.VGrooveBevel = attributes.GetStringAttributeValue("Z58L1BL");
                lite1.VGrooveVertOffset = attributes.GetDecimalAttributeValue("Z5801295");
                lite1.VGrooveHorzOffset = attributes.GetDecimalAttributeValue("Z5801300");
                lite1.VGrooveVertStartStop = attributes.GetStringAttributeValue("Z5801296");
                lite1.VGrooveHorzStartStop = attributes.GetStringAttributeValue("Z5801301");

                lite1.NotchHeight = attributes.GetDecimalAttributeValue("Z5801481");
                lite1.NotchLocation = attributes.GetStringAttributeValue("Z5801480");
                lite1.NotchWidth = attributes.GetDecimalAttributeValue("Z5801482");

                lite1.HolesDrawing = attributes.GetStringAttributeValue("Z58L1HL");
                lite1.HolesNum = attributes.GetDecimalAttributeValue("Z581365");

                lite1.EdgeWork = attributes.GetStringAttributeValue("Z58L1EW");
                lite1.EdgeWorkDesc = attributes.GetStringAttributeDesc1("Z58L1EW");
                lite1.EdgeDeleteWidthCode = attributes.GetStringAttributeValue("Z5805680");
                lite1.EdgeDeleteWidthValue = attributes.GetStringAttributeDesc3("Z5805680") == string.Empty
                                                 ? 0
                                                 : Convert.ToDecimal(attributes.GetStringAttributeDesc3("Z5805680"));

                lite1.SilkScreenDrawing = attributes.GetStringAttributeValue("Z58L1SD");
                lite1.SilkScreenSurface = attributes.GetDecimalAttributeValue("Z58L1SF");

                lite1.EtchingDrawing = attributes.GetStringAttributeValue("Z5801895");

                lites.Add(lite1);

                // Lite 2
                if (item.NumLites > 1)
                {
                    var lite2 = new Lite();
                    lite2.Position = 2;
                    lite2.PositionName = GetLitePositionName(2, numLites);
                    lite2.RoutingCode = attributes.GetStringAttributeValue("Z58L2RTG");
                    lite2.RoutingDesc = attributes.GetStringAttributeDesc1("Z58L2RTG");
                    lite2.RawPickItemNum = attributes.GetDecimalAttributeValue("Z5851552");
                    var gt2 = new GlassType();
                    gt2.Thickness = attributes.GetDecimalAttributeValue("Z58L2TK");
                    gt2.HeatTreatment = attributes.GetStringAttributeValue("Z58L2HT", "A");
                    gt2.HeatTreatmentCode = attributes.GetStringAttributeDesc3("Z58L2HT");
                    gt2.Color = attributes.GetStringAttributeValue("Z58L2CL", "");
                    gt2.ColorCode = attributes.GetStringAttributeDesc3("Z58L2CL", "CL");
                    gt2.Surface1Coating = attributes.GetStringAttributeValue("Z58L24C", string.Empty);
                    gt2.Surface1CoatingCode = attributes.GetStringAttributeDesc3("Z58L24C");
                    gt2.Surface2Coating = attributes.GetStringAttributeValue("Z58L23C", string.Empty);
                    gt2.Surface2CoatingCode = attributes.GetStringAttributeDesc3("Z58L23C");
                    gt2.PatternTexture = attributes.GetStringAttributeValue("Z58L2PX", string.Empty);
                    gt2.PatternTextureCode = attributes.GetStringAttributeDesc3("Z58L2PX");
                    gt2.PatternTextureSrf = attributes.GetDecimalAttributeValue("Z58L2PTS");
                    gt2.LamiProductType = attributes.GetStringAttributeValue("Z58L2LPT", string.Empty);
                    gt2.LamiProductTypeCode = attributes.GetStringAttributeDesc3("Z58L2LPT");
                    gt2.LamiPatternSurface = attributes.GetStringAttributeValue("Z58L2LPS");
                    gt2.LamiCoatingSurface = attributes.GetStringAttributeValue("Z58L2LXS");
                    gt2.TempLogo = attributes.GetStringAttributeValue("Z58L2TL");
                    gt2.CustomerSupplied = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                           attributes.GetStringAttributeValue("Z5840450").Contains("2");
                    lite2.GlassType = gt2;

                    lite2.VGrooveType = attributes.GetStringAttributeValue("Z58L2VG");
                    lite2.VGrooveTypeDesc = attributes.GetStringAttributeDesc1("Z58L2VG");
                    lite2.VGrooveStyle = attributes.GetStringAttributeValue("Z58L2VGT");
                    lite2.VGrooveStyleDesc = attributes.GetStringAttributeDesc1("Z58L2VGT");
                    lite2.VGrooveSurface = attributes.GetDecimalAttributeValue("Z58L2VGS");
                    lite2.VGrooveDrawing = attributes.GetStringAttributeValue("Z58L2VGD");
                    lite2.VGrooveHorzBars = attributes.GetDecimalAttributeValue("Z5802830");
                    lite2.VGrooveVertBars = attributes.GetDecimalAttributeValue("Z5802840");
                    lite2.VGrooveHighOpen = attributes.GetDecimalAttributeValue("Z58L2VGH");
                    lite2.VGrooveWideOpen = attributes.GetDecimalAttributeValue("Z58L2VGW");
                    lite2.VGrooveTotalOpen = attributes.GetDecimalAttributeValue("Z58L2VGO");
                    lite2.VGrooveBevel = attributes.GetStringAttributeValue("Z58L2BL");
                    lite2.VGrooveVertOffset = attributes.GetDecimalAttributeValue("Z5801295");
                    lite2.VGrooveHorzOffset = attributes.GetDecimalAttributeValue("Z5801300");
                    lite2.VGrooveVertStartStop = attributes.GetStringAttributeValue("Z5801296");
                    lite2.VGrooveHorzStartStop = attributes.GetStringAttributeValue("Z5801301");

                    lite2.NotchHeight = attributes.GetDecimalAttributeValue("Z5801481");
                    lite2.NotchLocation = attributes.GetStringAttributeValue("Z5801480");
                    lite2.NotchWidth = attributes.GetDecimalAttributeValue("Z5801482");

                    lite2.HolesDrawing = attributes.GetStringAttributeValue("Z58L2HL");
                    lite2.HolesNum = attributes.GetDecimalAttributeValue("Z5803005");

                    lite2.EdgeWork = attributes.GetStringAttributeValue("Z58L2EW");
                    lite2.EdgeDeleteWidthCode = attributes.GetStringAttributeValue("Z5805680");
                    lite2.EdgeDeleteWidthValue = attributes.GetStringAttributeDesc3("Z5805680") == string.Empty
                                                     ? 0
                                                     : Convert.ToDecimal(attributes.GetStringAttributeDesc3("Z5805680"));

                    lite2.SilkScreenDrawing = attributes.GetStringAttributeValue("Z58L2SD");
                    lite2.SilkScreenSurface = attributes.GetDecimalAttributeValue("Z58L2SF");

                    lite2.EtchingDrawing = attributes.GetStringAttributeValue("Z5803515");

                    lites.Add(lite2);
                }

                // Lite 3
                if (item.NumLites > 2)
                {
                    var lite3 = new Lite();
                    lite3.Position = 3;
                    lite3.PositionName = GetLitePositionName(3, numLites);
                    lite3.RoutingCode = attributes.GetStringAttributeValue("Z58L3RTG");
                    lite3.RoutingDesc = attributes.GetStringAttributeDesc1("Z58L3RTG");
                    lite3.RawPickItemNum = attributes.GetDecimalAttributeValue("Z5851553");
                    //if (lite3.RawPickItemNum > 0)
                    //{
                    //    lite3.RawPickLocation = GetPickLocation(session, item.PlantNum, lite3.RawPickItemNum, "R");
                    //}

                    var gt3 = new GlassType();
                    gt3.Thickness = attributes.GetDecimalAttributeValue("Z58L3TK");
                    gt3.HeatTreatment = attributes.GetStringAttributeValue("Z58L3HT", "A");
                    gt3.HeatTreatmentCode = attributes.GetStringAttributeDesc3("Z58L3HT");
                    gt3.Color = attributes.GetStringAttributeValue("Z58L3CL", "");
                    gt3.ColorCode = attributes.GetStringAttributeDesc3("Z58L3CL", "CL");
                    gt3.Surface1Coating = attributes.GetStringAttributeValue("Z58L36C", string.Empty);
                    gt3.Surface1CoatingCode = attributes.GetStringAttributeDesc3("Z58L36C");
                    gt3.Surface2Coating = attributes.GetStringAttributeValue("Z58L35C", string.Empty);
                    gt3.Surface2CoatingCode = attributes.GetStringAttributeDesc3("Z58L35C");
                    gt3.PatternTexture = attributes.GetStringAttributeValue("Z58L3PX", string.Empty);
                    gt3.PatternTextureCode = attributes.GetStringAttributeDesc3("Z58L3PX");
                    gt3.PatternTextureSrf = attributes.GetDecimalAttributeValue("Z58L3PTS");
                    gt3.LamiProductType = attributes.GetStringAttributeValue("Z58L3LPT", string.Empty);
                    gt3.LamiProductTypeCode = attributes.GetStringAttributeDesc3("Z58L3LPT");
                    gt3.LamiPatternSurface = attributes.GetStringAttributeValue("Z58L3LPS");
                    gt3.LamiCoatingSurface = attributes.GetStringAttributeValue("Z58L3LXS");
                    gt3.TempLogo = attributes.GetStringAttributeValue("Z58L3TL");
                    gt3.CustomerSupplied = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                           attributes.GetStringAttributeValue("Z5840450").Contains("3");
                    lite3.GlassType = gt3;

                    lite3.VGrooveType = attributes.GetStringAttributeValue("Z58L3VG");
                    lite3.VGrooveTypeDesc = attributes.GetStringAttributeDesc1("Z58L3VG");
                    lite3.VGrooveStyle = attributes.GetStringAttributeValue("Z58L3VGT");
                    lite3.VGrooveStyleDesc = attributes.GetStringAttributeDesc1("Z58L3VGT");
                    lite3.VGrooveSurface = attributes.GetDecimalAttributeValue("Z58L3VGS");
                    lite3.VGrooveDrawing = attributes.GetStringAttributeValue("Z58L3VGD");
                    lite3.VGrooveHorzBars = attributes.GetDecimalAttributeValue("Z5804420");
                    lite3.VGrooveVertBars = attributes.GetDecimalAttributeValue("Z5804430");
                    lite3.VGrooveHighOpen = attributes.GetDecimalAttributeValue("Z58L3VGH");
                    lite3.VGrooveWideOpen = attributes.GetDecimalAttributeValue("Z58L3VGW");
                    lite3.VGrooveTotalOpen = attributes.GetDecimalAttributeValue("Z58L3VGO");
                    lite3.VGrooveBevel = attributes.GetStringAttributeValue("Z58L3BL");
                    lite3.VGrooveVertOffset = attributes.GetDecimalAttributeValue("Z5801295");
                    lite3.VGrooveHorzOffset = attributes.GetDecimalAttributeValue("Z5801300");
                    lite3.VGrooveVertStartStop = attributes.GetStringAttributeValue("Z5801296");
                    lite3.VGrooveHorzStartStop = attributes.GetStringAttributeValue("Z5801301");

                    lite3.NotchHeight = attributes.GetDecimalAttributeValue("Z5801481");
                    lite3.NotchLocation = attributes.GetStringAttributeValue("Z5801480");
                    lite3.NotchWidth = attributes.GetDecimalAttributeValue("Z5801482");

                    lite3.HolesDrawing = attributes.GetStringAttributeValue("Z58L3HL");
                    lite3.HolesNum = attributes.GetDecimalAttributeValue("Z5804595");

                    lite3.EdgeWork = attributes.GetStringAttributeValue("Z58L3EW");
                    lite3.EdgeDeleteWidthCode = attributes.GetStringAttributeValue("Z5805680");
                    lite3.EdgeDeleteWidthValue = attributes.GetStringAttributeDesc3("Z5805680") == string.Empty
                                                     ? 0
                                                     : Convert.ToDecimal(attributes.GetStringAttributeDesc3("Z5805680"));

                    lite3.SilkScreenDrawing = attributes.GetStringAttributeValue("Z58L3SD");
                    lite3.SilkScreenSurface = attributes.GetDecimalAttributeValue("Z58L3SF");

                    lite3.EtchingDrawing = attributes.GetStringAttributeValue("Z5805105");

                    lites.Add(lite3);
                }
                item.Lites = lites;

                // Surfaces
                var surface1 = new Surface();
                surface1.PreserveColor = attributes.GetStringAttributeValue("Z587566");
                surface1.PreserveCutback = attributes.GetStringAttributeValue("Z587563");
                surface1.Opacifier = attributes.GetStringAttributeValue("Z587572");
                surface1.OpacifierDesc = attributes.GetStringAttributeDesc1("Z587572");
                surface1.ScoringWidth = attributes.GetDecimalAttributeValue("Z587569");

                item.ExteriorSurface = surface1;

                var surface2 = new Surface();
                surface2.PreserveColor = attributes.GetStringAttributeValue("Z587578");
                surface2.PreserveCutback = attributes.GetStringAttributeValue("Z587575");
                surface2.Opacifier = attributes.GetStringAttributeValue("Z587584");
                surface2.OpacifierDesc = attributes.GetStringAttributeDesc1("Z587584");
                surface2.ScoringWidth = attributes.GetDecimalAttributeValue("Z587581");

                item.InteriorSurface = surface2;

                // Logo
                var logo = new Logo();
                logo.AnsiLogo = attributes.GetStringAttributeValue("Z58L1ALT");
                logo.XOffset = attributes.GetDecimalAttributeValue("Z58L1LX");
                logo.YOffset = attributes.GetDecimalAttributeValue("Z58L1LY");
                logo.NfrcLogo1 = attributes.GetStringAttributeValue("Z58NFRC");
                logo.NfrcLogo2 = attributes.GetStringAttributeValue("Z58NFRC2");
                logo.NfrcHorizontalOffset = attributes.GetDecimalAttributeValue("Z58NFRCH");
                logo.NfrcVerticalOffset = attributes.GetDecimalAttributeValue("Z58NFRCV");
                logo.IgLogo = attributes.GetStringAttributeValue("Z5840430");
                item.Logo = logo;

            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return item;
        }

        public string GetLitePositionName(int litePos, int totalLites)
        {
            string val = "N/A";
            if (totalLites >= 2)
            {
                if (litePos == 1)
                {
                    val = "EXT";
                }
                else
                {
                    if (totalLites - litePos == 0)
                    {
                        val = "INT";
                    }
                    else
                    {
                        val = "MID";
                    }
                }
            }
            return val;
        }
    }
}
