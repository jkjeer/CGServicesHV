using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class RackAssetTypeTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public RackAssetTypeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackAssetTypeLedger]");
            Columns.Add("AssetTypeCode", typeof(string));
            Columns.Add("AssetTypeDesc", typeof(string));
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
                        dr["AssetTypeCode"] = Data.CastDbNull.To(reader["DRKY"], string.Empty).Trim();
                        dr["AssetTypeDesc"] = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["DRUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["DRUPMT"], 0);
                        dr["TransactionDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        ;
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
