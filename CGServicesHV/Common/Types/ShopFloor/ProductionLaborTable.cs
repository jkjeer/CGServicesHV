using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ProductionLaborTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("SourceUpdateDate")).Max(); }
            set { }
        }

        public ProductionLaborTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ProductionLaborLedger]");
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("Workgroup1", typeof(string));
            Columns.Add("Workgroup2", typeof(string));
            Columns.Add("Workgroup3", typeof(string));
            Columns.Add("PayTypeCode", typeof(string));
            Columns.Add("PayTypeDesc", typeof(string));
            Columns.Add("Hours", typeof(decimal));
            Columns.Add("Amount", typeof(decimal));
            Columns.Add("SourceInsertDate", typeof(DateTime));
            Columns.Add("SourceUpdateDate", typeof(DateTime));
            Columns.Add("IsDeleted", typeof(bool));
            Columns.Add("ShiftName", typeof(string));
        }

        public void AddUKG(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["TransactionDate"] = reader.Get<DateTime>("TransactionDate");
                    dr["Workgroup1"] = reader.Get<String>("WorkCenterName");
                    dr["PayTypeDesc"] = reader.Get<String>("PayCode");
                    dr["Hours"] = reader.Get<decimal>("TotalHours");
                    dr["Amount"] = reader.Get<decimal>("TotalAmt");
                    dr["SourceInsertDate"] = reader.Get<DateTime>("SourceInsertDate");
                    dr["SourceUpdateDate"] = reader.Get<DateTime>("SourceUpdateDate");
                    dr["IsDeleted"] = reader.Get<bool>("IsDeleted");
                    dr["ShiftName"] = reader.Get<String>("ShiftName");
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        public void AddAOD(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["TransactionDate"] = reader.Get<DateTime>("TransactionDate");
                    dr["Workgroup1"] = reader.Get<String>("Workgroup1");
                    dr["Workgroup2"] = reader.Get<String>("Workgroup2");
                    dr["Workgroup3"] = reader.Get<String>("Workgroup3");
                    dr["PayTypeCode"] = reader.Get<String>("PayTypeCode");
                    dr["PayTypeDesc"] = reader.Get<String>("PayTypeDesc");
                    dr["Hours"] = reader.Get<decimal>("Hours");
                    dr["Amount"] = reader.Get<decimal>("Amount");
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }
}
