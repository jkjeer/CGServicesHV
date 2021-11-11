using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class WorkOrderStatuses : List<WorkOrderStatus>
    {

        public DataTable SerializeToDataTable()
        {
            var dt = WorkOrderStatus.GetDataTable("WorkOrderStatus");
            foreach (WorkOrderStatus workOrderStatus in this)
            {
                workOrderStatus.AddDataRow(dt);
            }
            return dt;
        }
    }

    [DataContract(Namespace = "")]
    public class WorkOrderStatus
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal OrderNum { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string OrderType { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string StatusCode { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string PostedCode { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal SystemDate { get; set; }

        [DataMember(Order = 42, EmitDefaultValue = false)]
        public decimal SystemTime { get; set; }

        [DataMember(Order = 44, EmitDefaultValue = false)]
        public DateTime SystemDateTime
        {
            get
            {
                int iYear = 0;
                int iMonth = 0;
                int iDay = 0;
                int iHour = 0;
                int iMinute = 0;
                int iSecond = 0;

                var sDate = Convert.ToString(SystemDate);
                switch (sDate.Length)
                {
                    case 1:
                        iYear = 0;
                        iMonth = 1;
                        iDay = 1;
                        break;
                    case 5:
                        iYear = Convert.ToInt32(sDate.Substring(3, 2));
                        iMonth = Convert.ToInt32(sDate.Substring(0, 1));
                        iDay = Convert.ToInt32(sDate.Substring(1, 2));
                        break;
                    case 6:
                        iYear = Convert.ToInt32(sDate.Substring(4, 2));
                        iMonth = Convert.ToInt32(sDate.Substring(0, 2));
                        iDay = Convert.ToInt32(sDate.Substring(2, 2));
                        break;
                }

                var sTime = Convert.ToString(SystemTime);
                switch (sTime.Length)
                {
                    case 1:
                        iSecond = Convert.ToInt32(sTime.Substring(0, 1));
                        break;
                    case 2:
                        iSecond = Convert.ToInt32(sTime.Substring(0, 2));
                        break;
                    case 3:
                        iMinute = Convert.ToInt32(sTime.Substring(0, 1));
                        iSecond = Convert.ToInt32(sTime.Substring(1, 2));
                        break;
                    case 4:
                        iMinute = Convert.ToInt32(sTime.Substring(0, 2));
                        iSecond = Convert.ToInt32(sTime.Substring(2, 2));
                        break;
                    case 5:
                        iHour = Convert.ToInt32(sTime.Substring(0, 1));
                        iMinute = Convert.ToInt32(sTime.Substring(1, 2));
                        iSecond = Convert.ToInt32(sTime.Substring(3, 2));
                        break;
                    case 6:
                        iHour = Convert.ToInt32(sTime.Substring(0, 2));
                        iMinute = Convert.ToInt32(sTime.Substring(2, 2));
                        iSecond = Convert.ToInt32(sTime.Substring(4, 2));
                        break;
                }

                return new DateTime(2000+iYear, iMonth, iDay, iHour, iMinute, iSecond);                
            }
        }

        public static DataTable GetDataTable(String tableName)
        {
            DataTable dt = new DataTable(tableName);
            dt.Columns.Add("DocumentNum", typeof (Decimal));
            dt.Columns.Add("DocumentType", typeof (String));
            dt.Columns.Add("StatusCode", typeof (String));
            dt.Columns.Add("StatusDateTime", typeof (DateTime));
            return dt;
        }

        public void AddDataRow(DataTable dt)
        {
            DataRow dr = dt.NewRow();
            dr["DocumentNum"] = OrderNum;
            dr["DocumentType"] = OrderType;
            dr["StatusCode"] = StatusCode;
            dr["StatusDateTime"] = SystemDateTime;
            dt.Rows.Add(dr);
        }

 
    }
}
