
using TimeTracker.Models;

namespace TimeTracker;
internal class Program
    {
    static void Main(string[] args)
    {
        var calendar = new Calendar { Name = "My Calendar" };
        calendar.AddEntry("Morning Work", DateTime.Today.AddHours(9), DateTime.Today.AddHours(12), "Work", "#1F6FEB");
        calendar.AddEntry("Lunch Break", DateTime.Today.AddHours(12), DateTime.Today.AddHours(13), "Personal", "#FF5733");
        calendar.AddEntry("Afternoon Work", DateTime.Today.AddHours(13), DateTime.Today.AddHours(17), "Work", "#1F6FEB");

        Console.WriteLine($"Entries for {DateTime.Today:d}:");
        foreach (var entry in calendar.GetEntriesForDay(DateTime.Today))
        {
            Console.WriteLine(entry);
        }
    }
}

 