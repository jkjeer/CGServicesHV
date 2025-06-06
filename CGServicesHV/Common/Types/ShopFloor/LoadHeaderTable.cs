using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;
using System;
using System.Data;
using System.Linq;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class LoadHeaderTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        public LoadHeaderTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[LoadHeaderLedger]");
            Columns.Add("LoadNumber", typeof(decimal));
            Columns.Add("CostCenter", typeof(string));
            Columns.Add("DateRequested", typeof(DateTime));
            Columns.Add("AddressNumber", typeof(decimal));
            Columns.Add("AddressNumberShipTo", typeof(decimal));
            Columns.Add("Carrier", typeof(decimal));
            Columns.Add("TrailerNumber", typeof(string));
            Columns.Add("VerifyLocation", typeof(string));
            Columns.Add("GenericNumber2", typeof(decimal));
            Columns.Add("ReferenceNumber1", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("TransactionID", typeof(decimal));
            Columns.Add("MUserName", typeof(string));
            Columns.Add("MJobName", typeof(string));
            Columns.Add("MJobNumber", typeof(string));
            Columns.Add("InternalOperation", typeof(string));
            Columns.Add("MTransactionDate", typeof(DateTime));
            Columns.Add("CreateDate", typeof(DateTime));
            Columns.Add("UserName", typeof(string));
            Columns.Add("Comment1", typeof(string));
            Columns.Add("Comment2", typeof(string));
            Columns.Add("Comment3", typeof(string));
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
                        dr["LoadNumber"] = reader.Get<decimal>("LHLDNM");
                        dr["CostCenter"] = reader.Get<string>("LHMCU");
                        var requestDateId = Data.CastDbNull.To<decimal>(reader["LHDRQJ"], 0);
                        var requestTimeId = Data.CastDbNull.To<decimal>(reader["LHDRQT"], 0);
                        dr["DateRequested"] = E1TimeManager.CastToDateTime(requestDateId, requestTimeId).AddHours(plant.TimeOffset);
                        dr["AddressNumber"] = reader.Get<decimal>("LHAN8");
                        dr["AddressNumberShipTo"] = reader.Get<decimal>("LHSHAN");
                        dr["Carrier"] = reader.Get<decimal>("LHCARS");
                        dr["TrailerNumber"] = reader.Get<string>("LHZ58TN");
                        dr["VerifyLocation"] = reader.Get<string>("LHZ58VLOC");
                        //dr["GenericNumber2"] = reader.Get<decimal>("LHCFNO2");
                        //dr["ReferenceNumber1"] = reader.Get<string>("LHRF1");
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["LHUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["LHTDAY"], 0);
                        dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        //dr["TransactionID"] = CastDbNull.To<decimal>(reader["LHUKID"], 0);
                        dr["UserName"] = CastDbNull.To(reader["LHUSER"], string.Empty).Trim();
                        //dr["MJobName"] = CastDbNull.To(reader["LHJBNM"], string.Empty).Trim();
                        //dr["MJobNumber"] = CastDbNull.To(reader["LHJOB"], string.Empty).Trim();
                        //var operationCd = CastDbNull.To(reader["LHIOPR"], string.Empty).Trim();
                        //dr["InternalOperation"] = operationCd;
                        //if (operationCd == "Insert")
                        //{
                        //    dr["CreateDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        //}
                        //var updateDateIdM = Data.CastDbNull.To<decimal>(reader["LHUPMJ"], 0);
                        //var updateTimeIdM = Data.CastDbNull.To<decimal>(reader["LHTDAY"], 0);
                        //dr["MTransactionDate"] = E1TimeManager.CastToDateTime(updateDateIdM, updateTimeIdM).AddHours(plant.TimeOffset);
                        dr["Comment1"] = reader.Get<string>("LHZ58CMT1");
                        dr["Comment2"] = reader.Get<string>("LHZ58CMT2");
                        dr["Comment3"] = reader.Get<string>("LHZ58CMT3");
                        this.Rows.Add(dr);
                    }

                    reader.Close();
                    Logger.Trace($"Rows: {this.Rows.Count}");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
