// Decompiled with JetBrains decompiler
// Type: CGServicesHV.Common.Types.ShopFloor.SalesLineTable
// Assembly: CGServicesHV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85F30AF5-AC07-435F-A3C1-52D64835746D
// Assembly location: \\CGD1intsvcpd\c$\inetpub\wwwroot\CGServicesHVE1\bin\CGServicesHV.dll

using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [DesignerCategory("Code")]
    public class SalesLineTable : DataTable
    {
        public DateTime MaxDate
        {
            get
            {
                return this.AsEnumerable().Select<DataRow, DateTime>((Func<DataRow, DateTime>)(a => a.Field<DateTime>("TransactionDate"))).Max<DateTime>();
            }
            set
            {
            }
        }

        public SalesLineTable()
        {
            this.ExtendedProperties.Add((object)"Target", (object)"[EBS].[SalesLineLedger]");
            this.Columns.Add("OrderNum", typeof(Decimal));
            this.Columns.Add("OrderType", typeof(string));
            this.Columns.Add("OrderLineNum", typeof(Decimal));
            this.Columns.Add("LineType", typeof(string));
            this.Columns.Add("VolumeType", typeof(string));
            this.Columns.Add("Quantity", typeof(Decimal));
            this.Columns.Add("LastStatus", typeof(string));
            this.Columns.Add("NextStatus", typeof(string));
            this.Columns.Add("WOrderNum", typeof(Decimal));
            this.Columns.Add("ROrderNum", typeof(Decimal));
            this.Columns.Add("ROrderType", typeof(string));
            this.Columns.Add("ROrderLineNum", typeof(Decimal));
            this.Columns.Add("ItemNum", typeof(Decimal));
            this.Columns.Add("GLCategory", typeof(string));
            this.Columns.Add("StockType", typeof(string));
            this.Columns.Add("Description1", typeof(string));
            this.Columns.Add("Description2", typeof(string));
            this.Columns.Add("OrderDate", typeof(DateTime));
            this.Columns.Add("RequestedDate", typeof(DateTime));
            this.Columns.Add("OrigRequestedDate", typeof(DateTime));
            this.Columns.Add("CancelDate", typeof(DateTime));
            this.Columns.Add("ShipDate", typeof(DateTime));
            this.Columns.Add("InvoiceDate", typeof(DateTime));
            this.Columns.Add("CustomerSoldNum", typeof(Decimal));
            this.Columns.Add("CustomerShipNum", typeof(Decimal));
            this.Columns.Add("CustomerOrderNum", typeof(string));
            this.Columns.Add("CustomerPart1", typeof(string));
            this.Columns.Add("CustomerPart2", typeof(string));
            this.Columns.Add("CustomerPart3", typeof(string));
            this.Columns.Add("CustomerSeqNum", typeof(string));
            this.Columns.Add("GenericSeqNum", typeof(string));
            this.Columns.Add("ProductType", typeof(string));
            this.Columns.Add("CardinalProductType", typeof(string));
            this.Columns.Add("ReasonCode", typeof(string));
            this.Columns.Add("Priority", typeof(string));
            this.Columns.Add("UnitCostAmt", typeof(Decimal));
            this.Columns.Add("UnitPriceAmt", typeof(Decimal));
            this.Columns.Add("ExtendedPriceAmt", typeof(Decimal));
            this.Columns.Add("PriceOverride", typeof(string));
            this.Columns.Add("ZeroPriceOverride", typeof(string));
            this.Columns.Add("UnitActualSqft", typeof(Decimal));
            this.Columns.Add("UnitBillableSqft", typeof(Decimal));
            this.Columns.Add("InvoiceNum", typeof(Decimal));
            this.Columns.Add("LoadNum", typeof(Decimal));
            this.Columns.Add("TransactionDate", typeof(DateTime));
            this.Columns.Add("UserId", typeof(string));
            this.Columns.Add("ProgramId", typeof(string));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow row = this.NewRow();
                    row["OrderNum"] = (object)CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDDOCO"], Decimal.Zero);
                    row["OrderType"] = (object)CastDbNull.To<string>(reader["SDDCTO"], string.Empty).Trim();
                    row["OrderLineNum"] = (object)(CastDbNull.To<Decimal>(reader["SDLNID"], Decimal.Zero) / new Decimal(1000));
                    row["LineType"] = (object)CastDbNull.To<string>(reader["SDLNTY"], string.Empty).Trim();
                    row["VolumeType"] = (object)CastDbNull.To<string>(reader["ATZ5840460"], string.Empty).Trim();
                    row["Quantity"] = (object)(CastDbNull.To<Decimal>(reader["SDUORG"], Decimal.Zero) / new Decimal(1000));
                    row["LastStatus"] = (object)CastDbNull.To<string>(reader["SDLTTR"], string.Empty).Trim();
                    row["NextStatus"] = (object)CastDbNull.To<string>(reader["SDNXTR"], string.Empty).Trim();
                    string str = CastDbNull.To<string>(reader["SDRCTO"], string.Empty).Trim();
                    row["WOrderNum"] = (object)(str == "WO" ? CGServicesHV.Common.Utilities.Data.CastStringToDecimal(CastDbNull.To<string>(reader["SDRORN"], string.Empty).Trim()) : Decimal.Zero);
                    row["ROrderNum"] = (object)CGServicesHV.Common.Utilities.Data.CastStringToDecimal(CastDbNull.To<string>(reader["SDRORN"], string.Empty).Trim());
                    row["ROrderType"] = (object)CastDbNull.To<string>(reader["SDRCTO"], string.Empty).Trim();
                    row["ROrderLineNum"] = (object)(CastDbNull.To<Decimal>(reader["SDRLLN"], Decimal.Zero) / new Decimal(1000));
                    row["ItemNum"] = (object)CastDbNull.To<Decimal>(reader["SDITM"], Decimal.Zero);
                    row["GLCategory"] = (object)CastDbNull.To<string>(reader["SDGLC"], string.Empty).Trim();
                    row["StockType"] = (object)CastDbNull.To<string>(reader["IBSTKT"], string.Empty).Trim();
                    row["Description1"] = (object)CastDbNull.To<string>(reader["SDDSC1"], string.Empty).Trim();
                    row["Description2"] = (object)CastDbNull.To<string>(reader["SDDSC2"], string.Empty).Trim();
                    row["OrderDate"] = (object)Timestamp.CastToDateTime(CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDTRDJ"], Decimal.Zero), Decimal.Zero);
                    row["RequestedDate"] = (object)Timestamp.CastToDateTime(CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDDRQJ"], Decimal.Zero), Decimal.Zero);
                    row["OrigRequestedDate"] = (object)Timestamp.CastToDateTime(CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDOPDJ"], Decimal.Zero), Decimal.Zero);
                    row["CancelDate"] = (object)Timestamp.CastToDateTime(CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDCNDJ"], Decimal.Zero), Decimal.Zero);
                    row["ShipDate"] = (object)Timestamp.CastToDateTime(CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDADDJ"], Decimal.Zero), Decimal.Zero);
                    row["InvoiceDate"] = (object)Timestamp.CastToDateTime(CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDIVD"], Decimal.Zero), Decimal.Zero);
                    row["CustomerSoldNum"] = (object)CastDbNull.To<Decimal>(reader["SDAN8"], Decimal.Zero);
                    row["CUstomerShipNum"] = (object)CastDbNull.To<Decimal>(reader["SDSHAN"], Decimal.Zero);
                    row["CustomerOrderNum"] = (object)CastDbNull.To<string>(reader["SDVR01"], string.Empty).Trim();
                    row["CustomerPart1"] = (object)CastDbNull.To<string>(reader["ATZ58CP2"], string.Empty).Trim();
                    row["CustomerPart2"] = (object)CastDbNull.To<string>(reader["ATZ58CP3"], string.Empty).Trim();
                    row["CustomerPart3"] = (object)CastDbNull.To<string>(reader["ATZ58CP4"], string.Empty).Trim();
                    row["CustomerSeqNum"] = (object)CastDbNull.To<string>(reader["ATZ58CSQNO"], string.Empty).Trim();
                    row["GenericSeqNum"] = (object)CastDbNull.To<string>(reader["ATZ58GSQNO"], string.Empty).Trim();
                    row["ProductType"] = (object)CastDbNull.To<string>(reader["ATZ58PT"], string.Empty).Trim();
                    row["ReasonCode"] = (object)CastDbNull.To<string>(reader["SDRCD"], string.Empty).Trim();
                    row["Priority"] = (object)CastDbNull.To<string>(reader["SDURCD"], string.Empty).Trim();
                    row["UnitCostAmt"] = (object)(CastDbNull.To<Decimal>(reader["SDUNCS"], Decimal.Zero) / new Decimal(10000));
                    row["UnitPriceAmt"] = 0; //(object)(CastDbNull.To<Decimal>(reader["SDUPRC"], Decimal.Zero) / new Decimal(10000)); // Do NOT push any pricing info on the Sales Line … only Cost.
                    row["ExtendedPriceAmt"] = 0; //(object)(CastDbNull.To<Decimal>(reader["SDAEXP"], Decimal.Zero) / new Decimal(100));
                    row["PriceOverride"] = ""; //(object)CastDbNull.To<string>(reader["SDPROV"], string.Empty).Trim();
                    row["ZeroPriceOverride"] = ""; //(object)CastDbNull.To<string>(reader["ATZ58ZPO"], string.Empty).Trim();
                    row["UnitActualSqft"] = (object)(CastDbNull.To<Decimal>(reader["ATZ58USF"], Decimal.Zero) / new Decimal(10000));
                    row["UnitBillableSqft"] = (object)(CastDbNull.To<Decimal>(reader["ATZ58BSF"], Decimal.Zero) / new Decimal(10000));
                    row["LoadNum"] = (object)CastDbNull.To<Decimal>(reader["SDPSN"], Decimal.Zero);
                    row["InvoiceNum"] = (object)CastDbNull.To<Decimal>(reader["SDDOC"], Decimal.Zero);
                    row["TransactionDate"] = (object)Timestamp.CastToDateTime(CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDUPMJ"], Decimal.Zero), CGServicesHV.Common.Utilities.Data.CastDbNull.To<Decimal>(reader["SDTDAY"], Decimal.Zero)).AddHours((double)plant.TimeOffset);
                    row["UserId"] = (object)CastDbNull.To<string>(reader["SDUSER"], string.Empty).Trim();
                    row["ProgramId"] = (object)CastDbNull.To<string>(reader["SDPID"], string.Empty).Trim();
                    this.Rows.Add(row);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
