using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class RackAssetXrefTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public RackAssetXrefTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackAssetXrefLedger]");
            Columns.Add("RackTypeCode", typeof(string));
            Columns.Add("AssetTypeCode", typeof(string));
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
                        dr["RackTypeCode"] = Data.CastDbNull.To(reader["ACZ58RT"], string.Empty).Trim();
                        dr["AssetTypeCode"] = Data.CastDbNull.To(reader["ACZ58ATY"], string.Empty).Trim();
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["ACUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["ACUPMT"], 0);
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
