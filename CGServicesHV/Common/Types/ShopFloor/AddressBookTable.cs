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

    public class AddressBookTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public AddressBookTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[AddressBookLedger]");
            Columns.Add("AddressBookLedgerID", typeof(decimal));
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
            Columns.Add("ABDate", typeof(DateTime));
            Columns.Add("ALDate", typeof(DateTime));
            Columns.Add("MADate", typeof(DateTime));
            Columns.Add("WPDate", typeof(DateTime));
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
                        var updateDateId1 = Data.CastDbNull.To<decimal>(reader["ABUPMJ"], 0);
                        var updateTimeId1 = Data.CastDbNull.To<decimal>(reader["ABUPMT"], 0);
                        var updateDateId2 = Data.CastDbNull.To<decimal>(reader["ALUPMJ"], 0);
                        var updateTimeId2 = Data.CastDbNull.To<decimal>(reader["ALUPMT"], 0);
                        var updateDateId3 = Data.CastDbNull.To<decimal>(reader["WPUPMJ"], 0);
                        var updateTimeId3 = Data.CastDbNull.To<decimal>(reader["WPUPMT"], 0);
                        //var updateDateId4 = Data.CastDbNull.To<decimal>(reader["DRUPMJ"], 0);
                        //var updateTimeId4 = Data.CastDbNull.To<decimal>(reader["DRUPMT"], 0);
                        var updateDateId5 = Data.CastDbNull.To<decimal>(reader["MAUPMJ"], 0);
                        var updateTimeId5 = Data.CastDbNull.To<decimal>(reader["MAUPMT"], 0);
                        var updateDate1 = Utilities.Common.CastToDateTime(updateDateId1, updateTimeId1);
                        var updateDate2 = Utilities.Common.CastToDateTime(updateDateId2, updateTimeId2);
                        var updateDate3 = Utilities.Common.CastToDateTime(updateDateId3, updateTimeId3);
                        //var updateDate4 = Utilities.Common.CastToDateTime(updateDateId4, updateTimeId4);
                        var updateDate5 = Utilities.Common.CastToDateTime(updateDateId5, updateTimeId5);
                        //var updateDate = new[] {updateDate1, updateDate2, updateDate3, updateDate4, updateDate5 }.Max();
                        var updateDate = new[] { updateDate1, updateDate2, updateDate3, updateDate5 }.Max();
                        dr["UpdateDate"] = updateDate.AddHours(plant.TimeOffset);
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
