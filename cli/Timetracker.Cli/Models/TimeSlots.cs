
namespace Timetracker.Cli.Models;

public class TimeSlots
{

    public TimeSlots(int id, string Title, DateTime StartTime, DateTime EndTime)
    {
        Id = id;
        this.Title = Title;
        this.StartTime = StartTime;
        this.EndTime = EndTime;
    }
    
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }


}