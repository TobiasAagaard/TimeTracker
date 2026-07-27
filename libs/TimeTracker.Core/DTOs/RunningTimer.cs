namespace TimeTracker.Core.DTOs;

public sealed record RunningTimer
{
    public string TaskTitle { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public TimeSpan ElapsedTime => DateTime.UtcNow - StartedAt;
}