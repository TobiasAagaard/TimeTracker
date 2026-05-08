namespace TimeTracker.Models;

public class Calendar
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;

    public List<Timestamps> Entries { get; set; } = [];

    public Timestamps AddEntry(string name, DateTime start, DateTime end, string project = "", string color = "#1F6FEB")
    {
        var entry = Timestamps.FromStartEnd(name, start, end, project, color);
        entry.Id = Entries.Count == 0 ? 1 : Entries.Max(e => e.Id) + 1;
        Entries.Add(entry);
        return entry;
    }

    public Timestamps AddEntry(Timestamps entry)
    {
        entry.Id = Entries.Count == 0 ? 1 : Entries.Max(e => e.Id) + 1;
        Entries.Add(entry);
        return entry;
    }

    public bool RemoveEntry(int entryId) => Entries.RemoveAll(e => e.Id == entryId) > 0;

    public IEnumerable<Timestamps> GetEntriesForDay(DateTime day) =>
        Entries.Where(e => e.Start.Date <= day.Date && e.End.Date >= day.Date)
               .OrderBy(e => e.Start);

    public IEnumerable<Timestamps> GetEntriesForWeek(DateTime anyDayInWeek, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        var start = StartOfWeek(anyDayInWeek, firstDayOfWeek);
        var end = start.AddDays(7);
        return Entries.Where(e => e.Start < end && e.End > start)
                      .OrderBy(e => e.Start);
    }

    public IEnumerable<Timestamps> GetEntriesForMonth(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);
        return Entries.Where(e => e.Start < end && e.End > start)
                      .OrderBy(e => e.Start);
    }

    public TimeSpan TotalDurationForDay(DateTime day) =>
        GetEntriesForDay(day).Aggregate(TimeSpan.Zero, (acc, e) => acc + e.ClipTo(day.Date, day.Date.AddDays(1)));

    private static DateTime StartOfWeek(DateTime date, DayOfWeek firstDay)
    {
        int diff = (7 + (date.DayOfWeek - firstDay)) % 7;
        return date.Date.AddDays(-diff);
    }
}