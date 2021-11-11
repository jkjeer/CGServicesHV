using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ConfiguratorRules : List<ConfiguratorRule>
    {
    }

    [DataContract(Namespace = "")]
    public class ConfiguratorRule
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string RulesTableType { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string RulesTableName { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string CostCenter { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string ConfiguredTableKeyID { get; set; }
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string TableSegmentValue1 { get; set; }
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string TableSegmentValue2 { get; set; }
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public string TableSegmentValue3 { get; set; }
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public string TableSegmentValue4 { get; set; }
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string TableSegmentValue5 { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string TableSegmentValue6 { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string TableSegmentValue7 { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string TableSegmentValue8 { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string TableSegmentValue9 { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string TableSegmentValue0 { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string TableSegmentValue11 { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string TableSegmentValue12 { get; set; }
        [DataMember(Order = 17, EmitDefaultValue = false)]
        public string TableSegmentValue13 { get; set; }
        [DataMember(Order = 18, EmitDefaultValue = false)]
        public string TableSegmentValue14 { get; set; }
        [DataMember(Order = 19, EmitDefaultValue = false)]
        public string TableSegmentValue15 { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string TableSegmentValue16 { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string TableSegmentValue17 { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string TableSegmentValue18 { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public string TableSegmentValue19 { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string TableSegmentValue20 { get; set; }
        [DataMember(Order = 25, EmitDefaultValue = false)]
        public decimal BubbleSequence { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public decimal EffectiveFromDate { get; set; }
        [DataMember(Order = 27, EmitDefaultValue = false)]
        public decimal EffectiveThruDate { get; set; }
        [DataMember(Order = 28, EmitDefaultValue = false)]
        public string RulesTableValue { get; set; }
        [DataMember(Order = 29, EmitDefaultValue = false)]
        public decimal UnitPriceEntered { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal IdentifierShortItem { get; set; }
        [DataMember(Order = 31, EmitDefaultValue = false)]
        public decimal QtyRequiredStandard { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string UnitOfMeasure { get; set; }
        [DataMember(Order = 33, EmitDefaultValue = false)]
        public string UserReservedCode { get; set; }
        [DataMember(Order = 34, EmitDefaultValue = false)]
        public decimal UserReservedDate { get; set; }
        [DataMember(Order = 35, EmitDefaultValue = false)]
        public decimal UserReservedAmount { get; set; }
        [DataMember(Order = 36, EmitDefaultValue = false)]
        public decimal UserReservedNumber { get; set; }
        [DataMember(Order = 37, EmitDefaultValue = false)]
        public string UserReservedReference { get; set; }
        [DataMember(Order = 38, EmitDefaultValue = false)]
        public string UserId { get; set; }
        [DataMember(Order = 39, EmitDefaultValue = false)]
        public string ProgramId { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string WorkStationId { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDateTime { get; set; }
    }
}
