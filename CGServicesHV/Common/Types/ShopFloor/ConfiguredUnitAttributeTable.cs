using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ConfiguredUnitAttributeTable : DataTable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime?>("TransactionDate")).Max() ?? new DateTime(1900, 1, 1); }
            set { }
        }

        public ConfiguredUnitAttributeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ConfiguredUnitAttributeLedger]");
            ExtendedProperties.Add("BatchSize", "50000");
            Columns.Add("ConfigurationId", typeof(decimal));
            Columns.Add("SegmentNum", typeof(decimal));
            Columns.Add("AttributeValue", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataRecord record, Plant plant)
        {
            try
            {
                var dr = NewRow();
                dr["ConfigurationId"] = record.Get<decimal>("KZCFGID");
                dr["SegmentNum"] = record.Get<decimal>("KZPEL");
                dr["AttributeValue"] = record.Get<string>("KZSGVL");
                dr["TransactionDate"] = E1TimeManager.CastToDateTime(record.Get<decimal>("KZUPMJ"), record.Get<decimal>("KZTDAY")).AddMinutes(plant.BusinessSystemOffset);
                Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AddFromDataWarehouse(IDataRecord record, Plant plant)
        {
            try
            {
                var dr = NewRow();
                dr["ConfigurationId"] = record.Get<decimal>("ConfigurationId");
                dr["SegmentNum"] = record.Get<decimal>("SegmentNum");
                dr["AttributeValue"] = record.Get<string>("AttributeValue");
                dr["TransactionDate"] = record.Get<DateTime>("TransactionDate");
                Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
