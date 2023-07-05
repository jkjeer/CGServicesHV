using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.Enterprise;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class BusinessUnitTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public BusinessUnitTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[BusinessUnitLedger]");
            Columns.Add("BusinessUnitCode", typeof(string));
            Columns.Add("Company", typeof(string));
            Columns.Add("TypeCode", typeof(string));
            Columns.Add("Description", typeof(string));
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
                    dr["BusinessUnitCode"] = reader.Get<string>("MCMCU");
                    dr["Company"] = reader.Get<string>("MCCO");
                    dr["TypeCode"] = reader.Get<string>("MCSTYL");
                    dr["Description"] = reader.Get<string>("MCDC");
                    dr["Description1"] = reader.Get<string>("MCDL01");
                    var updateDateId = reader.Get<decimal>("MCUPMJ");
                    var updateTimeId = reader.Get<decimal>("MCUPMT");
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddMinutes(plant.BusinessSystemOffset);
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

