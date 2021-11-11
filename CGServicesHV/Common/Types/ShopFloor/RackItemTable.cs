using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackItemTable : DataTable
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

        public RackItemTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackItemLedger]");
            Columns.Add("LotNum", typeof(string));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));

            Columns.Add("RackId", typeof(decimal));
            Columns.Add("RackStatusCode", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("AssetId", typeof(string));

            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("QtyRecd", typeof(decimal));
            Columns.Add("Side", typeof(string));
            Columns.Add("Stack", typeof(decimal));
            Columns.Add("Row", typeof(decimal));
            Columns.Add("Slot", typeof(decimal));
            Columns.Add("Position", typeof(decimal));
            Columns.Add("Rotated", typeof(string));

            Columns.Add("DocumentNum", typeof(decimal));
            Columns.Add("DocumentType", typeof(string));
            Columns.Add("DocumentLineNum", typeof(decimal));
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("BoxNum", typeof(decimal));
            Columns.Add("ConfigId", typeof(decimal));
            Columns.Add("StockCode", typeof(string));
            Columns.Add("GlCatCode", typeof(string));

            Columns.Add("ReceivedDate", typeof(DateTime));
            Columns.Add("ManufacturedDate", typeof(DateTime));
            Columns.Add("TransactionDate", typeof(DateTime));

            Columns.Add("CustomerNum", typeof(decimal));
            Columns.Add("CustomerName", typeof(string));
            Columns.Add("SupplierNum", typeof(decimal));
            Columns.Add("SupplierName", typeof(string));

            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("OperationCode", typeof(string));
            Columns.Add("ProcessCode", typeof(string));
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
                        dr["LotNum"] = CastDbNull.To<string>(reader["LOTN"], String.Empty).Trim();
                        dr["SerialNum"] = CastDbNull.To(reader["Z58SN"], String.Empty).Trim();
                        dr["ItemNum"] = CastDbNull.To<decimal>(reader["ITM"], 0);
                        dr["StatusCode"] = CastDbNull.To<string>(reader["Z58DST"], String.Empty).Trim();

                        dr["RackId"] = CastDbNull.To<decimal>(reader["Z58RID"], 0);
                        dr["RackStatusCode"] = CastDbNull.To<string>(reader["Z58HST"], String.Empty).Trim();
                        dr["RackType"] = CastDbNull.To<string>(reader["Z58RT"], String.Empty).Trim();
                        dr["Location"] = CastDbNull.To<string>(reader["LOCN"], String.Empty).Trim();
                        dr["AssetId"] = CastDbNull.To<string>(reader["URRF"], String.Empty).Trim();

                        dr["QtyOnHand"] = CastDbNull.To<decimal>(reader["PQOH"], 0) / 1000;
                        dr["QtyRecd"] = CastDbNull.To<decimal>(reader["RQOH"], 0) / 1000;
                        dr["Side"] = CastDbNull.To<string>(reader["Z58SD"], String.Empty).Trim();
                        dr["Stack"] = CastDbNull.To<decimal>(reader["Z58STK"], 0);
                        dr["Row"] = CastDbNull.To<decimal>(reader["Z58ROW"], 0);
                        dr["Slot"] = CastDbNull.To<decimal>(reader["Z58SL"], 0);
                        dr["Position"] = CastDbNull.To<decimal>(reader["Z58PS"], 0);
                        dr["Rotated"] = CastDbNull.To<string>(reader["Z58BXRO"], String.Empty).Trim();

                        dr["DocumentNum"] = CastDbNull.To<decimal>(reader["DOCO"], 0);
                        dr["DocumentType"] = CastDbNull.To<string>(reader["DCTO"], String.Empty).Trim();
                        dr["DocumentLineNum"] = CastDbNull.To<decimal>(reader["LNID"], 0) / 1000;
                        dr["ScheduleNum"] = CastDbNull.To<decimal>(reader["Z58SCN"], 0);
                        dr["BoxNum"] = CastDbNull.To<decimal>(reader["Z58BOX"], 0);
                        dr["ConfigId"] = CastDbNull.To<decimal>(reader["CFGID"], 0);
                        dr["StockCode"] = CastDbNull.To<string>(reader["STKT"], String.Empty).Trim();
                        dr["GlCatCode"] = CastDbNull.To<string>(reader["GLPT"], String.Empty).Trim();

                        var processCode = CastDbNull.To<string>(reader["PROC"], String.Empty).Trim();
                        if (processCode == "PURCFG" || processCode == "PURSTK")
                        {
                            dr["SupplierNum"] = CastDbNull.To<decimal>(reader["AN8"], 0);
                            dr["SupplierName"] = CastDbNull.To<string>(reader["ALPH"], String.Empty).Trim();
                        }
                        else
                        {
                            dr["CustomerNum"] = CastDbNull.To<decimal>(reader["AN8"], 0);
                            dr["CustomerName"] = CastDbNull.To<string>(reader["ALPH"], String.Empty).Trim();
                        }

                        var mfgDateId = Data.CastDbNull.To<decimal>(reader["Z58MDT"], 0);
                        dr["ManufacturedDate"] = E1TimeManager.CastToDateTime(mfgDateId).AddHours(plant.TimeOffset);
                        var recdDateId = Data.CastDbNull.To<decimal>(reader["MUPM"], 0);
                        dr["ReceivedDate"] = E1TimeManager.CastToDateTime(recdDateId).AddHours(plant.TimeOffset);
                        var tranDateId = Data.CastDbNull.To<decimal>(reader["UPMJ"], 0);
                        var tranTimeId = Data.CastDbNull.To<decimal>(reader["TDAY"], 0);
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(tranDateId, tranTimeId).AddHours(plant.TimeOffset);

                        dr["TransactionId"] = CastDbNull.To<decimal>(reader["UKID"], 0);
                        dr["OperationCode"] = CastDbNull.To<string>(reader["IOPR"], String.Empty).Trim();
                        dr["ProcessCode"] = CastDbNull.To<string>(reader["PROC"], String.Empty).Trim();

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

        public void Add(List<dynamic> recs, Plant plant)
        {
            try
            {
                foreach (var rec in recs.Where(o => o.Z58DST == "4"))
                {
                    DataRow dr = NewRow();
                    dr["LotNum"] = rec.LOTN;
                    dr["SerialNum"] = rec.Z58SN;
                    dr["ItemNum"] = rec.ITM;
                    dr["StatusCode"] = rec.Z58DST;

                    dr["RackId"] = rec.Z58RID;
                    dr["RackStatusCode"] = rec.Z58HST;
                    dr["RackType"] = rec.Z58RT;
                    dr["Location"] = rec.LOCN;
                    dr["AssetId"] = rec.URRF;

                    dr["QtyOnHand"] = rec.PQOH;
                    dr["QtyRecd"] = rec.PQOH;
                    dr["Side"] = rec.Z58SD;
                    dr["Stack"] = rec.Z58STK;
                    dr["Row"] = rec.Z58ROW;
                    dr["Slot"] = rec.Z58SL;
                    dr["Position"] = rec.Z58PS;
                    dr["Rotated"] = rec.Z58BXRO;

                    dr["DocumentNum"] = rec.DOCO;
                    dr["DocumentType"] = rec.DCTO;
                    dr["DocumentLineNum"] = rec.LNID;
                    dr["ScheduleNum"] = rec.Z58SCN;
                    dr["BoxNum"] = rec.Z58BOX;
                    this.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class RackItemStatusTable : DataTable
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

        public RackItemStatusTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackItemStatusLedger]");
            Columns.Add("LotNum", typeof(string));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("RackStatusCode", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("AssetId", typeof(string));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("Side", typeof(string));
            Columns.Add("Stack", typeof(decimal));
            Columns.Add("Row", typeof(decimal));
            Columns.Add("Slot", typeof(decimal));
            Columns.Add("Position", typeof(decimal));
            Columns.Add("Rotated", typeof(string));
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("BoxNum", typeof(decimal));
            Columns.Add("StockCode", typeof(string));
            Columns.Add("GlCatCode", typeof(string));
            //Columns.Add("Version", typeof(string));
            Columns.Add("ManufacturedDate", typeof(DateTime));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("OperationCode", typeof(string));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["LotNum"] = CastDbNull.To<string>(reader["LOTN"], String.Empty).Trim();
                    dr["SerialNum"] = CastDbNull.To<string>(reader["Z58SN"], String.Empty).Trim();
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["ITM"], 0);
                    dr["StatusCode"] = CastDbNull.To<string>(reader["Z58DST"], String.Empty).Trim();
                    dr["RackId"] = CastDbNull.To<decimal>(reader["Z58RID"], 0);
                    dr["RackStatusCode"] = CastDbNull.To<string>(reader["Z58HST"], String.Empty).Trim();
                    dr["RackType"] = CastDbNull.To<string>(reader["Z58RT"], String.Empty).Trim();
                    dr["Location"] = CastDbNull.To<string>(reader["LOCN"], String.Empty).Trim();
                    dr["AssetId"] = CastDbNull.To<string>(reader["URRF"], String.Empty).Trim();
                    dr["QtyOnHand"] = CastDbNull.To<decimal>(reader["PQOH"], 0) / 1000;
                    dr["Side"] = CastDbNull.To<string>(reader["Z58SD"], String.Empty).Trim();
                    dr["Stack"] = CastDbNull.To<decimal>(reader["Z58STK"], 0);
                    dr["Row"] = CastDbNull.To<decimal>(reader["Z58ROW"], 0);
                    dr["Slot"] = CastDbNull.To<decimal>(reader["Z58SL"], 0);
                    dr["Position"] = CastDbNull.To<decimal>(reader["Z58PS"], 0);
                    dr["Rotated"] = CastDbNull.To<string>(reader["Z58BXRO"], String.Empty).Trim();
                    dr["ScheduleNum"] = CastDbNull.To<decimal>(reader["Z58SCN"], 0);
                    dr["BoxNum"] = CastDbNull.To<decimal>(reader["Z58BOX"], 0);
                    dr["StockCode"] = CastDbNull.To<string>(reader["STKT"], String.Empty).Trim();
                    dr["GlCatCode"] = CastDbNull.To<string>(reader["GLPT"], String.Empty).Trim();
                    //dr["Version"] = CastDbNull.To<string>(reader["VERS"], String.Empty).Trim();
                    var mfgDateId = Data.CastDbNull.To<decimal>(reader["Z58MDT"], 0);
                    dr["ManufacturedDate"] = E1TimeManager.CastToDateTime(mfgDateId).AddHours(plant.TimeOffset);
                    var tranDateId = Data.CastDbNull.To<decimal>(reader["UPMJ"], 0);
                    var tranTimeId = Data.CastDbNull.To<decimal>(reader["TDAY"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(tranDateId, tranTimeId).AddHours(plant.TimeOffset);
                    dr["TransactionId"] = CastDbNull.To<decimal>(reader["UKID"], 0);
                    dr["OperationCode"] = CastDbNull.To<string>(reader["IOPR"], String.Empty).Trim();
                    this.Rows.Add(dr);
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

    [System.ComponentModel.DesignerCategory("Code")]
    public class RackItemTableOld : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("Ukid")).Max(); }
            set { }
        }

        public RackItemTableOld()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackItemLedger]");
            Columns.Add("LotNum", typeof(string));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("Ukid", typeof(decimal));
            Columns.Add("OperationCode", typeof(string));
            Columns.Add("StatusCode", typeof(string));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("QtyRecd", typeof(decimal));
            Columns.Add("ManufacturedDate", typeof(DateTime));
            Columns.Add("Side", typeof(string));
            Columns.Add("Stack", typeof(decimal));
            Columns.Add("Row", typeof(decimal));
            Columns.Add("Slot", typeof(decimal));
            Columns.Add("Position", typeof(decimal));
            Columns.Add("Rotated", typeof(string));
            Columns.Add("DocumentNum", typeof(decimal));
            Columns.Add("DocumentType", typeof(string));
            Columns.Add("DocumentLineNum", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["Ukid"] = CastDbNull.To<decimal>(reader["DLUKID"], 0);
                    dr["RackId"] = CastDbNull.To<decimal>(reader["DLZ58RID"], 0);
                    dr["LotNum"] = CastDbNull.To<string>(reader["DLLOTN"], String.Empty).Trim();
                    dr["SerialNum"] = CastDbNull.To<string>(reader["DLZ58SN"], String.Empty).Trim();
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["DLITM"], 0);
                    dr["StatusCode"] = CastDbNull.To<string>(reader["DLZ58DST"], String.Empty).Trim();
                    dr["QtyOnHand"] = CastDbNull.To<decimal>(reader["DLPQOH"], 0) / 1000;
                    dr["Side"] = CastDbNull.To<string>(reader["DLZ58SD"], String.Empty).Trim();
                    dr["Stack"] = CastDbNull.To<decimal>(reader["DLZ58STK"], 0);
                    dr["Row"] = CastDbNull.To<decimal>(reader["DLZ58ROW"], 0);
                    dr["Slot"] = CastDbNull.To<decimal>(reader["DLZ58SL"], 0);
                    dr["Position"] = CastDbNull.To<decimal>(reader["DLZ58PS"], 0);
                    dr["Rotated"] = CastDbNull.To<string>(reader["DLZ58BXRO"], String.Empty).Trim();
                    var updateDateId = Data.CastDbNull.To<decimal>(reader["DLMUPM"], 0);
                    var updateTimeId = Data.CastDbNull.To<decimal>(reader["DLUPMT"], 0);
                    var operationCd = CastDbNull.To<string>(reader["DLIOPR"], String.Empty).Trim();
                    if (operationCd == "Insert")
                    {
                        dr["ReceivedQty"] = CastDbNull.To<decimal>(reader["DLPQOH"], 0) / 1000;
                        var dldoc1 = CastDbNull.To<decimal>(reader["DLDOC1"], 0);
                        var dldoc2 = CastDbNull.To<decimal>(reader["DLDOC2"], 0);
                        if (dldoc2 > 0)
                        {
                            dr["DocumentNum"] = CastDbNull.To<decimal>(reader["DLDOC2"], 0);
                            dr["DocumentType"] = CastDbNull.To<string>(reader["DLXCTO"], String.Empty).Trim();
                            dr["DocumentLineNum"] = CastDbNull.To<decimal>(reader["DLLNIC"], 0) / 1000;
                        }
                        else
                        {
                            dr["DocumentNum"] = CastDbNull.To<decimal>(reader["DLDOC1"], 0);
                            dr["DocumentType"] = CastDbNull.To<string>(reader["DLXDCTO"], String.Empty).Trim();
                            dr["DocumentLineNum"] = 0;
                        }
                    }
                    var mfgDateId = Data.CastDbNull.To<decimal>(reader["DLZ58MDT"], 0);
                    dr["ManufacturedDate"] = E1TimeManager.CastToDateTime(mfgDateId).AddHours(plant.TimeOffset);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                    dr["OperationCode"] = operationCd;
                    this.Rows.Add(dr);
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
