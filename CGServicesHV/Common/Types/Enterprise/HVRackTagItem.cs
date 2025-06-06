using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class HVRackTagItems : List<HVRackTagItem>
    {
        public DataTable GetDataTable()
        {
            var dt = new DataTable("RackTagItems");

            var propertyInfos = typeof(HVRackTagItem).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                dt.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                if (dt.Columns[propertyInfo.Name].DataType == typeof(DateTime))
                {
                    dt.Columns[propertyInfo.Name].DateTimeMode = DataSetDateTime.Unspecified;
                }
            }

            foreach (HVRackTagItem rackTag in this)
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
    public class HVRackTagItem
    {
        public DataTable GetDataTable()
        {
            var dt = new DataTable("RackTagItem");

            var propertyInfos = typeof(HVRackTagItem).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                dt.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                if (dt.Columns[propertyInfo.Name].DataType == typeof(DateTime))
                {
                    dt.Columns[propertyInfo.Name].DateTimeMode = DataSetDateTime.Unspecified;
                }
            }

            DataRow dr = dt.NewRow();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                dr[propertyInfo.Name] = propertyInfo.GetValue(this, null);
            }
            dt.Rows.Add(dr);

            return dt;
        }

        public HVRackTagItem()
        {
            RackId = 0;

            SerialNumber = "";
            Customer = "";
            PlannerCustomerNumber = 0;
            PlannerCustomer = "";
            ShipToCustomer = "";
            PackingTinInTinOut = "";
            E1ItemNumber = "";
            OriginalE1ItemNumber = "";
            GlassThickness = 0;
            GlassHeatTreatment = "";
            FirstSurfaceCoat = "";
            SecondSurfaceCoat = "";

            SecondSurfaceCoatDesc1 = "";
            TradeMarkInfoToPrint = "";

            Dimensions = "";
            DimensionsMetric = "";
            Quantity = 0;
            SqftActual = 0;
            WeightLbs = 0;
            GlassUnitWeightLbs = 0;
            TotalWeightLbs = 0;
            TotalGlassWeightLbs = 0;
            TotalRackWeightLbs = 0;
            RackTareWeightLbs = 0;
            //ProducedDateTime = DateTime.Now.ToLocalTime();
            WorkCenter = "";
            Message1 = "";
            Message2 = "";
            SalesMsg = "";
            SalesMsg2 = "";
            Lite1_1stSrfCoatKey = "";
            Lite1_2ndSrfCoatKey = "";
            WOrderNum = 0;
            CustPONum = "";
            CustPOLine = "";
            CustVendorID = "";
            CustPartNum = "";
            BaseFractional = "";
            LeftFractional = "";
            RightFractional = "";
            TopFractional = "";
            BaseMM = 0;
            LeftMM = 0;
            RightMM = 0;
            TopMM = 0;
            ProducedDate = "";
            ProducedTime = "";
            BaseDec = 0;
            LeftDec = 0;
            DueDate = "";
            LineID = "";
            Crew = "";
            CustPrtNum2 = "";
            CustPrtNum3 = "";
            CustPrtNum4 = "";
            User = "";
            CurrentDate = "";
            CurrentTime = "";
            SOrderNum = 0;
            SOrderLine = 0;
            SqftBillable = 0;
            ProducedTimeBarcode = "";
            EdgeDelete = "";
            Lite1_ColorShortDesc = "";
            Lite1_PatternShortDesc = "";
            Lite1_LamiShortDesc = "";
            RackType = "";
            FirmSchedule = 0;
            AnsiiLogoType = "";
            OriginalCustomer = "";
            AssetID = "";
            PlantID = "";
            ScheduleNum = 0;
            ShopFloorSystem = "";
            LitesPerInch = 0;
            CalculatedLitesPerInch = "";

            ProducingPlantID = "";
            ProducingPlantCode = "";
            ProducingPlantName = "";
            ProducingPlantAddress1 = "";
            ProducingPlantAddress2 = "";
            ProducingPlantCity = "";
            ProducingPlantState = "";
            ProducingPlantPostalCode = "";
            ProducingPlantAddressNum = 0;
            ProducingPlantCustomer = "";
            ProducingPlantShipToCustomer = "";

            ActualMfgPlantID = "";
            ActualMfgPlantCode = "";
            ActualMfgPlantName = "";
            ActualMfgPlantAddress1 = "";
            ActualMfgPlantAddress2 = "";
            ActualMfgPlantCity = "";
            ActualMfgPlantState = "";
            ActualMfgPlantPostalCode = "";
            ActualMfgPlantAddressNum = 0;
            ActualMfgPlantCustomer = "";
            ActualMfgPlantShipToCustomer = "";

            RequestingPlantID = "";
            RequestingPlantCode = "";
            RequestingPlantName = "";
            RequestingPlantAddress1 = "";
            RequestingPlantAddress2 = "";
            RequestingPlantCity = "";
            RequestingPlantState = "";
            RequestingPlantPostalCode = "";
            RequestingPlantAddressNum = 0;
            RequestingPlantCustomer = "";
            RequestingPlantShipToCustomer = "";

            CurrentPlantID = "";
            S7MCU = "";
            CurrentPlantCode = "";
            CurrentPlantName = "";
            CurrentPlantAddress1 = "";
            CurrentPlantAddress2 = "";
            CurrentPlantCity = "";
            CurrentPlantState = "";
            CurrentPlantPostalCode = "";
            CurrentPlantAddressNum = 0;
            CurrentPlantCustomer = "";
            CurrentPlantShipToCustomer = "";

            CurrentQuantity = 0;
            MfgRackQuantity = 0;
            ErrorMessage = "";

            DockLocal = "";
            Lite1PatTextSrf = "";
            ConfigId = 0;
            Target = "";
            FreeLiteQuantity = 0;
            FreeLiteUnitWeightLbs = 0;

            Comment1 = "";
            Comment2 = "";
        }

        [DataMember(EmitDefaultValue = true)]
        public decimal RackId { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string SerialNumber { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Customer { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ShipToCustomer { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string PackingTinInTinOut { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string E1ItemNumber { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string OriginalE1ItemNumber { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal GlassThickness { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string GlassHeatTreatment { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string FirstSurfaceCoat { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string SecondSurfaceCoat { get; set; }



        [DataMember(EmitDefaultValue = true)]
        public string SecondSurfaceCoatDesc1 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string TradeMarkInfoToPrint { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public string Dimensions { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string DimensionsMetric { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal Quantity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal SqftActual { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal WeightLbs { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal GlassUnitWeightLbs { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal TotalWeightLbs { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal TotalGlassWeightLbs { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal TotalRackWeightLbs { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal RackTareWeightLbs { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string WorkCenter { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public DateTime ProducedDateTime { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Message1 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Message2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string SalesMsg { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string SalesMsg2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1_1stSrfCoatKey { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1_2ndSrfCoatKey { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal WOrderNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CustPONum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CustPOLine { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CustVendorID { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CustPartNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string BaseFractional { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string LeftFractional { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RightFractional { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string TopFractional { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal BaseMM { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal LeftMM { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal RightMM { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal TopMM { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducedDate { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducedTime { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal BaseDec { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal LeftDec { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string DueDate { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string LineID { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Crew { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CustPrtNum2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CustPrtNum3 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CustPrtNum4 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string User { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentDate { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentTime { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal SOrderNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal SOrderLine { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal SqftBillable { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducedTimeBarcode { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string EdgeDelete { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1_ColorShortDesc { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1_PatternShortDesc { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1_LamiShortDesc { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RackType { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal FirmSchedule { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string AnsiiLogoType { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string OriginalCustomer { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string AssetID { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string PlantID { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public decimal ScheduleNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ShopFloorSystem { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantID { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantCode { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal ProducingPlantAddressNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantName { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantAddress1 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantAddress2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantCity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantState { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantPostalCode { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantCustomer { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ProducingPlantShipToCustomer { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantID { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal ActualMfgPlantAddressNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantName { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantAddress1 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantAddress2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantCity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantState { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantPostalCode { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantCustomer { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ActualMfgPlantShipToCustomer { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantID { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal RequestingPlantAddressNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantName { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantAddress1 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantAddress2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantCity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantState { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantPostalCode { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantCustomer { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string RequestingPlantShipToCustomer { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantID { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string S7MCU { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal CurrentPlantAddressNum { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantName { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantAddress1 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantAddress2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantCity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantState { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantPostalCode { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantCustomer { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CurrentPlantShipToCustomer { get; set; }


        [DataMember(EmitDefaultValue = true)]
        public decimal CurrentQuantity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal MfgRackQuantity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Shift { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string GlassHandler { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string GlassHandler2 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string GlassHandler3 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string GlassHandler4 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string ErrorMessage { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public Double LitesPerInch { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string CalculatedLitesPerInch { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Lite1PatTextSrf { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string DockLocal { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal ConfigId { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal PlannerCustomerNumber { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string PlannerCustomer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Target { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal FreeLiteQuantity { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public decimal FreeLiteUnitWeightLbs { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public string Comment1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Comment2 { get; set; }

        //[DataMember(EmitDefaultValue = true)]
        //public Plant ActualMfgPlant { get; set; }

        //[DataMember(EmitDefaultValue = true)]
        //public Plant ProducingPlant { get; set; }

        //[DataMember(EmitDefaultValue = true)]
        //public Plant RequestingPlant { get; set; }
        //[DataMember(EmitDefaultValue = true)]
        //public Plant CurrentPlant { get; set; }
    }
}