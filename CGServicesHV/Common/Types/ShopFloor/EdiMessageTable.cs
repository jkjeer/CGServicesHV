using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class EdiMessageTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public EdiMessageTable()
        {
            ExtendedProperties.Add("Target", "[ORD].[EDI_MESSAGE_LEDGER]");
            Columns.Add("ORDER_NUM", typeof(decimal));
            Columns.Add("ORDER_TYPE_CD", typeof(string));
            Columns.Add("ORDER_LINE_NUM", typeof(decimal));
            Columns.Add("MESSAGE_LINE_NUM", typeof(decimal));
            Columns.Add("MESSAGE_TYPE_CD", typeof(string));
            Columns.Add("MESSAGE_SEQ_NUM", typeof(decimal));
            Columns.Add("MESSAGE_TEXT", typeof(string));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                using (reader)
                {

                    while (reader.Read())
                    {
                        DataRow dr = NewRow();
                        dr["ORDER_NUM"] = Data.CastDbNull.To<decimal>(reader["ZDDOCO"], 0);
                        dr["ORDER_TYPE_CD"] = Data.CastDbNull.To(reader["ZDDCTO"], String.Empty).Trim();
                        dr["ORDER_LINE_NUM"] = Data.CastDbNull.To<decimal>(reader["ZDLNID"], 0) / 1000;
                        dr["MESSAGE_LINE_NUM"] = Data.CastDbNull.To<decimal>(reader["ZDLINS"], 0) / 100;
                        dr["MESSAGE_TYPE_CD"] = Data.CastDbNull.To(reader["ZDEDTY"], String.Empty).Trim();
                        dr["MESSAGE_SEQ_NUM"] = Data.CastDbNull.To<decimal>(reader["ZDEDSQ"], 0);
                        dr["MESSAGE_TEXT"] = Data.CastDbNull.To(reader["ZDTXLN"], String.Empty).Trim();
                        dr["UPDATE_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ZDUPMJ"], 0),
                            Data.CastDbNull.To<decimal>(reader["ZDTDAY"], 0)).AddHours(plant.TimeOffset);
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

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    foreach (var msg in line.F4715s)
                    {
                        DataRow dr = NewRow();
                        dr["ORDER_NUM"] = msg.ZDDOCO;
                        dr["ORDER_TYPE_CD"] = msg.ZDDCTO;
                        dr["ORDER_LINE_NUM"] = msg.ZDLNID/1000;
                        dr["MESSAGE_LINE_NUM"] = msg.ZDLINS;
                        dr["MESSAGE_TYPE_CD"] = msg.ZDEDTY;
                        dr["MESSAGE_SEQ_NUM"] = msg.ZDEDSQ;
                        dr["MESSAGE_TEXT"] = msg.ZDTXLN;
                        dr["UPDATE_DATE"] = E1TimeManager.CastToDateTime(msg.ZDUPMJ, msg.ZDTDAY);
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new FaultException(e.Message);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class EdiMessageEbsTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public EdiMessageEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[EdiMessage]");
            Columns.Add("WorkOrderId", typeof(int));
            Columns.Add("OrderNum", typeof(decimal));
            Columns.Add("OrderType", typeof(string));
            Columns.Add("OrderLineNum", typeof(decimal));
            Columns.Add("MessageLineNum", typeof(decimal));
            Columns.Add("MessageType", typeof(string));
            Columns.Add("MessageSeqNum", typeof(decimal));
            Columns.Add("MessageText", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    foreach (var msg in line.F4715s)
                    {
                        DataRow dr = NewRow();
                        dr["WorkOrderId"] = line.WorkOrderId;
                        dr["OrderNum"] = msg.ZDDOCO;
                        dr["OrderType"] = msg.ZDDCTO;
                        dr["OrderLineNum"] = msg.ZDLNID / 1000;
                        dr["MessageLineNum"] = msg.ZDLINS;
                        dr["MessageType"] = msg.ZDEDTY;
                        dr["MessageSeqNum"] = msg.ZDEDSQ;
                        dr["MessageText"] = msg.ZDTXLN;
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(msg.ZDUPMJ, msg.ZDTDAY);
                        Rows.Add(dr);
                    }
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
