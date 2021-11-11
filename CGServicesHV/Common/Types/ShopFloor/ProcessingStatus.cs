using System;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [DataContract(Namespace = "")]
    public class ProcessingStatus
    {
        public ProcessingStatus()
        {
            TransferStatus = TransferStatusCode.NotProcessed;
            ValidationStatus = ValidationStatusCode.Unknown;
        }

        [DataMember(Order = 10)] public DateTime? TransferredDate { get; set; }
        [DataMember(Order = 20)] public decimal TransferId { get; set; }
        [DataMember(Order = 30)] public decimal TransferBatchId { get; set; }
        [DataMember(Order = 40)] public string TransferMessage { get; set; }
        [DataMember(Order = 50)] public TransferStatusCode TransferStatus { get; set; }
        [DataMember(Order = 60)] public ValidationStatusCode ValidationStatus { get; set; }
        [DataMember(Order = 70)] public bool CanOverrideValidationFailure { get; set; }

        public enum TransferStatusCode
        {
            NotProcessed = 'N',
            Reserved = 'R',
            Skip = 'X',
            Error = 'E',
            Success = 'Y'
        }

        public enum ValidationStatusCode
        {
            Unknown = 'U',
            Success = 'Y',
            Warning = 'W',
            Failure = 'F',
            Error = 'E'
        }

    }
}