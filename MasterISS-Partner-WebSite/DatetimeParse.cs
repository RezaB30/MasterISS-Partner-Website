using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class DatetimeParse
    {
        public string ConvertDatetimeByWebService(string convertedDatetime)
        {
            var convertedValue = DateTime.ParseExact(convertedDatetime, "dd.MM.yyyy", null).ToString("yyyy-MM-dd");
            return convertedValue;
        }
        public string ConvertDatetimeByExpiryDate(string convertedDatetime)
        {
            var convertedValue = DateTime.ParseExact(convertedDatetime, "dd.MM.yyyy", null).AddYears(-10).ToString("yyyy-MM-dd");
            return convertedValue;
        }
        private string ConvertDatetimeForFilter(string convertedDateTime)
        {
            DateTime date;
            var convertedValue = DateTime.TryParseExact(convertedDateTime, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            if (convertedValue)
            {
                return date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return null;
        }
        public DateTime? ConvertDate(string convertedDateTime)
        {
            DateTime convertedDate;
            var validDate= DateTime.TryParseExact(convertedDateTime, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDate);
            if (validDate)
            {
                return convertedDate;
            }
            return null;
        }

        public bool DateIsCorrrect(params string[] dateTimes)
        {
            DateTime convertedDate;
            foreach (var date in dateTimes)
            {
                if (!string.IsNullOrEmpty(date))
                {
                    if (!DateTime.TryParseExact(ConvertDatetimeForFilter(date), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDate))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}