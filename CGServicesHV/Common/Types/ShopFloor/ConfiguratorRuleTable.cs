using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ConfiguratorRuleTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public ConfiguratorRuleTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ConfiguratorRulesLedger]");
            Columns.Add("ConfiguratorRulesLedgerID", typeof(decimal));
            Columns.Add("RulesTableType", typeof(string));
            Columns.Add("RulesTableName", typeof(string));
            Columns.Add("CostCenter", typeof(string));
            Columns.Add("ConfiguredTableKeyID", typeof(string));
            Columns.Add("TableSegmentValue1", typeof(string));
            Columns.Add("TableSegmentValue2", typeof(string));
            Columns.Add("TableSegmentValue3", typeof(string));
            Columns.Add("TableSegmentValue4", typeof(string));
            Columns.Add("TableSegmentValue5", typeof(string));
            Columns.Add("TableSegmentValue6", typeof(string));
            Columns.Add("TableSegmentValue7", typeof(string));
            Columns.Add("TableSegmentValue8", typeof(string));
            Columns.Add("TableSegmentValue9", typeof(string));
            Columns.Add("TableSegmentValue0", typeof(string));
            Columns.Add("TableSegmentValue11", typeof(string));
            Columns.Add("TableSegmentValue12", typeof(string));
            Columns.Add("TableSegmentValue13", typeof(string));
            Columns.Add("TableSegmentValue14", typeof(string));
            Columns.Add("TableSegmentValue15", typeof(string));
            Columns.Add("TableSegmentValue16", typeof(string));
            Columns.Add("TableSegmentValue17", typeof(string));
            Columns.Add("TableSegmentValue18", typeof(string));
            Columns.Add("TableSegmentValue19", typeof(string));
            Columns.Add("TableSegmentValue20", typeof(string));
            Columns.Add("BubbleSequence", typeof(decimal));
            Columns.Add("EffectiveFromDate", typeof(decimal));
            Columns.Add("EffectiveThruDate", typeof(decimal));
            Columns.Add("RulesTableValue", typeof(string));
            Columns.Add("UnitPriceEntered", typeof(decimal));
            Columns.Add("IdentifierShortItem", typeof(decimal));
            Columns.Add("QtyRequiredStandard", typeof(decimal));
            Columns.Add("UnitOfMeasure", typeof(string));
            Columns.Add("UserReservedCode", typeof(string));
            Columns.Add("UserReservedDate", typeof(decimal));
            Columns.Add("UserReservedAmount", typeof(decimal));
            Columns.Add("UserReservedNumber", typeof(decimal));
            Columns.Add("UserReservedReference", typeof(string));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
            Columns.Add("WorkStationId", typeof(string));
            Columns.Add("UpdateDate", typeof(DateTime));
            Columns.Add("PlantID", typeof(string));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                using (reader)
                {

                    while (reader.Read())
                    {
                        var dr = NewRow();
                        dr["RulesTableType"] = CastDbNull.To(reader["RDRTBT"], string.Empty).Trim();
                        dr["RulesTableName"] = CastDbNull.To(reader["RDTBLC"], string.Empty).Trim();
                        dr["CostCenter"] = CastDbNull.To(reader["RDMCU"], string.Empty).Trim();
                        dr["ConfiguredTableKeyID"] = CastDbNull.To(reader["RDCTKID"], string.Empty).Trim();
                        dr["TableSegmentValue1"] = CastDbNull.To(reader["RDTSV1"], string.Empty).Trim();
                        dr["TableSegmentValue2"] = CastDbNull.To(reader["RDTSV2"], string.Empty).Trim();
                        dr["TableSegmentValue3"] = CastDbNull.To(reader["RDTSV3"], string.Empty).Trim();
                        dr["TableSegmentValue4"] = CastDbNull.To(reader["RDTSV4"], string.Empty).Trim();
                        dr["TableSegmentValue5"] = CastDbNull.To(reader["RDTSV5"], string.Empty).Trim();
                        dr["TableSegmentValue6"] = CastDbNull.To(reader["RDTSV6"], string.Empty).Trim();
                        dr["TableSegmentValue7"] = CastDbNull.To(reader["RDTSV7"], string.Empty).Trim();
                        dr["TableSegmentValue8"] = CastDbNull.To(reader["RDTSV8"], string.Empty).Trim();
                        dr["TableSegmentValue9"] = CastDbNull.To(reader["RDTSV9"], string.Empty).Trim();
                        dr["TableSegmentValue0"] = CastDbNull.To(reader["RDTSV0"], string.Empty).Trim();
                        dr["TableSegmentValue11"] = CastDbNull.To(reader["RDTSV11"], string.Empty).Trim();
                        dr["TableSegmentValue12"] = CastDbNull.To(reader["RDTSV12"], string.Empty).Trim();
                        dr["TableSegmentValue13"] = CastDbNull.To(reader["RDTSV13"], string.Empty).Trim();
                        dr["TableSegmentValue14"] = CastDbNull.To(reader["RDTSV14"], string.Empty).Trim();
                        dr["TableSegmentValue15"] = CastDbNull.To(reader["RDTSV15"], string.Empty).Trim();
                        dr["TableSegmentValue16"] = CastDbNull.To(reader["RDTSV16"], string.Empty).Trim();
                        dr["TableSegmentValue17"] = CastDbNull.To(reader["RDTSV17"], string.Empty).Trim();
                        dr["TableSegmentValue18"] = CastDbNull.To(reader["RDTSV18"], string.Empty).Trim();
                        dr["TableSegmentValue19"] = CastDbNull.To(reader["RDTSV19"], string.Empty).Trim();
                        dr["TableSegmentValue20"] = CastDbNull.To(reader["RDTSV20"], string.Empty).Trim();
                        dr["BubbleSequence"] = CastDbNull.To<decimal>(reader["RDBSEQ"], 0);
                        dr["EffectiveFromDate"] = CastDbNull.To<decimal>(reader["RDEFFF"], 0);
                        dr["EffectiveThruDate"] = CastDbNull.To<decimal>(reader["RDEFFT"], 0);
                        dr["RulesTableValue"] = CastDbNull.To(reader["RDTVAL"], string.Empty).Trim();
                        dr["UnitPriceEntered"] = CastDbNull.To<decimal>(reader["RDUPAX"], 0);
                        dr["IdentifierShortItem"] = CastDbNull.To<decimal>(reader["RDITM"], 0);
                        dr["QtyRequiredStandard"] = CastDbNull.To<decimal>(reader["RDQNTY"], 0);
                        dr["UnitOfMeasure"] = CastDbNull.To(reader["RDUM"], string.Empty).Trim();
                        dr["UserReservedCode"] = CastDbNull.To(reader["RDURCD"], string.Empty).Trim();
                        dr["UserReservedDate"] = CastDbNull.To<decimal>(reader["RDURDT"], 0);
                        dr["UserReservedAmount"] = CastDbNull.To<decimal>(reader["RDURAT"], 0);
                        dr["UserReservedNumber"] = CastDbNull.To<decimal>(reader["RDURAB"], 0);
                        dr["UserReservedReference"] = CastDbNull.To(reader["RDURRF"], string.Empty).Trim();
                        dr["UserId"] = CastDbNull.To(reader["RDUSER"], string.Empty).Trim();
                        dr["ProgramId"] = CastDbNull.To(reader["RDPID"], string.Empty).Trim();
                        dr["WorkStationId"] = CastDbNull.To(reader["RDJOBN"], string.Empty).Trim();
                        var updateDateId = CastDbNull.To<decimal>(reader["RDUPMJ"], 0);
                        var updateTimeId = CastDbNull.To<decimal>(reader["RDTDAY"], 0);
                        dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                        dr["PlantID"] = CastDbNull.To(reader["RDMCU"], string.Empty).Trim();
                        Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new FaultException(e.Message);
            }
        }
    }
}
