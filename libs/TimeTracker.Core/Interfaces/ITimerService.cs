using TimeTracker.Core.DTOs;

namespace TimeTracker.Core.Interfaces;

public interface ITimerService
{
    Task<RunningTimer?> GetRunningTimerAsync();
    Task StartTimerAsync(string title);
    Task StopTimerAsync();
    Task<IReadOnlyList<DailyTaskSummary>> GetDailyTaskSummariesAsync();
}