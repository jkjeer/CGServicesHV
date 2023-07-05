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
                        dr["PlantID"] = CastDbNull.To(reader["MCRP01"], string.Empty).Trim();
                        dr["AddressNum"] = Convert.ToDouble(CastDbNull.To(reader["ABALKY"], "0"));
                        dr["AlternateNum"] = CastDbNull.To<decimal>(reader["ABAN8"], 0).ToString();
                        dr["Name"] = CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        dr["HomeCompany"] = CastDbNull.To(reader["MCCO"], string.Empty).Trim();
                        dr["HomeCompanyDesc"] = CastDbNull.To(reader["MCDL01"], string.Empty).Trim();
                        dr["Company"] = CastDbNull.To(reader["MCRP02"], string.Empty).Trim();
                        dr["HomeCostCenter"] = CastDbNull.To(reader["ABMCU"], string.Empty).Trim();
                        dr["CostCenter"] = CastDbNull.To(reader["ABMCU"], string.Empty).Trim();
                        dr["UnionCode"] = CastDbNull.To(reader["MCRP01"], string.Empty).Trim();
                        dr["PayStatus"] = CastDbNull.To(reader["ABAT1"], string.Empty).Trim();
                        var updateDateId = CastDbNull.To<decimal>(reader["ABUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<decimal>(reader["ABUPMT"], 0);
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
