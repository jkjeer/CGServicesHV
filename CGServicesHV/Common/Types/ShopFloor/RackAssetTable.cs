using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackAssetTable : DataTable
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

        public RackAssetTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackAssetLedger]");
            Columns.Add("AssetId", typeof(string));
            Columns.Add("Plant", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("Status", typeof(string));
            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("CustomerShipNum", typeof(decimal));
            Columns.Add("AssetType", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("CreationDate", typeof(DateTime));
            Columns.Add("RelabelDate", typeof(DateTime));
            Columns.Add("ShipDate", typeof(DateTime));
            Columns.Add("ReceivedDate", typeof(DateTime));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("TransactionType", typeof(string));
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
                        dr["AssetId"] = Data.CastDbNull.To(reader["ALZ58AID"], String.Empty).Trim();
                        dr["Plant"] = Data.CastDbNull.To(reader["ALMCU"], String.Empty).Trim();
                        dr["Location"] = Data.CastDbNull.To(reader["ALLOCN"], String.Empty).Trim();
                        dr["Status"] = Data.CastDbNull.To(reader["ALZ58AST"], String.Empty).Trim();
                        dr["CustomerSoldNum"] = Data.CastDbNull.To<decimal>(reader["ALAN8"], 0);
                        dr["CustomerShipNum"] = Data.CastDbNull.To<decimal>(reader["ALSHAN"], 0);
                        dr["AssetType"] = Data.CastDbNull.To(reader["ALZ58ATY"], String.Empty).Trim();
                        dr["RackType"] = Data.CastDbNull.To(reader["ALZ58RT"], String.Empty).Trim();
                        dr["RackId"] = Data.CastDbNull.To<decimal>(reader["ALZ58RID"], 0);
                        dr["CreationDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ALZ58ACD"], 0));
                        dr["RelabelDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ALZ58RDT"], 0));
                        dr["ShipDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ALSHPJ"], 0));
                        dr["ReceivedDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ALRCDJ"], 0));
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["ALMUPM"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["ALTDAY"], 0);
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        dr["TransactionId"] = Data.CastDbNull.To<decimal>(reader["ALUKID"], 0);
                        dr["TransactionType"] = Data.CastDbNull.To(reader["ALIOPR"], String.Empty).Trim();
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
