using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class PlantTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public PlantTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[PlantLedger]");
            Columns.Add("PlantId", typeof(int));
            Columns.Add("AlphaCode", typeof(string));
            Columns.Add("NumericCode", typeof(string));
            Columns.Add("Name", typeof(string));
            Columns.Add("BusinessSystemNum", typeof(decimal));
            Columns.Add("Address1", typeof(string));
            Columns.Add("Address2", typeof(string));
            Columns.Add("Address3", typeof(string));
            Columns.Add("Address4", typeof(string));
            Columns.Add("PostalCode", typeof(string));
            Columns.Add("City", typeof(string));
            Columns.Add("State", typeof(string));
            Columns.Add("Country", typeof(string));
            Columns.Add("AreaCode", typeof(string));
            Columns.Add("Phone", typeof(string));
            Columns.Add("Company", typeof(string));
            Columns.Add("Division", typeof(string));
            Columns.Add("TimeZone", typeof(string));
            Columns.Add("TimeZoneName", typeof(string));
            Columns.Add("IsLocal", typeof(bool));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(Plant localPlant, Plants plants)
        {
            try
            {
                foreach (var plant in plants)
                {
                    var dr = NewRow();
                    dr["PlantId"] = plant.NumericValue;
                    dr["AlphaCode"] = plant.PlantCode;
                    dr["NumericCode"] = plant.PlantNum;
                    dr["Name"] = plant.Name;
                    dr["BusinessSystemNum"] = plant.AddressNum;
                    dr["Address1"] = plant.Address?.Address1;
                    dr["Address2"] = plant.Address?.Address2;
                    dr["Address3"] = plant.Address?.Address3;
                    dr["Address4"] = plant.Address?.Address4;
                    dr["PostalCode"] = plant.Address?.PostalCode;
                    dr["City"] = plant.Address?.City;
                    dr["State"] = plant.Address?.State;
                    dr["Country"] = plant.Address?.Country;
                    dr["AreaCode"] = "";
                    dr["Phone"] = "";
                    dr["Company"] = plant.CompanyNum;
                    dr["Division"] = "";
                    dr["TimeZone"] = plant.TimeZone;
                    dr["TimeZoneName"] = plant.TimeZoneName;
                    dr["IsLocal"] = localPlant.PlantCode == plant.PlantCode;
                    dr["TransactionDate"] = DateTime.Now.AddHours(plant.TimeOffset);
                    Rows.Add(dr);
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
