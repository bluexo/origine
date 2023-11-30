using System;

namespace Origine.Interfaces
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

        public static ulong ToTimestamp(this DateTime dateTime)
        {
            if (dateTime < Origin)
                dateTime = Origin;
            return (ulong)(dateTime - Origin).TotalSeconds;
        }

        public static DateTime FromTimestamp(this DateTime dateTime, ulong timeStamp)
        {
            return dateTime = FromTimestamp(timeStamp);
        }

        public static DateTime FromTimestamp(ulong timeStamp)
        {
            return Origin + TimeSpan.FromSeconds(timeStamp);
        }

        public static string ToBackendDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string ToBackendString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static ulong ToTotalDays(this DateTime dateTime)
        {
            if (dateTime < Origin)
                dateTime = Origin;
            return (ulong)(dateTime - Origin).Days;
        }

        public static (int days, int weeks, int months) DateContinuity(this DateTime now, DateTime registerTime)
        {
            var days = (now.Date - registerTime.Date).Days;

            var wdays = days + (int)registerTime.DayOfWeek + 1;
            var weeks = wdays / 7;
            if (wdays % 7 == 0) --weeks;

            return (days, weeks, (now.Year - registerTime.Year) * 12 + now.Month - registerTime.Month);
        }
    }

    public static class LongExternsions
    {
        public static DateTime ToDateTime(this ulong timeStamp)
        {
            return DateTimeExtensions.Origin + TimeSpan.FromSeconds(timeStamp);
        }

        public static DateTime ToDate(this ulong days)
        {
            return DateTimeExtensions.Origin + TimeSpan.FromDays(days);
        }
    }
}
