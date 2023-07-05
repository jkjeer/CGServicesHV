using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class LoadTable : DataTable
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

        public LoadTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[LoadLedger]");
            Columns.Add("LoadNum", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));
            Columns.Add("DueDate", typeof(DateTime));
            Columns.Add("LoadDate", typeof(DateTime));
            Columns.Add("LoadScheduledDate", typeof(DateTime));
            Columns.Add("LoadConfirmDate", typeof(DateTime));
            Columns.Add("EstimatedArrivalDate", typeof(DateTime));
            Columns.Add("ActualShipDate", typeof(DateTime));
            Columns.Add("CancelDate", typeof(DateTime));
            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("CustomerShipNum", typeof(decimal));
            Columns.Add("CarrierNum", typeof(decimal));
            Columns.Add("TrailerDesc", typeof(string));
            Columns.Add("TrailerTypeDesc", typeof(string));
            Columns.Add("FreightHandlingCode", typeof(string));
            Columns.Add("BillableFreight", typeof(decimal));
            Columns.Add("PayableFreight", typeof(decimal));
            Columns.Add("BillOfLadingNum", typeof(decimal));
            Columns.Add("StagingLocation", typeof(string));
            Columns.Add("VerifyLocation", typeof(string));
            Columns.Add("ExportFlag", typeof(string));
            Columns.Add("BookingDesc", typeof(string));
            Columns.Add("RacksShipped", typeof(decimal));
            Columns.Add("RacksLoaded", typeof(decimal));
            Columns.Add("PlannedWeight", typeof(decimal));
            Columns.Add("GlassWeight", typeof(decimal));
            Columns.Add("TotalWeight", typeof(decimal));
            Columns.Add("TotalSqft", typeof(decimal));
            Columns.Add("Comment1", typeof(string));
            Columns.Add("Comment2", typeof(string));
            Columns.Add("Comment3", typeof(string));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("OperationCode", typeof(string));
            Columns.Add("TransactionId", typeof(decimal));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["LoadNum"] = CastDbNull.To<decimal>(reader["LDNM"], 0);
                    dr["StatusCode"] = CastDbNull.To<string>(reader["Z58LDS"], String.Empty).Trim();
                    dr["DueDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["DRQJ"], 0), CastDbNull.To<decimal>(reader["DRQT"], 0));
                    dr["LoadDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["LOAD"], 0), CastDbNull.To<decimal>(reader["TMLS"], 0));
                    //dr["LoadScheduledDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["UPMJ"], 0), CastDbNull.To<decimal>(reader["TDAY"], 0));
                    dr["LoadConfirmDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["LDDT"], 0), CastDbNull.To<decimal>(reader["Z58TMLC"], 0)).AddMinutes(plant.BusinessSystemOffset); ;
                    dr["EstimatedArrivalDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["Z58EDTA"], 0), CastDbNull.To<decimal>(reader["Z58ETMA"], 0));
                    dr["ActualShipDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["ADDJ"], 0), CastDbNull.To<decimal>(reader["ADTM"], 0)).AddMinutes(plant.BusinessSystemOffset); ;
                    dr["CancelDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["CNDJ"], 0), CastDbNull.To<decimal>(reader["Z58TMCX"], 0));
                    dr["CustomerSoldNum"] = CastDbNull.To<decimal>(reader["AN8"], 0);
                    dr["CustomerShipNum"] = CastDbNull.To<decimal>(reader["SHAN"], 0);
                    dr["CarrierNum"] = CastDbNull.To<decimal>(reader["CARS"], 0);
                    dr["TrailerDesc"] = CastDbNull.To<string>(reader["Z58TN"], String.Empty).Trim();
                    dr["TrailerTypeDesc"] = CastDbNull.To<string>(reader["Z58TT"], String.Empty).Trim();
                    dr["FreightHandlingCode"] = CastDbNull.To<string>(reader["FRTH"], String.Empty).Trim();
                    dr["BillableFreight"] = CastDbNull.To<decimal>(reader["Z58FRTB"], 0) / 100;
                    dr["PayableFreight"] = CastDbNull.To<decimal>(reader["Z58FRTO"], 0) / 100;
                    dr["BillOfLadingNum"] = CastDbNull.To<decimal>(reader["BOLN"], 0);
                    dr["StagingLocation"] = CastDbNull.To<string>(reader["Z58SLOC"], String.Empty).Trim();
                    dr["VerifyLocation"] = CastDbNull.To<string>(reader["Z58VLOC"], String.Empty).Trim();
                    dr["ExportFlag"] = CastDbNull.To<string>(reader["Z58EXPRT"], String.Empty).Trim();
                    dr["BookingDesc"] = CastDbNull.To<string>(reader["Z58BKNG#"], String.Empty).Trim();
                    dr["RacksShipped"] = CastDbNull.To<decimal>(reader["Z58RKN"], 0);
                    dr["RacksLoaded"] = CastDbNull.To<decimal>(reader["Z58RKL"], 0);
                    dr["PlannedWeight"] = CastDbNull.To<decimal>(reader["Z58PLNWT"], 0);
                    dr["GlassWeight"] = CastDbNull.To<decimal>(reader["Z58GWTT"], 0) / 100;
                    dr["TotalWeight"] = CastDbNull.To<decimal>(reader["TOWT"], 0) / 10000;
                    dr["TotalSqft"] = CastDbNull.To<decimal>(reader["Z58TSF"], 0);
                    dr["Comment1"] = CastDbNull.To<string>(reader["Z58CMT1"], String.Empty).Trim();
                    dr["Comment2"] = CastDbNull.To<string>(reader["Z58CMT2"], String.Empty).Trim();
                    dr["Comment3"] = CastDbNull.To<string>(reader["Z58CMT3"], String.Empty).Trim();
                    dr["UserId"] = CastDbNull.To<string>(reader["USER"], String.Empty).Trim();
                    dr["ProgramId"] = CastDbNull.To<string>(reader["PID"], String.Empty).Trim();
                    dr["TransactionDate"] = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["UPMJ"], 0), CastDbNull.To<decimal>(reader["TDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
                    dr["OperationCode"] = CastDbNull.To<string>(reader["IOPR"], String.Empty).Trim();
                    dr["TransactionId"] = CastDbNull.To<decimal>(reader["UKID"], 0);
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
