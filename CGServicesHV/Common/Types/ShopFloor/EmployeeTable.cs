using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class EmployeeTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public EmployeeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[EmployeeLedger]");
            Columns.Add("PlantID", typeof(string));
            Columns.Add("AddressNum", typeof(decimal));
            Columns.Add("AlternateNum", typeof(string));
            Columns.Add("Name", typeof(string));
            Columns.Add("HomeCompany", typeof(string));
            Columns.Add("HomeCompanyDesc", typeof(string));
            Columns.Add("Company", typeof(string));
            Columns.Add("HomeCostCenter", typeof(string));
            Columns.Add("CostCenter", typeof(string));
            Columns.Add("UnionCode", typeof(string));
            Columns.Add("PayStatus", typeof(string));
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
                        dr["PlantID"] = Data.CastDbNull.To(reader["MCRP01"], string.Empty).Trim();
                        dr["AddressNum"] = Convert.ToDouble(Data.CastDbNull.To(reader["ABALKY"], "0"));
                        dr["AlternateNum"] = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0).ToString();
                        dr["Name"] = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        dr["HomeCompany"] = Data.CastDbNull.To(reader["MCCO"], string.Empty).Trim();
                        dr["HomeCompanyDesc"] = Data.CastDbNull.To(reader["MCDL01"], string.Empty).Trim();
                        dr["Company"] = Data.CastDbNull.To(reader["MCRP02"], string.Empty).Trim();
                        dr["HomeCostCenter"] = Data.CastDbNull.To(reader["ABMCU"], string.Empty).Trim();
                        dr["CostCenter"] = Data.CastDbNull.To(reader["ABMCU"], string.Empty).Trim();
                        dr["UnionCode"] = Data.CastDbNull.To(reader["MCRP01"], string.Empty).Trim();
                        dr["PayStatus"] = Data.CastDbNull.To(reader["ABAT1"], string.Empty).Trim();
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["ABUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["ABUPMT"], 0);
                        dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        Rows.Add(dr);
                    }

                    reader.Close();
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
