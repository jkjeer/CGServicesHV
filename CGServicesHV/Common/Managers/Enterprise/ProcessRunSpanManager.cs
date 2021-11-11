using System;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.ShopFloor;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class ProcessRunSpanManager
    {

        public static ProcessRunSpan GetRunSpan(DateTime startDate, DateTime endDate)
        {
            var span = new ProcessRunSpan();
            try
            {
                span.StartDateTime = startDate;
                span.EndDateTime = endDate;
                span.E1TimeIdSpan = E1TimeManager.GetE1TimeIdSpan(startDate, endDate);
                span.IsEmpty = false;
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(e.ToString());
            }
            return span;
        }

        public static ProcessRunSpan GetRunSpan(HighWaterMark mark, TimeSpan timeSpan, DateTime endDateTime)
        {
            var span = new ProcessRunSpan();
            try
            {
                if (mark != null && mark.Exists)
                {
                        span.HighWaterMark = mark;
                        span.StartDateTime = mark.EnterpriseDateTime.AddSeconds(mark.LookbackSeconds * -1);
                        span.EndDateTime = endDateTime; 
                        span.E1TimeIdSpan = E1TimeManager.GetE1TimeIdSpan(span.StartDateTime, span.EndDateTime);
                        span.IsEmpty = false;
                }
                else
                {
                    if (timeSpan != TimeSpan.Zero)
                    {
                        span.HighWaterMark = new HighWaterMark();
                        span.StartDateTime = endDateTime.Add(-timeSpan);
                        span.EndDateTime = endDateTime;
                        span.E1TimeIdSpan = E1TimeManager.GetE1TimeIdSpan(span.StartDateTime, span.EndDateTime);
                        span.IsEmpty = false;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return span;
        }

    }
}
