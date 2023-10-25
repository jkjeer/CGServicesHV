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
    public class ConfiguredAttributeTable : DataTable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime?>("TransactionDate")).Max() ?? new DateTime(1900, 1, 1); }
            set { }
        }

        public ConfiguredAttributeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ConfiguredAttributeLedger]");
            ExtendedProperties.Add("BatchSize", "25000");
            Columns.Add("ConfigId", typeof(decimal));
            Columns.Add("ComponentId", typeof(decimal));
            Columns.Add("SegmentNum", typeof(decimal));
            Columns.Add("Value", typeof(string));
            Columns.Add("Name", typeof(string));
            Columns.Add("IsNumber", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataRecord record, Plant plant)
        {
            try
            {
                var dr = NewRow();
                dr["ConfigId"] = record.Get<decimal>("KZCFGID");
                dr["ComponentId"] = record.Get<decimal>("KZCFGCID");
                dr["SegmentNum"] = record.Get<decimal>("KZPEL");
                dr["Value"] = record.Get<string>("KZSGVL");
                dr["Name"] = record.Get<string>("AMZ58ATNM");
                dr["IsNumber"] = record.Get<string>("AMCNUM");
                dr["TransactionDate"] = E1TimeManager.CastToDateTime(record.Get<decimal>("KZUPMJ"), record.Get<decimal>("KZTDAY")).AddMinutes(plant.BusinessSystemOffset);
                Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


    }
}
