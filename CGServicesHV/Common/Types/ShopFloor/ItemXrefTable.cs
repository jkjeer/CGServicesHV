using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ItemXrefTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemXrefTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemXrefLedger]");
            Columns.Add("EntityNum", typeof(decimal));
            Columns.Add("XrefType", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("PlantCode", typeof(string));
            Columns.Add("EntityPartCode", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("Description2", typeof(string));
            Columns.Add("SearchText", typeof(string));
            Columns.Add("EffectiveDate1", typeof(DateTime));
            Columns.Add("EffectiveDate2", typeof(DateTime));
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
                        dr["EntityNum"] = CastDbNull.To<decimal>(reader["IVAN8"], 0);
                        dr["XrefType"] = CastDbNull.To(reader["IVXRT"], String.Empty).Trim();
                        dr["ItemNum"] = CastDbNull.To<decimal>(reader["IVITM"], 0);
                        dr["PlantCode"] = CastDbNull.To(reader["IVMCU"], String.Empty).Trim();
                        dr["EntityPartCode"] = CastDbNull.To(reader["IVCITM"], String.Empty).Trim();
                        dr["Description1"] = CastDbNull.To(reader["IVDSC1"], String.Empty).Trim();
                        dr["Description2"] = CastDbNull.To(reader["IVDSC2"], String.Empty).Trim();
                        dr["SearchText"] = CastDbNull.To(reader["IVALN"], String.Empty).Trim();
                        dr["EffectiveDate1"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["IVEFTJ"], 0));
                        dr["EffectiveDate2"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["IVEXDJ"], 0));
                        dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["IVUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["IVTDAY"], 0)).AddHours(plant.TimeOffset);
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
