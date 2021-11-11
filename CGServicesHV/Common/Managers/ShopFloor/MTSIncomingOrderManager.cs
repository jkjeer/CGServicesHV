using System;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.ShopFloor;
using NLog;

namespace CGServicesHV.Common.Managers.ShopFloor
{
    public class MTSIncomingOrderManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static class Tmp<T>
        {
            [ThreadStatic]
            public static T Value;
        }

        public static MTSIncomingOrderLines BuildMTSIncomingOrderLines(BusinessSystemSession e1Session, E1ScheduleData e1ScheduleData)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var incomingOrderLines = new MTSIncomingOrderLines();
            try
            {
                var f3111Mgr = new F3111Manager(e1Session);
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var f580021 in e1ScheduleData.F580021s)
                {
                    var f580024 = e1ScheduleData.F580024s?.Find(item => item.S8EDBT == f580021.S7EDBT && item.S8EDLN == f580021.S7EDLN && item.S8Z58SCN == f580021.S7Z58SCN && item.S8Z58SCSN == f580021.S7Z58SCSN);
                    if (f580024 != null)
                    {
                        var incomingOrderLine = new MTSIncomingOrderLine
                        {
                            DueDate = Utilities.Common.CastToDateTime(f580021.S7RSDJ),
                            E1Schedule = f580021.S7Z58SCN,
                            CustNum = f580021.S7SHAN,
                            CustName = f580021.S7ALPH,
                            WOrderNum = f580021.S7DOCO,
                            WOrderType = f580021.S7DCTO,
                            SOrderNum = Convert.ToDecimal(f580021.S7RORN),
                            SOrderType = f580021.S7RCTO,
                            SOrderLine = f580021.S7LNID / 1000,
                            ItemNum = f580021.S7LITM,
                            Construction = f580021.S7DSC1,
                            Dimensions = f580021.S7DSC2,
                            Lite1_Thick = f580021.S7Z58L1TK / 10,
                            Lite1_Color = f580021.S7Z58L1CL,
                            Lite1_HeatTreatment = f580021.S7Z58L1HT,
                            Lite1_1stSrfCoat = f580021.S7Z58L11C,
                            Lite1_2ndSrfCoat = f580021.S7Z58L12C,
                            ShapeNum = int.TryParse(f580021.S7Z58SHAPE, out Tmp<int>.Value) ? Tmp<int>.Value : 0,
                            BaseL = f580021.S7Z58L1BDM,
                            BaseN = f580021.S7Z58L1BNM,
                            BaseD = f580021.S7Z58L1BDN,
                            BaseMM = f580021.S7Z581BMIM,
                            LeftL = f580021.S7Z58L1LDM,
                            LeftN = f580021.S7Z58L1LNM,
                            LeftD = f580021.S7Z58L1LDN,
                            LeftMM = f580021.S7Z581LMIM,
                            RightL = f580021.S7Z58L1RDM,
                            RightN = f580021.S7Z58L1RNM,
                            RightD = f580021.S7Z58L1RDN,
                            RightMM = f580021.S7Z581RMIM,
                            TopL = f580021.S7Z58L1TDM,
                            TopN = f580021.S7Z58L1TNM,
                            TopD = f580021.S7Z58L1TDN,
                            TopMM = f580021.S7Z581TMIM,
                            Side1L = f580021.S7Z58L11DM,
                            Side1N = f580021.S7Z58L11NM,
                            Side1D = f580021.S7Z58L11DN,
                            Side1MM = f580021.S7Z5811MIM,
                            Side2L = f580021.S7Z58L12DM,
                            Side2N = f580021.S7Z58L12NM,
                            Side2D = f580021.S7Z58L12DN,
                            Side2MM = f580021.S7Z5812MIM,
                            ShortL = 0,
                            ShortN = 0,
                            ShortD = 0,
                            ShortMM = 0,
                            LongL = 0,
                            LongN = 0,
                            LongD = 0,
                            LongMM = 0,
                            EdgeDelete = f580021.S7Z58L1EW,
                            Priority = 20,
                            OrderQty = f580021.S7Z58QTY,
                            MaxRackQty = f580021.S7Z58CRXQ,
                            MinRackQty = f580021.S7Z58CRMQ,
                            RackOrientation = f580024.S8Z58RPRVH,
                            RackRows = f580024.S8Z58ROW,
                            RackTiers = f580024.S8Z58STK,
                            SqftActual = f580024.S8Z58USF / 10000,
                            SqftBillable = f580024.S8Z58BSF / 10000,
                            Status = "OPEN",
                            HeldCode = "",
                            QueueDateTime = DateTime.Now.ToLocalTime(),
                            FirmSchedule = 0,
                            SchedulerID = f580021.S7USER,
                            Message1 = f580021.S7Z58MSG1,
                            Message2 = f580021.S7Z58MSG2,
                            SalesMsg = "", //f580024.S8Z58GAL1,
                            SalesMsg2 = "", //f580024.S8Z58GAL2,
                            ProdType = f580021.S7RORN.Equals("0") ? "MTS" : "MTO",
                            RoutingCode = f580021.S7Z58L1RG,
                            WorkStation = f580024.S8MMCU,
                            LogoPlacement = f580024.S8Z58RTNIO,
                            CustomerShipTo = f580021.S7AN8DR,
                            TinInTinOut = f580024.S8Z58RTNIO,
                            E1BatchID = f580021.S7EDBT,
                            CustPrtNum2 = f580021.S7Z58CP2,
                            CustPrtNum3 = f580021.S7Z58CP3,
                            CustPrtNum4 = f580021.S7Z58CP4,
                            S7MCU = f580021.S7MCU,
                            CustomerPO = f580021.S7VR01,
                            LitesPerRack = f580021.S7Z58LPR,
                            RackType = f580021.S7Z58RT,
                            AnsiiLogoType = f580021.S7Z58L1ALT,
                            OrigCustNumSoldTo = f580021.S7Z58CTL,
                            Lite1_Pattern = f580021.S7Z58L1PX,
                            TaglessLogo = f580024.S8Z5801621,
                            RemakeFlag = f580024.S8TRNT,
                            RawItem = f3111Mgr.GetRawItem(e1Session, f580021.S7MCU, f580021.S7DOCO),
                            GlassWeightSqftLbs = f580024.S8Z58GLWT / 1000,
                            EdgeDeleteWidth = f580024.S8Z5805680
                    };
                    incomingOrderLine.DisplayCustomer = incomingOrderLine.CustName.Length == 0 ? f580021.ABALPH : incomingOrderLine.CustName;
                    if (incomingOrderLine.S7MCU == "05") incomingOrderLine.TinInTinOut = (incomingOrderLine.TinInTinOut.ToUpper() == "Q") ? "Q" : "";
                    incomingOrderLines.Add(incomingOrderLine);
                    }
                    else
                    {
                        throw new ApplicationException("Missing F580024 at BuildMTSIncomingOrderLines");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return incomingOrderLines;
        }
    }
}
