using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class HVRackTagManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ItemAttributeManager _itemAttributeMgr;

        private const string shopFloorSystemCGHV = "3";
        private const string shopFloorSystemFG = "4";
        private const string shopFloorSystemTG = "5";
        private const string shopFloorSystemCGHVOutside = "8";

        public HVRackTagManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"RackTagManager.GetMfgPlant", GetMfgPlant()},
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
            _itemAttributeMgr = new ItemAttributeManager(e1Session);
        }

        public HVRackTagItem GetHVRackTag(BusinessSystemSession e1Session, string requestingPlantCode, string serialNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + requestingPlantCode + " BEGIN: " + DateTime.Now.ToLocalTime());
            var rackTag = new HVRackTagItem();
            if (serialNum.Length == 6)
            {
                rackTag.ErrorMessage = "HIGH VOLUME serial number REQUIRED";
                rackTag.SerialNumber = serialNum;
                return rackTag;
            }
            var cnSF = new SqlConnection();
            try
            {
                // Resquesting Plant
                var reqPlant = PlantManager.GetPlant(requestingPlantCode);

                // Mfg Plant and RackId
                (string producingPlantNum, decimal rackId) = GetMfgPlant(e1Session, serialNum);
                if (rackId > 0)
                {
                    var producingPlant = PlantManager.GetPlant(producingPlantNum);

                    // Current Plant
                    var currentPlantNum = GetCurrentPlant(e1Session, rackId);
                    var currentPlant = PlantManager.GetPlant(currentPlantNum);

                    // Mfg plant E1 representation
                    var rackTagE1Mfg = GetRackTagE1Mfg(e1Session, producingPlantNum, rackId);

                    // Current E1 representation
                    var rackTagE1Current = GetRackTagE1Current(e1Session, rackId);

                    // Requesting plant E1 representation
                    var rackTagE1Requesting = GetRackTagE1Requesting(e1Session, requestingPlantCode, rackId);

                    // Get the RawTags data from the requesting plant
                    cnSF = GetSFSqlConnection(reqPlant.PlantCode);
                    var rawTag = GetRawTag(cnSF, serialNum, rackId);

                    // Mfg plant SF representation
                    var shopFloorSystem = rackTagE1Mfg.ShopFloorSystem;
                    var actualMfgPlantNum = producingPlantNum;

                    if (producingPlant.PlantCode.EndsWith("FG") || producingPlant.PlantCode.EndsWith("TG"))
                    {
                        // Get the FG/TG FinishedGoods data from the Data Warehouse
                        var cnDW = GetDWSqlConnection();
                        var rackTagFinishedGoods = GetFinishedGoods(e1Session, cnDW, serialNum, rackId);

                        // Combine the FinshedGoods
                        rackTag = rawTag;
                        rackTag.SerialNumber = serialNum;
                        rackTag.RackId = rackId;
                        rackTag.ScheduleNum = rackTagE1Mfg.ScheduleNum;
                        rackTag.ShopFloorSystem = rackTagE1Mfg.ShopFloorSystem;

                        var plantManager = new PlantManager(e1Session);
                        var actualMfgPlant = PlantManager.GetPlant(actualMfgPlantNum);

                        producingPlant.Address = plantManager.GetPlantAddress(e1Session, producingPlant.AddressNum);
                        actualMfgPlant.Address = plantManager.GetPlantAddress(e1Session, actualMfgPlant.AddressNum);
                        reqPlant.Address = plantManager.GetPlantAddress(e1Session, reqPlant.AddressNum);
                        currentPlant.Address = plantManager.GetPlantAddress(e1Session, currentPlant.AddressNum);

                        rackTag.ProducingPlantID = producingPlant.PlantNum;
                        rackTag.ProducingPlantCode = producingPlant.PlantCode;
                        rackTag.ProducingPlantName = producingPlant.Name;
                        rackTag.ProducingPlantAddress1 = producingPlant.Address.Address1;
                        rackTag.ProducingPlantAddress2 = producingPlant.Address.Address2;
                        rackTag.ProducingPlantCity = producingPlant.Address.City;
                        rackTag.ProducingPlantState = producingPlant.Address.State;
                        rackTag.ProducingPlantPostalCode = producingPlant.Address.PostalCode;
                        rackTag.ProducingPlantAddressNum = producingPlant.AddressNum;
                        rackTag.ProducingPlantCustomer = rackTagE1Mfg.Customer;
                        rackTag.ProducingPlantShipToCustomer = rackTagE1Mfg.ShipToCustomer;

                        rackTag.ActualMfgPlantID = actualMfgPlant.PlantNum;
                        rackTag.ActualMfgPlantCode = actualMfgPlant.PlantCode;
                        rackTag.ActualMfgPlantName = actualMfgPlant.Name;
                        rackTag.ActualMfgPlantAddress1 = actualMfgPlant.Address.Address1;
                        rackTag.ActualMfgPlantAddress2 = actualMfgPlant.Address.Address2;
                        rackTag.ActualMfgPlantCity = actualMfgPlant.Address.City;
                        rackTag.ActualMfgPlantState = actualMfgPlant.Address.State;
                        rackTag.ActualMfgPlantPostalCode = actualMfgPlant.Address.PostalCode;
                        rackTag.ActualMfgPlantAddressNum = actualMfgPlant.AddressNum;
                        rackTag.ActualMfgPlantCustomer = rackTagE1Mfg.Customer;
                        rackTag.ActualMfgPlantShipToCustomer = rackTagE1Mfg.ShipToCustomer;

                        rackTag.RequestingPlantID = reqPlant.PlantNum;
                        rackTag.RequestingPlantCode = reqPlant.PlantCode;
                        rackTag.RequestingPlantName = reqPlant.Name;
                        rackTag.RequestingPlantAddress1 = reqPlant.Address.Address1;
                        rackTag.RequestingPlantAddress2 = reqPlant.Address.Address2;
                        rackTag.RequestingPlantCity = reqPlant.Address.City;
                        rackTag.RequestingPlantState = reqPlant.Address.State;
                        rackTag.RequestingPlantPostalCode = reqPlant.Address.PostalCode;
                        rackTag.RequestingPlantAddressNum = reqPlant.AddressNum;
                        rackTag.RequestingPlantCustomer = rackTagE1Requesting.Customer;
                        rackTag.RequestingPlantShipToCustomer = rackTagE1Requesting.ShipToCustomer;

                        rackTag.CurrentPlantID = currentPlant.PlantNum;
                        rackTag.S7MCU = currentPlant.PlantNum;
                        rackTag.CurrentPlantCode = currentPlant.PlantCode;
                        rackTag.CurrentPlantName = currentPlant.Name;
                        rackTag.CurrentPlantAddress1 = currentPlant.Address.Address1;
                        rackTag.CurrentPlantAddress2 = currentPlant.Address.Address2;
                        rackTag.CurrentPlantCity = currentPlant.Address.City;
                        rackTag.CurrentPlantState = currentPlant.Address.State;
                        rackTag.CurrentPlantPostalCode = currentPlant.Address.PostalCode;
                        rackTag.CurrentPlantAddressNum = currentPlant.AddressNum;
                        rackTag.CurrentPlantCustomer = rackTagE1Current.Customer;
                        rackTag.CurrentPlantShipToCustomer = rackTagE1Current.ShipToCustomer;

                        rackTag.Customer = rackTagE1Mfg.Customer;
                        rackTag.ShipToCustomer = rackTagE1Mfg.ShipToCustomer;

                        rackTag.MfgRackQuantity = rackTagFinishedGoods.Quantity;
                        rackTag.Quantity = rackTagFinishedGoods.Quantity;
                        rackTag.TotalWeightLbs = rackTagFinishedGoods.TotalWeightLbs;
                        rackTag.AnsiiLogoType = rackTagFinishedGoods.AnsiiLogoType;
                        rackTag.OriginalCustomer = rackTagFinishedGoods.OriginalCustomer;
                        rackTag.Shift = rackTagFinishedGoods.Shift;
                        rackTag.GlassHandler = rackTagFinishedGoods.GlassHandler;
                        rackTag.GlassHandler2 = rackTagFinishedGoods.GlassHandler2;
                    }
                    else
                    {
                        if (shopFloorSystem == shopFloorSystemCGHVOutside && (producingPlantNum == "05" || producingPlantNum == "04" || producingPlantNum == "08")) actualMfgPlantNum = "32";
                        var plantManager = new PlantManager(e1Session);
                        var actualMfgPlant = PlantManager.GetPlant(actualMfgPlantNum);
                        cnSF = GetSFSqlConnection(actualMfgPlant.PlantCode);

                        //rackTagE1Mfg = AddAttributes(e1Session, rackTagE1Mfg);

                        HVRackTagItem rackTagSFMfg;
                        if (actualMfgPlantNum == "36") // SGRT
                        {
                            rackTagSFMfg = GetRackTagSFMfgSGRT(cnSF, serialNum, rackId);
                            rackTagE1Mfg = AddRackTagSFMfg(rackTagE1Mfg, rackTagSFMfg);
                        }
                        else
                        {
                            rackTagSFMfg = GetRackTagSFMfg(cnSF, serialNum, rackId);
                            rackTagE1Mfg = AddRackTagSFMfg(rackTagE1Mfg, rackTagSFMfg);
                            rackTagE1Mfg = AddRawTags(e1Session, rackTagE1Mfg, rawTag);
                        }

                        rackTag = rackTagE1Mfg; 
                        rackTag.SerialNumber = serialNum;
                        rackTag.RackId = rackId;

                        rackTag = AddAttributes(e1Session, rackTag);

                        //var plantManager = new PlantManager(e1Session);
                        //var actualMfgPlant = PlantManager.GetPlant(actualMfgPlantCode);

                        producingPlant.Address = plantManager.GetPlantAddress(e1Session, producingPlant.AddressNum);
                        actualMfgPlant.Address = plantManager.GetPlantAddress(e1Session, actualMfgPlant.AddressNum);
                        reqPlant.Address = plantManager.GetPlantAddress(e1Session, reqPlant.AddressNum);
                        currentPlant.Address = plantManager.GetPlantAddress(e1Session, currentPlant.AddressNum);

                        rackTag.ProducingPlantID = producingPlant.PlantNum;
                        rackTag.ProducingPlantCode = producingPlant.PlantCode;
                        rackTag.ProducingPlantName = producingPlant.Name;
                        rackTag.ProducingPlantAddress1 = producingPlant.Address.Address1;
                        rackTag.ProducingPlantAddress2 = producingPlant.Address.Address2;
                        rackTag.ProducingPlantCity = producingPlant.Address.City;
                        rackTag.ProducingPlantState = producingPlant.Address.State;
                        rackTag.ProducingPlantPostalCode = producingPlant.Address.PostalCode;
                        rackTag.ProducingPlantAddressNum = producingPlant.AddressNum;
                        rackTag.ProducingPlantCustomer = string.IsNullOrEmpty(rackTagE1Mfg.Customer) ? rackTagE1Mfg.Customer : rackTagSFMfg.Customer;
                        rackTag.ProducingPlantShipToCustomer = rackTagE1Mfg.ShipToCustomer;

                        rackTag.ActualMfgPlantID = actualMfgPlant.PlantNum;
                        rackTag.ActualMfgPlantCode = actualMfgPlant.PlantCode;
                        rackTag.ActualMfgPlantName = actualMfgPlant.Name;
                        rackTag.ActualMfgPlantAddress1 = actualMfgPlant.Address.Address1;
                        rackTag.ActualMfgPlantAddress2 = actualMfgPlant.Address.Address2;
                        rackTag.ActualMfgPlantCity = actualMfgPlant.Address.City;
                        rackTag.ActualMfgPlantState = actualMfgPlant.Address.State;
                        rackTag.ActualMfgPlantPostalCode = actualMfgPlant.Address.PostalCode;
                        rackTag.ActualMfgPlantAddressNum = actualMfgPlant.AddressNum;
                        rackTag.ActualMfgPlantCustomer = string.IsNullOrEmpty(rackTagE1Mfg.Customer) ? rackTagE1Mfg.Customer : rackTagSFMfg.Customer;
                        rackTag.ActualMfgPlantShipToCustomer = rackTagE1Mfg.ShipToCustomer;

                        rackTag.RequestingPlantID = reqPlant.PlantNum;
                        rackTag.RequestingPlantCode = reqPlant.PlantCode;
                        rackTag.RequestingPlantName = reqPlant.Name;
                        rackTag.RequestingPlantAddress1 = reqPlant.Address.Address1;
                        rackTag.RequestingPlantAddress2 = reqPlant.Address.Address2;
                        rackTag.RequestingPlantCity = reqPlant.Address.City;
                        rackTag.RequestingPlantState = reqPlant.Address.State;
                        rackTag.RequestingPlantPostalCode = reqPlant.Address.PostalCode;
                        rackTag.RequestingPlantAddressNum = reqPlant.AddressNum;
                        rackTag.RequestingPlantCustomer = rackTagE1Requesting.Customer;
                        if (string.IsNullOrEmpty(rackTag.RequestingPlantCustomer) && (rackTag.RequestingPlantID == rackTag.ActualMfgPlantID))
                        {
                            rackTag.RequestingPlantCustomer = rackTag.ActualMfgPlantCustomer;
                        }
                        rackTag.RequestingPlantShipToCustomer = rackTagE1Requesting.ShipToCustomer;

                        rackTag.CurrentPlantID = currentPlant.PlantNum;
                        rackTag.S7MCU = currentPlant.PlantNum;
                        rackTag.CurrentPlantCode = currentPlant.PlantCode;
                        rackTag.CurrentPlantName = currentPlant.Name;
                        rackTag.CurrentPlantAddress1 = currentPlant.Address.Address1;
                        rackTag.CurrentPlantAddress2 = currentPlant.Address.Address2;
                        rackTag.CurrentPlantCity = currentPlant.Address.City;
                        rackTag.CurrentPlantState = currentPlant.Address.State;
                        rackTag.CurrentPlantPostalCode = currentPlant.Address.PostalCode;
                        rackTag.CurrentPlantAddressNum = currentPlant.AddressNum;
                        rackTag.CurrentPlantCustomer = rackTagE1Current.Customer;
                        if (string.IsNullOrEmpty(rackTag.CurrentPlantCustomer) && (rackTag.CurrentPlantID == rackTag.ActualMfgPlantID))
                        {
                            rackTag.CurrentPlantCustomer = rackTag.ActualMfgPlantCustomer;
                        }
                        rackTag.CurrentPlantShipToCustomer = rackTagE1Current.ShipToCustomer;

                        rackTag.Customer = rackTagE1Mfg.Customer;
                        rackTag.ShipToCustomer = rackTagE1Mfg.ShipToCustomer;

                        rackTag.Quantity = rackTagE1Current.Quantity;
                        rackTag.CurrentQuantity = rackTagE1Current.Quantity;
                        rackTag.MfgRackQuantity = rackTagE1Current.Quantity;

                        rackTag.GlassUnitWeightLbs = rackTag.WeightLbs;
                        rackTag.FreeLiteUnitWeightLbs = (rackTag.FreeLiteQuantity == 1) ? rackTag.GlassUnitWeightLbs : 0;
                        rackTag.TotalGlassWeightLbs = rackTag.GlassUnitWeightLbs * rackTag.Quantity; 
                        rackTag.RackTareWeightLbs = (rackTag.RackTareWeightLbs > 0) ? rackTag.RackTareWeightLbs : rackTagE1Current.RackTareWeightLbs;
                        rackTag.TotalRackWeightLbs = rackTag.TotalGlassWeightLbs + rackTag.RackTareWeightLbs;
                        rackTag.TotalWeightLbs = rackTag.TotalGlassWeightLbs;

                        if (!rackTag.E1ItemNumber.Equals(rackTagSFMfg.E1ItemNumber)) // ReClass
                        {
                            var f4102Mgr = new F4102Manager(e1Session);
                            var custNameManager = new CustomerNameManager(e1Session);
                            var plannerNum = f4102Mgr.GetPlannerNum(e1Session, rackTag.CurrentPlantID, Utilities.Data.ParseStringToDecimal(rackTag.E1ItemNumber));
                            if (plannerNum > 0)
                            {
                                rackTag.PlannerCustomerNumber = plannerNum;
                                rackTag.PlannerCustomer = custNameManager.GetName(e1Session, plannerNum);
                                rackTag.CustVendorID = plannerNum.ToString();
                                rackTag.Customer = rackTag.PlannerCustomer;
                                rackTag.ActualMfgPlantCustomer = rackTag.PlannerCustomer;
                                rackTag.CurrentPlantCustomer = rackTag.PlannerCustomer;
                                rackTag.ProducingPlantShipToCustomer = rackTag.PlannerCustomer;
                                rackTag.RequestingPlantCustomer = rackTag.PlannerCustomer;
                            }
                            else
                            {
                                rackTag.PlannerCustomerNumber = 0;
                                rackTag.PlannerCustomer = "";
                                rackTag.CustVendorID = "";
                                rackTag.Customer = "";
                                rackTag.ActualMfgPlantCustomer = "";
                                rackTag.CurrentPlantCustomer = "";
                                rackTag.ProducingPlantShipToCustomer = "";
                                rackTag.RequestingPlantCustomer = "";
                            }
                        }

                        if (!string.IsNullOrEmpty(rackTag.BaseFractional) && !string.IsNullOrEmpty(rackTag.LeftFractional))
                        {
                            rackTag.Dimensions = $"{rackTag.BaseFractional} X {rackTag.LeftFractional}";
                        }
                    }
                }
                else
                {
                    Logger.Error($"ERROR: Serial Number {serialNum} not found");
                    rackTag.ErrorMessage = $"ERROR: Serial Number {serialNum} not found";
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                rackTag.ErrorMessage = $"ERROR: {e.Message}";
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                cnSF.Close();
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return rackTag;
        }

        public HVRackTagItem AddAttributes(BusinessSystemSession e1Session, HVRackTagItem rackTagE1Mfg)
        {
            var custNameMgr = new CustomerNameManager(e1Session);

            try
            {
                var segments = rackTagE1Mfg.ConfigId > 0 ? _itemAttributeMgr.GetConfiguredItemAttributes(e1Session, rackTagE1Mfg.ConfigId) : _itemAttributeMgr.GetDiscreteItemAttributes(e1Session, decimal.Parse(rackTagE1Mfg.E1ItemNumber));

                var rackTagItem = new RackTagItem();
                // Glass Type
                rackTagItem.Lite1Thickness = segments.GetDecimalAttributeValue("Z58L1TK");
                rackTagItem.Lite1Heat = segments.GetStringAttributeValue("Z58L1HT") ?? "";
                rackTagItem.Lite1FirstSrfCoat = segments.GetStringAttributeValue("Z58L11C") ?? "";
                rackTagItem.Lite1SecondSrfCoat = segments.GetStringAttributeValue("Z58L12C") ?? "";
                rackTagItem.Lite1Color = segments.GetStringAttributeDesc3("Z58L1CL") ?? "";
                //rackTagItem.Lite1Color = segments.GetStringAttributeDesc3("Z58L1CL", "CL") ?? "";
                rackTagItem.Lite1Pattern = segments.GetStringAttributeValue("Z58L1PX") ?? "";
                rackTagItem.Lite1LamiType = segments.GetStringAttributeValue("Z58L1LPT") ?? "";
                rackTagItem.Lite1LamiDesc = segments.GetStringAttributeDesc1("Z58L1LPT") ?? "";
                rackTagItem.Lite1LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L1LPT") ?? "";
                rackTagItem.Lite1VGroove = segments.GetStringAttributeDesc1("Z58L1VG") ?? "";
                rackTagItem.Lite1TempLogo = segments.GetStringAttributeValue("Z58L1TL") ?? "";
                rackTagItem.Lite1Routing = segments.GetStringAttributeDesc1("Z58L1RTG") ?? "";
                rackTagItem.Lite1EdgeWork = segments.GetStringAttributeDesc1("Z58L1EW") ?? "";
                rackTagItem.Lite1AnsiLogo = segments.GetStringAttributeValue("Z58L1ALT") ?? "";

                rackTagItem.Lite1PatTextSrf = segments.GetStringAttributeValue("Z58L1PTS") ?? "";
                if (rackTagItem.Lite1PatTextSrf != null) rackTagItem.Lite1PatTextSrf = rackTagItem.Lite1PatTextSrf.Length > 2 ? rackTagItem.Lite1PatTextSrf.Substring(0, 1) : rackTagItem.Lite1PatTextSrf;

                // Shapes
                rackTagItem.ShapeNum = segments.GetDecimalAttributeValue("Z58SHAPE");
                rackTagItem.Base = segments.GetDecimalAttributeValue("Z581BDEC");
                rackTagItem.Left = segments.GetDecimalAttributeValue("Z581LDEC");
                rackTagItem.Right = segments.GetDecimalAttributeValue("Z581RDEC");
                rackTagItem.Top = segments.GetDecimalAttributeValue("Z581TDEC");
                rackTagItem.S1 = segments.GetDecimalAttributeValue("Z5811DEC");
                rackTagItem.S2 = segments.GetDecimalAttributeValue("Z5812DEC");

                rackTagItem.BaseMM = segments.GetDecimalAttributeValue("Z581BMIM");
                rackTagItem.LeftMM = segments.GetDecimalAttributeValue("Z581LMIM");
                rackTagItem.RightMM = segments.GetDecimalAttributeValue("Z581RMIM");
                rackTagItem.TopMM = segments.GetDecimalAttributeValue("Z581TMIM");

                rackTagItem.TinOut = segments.GetStringAttributeDesc1("Z58RTNIO") ?? "";
                rackTagItem.RackType = segments.GetStringAttributeValue("Z58RT") ?? "";

                // Edge Delete Width
                rackTagItem.EdgeDeleteWidth = segments.GetStringAttributeDesc2("Z5805680") ?? "";

                // Unit Weight
                var qty = segments.GetDecimalAttributeValue("Z5840360");
                if (rackTagE1Mfg.ConfigId == 0)
                {
                    var unitWeight = segments.GetDecimalAttributeValue("Z58GLWT");
                    rackTagItem.RackGlassWeight = Math.Round(unitWeight * qty, 3);
                    rackTagItem.UnitWeight = unitWeight; 
                    rackTagE1Mfg.WeightLbs = (unitWeight > 0) ? unitWeight : rackTagE1Mfg.WeightLbs;
                    rackTagE1Mfg.GlassUnitWeightLbs = (rackTagItem.UnitWeight > 0) ? rackTagItem.UnitWeight : rackTagE1Mfg.GlassUnitWeightLbs;
                }
                else
                {
                    var totalWeight = segments.GetDecimalAttributeValue("Z58GLWT");
                    rackTagItem.RackGlassWeight = totalWeight;
                    rackTagItem.UnitWeight = (qty == 0) ? 0 : Math.Round(totalWeight / qty, 3);
                    rackTagE1Mfg.WeightLbs = (rackTagItem.UnitWeight > 0) ? rackTagItem.UnitWeight : rackTagE1Mfg.WeightLbs;
                    rackTagE1Mfg.GlassUnitWeightLbs = (rackTagItem.UnitWeight > 0) ? rackTagItem.UnitWeight : rackTagE1Mfg.GlassUnitWeightLbs;
                }

                // Sqft
                rackTagItem.UnitActualSqft = segments.GetDecimalAttributeValue("Z58USF");
                rackTagItem.UnitBillableSqft = segments.GetDecimalAttributeValue("Z58BSF");

                // Other
                rackTagItem.CardinalProductType = segments.GetStringAttributeValue("Z5840310") ?? "";
                rackTagItem.CrateType = segments.GetStringAttributeValue("Z58CRATE") ?? "";
                rackTagItem.CustomerSupplied = segments.GetStringAttributeValue("Z5840450") ?? "";
                rackTagItem.CustomerLiteNumber = segments.GetDecimalAttributeValue("Z58L1CLN");

                // Original Customer
                rackTagItem.OrigSoldToNum = segments.GetDecimalAttributeValue("Z58CTL");
                rackTagItem.OrigShipToNum = segments.GetDecimalAttributeValue("Z5840425");
                rackTagItem.OrigSoldToName = custNameMgr.GetName(e1Session, rackTagItem.OrigSoldToNum);
                rackTagItem.OrigShipToName = custNameMgr.GetName(e1Session, rackTagItem.OrigSoldToNum);

                // Map attributes
                rackTagE1Mfg.Customer = !string.IsNullOrEmpty(rackTagItem.OrigSoldToName) ? rackTagItem.OrigSoldToName : rackTagE1Mfg.Customer;
                if (string.IsNullOrEmpty(rackTagE1Mfg.Customer))
                {
                    var f4102Mgr = new F4102Manager(e1Session);
                    var custNameManager = new CustomerNameManager(e1Session);
                    var plannerNum = f4102Mgr.GetPlannerNum(e1Session, rackTagE1Mfg.PlantID, Data.ParseStringToDecimal(rackTagE1Mfg.E1ItemNumber));
                    if (plannerNum > 0)
                    {
                        rackTagE1Mfg.PlannerCustomerNumber = plannerNum;
                        rackTagE1Mfg.PlannerCustomer = custNameManager.GetName(e1Session, plannerNum);
                        rackTagE1Mfg.CustVendorID = plannerNum.ToString(CultureInfo.InvariantCulture);
                        rackTagE1Mfg.Customer = rackTagE1Mfg.PlannerCustomer;
                    }
                }

                rackTagE1Mfg.ActualMfgPlantCustomer = rackTagE1Mfg.Customer;
                rackTagE1Mfg.ProducingPlantCustomer = rackTagE1Mfg.Customer;
                rackTagE1Mfg.ShipToCustomer = !string.IsNullOrEmpty(rackTagItem.OrigShipToName) ? rackTagItem.OrigShipToName : rackTagE1Mfg.ShipToCustomer;
                rackTagE1Mfg.ActualMfgPlantShipToCustomer = rackTagE1Mfg.ShipToCustomer;
                rackTagE1Mfg.ProducingPlantShipToCustomer = rackTagE1Mfg.ShipToCustomer;
                rackTagE1Mfg.GlassThickness = (rackTagItem.Lite1Thickness > 0) ? rackTagItem.Lite1Thickness : rackTagE1Mfg.GlassThickness;
                rackTagE1Mfg.GlassHeatTreatment =  !string.IsNullOrEmpty(rackTagItem.Lite1Heat) ? rackTagItem.Lite1Heat : rackTagE1Mfg.GlassHeatTreatment;
                rackTagE1Mfg.FirstSurfaceCoat = !string.IsNullOrEmpty(rackTagItem.Lite1FirstSrfCoat) ? rackTagItem.Lite1FirstSrfCoat : rackTagE1Mfg.FirstSurfaceCoat; 
                rackTagE1Mfg.SecondSurfaceCoat = !string.IsNullOrEmpty(rackTagItem.Lite1SecondSrfCoat) ? rackTagItem.Lite1SecondSrfCoat : rackTagE1Mfg.SecondSurfaceCoat; 
                rackTagE1Mfg.PackingTinInTinOut = !string.IsNullOrEmpty(rackTagItem.TinOut) ? rackTagItem.TinOut : rackTagE1Mfg.PackingTinInTinOut; 
                rackTagE1Mfg.RackType = !string.IsNullOrEmpty(rackTagItem.RackType) ? rackTagItem.RackType : rackTagE1Mfg.RackType; 

                rackTagE1Mfg.Quantity = (qty > 0) ? qty : rackTagE1Mfg.Quantity;
                rackTagE1Mfg.SqftActual = (rackTagItem.UnitActualSqft > 0) ? rackTagItem.UnitActualSqft : rackTagE1Mfg.SqftActual; 
                //rackTagE1Mfg.WeightLbs = (totalWeight > 0) ? totalWeight : rackTagE1Mfg.WeightLbs;
                //rackTagE1Mfg.GlassUnitWeightLbs = (rackTagItem.UnitWeight > 0) ? rackTagItem.UnitWeight : rackTagE1Mfg.GlassUnitWeightLbs;
                rackTagE1Mfg.BaseMM = (rackTagItem.BaseMM > 0) ? rackTagItem.BaseMM : rackTagE1Mfg.BaseMM; 
                rackTagE1Mfg.LeftMM = (rackTagItem.LeftMM > 0) ? rackTagItem.LeftMM : rackTagE1Mfg.LeftMM; 
                rackTagE1Mfg.RightMM = rackTagItem.ShapeNum == 0 ? rackTagItem.LeftMM : rackTagItem.RightMM;
                rackTagE1Mfg.TopMM = rackTagItem.ShapeNum == 0 ? rackTagItem.BaseMM : rackTagItem.TopMM;
                rackTagE1Mfg.BaseDec = (rackTagItem.Base > 0) ? rackTagItem.Base : rackTagE1Mfg.BaseDec; 
                rackTagE1Mfg.LeftDec = (rackTagItem.Left > 0) ? rackTagItem.Left : rackTagE1Mfg.LeftDec; 
                rackTagE1Mfg.SqftBillable = (rackTagItem.UnitBillableSqft > 0) ? rackTagItem.UnitBillableSqft : rackTagE1Mfg.SqftBillable; 
                rackTagE1Mfg.EdgeDelete = !string.IsNullOrEmpty(rackTagItem.EdgeDeleteWidth) ? rackTagItem.EdgeDeleteWidth : rackTagE1Mfg.EdgeDelete; 
                rackTagE1Mfg.Lite1_ColorShortDesc = !string.IsNullOrEmpty(rackTagItem.Lite1Color) ? rackTagItem.Lite1Color : rackTagE1Mfg.Lite1_ColorShortDesc; 
                rackTagE1Mfg.Lite1_PatternShortDesc = !string.IsNullOrEmpty(rackTagItem.Lite1Pattern) ? rackTagItem.Lite1Pattern : rackTagE1Mfg.Lite1_PatternShortDesc; 
                rackTagE1Mfg.Lite1_LamiShortDesc = !string.IsNullOrEmpty(rackTagItem.Lite1LamiDesc) ? rackTagItem.Lite1LamiDesc : rackTagE1Mfg.Lite1_LamiShortDesc; 
                rackTagE1Mfg.OriginalCustomer = !string.IsNullOrEmpty(rackTagItem.OrigSoldToName) ? rackTagItem.OrigSoldToName : rackTagE1Mfg.OriginalCustomer; 

                var baseL = (int)rackTagE1Mfg.BaseDec;
                var leftL = (int)rackTagE1Mfg.LeftDec;
                var baseDict = E1DimensionManager.GetFractionalValues(e1Session, rackTagE1Mfg.BaseDec % 1.0m);
                var leftDict = E1DimensionManager.GetFractionalValues(e1Session, rackTagE1Mfg.LeftDec % 1.0m);
                var baseFrac = baseDict["FDZ58FRAC"].ToString();
                var baseNumr = Convert.ToDecimal(baseDict["FDZ58NUMR"]);
                var baseDenm = Convert.ToDecimal(baseDict["FDZ58DENM"]);
                var leftFrac = leftDict["FDZ58FRAC"].ToString();
                var leftNumr = Convert.ToDecimal(leftDict["FDZ58NUMR"]);
                var leftDenm = Convert.ToDecimal(leftDict["FDZ58DENM"]);
                //if (baseL > 0 && leftL > 0)
                //{
                //    if (baseFrac.Equals("0/0") )
                //    {
                //        rackTagE1Mfg.BaseFractional = $"{baseL}";
                //    }
                //    else
                //    {
                //        rackTagE1Mfg.BaseFractional = $"{baseL} {baseFrac}";
                //    }
                //    if (leftFrac.Equals("0/0"))
                //    {
                //        rackTagE1Mfg.LeftFractional = $"{leftL}";
                //    }
                //    else
                //    {
                //        rackTagE1Mfg.LeftFractional = $"{leftL} {leftFrac}";
                //    }
                //    var leftDim = Utilities.Common.CastToFractionalDimension(leftL, leftNumr, leftDenm);
                //    var baseDim = Utilities.Common.CastToFractionalDimension(baseL, baseNumr, baseDenm);
                //    rackTagE1Mfg.Dimensions = $@"{leftDim} X {baseDim}";
                //    rackTagE1Mfg.DimensionsMetric = $@"{((rackTagE1Mfg.LeftMM > 0) ? rackTagE1Mfg.LeftMM : rackTagE1Mfg.RightMM)} X {rackTagE1Mfg.BaseMM}";
                //}
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return rackTagE1Mfg;
        }

        public HVRackTagItem AddRawTags(BusinessSystemSession e1Session, HVRackTagItem rackTagE1Mfg, HVRackTagItem rawTag)
        {
            try
            {
                rackTagE1Mfg.Customer = !string.IsNullOrEmpty(rackTagE1Mfg.Customer) ? rackTagE1Mfg.Customer : rawTag.Customer;
                rackTagE1Mfg.PackingTinInTinOut = !string.IsNullOrEmpty(rackTagE1Mfg.PackingTinInTinOut) ? rackTagE1Mfg.PackingTinInTinOut : rawTag.PackingTinInTinOut;
                //rackTagE1Mfg.OriginalE1ItemNumber = rackTagE1Mfg.E1ItemNumber;

                rackTagE1Mfg.E1ItemNumber = !string.IsNullOrEmpty(rawTag.E1ItemNumber) ? rawTag.E1ItemNumber : rackTagE1Mfg.E1ItemNumber;

                rackTagE1Mfg.GlassThickness = rawTag.GlassThickness; //(rackTagE1Mfg.GlassThickness > 0) ? rackTagE1Mfg.GlassThickness : rawTag.GlassThickness;
                rackTagE1Mfg.GlassHeatTreatment = rawTag.GlassHeatTreatment; //!string.IsNullOrEmpty(rackTagE1Mfg.GlassHeatTreatment) ? rackTagE1Mfg.GlassHeatTreatment : rawTag.GlassHeatTreatment;
                rackTagE1Mfg.FirstSurfaceCoat = rawTag.FirstSurfaceCoat; //!string.IsNullOrEmpty(rackTagE1Mfg.FirstSurfaceCoat) ? rackTagE1Mfg.FirstSurfaceCoat : rawTag.FirstSurfaceCoat;
                rackTagE1Mfg.SecondSurfaceCoat = rawTag.SecondSurfaceCoat; //!string.IsNullOrEmpty(rackTagE1Mfg.SecondSurfaceCoat) ? rackTagE1Mfg.SecondSurfaceCoat : rawTag.SecondSurfaceCoat;

                rackTagE1Mfg.SecondSurfaceCoatDesc1 = rawTag.SecondSurfaceCoatDesc1; //!string.IsNullOrEmpty(rackTagE1Mfg.SecondSurfaceCoatDesc1) ? rackTagE1Mfg.SecondSurfaceCoatDesc1 : rawTag.SecondSurfaceCoatDesc1;
                rackTagE1Mfg.TradeMarkInfoToPrint = rawTag.TradeMarkInfoToPrint; //!string.IsNullOrEmpty(rackTagE1Mfg.TradeMarkInfoToPrint) ? rackTagE1Mfg.TradeMarkInfoToPrint : rawTag.TradeMarkInfoToPrint;

                rackTagE1Mfg.Dimensions = !string.IsNullOrEmpty(rackTagE1Mfg.Dimensions) ? rackTagE1Mfg.Dimensions : rawTag.Dimensions;
                rackTagE1Mfg.DimensionsMetric = !string.IsNullOrEmpty(rackTagE1Mfg.DimensionsMetric) ? rackTagE1Mfg.DimensionsMetric : rawTag.DimensionsMetric;
                rackTagE1Mfg.Quantity = (rackTagE1Mfg.Quantity > 0) ? rackTagE1Mfg.Quantity : rawTag.Quantity;
                rackTagE1Mfg.SqftActual = (rackTagE1Mfg.SqftActual > 0) ? rackTagE1Mfg.SqftActual : rawTag.SqftActual;
                rackTagE1Mfg.WeightLbs = rawTag.WeightLbs; //(rackTagE1Mfg.WeightLbs > 0) ? rackTagE1Mfg.WeightLbs : rawTag.WeightLbs;
                rackTagE1Mfg.WorkCenter = !string.IsNullOrEmpty(rackTagE1Mfg.WorkCenter) ? rackTagE1Mfg.WorkCenter : rawTag.WorkCenter;
                //rackTagE1Mfg.ProducedDateTime = rawTag.ProducedDateTime;
                //rackTagE1Mfg.TotalWeightLbs = rawTag.TotalWeightLbs; //(rackTagE1Mfg.TotalWeightLbs > 0) ? rackTagE1Mfg.TotalWeightLbs : rawTag.TotalWeightLbs;
                rackTagE1Mfg.Message1 = !string.IsNullOrEmpty(rackTagE1Mfg.Message1) ? rackTagE1Mfg.Message1 : rawTag.Message1;
                rackTagE1Mfg.Message2 = !string.IsNullOrEmpty(rackTagE1Mfg.Message2) ? rackTagE1Mfg.Message2 : rawTag.Message2;
                rackTagE1Mfg.SalesMsg = !string.IsNullOrEmpty(rackTagE1Mfg.SalesMsg) ? rackTagE1Mfg.SalesMsg : rawTag.SalesMsg;
                rackTagE1Mfg.SalesMsg2 = !string.IsNullOrEmpty(rackTagE1Mfg.SalesMsg2) ? rackTagE1Mfg.SalesMsg2 : rawTag.SalesMsg2;
                rackTagE1Mfg.Lite1_1stSrfCoatKey = rawTag.Lite1_1stSrfCoatKey; //!string.IsNullOrEmpty(rackTagE1Mfg.Lite1_1stSrfCoatKey) ? rackTagE1Mfg.Lite1_1stSrfCoatKey : rawTag.Lite1_1stSrfCoatKey;
                rackTagE1Mfg.Lite1_2ndSrfCoatKey = rawTag.Lite1_2ndSrfCoatKey; //!string.IsNullOrEmpty(rackTagE1Mfg.Lite1_2ndSrfCoatKey) ? rackTagE1Mfg.Lite1_2ndSrfCoatKey : rawTag.Lite1_2ndSrfCoatKey;
                rackTagE1Mfg.WOrderNum = (rackTagE1Mfg.WOrderNum > 0) ? rackTagE1Mfg.WOrderNum : rawTag.WOrderNum;
                //rackTagE1Mfg.CustPONum = !string.IsNullOrEmpty(rackTagE1Mfg.CustPONum) ? rackTagE1Mfg.CustPONum : rackTagE1Mfg.CustPONum;
                rackTagE1Mfg.CustPOLine = !string.IsNullOrEmpty(rackTagE1Mfg.CustPOLine) ? rackTagE1Mfg.CustPOLine : rawTag.CustPOLine;
                rackTagE1Mfg.CustVendorID = !string.IsNullOrEmpty(rackTagE1Mfg.CustVendorID) ? rackTagE1Mfg.CustVendorID : rawTag.CustVendorID;
                rackTagE1Mfg.CustPartNum = !string.IsNullOrEmpty(rackTagE1Mfg.CustPartNum) ? rackTagE1Mfg.CustPartNum : rawTag.CustPartNum;
                rackTagE1Mfg.BaseFractional = !string.IsNullOrEmpty(rackTagE1Mfg.BaseFractional) ? rackTagE1Mfg.BaseFractional : rawTag.BaseFractional;
                rackTagE1Mfg.LeftFractional = !string.IsNullOrEmpty(rackTagE1Mfg.LeftFractional) ? rackTagE1Mfg.LeftFractional : rawTag.LeftFractional;
                rackTagE1Mfg.RightFractional = !string.IsNullOrEmpty(rackTagE1Mfg.RightFractional) ? rackTagE1Mfg.RightFractional : rawTag.RightFractional;
                rackTagE1Mfg.TopFractional = !string.IsNullOrEmpty(rackTagE1Mfg.TopFractional) ? rackTagE1Mfg.TopFractional : rawTag.TopFractional;
                rackTagE1Mfg.BaseMM = (rackTagE1Mfg.BaseMM > 0) ? rackTagE1Mfg.BaseMM : rawTag.BaseMM;
                rackTagE1Mfg.LeftMM = (rackTagE1Mfg.LeftMM > 0) ? rackTagE1Mfg.LeftMM : rawTag.LeftMM;
                rackTagE1Mfg.RightMM = (rackTagE1Mfg.RightMM > 0) ? rackTagE1Mfg.RightMM : rawTag.RightMM;
                rackTagE1Mfg.TopMM = (rackTagE1Mfg.TopMM > 0) ? rackTagE1Mfg.TopMM : rawTag.TopMM;
                //rackTagE1Mfg.ProducedDate = rawTag.ProducedDate;
                //rackTagE1Mfg.ProducedTime = rawTag.ProducedTime;
                rackTagE1Mfg.BaseDec = (rackTagE1Mfg.BaseDec > 0) ? rackTagE1Mfg.BaseDec : rawTag.BaseDec;
                rackTagE1Mfg.LeftDec = (rackTagE1Mfg.LeftDec > 0) ? rackTagE1Mfg.LeftDec : rawTag.LeftDec;
                rackTagE1Mfg.DueDate = rawTag.DueDate;
                //rackTagE1Mfg.LineID = !string.IsNullOrEmpty(rackTagE1Mfg.LineID) ? rackTagE1Mfg.LineID : rawTag.LineID; //rawTag.LineID;
                //rackTagE1Mfg.Crew = !string.IsNullOrEmpty(rackTagE1Mfg.Crew) ? rackTagE1Mfg.Crew : rawTag.Crew; //rawTag.Crew;
                rackTagE1Mfg.CustPrtNum2 = !string.IsNullOrEmpty(rackTagE1Mfg.CustPrtNum2) ? rackTagE1Mfg.CustPrtNum2 : rawTag.CustPrtNum2; 
                rackTagE1Mfg.CustPrtNum3 = !string.IsNullOrEmpty(rackTagE1Mfg.CustPrtNum3) ? rackTagE1Mfg.CustPrtNum3 : rawTag.CustPrtNum3; 
                rackTagE1Mfg.CustPrtNum4 = !string.IsNullOrEmpty(rackTagE1Mfg.CustPrtNum4) ? rackTagE1Mfg.CustPrtNum4 : rawTag.CustPrtNum4; 
                rackTagE1Mfg.User = rawTag.User;
                rackTagE1Mfg.CurrentDate = rawTag.CurrentDate;
                rackTagE1Mfg.CurrentTime = rawTag.CurrentTime;
                rackTagE1Mfg.SOrderNum = (rackTagE1Mfg.SOrderNum > 0) ? rackTagE1Mfg.SOrderNum : rawTag.SOrderNum; 
                rackTagE1Mfg.SOrderLine = (rackTagE1Mfg.SOrderLine > 0) ? rackTagE1Mfg.SOrderLine : rawTag.SOrderLine; 
                rackTagE1Mfg.SqftBillable = (rackTagE1Mfg.SqftBillable > 0) ? rackTagE1Mfg.SqftBillable : rawTag.SqftBillable;
                //rackTagE1Mfg.ProducedTimeBarcode = !string.IsNullOrEmpty(rackTagE1Mfg.ProducedTimeBarcode) ? rackTagE1Mfg.ProducedTimeBarcode : rawTag.ProducedTimeBarcode;
                rackTagE1Mfg.EdgeDelete = !string.IsNullOrEmpty(rackTagE1Mfg.EdgeDelete) ? rackTagE1Mfg.EdgeDelete : rawTag.EdgeDelete;
                rackTagE1Mfg.Lite1_ColorShortDesc = rawTag.Lite1_ColorShortDesc; //!string.IsNullOrEmpty(rackTagE1Mfg.Lite1_ColorShortDesc) ? rackTagE1Mfg.Lite1_ColorShortDesc : rawTag.Lite1_ColorShortDesc;
                rackTagE1Mfg.Lite1_PatternShortDesc = rawTag.Lite1_PatternShortDesc; //!string.IsNullOrEmpty(rackTagE1Mfg.Lite1_PatternShortDesc) ? rackTagE1Mfg.Lite1_PatternShortDesc : rawTag.Lite1_PatternShortDesc;
                rackTagE1Mfg.Lite1_LamiShortDesc = rawTag.Lite1_LamiShortDesc; //!string.IsNullOrEmpty(rackTagE1Mfg.Lite1_LamiShortDesc) ? rackTagE1Mfg.Lite1_LamiShortDesc : rawTag.Lite1_LamiShortDesc;
                rackTagE1Mfg.RackType = !string.IsNullOrEmpty(rackTagE1Mfg.RackType) ? rackTagE1Mfg.RackType : rawTag.RackType;
                rackTagE1Mfg.OriginalCustomer = !string.IsNullOrEmpty(rackTagE1Mfg.OriginalCustomer) ? rackTagE1Mfg.OriginalCustomer : rawTag.OriginalCustomer;
                rackTagE1Mfg.AnsiiLogoType = !string.IsNullOrEmpty(rackTagE1Mfg.AnsiiLogoType) ? rackTagE1Mfg.AnsiiLogoType : rawTag.AnsiiLogoType;
                rackTagE1Mfg.PlantID = !string.IsNullOrEmpty(rackTagE1Mfg.PlantID) ? rackTagE1Mfg.PlantID : rawTag.PlantID;
                rackTagE1Mfg.AssetID = !string.IsNullOrEmpty(rackTagE1Mfg.AssetID) ? rackTagE1Mfg.AssetID : rawTag.AssetID;
                rackTagE1Mfg.FreeLiteQuantity = rawTag.FreeLiteQuantity;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(e.Message + ": " + e.StackTrace);
            }
            return rackTagE1Mfg;
        }

        public HVRackTagItem AddRackTagSFMfg(HVRackTagItem rackTagE1Mfg, HVRackTagItem rackTagSFMfg)
        {
            try
            {
                rackTagE1Mfg.Customer = !string.IsNullOrEmpty(rackTagE1Mfg.Customer) ? rackTagE1Mfg.Customer : rackTagSFMfg.Customer;
                rackTagE1Mfg.PackingTinInTinOut = !string.IsNullOrEmpty(rackTagE1Mfg.PackingTinInTinOut) ? rackTagE1Mfg.PackingTinInTinOut : rackTagSFMfg.PackingTinInTinOut;

                //rackTagE1Mfg.E1ItemNumber = !string.IsNullOrEmpty(rackTagE1Mfg.E1ItemNumber) ? rackTagE1Mfg.E1ItemNumber : rackTagSFMfg.E1ItemNumber;
                rackTagE1Mfg.OriginalE1ItemNumber = rackTagSFMfg.E1ItemNumber;

                rackTagE1Mfg.GlassThickness = (rackTagE1Mfg.GlassThickness > 0) ? rackTagE1Mfg.GlassThickness : rackTagSFMfg.GlassThickness;
                rackTagE1Mfg.GlassHeatTreatment = !string.IsNullOrEmpty(rackTagE1Mfg.GlassHeatTreatment) ? rackTagE1Mfg.GlassHeatTreatment : rackTagSFMfg.GlassHeatTreatment;
                rackTagE1Mfg.FirstSurfaceCoat = !string.IsNullOrEmpty(rackTagE1Mfg.FirstSurfaceCoat) ? rackTagE1Mfg.FirstSurfaceCoat : rackTagSFMfg.FirstSurfaceCoat;
                rackTagE1Mfg.SecondSurfaceCoat = !string.IsNullOrEmpty(rackTagE1Mfg.SecondSurfaceCoat) ? rackTagE1Mfg.SecondSurfaceCoat : rackTagSFMfg.SecondSurfaceCoat;
                rackTagE1Mfg.SecondSurfaceCoatDesc1 = !string.IsNullOrEmpty(rackTagE1Mfg.SecondSurfaceCoatDesc1) ? rackTagE1Mfg.SecondSurfaceCoatDesc1 : rackTagSFMfg.SecondSurfaceCoatDesc1;
                rackTagE1Mfg.TradeMarkInfoToPrint = !string.IsNullOrEmpty(rackTagE1Mfg.TradeMarkInfoToPrint) ? rackTagE1Mfg.TradeMarkInfoToPrint : rackTagSFMfg.TradeMarkInfoToPrint;

                rackTagE1Mfg.Dimensions = !string.IsNullOrEmpty(rackTagE1Mfg.Dimensions) ? rackTagE1Mfg.Dimensions : rackTagSFMfg.Dimensions;
                rackTagE1Mfg.DimensionsMetric = !string.IsNullOrEmpty(rackTagE1Mfg.DimensionsMetric) ? rackTagE1Mfg.DimensionsMetric : rackTagSFMfg.DimensionsMetric;
                //rackTagE1Mfg.Quantity = (rackTagE1Mfg.Quantity > 0) ? rackTagE1Mfg.Quantity : rackTagSFMfg.Quantity;
                rackTagE1Mfg.SqftActual = (rackTagE1Mfg.SqftActual > 0) ? rackTagE1Mfg.SqftActual : rackTagSFMfg.SqftActual;
                //rackTagE1Mfg.WeightLbs = (rackTagE1Mfg.WeightLbs > 0) ? rackTagE1Mfg.WeightLbs : rackTagSFMfg.WeightLbs;
                rackTagE1Mfg.WorkCenter = !string.IsNullOrEmpty(rackTagE1Mfg.WorkCenter) ? rackTagE1Mfg.WorkCenter : rackTagSFMfg.WorkCenter;
                rackTagE1Mfg.ProducedDateTime = rackTagSFMfg.ProducedDateTime;
                //rackTagE1Mfg.TotalWeightLbs = (rackTagE1Mfg.TotalWeightLbs > 0) ? rackTagE1Mfg.TotalWeightLbs : rackTagSFMfg.TotalWeightLbs;
                rackTagE1Mfg.Message1 = !string.IsNullOrEmpty(rackTagE1Mfg.Message1) ? rackTagE1Mfg.Message1 : rackTagSFMfg.Message1;
                rackTagE1Mfg.Message2 = !string.IsNullOrEmpty(rackTagE1Mfg.Message2) ? rackTagE1Mfg.Message2 : rackTagSFMfg.Message2;
                rackTagE1Mfg.SalesMsg = !string.IsNullOrEmpty(rackTagE1Mfg.SalesMsg) ? rackTagE1Mfg.SalesMsg : rackTagSFMfg.SalesMsg;
                rackTagE1Mfg.SalesMsg2 = !string.IsNullOrEmpty(rackTagE1Mfg.SalesMsg2) ? rackTagE1Mfg.SalesMsg2 : rackTagSFMfg.SalesMsg2;
                rackTagE1Mfg.Lite1_1stSrfCoatKey = !string.IsNullOrEmpty(rackTagE1Mfg.Lite1_1stSrfCoatKey) ? rackTagE1Mfg.Lite1_1stSrfCoatKey : rackTagSFMfg.Lite1_1stSrfCoatKey;
                rackTagE1Mfg.Lite1_2ndSrfCoatKey = !string.IsNullOrEmpty(rackTagE1Mfg.Lite1_2ndSrfCoatKey) ? rackTagE1Mfg.Lite1_2ndSrfCoatKey : rackTagSFMfg.Lite1_2ndSrfCoatKey;
                rackTagE1Mfg.WOrderNum = (rackTagE1Mfg.WOrderNum > 0) ? rackTagE1Mfg.WOrderNum : rackTagSFMfg.WOrderNum;
                rackTagE1Mfg.CustPONum = !string.IsNullOrEmpty(rackTagE1Mfg.CustPONum) ? rackTagE1Mfg.CustPONum : rackTagSFMfg.CustPONum;
                rackTagE1Mfg.CustPOLine = !string.IsNullOrEmpty(rackTagE1Mfg.CustPOLine) ? rackTagE1Mfg.CustPOLine : rackTagSFMfg.CustPOLine;
                rackTagE1Mfg.CustVendorID = !string.IsNullOrEmpty(rackTagE1Mfg.CustVendorID) ? rackTagE1Mfg.CustVendorID : rackTagSFMfg.CustVendorID;
                rackTagE1Mfg.CustPartNum = !string.IsNullOrEmpty(rackTagE1Mfg.CustPartNum) ? rackTagE1Mfg.CustPartNum : rackTagSFMfg.CustPartNum;
                rackTagE1Mfg.BaseFractional = !string.IsNullOrEmpty(rackTagE1Mfg.BaseFractional) ? rackTagE1Mfg.BaseFractional : rackTagSFMfg.BaseFractional;
                rackTagE1Mfg.LeftFractional = !string.IsNullOrEmpty(rackTagE1Mfg.LeftFractional) ? rackTagE1Mfg.LeftFractional : rackTagSFMfg.LeftFractional;
                rackTagE1Mfg.RightFractional = !string.IsNullOrEmpty(rackTagE1Mfg.RightFractional) ? rackTagE1Mfg.RightFractional : rackTagSFMfg.RightFractional;
                rackTagE1Mfg.TopFractional = !string.IsNullOrEmpty(rackTagE1Mfg.TopFractional) ? rackTagE1Mfg.TopFractional : rackTagSFMfg.TopFractional;
                rackTagE1Mfg.BaseMM = (rackTagE1Mfg.BaseMM > 0) ? rackTagE1Mfg.BaseMM : rackTagSFMfg.BaseMM;
                rackTagE1Mfg.LeftMM = (rackTagE1Mfg.LeftMM > 0) ? rackTagE1Mfg.LeftMM : rackTagSFMfg.LeftMM;
                rackTagE1Mfg.RightMM = (rackTagE1Mfg.RightMM > 0) ? rackTagE1Mfg.RightMM : rackTagSFMfg.RightMM;
                rackTagE1Mfg.TopMM = (rackTagE1Mfg.TopMM > 0) ? rackTagE1Mfg.TopMM : rackTagSFMfg.TopMM;
                rackTagE1Mfg.ProducedDate = string.IsNullOrEmpty(rackTagSFMfg.ProducedDate) ? rackTagE1Mfg.ProducedDate : rackTagSFMfg.ProducedDate; //rackTagSFMfg.ProducedDate;
                rackTagE1Mfg.ProducedTime = rackTagSFMfg.ProducedTime;
                rackTagE1Mfg.BaseDec = (rackTagE1Mfg.BaseDec > 0) ? rackTagE1Mfg.BaseDec : rackTagSFMfg.BaseDec;
                rackTagE1Mfg.LeftDec = (rackTagE1Mfg.LeftDec > 0) ? rackTagE1Mfg.LeftDec : rackTagSFMfg.LeftDec;
                rackTagE1Mfg.DueDate = rackTagSFMfg.DueDate;
                rackTagE1Mfg.LineID = rackTagSFMfg.LineID;
                rackTagE1Mfg.Crew = rackTagSFMfg.Crew;
                rackTagE1Mfg.CustPrtNum2 = !string.IsNullOrEmpty(rackTagE1Mfg.CustPrtNum2) ? rackTagE1Mfg.CustPrtNum2 : rackTagSFMfg.CustPrtNum2;  //rackTagSFMfg.CustPrtNum2;
                rackTagE1Mfg.CustPrtNum3 = !string.IsNullOrEmpty(rackTagE1Mfg.CustPrtNum3) ? rackTagE1Mfg.CustPrtNum3 : rackTagSFMfg.CustPrtNum3;  //rackTagSFMfg.CustPrtNum3;
                rackTagE1Mfg.CustPrtNum4 = !string.IsNullOrEmpty(rackTagE1Mfg.CustPrtNum4) ? rackTagE1Mfg.CustPrtNum4 : rackTagSFMfg.CustPrtNum4;  //rackTagSFMfg.CustPrtNum4;
                rackTagE1Mfg.User = rackTagSFMfg.User;
                rackTagE1Mfg.CurrentDate = rackTagSFMfg.CurrentDate;
                rackTagE1Mfg.CurrentTime = rackTagSFMfg.CurrentTime;
                //rackTagE1Mfg.SOrderNum = rackTagSFMfg.SOrderNum;
                //rackTagE1Mfg.SOrderLine = rackTagSFMfg.SOrderLine;
                rackTagE1Mfg.SOrderNum = (rackTagSFMfg.SOrderNum > 0) ? rackTagSFMfg.SOrderNum : rackTagE1Mfg.SOrderNum;
                rackTagE1Mfg.SOrderLine = (rackTagSFMfg.SOrderLine > 0) ? rackTagSFMfg.SOrderLine : rackTagE1Mfg.SOrderLine;
                rackTagE1Mfg.SqftBillable = (rackTagE1Mfg.SqftBillable > 0) ? rackTagE1Mfg.SqftBillable : rackTagSFMfg.SqftBillable;
                rackTagE1Mfg.ProducedTimeBarcode = string.IsNullOrEmpty(rackTagSFMfg.ProducedTimeBarcode) ? rackTagE1Mfg.ProducedTimeBarcode : rackTagSFMfg.ProducedTimeBarcode;
                rackTagE1Mfg.EdgeDelete = !string.IsNullOrEmpty(rackTagE1Mfg.EdgeDelete) ? rackTagE1Mfg.EdgeDelete : rackTagSFMfg.EdgeDelete;
                //rackTagE1Mfg.Lite1_ColorShortDesc = !string.IsNullOrEmpty(rackTagE1Mfg.Lite1_ColorShortDesc) ? rackTagE1Mfg.Lite1_ColorShortDesc : rackTagSFMfg.Lite1_ColorShortDesc;
                rackTagE1Mfg.Lite1_PatternShortDesc = !string.IsNullOrEmpty(rackTagE1Mfg.Lite1_PatternShortDesc) ? rackTagE1Mfg.Lite1_PatternShortDesc : rackTagSFMfg.Lite1_PatternShortDesc;
                rackTagE1Mfg.Lite1_LamiShortDesc = !string.IsNullOrEmpty(rackTagE1Mfg.Lite1_LamiShortDesc) ? rackTagE1Mfg.Lite1_LamiShortDesc : rackTagSFMfg.Lite1_LamiShortDesc;
                rackTagE1Mfg.RackType = !string.IsNullOrEmpty(rackTagE1Mfg.RackType) ? rackTagE1Mfg.RackType : rackTagSFMfg.RackType;
                rackTagE1Mfg.OriginalCustomer = !string.IsNullOrEmpty(rackTagE1Mfg.OriginalCustomer) ? rackTagE1Mfg.OriginalCustomer : rackTagSFMfg.OriginalCustomer;
                rackTagE1Mfg.AnsiiLogoType = !string.IsNullOrEmpty(rackTagE1Mfg.AnsiiLogoType) ? rackTagE1Mfg.AnsiiLogoType : rackTagSFMfg.AnsiiLogoType;
                rackTagE1Mfg.AssetID = !string.IsNullOrEmpty(rackTagE1Mfg.AssetID) ? rackTagE1Mfg.AssetID : rackTagSFMfg.AssetID;
                rackTagE1Mfg.Shift = !string.IsNullOrEmpty(rackTagE1Mfg.Shift) ? rackTagE1Mfg.Shift : rackTagSFMfg.Shift;
                rackTagE1Mfg.GlassHandler = !string.IsNullOrEmpty(rackTagE1Mfg.GlassHandler) ? rackTagE1Mfg.GlassHandler : rackTagSFMfg.GlassHandler;
                rackTagE1Mfg.GlassHandler2 = !string.IsNullOrEmpty(rackTagE1Mfg.GlassHandler2) ? rackTagE1Mfg.GlassHandler2 : rackTagSFMfg.GlassHandler2;
                rackTagE1Mfg.GlassHandler3 = !string.IsNullOrEmpty(rackTagE1Mfg.GlassHandler3) ? rackTagE1Mfg.GlassHandler3 : rackTagSFMfg.GlassHandler3;
                rackTagE1Mfg.GlassHandler4 = !string.IsNullOrEmpty(rackTagE1Mfg.GlassHandler4) ? rackTagE1Mfg.GlassHandler4 : rackTagSFMfg.GlassHandler4;
                rackTagE1Mfg.LitesPerInch = (rackTagE1Mfg.LitesPerInch > 0) ? rackTagE1Mfg.LitesPerInch : rackTagSFMfg.LitesPerInch;
                rackTagE1Mfg.CalculatedLitesPerInch = !string.IsNullOrEmpty(rackTagE1Mfg.CalculatedLitesPerInch) ? rackTagE1Mfg.CalculatedLitesPerInch : rackTagSFMfg.CalculatedLitesPerInch;
                rackTagE1Mfg.Target = rackTagSFMfg.Target;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(e.Message + ": " + e.StackTrace);
            }
            return rackTagE1Mfg;
        }
        public SqlCommand GetFinishedGoods()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT [ticket]
                          ,[ticket_char]
                          ,[fg_ticket]
                          ,[customer]
                          ,[po]
                          ,[line]
                          ,[status]
                          ,[marking]
                          ,[sizex]
                          ,[sizey]
                          ,[schedule]
                          ,[job]
                          ,CONVERT(CHAR(10), [timeProduced], 23) AS dateProducedString
                          ,CONVERT(CHAR(8), [timeProduced], 8) AS timeProducedString
                          ,[timeProduced]
                          ,[timeShipped]
                          ,[timeScheduled]
                          ,CAST(ISNULL([thickness], 0) AS NUMERIC(15,8)) AS thickness
                          ,[treatment]
                          ,[legacyShift]
                          ,[legacyCrew]
                          ,[rackType]
                          ,[rackCode]
                          ,CAST(ISNULL([productionLites1], 0) AS NUMERIC(15,2)) AS productionLites1
                          ,CAST(ISNULL([productionLites2], 0) AS NUMERIC(15,2)) AS productionLites2
                          ,CAST(ISNULL([productionLitesTotal], 0) AS NUMERIC(15,2)) AS productionLitesTotal
                          ,[splitFraction1]
                          ,[splitFraction2]
                          ,CAST(ISNULL([tons], 0) AS NUMERIC(15,8)) AS tons
                          ,CAST(ISNULL([sqft], 0) AS NUMERIC(15,8)) AS sqft
                          ,CAST(ISNULL([litesPerRack], 0) AS NUMERIC(15,2)) AS litesPerRack
                          ,[area]
                          ,[whzone]
                          ,[whrow]
                          ,[shippingNumber]
                          ,[plant]
                          ,[plant_char]
                          ,[myKey]
                          ,CAST(ISNULL([racks], 0) AS NUMERIC(15,2)) AS racks
                          ,[productionDate1]
                          ,[productionDate2]
                          ,CAST(ISNULL([productionShift1], 0) AS VARCHAR(10)) AS productionShift1
                          ,CAST(ISNULL([productionShift2], 0) AS VARCHAR(10)) AS productionShift2
                          ,[productionCrew1]
                          ,[productionCrew2]
                          ,[releaseStatus]
                          ,[solarShippedStatus]
                          ,[rack]
                          ,CAST(ISNULL([rackID], 0) AS NUMERIC(15,2)) AS rackID
                          ,[packLocation]
                          ,[inspectedBy]
                          ,[closingShift]
                          ,[glassWeight]
                          ,[grossWeight]
                          ,[E1Part]
                          ,CAST(ISNULL([targetLites], 0) AS NUMERIC(15,2)) AS targetLites
                          ,[targetRackType]
                          ,[overPack]
                          ,[underPack]
                          ,[thicknessSpec]
                          ,[packStart]
                          ,[packEnd]
                          ,[packedBy]
                          ,[inspInitials]
                          ,[processingStatus]
                          ,[ticketStyle]
                          ,[customerPart]
                          ,[closingComments]
                          ,[longItem]
                          ,CAST(ISNULL([workOrder], 0) AS NUMERIC(15,2)) AS workOrder
                          ,[workOrderType]
                          ,[ticketComment1]
                          ,[ticketComment2]
                          ,[holdCode]
                          ,[customerName]
                          ,[ticketFile]
                          ,[sourceTicket1]
                          ,[sourceTicket2]
                          ,[sourceTicket3]
                          ,[curPlant]
                          ,[curStatus]
                          ,CAST(ISNULL([curLites], 0) AS NUMERIC(15,2)) AS curLites
                          ,[consumedTime]
                          ,[consumedDate1]
                          ,[consumedDate2]
                          ,CAST(ISNULL([consumedShift1], 0) AS NUMERIC(15,2)) AS consumedShift1
                          ,CAST(ISNULL([consumedShift2], 0) AS NUMERIC(15,2)) AS consumedShift2
                          ,[consumedCrew1]
                          ,[consumedCrew2]
                          ,CAST(ISNULL([consumedLites1], 0) AS NUMERIC(15,2)) AS consumedLites1
                          ,CAST(ISNULL([consumedLites2], 0) AS NUMERIC(15,2)) AS consumedLites2
                          ,CAST(ISNULL([consumedArea], 0) AS NUMERIC(15,2)) AS consumedArea
                          ,CAST(ISNULL([consumedLine], 0) AS NUMERIC(15,2)) AS consumedLine
                          ,[whLoc]
                          ,[rowguid]
                          ,[lastUpdate]
                          ,[upload]
                          ,[transferred]
                          ,[transferredDateTime]
                          ,[comment]
                          ,[logo]
                          ,CAST(ISNULL([measurementInches], 0) AS NUMERIC(15,8)) AS measurementInches
                          ,CAST(ISNULL([consumedWorkOrder1], 0) AS NUMERIC(15,2)) AS consumedWorkOrder1
                          ,CAST(ISNULL([consumedWorkOrder2], 0) AS NUMERIC(15,2)) AS consumedWorkOrder2
                          ,[poReferenceNumber]
                          ,[taggedBy]
                          ,CAST(ISNULL([tonsPerLite], 0) AS NUMERIC(15,8)) AS tonsPerLite
                          ,[consumedArea1]
                          ,[consumedArea2]
                          ,CAST(ISNULL([plannerNumber], 0) AS NUMERIC(15,2)) AS plannerNumber
                          ,CAST(ISNULL([salesOrder], 0) AS NUMERIC(15,2)) AS salesOrder
                          ,[salesOrderType]
                          ,[salesOrderLine]
                          ,CAST(ISNULL([consumedLine2], 0) AS NUMERIC(15,2)) AS consumedLine2
                          ,[coating1]
                          ,[coating2]
                          ,[pattern1]
                          ,[pattern2]
                          ,[color]
                          ,[drawing]
                          ,CAST(ISNULL([transmittance], 0) AS NUMERIC(15,2)) AS transmittance
                          ,[longCustomerPO]
                          ,CAST(ISNULL([partialReason], 0) AS NUMERIC(15,2)) AS partialReason
                          ,CAST(ISNULL([voidReason], 0) AS NUMERIC(15,2)) AS voidReason
                          ,[voidReason]
                          ,[voidTime]
                          ,[e1ThicknessName]
                          ,[J_ETL_DATE]
                          ,CONVERT(CHAR(10), GetDate(), 23) AS currentDateString
                          ,CONVERT(CHAR(8), GetDate(), 8) AS currentTimeString
                      FROM [PSBI_WH_PROD].[PROD].[PS_JWS_FG_FINISHED_GOODS]
                      WHERE [fg_ticket] = @SerialNum
                    "
            };
            cmd.Parameters.Add(new SqlParameter("@SerialNum", SqlDbType.VarChar, 20));
            return cmd;
        }

        public HVRackTagItem GetFinishedGoods(BusinessSystemSession e1Session, SqlConnection cnDW, string serialNum, decimal rackId)
        {
            var rackTagFinishedGoods = new HVRackTagItem();
            try
            {
                var cmd = GetFinishedGoods();
                cmd.Parameters["@SerialNum"].Value = serialNum;
                cmd.Connection = cnDW;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new HVRackTagItem();
                        rackTag.Customer = CastDbNull.To(reader["customerName"], string.Empty).Trim();
                        rackTag.PackingTinInTinOut = "";
                        rackTag.E1ItemNumber = CastDbNull.To(reader["longItem"], string.Empty).Trim();
                        rackTag.GlassThickness = CastDbNull.To<decimal>(reader["thickness"], 0);
                        rackTag.GlassHeatTreatment = CastDbNull.To(reader["treatment"], string.Empty).Trim();
                        rackTag.FirstSurfaceCoat = "";
                        rackTag.SecondSurfaceCoat = "";
                        rackTag.Dimensions = "";
                        rackTag.DimensionsMetric = "";
                        rackTag.Quantity = CastDbNull.To<decimal>(reader["productionLitesTotal"], 0);
                        rackTag.SqftActual = CastDbNull.To<decimal>(reader["sqft"], 0);
                        rackTag.WeightLbs = CastDbNull.To<decimal>(reader["glassWeight"], 0);
                        rackTag.WorkCenter = CastDbNull.To(reader["area"], string.Empty).Trim();
                        rackTag.ProducedDateTime = CastDbNull.To(reader["timeProduced"], new DateTime(1900, 1, 1));
                        rackTag.TotalWeightLbs = CastDbNull.To<decimal>(reader["grossWeight"], 0);
                        rackTag.Message1 = CastDbNull.To(reader["closingComments"], string.Empty).Trim();
                        rackTag.Message2 = "";
                        rackTag.SalesMsg = "";
                        rackTag.SalesMsg2 = "";
                        rackTag.Lite1_1stSrfCoatKey = "";
                        rackTag.Lite1_2ndSrfCoatKey = "";
                        rackTag.WOrderNum = CastDbNull.To<decimal>(reader["workOrder"], 0);
                        rackTag.CustPONum = "";
                        rackTag.CustPOLine = "";
                        rackTag.CustVendorID = "";
                        rackTag.CustPartNum = "";
                        rackTag.BaseFractional = "";
                        rackTag.LeftFractional = "";
                        rackTag.RightFractional = "";
                        rackTag.TopFractional = "";
                        rackTag.BaseMM = 0;
                        rackTag.LeftMM = 0;
                        rackTag.RightMM = 0;
                        rackTag.TopMM = 0;
                        rackTag.ProducedDate = CastDbNull.To(reader["dateProducedString"], string.Empty).Trim();
                        rackTag.ProducedTime = CastDbNull.To(reader["timeProducedString"], string.Empty).Trim();
                        rackTag.BaseDec = 0;
                        rackTag.LeftDec = 0;
                        rackTag.DueDate = "";
                        rackTag.LineID = CastDbNull.To(reader["area"], string.Empty).Trim();
                        rackTag.Crew = CastDbNull.To(reader["productionCrew1"], string.Empty).Trim();
                        rackTag.CustPrtNum2 = "";
                        rackTag.CustPrtNum3 = "";
                        rackTag.CustPrtNum4 = "";
                        rackTag.User = "";
                        rackTag.CurrentDate = CastDbNull.To(reader["currentDateString"], string.Empty).Trim();
                        rackTag.CurrentTime = CastDbNull.To(reader["currentTimeString"], string.Empty).Trim();
                        rackTag.SOrderNum = CastDbNull.To<decimal>(reader["salesOrder"], 0);
                        rackTag.SOrderLine = CastDbNull.To<decimal>(reader["salesOrderLine"], 0);
                        rackTag.SqftBillable = CastDbNull.To<decimal>(reader["sqft"], 0);
                        rackTag.ProducedTimeBarcode = rackTag.ProducedDate + " " + rackTag.ProducedTime;
                        rackTag.EdgeDelete = "";
                        rackTag.Lite1_ColorShortDesc = "";
                        rackTag.Lite1_PatternShortDesc = "";
                        rackTag.Lite1_LamiShortDesc = "";
                        rackTag.RackType = CastDbNull.To(reader["rackType"], string.Empty).Trim();
                        rackTag.FirmSchedule = 0;
                        rackTag.AnsiiLogoType = CastDbNull.To(reader["logo"], string.Empty).Trim();
                        var plannerNumber = CastDbNull.To<decimal>(reader["plannerNumber"], 0);
                        rackTag.OriginalCustomer = CustomerManager.GetCustomerByAddressNum(e1Session, plannerNumber);
                        rackTag.AssetID = "";
                        rackTag.PlantID = "";
                        rackTag.Shift = CastDbNull.To(reader["productionShift1"], string.Empty).Trim();
                        rackTag.GlassHandler = CastDbNull.To(reader["packedBy"], string.Empty).Trim();
                        rackTag.GlassHandler2 = CastDbNull.To(reader["inspInitials"], string.Empty).Trim();
                        rackTag.GlassHandler3 = "";
                        rackTag.GlassHandler4 = "";
                        rackTagFinishedGoods = rackTag;
                        Logger.Trace($"Rack Id [{rackTagFinishedGoods.RackId}] 1 row found");
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnDW.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cnDW.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return rackTagFinishedGoods;
        }

        public SqlCommand GetRawTag()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT CTE.RawTagsID
                         , 0 AS HVQueID
                         , ISNULL(CTE.SerialNum, '') AS SerialNumber
                         , '' AS Customer
                         , '' AS PackingTinInTinOut
                         , ISNULL(CTE.ItemNum, '') AS E1ItemNumber
                         , ISNULL(CTE.Lite1_Thick, 0) AS GlassThickness
                         , ISNULL(CTE.Lite1_HeatTreatment, '') AS GlassHeatTreatment
                         , ISNULL(CTE.Lite1_1stSrfCoat, '') AS FirstSurfaceCoat
                         , ISNULL(CTE.Lite1_2ndSrfCoat, '') AS SecondSurfaceCoat
                        ,ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', CTE.Lite1_2ndSrfCoat),'') AS SecondSurfaceCoatDesc1
                        ,CASE WHEN LEFT(CTE.Lite1_2ndSrfCoat,2) = 'CS' THEN
						LEFT(ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', CTE.Lite1_2ndSrfCoat),''),CHARINDEX(' ',ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', CTE.Lite1_2ndSrfCoat),''),9)) + CHAR(0174) + ' ' + SUBSTRING(ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', CTE.Lite1_2ndSrfCoat),''),CHARINDEX(' ',ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', CTE.Lite1_2ndSrfCoat),''),9)+1,2)
						ELSE '' END AS TradeMarkInfoToPrint
                         , ISNULL(CTE.Dimensions, '') AS Dimensions
                         , ISNULL(CAST(CASE WHEN ISNULL(CTE.LeftMM, 0) = 0 THEN CTE.RightMM
                                            ELSE CTE.LeftMM
                                       END AS VARCHAR(10)) + ' X ' + CAST(CASE WHEN ISNULL(CTE.BaseMM, 0) = 0 THEN CTE.TopMM
                                                                               ELSE CTE.BaseMM
                                                                          END AS VARCHAR(10))
                                , '') AS DimensionsMetric
                         , ISNULL(CAST(CTE.[QtyOnHand] AS NUMERIC(15, 2)), 0) AS Quantity
                         , ISNULL(CAST(CTE.SqftActual AS NUMERIC(15, 2)), 0) AS SqftActual
                         , ISNULL(CAST(GlassWeight.Pounds AS NUMERIC(38, 2)), 0) AS WeightLbs
                         , '' AS WorkCenter
                         , CTE.[ProductionDate] AS ProducedDateTime
                         , CAST(0.0 AS NUMERIC(38, 2)) AS TotalWeightLbs
                         , '' AS Message1
                         , '' AS Message2
                         , '' AS SalesMsg
                         , '' AS SalesMsg2
                         , ISNULL(dbo.fnGetAttrKeyFromShortDesc('57', 'C1', CTE.Lite1_1stSrfCoat), '') AS Lite1_1stSrfCoatKey
                         , ISNULL(dbo.fnGetAttrKeyFromShortDesc('57', 'CT', CTE.Lite1_2ndSrfCoat), '') AS Lite1_2ndSrfCoatKey
                         , CAST(0.0 AS NUMERIC(38, 2)) AS WOrderNum
                         , '' AS CustPONum
                         , '' AS CustPOLine
                         , '' AS CustVendorID
                         , '' AS CustPartNum
                         , ISNULL(
                               CAST(ISNULL(CTE.BaseL, 0) AS VARCHAR(3))
                               + CASE WHEN ISNULL(CTE.BaseD, 0) <> 0 THEN
                                          ' ' + CAST(ISNULL(CTE.BaseN, 0) AS VARCHAR(2)) + '/' + CAST(ISNULL(CTE.BaseD, 0) AS VARCHAR(2))
                                      ELSE ''
                                 END
                             , '') AS BaseFractional
                         , ISNULL(
                               CAST(ISNULL(CTE.LeftL, 0) AS VARCHAR(3))
                               + CASE WHEN ISNULL(CTE.LeftD, 0) <> 0 THEN
                                          ' ' + CAST(ISNULL(CTE.LeftN, 0) AS VARCHAR(2)) + '/' + CAST(ISNULL(CTE.LeftD, 0) AS VARCHAR(2))
                                      ELSE ''
                                 END
                             , '') AS LeftFractional
                         , ISNULL(
                               CAST(ISNULL(CTE.RightL, 0) AS VARCHAR(3))
                               + CASE WHEN ISNULL(CTE.RightD, 0) <> 0 THEN
                                          ' ' + CAST(ISNULL(CTE.RightN, 0) AS VARCHAR(2)) + '/' + CAST(ISNULL(CTE.RightD, 0) AS VARCHAR(2))
                                      ELSE ''
                                 END
                             , '') AS RightFractional
                         , ISNULL(
                               CAST(ISNULL(CTE.TopL, 0) AS VARCHAR(3))
                               + CASE WHEN ISNULL(CTE.TopD, 0) <> 0 THEN
                                          ' ' + CAST(ISNULL(CTE.TopN, 0) AS VARCHAR(2)) + '/' + CAST(ISNULL(CTE.TopD, 0) AS VARCHAR(2))
                                      ELSE ''
                                 END
                             , '') AS TopFractional
                         , ISNULL(CTE.BaseMM, 0) AS BaseMM
                         , ISNULL(CTE.LeftMM, 0) AS LeftMM
                         , ISNULL(CTE.RightMM, 0) AS RightMM
                         , ISNULL(CTE.TopMM, 0) AS TopMM
                         , ISNULL(LEFT(CONVERT(VARCHAR, CTE.Writtendate, 120), 10), '') AS ProducedDate
                         , ISNULL(CONVERT(VARCHAR, CTE.Writtendate, 108), '') AS ProducedTime
                         , ISNULL(CAST(CTE.BaseL + ( CTE.BaseN / ISNULL(NULLIF(CTE.BaseD, 0), 1)) AS DECIMAL(19, 4)), 0) AS baseDec
                         , ISNULL(CAST(CTE.LeftL + ( CTE.LeftN / ISNULL(NULLIF(CTE.LeftD, 0), 1)) AS DECIMAL(19, 4)), 0) AS leftDec
                         , '' AS DueDate
                         , '' AS LineID
                         , '' AS Crew
                         , '' AS CustPrtNum2
                         , '' AS CustPrtNum3
                         , '' AS CustPrtNum4
                         , ISNULL(S.login_name, '') AS [User]
                         , ISNULL(LEFT(CONVERT(VARCHAR, GETDATE(), 120), 10), '') AS CurrentDate
                         , ISNULL(CONVERT(VARCHAR, GETDATE(), 108), '') AS CurrentTime
                         , CAST(0.0 AS NUMERIC(38, 2)) AS SOrderNum
                         , CAST(0.0 AS NUMERIC(38, 2)) AS SOrderLine
                         , ISNULL(CTE.SqftBillable, 0) AS SqftBillable
                         , ISNULL(REPLACE(CONVERT(VARCHAR, CTE.Writtendate, 108), ':', ''), '') AS ProducedTimeBarcode
                         , '' AS EdgeDelete
                         , CASE WHEN ISNULL(dbo.fnGetAttrShortDescription('57', 'CL', CTE.Lite1_Color), '') = 'CL' THEN ''
                                ELSE ISNULL(dbo.fnGetAttrShortDescription('57', 'CL', CTE.Lite1_Color), '')
                           END AS Lite1_ColorShortDesc
                         , ISNULL(dbo.fnGetAttrShortDescription('57', 'PT', CTE.Pattern), '') AS Lite1_PatternShortDesc
                         , ISNULL(dbo.fnGetAttrShortDescription('57', 'LM', CTE.Lite1_Lami), '') AS Lite1_LamiShortDesc
                         , ISNULL(RackType, '') AS RackType
                         , CAST(0.0 AS NUMERIC(38, 2)) AS FirmSchedule
                         , '' AS OriginalCustomer
                         , CTE.AnsiiLogoType
                         , CTE.PlantID
	                     , CTE.[AssetId]
                         , '' AS Shift
                         , '' AS GlassHandler
                         , '' AS GlassHandler2
                         , '' AS GlassHandler3
                         , '' AS GlassHandler4
						 , ISNULL(CTE.FreeLiteQuantity, 0) as FreeLiteQuantity
                    FROM   (   SELECT RawTagsID
                                    , SerialNum
                                    , ItemNum
                                    , Lite1_Thick
                                    , Lite1_Heat AS Lite1_HeatTreatment
                                    , Lite1_1stSrfCoating AS Lite1_1stSrfCoat
                                    , Lite1_2ndSrfCoating AS Lite1_2ndSrfCoat
                                    , [QtyOnHand]
                                    , CAST(CAST(dbo.ufn_ConvertDecimalToRational(( BaseL + ( BaseN / CASE WHEN BaseD <> 0 THEN BaseD
                                                                                                          ELSE 1.0
                                                                                                     END ))) AS VARCHAR(MAX)) + ' X '
                                           + CAST(dbo.ufn_ConvertDecimalToRational(( LeftL + ( LeftN / CASE WHEN LeftD <> 0 THEN LeftD
                                                                                                            ELSE 1.0
                                                                                                       END ))) AS VARCHAR(MAX)) AS VARCHAR(50)) AS Dimensions
                                    , BaseL
                                    , BaseN
                                    , BaseD
                                    , TopL
                                    , TopN
                                    , TopD
                                    , LeftL
                                    , LeftN
                                    , LeftD
                                    , RightL
                                    , RightN
                                    , RightD
                                    , CAST(ROUND((( BaseL + ( BaseN / CASE WHEN BaseD <> 0 THEN BaseD
                                                                           ELSE 1.0
                                                                      END )) * 25.4 )
                                               , 0) AS NUMERIC(5, 0)) AS BaseMM
                                    , CAST(ROUND((( TopL + ( TopN / CASE WHEN TopD <> 0 THEN TopD
                                                                         ELSE 1.0
                                                                    END )) * 25.4 )
                                               , 0) AS NUMERIC(5, 0)) AS TopMM
                                    , CAST(ROUND((( LeftL + ( LeftN / CASE WHEN LeftD <> 0 THEN LeftD
                                                                           ELSE 1.0
                                                                      END )) * 25.4 )
                                               , 0) AS NUMERIC(5, 0)) AS LeftMM
                                    , CAST(ROUND((( RightL + ( RightN / CASE WHEN RightD <> 0 THEN RightD
                                                                             ELSE 1.0
                                                                        END )) * 25.4 )
                                               , 0) AS NUMERIC(5, 0)) AS RightMM
                                    , SqftActual
                                    , SqftBillable
                                    , [ProductionDate]
                                    , Writtendate
                                    , Lite1_Color
                                    , Pattern
                                    , Lite1_Lami
                                    , RackType AS RackType
                                    , AnsiiLogoType
                                    , PlantID
				                    , [AssetId]
									, (SELECT SUM([FreeLiteQuantity]) FROM [dbo].[HVReceipts] WHERE SerialNumber = Rawtags.SerialNum) AS FreeLiteQuantity 
                               FROM   Rawtags
                               WHERE [SerialNum] = @SerialNum ) CTE
                           CROSS APPLY ( SELECT HOST_NAME() AS login_name ) S
                           OUTER APPLY ( SELECT [dbo].[ufn_gSFgetPoundsByThicknessAndArea](CTE.Lite1_Thick, CTE.SqftActual) AS Pounds ) AS GlassWeight
                    "
            };
            cmd.Parameters.Add(new SqlParameter("@SerialNum", SqlDbType.VarChar, 20));
            return cmd;
        }

        public HVRackTagItem GetRawTag(SqlConnection cnSF, string serialNum, decimal rackId)
        {
            var rackTagE1Current = new HVRackTagItem();
            try
            {
                var cmd = GetRawTag();
                cmd.Parameters["@SerialNum"].Value = serialNum;
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new HVRackTagItem();
                        rackTag.Customer = CastDbNull.To(reader["Customer"], string.Empty).Trim();
                        rackTag.PackingTinInTinOut = CastDbNull.To(reader["PackingTinInTinOut"], string.Empty).Trim();
                        rackTag.E1ItemNumber = CastDbNull.To(reader["E1ItemNumber"], string.Empty).Trim();
                        rackTag.GlassThickness = CastDbNull.To<decimal>(reader["GlassThickness"], 0);
                        rackTag.GlassHeatTreatment = CastDbNull.To(reader["GlassHeatTreatment"], string.Empty).Trim();
                        rackTag.FirstSurfaceCoat = CastDbNull.To(reader["FirstSurfaceCoat"], string.Empty).Trim();
                        rackTag.SecondSurfaceCoat = CastDbNull.To(reader["SecondSurfaceCoat"], string.Empty).Trim();

                        rackTag.SecondSurfaceCoatDesc1 = CastDbNull.To(reader["SecondSurfaceCoatDesc1"], string.Empty).Trim();
                        rackTag.TradeMarkInfoToPrint = CastDbNull.To(reader["TradeMarkInfoToPrint"], string.Empty).Trim();

                        rackTag.Dimensions = CastDbNull.To(reader["Dimensions"], string.Empty).Trim();
                        rackTag.DimensionsMetric = CastDbNull.To(reader["DimensionsMetric"], string.Empty).Trim();
                        rackTag.CurrentQuantity = CastDbNull.To<decimal>(reader["Quantity"], 0);
                        rackTag.SqftActual = CastDbNull.To<decimal>(reader["SqftActual"], 0);
                        rackTag.WeightLbs = CastDbNull.To<decimal>(reader["WeightLbs"], 0);
                        rackTag.WorkCenter = CastDbNull.To(reader["WorkCenter"], string.Empty).Trim();
                        rackTag.ProducedDateTime = CastDbNull.To<DateTime>(reader["ProducedDateTime"], new DateTime(1900, 1, 1));
                        rackTag.TotalWeightLbs = CastDbNull.To<decimal>(reader["TotalWeightLbs"], 0);
                        rackTag.Message1 = CastDbNull.To(reader["Message1"], string.Empty).Trim();
                        rackTag.Message2 = CastDbNull.To(reader["Message2"], string.Empty).Trim();
                        rackTag.SalesMsg = CastDbNull.To(reader["SalesMsg"], string.Empty).Trim();
                        rackTag.SalesMsg2 = CastDbNull.To(reader["SalesMsg2"], string.Empty).Trim();
                        rackTag.Lite1_1stSrfCoatKey = CastDbNull.To(reader["Lite1_1stSrfCoatKey"], string.Empty).Trim();
                        rackTag.Lite1_2ndSrfCoatKey = CastDbNull.To(reader["Lite1_2ndSrfCoatKey"], string.Empty).Trim();
                        rackTag.WOrderNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                        rackTag.CustPONum = CastDbNull.To(reader["CustPONum"], string.Empty).Trim();
                        rackTag.CustPOLine = CastDbNull.To(reader["CustPOLine"], string.Empty).Trim();
                        rackTag.CustVendorID = CastDbNull.To(reader["CustVendorID"], string.Empty).Trim();
                        rackTag.CustPartNum = CastDbNull.To(reader["CustPartNum"], string.Empty).Trim();
                        rackTag.BaseFractional = CastDbNull.To(reader["BaseFractional"], string.Empty).Trim();
                        rackTag.LeftFractional = CastDbNull.To(reader["LeftFractional"], string.Empty).Trim();
                        rackTag.RightFractional = CastDbNull.To(reader["RightFractional"], string.Empty).Trim();
                        rackTag.TopFractional = CastDbNull.To(reader["TopFractional"], string.Empty).Trim();
                        rackTag.BaseMM = CastDbNull.To<decimal>(reader["BaseMM"], 0);
                        rackTag.LeftMM = CastDbNull.To<decimal>(reader["LeftMM"], 0);
                        rackTag.RightMM = CastDbNull.To<decimal>(reader["RightMM"], 0);
                        rackTag.TopMM = CastDbNull.To<decimal>(reader["TopMM"], 0);
                        rackTag.ProducedDate = CastDbNull.To(reader["ProducedDate"], string.Empty).Trim();
                        rackTag.ProducedTime = CastDbNull.To(reader["ProducedTime"], string.Empty).Trim();
                        rackTag.BaseDec = CastDbNull.To<decimal>(reader["BaseDec"], 0);
                        rackTag.LeftDec = CastDbNull.To<decimal>(reader["LeftDec"], 0);
                        rackTag.DueDate = CastDbNull.To(reader["DueDate"], string.Empty).Trim();
                        rackTag.LineID = CastDbNull.To(reader["LineID"], string.Empty).Trim();
                        rackTag.Crew = CastDbNull.To(reader["Crew"], string.Empty).Trim();
                        rackTag.CustPrtNum2 = CastDbNull.To(reader["CustPrtNum2"], string.Empty).Trim();
                        rackTag.CustPrtNum3 = CastDbNull.To(reader["CustPrtNum3"], string.Empty).Trim();
                        rackTag.CustPrtNum4 = CastDbNull.To(reader["CustPrtNum4"], string.Empty).Trim();
                        rackTag.User = CastDbNull.To(reader["User"], string.Empty).Trim();
                        rackTag.CurrentDate = CastDbNull.To(reader["CurrentDate"], string.Empty).Trim();
                        rackTag.CurrentTime = CastDbNull.To(reader["CurrentTime"], string.Empty).Trim();
                        rackTag.SOrderNum = CastDbNull.To<decimal>(reader["SOrderNum"], 0);
                        rackTag.SOrderLine = CastDbNull.To<decimal>(reader["SOrderLine"], 0);
                        rackTag.SqftBillable = CastDbNull.To<decimal>(reader["SqftBillable"], 0);
                        rackTag.ProducedTimeBarcode = CastDbNull.To(reader["ProducedTimeBarcode"], string.Empty).Trim();
                        rackTag.EdgeDelete = CastDbNull.To(reader["EdgeDelete"], string.Empty).Trim();
                        rackTag.Lite1_ColorShortDesc = CastDbNull.To(reader["Lite1_ColorShortDesc"], string.Empty).Trim();
                        rackTag.Lite1_PatternShortDesc = CastDbNull.To(reader["Lite1_PatternShortDesc"], string.Empty).Trim();
                        rackTag.Lite1_LamiShortDesc = CastDbNull.To(reader["Lite1_LamiShortDesc"], string.Empty).Trim();
                        rackTag.RackType = CastDbNull.To(reader["RackType"], string.Empty).Trim();
                        rackTag.FirmSchedule = CastDbNull.To<decimal>(reader["FirmSchedule"], 0);
                        rackTag.AnsiiLogoType = CastDbNull.To(reader["AnsiiLogoType"], string.Empty).Trim();
                        rackTag.OriginalCustomer = CastDbNull.To(reader["OriginalCustomer"], string.Empty).Trim();
                        rackTag.AssetID = CastDbNull.To(reader["AssetID"], string.Empty).Trim();
                        rackTag.PlantID = CastDbNull.To(reader["PlantID"], string.Empty).Trim();
                        rackTag.Shift = CastDbNull.To(reader["Shift"], string.Empty).Trim();
                        rackTag.GlassHandler = CastDbNull.To(reader["GlassHandler"], string.Empty).Trim();
                        rackTag.GlassHandler2 = CastDbNull.To(reader["GlassHandler2"], string.Empty).Trim();
                        rackTag.GlassHandler3 = CastDbNull.To(reader["GlassHandler3"], string.Empty).Trim();
                        rackTag.GlassHandler4 = CastDbNull.To(reader["GlassHandler4"], string.Empty).Trim();
                        rackTag.FreeLiteQuantity = CastDbNull.To<decimal>(reader["FreeLiteQuantity"], 0);
                        rackTagE1Current = rackTag;
                        Logger.Trace($"Rack Id [{rackTagE1Current.RackId}] 1 row found");
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
            }
            return rackTagE1Current;
        }

        public HVRackTagItem GetRackTagSFMfgSGRT(SqlConnection cnSF, string serialNum, decimal rackId)
        {
            var rackTagE1Current = new HVRackTagItem();
            try
            {
                var cmd = GetRackTagSFMfgSGRT();
                cmd.Parameters["@SerialNum"].Value = serialNum;
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new HVRackTagItem();
                        rackTag.Customer = CastDbNull.To(reader["Customer"], string.Empty).Trim();
                        rackTag.PackingTinInTinOut = CastDbNull.To(reader["PackingTinInTinOut"], string.Empty).Trim();
                        rackTag.E1ItemNumber = CastDbNull.To(reader["E1ItemNumber"], string.Empty).Trim();
                        rackTag.GlassThickness = CastDbNull.To<decimal>(reader["GlassThickness"], 0);
                        rackTag.GlassHeatTreatment = CastDbNull.To(reader["GlassHeatTreatment"], string.Empty).Trim();
                        rackTag.FirstSurfaceCoat = CastDbNull.To(reader["FirstSurfaceCoat"], string.Empty).Trim();
                        rackTag.SecondSurfaceCoat = CastDbNull.To(reader["SecondSurfaceCoat"], string.Empty).Trim();
                        rackTag.Dimensions = CastDbNull.To(reader["Dimensions"], string.Empty).Trim();
                        rackTag.DimensionsMetric = CastDbNull.To(reader["DimensionsMetric"], string.Empty).Trim();
                        rackTag.Quantity = CastDbNull.To<decimal>(reader["Quantity"], 0);
                        rackTag.SqftActual = CastDbNull.To<decimal>(reader["SqftActual"], 0);
                        rackTag.WeightLbs = CastDbNull.To<decimal>(reader["WeightLbs"], 0);
                        rackTag.WorkCenter = CastDbNull.To(reader["WorkCenter"], string.Empty).Trim();
                        rackTag.ProducedDateTime = CastDbNull.To<DateTime>(reader["ProducedDateTime"], new DateTime(1900, 1, 1));
                        rackTag.TotalWeightLbs = CastDbNull.To<decimal>(reader["TotalWeightLbs"], 0);
                        rackTag.Message1 = CastDbNull.To(reader["Message1"], string.Empty).Trim();
                        rackTag.Message2 = CastDbNull.To(reader["Message2"], string.Empty).Trim();
                        rackTag.SalesMsg = CastDbNull.To(reader["SalesMsg"], string.Empty).Trim();
                        rackTag.SalesMsg2 = CastDbNull.To(reader["SalesMsg2"], string.Empty).Trim();
                        rackTag.Lite1_1stSrfCoatKey = CastDbNull.To(reader["Lite1_1stSrfCoatKey"], string.Empty).Trim();
                        rackTag.Lite1_2ndSrfCoatKey = CastDbNull.To(reader["Lite1_2ndSrfCoatKey"], string.Empty).Trim();
                        rackTag.WOrderNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                        rackTag.CustPONum = CastDbNull.To(reader["CustPONum"], string.Empty).Trim();
                        rackTag.CustPOLine = CastDbNull.To(reader["CustPOLine"], string.Empty).Trim();
                        rackTag.CustVendorID = CastDbNull.To(reader["CustVendorID"], string.Empty).Trim();
                        rackTag.CustPartNum = CastDbNull.To(reader["CustPartNum"], string.Empty).Trim();
                        rackTag.BaseFractional = CastDbNull.To(reader["BaseFractional"], string.Empty).Trim();
                        rackTag.LeftFractional = CastDbNull.To(reader["LeftFractional"], string.Empty).Trim();
                        rackTag.RightFractional = CastDbNull.To(reader["RightFractional"], string.Empty).Trim();
                        rackTag.TopFractional = CastDbNull.To(reader["TopFractional"], string.Empty).Trim();
                        rackTag.BaseMM = CastDbNull.To<decimal>(reader["BaseMM"], 0);
                        rackTag.LeftMM = CastDbNull.To<decimal>(reader["LeftMM"], 0);
                        rackTag.RightMM = CastDbNull.To<decimal>(reader["RightMM"], 0);
                        rackTag.TopMM = CastDbNull.To<decimal>(reader["TopMM"], 0);
                        rackTag.ProducedDate = CastDbNull.To(reader["ProducedDate"], string.Empty).Trim();
                        rackTag.ProducedTime = CastDbNull.To(reader["ProducedTime"], string.Empty).Trim();
                        rackTag.BaseDec = CastDbNull.To<decimal>(reader["BaseDec"], 0);
                        rackTag.LeftDec = CastDbNull.To<decimal>(reader["LeftDec"], 0);
                        rackTag.DueDate = CastDbNull.To(reader["DueDate"], string.Empty).Trim();
                        rackTag.LineID = CastDbNull.To(reader["LineID"], string.Empty).Trim();
                        rackTag.Crew = CastDbNull.To(reader["Crew"], string.Empty).Trim();
                        rackTag.CustPrtNum2 = CastDbNull.To(reader["CustPrtNum2"], string.Empty).Trim();
                        rackTag.CustPrtNum3 = CastDbNull.To(reader["CustPrtNum3"], string.Empty).Trim();
                        rackTag.CustPrtNum4 = CastDbNull.To(reader["CustPrtNum4"], string.Empty).Trim();
                        rackTag.User = CastDbNull.To(reader["User"], string.Empty).Trim();
                        rackTag.CurrentDate = CastDbNull.To(reader["CurrentDate"], string.Empty).Trim();
                        rackTag.CurrentTime = CastDbNull.To(reader["CurrentTime"], string.Empty).Trim();
                        rackTag.SOrderNum = CastDbNull.To<decimal>(reader["SOrderNum"], 0);
                        rackTag.SOrderLine = CastDbNull.To<decimal>(reader["SOrderLine"], 0);
                        rackTag.SqftBillable = CastDbNull.To<decimal>(reader["SqftBillable"], 0);
                        rackTag.ProducedTimeBarcode = CastDbNull.To(reader["ProducedTimeBarcode"], string.Empty).Trim();
                        rackTag.EdgeDelete = CastDbNull.To(reader["EdgeDelete"], string.Empty).Trim();
                        rackTag.Lite1_ColorShortDesc = CastDbNull.To(reader["Lite1_ColorShortDesc"], string.Empty).Trim();
                        rackTag.Lite1_PatternShortDesc = CastDbNull.To(reader["Lite1_PatternShortDesc"], string.Empty).Trim();
                        rackTag.Lite1_LamiShortDesc = CastDbNull.To(reader["Lite1_LamiShortDesc"], string.Empty).Trim();
                        rackTag.RackType = CastDbNull.To(reader["RackType"], string.Empty).Trim();
                        rackTag.Shift = CastDbNull.To(reader["Shift"], string.Empty).Trim();
                        rackTag.GlassHandler = CastDbNull.To(reader["GlassHandler"], string.Empty).Trim();
                        rackTag.GlassHandler2 = CastDbNull.To(reader["GlassHandler2"], string.Empty).Trim();
                        rackTag.GlassHandler3 = CastDbNull.To(reader["GlassHandler3"], string.Empty).Trim();
                        rackTag.GlassHandler4 = CastDbNull.To(reader["GlassHandler4"], string.Empty).Trim();
                        rackTag.Target = CastDbNull.To(reader["Target"], string.Empty).Trim();
                        rackTagE1Current = rackTag;
                        Logger.Trace($"Rack Id [{rackTagE1Current.RackId}] 1 row found");
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
            }
            return rackTagE1Current;
        }

        public SqlCommand GetRackTagSFMfgSGRT()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT TOP (1) v.[HVQueID]
                          ,v.[SerialNumber]
                          ,v.[Customer]
                          ,v.[PackingTinInTinOut]
                          ,v.[E1ItemNumber]
                          ,v.[GlassThickness]
                          ,v.[GlassHeatTreatment]
                          ,v.[FirstSurfaceCoat]
                          ,v.[SecondSurfaceCoat]
                          ,v.[Dimensions]
                          ,v.[DimensionsMetric]
                          ,v.[Quantity]
                          ,v.[SqftActual]
                          ,v.[WeightLbs]
                          ,v.[WorkCenter]
                          ,v.[ProducedDateTime]
                          ,v.[TotalWeightLbs]
                          ,v.[Message1]
                          ,v.[Message2]
                          ,v.[SalesMsg]
                          ,v.[SalesMsg2]
                          ,v.[Lite1_1stSrfCoatKey]
                          ,v.[Lite1_2ndSrfCoatKey]
                          ,v.[WOrderNum]
                          ,v.[CustPONum]
                          ,v.[CustPOLine]
                          ,v.[CustVendorID]
                          ,v.[CustPartNum]
                          ,v.[BaseFractional]
                          ,v.[LeftFractional]
                          ,v.[RightFractional]
                          ,v.[TopFractional]
                          ,v.[BaseMM]
                          ,v.[LeftMM]
                          ,v.[RightMM]
                          ,v.[TopMM]
                          ,v.[ProducedDate]
                          ,v.[ProducedTime]
                          ,v.[baseDec]
                          ,v.[leftDec]
                          ,v.[DueDate]
                          ,v.[LineID]
                          ,v.[Crew]
                          ,v.[CustPrtNum2]
                          ,v.[CustPrtNum3]
                          ,v.[CustPrtNum4]
                          ,v.[User]
                          ,v.[CurrentDate]
                          ,v.[CurrentTime]
                          ,v.[SOrderNum]
                          ,v.[SOrderLine]
                          ,v.[SqftBillable]
                          ,v.[ProducedTimeBarcode]
                          ,v.[EdgeDelete]
                          ,v.[Lite1_ColorShortDesc]
                          ,v.[Lite1_PatternShortDesc]
                          ,v.[Lite1_LamiShortDesc]
                          ,v.[RackType]
                          ,v.[Target]
                          ,h.[Shift]
                          ,h.[GlassHandler]
                          ,h.[GlassHandler2]
                          ,h.[GlassHandler3]
                          ,h.[GlassHandler4]
                      FROM [dbo].[vw_HVTagPrinting] v
                      LEFT OUTER JOIN [dbo].[HVCGCrateTagHistory] h ON h.[SerialNumber] = v.[SerialNumber]
                      WHERE v.[SerialNumber] = @SerialNum
                    "
            };
            cmd.Parameters.Add(new SqlParameter("@SerialNum", SqlDbType.VarChar, 20));
            return cmd;
        }


        public HVRackTagItem GetRackTagSFMfg(SqlConnection cnSF, string serialNum, decimal rackId)
        {
            var rackTagE1Current = new HVRackTagItem();
            try
            {
                var cmd = GetRackTagSFMfg();
                cmd.Parameters["@SerialNum"].Value = serialNum;
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new HVRackTagItem();
                        rackTag.Customer = CastDbNull.To(reader["Customer"], string.Empty).Trim();
                        rackTag.PackingTinInTinOut = CastDbNull.To(reader["PackingTinInTinOut"], string.Empty).Trim();
                        rackTag.E1ItemNumber = CastDbNull.To(reader["E1ItemNumber"], string.Empty).Trim();
                        rackTag.GlassThickness = CastDbNull.To<decimal>(reader["GlassThickness"], 0);
                        rackTag.GlassHeatTreatment = CastDbNull.To(reader["GlassHeatTreatment"], string.Empty).Trim();
                        rackTag.FirstSurfaceCoat = CastDbNull.To(reader["FirstSurfaceCoat"], string.Empty).Trim();
                        rackTag.SecondSurfaceCoat = CastDbNull.To(reader["SecondSurfaceCoat"], string.Empty).Trim();

                        rackTag.SecondSurfaceCoatDesc1 = CastDbNull.To(reader["SecondSurfaceCoatDesc1"], string.Empty).Trim();
                        rackTag.TradeMarkInfoToPrint = CastDbNull.To(reader["TradeMarkInfoToPrint"], string.Empty).Trim();

                        rackTag.Dimensions = CastDbNull.To(reader["Dimensions"], string.Empty).Trim();
                        rackTag.DimensionsMetric = CastDbNull.To(reader["DimensionsMetric"], string.Empty).Trim();
                        rackTag.Quantity = CastDbNull.To<decimal>(reader["Quantity"], 0);
                        rackTag.SqftActual = CastDbNull.To<decimal>(reader["SqftActual"], 0);
                        rackTag.WeightLbs = CastDbNull.To<decimal>(reader["WeightLbs"], 0);
                        rackTag.WorkCenter = CastDbNull.To(reader["WorkCenter"], string.Empty).Trim();
                        rackTag.ProducedDateTime = CastDbNull.To<DateTime>(reader["ProducedDateTime"], new DateTime(1900, 1, 1));
                        rackTag.TotalWeightLbs = CastDbNull.To<decimal>(reader["TotalWeightLbs"], 0);
                        rackTag.Message1 = CastDbNull.To(reader["Message1"], string.Empty).Trim();
                        rackTag.Message2 = CastDbNull.To(reader["Message2"], string.Empty).Trim();
                        rackTag.SalesMsg = CastDbNull.To(reader["SalesMsg"], string.Empty).Trim();
                        rackTag.SalesMsg2 = CastDbNull.To(reader["SalesMsg2"], string.Empty).Trim();
                        rackTag.Lite1_1stSrfCoatKey = CastDbNull.To(reader["Lite1_1stSrfCoatKey"], string.Empty).Trim();
                        rackTag.Lite1_2ndSrfCoatKey = CastDbNull.To(reader["Lite1_2ndSrfCoatKey"], string.Empty).Trim();
                        rackTag.WOrderNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                        rackTag.CustPONum = CastDbNull.To(reader["CustPONum"], string.Empty).Trim();
                        rackTag.CustPOLine = CastDbNull.To(reader["CustPOLine"], string.Empty).Trim();
                        rackTag.CustVendorID = CastDbNull.To(reader["CustVendorID"], string.Empty).Trim();
                        rackTag.CustPartNum = CastDbNull.To(reader["CustPartNum"], string.Empty).Trim();
                        rackTag.BaseFractional = CastDbNull.To(reader["BaseFractional"], string.Empty).Trim();
                        rackTag.LeftFractional = CastDbNull.To(reader["LeftFractional"], string.Empty).Trim();
                        rackTag.RightFractional = CastDbNull.To(reader["RightFractional"], string.Empty).Trim();
                        rackTag.TopFractional = CastDbNull.To(reader["TopFractional"], string.Empty).Trim();
                        rackTag.BaseMM = CastDbNull.To<decimal>(reader["BaseMM"], 0);
                        rackTag.LeftMM = CastDbNull.To<decimal>(reader["LeftMM"], 0);
                        rackTag.RightMM = CastDbNull.To<decimal>(reader["RightMM"], 0);
                        rackTag.TopMM = CastDbNull.To<decimal>(reader["TopMM"], 0);
                        rackTag.ProducedDate = CastDbNull.To(reader["ProducedDate"], string.Empty).Trim();
                        rackTag.ProducedTime = CastDbNull.To(reader["ProducedTime"], string.Empty).Trim();
                        rackTag.BaseDec = CastDbNull.To<decimal>(reader["BaseDec"], 0);
                        rackTag.LeftDec = CastDbNull.To<decimal>(reader["LeftDec"], 0);
                        rackTag.DueDate = CastDbNull.To(reader["DueDate"], string.Empty).Trim();
                        rackTag.LineID = CastDbNull.To(reader["LineID"], string.Empty).Trim();
                        rackTag.Crew = CastDbNull.To(reader["Crew"], string.Empty).Trim();
                        rackTag.CustPrtNum2 = CastDbNull.To(reader["CustPrtNum2"], string.Empty).Trim();
                        rackTag.CustPrtNum3 = CastDbNull.To(reader["CustPrtNum3"], string.Empty).Trim();
                        rackTag.CustPrtNum4 = CastDbNull.To(reader["CustPrtNum4"], string.Empty).Trim();
                        rackTag.User = CastDbNull.To(reader["User"], string.Empty).Trim();
                        rackTag.CurrentDate = CastDbNull.To(reader["CurrentDate"], string.Empty).Trim();
                        rackTag.CurrentTime = CastDbNull.To(reader["CurrentTime"], string.Empty).Trim();
                        rackTag.SOrderNum = CastDbNull.To<decimal>(reader["SOrderNum"], 0);
                        rackTag.SOrderLine = CastDbNull.To<decimal>(reader["SOrderLine"], 0);
                        rackTag.SqftBillable = CastDbNull.To<decimal>(reader["SqftBillable"], 0);
                        rackTag.ProducedTimeBarcode = CastDbNull.To(reader["ProducedTimeBarcode"], string.Empty).Trim();
                        rackTag.EdgeDelete = CastDbNull.To(reader["EdgeDelete"], string.Empty).Trim();
                        rackTag.Lite1_ColorShortDesc = CastDbNull.To(reader["Lite1_ColorShortDesc"], string.Empty).Trim();
                        rackTag.Lite1_PatternShortDesc = CastDbNull.To(reader["Lite1_PatternShortDesc"], string.Empty).Trim();
                        rackTag.Lite1_LamiShortDesc = CastDbNull.To(reader["Lite1_LamiShortDesc"], string.Empty).Trim();
                        rackTag.RackType = CastDbNull.To(reader["RackType"], string.Empty).Trim();
                        rackTag.FirmSchedule = CastDbNull.To<decimal>(reader["FirmSchedule"], 0);
                        rackTag.AnsiiLogoType = CastDbNull.To(reader["AnsiiLogoType"], string.Empty).Trim();
                        rackTag.OriginalCustomer = CastDbNull.To(reader["OriginalCustomer"], string.Empty).Trim();
                        rackTag.AssetID = CastDbNull.To(reader["AssetID"], string.Empty).Trim();
                        rackTag.PlantID = CastDbNull.To(reader["PlantID"], string.Empty).Trim();
                        rackTag.Shift = CastDbNull.To(reader["Shift"], string.Empty).Trim();
                        rackTag.GlassHandler = CastDbNull.To(reader["GlassHandler"], string.Empty).Trim();
                        rackTag.GlassHandler2 = CastDbNull.To(reader["GlassHandler2"], string.Empty).Trim();
                        rackTag.GlassHandler3 = CastDbNull.To(reader["GlassHandler3"], string.Empty).Trim();
                        rackTag.GlassHandler4 = CastDbNull.To(reader["GlassHandler4"], string.Empty).Trim();
                        rackTag.LitesPerInch = CastDbNull.To<Double>(reader["LitesPerInch"], 0);
                        ;
                        rackTag.CalculatedLitesPerInch = CastDbNull.To(reader["CalculatedLitesPerInch"], string.Empty).Trim();
                        rackTagE1Current = rackTag;
                        Logger.Trace($"Rack Id [{rackTagE1Current.RackId}] 1 row found");
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
            }
            return rackTagE1Current;
        }

        public SqlCommand GetRackTagSFMfg()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT TOP (1) v.[HVQueID]
                          ,v.[SerialNumber]
                          ,v.[Customer]
                          ,v.[PackingTinInTinOut]
                          ,v.[E1ItemNumber]
                          ,v.[GlassThickness]
                          ,v.[GlassHeatTreatment]
                          ,v.[FirstSurfaceCoat]
                          ,v.[SecondSurfaceCoat]
                        ,ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', v.[SecondSurfaceCoat]),'') AS SecondSurfaceCoatDesc1
                        ,CASE WHEN LEFT(v.[SecondSurfaceCoat],2) = 'CS' THEN 
                        LEFT(ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', v.[SecondSurfaceCoat]),''),CHARINDEX(' ',ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', v.[SecondSurfaceCoat]),''),9)) + char(0174) + ' ' + SUBSTRING(ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', v.[SecondSurfaceCoat]),''),CHARINDEX(' ',ISNULL(dbo.fnGetAttrDescriptionOne('57', 'CT', v.[SecondSurfaceCoat]),''),9)+1,2) 
						ELSE '' END AS TradeMarkInfoToPrint
                          ,v.[Dimensions]
                          ,v.[DimensionsMetric]
                          ,v.[Quantity]
                          ,v.[SqftActual]
                          ,v.[WeightLbs]
                          ,v.[WorkCenter]
                          ,v.[ProducedDateTime]
                          ,v.[TotalWeightLbs]
                          ,v.[Message1]
                          ,v.[Message2]
                          ,v.[SalesMsg]
                          ,v.[SalesMsg2]
                          ,v.[Lite1_1stSrfCoatKey]
                          ,v.[Lite1_2ndSrfCoatKey]
                          ,v.[WOrderNum]
                          ,v.[CustPONum]
                          ,v.[CustPOLine]
                          ,v.[CustVendorID]
                          ,v.[CustPartNum]
                          ,v.[BaseFractional]
                          ,v.[LeftFractional]
                          ,v.[RightFractional]
                          ,v.[TopFractional]
                          ,v.[BaseMM]
                          ,v.[LeftMM]
                          ,v.[RightMM]
                          ,v.[TopMM]
                          ,v.[ProducedDate]
                          ,v.[ProducedTime]
                          ,v.[baseDec]
                          ,v.[leftDec]
                          ,v.[DueDate]
                          ,v.[LineID]
                          ,v.[Crew]
                          ,v.[CustPrtNum2]
                          ,v.[CustPrtNum3]
                          ,v.[CustPrtNum4]
                          ,v.[User]
                          ,v.[CurrentDate]
                          ,v.[CurrentTime]
                          ,v.[SOrderNum]
                          ,v.[SOrderLine]
                          ,v.[SqftBillable]
                          ,v.[ProducedTimeBarcode]
                          ,v.[EdgeDelete]
                          ,v.[Lite1_ColorShortDesc]
                          ,v.[Lite1_PatternShortDesc]
                          ,v.[Lite1_LamiShortDesc]
                          ,v.[RackType]
                          ,v.[FirmSchedule]
                          ,v.[AnsiiLogoType]
                          ,v.[OriginalCustomer]
                          ,v.[AssetID]
                          ,v.[PlantID]
                          ,h.[Shift]
                          ,h.[GlassHandler]
                          ,h.[GlassHandler2]
                          ,h.[GlassHandler3]
                          ,h.[GlassHandler4]
                          ,h.[LitesPerInch]
						  ,( SELECT MAX(T.LitesPerInch) AS LitesPerInch
                            FROM   (   SELECT   TableSegmentValue1 AS ThicknessTypeCode
                                              , MAX(CASE WHEN BubbleSequence = 1 THEN RulesTableValue
                                                    END) AS Thickness
                                              , MAX(CASE WHEN BubbleSequence = 2 THEN RulesTableValue
                                                    END) AS LbsPerSqFt
                                              , MAX(CASE WHEN BubbleSequence = 3 THEN RulesTableValue
                                                    END) AS LitesPerInch
                                       FROM     EBS.ConfiguratorRule
                                       WHERE    RulesTableType = 'C'
                                                AND RulesTableName = 'THKWGTLPI'
                                       GROUP BY TableSegmentValue1 ) AS T
                            WHERE  T.Thickness = v.[GlassThickness] ) AS 'CalculatedLitesPerInch'
                      FROM [dbo].[vw_HVTagPrinting] v
                      LEFT OUTER JOIN [dbo].[HVCGCrateTagHistory] h ON h.[SerialNumber] = v.[SerialNumber]
                      WHERE v.[SerialNumber] = @SerialNum
                    "
            };
            cmd.Parameters.Add(new SqlParameter("@SerialNum", SqlDbType.VarChar, 20));
            return cmd;
        }

        public HVRackTagItem GetRackTagE1Current(BusinessSystemSession e1Session, decimal rackId)
        {
            var rackTagE1Current = new HVRackTagItem();
            try
            {
                var cmd = GetRackTagItem();
                cmd.Parameters[0].Value = rackId;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new HVRackTagItem();
                        rackTag.RackId = CastDbNull.To<decimal>(reader["RHZ58RID"], 0);
                        rackTag.Customer = CastDbNull.To(reader["AN8ALPH"], string.Empty).Trim();
                        rackTag.CurrentPlantCustomer = rackTag.Customer;
                        rackTag.ShipToCustomer = CastDbNull.To(reader["SHANALPH"], string.Empty).Trim();
                        rackTag.CurrentPlantShipToCustomer = rackTag.ShipToCustomer;
                        rackTag.PackingTinInTinOut = "";
                        rackTag.E1ItemNumber = CastDbNull.To<decimal>(reader["RDITM"], 0).ToString();
                        rackTag.GlassThickness = 0;
                        rackTag.GlassHeatTreatment = "";
                        rackTag.FirstSurfaceCoat = "";
                        rackTag.SecondSurfaceCoat = "";
                        rackTag.Dimensions = "";
                        rackTag.DimensionsMetric = "";
                        rackTag.Quantity = CastDbNull.To<decimal>(reader["RDPQOH"], 0);
                        rackTag.SqftActual = 0;
                        rackTag.WeightLbs = 0;
                        rackTag.WorkCenter = "";
                        rackTag.ProducedDateTime = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["RDZ58MDT"], 0));
                        rackTag.TotalWeightLbs = 0;
                        rackTag.Message1 = "";
                        rackTag.Message2 = "";
                        rackTag.SalesMsg = "";
                        rackTag.SalesMsg2 = "";
                        rackTag.Lite1_1stSrfCoatKey = "";
                        rackTag.Lite1_2ndSrfCoatKey = "";
                        rackTag.WOrderNum = CastDbNull.To<decimal>(reader["RDDOC1"], 0);
                        rackTag.CustPONum = CastDbNull.To(reader["SDVR01"], string.Empty).Trim();
                        rackTag.CustPOLine = "";
                        rackTag.CustVendorID = "";
                        rackTag.CustPartNum = CastDbNull.To(reader["ATZ58CP2"], string.Empty).Trim();
                        rackTag.BaseFractional = "";
                        rackTag.LeftFractional = "";
                        rackTag.RightFractional = "";
                        rackTag.TopFractional = "";
                        rackTag.BaseMM = 0;
                        rackTag.LeftMM = 0;
                        rackTag.RightMM = 0;
                        rackTag.TopMM = 0;
                        rackTag.ProducedDate = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["RDZ58MDT"], 0)).ToString(); ;
                        rackTag.ProducedTime = "";
                        rackTag.BaseDec = 0;
                        rackTag.LeftDec = 0;
                        rackTag.DueDate = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0)).ToString();
                        rackTag.LineID = "";
                        rackTag.Crew = "";
                        rackTag.CustPrtNum2 = CastDbNull.To(reader["ATZ58CP3"], string.Empty).Trim();
                        rackTag.CustPrtNum3 = CastDbNull.To(reader["ATZ58CP4"], string.Empty).Trim();
                        rackTag.CustPrtNum4 = "";
                        rackTag.User = "";
                        rackTag.CurrentDate = "";
                        rackTag.CurrentTime = "";
                        rackTag.SOrderNum = CastDbNull.To<decimal>(reader["RDDOCO"], 0);
                        rackTag.SOrderLine = Math.Round(CastDbNull.To<decimal>(reader["RDLNID"], 0) / 1000, 3);
                        rackTag.SqftBillable = 0;
                        rackTag.ProducedTimeBarcode = "";
                        rackTag.EdgeDelete = "";
                        rackTag.Lite1_ColorShortDesc = "";
                        rackTag.Lite1_PatternShortDesc = "";
                        rackTag.Lite1_LamiShortDesc = "";
                        rackTag.RackType = CastDbNull.To(reader["RHZ58RT"], string.Empty).Trim();
                        rackTag.RackTareWeightLbs = CastDbNull.To<decimal>(reader["RHZ58RTWT"], 0) / 100;
                        rackTag.FirmSchedule = 0;
                        rackTag.AnsiiLogoType = "";
                        rackTag.OriginalCustomer = "";
                        rackTag.AssetID = "";
                        rackTag.PlantID = CastDbNull.To(reader["RHMCU"], string.Empty).Trim();
                        rackTag.ScheduleNum = CastDbNull.To<decimal>(reader["S1Z58SCN"], 0);
                        rackTag.ShopFloorSystem = CastDbNull.To(reader["S1Z58SFS"], string.Empty).Trim();
                        rackTag.ConfigId = CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                        rackTag.Comment1 = CastDbNull.To(reader["S5Z58CMT1"], string.Empty).Trim();
                        rackTag.Comment2 = CastDbNull.To(reader["S5Z58CMT2"], string.Empty).Trim();

                        rackTagE1Current = rackTag;
                        Logger.Trace($"Rack Id [{rackTagE1Current.RackId}] 1 row found");
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
            }
            return rackTagE1Current;
        }

        public HVRackTagItem GetRackTagE1Mfg(BusinessSystemSession e1Session, string plantCode, decimal rackId)
        {
            var rackTagE1Mfg = new HVRackTagItem();
            try
            {
                var cmd = GetRackTagItemArchive();
                cmd.Parameters[0].Value = rackId;
                cmd.Parameters[1].Value = plantCode.PadLeft(12);
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new HVRackTagItem();
                        rackTag.RackId = CastDbNull.To<decimal>(reader["RHZ58RID"], 0);
                        rackTag.Customer = CastDbNull.To(reader["AN8ALPH"], string.Empty).Trim();
                        rackTag.ActualMfgPlantCustomer = rackTag.Customer;
                        rackTag.ProducingPlantCustomer = rackTag.Customer;
                        rackTag.ShipToCustomer = CastDbNull.To(reader["SHANALPH"], string.Empty).Trim();
                        rackTag.ActualMfgPlantShipToCustomer = rackTag.ShipToCustomer;
                        rackTag.ProducingPlantShipToCustomer = rackTag.ShipToCustomer;
                        rackTag.PackingTinInTinOut = "";
                        rackTag.E1ItemNumber = CastDbNull.To<decimal>(reader["RDITM"], 0).ToString();
                        rackTag.GlassThickness = 0;
                        rackTag.GlassHeatTreatment = "";
                        rackTag.FirstSurfaceCoat = "";
                        rackTag.SecondSurfaceCoat = "";
                        rackTag.Dimensions = "";
                        rackTag.DimensionsMetric = "";
                        rackTag.Quantity = CastDbNull.To<decimal>(reader["RDPQOH"], 0);
                        rackTag.SqftActual = 0;
                        rackTag.WeightLbs = 0;
                        rackTag.WorkCenter = "";
                        rackTag.ProducedDateTime = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["DLZ58MDT"], 0));
                        rackTag.TotalWeightLbs = 0;
                        rackTag.Message1 = "";
                        rackTag.Message2 = "";
                        rackTag.SalesMsg = "";
                        rackTag.SalesMsg2 = "";
                        rackTag.Lite1_1stSrfCoatKey = "";
                        rackTag.Lite1_2ndSrfCoatKey = "";
                        rackTag.WOrderNum = CastDbNull.To<decimal>(reader["RDDOC1"], 0);
                        rackTag.CustPONum = CastDbNull.To(reader["SDVR01"], string.Empty).Trim();
                        rackTag.CustPOLine = "";
                        rackTag.CustVendorID = "";
                        rackTag.CustPartNum = CastDbNull.To(reader["ATZ58CP2"], string.Empty).Trim();
                        rackTag.BaseFractional = "";
                        rackTag.LeftFractional = "";
                        rackTag.RightFractional = "";
                        rackTag.TopFractional = "";
                        rackTag.BaseMM = 0;
                        rackTag.LeftMM = 0;
                        rackTag.RightMM = 0;
                        rackTag.TopMM = 0;
                        rackTag.ProducedDate = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["DLZ58MDT"], 0)).ToString();
                        rackTag.LineID = ""; ;
                        rackTag.ProducedTime = "";
                        rackTag.BaseDec = 0;
                        rackTag.LeftDec = 0;
                        rackTag.DueDate = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0)).ToString();
                        rackTag.LineID = "";
                        rackTag.Crew = "";
                        rackTag.CustPrtNum2 = CastDbNull.To(reader["ATZ58CP3"], string.Empty).Trim();
                        rackTag.CustPrtNum3 = CastDbNull.To(reader["ATZ58CP4"], string.Empty).Trim();
                        rackTag.CustPrtNum4 = "";
                        rackTag.User = "";
                        rackTag.CurrentDate = "";
                        rackTag.CurrentTime = "";
                        rackTag.SOrderNum = CastDbNull.To<decimal>(reader["RDDOCO"], 0);
                        rackTag.SOrderLine = Math.Round(CastDbNull.To<decimal>(reader["RDLNID"], 0) / 1000, 3);
                        rackTag.SqftBillable = 0;
                        rackTag.ProducedTimeBarcode = "";
                        rackTag.EdgeDelete = "";
                        rackTag.Lite1_ColorShortDesc = "";
                        rackTag.Lite1_PatternShortDesc = "";
                        rackTag.Lite1_LamiShortDesc = "";
                        rackTag.RackType = CastDbNull.To(reader["RHZ58RT"], string.Empty).Trim();
                        rackTag.RackTareWeightLbs = CastDbNull.To<decimal>(reader["RTZ58RTWT"], 0) / 100;
                        rackTag.FirmSchedule = 0;
                        rackTag.AnsiiLogoType = "";
                        rackTag.OriginalCustomer = "";
                        rackTag.AssetID = CastDbNull.To(reader["AMZ58AID"], string.Empty).Trim();
                        rackTag.PlantID = CastDbNull.To(reader["RHMCU"], string.Empty).Trim();
                        rackTag.ScheduleNum = CastDbNull.To<decimal>(reader["S1Z58SCN"], 0);
                        rackTag.ShopFloorSystem = CastDbNull.To(reader["S1Z58SFS"], string.Empty).Trim();
                        rackTag.ConfigId = CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                        rackTag.Comment1 = CastDbNull.To(reader["S5Z58CMT1"], string.Empty).Trim();
                        rackTag.Comment2 = CastDbNull.To(reader["S5Z58CMT2"], string.Empty).Trim();

                        rackTagE1Mfg = rackTag;
                        Logger.Trace($"Rack Id [{rackTagE1Mfg.RackId}] 1 row found");
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
            }
            return rackTagE1Mfg;
        }

        public HVRackTagItem GetRackTagE1Requesting(BusinessSystemSession e1Session, string plantCode, decimal rackId)
        {
            var rackTagE1Requesting = new HVRackTagItem();
            try
            {
                var cmd = GetRackTagItemArchive();
                cmd.Parameters[0].Value = rackId;
                cmd.Parameters[1].Value = plantCode.PadLeft(12);
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackTag = new HVRackTagItem();
                        rackTag.RackId = CastDbNull.To<decimal>(reader["RHZ58RID"], 0);
                        rackTag.Customer = CastDbNull.To(reader["AN8ALPH"], string.Empty).Trim();
                        rackTag.RequestingPlantCustomer = rackTag.Customer;
                        rackTag.ShipToCustomer = CastDbNull.To(reader["SHANALPH"], string.Empty).Trim();
                        rackTag.RequestingPlantShipToCustomer = rackTag.ShipToCustomer;
                        rackTag.PackingTinInTinOut = "";
                        rackTag.E1ItemNumber = CastDbNull.To<decimal>(reader["RDITM"], 0).ToString();
                        rackTag.GlassThickness = 0;
                        rackTag.GlassHeatTreatment = "";
                        rackTag.FirstSurfaceCoat = "";
                        rackTag.SecondSurfaceCoat = "";
                        rackTag.Dimensions = "";
                        rackTag.DimensionsMetric = "";
                        rackTag.Quantity = CastDbNull.To<decimal>(reader["RDPQOH"], 0);
                        rackTag.SqftActual = 0;
                        rackTag.WeightLbs = 0;
                        rackTag.WorkCenter = "";
                        rackTag.ProducedDateTime = DateTime.Now.ToLocalTime();
                        rackTag.TotalWeightLbs = 0;
                        rackTag.Message1 = "";
                        rackTag.Message2 = "";
                        rackTag.SalesMsg = "";
                        rackTag.SalesMsg2 = "";
                        rackTag.Lite1_1stSrfCoatKey = "";
                        rackTag.Lite1_2ndSrfCoatKey = "";
                        rackTag.WOrderNum = CastDbNull.To<decimal>(reader["RDDOC1"], 0);
                        rackTag.CustPONum = CastDbNull.To(reader["SDVR01"], string.Empty).Trim();
                        rackTag.CustPOLine = "";
                        rackTag.CustVendorID = "";
                        rackTag.CustPartNum = CastDbNull.To(reader["ATZ58CP2"], string.Empty).Trim();
                        rackTag.BaseFractional = "";
                        rackTag.LeftFractional = "";
                        rackTag.RightFractional = "";
                        rackTag.TopFractional = "";
                        rackTag.BaseMM = 0;
                        rackTag.LeftMM = 0;
                        rackTag.RightMM = 0;
                        rackTag.TopMM = 0;
                        rackTag.ProducedDate = "";
                        rackTag.ProducedTime = "";
                        rackTag.BaseDec = 0;
                        rackTag.LeftDec = 0;
                        rackTag.DueDate = Utilities.Common.ConvertE1Date(CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0)).ToString();
                        rackTag.LineID = "";
                        rackTag.Crew = "";
                        rackTag.CustPrtNum2 = CastDbNull.To(reader["ATZ58CP3"], string.Empty).Trim();
                        rackTag.CustPrtNum3 = CastDbNull.To(reader["ATZ58CP4"], string.Empty).Trim();
                        rackTag.CustPrtNum4 = "";
                        rackTag.User = "";
                        rackTag.CurrentDate = "";
                        rackTag.CurrentTime = "";
                        rackTag.SOrderNum = CastDbNull.To<decimal>(reader["RDDOCO"], 0);
                        rackTag.SOrderLine = Math.Round(CastDbNull.To<decimal>(reader["RDLNID"], 0) / 1000, 3);
                        rackTag.SqftBillable = 0;
                        rackTag.ProducedTimeBarcode = "";
                        rackTag.EdgeDelete = "";
                        rackTag.Lite1_ColorShortDesc = "";
                        rackTag.Lite1_PatternShortDesc = "";
                        rackTag.Lite1_LamiShortDesc = "";
                        rackTag.RackType = CastDbNull.To(reader["RHZ58RT"], string.Empty).Trim();
                        rackTag.RackTareWeightLbs = CastDbNull.To<decimal>(reader["RHZ58RTWT"], 0) / 100;
                        rackTag.FirmSchedule = 0;
                        rackTag.AnsiiLogoType = "";
                        rackTag.OriginalCustomer = "";
                        rackTag.AssetID = CastDbNull.To(reader["AMZ58AID"], string.Empty).Trim();
                        rackTag.PlantID = CastDbNull.To(reader["RHMCU"], string.Empty).Trim();
                        rackTag.ScheduleNum = CastDbNull.To<decimal>(reader["S1Z58SCN"], 0);
                        rackTag.ShopFloorSystem = CastDbNull.To(reader["S1Z58SFS"], string.Empty).Trim();
                        rackTag.ConfigId = CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                        rackTag.Comment1 = CastDbNull.To(reader["S5Z58CMT1"], string.Empty).Trim();
                        rackTag.Comment2 = CastDbNull.To(reader["S5Z58CMT2"], string.Empty).Trim();

                        rackTagE1Requesting = rackTag;
                        Logger.Trace($"Rack Id [{rackTagE1Requesting.RackId}] 1 row found");
                    }

                    reader.Close();
                    //Logger.Trace($"Rack Ids [{string.Join(",", rackTagsE1Requesting.GroupBy(x => x.RackId).Select(x => x.Key).ToArray())}]");
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
            }

            return rackTagE1Requesting;
        }

        public iDB2Command GetRackTagItem()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        RHZ58RID,
                        RHMCU,
                        RHAN8,
                        RHZ58HST,
                        RHZ58RSDT,
                        AB1.ABALPH AS AN8ALPH,
                        RHSHAN,
                        AB2.ABALPH AS SHANALPH,  
                        RHZ58RT,
                        UD1.DRDL01 AS RHZ58RT_DESC,
						RTZ58RSL,
                        RTZ58RTWT,
                        RHZ58BOX,
                        RHZ58GWTT,
                        RHZ58RTWT,
                        RHZ58SCN,
                        RHLOCN,
                        ATZ5899003,
                        ATZ58CP2,
                        ATZ58CP3,
                        ATZ58CP4,
                        ATZ58CSQNO,
                        ATZ58GSQNO,
                        ATZ5899009,
                        ATZ5899010,
                        ATZ5899011,
                        ATZ5899012,
                        ATZ58PT,
ATZ5800520,
ATZ5899010,
                        SHDRQJ,
                        SDDRQJ,
                        SDUORG,
                        SDVR01,
                        SDRCD,
                        S1Z58SCN,
                        S1Z58SFS,
                        S1Z58SCDDJ,
                        S1Z58SCTYP,
				S5Z58CMT1,
				S5Z58CMT2,
                        COALESCE(BIZ58IGLN,'0') AS BIZ58IGLN,
                        RDKCOO,
                        RDDOCO,
                        RDDCTO,
                        RDLNID,
                        RDDOC1,
                        RDXDCTO,
                        RDDOC2,
                        RDPKCO,
                        RDXCTO,
                        RDLNIC,
                        RDZ58SD,
                        RDZ58ROW,
                        RDZ58STK,
                        RDZ58SN ,
                        RDZ58BXRO,
                        RDZ58PS,
                        RDZ58SL,
                        RDITM,
                        RDZ58MDT,
                        BIZ58L1R,
                        BIZ58L1SLN,
                        BIZ58L2R,
                        BIZ58L2SLN,
                        BIZ58L3R,
                        BIZ58L3SLN,               
                        CAST((RDPQOH / 1000) AS DECIMAL(15,3)) AS RDPQOH,
                        COALESCE(CMCFGID, 0) AS CMCFGID
                    FROM
                        F580055
                        INNER JOIN F580056
                        ON RDZ58RID = RHZ58RID
                        LEFT OUTER JOIN F4201
                        ON  SHDOCO = RDDOCO AND
                            SHDCTO = RDDCTO AND
                            SHKCOO = RDKCOO
                        LEFT OUTER JOIN F4211
                        ON  SDDOCO = RDDOCO AND
                            SDLNID = RDLNID AND
                            SDDCTO = RDDCTO AND
                            SDKCOO = RDKCOO
                        LEFT OUTER JOIN F580032
                        ON  ATDOCO = RDDOCO AND
                            ATLNID = RDLNID AND
                            ATDCTO = RDDCTO AND
                            ATKCOO = RDKCOO AND
                            ATDOCO > 0
                        LEFT OUTER JOIN F5834A11
                        ON  S5MCU = RDMCU AND
                            S5DOCO = RDDOC1
                        LEFT OUTER JOIN F5834A01
                        ON  S1MCU = S5MCU AND
                            S1Z58SCN = S5Z58SCN AND
                            S1Z58SCD = S5Z58SCD
                        LEFT OUTER JOIN F0101 AB1
                        ON  AB1.ABAN8 = RHAN8
                        LEFT OUTER JOIN F0101 AB2
                        ON  AB2.ABAN8 = RHSHAN
                        LEFT OUTER JOIN F580200 
                        ON  BIMCU = RDMCU AND
                            BIDOCO = RDDOC1 AND
                            BIZ58UNID = RDZ58SN AND
                            BIEDSP != 'D'
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                        ON  UD1.DRSY = '57' AND
                            UD1.DRRT = 'RK' AND
                            TRIM(UD1.DRKY) = TRIM(RHZ58RT)
                        LEFT OUTER JOIN F3201
                        ON  CMDOCO = SDDOCO AND
                            CMDCTO = SDDCTO AND
                            CMLNID = SDLNID AND
                            CMKCOO = SDKCOO AND
                            CMDOCO > 0
						LEFT OUTER JOIN F580061
						ON RTZ58RT = RHZ58RT AND
						    RTMCU = '' AND --RHMCU AND
                            RTAN8 = 0
                    WHERE
                        RHZ58RID = ?
                    ORDER BY
                        RDZ58SD,
                        RDZ58ROW,
                        RDZ58STK,
                        RDZ58SL,
                        RDZ58PS
                    "
            };
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 15;
            cmd.Parameters["RackId"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetRackTagItemArchive()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT * FROM (
                        SELECT 
                            HLZ58RID AS RHZ58RID,
                            HLMCU AS RHMCU,
                            HLAN8 AS RHAN8,
                            HLZ58HST AS RHZ58HST,
                            HLZ58RSDT AS RHZ58RSDT,
                            AB1.ABALPH AS AN8ALPH,
                            HLSHAN AS RHSHAN,
                            AB2.ABALPH AS SHANALPH,  
                            HLZ58RT AS RHZ58RT,
                            UD1.DRDL01 AS RHZ58RT_DESC,
					        RTZ58RSL,
                            RTZ58RTWT,
                            HLZ58BOX AS RHZ58BOX,
                            HLZ58GWTT AS RHZ58GWTT,
                            RTZ58RTWT AS RHZ58RTWT,
                            HLZ58SCN AS RHZ58SCN,
                            HLLOCN AS RHLOCN,
                            ATZ5899003,
                            ATZ58CP2,
                            ATZ58CP3,
                            ATZ58CP4,
                            ATZ58CSQNO,
                            ATZ58GSQNO,
                            ATZ5899009,
                            ATZ5899010,
                            ATZ5899011,
                            ATZ5899012,
                            ATZ58PT,
ATZ5800520,
ATZ5899010,
                            SHDRQJ,
                            SDDRQJ,
                            SDUORG,
                            SDVR01,
                            SDRCD,
                            S1Z58SCN,
                            S1Z58SFS,
                            S1Z58SCDDJ,
                            S1Z58SCTYP,
				S5Z58CMT1,
				S5Z58CMT2,
                            COALESCE(BIZ58IGLN,'0') AS BIZ58IGLN,
                            DLKCOO AS RDKCOO,
                            DLDOCO AS RDDOCO,
                            DLDCTO AS RDDCTO,
                            DLLNID AS RDLNID,
                            DLDOC1 AS RDDOC1,
                            DLXDCTO AS RDXDCTO,
                            DLDOC2 AS RDDOC2,
                            DLPKCO AS RDPKCO,
                            DLXCTO AS RDXCTO,
                            DLLNIC AS RDLNIC,
                            DLZ58SD AS RDZ58SD,
                            DLZ58ROW AS RDZ58ROW,
                            DLZ58STK AS RDZ58STK,
                            DLZ58SN AS RDZ58SN,
                            DLZ58BXRO AS RDZ58BXRO,
                            DLZ58PS AS RDZ58PS,
                            DLZ58SL AS RDZ58SL,
                            DLITM AS RDITM,
                            DLZ58MDT,
                            BIZ58L1R,
                            BIZ58L1SLN,
                            BIZ58L2R,
                            BIZ58L2SLN,
                            BIZ58L3R,
                            BIZ58L3SLN,               
							AMZ58AID, 
                            CAST((DLPQOH / 1000) AS DECIMAL(15,3)) AS RDPQOH,
                            COALESCE(CMCFGID, 0) AS CMCFGID,
        					ROW_NUMBER() OVER ( PARTITION BY HLZ58RID  ORDER BY HLUPMJ DESC, HLTDAY DESC ) AS RowNum
                        FROM
                            F580559
                            INNER JOIN F580569
                            ON	DLZ58RID = HLZ58RID AND
						        DLZ58DST = HLZ58HST AND
						        DLMCU = HLMCU
                            LEFT OUTER JOIN F4201
                            ON  SHDOCO = DLDOCO AND
                                SHDCTO = DLDCTO AND
                                SHKCOO = DLKCOO
                            LEFT OUTER JOIN F4211
                            ON  SDDOCO = DLDOCO AND
                                SDLNID = DLLNID AND
                                SDDCTO = DLDCTO AND
                                SDKCOO = DLKCOO
                            LEFT OUTER JOIN F580032
                            ON  ATDOCO = DLDOCO AND
                                ATLNID = DLLNID AND
                                ATDCTO = DLDCTO AND
                                ATKCOO = DLKCOO AND
                                ATDOCO > 0
                            LEFT OUTER JOIN F5834A11
                            ON  S5MCU = DLMCU AND
                                S5DOCO = DLDOC1
                            LEFT OUTER JOIN F5834A01
                            ON  S1MCU = S5MCU AND
                                S1Z58SCN = S5Z58SCN AND
                                S1Z58SCD = S5Z58SCD
                            LEFT OUTER JOIN F0101 AB1
                            ON  AB1.ABAN8 = HLAN8
                            LEFT OUTER JOIN F0101 AB2
                            ON  AB2.ABAN8 = HLSHAN
                            LEFT OUTER JOIN F580200 
                            ON  BIMCU = DLMCU AND
                                BIDOCO = DLDOC1 AND
                                BIZ58UNID = DLZ58SN AND
                                BIEDSP != 'D'
                            LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                            ON  UD1.DRSY = '57' AND
                                UD1.DRRT = 'RK' AND
                                TRIM(UD1.DRKY) = TRIM(HLZ58RT)
                            LEFT OUTER JOIN F3201
                            ON  CMDOCO = SDDOCO AND
                                CMDCTO = SDDCTO AND
                                CMLNID = SDLNID AND
                                CMKCOO = SDKCOO AND
                                CMDOCO > 0
					        LEFT OUTER JOIN F580061
					        ON RTZ58RT = HLZ58RT AND
						        RTMCU = '' AND --HLMCU AND 
                                RTAN8 = 0
					        LEFT OUTER JOIN F581201
					        ON AMZ58RID = HLZ58RID
                        WHERE
                            HLZ58RID = ? AND
					        HLMCU = ? 
                        ORDER BY
                            DLZ58SD,
                            DLZ58ROW,
                            DLZ58STK,
                            DLZ58SL,
                            DLZ58PS
                    ) X WHERE RowNum = 1
                    "
            };
            cmd.Parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RID"].Precision = 15;
            cmd.Parameters["RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        /**************************************************************
         *  EDI Messages
         * ************************************************************/
        public iDB2Command GetEdiMessages()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        ZDEDTY,
                        ZDEDSQ,
                        ZDLINS,
                        ZDTXLN
                    FROM
                        F4715 
                    WHERE
                        ZDKCOO = ? AND
                        ZDDOCO = ? AND
                        ZDDCTO = ? AND
                        ZDLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 9;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        /**************************************************************
         *  Original Customer PO Line
         * ************************************************************/
        public iDB2Command GetRelatedOrderInfo()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	                SD2.SDVR01,
                    RO.ROEDTR
                FROM 
	                F4211 SD1 
	                INNER JOIN F580400 RO 
	                ON	RO.RODOC1 = SD1.SDRORN AND 
		                RO.RORCTO = SD1.SDRCTO AND 
		                RO.RORLLN = SD1.SDRLLN AND 
		                RO.RORKCO = SD1.SDRKCO
	                INNER JOIN F4211 SD2 
	                ON	RO.RODOCO = SD2.SDDOCO AND 
		                RO.RODCTO = SD2.SDDCTO AND 
		                RO.ROLNID = SD2.SDLNID AND 
		                RO.ROKCOO = SD2.SDKCOO
                WHERE	                
                    SD1.SDKCOO = ? AND
	                SD1.SDDOCO = ? AND
	                SD1.SDDCTO = ? AND
	                SD1.SDLNID = ? "
            };
            cmd.Parameters.Add(new iDB2Parameter("KCOO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("LNID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LNID"].Precision = 6;
            cmd.Parameters["LNID"].Scale = 0;
            return cmd;
        }

        /**************************************************************
         *  Original Customer PO Line
         * ************************************************************/
        public iDB2Command GetCoatedWorkCenter()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	               COUNT(*) AS RECS
                FROM
	                F3112 
                WHERE	                
                    WLDOCO = ? AND
	                WLOPSQ IN (2700, 4200)"
            };
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetMfgPlant()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT * FROM (
                        SELECT 
                            RHZ58RID,
		                    RDLOT1,
                            RDZ58SN,
							RDMCU,
                            RHMCU,
							RHZ58RT,
		                    HLMCU,
							HLZ58RT,
							HLIOPR,
							DLMCU,
							DLLOT1,
							DLZ58SN,
                            ROW_NUMBER() OVER ( PARTITION BY DLZ58RID  ORDER BY DLUPMJ ASC, DLTDAY ASC ) AS RowNum
                            --ROW_NUMBER() OVER ( PARTITION BY HLZ58RID  ORDER BY HLUPMJ ASC, HLTDAY ASC ) AS RowNum
                        FROM
                            F580055
                            INNER JOIN F580056
			                    ON RDZ58RID = RHZ58RID
                            INNER JOIN F580559
			                    ON HLZ58RID = RHZ58RID
                            INNER JOIN F580569
			                    ON DLZ58RID = RHZ58RID
								AND DLZ58SN = RDZ58SN
                        WHERE
                            RDZ58SN = ?
                    ) x WHERE RowNum = 1
                    "
            };
            cmd.Parameters.Add(new iDB2Parameter("SerialNum", iDB2DbType.iDB2Graphic, 30));
            return cmd;
        }

        public (string, decimal) GetMfgPlant(BusinessSystemSession e1Session, string serialNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"Serial Number: {serialNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            string currentPlant = "";
            decimal rackId = 0;
            try
            {
                // Mfg (First) Plant
                var cmd = GetMfgPlant();
                cmd.Parameters[0].Value = serialNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader0 = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader0.Read())
                    {
                        currentPlant = CastDbNull.To(reader0["DLMCU"], string.Empty).Trim();
                        rackId = CastDbNull.To<decimal>(reader0["RHZ58RID"], 0);
                    }

                    reader0.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return (currentPlant, rackId);
        }

        /**************************************************************
         *  Current Plant
         * ************************************************************/
        public string GetCurrentPlant(BusinessSystemSession e1Session, decimal rackId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"RackId: {rackId} BEGIN: {DateTime.Now.ToLocalTime()}");
            string currentPlant = "";
            try
            {
                // Mfg (First) Plant
                var cmd = GetCurrentPlant();
                cmd.Parameters[0].Value = rackId;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        currentPlant = CastDbNull.To(reader["RHMCU"], string.Empty).Trim();
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return currentPlant;
        }

        public iDB2Command GetCurrentPlant()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        RHMCU
                    FROM
                        F580055 
                    WHERE
                        RHZ58RID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RID"].Precision = 15;
            cmd.Parameters["RID"].Scale = 0;
            return cmd;
        }

        private static SqlConnection GetSFSqlConnection(string plantCode)
        {
            var cnSF = new SqlConnection();
            //if (plantCode != "0")
            //{
                try
                {
                    cnSF = ConnectionManager.GetSqlConnectionFromKey(plantCode + ".SHOPFLOOR", false);
                    cnSF.Open();
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message);
                }
            //}
            return cnSF;
        }

        private static SqlConnection GetDWSqlConnection()
        {
            var cnDW = new SqlConnection();
                try
                {
                    cnDW = ConnectionManager.GetSqlConnectionFromKey("DW", false);
                    cnDW.Open();
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {cnDW.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException(cnDW.ConnectionString + "  " + e.Message);
                }
            return cnDW;
        }

        /******************************************************************
        * Grouping Strategies
        ******************************************************************/
    }
}