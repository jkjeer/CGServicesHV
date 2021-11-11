using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace CGServices.Common.Types.Response
{
    [CollectionDataContract(Namespace = "")]
    public class ResponseTimes : List<ResponseTime>
    {
    }

    [DataContract(Namespace = "")]
    public class ResponseTime
    {
        public ResponseTime()
        {
        }

        public ResponseTime(string name)
        {
            Name = name;
            StartTime = DateTime.Now;
        }

        public ResponseTime(string name, string description, string connectionString)
        {
            Name = name;
            Description = description;
            ConnectionString = connectionString;
            StartTime = DateTime.Now;
            StartTimeString = StartTime.ToString(CultureInfo.CurrentCulture);
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string Description { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string ConnectionString { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public DateTime StartTime { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public DateTime EndTime { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = true)]
        public decimal ElapsedSeconds { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string StartTimeString { get; set; }

        public ResponseTime Done()
        {
            EndTime = DateTime.Now;
            ElapsedSeconds = (decimal)(EndTime - StartTime).TotalSeconds;
            return this;
        }
    }
}
