using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ScheduleTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ScheduleTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ScheduleLedger]");
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("ScheduleDate", typeof(DateTime));
            Columns.Add("ScheduleType", typeof(string));
            Columns.Add("BatchNum", typeof(string));
            Columns.Add("SystemCode", typeof(string));
            Columns.Add("SystemName", typeof(string));
            Columns.Add("DueDate", typeof(DateTime));
            Columns.Add("Status", typeof(string));
            Columns.Add("Lines", typeof(int));
            Columns.Add("Quantity", typeof(int));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                using (reader)
                {

                    while (reader.Read())
                    {
                        DataRow dr = this.NewRow();
                        dr["ScheduleNum"] = CastDbNull.To<decimal>(reader["S1Z58SCN"], 0);
                        dr["ScheduleDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["S1Z58SCD"], 0));
                        dr["DueDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0));
                        dr["BatchNum"] = CastDbNull.To<string>(reader["S6EDBT"], String.Empty).Trim();
                        dr["SystemCode"] = CastDbNull.To<string>(reader["S1Z58SFS"], String.Empty).Trim();
                        dr["SystemName"] = CastDbNull.To<string>(reader["S1Z58SFS_DESC"], String.Empty).Trim();
                        dr["ScheduleType"] = CastDbNull.To<string>(reader["S1Z58SCTYP"], String.Empty).Trim();
                        dr["Status"] = CastDbNull.To<string>(reader["S1Z58SCSTS"], String.Empty).Trim();
                        dr["Lines"] = 0;
                        dr["Quantity"] = 0;
                        dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["S1UPMJ"], 0),
                            Data.CastDbNull.To<decimal>(reader["S1TDAY"], 0));
                        this.Rows.Add(dr);
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

        public void Add(Schedule schedule)
        {
            try
            {
                DataRow dr = this.NewRow();
                dr["BatchNum"] = schedule.f580020.S6EDBT;
                dr["ScheduleNum"] = schedule.f580020.S6Z58SCN;
                dr["ScheduleDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(schedule.f580020.S6Z58SCD, 0));
                dr["DueDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(schedule.ScheduleLines.First().F580021.S7Z58SCDDJ, 0));
                dr["SystemCode"] = schedule.f580020.S6Z58SFS;
                dr["SystemName"] = schedule.f580020.S6Z58SFS_NAME;
                dr["ScheduleType"] = schedule.f5834A01.S1Z58SCTYP;
                dr["Status"] = schedule.f5834A01.S1Z58SCSTS;
                dr["Lines"] = schedule.ScheduleLines.Count;
                dr["Quantity"] = schedule.ScheduleLines.Sum(z => z.F580021.S7Z58QTY);
                dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(schedule.f5834A01.S1UPMJ, 0),
                    Data.CastDbNull.To<decimal>(schedule.f5834A01.S1TDAY, 0));
                this.Rows.Add(dr);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }

}
