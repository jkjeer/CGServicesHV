using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class ScheduleHeaderTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("TransactionId")).Max(); }
            set { }
        }

        public ScheduleHeaderTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ScheduleHeaderLedger]");
            Columns.Add("EdiBatchNumber", typeof(string));
            Columns.Add("CostCenter", typeof(string));
            Columns.Add("ScheduleNumber", typeof(decimal));
            Columns.Add("ScheduleDate", typeof(DateTime));
            Columns.Add("ShopFloorSystem", typeof(string));
            Columns.Add("StatusCodeTransaction", typeof(string));
            Columns.Add("EdiSuccessfullyProcess", typeof(string));
            Columns.Add("MathNumeric06", typeof(decimal));
            Columns.Add("OperationStatus", typeof(string));
            Columns.Add("Description50Characters", typeof(string));
            Columns.Add("UserReservedNumber", typeof(decimal));
            Columns.Add("UserReservedCode", typeof(string));
            Columns.Add("UserReservedDate", typeof(DateTime));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("WorkStationId", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("E1Boxed", typeof(string));
            Columns.Add("E1Racked", typeof(string));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                using (reader)
                {

                    while (reader.Read())
                    {
                        DataRow dr = this.NewRow();
                        dr["EdiBatchNumber"] = reader.Get<string>("S6EDBT");
                        dr["CostCenter"] = reader.Get<string>("S6MCU");
                        dr["ScheduleNumber"] = reader.Get<decimal>("S6Z58SCN");
                        var scheduleDateId = Data.CastDbNull.To<decimal>(reader["S6Z58SCD"], 0);
                        dr["ScheduleDate"] = E1TimeManager.CastToDateTime(scheduleDateId);
                        dr["ShopFloorSystem"] = reader.Get<string>("S6Z58SFS");
                        dr["StatusCodeTransaction"] = reader.Get<string>("S6TSC");
                        dr["EdiSuccessfullyProcess"] = reader.Get<string>("S6EDSP");
                        dr["MathNumeric06"] = reader.Get<decimal>("S6MATH06");
                        dr["OperationStatus"] = reader.Get<string>("S6WOPST");
                        dr["Description50Characters"] = reader.Get<string>("S6DS50");
                        dr["UserReservedNumber"] = reader.Get<decimal>("S6URAB");
                        dr["UserReservedCode"] = reader.Get<string>("S6URCD");
                        var userReservedDateId = Data.CastDbNull.To<decimal>(reader["S6URDT"], 0);
                        dr["UserReservedDate"] = E1TimeManager.CastToDateTime(userReservedDateId);
                        dr["UserId"] = reader.Get<string>("S6USER");
                        dr["ProgramId"] = reader.Get<string>("S6PID");
                        dr["WorkStationId"] = reader.Get<string>("S6JOBN");
                        dr["E1Boxed"] = reader.Get<string>("E1Boxed");
                        dr["E1Racked"] = reader.Get<string>("E1Racked");
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["S6UPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["S6UPMT"], 0);
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        this.Rows.Add(dr);
                    }

                    reader.Close();
                    Logger.Trace($"Rows: {this.Rows.Count}");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
