using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class WorkOrderCostTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public WorkOrderCostTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[WorkOrderCostLedger]");
            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("CostType", typeof(string));
            Columns.Add("RelationType", typeof(string));
            Columns.Add("StandardQty", typeof(decimal));
            Columns.Add("StandardAmt", typeof(decimal));
            Columns.Add("ActualQty", typeof(decimal));
            Columns.Add("ActualAmt", typeof(decimal));
            Columns.Add("CurrentQty", typeof(decimal));
            Columns.Add("CurrentAmt", typeof(decimal));
            Columns.Add("PlannedQty", typeof(decimal));
            Columns.Add("PlannedAmt", typeof(decimal));
            Columns.Add("CompletedQty", typeof(decimal));
            Columns.Add("CompletedAmt", typeof(decimal));
            Columns.Add("ScrappedQty", typeof(decimal));
            Columns.Add("ScrappedAmt", typeof(decimal));
            Columns.Add("UOM", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["WOrderNum"] = CastDbNull.To<decimal>(reader["IGDOCO"], 0);
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["IGITM"], 0);
                    dr["CostType"] = CastDbNull.To(reader["IGCOST"], string.Empty).Trim();
                    dr["RelationType"] = CastDbNull.To(reader["IGPART"], string.Empty).Trim();
                    dr["StandardQty"] = CastDbNull.To<decimal>(reader["IGOPUN"], 0) / 10000;
                    dr["StandardAmt"] = CastDbNull.To<decimal>(reader["IGOPAT"], 0) / 10000;
                    dr["ActualQty"] = CastDbNull.To<decimal>(reader["IGCLUN"], 0) / 10000;
                    dr["ActualAmt"] = CastDbNull.To<decimal>(reader["IGCLAT"], 0) / 10000;
                    dr["CurrentQty"] = CastDbNull.To<decimal>(reader["IGCCCU"], 0) / 10000;
                    dr["CurrentAmt"] = CastDbNull.To<decimal>(reader["IGCCCA"], 0) / 10000;
                    dr["PlannedQty"] = CastDbNull.To<decimal>(reader["IGPLUN"], 0) / 10000;
                    dr["PlannedAmt"] = CastDbNull.To<decimal>(reader["IGPLAT"], 0) / 10000;
                    dr["CompletedQty"] = CastDbNull.To<decimal>(reader["IGCPUN"], 0) / 10000;
                    dr["CompletedAmt"] = CastDbNull.To<decimal>(reader["IGCPAT"], 0) / 10000;
                    dr["ScrappedQty"] = CastDbNull.To<decimal>(reader["IGSLUN"], 0) / 10000;
                    dr["ScrappedAmt"] = CastDbNull.To<decimal>(reader["IGSLAT"], 0) / 10000;
                    dr["UOM"] = CastDbNull.To(reader["IGUOM"], string.Empty).Trim();
                    var updateDateId = CastDbNull.To<decimal>(reader["IGUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["IGTDAY"], 0);
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddMinutes(plant.BusinessSystemOffset);
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
