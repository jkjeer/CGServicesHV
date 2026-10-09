using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RawTagsTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public RawTagsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RawTagsLedger]");
            Columns.Add("ItemNum", typeof(string));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("Writtendate", typeof(DateTime));
            Columns.Add("VenderID", typeof(string));
            Columns.Add("BaseL", typeof(decimal));
            Columns.Add("BaseN", typeof(decimal));
            Columns.Add("BaseD", typeof(decimal));
            Columns.Add("LeftL", typeof(decimal));
            Columns.Add("LeftN", typeof(decimal));
            Columns.Add("LeftD", typeof(decimal));
            Columns.Add("RightL", typeof(decimal));
            Columns.Add("RightN", typeof(decimal));
            Columns.Add("RightD", typeof(decimal));
            Columns.Add("TopL", typeof(decimal));
            Columns.Add("TopN", typeof(decimal));
            Columns.Add("TopD", typeof(decimal));
            Columns.Add("ShapeNum", typeof(decimal));
            Columns.Add("Lite1_Thick", typeof(decimal));
            Columns.Add("Lite1_Color", typeof(string));
            Columns.Add("Lite1_Heat", typeof(string));
            Columns.Add("Lite1_1stSrfCoating", typeof(string));
            Columns.Add("Lite1_2ndSrfCoating", typeof(string));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("ReceivedDate", typeof(DateTime));
            Columns.Add("ExpirationDate", typeof(DateTime));
            Columns.Add("SqftActual", typeof(decimal));
            Columns.Add("SqftBillable", typeof(decimal));
            Columns.Add("Side1L", typeof(decimal));
            Columns.Add("Side1N", typeof(decimal));
            Columns.Add("Side1D", typeof(decimal));
            Columns.Add("Side2L", typeof(decimal));
            Columns.Add("Side2N", typeof(decimal));
            Columns.Add("Side2D", typeof(decimal));
            Columns.Add("Location", typeof(string));
            Columns.Add("Pattern", typeof(string));
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("ReceivedQty", typeof(decimal));
            Columns.Add("Status", typeof(string));
            Columns.Add("Lite1_ProdQualityLevel", typeof(string));
            Columns.Add("ProductionDate", typeof(DateTime));
            Columns.Add("ProcessID", typeof(string));
            Columns.Add("Lite1_Lami", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("LitesPerRack", typeof(decimal));
            Columns.Add("SequenceNum", typeof(decimal));
            Columns.Add("BatchNumber", typeof(int));
            Columns.Add("PlantID", typeof(string));
            Columns.Add("AnsiiLogoType", typeof(string));
            Columns.Add("GLCategory", typeof(string));
            Columns.Add("ReasonCode", typeof(string));
            Columns.Add("AssetId", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                using (reader)
                {

                    while (reader.Read())
                    {
                        DataRow dr = this.NewRow();
                        dr["ItemNum"] = Data.CastDbNull.To<string>(reader["RELITM"], String.Empty).Trim();
                        dr["SerialNum"] = Data.CastDbNull.To<string>(reader["REZ58SN"], String.Empty).Trim();
                        dr["VenderID"] = Data.CastDbNull.To<decimal>(reader["REVEND"], 0).ToString();
                        dr["BaseL"] = Data.CastDbNull.To<decimal>(reader["REZ58L1BDM"], 0);
                        dr["BaseN"] = Data.CastDbNull.To<decimal>(reader["REZ58L1BNM"], 0);
                        dr["BaseD"] = Data.CastDbNull.To<decimal>(reader["REZ58L1BDN"], 0);
                        dr["LeftL"] = Data.CastDbNull.To<decimal>(reader["REZ58L1LDM"], 0);
                        dr["LeftN"] = Data.CastDbNull.To<decimal>(reader["REZ58L1LNM"], 0);
                        dr["LeftD"] = Data.CastDbNull.To<decimal>(reader["REZ58L1LDN"], 0);
                        dr["RightL"] = Data.CastDbNull.To<decimal>(reader["REZ58L1RDM"], 0);
                        dr["RightN"] = Data.CastDbNull.To<decimal>(reader["REZ58L1RNM"], 0);
                        dr["RightD"] = Data.CastDbNull.To<decimal>(reader["REZ58L1RDN"], 0);
                        dr["TopL"] = Data.CastDbNull.To<decimal>(reader["REZ58L1TDM"], 0);
                        dr["TopN"] = Data.CastDbNull.To<decimal>(reader["REZ58L1TNM"], 0);
                        dr["TopD"] = Data.CastDbNull.To<decimal>(reader["REZ58L1TDN"], 0);
                        dr["ShapeNum"] = Data.CastStringToDecimal(Data.CastDbNull.To<string>(reader["REZ58SHAPE"], String.Empty).Trim());
                        dr["Lite1_Thick"] = Data.CastDbNull.To<decimal>(reader["REZ58L1TK"], 0) / 10;
                        dr["Lite1_Color"] = Data.CastDbNull.To<string>(reader["REZ58L1CL"], String.Empty).Trim();
                        dr["Lite1_Heat"] = Data.CastDbNull.To<string>(reader["REZ58L1HT"], String.Empty).Trim();
                        dr["Lite1_1stSrfCoating"] = Data.CastDbNull.To<string>(reader["REZ58L11C"], String.Empty).Trim();
                        dr["Lite1_2ndSrfCoating"] = Data.CastDbNull.To<string>(reader["REZ58L12C"], String.Empty).Trim();
                        dr["QtyOnHand"] = Data.CastDbNull.To<decimal>(reader["REPQOH"], 0) / 1000;
                        dr["SqftActual"] = Data.CastDbNull.To<decimal>(reader["REZ58USF"], 0) / 10000;
                        dr["SqftBillable"] = Data.CastDbNull.To<decimal>(reader["REZ58BSF"], 0) / 10000;
                        dr["Side1L"] = Data.CastDbNull.To<decimal>(reader["REZ58L11DM"], 0);
                        dr["Side1N"] = Data.CastDbNull.To<decimal>(reader["REZ58L11NM"], 0);
                        dr["Side1D"] = Data.CastDbNull.To<decimal>(reader["REZ58L11DN"], 0);
                        dr["Side2L"] = Data.CastDbNull.To<decimal>(reader["REZ58L12DM"], 0);
                        dr["Side2N"] = Data.CastDbNull.To<decimal>(reader["REZ58L12NM"], 0);
                        dr["Side2D"] = Data.CastDbNull.To<decimal>(reader["REZ58L12DN"], 0);
                        dr["Location"] = Data.CastDbNull.To<string>(reader["RELOCN"], String.Empty).Trim();
                        dr["Pattern"] = Data.CastDbNull.To<string>(reader["REZ58L1PX"], String.Empty).Trim();
                        dr["RackId"] = Data.CastDbNull.To<decimal>(reader["REZ58RID"], 0);
                        // dr["ReceivedQty"] = 0;
                        dr["Status"] = Data.CastDbNull.To<string>(reader["REZ58DST"], String.Empty).Trim();
                        dr["Lite1_ProdQualityLevel"] = Data.CastDbNull.To<string>(reader["REZ58L1PQL"], String.Empty).Trim();
                        dr["Lite1_Lami"] = Data.CastDbNull.To<string>(reader["REZ58L1LPT"], String.Empty).Trim();
                        dr["RackType"] = Data.CastDbNull.To<string>(reader["REZ58RT"], String.Empty).Trim();
                        dr["LitesPerRack"] = Data.CastDbNull.To<decimal>(reader["REZ58LPR"], 0);
                        dr["SequenceNum"] = Data.CastDbNull.To<decimal>(reader["RESEQNO"], 0);
                        dr["PlantID"] = Data.CastDbNull.To<string>(reader["REMCU"], String.Empty).Trim();
                        dr["AnsiiLogoType"] = Data.CastDbNull.To<string>(reader["REZ58L1ALT"], String.Empty).Trim();
                        dr["GLCategory"] = Data.CastDbNull.To<string>(reader["REGLPT"], String.Empty).Trim();
                        dr["ReasonCode"] = Data.CastDbNull.To<string>(reader["REZ58RCD"], String.Empty).Trim();
                        dr["AssetId"] = Data.CastDbNull.To<string>(reader["REZ58AID"], String.Empty).Trim();
                        // Dates
                        dr["WrittenDate"] = DateTime.Now;
                        var pid = Data.CastDbNull.To<string>(reader["REPID"], String.Empty).Trim();
                        var upmj = Data.CastDbNull.To<decimal>(reader["REUPMJ"], 0);
                        var tday = Data.CastDbNull.To<decimal>(reader["RETDAY"], 0);
                        var mdt = Data.CastDbNull.To<decimal>(reader["REZ58MDT"], 0);
                        dr["ProcessID"] = pid;
                        if (pid == "P4312" || pid == "R47071")
                        {
                            dr["ReceivedDate"] = Timestamp.CastToDateTime(upmj);
                            dr["ReceivedQty"] = Data.CastDbNull.To<decimal>(reader["REPQOH"], 0) / 1000;
                        }
                        else
                        {
                            dr["ReceivedDate"] = Timestamp.CastToDateTime(mdt);
                        }

                        if (mdt == 0)
                        {
                            dr["ProductionDate"] = Timestamp.CastToDateTime(upmj);
                        }
                        else
                        {
                            dr["ProductionDate"] = Timestamp.CastToDateTime(mdt);
                        }

                        var updateDateId = Data.CastDbNull.To<decimal>(reader["REUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["RETDAY"], 0);
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        this.Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add(Rack rack)
        {
            try
            {
                foreach (RackItem item in rack.RackItems)
                {
                    DataRow dr = this.NewRow();
                    dr["ItemNum"] = item.ItemNum;
                    dr["SerialNum"] = item.SerialNum;
                    dr["VenderID"] = item.SupplierNum;
                    dr["BaseL"] = item.GlassItem.Shape.BaseDimension.Base;
                    dr["BaseN"] = item.GlassItem.Shape.BaseDimension.Numerator;
                    dr["BaseD"] = item.GlassItem.Shape.BaseDimension.Denominator;
                    dr["LeftL"] = item.GlassItem.Shape.LeftDimension.Base;
                    dr["LeftN"] = item.GlassItem.Shape.LeftDimension.Numerator;
                    dr["LeftD"] = item.GlassItem.Shape.LeftDimension.Denominator;
                    dr["RightL"] = item.GlassItem.Shape.RightDimension.Base;
                    dr["RightN"] = item.GlassItem.Shape.RightDimension.Numerator;
                    dr["RightD"] = item.GlassItem.Shape.RightDimension.Denominator;
                    dr["TopL"] = item.GlassItem.Shape.TopDimension.Base;
                    dr["TopN"] = item.GlassItem.Shape.TopDimension.Numerator;
                    dr["TopD"] = item.GlassItem.Shape.TopDimension.Denominator;
                    dr["ShapeNum"] = item.GlassItem.Shape.ShapeNum;
                    dr["Lite1_Thick"] = item.GlassItem.Lites[0].GlassType.Thickness;
                    dr["Lite1_Color"] = item.GlassItem.Lites[0].GlassType.Color;
                    dr["Lite1_Heat"] = item.GlassItem.Lites[0].GlassType.HeatTreatment;
                    dr["Lite1_1stSrfCoating"] = item.GlassItem.Lites[0].GlassType.Surface1Coating;
                    dr["Lite1_2ndSrfCoating"] = item.GlassItem.Lites[0].GlassType.Surface2Coating;
                    dr["QtyOnHand"] = item.Quantity;
                    dr["SqftActual"] = item.GlassItem.Shape.ActualSqft;
                    dr["SqftBillable"] = item.GlassItem.Shape.BillableSqft;
                    dr["Side1L"] = item.GlassItem.Shape.Side1Dimension.Base;
                    dr["Side1N"] = item.GlassItem.Shape.Side1Dimension.Numerator;
                    dr["Side1D"] = item.GlassItem.Shape.Side1Dimension.Denominator;
                    dr["Side2L"] = item.GlassItem.Shape.Side2Dimension.Base;
                    dr["Side2N"] = item.GlassItem.Shape.Side2Dimension.Numerator;
                    dr["Side2D"] = item.GlassItem.Shape.Side2Dimension.Denominator;
                    dr["Location"] = rack.Location;
                    dr["Pattern"] = item.GlassItem.Lites[0].GlassType.PatternTexture;
                    dr["RackId"] = rack.RackId;
                    dr["ReceivedQty"] = item.ReceivedQty;
                    dr["Status"] = item.DetailStatus;
                    dr["Lite1_ProdQualityLevel"] = item.GlassItem.Lites[0].QualityLevel ?? "";
                    dr["Lite1_Lami"] = item.GlassItem.Lites[0].GlassType.LamiProductType;
                    dr["RackType"] = rack.RackType;
                    dr["LitesPerRack"] = item.GlassItem.LitesPerRack;
                    dr["SequenceNum"] = 0;
                    dr["PlantID"] = rack.CostCenter;
                    dr["AnsiiLogoType"] = item.GlassItem.Logo.AnsiLogo ?? "";
                    dr["GLCategory"] = item.GLCategory;
                    dr["ReasonCode"] = rack.ReasonCode;
                    // Dates
                    dr["WrittenDate"] = DateTime.Now;
                    dr["ProcessID"] = rack.ProcessId;
                    dr["ReceivedDate"] = item.ReceivedDate;
                    dr["ProductionDate"] = item.ProductionDate;
                    dr["TransactionDate"] = item.UpdateDateTime;
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
}
