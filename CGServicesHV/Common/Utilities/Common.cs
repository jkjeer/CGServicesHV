using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CGServicesHV.Common.Utilities
{
    public class Common
    {
        public static string GetAllProperties(object obj)
        {
            return string.Join(" ", obj.GetType()
                .GetProperties()
                .Select(prop => prop.GetValue(obj)));
        }

        public static string PersistSqlString(string inputString)
        {
            var retValue = "";
            if (!string.IsNullOrEmpty(inputString))
            {
                retValue = inputString.Replace("'", "''");
            }
            return retValue;
        }

        public static string RetrieveSqlString(string inputString)
        {
            return inputString.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
        }

        public static string OmitBrackets(string inputString)
        {
            return inputString.Replace("[", "").Replace("]", "");
        }

        public static DateTime ConvertE1Date(decimal dateId)
        {
            //var dateBase = DateTime.SpecifyKind(new DateTime(1900, 1, 1), DateTimeKind.Unspecified);
            var dateBase = new DateTime(1900, 1, 1);
            var dateDays = 0;
            var dateYears = 0;
            try
            {
                var dateString = Convert.ToString(dateId, CultureInfo.InvariantCulture);
                var dateStringLength = dateString.Length;
                if (dateStringLength == 5 | dateStringLength == 6)
                {
                    dateDays = Convert.ToInt16(dateString.Substring(dateStringLength - 3, 3)) - 1;
                    dateYears = Convert.ToInt16(dateString.Substring(0, dateStringLength - 3));
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return dateBase.AddYears(dateYears).AddDays(dateDays);
        }

        public static decimal CastToE1Date(DateTime inputDate)
        {
            return (inputDate.Year - 1900)*1000 + inputDate.DayOfYear;
        }

        public static decimal CastToE1Time(DateTime inputDate)
        {
            return (inputDate.Hour*10000) + (inputDate.Minute*100) + inputDate.Second;
        }

        public static DateTime CastToDateTime(decimal dateId, decimal timeId = 0)
        {
            var dt = ConvertE1Date(dateId);
            Data.CastDbNull.To<decimal>(timeId, 0);
            try
            {
                var timeString = Convert.ToString(timeId, CultureInfo.InvariantCulture).PadLeft(6).Replace(" ", "0");
                var hours = Convert.ToInt16(timeString.Substring(0, 2));
                var minutes = Convert.ToInt16(timeString.Substring(2, 2));
                var seconds = Convert.ToInt16(timeString.Substring(4, 2));
                var ts = new TimeSpan(hours, minutes, seconds);
                return dt.Add(ts);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public static DateTime CastSttmToDateTime(string sttm)
        {
            try
            {
                var years = Convert.ToInt16(sttm.Substring(0, 4));
                var months = Convert.ToInt16(sttm.Substring(5, 2));
                var days = Convert.ToInt16(sttm.Substring(8, 2));
                var hours = Convert.ToInt16(sttm.Substring(11, 2));
                var minutes = Convert.ToInt16(sttm.Substring(14, 2));
                var seconds = Convert.ToInt16(sttm.Substring(17, 2));
                return new DateTime(years, months, days, hours, minutes, seconds);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public static decimal CastStringToDecimal(string inputValue)
        {
            try
            {
                return Convert.ToDecimal(inputValue);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string CastToFractionalDimension(decimal whole, decimal num, decimal den)
        {
            var sb = new StringBuilder();
            if (den > 0)
            {
                sb.Append(Convert.ToString(whole, CultureInfo.CurrentCulture));
                sb.Append(" ");
                sb.Append(Convert.ToString(num, CultureInfo.CurrentCulture));
                sb.Append("/");
                sb.Append(Convert.ToString(den, CultureInfo.CurrentCulture));
            }
            else
            {
                sb.Append(Convert.ToString(whole, CultureInfo.CurrentCulture));
            }

            if (sb.ToString() == "0")
            {
                return "";
            }
            else
            {
                return sb.ToString();
            }

        }

        public static TimeSpan GetTimeSpan(string days, string hours, string minutes)
        {
            TimeSpan timeSpan;
            try
            {
                var daysInt = (string.IsNullOrEmpty(days)) ? 0 : Convert.ToInt16(days);
                var hoursInt = (string.IsNullOrEmpty(hours)) ? 0 : Convert.ToInt16(hours);
                var minsInt = (string.IsNullOrEmpty(minutes)) ? 0 : Convert.ToInt16(minutes);
                timeSpan = new TimeSpan(daysInt, hoursInt, minsInt, 0);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.StackTrace);
            }
            return timeSpan;
        }

        public static Dictionary<string, decimal> GetE1DateHash(double days)
        {
            var hash = new Dictionary<string, decimal>();
            var adjDateTime = DateTime.Now.AddDays(days * -1);
            if (DateTime.Now.Date == adjDateTime.Date)
            {
                hash.Add("Date1", CastToE1Date(adjDateTime.Date));
                hash.Add("Time1", CastToE1Time(adjDateTime));
                hash.Add("Date2", 999999);
            }
            else
            {
                hash.Add("Date1", CastToE1Date(adjDateTime.Date));
                hash.Add("Time1", CastToE1Time(adjDateTime));
                hash.Add("Date2", CastToE1Date(adjDateTime.Date.AddDays(1)));
            }
            return hash;
        }
    }
}
