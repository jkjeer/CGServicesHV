using System;
using System.Runtime.Serialization;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.ShopFloor;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class ProcessRunSpan
    {
        public ProcessRunSpan()
        {
            IsEmpty = true;
            HighWaterMark = new HighWaterMark();
        }

        [DataMember(Order = 10)]
        public DateTime StartDateTime { get; set; }

        [DataMember(Order = 12)]
        public DateTime BusinessStartDateTime { get; set; }

        [DataMember(Order = 14)]
        public DateTime LocalStartDateTime { get; set; }

        [DataMember(Order = 20)]
        public DateTime EndDateTime { get; set; }

        [DataMember(Order = 22)]
        public DateTime BusinessEndDateTime { get; set; }

        [DataMember(Order = 24)]
        public DateTime LocalEndDateTime { get; set; }

        [DataMember(Order = 50)]
        public E1TimeIdSpan E1TimeIdSpan { get; set; }

        [DataMember(Order = 100)]
        public HighWaterMark HighWaterMark { get; set; }

        [DataMember(Order = 200)]
        public bool IsEmpty { get; set; }

        public string Serialize()
        {
            if (HighWaterMark.Exists)
            {
                return
                    $"StartDate: {StartDateTime} EndDate: {EndDateTime} HighWaterMarkName: {HighWaterMark.HighWaterMarkName} HighWaterMarkValue: {HighWaterMark.UniqueID} MaxHighWaterMarkValue: {HighWaterMark.MaxUniqueID}";
            }
            return
                $"StartDate: {StartDateTime} EndDate: {EndDateTime}";
        }
    }
}
