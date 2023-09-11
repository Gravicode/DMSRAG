using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DMSRAG.Web.Data
{
    public class DateHelper
    {
        static CultureInfo ci = new CultureInfo("id-ID");
        public static string GetMonthName(int month)
        {
            if (month < 1 || month > 12) return "";
            var monthName = ci.DateTimeFormat.GetMonthName(month);
            return monthName;
        }
        public static string GetDayName(DayOfWeek day)
        {
            var dayName = ci.DateTimeFormat.GetDayName(day);
            return dayName;
        }
        public static DateTime GetLocalTimeNow()
        {
            var localTimeZoneId = "SE Asia Standard Time";
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(localTimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);
        }
        public static string GetTimeLeft(DateTime date)
        {
            var now = GetLocalTimeNow();
            if (now > date)
            {
                return "No time left (expire)";
            }
            else
            {
                var ts = date - now;
                var speak = "";
                if (ts.Days > 0)
                {
                    speak = $"{ts.Days} days";
                }
                if (ts.Hours > 0)
                {
                    speak += string.IsNullOrEmpty(speak) ? $"{ts.Hours} hours" : $", {ts.Hours} hours";
                }

                if (ts.Minutes > 0)
                {
                    speak += string.IsNullOrEmpty(speak) ? $"{ts.Minutes} minutes" : $", {ts.Minutes} minutes";
                }

                if (ts.Seconds > 0)
                {
                    speak += string.IsNullOrEmpty(speak) ? $"{ts.Seconds} seconds" : $", {ts.Seconds} seconds";
                }

                speak += " left";
                return speak;
            }
        }
        public static string GetElapsedTime(DateTime date)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateHelper.GetLocalTimeNow().Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }
}
