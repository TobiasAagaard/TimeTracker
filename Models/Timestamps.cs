using System.Globalization;

namespace TimeTracker.Models;

public class Timestamps
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public TimeSpan Duration { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Color { get; set; } = "#1F6FEB";

    public DateTime End => Start + Duration;
    public DateTime Date => Start.Date;
    public bool IsAllDay => Start.TimeOfDay == TimeSpan.Zero
                            && Duration > TimeSpan.Zero
                            && Duration.TotalSeconds % TimeSpan.FromDays(1).TotalSeconds == 0;
    public bool SpansMultipleDays => Start.Date != End.Date;

    public TimeSpan ClipTo(DateTime windowStart, DateTime windowEnd)
    {
        var s = Start > windowStart ? Start : windowStart;
        var e = End < windowEnd ? End : windowEnd;
        return e > s ? e - s : TimeSpan.Zero;
    }

    public static Timestamps FromStartEnd(string name, DateTime start, DateTime end, string project = "", string color = "#1F6FEB")
    {
        if (end <= start)
            throw new ArgumentException("End must be after start.", nameof(end));

        return new Timestamps
        {
            Name = name,
            Start = start,
            Duration = end - start,
            Project = project,
            Color = color
        };
    }

    public override string ToString() =>
        $"{Start.ToString("g", CultureInfo.CurrentCulture)} ({Duration:hh\\:mm})  {Name}";
}