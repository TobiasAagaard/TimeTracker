namespace TimeTracker.Core.DTOs;

public sealed record RunningTimer
{
    public string TaskTitle { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    public TimeSpan ElapsedTime => TimeProvider.GetUtcNow().UtcDateTime - StartedAt;
}