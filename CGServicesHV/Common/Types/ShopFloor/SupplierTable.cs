using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    class SupplierTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public SupplierTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SupplierLedger]");
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
                        dr["ParentAddressNum"] = parentAddressNum == 0 ? addressNum : parentAddressNum;
                        var company = Data.CastDbNull.To(reader["ABAC29"], string.Empty).Trim();
                        dr["Company"] = company == "" ? 0 : Convert.ToDecimal(company);
                        var dates = new List<DateTime>();
                        var updateDateIdAB = Data.CastDbNull.To<decimal>(reader["ABUPMJ"], 0);
                        var updateTimeIdAB = Data.CastDbNull.To<decimal>(reader["ABUPMT"], 0);
                        var updateDateAB = Utilities.Common.CastToDateTime(updateDateIdAB, updateTimeIdAB);
                        if (updateDateAB < DateTime.Now) dates.Add(updateDateAB);
                        var updateDateIdAL = Data.CastDbNull.To<decimal>(reader["ALUPMJ"], 0);
                        var updateTimeIdAL = Data.CastDbNull.To<decimal>(reader["ALUPMT"], 0);
                        var updateDateAL = Utilities.Common.CastToDateTime(updateDateIdAL, updateTimeIdAL);
                        if (updateDateAL < DateTime.Now) dates.Add(updateDateAL);
                        var updateDateIdA6 = Data.CastDbNull.To<decimal>(reader["A6UPMJ"], 0);
                        var updateTimeIdA6 = Data.CastDbNull.To<decimal>(reader["A6UPMT"], 0);
                        var updateDateA6 = Utilities.Common.CastToDateTime(updateDateIdA6, updateTimeIdA6);
                        if (updateDateA6 < DateTime.Now) dates.Add(updateDateA6);
                        //var updateDateIdDR = Data.CastDbNull.To<decimal>(reader["DRUPMJ"], 0);
                        //var updateTimeIdDR = Data.CastDbNull.To<decimal>(reader["DRUPMT"], 0);
                        //var updateDateDR = Utilities.Common.CastToDateTime(updateDateIdDR, updateTimeIdDR);
                        //if (updateDateDR < DateTime.Now) dates.Add(updateDateDR);
                        var updateDateIdWP = Data.CastDbNull.To<decimal>(reader["WPUPMJ"], 0);
                        var updateTimeIdWP = Data.CastDbNull.To<decimal>(reader["WPUPMT"], 0);
                        var updateDateWP = Utilities.Common.CastToDateTime(updateDateIdWP, updateTimeIdWP);
                        if (updateDateWP < DateTime.Now) dates.Add(updateDateWP);
                        var maxDate = dates.FirstOrDefault(s => s.Date == dates.Max(x => x.Date));
                        dr["UpdateDate"] = maxDate.AddHours(plant.TimeOffset);
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
