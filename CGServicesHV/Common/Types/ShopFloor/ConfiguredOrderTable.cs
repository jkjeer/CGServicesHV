using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ConfiguredOrderTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ConfiguredOrderTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ConfiguredOrderLedger]");
            ExtendedProperties.Add("BatchSize", "50000");
            Columns.Add("ConfigurationId", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("OrderNum", typeof(decimal));
            Columns.Add("OrderType", typeof(string));
            Columns.Add("OrderLineNum", typeof(decimal));
            Columns.Add("OrderCompany", typeof(string));
            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("CustomerShipNum", typeof(decimal));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataRecord record, Plant plant)
        {
            try
            {
                var dr = NewRow();
                dr["ConfigurationId"] = record.Get<decimal>("CMCFGID");
                dr["ItemNum"] = record.Get<decimal>("CCITM");
                dr["OrderNum"] = record.Get<decimal>("CMDOCO");
                dr["OrderType"] = record.Get<string>("CMDCTO");
                dr["OrderLineNum"] = record.Get<decimal>("CMLNID") / 1000;
                dr["OrderCompany"] = record.Get<string>("CMKCOO");
                dr["CustomerSoldNum"] = record.Get<decimal>("CMAN8");
                dr["CustomerShipNum"] = record.Get<decimal>("CMSHAN");
                dr["UserId"] = record.Get<string>("CMUSER");
                dr["ProgramId"] = record.Get<string>("CMPID");
                dr["TransactionDate"] = E1TimeManager.CastToDateTime(record.Get<decimal>("CMUPMJ"), record.Get<decimal>("CMTDAY")).AddMinutes(plant.BusinessSystemOffset); Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
