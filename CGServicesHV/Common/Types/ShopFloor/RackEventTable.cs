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
    public class RackEventTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("RackEventId")).Max(); }
            set { }
        }

        public RackEventTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackEventLedger]");
            Columns.Add("RackEventId", typeof(decimal));
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("RackType", typeof(string));
            Columns.Add("StatusCode", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("ReasonCode", typeof(string));
            Columns.Add("GlassWeight", typeof(decimal));
            Columns.Add("TareWeight", typeof(decimal));
            Columns.Add("BoxNum", typeof(decimal));
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("AssetId", typeof(string));
            Columns.Add("OperationCode", typeof(string));
            Columns.Add("ReceivedDate", typeof(DateTime));
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
                        dr["RackEventId"] = Data.CastDbNull.To<decimal>(reader["HLUKID"], 0);
                        dr["RackId"] = Data.CastDbNull.To<decimal>(reader["HLZ58RID"], 0);
                        dr["RackType"] = Data.CastDbNull.To(reader["HLZ58RT"], String.Empty).Trim();
                        dr["StatusCode"] = Data.CastDbNull.To(reader["HLZ58HST"], String.Empty).Trim();
                        dr["Location"] = Data.CastDbNull.To(reader["HLLOCN"], String.Empty).Trim();
                        dr["ReasonCode"] = Data.CastDbNull.To(reader["HLURCD"], String.Empty).Trim();
                        dr["BoxNum"] = Data.CastDbNull.To<decimal>(reader["HLZ58BOX"], 0);
                        dr["ScheduleNum"] = Data.CastDbNull.To<decimal>(reader["HLZ58SCN"], 0);
                        dr["AssetId"] = Data.CastDbNull.To(reader["HLURRF"], String.Empty).Trim();
                        dr["OperationCode"] = Data.CastDbNull.To(reader["HLIOPR"], String.Empty).Trim();
                        dr["TareWeight"] = Data.CastDbNull.To<decimal>(reader["HLZ58RTWT"], 0) / 100;
                        dr["GlassWeight"] = Data.CastDbNull.To<decimal>(reader["HLZ58GWTT"], 0) / 100;
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["HLMUPM"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["HLUPMT"], 0);
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
