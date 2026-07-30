using System.Globalization;

namespace TimeTracker.Cli.utils;

public static class TimeFormat
{
    public static string Hms(TimeSpan timeSpan)
    {
        return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    public static string DecimalHours(TimeSpan timeSpan)
    {
        return timeSpan.TotalHours.ToString("F2", CultureInfo.InvariantCulture);
    }
}
