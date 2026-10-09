using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SalesAttributeTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public SalesAttributeTable()
        {
            ExtendedProperties.Add("Target", "[ORD].[SALES_ATTRIBUTE_LEDGER]");
            Columns.Add("ORDER_NUM", typeof(decimal));
            Columns.Add("ORDER_TYPE_CD", typeof(string));
            Columns.Add("ORDER_LINE_NUM", typeof(decimal));
            Columns.Add("PRODUCT_TYPE_CD", typeof(string));
            Columns.Add("VOLUME_CD", typeof(string));
            Columns.Add("CUSTOMER_PO", typeof(string));
            Columns.Add("CUSTOMER_ORIG_PO", typeof(string));
            Columns.Add("CUSTOMER_PART1", typeof(string));
            Columns.Add("CUSTOMER_PART2", typeof(string));
            Columns.Add("CUSTOMER_PART3", typeof(string));
            Columns.Add("CUSTOMER_ORIG_PART", typeof(string));
            Columns.Add("CUSTOMER_SEQ_NUM", typeof(string));
            Columns.Add("GENERIC_SEQ_NUM", typeof(string));
            Columns.Add("MESSAGE1", typeof(string));
            Columns.Add("MESSAGE2", typeof(string));
            Columns.Add("PICK_NOTES", typeof(string));
            Columns.Add("PACK_NOTES", typeof(string));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                using (reader)
                {

                    while (reader.Read())
                    {
                        DataRow dr = this.NewRow();
                        dr["ORDER_NUM"] = Data.CastDbNull.To<decimal>(reader["ATDOCO"], 0);
                        dr["ORDER_TYPE_CD"] = Data.CastDbNull.To<string>(reader["ATDCTO"], String.Empty).Trim();
                        dr["ORDER_LINE_NUM"] = Data.CastDbNull.To<decimal>(reader["ATLNID"], 0) / 1000;
                        dr["PRODUCT_TYPE_CD"] = Data.CastDbNull.To<string>(reader["ATZ58PT"], String.Empty).Trim();
                        dr["CUSTOMER_PART1"] = Data.CastDbNull.To<string>(reader["ATZ58CP2"], String.Empty).Trim();
                        dr["CUSTOMER_PART2"] = Data.CastDbNull.To<string>(reader["ATZ58CP3"], String.Empty).Trim();
                        dr["CUSTOMER_PART3"] = Data.CastDbNull.To<string>(reader["ATZ58CP4"], String.Empty).Trim();
                        dr["CUSTOMER_SEQ_NUM"] = Data.CastDbNull.To<string>(reader["ATZ58CSQNO"], String.Empty).Trim();
                        dr["GENERIC_SEQ_NUM"] = Data.CastDbNull.To<string>(reader["ATZ58GSQNO"], String.Empty).Trim();
                        dr["VOLUME_CD"] = Data.CastDbNull.To<string>(reader["ATZ5840460"], String.Empty).Trim();
                        dr["MESSAGE1"] = Data.CastDbNull.To<string>(reader["ATZ5899009"], String.Empty).Trim();
                        dr["MESSAGE2"] = Data.CastDbNull.To<string>(reader["ATZ5899010"], String.Empty).Trim();
                        dr["PICK_NOTES"] = Data.CastDbNull.To<string>(reader["ATZ5899011"], String.Empty).Trim();
                        dr["PACK_NOTES"] = Data.CastDbNull.To<string>(reader["ATZ5899012"], String.Empty).Trim();
                        dr["CUSTOMER_ORIG_PART"] = Data.CastDbNull.To<string>(reader["ATZ5899076"], String.Empty).Trim();
                        dr["CUSTOMER_PO"] = "";
                        dr["CUSTOMER_ORIG_PO"] = Data.CastDbNull.To<string>(reader["ATZ5899074"], String.Empty).Trim();
                        dr["UPDATE_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ATUPMJ"], 0),
                            Data.CastDbNull.To<decimal>(reader["ATTDAY"], 0));
                        this.Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
