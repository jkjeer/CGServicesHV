using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackTagItems : List<RackTagItem>
    {
        public DataTable GetDataTable()
        {
            var dt = new DataTable("RackTagItems");

            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(RackTagItem).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                dt.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                if (dt.Columns[propertyInfo.Name].DataType == typeof(DateTime))
                {
                    dt.Columns[propertyInfo.Name].DateTimeMode = DataSetDateTime.Unspecified;                   
                }
            }

            foreach (RackTagItem rackTag in this)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    dr[propertyInfo.Name] = propertyInfo.GetValue(rackTag, null);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }

    [DataContract(Namespace = "")]
    public class RackTagItem
    {

        public RackTagItem()
        {
            RackId = 0;
            RackSide = "";
            RackRow = 0;
            RackStack = 0;
            RackSlot = 0;
            RackPosition = 0;
            RackSlotPosition = 0;
            GroupingKey = "";
            BeginSerialNum = "";
            EndSerialNum = "";
            CompanyNum = "";
            PlantNum = "";
            SalesOrderNum = 0;
            SalesOrderLineNum = 0;
            SalesOrderType = "";
            WorkOrderNum = 0;
            ScheduleNum = 0;
            ScheduleType = "";
            ItemNum = 0;
            Location = "";
            SoldToNum = 0;
            SoldToName = "";
            ShipToNum = 0;
            ShipToName = "";
            OrigSoldToNum = 0;
            OrigSoldToName = "";
            OrigShipToNum = 0;
            OrigShipToName = "";
            CustPartNum1 = "";
            CustPartNum2 = "";
            CustPartNum3 = "";
            CustSeqNum1 = "";
            GenericSeqNum = "";
            IGLine = "";
            ReasonCode = "";
            RemakeFlag = "";
            RackType = "";
            RackTypeDesc = "";
            Status = "";
            StatusDate = DateTime.Now.ToLocalTime();
            BoxRotated = "";
            BoxNum = 0;
            RackSlotsTotal = 0;
            IsEmptySlot = false;
            Lite1Rack = "";
            Lite1Slot = 0;
            Lite2Rack = "";
            Lite2Slot = 0;
            Lite3Rack = "";
            Lite3Slot = 0;
            RackGlassWeight = 0;
            RackTareWeight = 0;
            Quantity = 0;
            UnitThickness = 0;
            CustomerPO = "";
            OrigCustomerPO = "";
            ProductType = "";
            CardinalProductType = "";
            Comment1 = "";
            ConfigId = 0;
            CustomerLiteNumber = 0;
            Lite1Heat = "";
            Lite1Thickness = 0;
            Lite1FirstSrfCoat = "";
            Lite1SecondSrfCoat = "";
            Lite1PatTextSrf = "";
            Lite1Color = "";
            Lite1Pattern = "";
            Lite1LamiType = "";
            Lite1LamiDesc = "";
            Lite1LamiShopFloorDesc = "";
            Lite1TempLogo = "";
            Lite1Routing = "";
            Lite1VGroove = "";
            Lite1EdgeWork = "";
            Lite1AnsiLogo = "";
            IsCoated = false;
            Lite2Heat = "";
            Lite2Thickness = 0;
            Lite2ThirdSrfCoat = "";
            Lite2FourthSrfCoat = "";
            Lite2Color = "";
            Lite2Pattern = "";
            Lite2LamiType = "";
            Lite2LamiDesc = "";
            Lite2LamiShopFloorDesc = "";
            Lite2Routing = "";
            Lite2VGroove = "";
            Lite3Heat = "";
            Lite3Thickness = 0;
            Lite3FifthSrfCoat = "";
            Lite3SixthSrfCoat = "";
            Lite3Color = "";
            Lite3Pattern = "";
            Lite3LamiType = "";
            Lite3LamiDesc = "";
            Lite3LamiShopFloorDesc = "";
            Lite3VGroove = "";
            Lite3Routing = "";
            EdgeDeleteWidth = "";
            Spacer1Size = 0;
            Spacer1Type = "";
            Spacer2Size = 0;
            Spacer2Type = "";
            InteriorScoring = "";
            ExteriorScoring = "";
            InteriorPreserveCutback = "";
            InteriorPreserveColor = "";
            ExteriorPreserveCutback = "";
            ExteriorPreserveColor = "";
            BreatherTube1 = "";
            BreatherTubeUnitsCount = 0;
            ScoredUnitsCount = 0;
            PickItemNum = 0;
            PickedUnitsCount = 0;
            CrateType = "";
            ShopFloorMessage1 = "";
            ShopFloorMessage2 = "";
            PickNotes = "";
            PackNotes = "";
            CustomerDate = DateTime.Now.ToLocalTime();
            DueDate = DateTime.Now.ToLocalTime();
            DueDateString = "";
            ShapeNum = 0;
            Base = 0;
            Left = 0;
            Right = 0;
            Top = 0;
            S1 = 0;
            S2 = 0;
            FooterMsg1 = "";
            FooterMsg2 = "";
            FooterMsg3 = "";
            FooterMsg4 = "";
            FooterMsg5 = "";
            FooterMsg6 = "";
            LabelMsg1 = "";
            LabelMsg2 = "";
            LabelMsg3 = "";
            LabelMsg4 = "";
            LabelMsg5 = "";
            UnitWeight = 0;
            UnitActualSqft = 0;
            UnitBillableSqft = 0;
            DockLocal = "";
            HasMate = "";
            BaseMM = 0;
            LeftMM = 0;
            RightMM = 0;
            TopMM = 0;
            TinOut = "";
        }

        [DataMember(EmitDefaultValue = true)]
        public decimal RackId { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string RackSide { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal RackRow { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal RackStack { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal RackSlot { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal RackPosition { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal RackSlotPosition { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string GroupingKey { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 51)]
        public string LotNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string BeginSerialNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string EndSerialNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CompanyNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string PlantNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal SalesOrderNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal SalesOrderLineNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string SalesOrderType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal ScheduleNum { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 19)]
        public Decimal OriginalScheduleNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ScheduleType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal ItemNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Location { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal SoldToNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string SoldToName { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal ShipToNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ShipToName { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal OrigSoldToNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string OrigSoldToName { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal OrigShipToNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string OrigShipToName { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CustPartNum1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string CustPartNum2 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string CustPartNum3 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string CustSeqNum1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string GenericSeqNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string IGLine { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ReasonCode { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string RemakeFlag { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RackType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string RackTypeDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Status { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime StatusDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string BoxRotated { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal BoxNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal RackSlotsTotal { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool IsEmptySlot { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1Rack { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Lite1Slot { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2Rack { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Lite2Slot { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3Rack { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Lite3Slot { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public decimal RackGlassWeight { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal RackTareWeight { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int Quantity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal UnitThickness { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string CustomerPO { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string OrigCustomerPO { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ProductType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string CardinalProductType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Comment1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string CustomerSupplied { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal CustomerLiteNumber { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal ConfigId { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1Heat { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Lite1Thickness { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1FirstSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1SecondSrfCoat { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1PatTextSrf { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1Color { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1Pattern { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1LamiType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1LamiDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1LamiShopFloorDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1TempLogo { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1Routing { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1VGroove { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1EdgeWork { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite1AnsiLogo { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool IsCoated { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite2Heat { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Lite2Thickness { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2ThirdSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2FourthSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2Color { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2Pattern { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2LamiType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2LamiDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2LamiShopFloorDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite2Routing { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite2VGroove { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3Heat { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Lite3Thickness { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3FifthSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3SixthSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3Color { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3Pattern { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3LamiType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3LamiDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3LamiShopFloorDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3VGroove { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Lite3Routing { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string EdgeDeleteWidth { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal Spacer1Size { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Spacer1Type { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Spacer2Size { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Spacer2Type { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string InteriorScoring { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ExteriorScoring { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string InteriorPreserveCutback { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string InteriorPreserveColor { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ExteriorPreserveCutback { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ExteriorPreserveColor { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string BreatherTube1 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public int BreatherTubeUnitsCount { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int ScoredUnitsCount { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal PickItemNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int PickedUnitsCount { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CrateType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ShopFloorMessage1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string ShopFloorMessage2 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string PickNotes { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string PackNotes { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime CustomerDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime DueDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string DueDateString { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal ShapeNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Base { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Left { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Right { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Top { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal S1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal S2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string FooterMsg1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string FooterMsg2 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string FooterMsg3 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string FooterMsg4 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string FooterMsg5 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string FooterMsg6 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg7 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg8 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg9 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg10 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg11 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg12 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg13 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg14 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg15 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg16 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg17 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg18 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg19 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg20 { get; set; }
        public string FooterMsg21 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg22 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg23 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg24 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg25 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg26 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg27 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg28 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg29 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg30 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string LabelMsg1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LabelMsg2 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LabelMsg3 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LabelMsg4 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LabelMsg5 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg6 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg7 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg8 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg9 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg10 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg11 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg12 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg13 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg14 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg15 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg16 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg17 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg18 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg19 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg20 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg21 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg22 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg23 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg24 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg25 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg26 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg27 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg28 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg29 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg30 { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public decimal UnitWeight { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal UnitActualSqft { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal UnitBillableSqft { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string DockLocal { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string HasMate { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal BaseMM { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal LeftMM { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal RightMM { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal TopMM { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string TinOut { get; set; }
        //[DataMember(EmitDefaultValue = true)]
        //public decimal Top { get; set; }
        //[DataMember(EmitDefaultValue = true)]
        //public decimal S1 { get; set; }
        //[DataMember(EmitDefaultValue = true)]
        //public decimal S2 { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 24)]
        public string RackStatus { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 25)]
        public string RackItemStatus { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 196)]
        public string Dock { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 198)]
        public string BoxingMessage { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 82)]
        public string RemakeRack { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 84)]
        public Decimal RemakeSlot { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 500)]
        public Decimal PrinterNum { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 78)]
        public string Lite4Rack { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 80)]
        public Decimal Lite4Slot { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 192)]
        public string PriorityCode { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 155)]
        public string BlindDocumentFlag { get; set; }
    }
}
