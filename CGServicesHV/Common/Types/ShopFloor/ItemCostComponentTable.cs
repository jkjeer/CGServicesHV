using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ItemCostComponentTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemCostComponentTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemCostComponentLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("Location", typeof(string));
            Columns.Add("Lot", typeof(string));
            Columns.Add("LedgerTypeCode", typeof(string));
            Columns.Add("CostType", typeof(string));
            Columns.Add("LotGrade", typeof(string));
            Columns.Add("MfgCostStdAmt", typeof(decimal));
            Columns.Add("MfgCostSimStdAmt", typeof(decimal));
            Columns.Add("RollupStdAmt", typeof(decimal));
            Columns.Add("RollupSimStdAmt", typeof(decimal));
            Columns.Add("StdFactorCodeFrozen", typeof(string));
            Columns.Add("StdFactorCodeSim", typeof(string));
            Columns.Add("StdFactorFrozen", typeof(decimal));
            Columns.Add("StdFactorSim", typeof(decimal));
            Columns.Add("StdRateCodeFrozen", typeof(string));
            Columns.Add("StdRateCodeSim", typeof(string));
            Columns.Add("StdRateFrozen", typeof(decimal));
            Columns.Add("StdRateSim", typeof(decimal));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }


        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ItemNum"] = reader.Get<decimal>("IEITM");
                    dr["Location"] = reader.Get<string>("IELOCN");
                    dr["Lot"] = reader.Get<string>("IELOTN");
                    dr["LedgerTypeCode"] = reader.Get<string>("IELEDG");
                    dr["CostType"] = reader.Get<string>("IECOST");
                    dr["LotGrade"] = reader.Get<string>("IELOTG");
                    dr["MfgCostStdAmt"] = reader.Get<decimal>("IESTDC") / 10000;
                    dr["MfgCostSimStdAmt"] = reader.Get<decimal>("IEXSMC") / 10000;
                    dr["RollupStdAmt"] = reader.Get<decimal>("IECSL") / 10000;
                    dr["RollupSimStdAmt"] = reader.Get<decimal>("IEXSCR") / 10000;
                    dr["StdFactorCodeFrozen"] = reader.Get<string>("IESCTC");
                    dr["StdFactorCodeSim"] = reader.Get<string>("IEXSFC");
                    dr["StdFactorFrozen"] = reader.Get<decimal>("IESTFC") / 10000;
                    dr["StdFactorSim"] = reader.Get<decimal>("IEXSF") / 10000;
                    dr["StdRateCodeFrozen"] = reader.Get<string>("IERATS");
                    dr["StdRateCodeSim"] = reader.Get<string>("IEXSRC");
                    dr["StdRateFrozen"] = reader.Get<decimal>("IERTSD") / 10000;
                    dr["StdRateSim"] = reader.Get<decimal>("IEXSR") / 10000;
                    dr["UserId"] = reader.Get<string>("IEUSER");
                    dr["ProgramId"] = reader.Get<string>("IEPID");
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(reader.Get<decimal>("IEUPMJ"), reader.Get<decimal>("IETDAY")).AddMinutes(plant.BusinessSystemOffset);
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

