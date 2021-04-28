using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public static class DateTimeValidation
    {
        public static DateTime? TryParseDate(int year, int month, int day, Calendar calendar)
        {
            if (calendar == null)
                calendar = new GregorianCalendar();

            if (year < calendar.MinSupportedDateTime.Year)
                return null;

            if (year > calendar.MaxSupportedDateTime.Year)
                return null;

            if (month < 1 || month > calendar.GetMonthsInYear(year))
                return null;

            if (day <= 0 || day > DateTime.DaysInMonth(year, month))
                return null;

            try
            {
                return new DateTime(year, month, day, calendar);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }
}