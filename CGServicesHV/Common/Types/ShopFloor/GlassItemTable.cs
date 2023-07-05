using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

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
            Columns.Add("MasterPlanningCode", typeof(string));
            Columns.Add("VendorNum", typeof(decimal));
            Columns.Add("PlannerNum", typeof(decimal));
            Columns.Add("BuyerNum", typeof(decimal));
            Columns.Add("SRP1Code", typeof(string));
            Columns.Add("SRP2Code", typeof(string));
            Columns.Add("SRP3Code", typeof(string));
            Columns.Add("SRP4Code", typeof(string));
            Columns.Add("SRP5Code", typeof(string));
            Columns.Add("SRP6Code", typeof(string));
            Columns.Add("SRP7Code", typeof(string));
            Columns.Add("SRP8Code", typeof(string));
            Columns.Add("SRP9Code", typeof(string));
            Columns.Add("SRP0Code", typeof(string));
            Columns.Add("PRP1Code", typeof(string));
            Columns.Add("PRP2Code", typeof(string));
            Columns.Add("PRP3Code", typeof(string));
            Columns.Add("PRP4Code", typeof(string));
            Columns.Add("PRP5Code", typeof(string));
            Columns.Add("PRP6Code", typeof(string));
            Columns.Add("PRP7Code", typeof(string));
            Columns.Add("PRP8Code", typeof(string));
            Columns.Add("PRP9Code", typeof(string));
            Columns.Add("PRP0Code", typeof(string));
            Columns.Add("CustomerNum", typeof(decimal));
            Columns.Add("CustomerName", typeof(string));
            Columns.Add("CustomerPartNum", typeof(string));
            Columns.Add("SupplierNum", typeof(decimal));
            Columns.Add("SupplierName", typeof(string));
            Columns.Add("PrimaryLocation", typeof(string));
            Columns.Add("PrimaryLocationFull", typeof(string));
            Columns.Add("PrimaryUom", typeof(string));
            Columns.Add("CostPerEa", typeof(decimal));
            Columns.Add("LbsPerEa", typeof(decimal));
            Columns.Add("ActualSqft", typeof(decimal));
            Columns.Add("BillableSqft", typeof(decimal));
            Columns.Add("ReorderInputPoint", typeof(decimal));
            Columns.Add("ReorderInputQty", typeof(decimal));
            Columns.Add("ReorderMinQty", typeof(decimal));
            Columns.Add("ReorderMaxQty", typeof(decimal));
            Columns.Add("SafetyStockSupply", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("IBDate", typeof(DateTime));
            Columns.Add("LIDate", typeof(DateTime));
            Columns.Add("IMDate", typeof(DateTime));
            Columns.Add("IVDate", typeof(DateTime));
            Columns.Add("IEDate", typeof(DateTime));
            Columns.Add("UMDate", typeof(DateTime));
        }

        //public void Add(IDataReader reader)
        //{
        //    try
        //    {
        //        while (reader.Read())
        //        {
        //            DataRow dr = this.NewRow();
        //            dr["ItemNum"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["UnitType"] = CastDbNull.To<string>(reader["IMSRP4"], String.Empty).Trim();
        //            dr["Description1"] = CastDbNull.To<string>(reader["IMDSC1"], String.Empty).Trim();
        //            dr["Description2"] = CastDbNull.To<string>(reader["IMDSC2"], String.Empty).Trim();
        //            dr["StockType"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
        //            dr["GlCategory"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();

        //            dr["MasterPlanningCode"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["VendorNum"] = CastDbNull.To<decimal>(reader["IBVEND"], 0);
        //            dr["PlannerNum"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["BuyerNum"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["SRP1Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP2Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP3Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP4Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP5Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP6Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP7Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP8Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP9Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["SRP0Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP1Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP2Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP3Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP4Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP5Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP6Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP7Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP8Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP9Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["PRP0Code"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();

        //            dr["CustomerNum"] = CastDbNull.To<decimal>(reader["IBANPL"], 0);
        //            dr["CustomerName"] = CastDbNull.To<string>(reader["ANPLALPH"], String.Empty).Trim();
        //            dr["CustomerPartNum"] = CastDbNull.To<string>(reader["IVCITM"], String.Empty).Trim();
        //            dr["SupplierNum"] = CastDbNull.To<decimal>(reader["IBVEND"], 0);
        //            dr["SupplierName"] = CastDbNull.To<string>(reader["VENDALPH"], String.Empty).Trim();
        //            dr["PrimaryLocation"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty).Trim();
        //            dr["PrimaryLocationFull"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty);
        //            dr["PrimaryUom"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
        //            dr["CostPerEa"] = (decimal)0;
        //            dr["LbsPerEa"] = CastDbNull.To<decimal>(reader["UMCONV"], 0);
        //            //dr["QtyOnHand"] = (decimal)0;
        //            dr["ActualSqft"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["BillableSqft"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["ReorderInputPoint"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["ReorderInputQty"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["ReorderMinQty"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["ReorderMaxQty"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["SafetyStockSupply"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["TransactionId"] = (decimal)0;
        //            dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["IBUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["IBTDAY"], 0));
        //            this.Rows.Add(dr);
        //        }
        //        reader.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

    }
}
