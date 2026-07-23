namespace Timetracker.Core.Interfaces;

/// <summary>
/// Snapshot of the currently running timer, returned by <see cref="ITimerService"/>.
/// </summary>
public sealed class RunningTimer
{
    public string TaskTitle { get; set; } = string.Empty;
    public DateTime StartedAtUtc { get; set; }

    public TimeSpan Elapsed => DateTime.UtcNow - StartedAtUtc;
}
