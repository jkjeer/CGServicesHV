using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class CustomerShipPreferenceTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public CustomerShipPreferenceTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[CustomerShipPreferenceLedger]");
            Columns.Add("ShipToCustNum", typeof(decimal));
            Columns.Add("Code01", typeof(string));
            Columns.Add("Code02", typeof(string));
            Columns.Add("Code03", typeof(string));
            Columns.Add("Code04", typeof(string));
            Columns.Add("Code05", typeof(string));
            Columns.Add("Code06", typeof(string));
            Columns.Add("Code07", typeof(string));
            Columns.Add("Code08", typeof(string));
            Columns.Add("Code09", typeof(string));
            Columns.Add("Code10", typeof(string));
            Columns.Add("Code11", typeof(string));
            Columns.Add("Code12", typeof(string));
            Columns.Add("Code13", typeof(string));
            Columns.Add("Code14", typeof(string));
            Columns.Add("Code15", typeof(string));
            Columns.Add("DeliveryInstruction01", typeof(string));
            Columns.Add("DeliveryInstruction02", typeof(string));
            Columns.Add("UnitType", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ShipToCustNum"] = CastDbNull.To<decimal>(reader["CPSHAN"], 0);
                    dr["Code01"] = CastDbNull.To<string>(reader["CPZ58CCV01"], String.Empty).Trim();
                    dr["Code02"] = CastDbNull.To<string>(reader["CPZ58CCV02"], String.Empty).Trim();
                    dr["Code03"] = CastDbNull.To<string>(reader["CPZ58CCV03"], String.Empty).Trim();
                    dr["Code04"] = CastDbNull.To<string>(reader["CPZ58CCV04"], String.Empty).Trim();
                    dr["Code05"] = CastDbNull.To<string>(reader["CPZ58CCV05"], String.Empty).Trim();
                    dr["Code06"] = CastDbNull.To<string>(reader["CPZ58CCV06"], String.Empty).Trim();
                    dr["Code07"] = CastDbNull.To<string>(reader["CPZ58CCV07"], String.Empty).Trim();
                    dr["Code08"] = CastDbNull.To<string>(reader["CPZ58CCV08"], String.Empty).Trim();
                    dr["Code09"] = CastDbNull.To<string>(reader["CPZ58CCV09"], String.Empty).Trim();
                    dr["Code10"] = CastDbNull.To<string>(reader["CPZ58CCV10"], String.Empty).Trim();
                    dr["Code11"] = CastDbNull.To<string>(reader["CPZ58CCV11"], String.Empty).Trim();
                    dr["Code12"] = CastDbNull.To<string>(reader["CPZ58CCV12"], String.Empty).Trim();
                    dr["Code13"] = CastDbNull.To<string>(reader["CPZ58CCV13"], String.Empty).Trim();
                    dr["Code14"] = CastDbNull.To<string>(reader["CPZ58CCV14"], String.Empty).Trim();
                    dr["Code15"] = CastDbNull.To<string>(reader["CPZ58CCV15"], String.Empty).Trim();
                    dr["DeliveryInstruction01"] = CastDbNull.To<string>(reader["CPDEL1"], String.Empty).Trim();
                    dr["DeliveryInstruction02"] = CastDbNull.To<string>(reader["CPDEL2"], String.Empty).Trim();
                    dr["UnitType"] = CastDbNull.To<string>(reader["CPZ5860010"], String.Empty).Trim();
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["CPUPMJ"], 0), CastDbNull.To<decimal>(reader["CPUPMT"], 0)).AddMinutes(plant.BusinessSystemOffset);
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
