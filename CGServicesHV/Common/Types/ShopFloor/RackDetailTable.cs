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
    public class RackDetailTable : DataTable
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

        public RackDetailTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackDetailLedger]");
            Columns.Add("LotNum", typeof(string));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("QtyRecd", typeof(decimal));
            Columns.Add("Side", typeof(string));
            Columns.Add("Stack", typeof(decimal));
            Columns.Add("Row", typeof(decimal));
            Columns.Add("Slot", typeof(decimal));
            Columns.Add("Position", typeof(decimal));
            Columns.Add("Rotated", typeof(string));
            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("WOrderType", typeof(string));
            Columns.Add("SOrderNum", typeof(decimal));
            Columns.Add("SOrderType", typeof(string));
            Columns.Add("SOrderLineNum", typeof(decimal));
            Columns.Add("POrderNum", typeof(decimal));
            Columns.Add("POrderType", typeof(string));
            Columns.Add("POrderLineNum", typeof(decimal));
            Columns.Add("ManufacturedDate", typeof(DateTime));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("OperationCode", typeof(string));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("CreateDate", typeof(DateTime));

            Columns.Add("WOrderNumInit", typeof(decimal));
            Columns.Add("WOrderTypeInit", typeof(string));
            Columns.Add("SOrderNumInit", typeof(decimal));
            Columns.Add("SOrderTypeInit", typeof(string));
            Columns.Add("SOrderLineNumInit", typeof(decimal));
            Columns.Add("POrderNumInit", typeof(decimal));
            Columns.Add("POrderTypeInit", typeof(string));
            Columns.Add("POrderLineNumInit", typeof(decimal));

            Columns.Add("PlantID", typeof(string));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["LotNum"] = Data.CastDbNull.To<string>(reader["LOTN"], String.Empty).Trim();
                    dr["SerialNum"] = Data.CastDbNull.To<string>(reader["Z58SN"], String.Empty).Trim();
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["ITM"], 0);
                    dr["StatusCode"] = Data.CastDbNull.To<string>(reader["Z58DST"], String.Empty).Trim();
                    dr["RackId"] = Data.CastDbNull.To<decimal>(reader["Z58RID"], 0);
                    dr["QtyOnHand"] = Data.CastDbNull.To<decimal>(reader["PQOH"], 0) / 1000;
                    dr["QtyRecd"] = Data.CastDbNull.To<decimal>(reader["RQOH"], 0) / 1000;
                    dr["Side"] = Data.CastDbNull.To<string>(reader["Z58SD"], String.Empty).Trim();
                    dr["Stack"] = Data.CastDbNull.To<decimal>(reader["Z58STK"], 0);
                    dr["Row"] = Data.CastDbNull.To<decimal>(reader["Z58ROW"], 0);
                    dr["Slot"] = Data.CastDbNull.To<decimal>(reader["Z58SL"], 0);
                    dr["Position"] = Data.CastDbNull.To<decimal>(reader["Z58PS"], 0);
                    dr["Rotated"] = Data.CastDbNull.To<string>(reader["Z58BXRO"], String.Empty).Trim();
                    var mfgDateId = Data.CastDbNull.To<decimal>(reader["Z58MDT"], 0);
                    dr["ManufacturedDate"] = E1TimeManager.CastToDateTime(mfgDateId);
                    var tranDateId = Data.CastDbNull.To<decimal>(reader["UPMJ"], 0);
                    var tranTimeId = Data.CastDbNull.To<decimal>(reader["TDAY"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(tranDateId, tranTimeId).AddHours(plant.TimeOffset);
                    dr["TransactionId"] = Data.CastDbNull.To<decimal>(reader["UKID"], 0);
                    dr["UserId"] = Data.CastDbNull.To<string>(reader["USERID"], String.Empty).Trim();
                    dr["ProgramId"] = Data.CastDbNull.To<string>(reader["PID"], String.Empty).Trim();
                    dr["WOrderNum"] = Data.CastDbNull.To<decimal>(reader["DOC1"], 0);
                    dr["WOrderType"] = Data.CastDbNull.To<string>(reader["XDCTO"], String.Empty).Trim();
                    dr["SOrderNum"] = Data.CastDbNull.To<decimal>(reader["DOCO"], 0);
                    dr["SOrderType"] = Data.CastDbNull.To<string>(reader["DCTO"], String.Empty).Trim();
                    dr["SOrderLineNum"] = Data.CastDbNull.To<decimal>(reader["LNID"], 0) / 1000;
                    dr["POrderNum"] = Data.CastDbNull.To<decimal>(reader["DOC2"], 0);
                    dr["POrderType"] = Data.CastDbNull.To<string>(reader["XCTO"], String.Empty).Trim();
                    dr["POrderLineNum"] = Data.CastDbNull.To<decimal>(reader["LNIC"], 0) / 1000;

                    dr["WOrderNumInit"] = Data.CastDbNull.To<decimal>(reader["DOC1INIT"], 0);
                    dr["WOrderTypeInit"] = Data.CastDbNull.To<string>(reader["XDCTOINIT"], String.Empty).Trim();
                    dr["SOrderNumInit"] = Data.CastDbNull.To<decimal>(reader["DOCOINIT"], 0);
                    dr["SOrderTypeInit"] = Data.CastDbNull.To<string>(reader["DCTOINIT"], String.Empty).Trim();
                    dr["SOrderLineNumInit"] = Data.CastDbNull.To<decimal>(reader["LNIDINIT"], 0) / 1000;
                    dr["POrderNumInit"] = Data.CastDbNull.To<decimal>(reader["DOC2INIT"], 0);
                    dr["POrderTypeInit"] = Data.CastDbNull.To<string>(reader["XCTOINIT"], String.Empty).Trim();
                    dr["POrderLineNumInit"] = Data.CastDbNull.To<decimal>(reader["LNICINIT"], 0) / 1000;

                    var operationCd = Data.CastDbNull.To<string>(reader["IOPR"], String.Empty).Trim();
                    dr["OperationCode"] = operationCd;
                    var updateDateIdOrig = Data.CastDbNull.To<decimal>(reader["MUPM"], 0);
                    var updateTimeIdOrig = Data.CastDbNull.To<decimal>(reader["UPMT"], 0);
                    var updateDateId = Data.CastDbNull.To<decimal>(reader["UPMJ"], 0);
                    var updateTimeId = Data.CastDbNull.To<decimal>(reader["TDAY"], 0);
                    if (operationCd == "Init")
                    {
                        dr["CreateDate"] = E1TimeManager.CastToDateTime(updateDateIdOrig, updateTimeIdOrig).AddHours(plant.TimeOffset);
                    }
                    if (operationCd == "Insert")
                    {
                        dr["CreateDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                    }
                    dr["PlantID"] = Data.CastDbNull.To(reader["MCU"], string.Empty).Trim();
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
