using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackItemEventTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("RackItemEventId")).Max(); }
            set { }
        }

        public RackItemEventTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackItemEventLedger]");
            Columns.Add("RackItemEventId", typeof(decimal));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("ReceivedQty", typeof(decimal));
            Columns.Add("ManufacturedDate", typeof(DateTime));
            Columns.Add("Side", typeof(string));
            Columns.Add("Stack", typeof(decimal));
            Columns.Add("Row", typeof(decimal));
            Columns.Add("Slot", typeof(decimal));
            Columns.Add("Position", typeof(decimal));
            Columns.Add("Rotated", typeof(string));
            Columns.Add("DocumentNum", typeof(decimal));
            Columns.Add("DocumentType", typeof(string));
            Columns.Add("DocumentLineNum", typeof(decimal));
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
                        DataRow dr = NewRow();
                        dr["RackItemEventId"] = Data.CastDbNull.To<decimal>(reader["DLUKID"], 0);
                        dr["RackId"] = CastDbNull.To<decimal>(reader["DLZ58RID"], 0);
                        dr["SerialNum"] = CastDbNull.To(reader["DLZ58SN"], String.Empty).Trim();
                        dr["ItemNum"] = CastDbNull.To<decimal>(reader["DLITM"], 0);
                        dr["LotNum"] = CastDbNull.To(reader["DLLOTN"], String.Empty).Trim();
                        dr["StatusCode"] = CastDbNull.To(reader["DLZ58DST"], String.Empty).Trim();
                        dr["QtyOnHand"] = CastDbNull.To<decimal>(reader["DLPQOH"], 0) / 1000;
                        dr["Side"] = CastDbNull.To(reader["DLZ58SD"], String.Empty).Trim();
                        dr["Stack"] = CastDbNull.To<decimal>(reader["DLZ58STK"], 0);
                        dr["Row"] = CastDbNull.To<decimal>(reader["DLZ58ROW"], 0);
                        dr["Slot"] = CastDbNull.To<decimal>(reader["DLZ58SL"], 0);
                        dr["Position"] = CastDbNull.To<decimal>(reader["DLZ58PS"], 0);
                        dr["Rotated"] = CastDbNull.To(reader["DLZ58BXRO"], String.Empty).Trim();
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["DLMUPM"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["DLUPMT"], 0);
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        Rows.Add(dr);
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
