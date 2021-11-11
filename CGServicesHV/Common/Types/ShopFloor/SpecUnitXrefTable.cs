using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServices.Common.Managers.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Utilities;

namespace CGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitXrefTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public SpecUnitXrefTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_XREF_LEDGER]");
            Columns.Add("SPEC_UNIT_ID", typeof(int));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("PLANT_CD", typeof(string));
            Columns.Add("DOCUMENT_NUM", typeof(decimal));
            Columns.Add("DOCUMENT_TYPE_CD", typeof(string));
            Columns.Add("DOCUMENT_LINE_NUM", typeof(decimal));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(SpecUnits specUnits)
        {
            try
            {
                foreach(var specUnit in specUnits)
                    foreach (OrderId orderId in specUnit.OrderIds)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = specUnit.SpecUnitId;
                        dr["ITEM_NUM"] = specUnit.ItemNum;
                        dr["CONFIGURATION_ID"] = specUnit.ConfigurationId;
                        dr["PLANT_CD"] = orderId.Plant;
                        dr["DOCUMENT_NUM"] = orderId.DocumentNum;
                        dr["DOCUMENT_TYPE_CD"] = orderId.DocumentType;
                        dr["DOCUMENT_LINE_NUM"] = orderId.DocumentLineNum;
                        dr["UPDATE_DATE"] = specUnit.TransactionDate;
                        Rows.Add(dr);
                    }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add(SpecUnit item)
        {
            try
            {
                if (item.ConfigurationId > 0)
                {
                    foreach (OrderId orderId in item.OrderIds)
                    {
                        DataRow dr = NewRow();
                        dr["DOCUMENT_NUM"] = orderId.DocumentNum;
                        dr["DOCUMENT_TYPE_CD"] = orderId.DocumentType;
                        dr["DOCUMENT_LINE_NUM"] = orderId.DocumentLineNum / 1000;
                        dr["PLANT_CD"] = orderId.Plant;
                        dr["LOT_NUM"] = orderId.LotNum;
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIGURATION_ID"] = item.ConfigurationId;
                        dr["UPDATE_DATE"] = item.TransactionDate;
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    if (line.F580021.CMCFGID > 0)
                    {
                        if (line.F580021.S7RORN != "")
                        {
                            DataRow dr = NewRow();
                            dr["DOCUMENT_NUM"] = Data.ParseStringToDecimal(line.F580021.S7RORN);
                            dr["DOCUMENT_TYPE_CD"] = "WO";
                            dr["DOCUMENT_LINE_NUM"] = 0;
                            dr["PLANT_CD"] = line.F580021.S7MCU;
                            dr["LOT_NUM"] = line.F580021.S7RORN;
                            dr["ITEM_NUM"] = line.F580021.S7ITM;
                            dr["CONFIGURATION_ID"] = line.F580021.CMCFGID;
                            dr["UPDATE_DATE"] = E1TimeManager.CastToDateTime(line.F580021.S7UPMJ, line.F580021.S7TDAY);
                            Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitXrefEbsTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public SpecUnitXrefEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitXref]");
            Columns.Add("SpecUnitId", typeof(int));
            Columns.Add("DocumentNum", typeof(decimal));
            Columns.Add("DocumentType", typeof(string));
            Columns.Add("DocumentLineNum", typeof(decimal));
            Columns.Add("Plant", typeof(string));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("ConfigurationId", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    if (line.F580021.CMCFGID > 0)
                    {
                        if (line.F580021.S7RORN != "")
                        {
                            DataRow dr = NewRow();
                            dr["SpecUnitId"] = line.SpecUnitId;
                            dr["DocumentNum"] = line.F580021.S7DOCO;
                            dr["DocumentType"] = "WO";
                            dr["DocumentLineNum"] = 0;
                            dr["Plant"] = line.F580021.S7MCU;
                            dr["LotNum"] = line.F580021.S7DOCO.ToString();
                            dr["ItemNum"] = line.F580021.S7ITM;
                            dr["ConfigurationId"] = line.F580021.CMCFGID;
                            dr["TransactionDate"] = E1TimeManager.CastToDateTime(line.F580021.S7UPMJ, line.F580021.S7TDAY);
                            Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
