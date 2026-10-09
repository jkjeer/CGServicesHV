using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RelatedOrderTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public RelatedOrderTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RelatedOrderLedger]");
            Columns.Add("PlantNum", typeof(string));
            Columns.Add("OrderNum", typeof(decimal));
            Columns.Add("OrderType", typeof(string));
            Columns.Add("OrderLineNum", typeof(decimal));
            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("WOrderType", typeof(string));
            Columns.Add("RelatedOrderNum", typeof(decimal));
            Columns.Add("RelatedOrderType", typeof(string));
            Columns.Add("RelatedOrderLineNum", typeof(decimal));
            Columns.Add("RelatedOrderOrigLineNum", typeof(decimal));
            Columns.Add("RelatedSupplierPlant", typeof(string));
            Columns.Add("RelatedSupplierNum", typeof(decimal));
            Columns.Add("LiteNum", typeof(decimal));
            Columns.Add("TransactionType", typeof(string));
            Columns.Add("OrigCustomerPO", typeof(string));
            Columns.Add("Quantity", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));
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
                        dr["PlantNum"] = Data.CastDbNull.To<string>(reader["ROMCU"], String.Empty).Trim();
                        dr["OrderNum"] = Data.CastDbNull.To<decimal>(reader["RODOCO"], 0);
                        dr["OrderType"] = Data.CastDbNull.To<string>(reader["RODCTO"], String.Empty).Trim();
                        dr["OrderLineNum"] = Data.CastDbNull.To<decimal>(reader["ROLNID"], 0) / 1000;
                        dr["WOrderNum"] = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                        dr["WOrderType"] = Data.CastDbNull.To<string>(reader["WADCTO"], String.Empty).Trim();
                        dr["RelatedOrderNum"] = Data.CastDbNull.To<decimal>(reader["RODOC1"], 0);
                        dr["RelatedOrderType"] = Data.CastDbNull.To<string>(reader["RORCTO"], String.Empty).Trim();
                        dr["RelatedOrderLineNum"] = Data.CastDbNull.To<decimal>(reader["RORLLN"], 0) / 1000;
                        dr["RelatedOrderOrigLineNum"] = Math.Round(Data.CastDbNull.To<decimal>(reader["RORLLN"], 0) / 1000, 0);
                        dr["RelatedSupplierPlant"] = Data.CastDbNull.To<string>(reader["ROMCU2"], String.Empty).Trim();
                        dr["RelatedSupplierNum"] = Data.CastDbNull.To<decimal>(reader["PDAN8"], 0);
                        dr["LiteNum"] = Data.CastDbNull.To<decimal>(reader["ROZ58L1CLN"], 0);
                        dr["TransactionType"] = Data.CastDbNull.To<string>(reader["ROEDTR"], String.Empty).Trim();
                        dr["OrigCustomerPO"] = Data.CastDbNull.To<string>(reader["SDVR01"], String.Empty).Trim();
                        dr["Quantity"] = Data.CastDbNull.To<decimal>(reader["ROUORG"], 0) / 1000;
                        dr["StatusCode"] = Data.CastDbNull.To<string>(reader["ROZ58JBSTS"], String.Empty).Trim();
                        dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ROUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["ROTDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
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
