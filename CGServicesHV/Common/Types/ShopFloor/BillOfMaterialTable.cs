
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class BillOfMaterialTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public BillOfMaterialTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[BillOfMaterialLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("TypeCode", typeof(string));
            Columns.Add("PlantNum", typeof(string));
            Columns.Add("PartItemNum", typeof(decimal));
            Columns.Add("Description1", typeof(string));
            Columns.Add("LineTypeCode", typeof(string));
            Columns.Add("IssueTypeCode", typeof(string));
            Columns.Add("ComponentNum", typeof(decimal));
            Columns.Add("OperationSeqNum", typeof(decimal));
            Columns.Add("BOMComponentNum", typeof(decimal));
            Columns.Add("SubItemSeqNum", typeof(decimal));
            Columns.Add("ComponentTypeCode", typeof(string));
            Columns.Add("FixedVariableQtyCode", typeof(string));
            Columns.Add("CoproductsCode", typeof(string));
            Columns.Add("StandardQty", typeof(decimal));
            Columns.Add("UnitsBatchQty", typeof(decimal));
            Columns.Add("UOM", typeof(string));
            Columns.Add("InputUOM", typeof(string));
            Columns.Add("PlannedPct", typeof(decimal));
            Columns.Add("CostPct", typeof(decimal));
            Columns.Add("ScrapPct", typeof(decimal));
            Columns.Add("ReworkPct", typeof(decimal));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("EffectiveStartDate", typeof(DateTime));
            Columns.Add("EffectiveEndDate", typeof(DateTime));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ItemNum"] = reader.Get<decimal>("IXKIT");
                    dr["TypeCode"] = reader.Get<string>("IXTBM");
                    dr["PlantNum"] = reader.Get<string>("IXMMCU");
                    dr["PartItemNum"] = reader.Get<decimal>("IXITM");
                    dr["Description1"] = reader.Get<string>("IXDSC1");
                    dr["LineTypeCode"] = reader.Get<string>("IXLNTY");
                    dr["IssueTypeCode"] = reader.Get<string>("IXITC");
                    dr["ComponentNum"] = reader.Get<decimal>("IXCPNT");
                    dr["OperationSeqNum"] = reader.Get<decimal>("IXOPSQ") / 100;
                    dr["BOMComponentNum"] = reader.Get<decimal>("IXCPNB") / 100;
                    dr["SubItemSeqNum"] = reader.Get<decimal>("IXSBNT") / 100;
                    dr["ComponentTypeCode"] = reader.Get<string>("IXCOTY");
                    dr["FixedVariableQtyCode"] = reader.Get<string>("IXFORQ");
                    dr["CoproductsCode"] = reader.Get<string>("IXCOBY");
                    dr["StandardQty"] = reader.Get<decimal>("IXQNTY") / 1000;
                    dr["UnitsBatchQty"] = reader.Get<decimal>("IXBQTY") / 1000;
                    dr["UOM"] = reader.Get<string>("IXUM");
                    dr["InputUOM"] = reader.Get<string>("IXUOM");
                    dr["PlannedPct"] = reader.Get<decimal>("IXFTRP") / 100;
                    dr["CostPct"] = reader.Get<decimal>("IXF$RP") / 100;
                    dr["ScrapPct"] = reader.Get<decimal>("IXSCRP") / 100;
                    dr["ReworkPct"] = reader.Get<decimal>("IXREWP") / 100;
                    dr["UserId"] = reader.Get<string>("IXUSER");
                    dr["ProgramId"] = reader.Get<string>("IXPID");
                    dr["EffectiveStartDate"] = E1TimeManager.CastToDateTime(reader.Get<decimal>("IXEFFF"));
                    dr["EffectiveEndDate"] = E1TimeManager.CastToDateTime(reader.Get<decimal>("IXEFFT"));
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(reader.Get<decimal>("IXUPMJ"), reader.Get<decimal>("IXTDAY")).AddMinutes(plant.BusinessSystemOffset);
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
