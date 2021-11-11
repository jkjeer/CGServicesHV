using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ScheduleLineTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public ScheduleLineTable()
        {
            ExtendedProperties.Add("Target", "[ORD].[SCHEDULE_LINE_LEDGER]");
            Columns.Add("SCHEDULE_NUM", typeof(decimal));
            Columns.Add("SCHEDULE_DATE", typeof(DateTime));
            Columns.Add("SCHEDULE_LINE_NUM", typeof(int));
            Columns.Add("WORK_ORDER_NUM", typeof(decimal));
            Columns.Add("SALES_ORDER_NUM", typeof(decimal));
            Columns.Add("SALES_ORDER_TYPE", typeof(string));
            Columns.Add("SALES_ORDER_LINE_NUM", typeof(decimal));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("QUANTITY", typeof(decimal));
            Columns.Add("ORDER_DATE", typeof(DateTime));
            Columns.Add("DUE_DATE", typeof(DateTime));
            Columns.Add("PRIORITY_CODE", typeof(string));
            Columns.Add("REASON_CODE", typeof(string));
            Columns.Add("PRODUCT_TYPE", typeof(string));
            Columns.Add("CARDINAL_PRODUCT_TYPE", typeof(string));
            Columns.Add("CUSTOMER_SOLD_NUM", typeof(decimal));
            Columns.Add("CUSTOMER_SOLD_NAME", typeof(string));
            Columns.Add("CUSTOMER_SHIP_NUM", typeof(decimal));
            Columns.Add("CUSTOMER_SHIP_NAME", typeof(string));
            Columns.Add("CUSTOMER_BILL_NUM", typeof(decimal));
            Columns.Add("CUSTOMER_MAIL_NAME", typeof(string));
            Columns.Add("CUSTOMER_PART_NUM1", typeof(string));
            Columns.Add("CUSTOMER_PART_NUM2", typeof(string));
            Columns.Add("CUSTOMER_PART_NUM3", typeof(string));
            Columns.Add("CUSTOMER_PART_NUM4", typeof(string));
            Columns.Add("CUSTOMER_PO", typeof(string));
            Columns.Add("CUSTOMER_SCHEDULE_NUM", typeof(string));
            Columns.Add("CUSTOMER_IG_LINE", typeof(decimal));
            Columns.Add("CUSTOMER_SEQUENCE_NUM", typeof(string));
            Columns.Add("GENERIC_SEQUENCE_NUM", typeof(string));
            Columns.Add("IG_LINE", typeof(string));
            Columns.Add("ROUTING1", typeof(string));
            Columns.Add("ROUTING2", typeof(string));
            Columns.Add("ROUTING3", typeof(string));
            Columns.Add("BOX_PRIORITY", typeof(string));
            Columns.Add("BUNGEE", typeof(string));
            Columns.Add("BUNGEE_SIZE", typeof(decimal));
            Columns.Add("FLIP_GLASS", typeof(string));
            Columns.Add("TUBES", typeof(string));
            Columns.Add("PRESERVE", typeof(string));
            Columns.Add("SCORING", typeof(string));
            Columns.Add("DRAWING", typeof(string));
            Columns.Add("QC_INSPECT", typeof(string));
            Columns.Add("CRATE", typeof(string));
            Columns.Add("CRATE_ID", typeof(decimal));
            Columns.Add("CRATE_TYPE", typeof(string));
            Columns.Add("BATCH_NAME", typeof(string));
            Columns.Add("SCHEDULER_VALUE1", typeof(string));
            Columns.Add("SCHEDULER_VALUE2", typeof(string));
            Columns.Add("SCHEDULER_VALUE3", typeof(string));
            Columns.Add("SCHEDULER_VALUE4", typeof(string));
            Columns.Add("SCHEDULER_MESSAGE1", typeof(string));
            Columns.Add("SCHEDULER_MESSAGE2", typeof(string));
            Columns.Add("SALES_MESSAGE1", typeof(string));
            Columns.Add("SALES_MESSAGE2", typeof(string));
            Columns.Add("COMMENT1", typeof(string));
            Columns.Add("COMMENT2", typeof(string));
            Columns.Add("TRANSACTION_DATE", typeof(DateTime));
        }


        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    DataRow dr = this.NewRow();
                    dr["SCHEDULE_NUM"] = line.F580021.S7Z58SCN;
                    dr["SCHEDULE_DATE"] = E1TimeManager.CastToDateTime(line.F580021.S7Z58SCD);
                    dr["SCHEDULE_LINE_NUM"] = line.F580021.S7Z58SCSN;
                    dr["WORK_ORDER_NUM"] = line.F580021.S7DOCO;
                    dr["SALES_ORDER_NUM"] = Data.ParseStringToDecimal(line.F580021.S7RORN); 
                    dr["SALES_ORDER_TYPE"] = line.F580021.S7RCTO;
                    dr["SALES_ORDER_LINE_NUM"] = line.F580021.S7LNID / 1000;
                    dr["ITEM_NUM"] = line.F580021.S7ITM;
                    dr["CONFIGURATION_ID"] = line.F580021.CMCFGID;
                    dr["QUANTITY"] = line.F580021.S7Z58QTY;
                    dr["ORDER_DATE"] = E1TimeManager.CastToDateTime(line.F580021.S7RSDJ);
                    dr["DUE_DATE"] = E1TimeManager.CastToDateTime(line.F580021.S7Z58SCDDJ);
                    dr["PRIORITY_CODE"] = line.F580024.S8URCD;
                    dr["REASON_CODE"] = line.F580024.S8URCD;
                    dr["PRODUCT_TYPE"] = line.F580021.S7Z58PT;
                    dr["CARDINAL_PRODUCT_TYPE"] = line.F580024.S8URRF;
                    dr["CUSTOMER_SOLD_NUM"] = 0;
                    dr["CUSTOMER_SOLD_NAME"] = "";
                    dr["CUSTOMER_SHIP_NUM"] = line.F580021.S7SHAN;
                    dr["CUSTOMER_SHIP_NAME"] = line.F580021.S7ALPH;
                    dr["CUSTOMER_BILL_NUM"] = line.F580021.S7AN8DR;
                    dr["CUSTOMER_MAIL_NAME"] = line.F580024.S8MLNM;
                    dr["CUSTOMER_PART_NUM1"] = line.F580021.S7CITM;
                    dr["CUSTOMER_PART_NUM2"] = line.F580021.S7Z58CP2;
                    dr["CUSTOMER_PART_NUM3"] = line.F580021.S7Z58CP3;
                    dr["CUSTOMER_PART_NUM4"] = line.F580021.S7Z58CP4;
                    dr["CUSTOMER_PO"] = line.F580021.S7VR01;
                    dr["CUSTOMER_SCHEDULE_NUM"] = line.F580021.S7Z58CSCNO;
                    dr["CUSTOMER_IG_LINE"] = line.F580021.S7Z58IGCLN;
                    dr["CUSTOMER_SEQUENCE_NUM"] = line.F580021.S7Z58CSQNO;
                    dr["GENERIC_SEQUENCE_NUM"] = line.F580021.S7Z58GSQNO;
                    dr["IG_LINE"] = line.F580021.S7Z58IGLN;
                    dr["ROUTING1"] = line.F580021.S7Z58L1RG;
                    dr["ROUTING2"] = line.F580021.S7Z58L2RG;
                    dr["ROUTING3"] = line.F580021.S7Z58L3RG;
                    dr["BOX_PRIORITY"] = line.F580021.S7Z58BOXP;
                    dr["BUNGEE"] = line.F580021.S7Z58BGEE;
                    dr["BUNGEE_SIZE"] = line.F580021.S7Z58BGSZ;
                    dr["FLIP_GLASS"] = line.F580021.S7Z58FLIP;
                    dr["TUBES"] = line.F580021.S7Z58TBAD;
                    dr["PRESERVE"] = line.F580021.S7Z58PVAD;
                    dr["SCORING"] = line.F580021.S7Z58SCAD;
                    dr["DRAWING"] = line.F580021.S7Z58DRAW;
                    dr["QC_INSPECT"] = line.F580024.S8Z5801840;
                    dr["CRATE"] = line.F580021.S7Z58CRATE;
                    dr["CRATE_ID"] = (line.F580220s.Any()) ? line.F580220s[0].CRZ58CRID : 0;
                    dr["CRATE_TYPE"] = (line.F580220s.Any()) ? line.F580220s[0].CRZ58CRTY : string.Empty;
                    dr["BATCH_NAME"] = line.F580024.S8Z58BDES;
                    dr["SCHEDULER_VALUE1"] = line.F580024.S8Z58CCV01;
                    dr["SCHEDULER_VALUE2"] = line.F580024.S8Z58CCV02;
                    dr["SCHEDULER_VALUE3"] = line.F580024.S8Z58CCV03;
                    dr["SCHEDULER_VALUE4"] = line.F580024.S8Z58CCV04;
                    dr["SCHEDULER_MESSAGE1"] = line.F580021.S7Z58MSG1;
                    dr["SCHEDULER_MESSAGE2"] = line.F580021.S7Z58MSG2;
                    dr["SALES_MESSAGE1"] = line.F580021.S7Z58GAL1;
                    dr["SALES_MESSAGE2"] = line.F580021.S7Z58GAL2;
                    dr["COMMENT1"] = line.F580021.S7Z58CMT1;
                    dr["COMMENT2"] = line.F580021.S7Z58CMT2;
                    dr["TRANSACTION_DATE"] = E1TimeManager.CastToDateTime(line.F580021.S7UPMJ, line.F580021.S7TDAY);
                    this.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException(e.Message + " " + e.StackTrace);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class ScheduleLineEbsTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ScheduleLineEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ScheduleLine]");
            Columns.Add("ScheduleId", typeof(int));
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("ScheduleDate", typeof(DateTime));
            Columns.Add("ScheduleLineNum", typeof(int));
            Columns.Add("WorkOrderNum", typeof(decimal));
            Columns.Add("WorkOrderId", typeof(int));
            Columns.Add("SchedulerMessage1", typeof(string));
            Columns.Add("SchedulerMessage2", typeof(string));
            Columns.Add("Comment1", typeof(string));
            Columns.Add("Comment2", typeof(string));
            Columns.Add("Comment3", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    DataRow dr = this.NewRow();
                    dr["ScheduleId"] = schedule.ScheduleId;
                    dr["ScheduleNum"] = line.F580021.S7Z58SCN;
                    dr["ScheduleDate"] = E1TimeManager.CastToDateTime(line.F580021.S7Z58SCD);
                    dr["ScheduleLineNum"] = line.F580021.S7Z58SCSN;
                    dr["WorkOrderNum"] = line.F580021.S7DOCO;
                    dr["WorkOrderId"] = line.WorkOrderId;
                    dr["SchedulerMessage1"] = line.F580021.S7Z58GAL1;
                    dr["SchedulerMessage2"] = line.F580021.S7Z58GAL2;
                    dr["Comment1"] = line.F580021.S7Z58CMT1;
                    dr["Comment2"] = line.F580021.S7Z58CMT2;
                    dr["Comment3"] = line.F580021.S7Z58CMT3;
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(line.F580021.S7UPMJ, line.F580021.S7TDAY);
                    this.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message + " " + e.StackTrace);
            }
        }
    }
}
