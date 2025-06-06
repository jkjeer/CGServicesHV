using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class MTSIncomingOrderLines : List<MTSIncomingOrderLine>
    {
    }

    [DataContract(Namespace = "")]
    public class MTSIncomingOrderLine
    {
        public MTSIncomingOrderLine()
        {
            E1Schedule = 0;
            CustNum = 0;
            CustName = "";
            DisplayCustomer = "";
            WOrderNum = 0;
            WOrderType = "";
            SOrderNum = 0;
            SOrderLine = 0;
            SOrderType = "";
            ItemNum = "";
            Construction = "";
            Dimensions = "";
            SerialNumb = "";
            RackType = "";
            RackNum = 0;
            Lite1_Thick = 0;
            Lite1_Color = "";
            Lite1_HeatTreatment = "";
            Lite1_1stSrfCoat = "";
            Lite1_2ndSrfCoat = "";
            ShapeNum = 0;
            BaseL = 0;
            BaseN = 0;
            BaseD = 0;
            BaseMM = 0;
            LeftL = 0;
            LeftN = 0;
            LeftD = 0;
            LeftMM = 0;
            RightL = 0;
            RightN = 0;
            RightD = 0;
            RightMM = 0;
            TopL = 0;
            TopN = 0;
            TopD = 0;
            TopMM = 0;
            Side1L = 0;
            Side1N = 0;
            Side1D = 0;
            Side1MM = 0;
            Side2L = 0;
            Side2N = 0;
            Side2D = 0;
            Side2MM = 0;
            ShortL = 0;
            ShortN = 0;
            ShortD = 0;
            ShortMM = 0;
            LongL = 0;
            LongN = 0;
            LongD = 0;
            LongMM = 0;
            EdgeDelete = "";
            EdgeDeleteWidth = "";
            DueDate = new DateTime(1900, 1, 1);
            Priority = 0;
            OrderQty = 0;
            CaseQty = 0;
            MaxRackQty = 0;
            MinRackQty = 0;
            Status = "";
            HeldCode = "";
            QueueDateTime = new DateTime(1900, 1, 1);
            FirmSchedule = 0;
            FirmDateTime = new DateTime(1900, 1, 1);
            SchedulerID = "";
            ProducedDateTime = new DateTime(1900, 1, 1);
            Message1 = "";
            Message2 = "";
            SalesMsg = "";
            SalesMsg2 = "";
            ProdType = "";
            RoutingCode = "";
            WorkStation = "";
            Routed_WorkStation = "";
            RackOrientation = "";
            RackRows = 0;
            RackTiers = 0;
            SqftActual = 0;
            SqftBillable = 0;
            LogoPlacement = "";
            CustomerShipTo = 0;
            TinInTinOut = "";
            E1BatchID = "";
            CustPrtNum2 = "";
            CustPrtNum3 = "";
            CustPrtNum4 = "";
            S7MCU = "";
            E1SerialNumb = "";
            CustomerPO = "";
            LitesPerRack = 0;
            AnsiiLogoType = "";
            OrigCustNumSoldTo = 0;
            OrigCustNameSoldTo = "";
            Lite1_Pattern = "";
            TaglessLogo = "";
            RemakeFlag = "";
            GlassWeightSqftLbs = 0;
            RawItem = "";
            Comment1 = "";
            Comment2 = "";
        }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public decimal E1Schedule { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public decimal CustNum { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string CustName { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false)]
        public decimal WOrderNum { get; set; }

        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string WOrderType { get; set; }

        [DataMember(Order = 7, EmitDefaultValue = false)]
        public decimal SOrderNum { get; set; }

        [DataMember(Order = 8, EmitDefaultValue = false)]
        public decimal SOrderLine { get; set; }

        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string SOrderType { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string ItemNum { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string Construction { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string Dimensions { get; set; }

        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string SerialNumb { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string RackType { get; set; }

        [DataMember(Order = 15, EmitDefaultValue = false)]
        public decimal RackNum { get; set; }

        [DataMember(Order = 16, EmitDefaultValue = false)]
        public decimal Lite1_Thick { get; set; }

        [DataMember(Order = 17, EmitDefaultValue = false)]
        public string Lite1_Color { get; set; }

        [DataMember(Order = 18, EmitDefaultValue = false)]
        public string Lite1_HeatTreatment { get; set; }

        [DataMember(Order = 19, EmitDefaultValue = false)]
        public string Lite1_1stSrfCoat { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Lite1_2ndSrfCoat { get; set; }

        [DataMember(Order = 21, EmitDefaultValue = false)]
        public decimal ShapeNum { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public decimal BaseL { get; set; }

        [DataMember(Order = 23, EmitDefaultValue = false)]
        public decimal BaseN { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public decimal BaseD { get; set; }

        [DataMember(Order = 25, EmitDefaultValue = false)]
        public decimal BaseMM { get; set; }

        [DataMember(Order = 26, EmitDefaultValue = false)]
        public decimal LeftL { get; set; }

        [DataMember(Order = 27, EmitDefaultValue = false)]
        public decimal LeftN { get; set; }

        [DataMember(Order = 28, EmitDefaultValue = false)]
        public decimal LeftD { get; set; }

        [DataMember(Order = 29, EmitDefaultValue = false)]
        public decimal LeftMM { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal RightL { get; set; }

        [DataMember(Order = 31, EmitDefaultValue = false)]
        public decimal RightN { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public decimal RightD { get; set; }

        [DataMember(Order = 33, EmitDefaultValue = false)]
        public decimal RightMM { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public decimal TopL { get; set; }

        [DataMember(Order = 35, EmitDefaultValue = false)]
        public decimal TopN { get; set; }

        [DataMember(Order = 36, EmitDefaultValue = false)]
        public decimal TopD { get; set; }

        [DataMember(Order = 37, EmitDefaultValue = false)]
        public decimal TopMM { get; set; }

        [DataMember(Order = 38, EmitDefaultValue = false)]
        public decimal Side1L { get; set; }

        [DataMember(Order = 39, EmitDefaultValue = false)]
        public decimal Side1N { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal Side1D { get; set; }

        [DataMember(Order = 41, EmitDefaultValue = false)]
        public decimal Side1MM { get; set; }

        [DataMember(Order = 42, EmitDefaultValue = false)]
        public decimal Side2L { get; set; }

        [DataMember(Order = 43, EmitDefaultValue = false)]
        public decimal Side2N { get; set; }

        [DataMember(Order = 44, EmitDefaultValue = false)]
        public decimal Side2D { get; set; }

        [DataMember(Order = 45, EmitDefaultValue = false)]
        public decimal Side2MM { get; set; }

        [DataMember(Order = 46, EmitDefaultValue = false)]
        public decimal ShortL { get; set; }

        [DataMember(Order = 47, EmitDefaultValue = false)]
        public decimal ShortN { get; set; }

        [DataMember(Order = 48, EmitDefaultValue = false)]
        public decimal ShortD { get; set; }

        [DataMember(Order = 49, EmitDefaultValue = false)]
        public decimal ShortMM { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal LongL { get; set; }

        [DataMember(Order = 51, EmitDefaultValue = false)]
        public decimal LongN { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public decimal LongD { get; set; }

        [DataMember(Order = 53, EmitDefaultValue = false)]
        public decimal LongMM { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public string EdgeDelete { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public string EdgeDeleteWidth { get; set; }

        [DataMember(Order = 55, EmitDefaultValue = false)]
        public DateTime DueDate { get; set; }

        [DataMember(Order = 56, EmitDefaultValue = false)]
        public decimal Priority { get; set; }

        [DataMember(Order = 57, EmitDefaultValue = false)]
        public decimal OrderQty { get; set; }

        [DataMember(Order = 58, EmitDefaultValue = false)]
        public decimal CaseQty { get; set; }

        [DataMember(Order = 59, EmitDefaultValue = false)]
        public decimal MaxRackQty { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal MinRackQty { get; set; }

        [DataMember(Order = 61, EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = false)]
        public string HeldCode { get; set; }

        [DataMember(Order = 63, EmitDefaultValue = false)]
        public DateTime QueueDateTime { get; set; }

        [DataMember(Order = 64, EmitDefaultValue = false)]
        public decimal FirmSchedule { get; set; }

        [DataMember(Order = 65, EmitDefaultValue = false)]
        public DateTime FirmDateTime { get; set; }

        [DataMember(Order = 66, EmitDefaultValue = false)]
        public string SchedulerID { get; set; }

        [DataMember(Order = 67, EmitDefaultValue = false)]
        public DateTime ProducedDateTime { get; set; }

        [DataMember(Order = 68, EmitDefaultValue = false)]
        public string Message1 { get; set; }

        [DataMember(Order = 69, EmitDefaultValue = false)]
        public string Message2 { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string SalesMsg { get; set; }

        [DataMember(Order = 71, EmitDefaultValue = false)]
        public string SalesMsg2 { get; set; }

        [DataMember(Order = 72, EmitDefaultValue = false)]
        public string ProdType { get; set; }

        [DataMember(Order = 73, EmitDefaultValue = false)]
        public string RoutingCode { get; set; }

        [DataMember(Order = 74, EmitDefaultValue = false)]
        public string WorkStation { get; set; }

        [DataMember(Order = 75, EmitDefaultValue = false)]
        public string Routed_WorkStation { get; set; }

        [DataMember(Order = 76, EmitDefaultValue = false)]
        public string RackOrientation { get; set; }

        [DataMember(Order = 77, EmitDefaultValue = false)]
        public decimal RackRows { get; set; }

        [DataMember(Order = 78, EmitDefaultValue = false)]
        public decimal RackTiers { get; set; }

        [DataMember(Order = 79, EmitDefaultValue = false)]
        public decimal SqftActual { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public decimal SqftBillable { get; set; }

        [DataMember(Order = 81, EmitDefaultValue = false)]
        public string LogoPlacement { get; set; }

        [DataMember(Order = 82, EmitDefaultValue = false)]
        public decimal CustomerShipTo { get; set; }

        [DataMember(Order = 83, EmitDefaultValue = false)]
        public string TinInTinOut { get; set; }

        [DataMember(Order = 84, EmitDefaultValue = false)]
        public string E1BatchID { get; set; }

        [DataMember(Order = 85, EmitDefaultValue = false)]
        public string CustPrtNum2 { get; set; }

        [DataMember(Order = 86, EmitDefaultValue = false)]
        public string CustPrtNum3 { get; set; }

        [DataMember(Order = 87, EmitDefaultValue = false)]
        public string CustPrtNum4 { get; set; }

        [DataMember(Order = 88, EmitDefaultValue = false)]
        public string S7MCU { get; set; }

        [DataMember(Order = 89, EmitDefaultValue = false)]
        public string E1SerialNumb { get; set; }

        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string CustomerPO { get; set; }

        [DataMember(Order = 91, EmitDefaultValue = false)]
        public decimal LitesPerRack { get; set; }

        [DataMember(Order = 92, EmitDefaultValue = false)]
        public string AnsiiLogoType { get; set; }

        [DataMember(Order = 93, EmitDefaultValue = false)]
        public decimal OrigCustNumSoldTo { get; set; }

        [DataMember(Order = 93, EmitDefaultValue = false)]
        public string OrigCustNameSoldTo { get; set; }

        [DataMember(Order = 94, EmitDefaultValue = false)]
        public string Lite1_Pattern { get; set; }

        [DataMember(Order = 95, EmitDefaultValue = false)]
        public string TaglessLogo { get; set; }

        [DataMember(Order = 96, EmitDefaultValue = false)]
        public string RemakeFlag { get; set; }

        [DataMember(Order = 97, EmitDefaultValue = false)]
        public string DisplayCustomer { get; set; }

        [DataMember(Order = 364, EmitDefaultValue = false)]
        public decimal GlassWeightSqftLbs { get; set; }
        [DataMember(Order = 98, EmitDefaultValue = false)]
        public string RawItem { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string Comment1 { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string Comment2 { get; set; }
    }
}