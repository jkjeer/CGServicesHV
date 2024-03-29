using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.Enterprise;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ItemAttributeTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemAttributeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemAttributeLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("AttributeName", typeof(string));
            Columns.Add("AttributeValue", typeof(string));
            Columns.Add("Numeric", typeof(string));
            Columns.Add("Udc1", typeof(string));
            Columns.Add("Udc2", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(ItemAttributes attributes, Plant plant)
        {
            try
            {
                foreach (var attribute in attributes)
                {
                    var dr = NewRow();
                    dr["ItemNum"] = attribute.ItemNum;
                    dr["AttributeName"] = attribute.Name;
                    dr["AttributeValue"] = attribute.Value;
                    dr["Numeric"] = attribute.Numeric;
                    dr["Udc1"] = attribute.Udc1;
                    dr["Udc2"] = attribute.Udc2;
                    dr["TransactionDate"] = attribute.TransactionDate.AddMinutes(plant.BusinessSystemOffset);
                    Rows.Add(dr);
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
