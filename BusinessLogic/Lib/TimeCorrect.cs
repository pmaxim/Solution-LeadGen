using System;

namespace BusinessLogic.Lib
{
    public static class TimeCorrect
    {
        public static string ToDateTimeString(this DateTime? time)
        {
            if (time == null) return string.Empty;
            var dt = (DateTime)time;
            return $"{dt.ToShortDateString()} {dt.ToShortTimeString()}";
        }

        public static string ToDateString(this DateTime? time)
        {
            if (time == null) return string.Empty;
            var dt = (DateTime)time;
            return $"{dt.ToShortDateString()}";
        }

        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date.AddTicks(1);
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }

        public static string ToTimeStringHtml2Line(this DateTime time)
        {
            return $"{time.ToShortDateString()}<br>{time.ToLongTimeString()}";
        }

        public static string ConvertDate(string date)
        {
            if (string.IsNullOrEmpty(date)) return string.Empty;
            var sp = date.Split("-");
            if (sp.Length < 3) return date;
            return $"{sp[1]}/{sp[2]}/{sp[0]}";
        }
    }
}
