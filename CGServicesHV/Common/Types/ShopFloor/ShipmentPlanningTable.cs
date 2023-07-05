using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class ShipmentPlanningTable : DataTable
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

        public ShipmentPlanningTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ShipmentPlanningLedger]");
            Columns.Add("ShipmentPlanID", typeof(string));
            Columns.Add("SequenceNumber2", typeof(decimal));
            Columns.Add("EdiSuccessfullyProcess", typeof(string));
            Columns.Add("ShipPriority", typeof(decimal));
            Columns.Add("LoadZone", typeof(string));
            Columns.Add("OrderType", typeof(string));
            Columns.Add("CostCenter", typeof(string));
            Columns.Add("AddressNumberShipTo", typeof(decimal));
            Columns.Add("City", typeof(string));
            Columns.Add("State", typeof(string));
            Columns.Add("AddressNumber", typeof(decimal));
            Columns.Add("NumberRacks", typeof(decimal));
            Columns.Add("DateRequested", typeof(DateTime));
            Columns.Add("TotalGlassWeight", typeof(decimal));
            Columns.Add("MaximumTotalRackWeight", typeof(decimal));
            Columns.Add("LitesPerRack", typeof(decimal));
            Columns.Add("RackID", typeof(decimal));
            Columns.Add("RackHeaderStatus", typeof(string));
            Columns.Add("AssetID", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("RequiredSuggestedFlag", typeof(string));
            Columns.Add("DocumentNumber", typeof(decimal));
            Columns.Add("DocumentType", typeof(string));
            Columns.Add("DocumentCompany", typeof(string));
            Columns.Add("DocumentLineNumber", typeof(decimal));
            Columns.Add("Reference1", typeof(string));
            Columns.Add("ItemNumber", typeof(decimal));
            Columns.Add("DescriptionLine2", typeof(string));
            Columns.Add("BoxNumber", typeof(decimal));
            Columns.Add("ScheduleNumber", typeof(decimal));
            Columns.Add("Carrier", typeof(decimal));
            Columns.Add("ShippingLane", typeof(string));
            Columns.Add("ShippingRoute", typeof(string));
            Columns.Add("TrailerNumber", typeof(string));
            Columns.Add("FreightHandlingCode", typeof(string));
            Columns.Add("ProductType", typeof(string));
            Columns.Add("PickSlipNumber", typeof(decimal));
            Columns.Add("ShipmentID", typeof(decimal));
            Columns.Add("UserReservedCode", typeof(string));
            Columns.Add("UserReservedReference", typeof(string));
            Columns.Add("JobNumberA", typeof(decimal));
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
            Columns.Add("RackLocation", typeof(string));
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
                        dr["ShipmentPlanID"] = CastDbNull.To(reader["PLZ58SPID"], string.Empty).Trim();
                        dr["SequenceNumber2"] = CastDbNull.To<decimal>(reader["PLSEQN"], 0);
                        dr["EdiSuccessfullyProcess"] = CastDbNull.To(reader["PLEDSP"], string.Empty).Trim();
                        dr["ShipPriority"] = CastDbNull.To<decimal>(reader["PLZ58SPRT"], 0);
                        dr["LoadZone"] = CastDbNull.To(reader["PLZ58LOZN"], string.Empty).Trim();
                        dr["OrderType"] = CastDbNull.To(reader["PLZ58ORTY"], string.Empty).Trim();
                        dr["CostCenter"] = CastDbNull.To(reader["PLMCU"], string.Empty).Trim();
                        dr["AddressNumberShipTo"] = CastDbNull.To<decimal>(reader["PLSHAN"], 0);
                        dr["City"] = CastDbNull.To(reader["PLCTY1"], string.Empty).Trim();
                        dr["State"] = CastDbNull.To(reader["PLADDS"], string.Empty).Trim();
                        dr["AddressNumber"] = CastDbNull.To<decimal>(reader["PLAN8"], 0);
                        dr["NumberRacks"] = CastDbNull.To<decimal>(reader["PLZ58RKS"], 0);
                        dr["DateRequested"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["PLDRQJ"], 0));
                        dr["TotalGlassWeight"] = CastDbNull.To<decimal>(reader["PLZ58GWTT"], 0) / 100;
                        dr["MaximumTotalRackWeight"] = CastDbNull.To<decimal>(reader["PLZ58MTRW"], 0) / 100;
                        dr["LitesPerRack"] = CastDbNull.To<decimal>(reader["PLZ58LPR"], 0);
                        dr["RackID"] = CastDbNull.To<decimal>(reader["PLZ58RID"], 0);
                        dr["RackHeaderStatus"] = CastDbNull.To(reader["PLZ58HST"], string.Empty).Trim();
                        dr["AssetID"] = CastDbNull.To(reader["PLZ58AID"], string.Empty).Trim();
                        dr["RackType"] = CastDbNull.To(reader["PLZ58RT"], string.Empty).Trim();
                        dr["RequiredSuggestedFlag"] = CastDbNull.To(reader["PLZ58RQSG"], string.Empty).Trim();
                        dr["DocumentNumber"] = CastDbNull.To<decimal>(reader["PLDOCO"], 0);
                        dr["DocumentType"] = CastDbNull.To(reader["PLDCTO"], string.Empty).Trim();
                        dr["DocumentCompany"] = CastDbNull.To(reader["PLKCOO"], string.Empty).Trim();
                        dr["DocumentLineNumber"] = CastDbNull.To<decimal>(reader["PLLNID"], 0) / 1000;
                        dr["Reference1"] = CastDbNull.To(reader["PLVR01"], string.Empty).Trim();
                        dr["ItemNumber"] = CastDbNull.To<decimal>(reader["PLITM"], 0);
                        dr["DescriptionLine2"] = CastDbNull.To(reader["PLDSC2"], string.Empty).Trim();
                        dr["BoxNumber"] = CastDbNull.To<decimal>(reader["PLZ58BOX"], 0);
                        dr["ScheduleNumber"] = CastDbNull.To<decimal>(reader["PLZ58SCN"], 0);
                        dr["Carrier"] = CastDbNull.To<decimal>(reader["PLCARS"], 0);
                        dr["ShippingLane"] = CastDbNull.To(reader["PLZ58SHLN"], string.Empty).Trim();
                        dr["ShippingRoute"] = CastDbNull.To(reader["PLZ58SHRT"], string.Empty).Trim();
                        dr["TrailerNumber"] = CastDbNull.To(reader["PLZ58TN"], string.Empty).Trim();
                        dr["FreightHandlingCode"] = CastDbNull.To(reader["PLFRTH"], string.Empty).Trim();
                        dr["ProductType"] = CastDbNull.To(reader["PLZ58PT"], string.Empty).Trim();
                        dr["PickSlipNumber"] = CastDbNull.To<decimal>(reader["PLPSN"], 0);
                        dr["ShipmentID"] = CastDbNull.To<decimal>(reader["PLZ58SHID"], 0);
                        dr["UserReservedCode"] = CastDbNull.To(reader["PLURCD"], string.Empty).Trim();
                        dr["UserReservedReference"] = CastDbNull.To(reader["PLURRF"], string.Empty).Trim();
                        dr["JobnumberA"] = CastDbNull.To<decimal>(reader["PLJOBS"], 0);
                        dr["UserId"] = CastDbNull.To(reader["PLUSER"], string.Empty).Trim();
                        dr["ProgramId"] = CastDbNull.To(reader["PLPID"], string.Empty).Trim();
                        dr["WorkStationId"] = CastDbNull.To(reader["PLJOBN"], string.Empty).Trim();
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["PLUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["PLUPMT"], 0);
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        dr["TransactionID"] = CastDbNull.To<decimal>(reader["PLUKID"], 0);
                        dr["MUserName"] = CastDbNull.To(reader["PLMUSE"], string.Empty).Trim();
                        dr["MJobName"] = CastDbNull.To(reader["PLJBNM"], string.Empty).Trim();
                        dr["MJobNumber"] = CastDbNull.To(reader["PLJOB"], string.Empty).Trim();
                        var operationCd = CastDbNull.To(reader["PLIOPR"], string.Empty).Trim();
                        dr["InternalOperation"] = operationCd;
                        if (operationCd == "Insert")
                        {
                            dr["CreateDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        }
                        var updateDateIdM = Data.CastDbNull.To<decimal>(reader["PLMUPM"], 0);
                        var updateTimeIdM = Data.CastDbNull.To<decimal>(reader["PLMUPT"], 0);
                        dr["MTransactionDate"] = E1TimeManager.CastToDateTime(updateDateIdM, updateTimeIdM).AddHours(plant.TimeOffset);
                        dr["RackLocation"] = CastDbNull.To(reader["RHLOCN"], string.Empty).Trim();
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
