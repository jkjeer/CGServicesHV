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
    public class ItemAttributeMasterTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemAttributeMasterTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemAttributeMasterLedger]");
            Columns.Add("AttributeName", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("SegmentNum", typeof(decimal));
            Columns.Add("ProductCode", typeof(string));
            Columns.Add("UserCode", typeof(string));
            Columns.Add("IsNumber", typeof(bool));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["AttributeName"] = reader.Get<string>("AMZ58ATNM");
                    dr["Description1"] = reader.Get<string>("AMDSC1");
                    dr["SegmentNum"] = reader.Get<decimal>("AMPEL");
                    dr["ProductCode"] = reader.Get<string>("AMSY");
                    dr["UserCode"] = reader.Get<string>("AMRT");
                    dr["IsNumber"] = reader.Get<string>("AMCNUM") == "Y";
                    dr["UserId"] = reader.Get<string>("AMUSER");
                    dr["ProgramId"] = reader.Get<string>("AMPID");
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(reader.Get<decimal>("AMUPMJ"), reader.Get<decimal>("AMTDAY"));
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

    }
}
