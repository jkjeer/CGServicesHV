using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class WorkOrderTable : DataTable
    {
        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("TransactionId")).Max(); }
            set { }
        }

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public WorkOrderTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[WorkOrderLedger]");

            Columns.Add("OrderNum", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));
            Columns.Add("TypeCode", typeof(string));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("ConfigId", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("StockCode", typeof(string));
            Columns.Add("OrderDate", typeof(DateTime));
            Columns.Add("DueDate", typeof(DateTime));
            Columns.Add("CancelDate", typeof(DateTime));
            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("Quantity", typeof(decimal));
            Columns.Add("RelatedOrderNum", typeof(decimal));
            Columns.Add("RelatedOrderType", typeof(string));
            Columns.Add("RelatedOrderLineNum", typeof(decimal));
            Columns.Add("OperationCode", typeof(string));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void AddCurrent(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["OrderNum"] = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                    dr["StatusCode"] = CastDbNull.To<string>(reader["WASRST"], String.Empty).Trim();
                    dr["TypeCode"] = CastDbNull.To<string>(reader["WATYPS"], String.Empty).Trim();
                    dr["LotNum"] = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0).ToString();
                    dr["ConfigId"] = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["WAITM"], 0);
                    dr["StockCode"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                    dr["OrderDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WATRDJ"], 0)).AddHours(plant.TimeOffset);
                    dr["DueDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WADRQJ"], 0)).AddHours(plant.TimeOffset);
                    if (CastDbNull.To<string>(reader["WASRST"], String.Empty).Trim() == "98")
                    {
                        dr["CancelDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WAUPMJ"], 0)).AddHours(plant.TimeOffset);
                    }
                    //dr["CancelDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WAUPMJ"], 0)).AddHours(plant.TimeOffset);
                    dr["CustomerSoldNum"] = Data.CastDbNull.To<decimal>(reader["WAAN8"], 0);
                    dr["Quantity"] = Data.CastDbNull.To<decimal>(reader["WAUORG"], 0) / 1000;
                    dr["RelatedOrderNum"] = Data.CastStringToDecimal(CastDbNull.To<string>(reader["WARORN"], String.Empty).Trim());
                    dr["RelatedOrderType"] = CastDbNull.To<string>(reader["WARCTO"], String.Empty).Trim();
                    dr["RelatedOrderLineNum"] = Data.CastDbNull.To<decimal>(reader["WALNID"], 0) / 1000;
                    dr["OperationCode"] = "Insert";
                    dr["TransactionId"] = 0;
                    dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WAUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["WATDAY"], 0)).AddHours(plant.TimeOffset);
                    this.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AddLedger(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["OrderNum"] = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                    dr["StatusCode"] = CastDbNull.To<string>(reader["WASRST"], String.Empty).Trim();
                    dr["TypeCode"] = CastDbNull.To<string>(reader["WATYPS"], String.Empty).Trim();
                    dr["LotNum"] = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0).ToString();
                    dr["ConfigId"] = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["WAITM"], 0);
                    dr["StockCode"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                    dr["OrderDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WATRDJ"], 0)).AddHours(plant.TimeOffset);
                    dr["DueDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WADRQJ"], 0)).AddHours(plant.TimeOffset);
                    if (CastDbNull.To<string>(reader["WASRST"], String.Empty).Trim() == "98")
                    {
                        dr["CancelDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WAUPMJ"], 0));
                    }
                    dr["CustomerSoldNum"] = Data.CastDbNull.To<decimal>(reader["WAAN8"], 0);
                    dr["Quantity"] = Data.CastDbNull.To<decimal>(reader["WAUORG"], 0) / 1000;
                    dr["RelatedOrderNum"] = Data.CastStringToDecimal(CastDbNull.To<string>(reader["WARORN"], String.Empty).Trim());
                    dr["RelatedOrderType"] = CastDbNull.To<string>(reader["WARCTO"], String.Empty).Trim();
                    dr["RelatedOrderLineNum"] = Data.CastDbNull.To<decimal>(reader["WALNID"], 0) / 1000;
                    dr["OperationCode"] = CastDbNull.To<string>(reader["WAIOPR"], String.Empty).Trim();
                    dr["TransactionId"] = Data.CastDbNull.To<decimal>(reader["WAUKID"], 0);
                    dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WAMUPM"], 0),
                        Data.CastDbNull.To<decimal>(reader["WAUPMT"], 0)).AddHours(plant.TimeOffset);
                    this.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AddStatus(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["OrderNum"] = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                    dr["StatusCode"] = CastDbNull.To<string>(reader["WASRST"], String.Empty).Trim();
                    dr["TransactionId"] = Data.CastDbNull.To<decimal>(reader["WAUKID"], 0) / 1000;
                    dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WAMUPM"], 0),
                        Data.CastDbNull.To<decimal>(reader["WAUPMT"], 0)).AddHours(plant.TimeOffset);
                    this.Rows.Add(dr);
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
