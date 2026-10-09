using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class CustomerTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public CustomerTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[CustomerLedger]");
            Columns.Add("CustomerLedgerID", typeof(decimal));
            Columns.Add("AddressNum", typeof(decimal));
            Columns.Add("AddressType", typeof(string));
            Columns.Add("AddressTypeName", typeof(string));
            Columns.Add("Name", typeof(string));
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
            Columns.Add("ParentAddressNum", typeof(decimal));
            Columns.Add("NationalAccountGroup", typeof(string));
            Columns.Add("Company", typeof(string));
            Columns.Add("UpdateDate", typeof(DateTime));
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
                        var addressNum = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0);
                        dr["AddressNum"] = addressNum;
                        dr["Name"] = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        dr["AddressType"] = Data.CastDbNull.To(reader["ABAT1"], string.Empty).Trim();
                        dr["AddressTypeName"] = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                        dr["NationalAccountGroup"] = Data.CastDbNull.To(reader["ABAC05"], string.Empty).Trim();
                        dr["Address1"] = Data.CastDbNull.To(reader["ALADD1"], string.Empty).Trim();
                        dr["Address2"] = Data.CastDbNull.To(reader["ALADD2"], string.Empty).Trim();
                        dr["Address3"] = Data.CastDbNull.To(reader["ALADD3"], string.Empty).Trim();
                        dr["Address4"] = Data.CastDbNull.To(reader["ALADD4"], string.Empty).Trim();
                        dr["PostalCode"] = Data.CastDbNull.To(reader["ALADDZ"], string.Empty).Trim();
                        dr["City"] = Data.CastDbNull.To(reader["ALCTY1"], string.Empty).Trim();
                        dr["State"] = Data.CastDbNull.To(reader["ALADDS"], string.Empty).Trim();
                        dr["Country"] = Data.CastDbNull.To(reader["ALCTR"], string.Empty).Trim();
                        dr["AreaCode"] = Data.CastDbNull.To(reader["WPAR1"], string.Empty).Trim();
                        dr["Phone"] = Data.CastDbNull.To(reader["WPPH1"], string.Empty).Trim();
                        var parentAddressNum = Data.CastDbNull.To<decimal>(reader["MAPA8"], 0);
                        dr["ParentAddressNum"] = (parentAddressNum == 0) ? addressNum : parentAddressNum;
                        var company = Data.CastDbNull.To(reader["ABAC29"], string.Empty).Trim();
                        dr["Company"] = (company == "") ? 0 : Convert.ToDecimal(company);
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["ABUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["ABUPMT"], 0);
                        dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        ;
                        Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new FaultException(e.Message);
            }
        }
    }
}
