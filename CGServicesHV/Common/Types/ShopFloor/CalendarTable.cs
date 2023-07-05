using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class CalendarTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public CalendarTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[CalendarLedger]");
            Columns.Add("CalendarName", typeof(string));
            Columns.Add("CalendarDate", typeof(DateTime));
            Columns.Add("CalendarCode", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["CalendarName"] = CastDbNull.To(reader["CZWDCT"], string.Empty).Trim();
                    dr["CalendarCode"] = CastDbNull.To(reader["VAL"], string.Empty).Trim();
                    var ctry = CastDbNull.To<decimal>(reader["CZCTRY"], 0);
                    var yr = CastDbNull.To<decimal>(reader["CZYR"], 0);
                    var mt = CastDbNull.To<decimal>(reader["CZMT"], 0);
                    var dy = CastDbNull.To<Int32>(reader["NAME"], 0);
                    dr["CalendarDate"] = new DateTime((int)(1900 + (ctry * 100) + yr), (int)mt, dy);
                    var tranDateId = CastDbNull.To<decimal>(reader["CZUPMJ"], 0);
                    var tranTimeId = CastDbNull.To<decimal>(reader["CZTDAY"], 0);
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(tranDateId, tranTimeId).AddMinutes(plant.BusinessSystemOffset);
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
