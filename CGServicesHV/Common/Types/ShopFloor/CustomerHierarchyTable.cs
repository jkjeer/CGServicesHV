using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.Enterprise;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class CustomerHierarchyTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public CustomerHierarchyTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[CustomerHierarchyLedger]");
            Columns.Add("ChildNum", typeof(decimal));
            Columns.Add("ParentNum", typeof(decimal));
            Columns.Add("LevelNum", typeof(int));
            Columns.Add("ChildName", typeof(string));
            Columns.Add("ChildType", typeof(string));
            Columns.Add("ParentName", typeof(string));
            Columns.Add("ParentType", typeof(string));
            Columns.Add("SortKey", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["ChildNum"] = reader.Get<decimal>("CHILD_NO");
                    dr["ParentNum"] = reader.Get<decimal>("PARENT_NO");
                    dr["LevelNum"] = reader.Get<int>("LEVEL_NO");
                    dr["ChildName"] = reader.Get<string>("CHILD_NAME");
                    dr["ChildType"] = reader.Get<string>("CHILD_TYPE_CD");
                    dr["ParentName"] = reader.Get<string>("PARENT_NAME");
                    dr["ParentType"] = reader.Get<string>("PARENT_TYPE_CD");
                    dr["SortKey"] = reader.Get<string>("SORT_KEY");
                    var updateDateId = Data.CastDbNull.To<decimal>(reader["E1_DATE_ID"], 0);
                    var updateTimeId = Data.CastDbNull.To<decimal>(reader["E1_TIME_ID"], 0);
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddMinutes(plant.BusinessSystemOffset);
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
