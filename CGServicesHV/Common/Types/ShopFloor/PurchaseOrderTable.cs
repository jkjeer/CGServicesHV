using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class PurchaseOrderTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }
        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable().Where(p=>p.Field<decimal>("LineUkid") > 0) select a.Field<decimal>("LineUkid")).Max(); }
            set { }
        }

        public PurchaseOrderTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[PurchaseOrderLedger]");
            Columns.Add("OrderNum", typeof(decimal));
            Columns.Add("OrderType", typeof(string));
            Columns.Add("OrderLineNum", typeof(decimal));
            Columns.Add("ConfigId", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("QuantityOrdered", typeof(decimal));
            Columns.Add("QuantityReceived", typeof(decimal));
            Columns.Add("OrderDate", typeof(DateTime));
            Columns.Add("RequestedDate", typeof(DateTime));
            Columns.Add("ReceivedDate", typeof(DateTime));
            Columns.Add("CancelDate", typeof(DateTime));
            Columns.Add("SupplierNum", typeof(decimal));
            //Columns.Add("StockCode", typeof(string));
            Columns.Add("LineTypeCode", typeof(string));
            Columns.Add("GlCatCode", typeof(string));
            Columns.Add("StatusLastCode", typeof(string));
            Columns.Add("StatusNextCode", typeof(string));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                using (reader)
                {

                    while (reader.Read())
                    {
                        DataRow dr = this.NewRow();
                        dr["OrderNum"] = Data.CastDbNull.To<decimal>(reader["OLDOCO"], 0);
                        dr["OrderType"] = CastDbNull.To<string>(reader["OLDCTO"], String.Empty).Trim();
                        dr["OrderLineNum"] = CastDbNull.To<decimal>(reader["OLLNID"], 0) / 1000;
                        dr["ConfigId"] = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                        dr["ItemNum"] = CastDbNull.To<decimal>(reader["OLITM"], 0);
                        dr["LotNum"] = CastDbNull.To<string>(reader["OLLOTN"], String.Empty).Trim();
                        dr["QuantityOrdered"] = CastDbNull.To<decimal>(reader["OLUORG"], 0) / 1000;
                        dr["QuantityReceived"] = CastDbNull.To<decimal>(reader["OLUREC"], 0) / 1000;
                        dr["OrderDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLTRDJ"], 0));
                        dr["RequestedDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLDRQJ"], 0));
                        dr["ReceivedDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLADDJ"], 0));
                        dr["CancelDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLCNDJ"], 0));
                        dr["SupplierNum"] = CastDbNull.To<decimal>(reader["OLAN8"], 0);
                        //dr["StockCode"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                        dr["LineTypeCode"] = CastDbNull.To<string>(reader["OLLNTY"], String.Empty).Trim();
                        dr["GlCatCode"] = CastDbNull.To<string>(reader["OLGLC"], String.Empty).Trim();
                        dr["StatusLastCode"] = CastDbNull.To<string>(reader["OLLTTR"], String.Empty).Trim();
                        dr["StatusNextCode"] = CastDbNull.To<string>(reader["OLNXTR"], String.Empty).Trim();
                        dr["TransactionId"] = CastDbNull.To<decimal>(reader["OLUKID"], 0);
                        dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["OLTDAY"], 0)).AddHours(plant.TimeOffset);
                        this.Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }
}
