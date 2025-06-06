using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ItemBalanceTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemBalanceTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemBalanceLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("E1LocationCode", typeof(string));
            Columns.Add("E1LotNum", typeof(string));
            Columns.Add("LocationCode", typeof(string));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("PrimaryBinCode", typeof(string));
            Columns.Add("LotStatusCode", typeof(string));
            Columns.Add("GLCategoryCode", typeof(string));
            Columns.Add("LastReceiptDate", typeof(DateTime));
            Columns.Add("Quantity", typeof(decimal));
            Columns.Add("QuantityOnOrder", typeof(decimal));
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
                    dr["ItemNum"] = reader.Get<decimal>("LIITM");
                    dr["E1LocationCode"] = reader.Get<string>("LILOCN", string.Empty, false);
                    dr["E1LotNum"] = reader.Get<string>("LILOTN", string.Empty, false);
                    dr["LocationCode"] = reader.Get<string>("LILOCN");
                    dr["LotNum"] = reader.Get<string>("LILOTN");
                    dr["PrimaryBinCode"] = reader.Get<string>("LIPBIN");
                    dr["LotStatusCode"] = reader.Get<string>("LILOTS");
                    dr["GLCategoryCode"] = reader.Get<string>("LIGLPT");
                    dr["LastReceiptDate"] = E1TimeManager.CastToNullableDate(reader.Get<decimal>("LILRCJ")) ?? (object)DBNull.Value;
                    dr["Quantity"] = reader.Get<decimal>("LIPQOH") / 1000;
                    dr["QuantityOnOrder"] = reader.Get<decimal>("LIPREQ") / 1000;
                    dr["UserId"] = reader.Get<string>("LIUSER");
                    dr["ProgramId"] = reader.Get<string>("LIPID");
                    var liDate = E1TimeManager.CastToDateTime(reader.Get<decimal>("LIUPMJ"), reader.Get<decimal>("LITDAY"));
                    dr["TransactionDate"] = new DateTime[1] { liDate }.Max().AddMinutes(plant.BusinessSystemOffset);
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
