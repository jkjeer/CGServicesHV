using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class WorkOrderStatusTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        public WorkOrderStatusTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[WorkOrderStatusLedger]");
            Columns.Add("WorkOrderNum", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));
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
                        var dr = NewRow();
                        dr["WorkOrderNum"] = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                        dr["StatusCode"] = Data.CastDbNull.To(reader["WASRST"], string.Empty);
                        dr["TransactionId"] = Data.CastDbNull.To<decimal>(reader["WAUKID"], 0);
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["WAMUPM"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["WAUPMT"], 0);
                        dr["TransactionDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
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
