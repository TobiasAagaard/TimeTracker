
namespace TimeTracker.Core.DTOs;

public sealed record DailyTaskSummary
{
    public string TaskTitle { get; set; } = string.Empty;
    public TimeSpan TotalTimeSpent { get; set; }
}