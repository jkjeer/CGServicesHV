using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class GlassItemTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public GlassItemTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[GlassItemLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("Plant", typeof(string));
            Columns.Add("UnitType", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("Description2", typeof(string));
            Columns.Add("StockType", typeof(string));
            Columns.Add("GlCategory", typeof(string));
            Columns.Add("CustomerNum", typeof(decimal));
            Columns.Add("CustomerName", typeof(string));
            Columns.Add("CustomerPartNum", typeof(string));
            Columns.Add("SupplierNum", typeof(decimal));
            Columns.Add("SupplierName", typeof(string));
            Columns.Add("PrimaryLocation", typeof(string));
            Columns.Add("PrimaryUom", typeof(string));
            //Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("CostPerEa", typeof(decimal));
            Columns.Add("LbsPerEa", typeof(decimal));
            Columns.Add("ActualSqft", typeof(decimal));
            Columns.Add("BillableSqft", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
            //Columns.Add("SpecUnitId", typeof(int));
            Columns.Add("IBDate", typeof(DateTime));
            Columns.Add("LIDate", typeof(DateTime));
            Columns.Add("IMDate", typeof(DateTime));
            Columns.Add("IVDate", typeof(DateTime));
            Columns.Add("IEDate", typeof(DateTime));
            Columns.Add("UMDate", typeof(DateTime));
        }

        public void SetTransactionDate(Plant plant)
        {
            try
            {
                foreach (DataRow dr in this.Rows)
                {
                    var ibDate = (dr["IBDate"].ToString() == "") ? DateTime.MinValue : Convert.ToDateTime(dr["IBDate"].ToString());
                    var liDate = (dr["LIDate"].ToString() == "") ? DateTime.MinValue : Convert.ToDateTime(dr["LIDate"].ToString());
                    var imDate = (dr["IMDate"].ToString() == "") ? DateTime.MinValue : Convert.ToDateTime(dr["IMDate"].ToString());
                    var ivDate = (dr["IVDate"].ToString() == "") ? DateTime.MinValue : Convert.ToDateTime(dr["IVDate"].ToString());
                    var ieDate = (dr["IEDate"].ToString() == "") ? DateTime.MinValue : Convert.ToDateTime(dr["IEDate"].ToString());
                    var umDate = (dr["UMDate"].ToString() == "") ? DateTime.MinValue : Convert.ToDateTime(dr["UMDate"].ToString());

                    dr["TransactionDate"] = new[] { ibDate, liDate, imDate, ivDate, ieDate, umDate }.Max().AddHours(plant.TimeOffset);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
