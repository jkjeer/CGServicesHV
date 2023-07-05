using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackLoadItems : List<RackLoadItem>
    {
        public DataTable GetDataTable()
        {
            var dt = new DataTable("RackLoadItems");

            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(RackLoadItem).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                dt.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                if (dt.Columns[propertyInfo.Name].DataType == typeof(DateTime))
                {
                    dt.Columns[propertyInfo.Name].DateTimeMode = DataSetDateTime.Unspecified;
                }
            }

            foreach (RackLoadItem rackTag in this)
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
    public class RackLoadItem
    {

        public RackLoadItem()
        {
            RackId = 0;
            SerialNum = "";
            CompanyNum = "";
            PlantNum = "";
            SalesOrderNum = 0;
            SalesOrderLineNum = 0;
            SalesOrderType = "";
            WorkOrderNum = 0;
            ScheduleNum = 0;
            ItemNum = 0;
            Location = "";
            SoldToNum = 0;
            RackType = "";
            RackTypeDesc = "";
            RackStatus = "";
            RackStatusDate = DateTime.Now.ToLocalTime();
            Quantity = 0;

        }

        [DataMember(EmitDefaultValue = true)]
        public decimal RackId { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string SerialNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string PlantNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Location { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string RackType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string RackTypeDesc { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 25)]
        public string RackItemStatus { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string RackStatus { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string RackStatusDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime RackStatusDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal LoadNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal SuggestedLoadNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string RackUserReservedCode { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string AssetID { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime RackUpdateDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal ScheduleNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal SoldToNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal ItemNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime TransactionDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Quantity { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal SalesOrderNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal SalesOrderLineNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string SalesOrderType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string CompanyNum { get; set; }
        [DataMember(EmitDefaultValue = false, Order = 51)]
        public string LotNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal PurchaseOrderNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string PurchaseOrderType { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal PurchaseOrderLineNum { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal CompletedQuantity { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime ManufacturedDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime ProductionDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Description1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Description2 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LoadStatus { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LoadStatusDesc { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime RequestedDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime LoadedDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LoadComment1 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LoadComment2 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string LoadComment3 { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal Carrier { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string TrailerNumber { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime LoadConfirmDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime ActualShipDate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal BillOfLadingNumber { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public decimal MBOL { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime LoadUpdateDate { get; set; }

    }
}
