using System;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackTable : DataTable
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

        public RackTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackLedger]");
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("PlantID", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("StatusCode", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("AssetId", typeof(string));
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("BoxNum", typeof(decimal));
            Columns.Add("GlassWeight", typeof(decimal));
            Columns.Add("TareWeight", typeof(decimal));
            Columns.Add("ReasonCode", typeof(string));
            Columns.Add("LoadNum", typeof(decimal));
            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("CustomerShipNum", typeof(decimal));
            Columns.Add("CreateDate", typeof(DateTime));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("OperationCode", typeof(string));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("RackUnitQty", typeof(decimal));
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
                        dr["TransactionId"] = Data.CastDbNull.To<decimal>(reader["HLUKID"], 0);
                        dr["RackId"] = Data.CastDbNull.To<decimal>(reader["HLZ58RID"], 0);
                        dr["PlantID"] = Data.CastDbNull.To<string>(reader["HLMCU"], String.Empty).Trim();
                        dr["RackType"] = Data.CastDbNull.To<string>(reader["HLZ58RT"], String.Empty).Trim();
                        dr["StatusCode"] = Data.CastDbNull.To<string>(reader["HLZ58HST"], String.Empty).Trim();
                        dr["Location"] = Data.CastDbNull.To<string>(reader["HLLOCN"], String.Empty).Trim();
                        dr["ReasonCode"] = Data.CastDbNull.To<string>(reader["HLZ58RCD"], String.Empty).Trim();
                        dr["BoxNum"] = Data.CastDbNull.To<decimal>(reader["HLZ58BOX"], 0);
                        dr["ScheduleNum"] = Data.CastDbNull.To<decimal>(reader["HLZ58SCN"], 0);
                        dr["AssetId"] = Data.CastDbNull.To<string>(reader["HLURRF"], String.Empty).Trim();
                        dr["TareWeight"] = Data.CastDbNull.To<decimal>(reader["HLZ58RTWT"], 0) / 100;
                        dr["GlassWeight"] = Data.CastDbNull.To<decimal>(reader["HLZ58GWTT"], 0) / 100;
                        dr["LoadNum"] = Data.CastDbNull.To<decimal>(reader["HLPSN"], 0);
                        dr["CustomerSoldNum"] = Data.CastDbNull.To<decimal>(reader["HLAN8"], 0);
                        dr["CustomerShipNum"] = Data.CastDbNull.To<decimal>(reader["HLSHAN"], 0);
                        var operationCd = Data.CastDbNull.To<string>(reader["HLIOPR"], String.Empty).Trim();
                        dr["OperationCode"] = operationCd;

                        var updateDateIdOrig = Data.CastDbNull.To<decimal>(reader["HLMUPM_ORIG"], 0);
                        var updateTimeIdOrig = Data.CastDbNull.To<decimal>(reader["HLUPMT_ORIG"], 0);
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["HLMUPM"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["HLUPMT"], 0);
                        if (operationCd == "Init")
                        {
                            dr["CreateDate"] = E1TimeManager.CastToDateTime(updateDateIdOrig, updateTimeIdOrig).AddHours(plant.TimeOffset);
                        }
                        if (operationCd == "Insert")
                        {
                            dr["CreateDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        }

                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        dr["UserId"] = Data.CastDbNull.To<string>(reader["USERID"], String.Empty).Trim();
                        dr["ProgramId"] = Data.CastDbNull.To<string>(reader["PID"], String.Empty).Trim();
                        dr["RackUnitQty"] = Data.CastDbNull.To<decimal>(reader["HLZ58NA02"], 0);
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
    }
}
