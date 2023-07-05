using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.Enterprise;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class AccountTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public AccountTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[AccountLedger]");
            Columns.Add("AccountCode", typeof(string));
            Columns.Add("AccountKey", typeof(string));
            Columns.Add("CompanyCode", typeof(string));
            Columns.Add("CostCenterCode", typeof(string));
            Columns.Add("ObjectCode", typeof(string));
            Columns.Add("SubsidiaryCode", typeof(string));
            Columns.Add("LevelNum", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["AccountCode"] = reader.Get<string>("GMAID");
                    dr["AccountKey"] = $"{reader.Get<string>("GMCO")}.{reader.Get<string>("GMMCU")}.{reader.Get<string>("GMOBJ")}.{reader.Get<string>("GMSUB")}";
                    dr["CompanyCode"] = reader.Get<string>("GMCO");
                    dr["CostCenterCode"] = reader.Get<string>("GMMCU");
                    dr["ObjectCode"] = reader.Get<string>("GMOBJ");
                    dr["SubsidiaryCode"] = reader.Get<string>("GMSUB");
                    dr["LevelNum"] = reader.Get<string>("GMLDA");
                    dr["Description1"] = reader.Get<string>("GMDL01");
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(reader.Get<decimal>("GMUPMJ"), reader.Get<decimal>("GMUPMT")).AddMinutes(plant.BusinessSystemOffset);
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
