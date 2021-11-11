using System;
using System.Data;
using System.ServiceModel;
using NLog;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class HVQueTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static class Tmp<T>
        {
            [ThreadStatic]
            public static T Value;
        }

        public HVQueTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[HVQueLedger]");
            //Columns.Add("HVQueLedgerID", typeof(int));
            Columns.Add("E1Schedule", typeof(decimal));
            Columns.Add("CustNum", typeof(decimal));
            Columns.Add("CustName", typeof(string));
            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("WOrderType", typeof(string));
            Columns.Add("SOrderNum", typeof(decimal));
            Columns.Add("SOrderLine", typeof(decimal));
            Columns.Add("SOrderType", typeof(string));
            Columns.Add("ItemNum", typeof(string));
            Columns.Add("Construction", typeof(string));
            Columns.Add("Dimensions", typeof(string));
            Columns.Add("SerialNumb", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("RackNum", typeof(decimal));
            Columns.Add("Lite1_Thick", typeof(decimal));
            Columns.Add("Lite1_Color", typeof(string));
            Columns.Add("Lite1_HeatTreatment", typeof(string));
            Columns.Add("Lite1_1stSrfCoat", typeof(string));
            Columns.Add("Lite1_2ndSrfCoat", typeof(string));
            Columns.Add("ShapeNum", typeof(decimal));
            Columns.Add("BaseL", typeof(decimal));
            Columns.Add("BaseN", typeof(decimal));
            Columns.Add("BaseD", typeof(decimal));
            Columns.Add("BaseMM", typeof(decimal));
            Columns.Add("LeftL", typeof(decimal));
            Columns.Add("LeftN", typeof(decimal));
            Columns.Add("LeftD", typeof(decimal));
            Columns.Add("LeftMM", typeof(decimal));
            Columns.Add("RightL", typeof(decimal));
            Columns.Add("RightN", typeof(decimal));
            Columns.Add("RightD", typeof(decimal));
            Columns.Add("RightMM", typeof(decimal));
            Columns.Add("TopL", typeof(decimal));
            Columns.Add("TopN", typeof(decimal));
            Columns.Add("TopD", typeof(decimal));
            Columns.Add("TopMM", typeof(decimal));
            Columns.Add("Side1L", typeof(decimal));
            Columns.Add("Side1N", typeof(decimal));
            Columns.Add("Side1D", typeof(decimal));
            Columns.Add("Side1MM", typeof(decimal));
            Columns.Add("Side2L", typeof(decimal));
            Columns.Add("Side2N", typeof(decimal));
            Columns.Add("Side2D", typeof(decimal));
            Columns.Add("Side2MM", typeof(decimal));
            Columns.Add("ShortL", typeof(decimal));
            Columns.Add("ShortN", typeof(decimal));
            Columns.Add("ShortD", typeof(decimal));
            Columns.Add("ShortMM", typeof(decimal));
            Columns.Add("LongL", typeof(decimal));
            Columns.Add("LongN", typeof(decimal));
            Columns.Add("LongD", typeof(decimal));
            Columns.Add("LongMM", typeof(decimal));
            Columns.Add("EdgeDelete", typeof(string));
            Columns.Add("DueDate", typeof(DateTime));
            Columns.Add("Priority", typeof(decimal));
            Columns.Add("OrderQty", typeof(decimal));
            Columns.Add("CaseQty", typeof(decimal));
            Columns.Add("MaxRackQty", typeof(decimal));
            Columns.Add("MinRackQty", typeof(decimal));
            Columns.Add("Status", typeof(string));
            Columns.Add("HeldCode", typeof(string));
            Columns.Add("QueueDateTime", typeof(DateTime));
            Columns.Add("FirmSchedule", typeof(decimal));
            Columns.Add("FirmDateTime", typeof(DateTime));
            Columns.Add("SchedulerID", typeof(string));
            Columns.Add("ProducedDateTime", typeof(DateTime));
            Columns.Add("Message1", typeof(string));
            Columns.Add("Message2", typeof(string));
            Columns.Add("SalesMsg", typeof(string));
            Columns.Add("SalesMsg2", typeof(string));
            Columns.Add("ProdType", typeof(string));
            Columns.Add("RoutingCode", typeof(string));
            Columns.Add("WorkStation", typeof(string));
            Columns.Add("Routed_WorkStation", typeof(string));
            Columns.Add("RackOrientation", typeof(string));
            Columns.Add("RackRows", typeof(decimal));
            Columns.Add("RackTiers", typeof(decimal));
            Columns.Add("SqftActual", typeof(decimal));
            Columns.Add("SqftBillable", typeof(decimal));
            Columns.Add("LogoPlacement", typeof(string));
            Columns.Add("CustomerShipTo", typeof(decimal));
            Columns.Add("TinInTinOut", typeof(string));
            Columns.Add("E1BatchID", typeof(string));
            Columns.Add("CustPrtNum2", typeof(string));
            Columns.Add("CustPrtNum3", typeof(string));
            Columns.Add("CustPrtNum4", typeof(string));
            Columns.Add("S7MCU", typeof(string));
            Columns.Add("E1SerialNumb", typeof(string));
            Columns.Add("CustomerPO", typeof(string));
            Columns.Add("LitesPerRack", typeof(decimal));
            Columns.Add("AnsiiLogoType", typeof(string));
            Columns.Add("OrigCustNumSoldTo", typeof(decimal));
            Columns.Add("OrigCustNameSoldTo", typeof(string));
            Columns.Add("Lite1_Pattern", typeof(string));
            Columns.Add("TaglessLogo", typeof(string));
            Columns.Add("RawItem", typeof(string));
            Columns.Add("GlassWeightSqftLbs", typeof(decimal));
            Columns.Add("EdgeDeleteWidth", typeof(string));
            //Columns.Add("RemakeFlag", typeof(string));
        }

        public void Add(MTSIncomingOrderLines incomingOrderLines)
        {
            try
            {
                foreach (var incomingOrderLine in incomingOrderLines)
                {
                    var dr = NewRow();
                    //dr["HVQueLedgerID"] = incomingOrderLine.HVQueLedgerID;
                    dr["E1Schedule"] = incomingOrderLine.E1Schedule;
                    dr["CustNum"] = incomingOrderLine.CustNum;
                    dr["CustName"] = incomingOrderLine.CustName;
                    dr["WOrderNum"] = incomingOrderLine.WOrderNum;
                    dr["WOrderType"] = incomingOrderLine.WOrderType;
                    dr["SOrderNum"] = incomingOrderLine.SOrderNum;
                    dr["SOrderLine"] = incomingOrderLine.SOrderLine;
                    dr["SOrderType"] = incomingOrderLine.SOrderType;
                    dr["ItemNum"] = incomingOrderLine.ItemNum;
                    dr["Construction"] = incomingOrderLine.Construction;
                    dr["Dimensions"] = incomingOrderLine.Dimensions;
                    dr["SerialNumb"] = incomingOrderLine.SerialNumb;
                    dr["RackType"] = incomingOrderLine.RackType;
                    dr["RackNum"] = incomingOrderLine.RackNum;
                    dr["Lite1_Thick"] = incomingOrderLine.Lite1_Thick;
                    dr["Lite1_Color"] = incomingOrderLine.Lite1_Color;
                    dr["Lite1_HeatTreatment"] = incomingOrderLine.Lite1_HeatTreatment;
                    dr["Lite1_1stSrfCoat"] = incomingOrderLine.Lite1_1stSrfCoat;
                    dr["Lite1_2ndSrfCoat"] = incomingOrderLine.Lite1_2ndSrfCoat;
                    dr["ShapeNum"] = incomingOrderLine.ShapeNum;
                    dr["BaseL"] = incomingOrderLine.BaseL;
                    dr["BaseN"] = incomingOrderLine.BaseN;
                    dr["BaseD"] = incomingOrderLine.BaseD;
                    dr["BaseMM"] = incomingOrderLine.BaseMM;
                    dr["LeftL"] = incomingOrderLine.LeftL;
                    dr["LeftN"] = incomingOrderLine.LeftN;
                    dr["LeftD"] = incomingOrderLine.LeftD;
                    dr["LeftMM"] = incomingOrderLine.LeftMM;
                    dr["RightL"] = incomingOrderLine.RightL;
                    dr["RightN"] = incomingOrderLine.RightN;
                    dr["RightD"] = incomingOrderLine.RightD;
                    dr["RightMM"] = incomingOrderLine.RightMM;
                    dr["TopL"] = incomingOrderLine.TopL;
                    dr["TopN"] = incomingOrderLine.TopN;
                    dr["TopD"] = incomingOrderLine.TopD;
                    dr["TopMM"] = incomingOrderLine.TopMM;
                    dr["Side1L"] = incomingOrderLine.Side1L;
                    dr["Side1N"] = incomingOrderLine.Side1N;
                    dr["Side1D"] = incomingOrderLine.Side1D;
                    dr["Side1MM"] = incomingOrderLine.Side1MM;
                    dr["Side2L"] = incomingOrderLine.Side2L;
                    dr["Side2N"] = incomingOrderLine.Side2N;
                    dr["Side2D"] = incomingOrderLine.Side2D;
                    dr["Side2MM"] = incomingOrderLine.Side2MM;
                    dr["ShortL"] = incomingOrderLine.ShortL;
                    dr["ShortN"] = incomingOrderLine.ShortN;
                    dr["ShortD"] = incomingOrderLine.ShortD;
                    dr["ShortMM"] = incomingOrderLine.ShortMM;
                    dr["LongL"] = incomingOrderLine.LongL;
                    dr["LongN"] = incomingOrderLine.LongN;
                    dr["LongD"] = incomingOrderLine.LongD;
                    dr["LongMM"] = incomingOrderLine.LongMM;
                    dr["EdgeDelete"] = incomingOrderLine.EdgeDelete;
                    dr["DueDate"] = incomingOrderLine.DueDate;
                    dr["Priority"] = incomingOrderLine.Priority;
                    dr["OrderQty"] = incomingOrderLine.OrderQty;
                    dr["CaseQty"] = incomingOrderLine.CaseQty;
                    dr["MaxRackQty"] = incomingOrderLine.MaxRackQty;
                    dr["MinRackQty"] = incomingOrderLine.MinRackQty;
                    dr["Status"] = incomingOrderLine.Status;
                    dr["HeldCode"] = incomingOrderLine.HeldCode;
                    dr["QueueDateTime"] = incomingOrderLine.QueueDateTime;
                    dr["FirmSchedule"] = incomingOrderLine.FirmSchedule;
                    dr["FirmDateTime"] = incomingOrderLine.FirmDateTime;
                    dr["SchedulerID"] = incomingOrderLine.SchedulerID;
                    dr["ProducedDateTime"] = incomingOrderLine.ProducedDateTime;
                    dr["Message1"] = incomingOrderLine.Message1;
                    dr["Message2"] = incomingOrderLine.Message2;
                    dr["SalesMsg"] = incomingOrderLine.SalesMsg;
                    dr["SalesMsg2"] = incomingOrderLine.SalesMsg2;
                    dr["ProdType"] = incomingOrderLine.ProdType;
                    dr["RoutingCode"] = incomingOrderLine.RoutingCode;
                    dr["WorkStation"] = incomingOrderLine.WorkStation;
                    dr["Routed_WorkStation"] = incomingOrderLine.Routed_WorkStation;
                    dr["RackOrientation"] = incomingOrderLine.RackOrientation;
                    dr["RackRows"] = incomingOrderLine.RackRows;
                    dr["RackTiers"] = incomingOrderLine.RackTiers;
                    dr["SqftActual"] = incomingOrderLine.SqftActual;
                    dr["SqftBillable"] = incomingOrderLine.SqftBillable;
                    dr["LogoPlacement"] = incomingOrderLine.LogoPlacement;
                    dr["CustomerShipTo"] = incomingOrderLine.CustomerShipTo;
                    dr["TinInTinOut"] = incomingOrderLine.TinInTinOut;
                    dr["E1BatchID"] = incomingOrderLine.E1BatchID;
                    dr["CustPrtNum2"] = incomingOrderLine.CustPrtNum2;
                    dr["CustPrtNum3"] = incomingOrderLine.CustPrtNum3;
                    dr["CustPrtNum4"] = incomingOrderLine.CustPrtNum4;
                    dr["S7MCU"] = incomingOrderLine.S7MCU;
                    dr["E1SerialNumb"] = incomingOrderLine.E1SerialNumb;
                    dr["CustomerPO"] = incomingOrderLine.CustomerPO;
                    dr["LitesPerRack"] = incomingOrderLine.LitesPerRack; 
                    dr["AnsiiLogoType"] = incomingOrderLine.AnsiiLogoType;
                    dr["OrigCustNumSoldTo"] = incomingOrderLine.OrigCustNumSoldTo;
                    dr["OrigCustNameSoldTo"] = incomingOrderLine.OrigCustNameSoldTo;
                    dr["Lite1_Pattern"] = incomingOrderLine.Lite1_Pattern;
                    dr["TaglessLogo"] = incomingOrderLine.TaglessLogo;
                    dr["RawItem"] = incomingOrderLine.RawItem;
                    dr["GlassWeightSqftLbs"] = incomingOrderLine.GlassWeightSqftLbs;
                    dr["EdgeDeleteWidth"] = incomingOrderLine.EdgeDeleteWidth;
                    //dr["RemakeFlag"] = incomingOrderLine.RemakeFlag;
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new FaultException(e.Message);
            }
        }
    }
}
