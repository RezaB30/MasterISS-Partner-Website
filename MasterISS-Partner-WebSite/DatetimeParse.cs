using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class DatetimeParse
    {
        public string ConvertDatetimeByWebService(string convertedDatetime)
        {
            var convertedValue = DateTime.ParseExact(convertedDatetime, "dd.MM.yyyy HH:mm:ss", null).ToString("yyyy-MM-dd");
            return convertedValue;
        }
        public string ConvertDatetimeByExpiryDate(string convertedDatetime)
        {
            var convertedValue = DateTime.ParseExact(convertedDatetime, "dd.MM.yyyy HH:mm:ss", null).AddYears(-10).ToString("yyyy-MM-dd");
            return convertedValue;
        }
    }
}