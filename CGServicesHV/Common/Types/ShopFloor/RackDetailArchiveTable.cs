using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.Enterprise;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackDetailArchiveTable : DataTable
    {

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

        public RackDetailArchiveTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackDetailArchiveLedger]");
            Columns.Add("TransactionId", typeof(decimal));
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
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["TransactionId"] = Data.CastDbNull.To<decimal>(reader["DLUKID"], 0);
                    dr["LotNum"] = Data.CastDbNull.To<string>(reader["DLLOTN"], String.Empty).Trim();
                    dr["SerialNum"] = Data.CastDbNull.To<string>(reader["DLZ58SN"], String.Empty).Trim();
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["DLITM"], 0);
                    dr["StatusCode"] = Data.CastDbNull.To<string>(reader["DLZ58DST"], String.Empty).Trim();
                    dr["RackId"] = Data.CastDbNull.To<decimal>(reader["DLZ58RID"], 0);
                    dr["QtyOnHand"] = Data.CastDbNull.To<decimal>(reader["DLPQOH"], 0) / 1000;
                    dr["QtyRecd"] = Data.CastDbNull.To<decimal>(reader["DLZ58CQTY"], 0) / 1000;
                    dr["Side"] = Data.CastDbNull.To<string>(reader["DLZ58SD"], String.Empty).Trim();
                    dr["Stack"] = Data.CastDbNull.To<decimal>(reader["DLZ58STK"], 0);
                    dr["Row"] = Data.CastDbNull.To<decimal>(reader["DLZ58ROW"], 0);
                    dr["Slot"] = Data.CastDbNull.To<decimal>(reader["DLZ58SL"], 0);
                    dr["Position"] = Data.CastDbNull.To<decimal>(reader["DLZ58PS"], 0);
                    dr["Rotated"] = Data.CastDbNull.To<string>(reader["DLZ58BXRO"], String.Empty).Trim();
                    dr["ManufacturedDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["DLZ58MDT"], 0));
                    dr["WOrderNum"] = Data.CastDbNull.To<decimal>(reader["DLDOC1"], 0);
                    dr["WOrderType"] = Data.CastDbNull.To<string>(reader["DLXDCTO"], String.Empty).Trim();
                    dr["SOrderNum"] = Data.CastDbNull.To<decimal>(reader["DLDOCO"], 0);
                    dr["SOrderType"] = Data.CastDbNull.To<string>(reader["DLDCTO"], String.Empty).Trim();
                    dr["SOrderLineNum"] = Data.CastDbNull.To<decimal>(reader["DLLNID"], 0) / 1000;
                    dr["POrderNum"] = Data.CastDbNull.To<decimal>(reader["DLDOC2"], 0);
                    dr["POrderType"] = Data.CastDbNull.To<string>(reader["DLXCTO"], String.Empty).Trim();
                    dr["POrderLineNum"] = Data.CastDbNull.To<decimal>(reader["DLLNIC"], 0) / 1000;
                    dr["OperationCode"] = Data.CastDbNull.To<string>(reader["DLIOPR"], String.Empty).Trim();
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["DLMUPM"], 0), Data.CastDbNull.To<decimal>(reader["DLUPMT"], 0)).AddMinutes(plant.BusinessSystemOffset);
                    dr["UserId"] = Data.CastDbNull.To<string>(reader["DLUSER"], String.Empty).Trim();
                    dr["ProgramId"] = Data.CastDbNull.To<string>(reader["DLPID"], String.Empty).Trim();
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
