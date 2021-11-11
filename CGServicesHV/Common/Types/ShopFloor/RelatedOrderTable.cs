using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

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
            Columns.Add("OrderNum", typeof(decimal));
            Columns.Add("OrderType", typeof(string));
            Columns.Add("OrderLineNum", typeof(decimal));
            Columns.Add("RelatedOrderNum", typeof(decimal));
            Columns.Add("RelatedOrderType", typeof(string));
            Columns.Add("RelatedOrderLineNum", typeof(decimal));
            Columns.Add("RelatedOrderOrigLineNum", typeof(decimal));
            Columns.Add("RelatedPlant", typeof(string));
            Columns.Add("LiteNum", typeof(decimal));
            Columns.Add("TransactionType", typeof(string));
            Columns.Add("Quantity", typeof(decimal));
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
                        dr["OrderNum"] = CastDbNull.To<decimal>(reader["RODOCO"], 0);
                        dr["OrderType"] = CastDbNull.To<string>(reader["RODCTO"], String.Empty).Trim();
                        dr["OrderLineNum"] = CastDbNull.To<decimal>(reader["ROLNID"], 0) / 1000;
                        dr["RelatedOrderNum"] = CastDbNull.To<decimal>(reader["RODOC1"], 0);
                        dr["RelatedOrderType"] = CastDbNull.To<string>(reader["RORCTO"], String.Empty).Trim();
                        dr["RelatedOrderLineNum"] = CastDbNull.To<decimal>(reader["RORLLN"], 0) / 1000;
                        dr["RelatedPlant"] = CastDbNull.To<string>(reader["ROMCU2"], String.Empty).Trim();
                        dr["LiteNum"] = CastDbNull.To<decimal>(reader["ROZ58L1CLN"], 0);
                        dr["TransactionType"] = CastDbNull.To<string>(reader["ROEDTR"], String.Empty).Trim();
                        dr["Quantity"] = Data.CastDbNull.To<decimal>(reader["ROUORG"], 0) / 1000;
                        dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ROUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["ROTDAY"], 0)).AddHours(plant.TimeOffset);
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
