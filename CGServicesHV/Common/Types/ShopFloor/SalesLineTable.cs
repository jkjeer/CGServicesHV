using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SalesLineTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public SalesLineTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SalesLineLedger]");
            Columns.Add("OrderNum", typeof(decimal));
            Columns.Add("OrderType", typeof(string));
            Columns.Add("OrderLineNum", typeof(decimal));

            Columns.Add("LineType", typeof(string));
            Columns.Add("VolumeType", typeof(string));
            Columns.Add("Quantity", typeof(decimal));
            Columns.Add("LastStatus", typeof(string));
            Columns.Add("NextStatus", typeof(string));

            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("ROrderNum", typeof(decimal));
            Columns.Add("ROrderType", typeof(string));
            Columns.Add("ROrderLineNum", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("ConfigId", typeof(decimal));
            Columns.Add("GLCategory", typeof(string));
            Columns.Add("StockType", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("Description2", typeof(string));

            Columns.Add("OrderDate", typeof(DateTime));
            Columns.Add("RequestedDate", typeof(DateTime));
            Columns.Add("OrigRequestedDate", typeof(DateTime));
            Columns.Add("CancelDate", typeof(DateTime));
            Columns.Add("ShipDate", typeof(DateTime));
            Columns.Add("InvoiceDate", typeof(DateTime));
            Columns.Add("DeliveryDateTime", typeof(DateTime));

            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("CustomerShipNum", typeof(decimal));
            Columns.Add("CustomerOrderNum", typeof(string));
            Columns.Add("CustomerPart1", typeof(string));
            Columns.Add("CustomerPart2", typeof(string));
            Columns.Add("CustomerPart3", typeof(string));
            Columns.Add("CustomerSeqNum", typeof(string));
            Columns.Add("GenericSeqNum", typeof(string));
            Columns.Add("ProductType", typeof(string));
            Columns.Add("ReasonCode", typeof(string));
            Columns.Add("Priority", typeof(string));

            Columns.Add("UnitCostAmt", typeof(decimal));
            Columns.Add("UnitPriceAmt", typeof(decimal));
            Columns.Add("UnitListPriceAmt", typeof(decimal));
            Columns.Add("ExtendedPriceAmt", typeof(decimal));
            Columns.Add("PriceOverride", typeof(string));
            Columns.Add("ZeroPriceOverride", typeof(string));
            Columns.Add("UnitActualSqft", typeof(decimal));
            Columns.Add("UnitBillableSqft", typeof(decimal));

            Columns.Add("InvoiceNum", typeof(decimal));
            Columns.Add("LoadNum", typeof(decimal));

            Columns.Add("ScheduleBatchName", typeof(string));
            Columns.Add("ScheduleGroupName", typeof(string));

            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Enterprise.Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["OrderNum"] = Data.CastDbNull.To<decimal>(reader["SDDOCO"], 0);
                    dr["OrderType"] = Data.CastDbNull.To<string>(reader["SDDCTO"], String.Empty).Trim();
                    dr["OrderLineNum"] = Data.CastDbNull.To<decimal>(reader["SDLNID"], 0) / 1000;

                    dr["LineType"] = Data.CastDbNull.To<string>(reader["SDLNTY"], String.Empty).Trim();
                    dr["VolumeType"] = Data.CastDbNull.To<string>(reader["ATZ5840460"], String.Empty).Trim();
                    dr["Quantity"] = Data.CastDbNull.To<decimal>(reader["SDUORG"], 0) / 1000;
                    dr["LastStatus"] = Data.CastDbNull.To<string>(reader["SDLTTR"], String.Empty).Trim();
                    dr["NextStatus"] = Data.CastDbNull.To<string>(reader["SDNXTR"], String.Empty).Trim();

                    if (Data.CastDbNull.To<string>(reader["SDRCTO"], String.Empty).Trim() == "WO")
                    {
                        dr["WOrderNum"] = Data.CastStringToDecimal(Data.CastDbNull.To<string>(reader["SDRORN"], String.Empty).Trim());
                    }
                    else
                    {
                        if (Data.CastDbNull.To<string>(reader["SDDCTO"], String.Empty).Trim() == "ST")
                        {
                            dr["WOrderNum"] = Data.CastDbNull.To<decimal>(reader["ATZ58WODC"], 0);
                        }
                        else
                        {
                            dr["WOrderNum"] = (decimal)0;
                        }
                    }
                    dr["ROrderNum"] = Data.CastStringToDecimal(Data.CastDbNull.To<string>(reader["SDRORN"], String.Empty).Trim());
                    dr["ROrderType"] = Data.CastDbNull.To<string>(reader["SDRCTO"], String.Empty).Trim();
                    dr["ROrderLineNum"] = Data.CastDbNull.To<decimal>(reader["SDRLLN"], 0) / 1000;

                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["SDITM"], 0);
                    dr["ConfigId"] = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                    dr["GLCategory"] = Data.CastDbNull.To<string>(reader["SDGLC"], String.Empty).Trim();
                    dr["Description1"] = Data.CastDbNull.To<string>(reader["SDDSC1"], String.Empty).Trim();
                    dr["Description2"] = Data.CastDbNull.To<string>(reader["SDDSC2"], String.Empty).Trim();

                    dr["OrderDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDTRDJ"], 0));
                    dr["RequestedDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDDRQJ"], 0));
                    dr["OrigRequestedDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDOPDJ"], 0));
                    dr["CancelDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDCNDJ"], 0));
                    dr["ShipDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDADDJ"], 0));
                    dr["InvoiceDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDIVD"], 0));
                    dr["DeliveryDateTime"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDPDDJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["SDDRQT"], 0));

                    dr["CustomerSoldNum"] = Data.CastDbNull.To<decimal>(reader["SDAN8"], 0);
                    dr["CUstomerShipNum"] = Data.CastDbNull.To<decimal>(reader["SDSHAN"], 0);
                    dr["CustomerOrderNum"] = Data.CastDbNull.To<string>(reader["SDVR01"], String.Empty).Trim();
                    dr["CustomerPart1"] = Data.CastDbNull.To<string>(reader["ATZ58CP2"], String.Empty).Trim();
                    dr["CustomerPart2"] = Data.CastDbNull.To<string>(reader["ATZ58CP3"], String.Empty).Trim();
                    dr["CustomerPart3"] = Data.CastDbNull.To<string>(reader["ATZ58CP4"], String.Empty).Trim();
                    dr["CustomerSeqNum"] = Data.CastDbNull.To<string>(reader["ATZ58CSQNO"], String.Empty).Trim();
                    dr["GenericSeqNum"] = Data.CastDbNull.To<string>(reader["ATZ58GSQNO"], String.Empty).Trim();
                    dr["ProductType"] = Data.CastDbNull.To<string>(reader["ATZ58PT"], String.Empty).Trim();
                    dr["ReasonCode"] = Data.CastDbNull.To<string>(reader["SDRCD"], String.Empty).Trim();
                    dr["Priority"] = Data.CastDbNull.To<string>(reader["SDURCD"], String.Empty).Trim();

                    dr["UnitCostAmt"] = Data.CastDbNull.To<decimal>(reader["SDUNCS"], 0) / 10000;
                    dr["UnitPriceAmt"] = Data.CastDbNull.To<decimal>(reader["SDUPRC"], 0) / 10000;
                    dr["UnitListPriceAmt"] = Data.CastDbNull.To<decimal>(reader["SDLPRC"], 0) / 10000;
                    dr["ExtendedPriceAmt"] = Data.CastDbNull.To<decimal>(reader["SDAEXP"], 0) / 100;
                    dr["PriceOverride"] = Data.CastDbNull.To<string>(reader["SDPROV"], String.Empty).Trim();
                    dr["ZeroPriceOverride"] = Data.CastDbNull.To<string>(reader["ATZ58ZPO"], String.Empty).Trim();

                    dr["UnitActualSqft"] = Data.CastDbNull.To<decimal>(reader["ATZ58USF"], 0) / 10000;
                    dr["UnitBillableSqft"] = Data.CastDbNull.To<decimal>(reader["ATZ58BSF"], 0) / 10000;

                    dr["LoadNum"] = Data.CastDbNull.To<decimal>(reader["SDPSN"], 0);
                    dr["InvoiceNum"] = Data.CastDbNull.To<decimal>(reader["SDDOC"], 0);

                    dr["ScheduleBatchName"] = Data.CastDbNull.To<string>(reader["BHZ58BDES"], String.Empty).Trim();
                    dr["ScheduleGroupName"] = Data.CastDbNull.To<string>(reader["BHZ58GDES"], String.Empty).Trim();

                    dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["SDTDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
                    this.Rows.Add(dr);
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
