using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class ShipmentSummaryTable : DataTable
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

        public ShipmentSummaryTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ShipmentSummaryLedger]");
            Columns.Add("CostCenter", typeof(string));
            Columns.Add("ShipmentID", typeof(decimal));
            Columns.Add("AddressNumber", typeof(decimal));
            Columns.Add("AddressNumberShipTo", typeof(decimal));
            Columns.Add("Carrier", typeof(decimal));
            Columns.Add("TrailerNumber", typeof(string));
            Columns.Add("TrailerType", typeof(string));
            Columns.Add("FreightHandlingCode", typeof(string));
            Columns.Add("LoadZone", typeof(string));
            Columns.Add("NumberOfStops", typeof(decimal));
            Columns.Add("NumberOfRacks", typeof(decimal));
            Columns.Add("MaximumTotalRackWeight", typeof(decimal));
            Columns.Add("DateRequested", typeof(DateTime));
            Columns.Add("PickSlipNumber", typeof(decimal));
            Columns.Add("DateLoaded", typeof(DateTime));
            Columns.Add("RequestedDeliveryTime", typeof(decimal));
            Columns.Add("CommentOne", typeof(string));
            Columns.Add("CommentTwo", typeof(string));
            Columns.Add("CommentThree", typeof(string));
            Columns.Add("EdiSuccessfullyProcess", typeof(string));
            Columns.Add("UserReservedCode", typeof(string));
            Columns.Add("UserReservedReference", typeof(string));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("WorkStationId", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("TransactionID", typeof(decimal));
            Columns.Add("MUserName", typeof(string));
            Columns.Add("MJobName", typeof(string));
            Columns.Add("MJobNumber", typeof(string));
            Columns.Add("InternalOperation", typeof(string));
            Columns.Add("MTransactionDate", typeof(DateTime));
            Columns.Add("CreateDate", typeof(DateTime));
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
                        dr["CostCenter"] = Data.CastDbNull.To(reader["SLMCU"], string.Empty).Trim();
                        dr["ShipmentID"] = Data.CastDbNull.To<decimal>(reader["SLZ58SHID"], 0);
                        dr["AddressNumber"] = Data.CastDbNull.To<decimal>(reader["SLAN8"], 0);
                        dr["AddressNumberShipTo"] = Data.CastDbNull.To<decimal>(reader["SLSHAN"], 0);
                        dr["Carrier"] = Data.CastDbNull.To<decimal>(reader["SLCARS"], 0);
                        dr["TrailerNumber"] = Data.CastDbNull.To(reader["SLZ58TN"], string.Empty).Trim();
                        dr["TrailerType"] = Data.CastDbNull.To(reader["SLZ58TT"], string.Empty).Trim();
                        dr["FreightHandlingCode"] = Data.CastDbNull.To(reader["SLFRTH"], string.Empty).Trim();
                        dr["LoadZone"] = Data.CastDbNull.To(reader["SLZ58LOZN"], string.Empty).Trim();
                        dr["NumberOfStops"] = Data.CastDbNull.To<decimal>(reader["SLNSTP"], 0);
                        dr["NumberOfRacks"] = Data.CastDbNull.To<decimal>(reader["SLZ58RKN"], 0);
                        dr["MaximumTotalRackWeight"] = Data.CastDbNull.To<decimal>(reader["SLZ58MTRW"], 0) / 100;
                        //dr["DateRequested"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SLDRQJ"], 0));
                        dr["DateRequested"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SLLOAD"], 0));
                        dr["PickSlipNumber"] = Data.CastDbNull.To<decimal>(reader["SLPSN"], 0);
                        dr["DateLoaded"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SLLOAD"], 0));
                        dr["RequestedDeliveryTime"] = Data.CastDbNull.To<decimal>(reader["SLDRQT"], 0);
                        dr["CommentOne"] = Data.CastDbNull.To(reader["SLZ58CMT1"], string.Empty).Trim();
                        dr["CommentTwo"] = Data.CastDbNull.To(reader["SLZ58CMT2"], string.Empty).Trim();
                        dr["CommentThree"] = Data.CastDbNull.To(reader["SLZ58CMT3"], string.Empty).Trim();
                        dr["EdiSuccessfullyProcess"] = Data.CastDbNull.To(reader["SLEDSP"], string.Empty).Trim();
                        dr["UserReservedCode"] = Data.CastDbNull.To(reader["SLURCD"], string.Empty).Trim();
                        dr["UserReservedReference"] = Data.CastDbNull.To(reader["SLURRF"], string.Empty).Trim();
                        dr["UserId"] = Data.CastDbNull.To(reader["SLUSER"], string.Empty).Trim();
                        dr["ProgramId"] = Data.CastDbNull.To(reader["SLPID"], string.Empty).Trim();
                        dr["WorkStationId"] = Data.CastDbNull.To(reader["SLJOBN"], string.Empty).Trim();
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["SLUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["SLUPMT"], 0);
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        dr["TransactionID"] = Data.CastDbNull.To<decimal>(reader["SLUKID"], 0);
                        dr["MUserName"] = Data.CastDbNull.To(reader["SLMUSE"], string.Empty).Trim();
                        dr["MJobName"] = Data.CastDbNull.To(reader["SLJBNM"], string.Empty).Trim();
                        dr["MJobNumber"] = Data.CastDbNull.To(reader["SLJOB"], string.Empty).Trim();
                        var operationCd = Data.CastDbNull.To(reader["SLIOPR"], string.Empty).Trim();
                        dr["InternalOperation"] = operationCd;
                        if (operationCd == "Insert")
                        {
                            dr["CreateDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        }
                        var updateDateIdM = Data.CastDbNull.To<decimal>(reader["SLMUPM"], 0);
                        var updateTimeIdM = Data.CastDbNull.To<decimal>(reader["SLMUPT"], 0);
                        dr["MTransactionDate"] = E1TimeManager.CastToDateTime(updateDateIdM, updateTimeIdM).AddHours(plant.TimeOffset);
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
