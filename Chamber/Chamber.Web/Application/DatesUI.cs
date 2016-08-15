using System;
using Chamber.Utilities;

namespace Chamber.Web.Application
{
    public static class DatesUI
    {
        public static string GetPrettyDate(string date)
        {
            DateTime time;
            if (DateTime.TryParse(date, out time))
            {
                var span = DateTime.UtcNow.Subtract(time);
                var totalDays = (int)span.TotalDays;
                var totalSeconds = (int)span.TotalSeconds;
                if ((totalDays < 0) || (totalDays >= 0x1f))
                {
                    return DateUtils.FormatDateTime(date, "dd MMMM yyyy");
                }
                if (totalDays == 0)
                {
                    if (totalSeconds < 60)
                    {
                        return "Just now";
                    }
                    if (totalSeconds < 120)
                    {
                        return "{0} minutes ago";
                    }
                    if (totalSeconds < 0xe10)
                    {
                        return string.Format("{0} minutes ago", Math.Floor((double)(((double)totalSeconds) / 60.0)));
                    }
                    if (totalSeconds < 0x1c20)
                    {
                        return "1 hour ago";
                    }
                    if (totalSeconds < 0x15180)
                    {
                        return string.Format("{0} hours ago", Math.Floor((double)(((double)totalSeconds) / 3600.0)));
                    }
                }
                if (totalDays == 1)
                {
                    return "yesterday";
                }
                if (totalDays < 7)
                {
                    return string.Format("{0} days ago", totalDays);
                }
                if (totalDays < 0x1f)
                {
                    return string.Format("{0} weeks ago", Math.Ceiling((double)(((double)totalDays) / 7.0)));
                }
            }
            return date;
        }
    }
}