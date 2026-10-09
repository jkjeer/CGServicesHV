using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class WorkOrderPartTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("TransactionId")).Max(); }
            set { }
        }

        public WorkOrderPartTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[WorkOrderPartLedger]");
            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("ComponentNum", typeof(decimal));
            Columns.Add("BillType", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("SequenceNum", typeof(decimal));
            Columns.Add("PlannedQty", typeof(decimal));
            Columns.Add("ActualQty", typeof(decimal));
            Columns.Add("UOM", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("TransactionId", typeof(decimal));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["WOrderNum"] = Data.CastDbNull.To<decimal>(reader["WMDOCO"], 0);
                    dr["ComponentNum"] = Data.CastDbNull.To<decimal>(reader["WMCPNB"], 0) / 100;
                    dr["BillType"] = Data.CastDbNull.To(reader["WMTBM"], string.Empty).Trim();
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["WMCPIT"], 0);
                    dr["LotNum"] = Data.CastDbNull.To(reader["WMLOTN"], string.Empty).Trim();
                    dr["Location"] = Data.CastDbNull.To(reader["WMLOCN"], string.Empty).Trim();
                    dr["SequenceNum"] = Data.CastDbNull.To<decimal>(reader["WMOPSQ"], 0) / 100;
                    dr["PlannedQty"] = Data.CastDbNull.To<decimal>(reader["WMUORG"], 0) / 1000;
                    dr["ActualQty"] = Data.CastDbNull.To<decimal>(reader["WMTRQT"], 0) / 1000;
                    dr["UOM"] = Data.CastDbNull.To(reader["WMUM"], string.Empty).Trim();
                    var updateDateId = Data.CastDbNull.To<decimal>(reader["WMUPMJ"], 0);
                    var updateTimeId = Data.CastDbNull.To<decimal>(reader["WMTDAY"], 0);
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddMinutes(plant.BusinessSystemOffset);
                    dr["TransactionId"] = Data.CastDbNull.To<decimal>(reader["WMUKID"], 0);
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
