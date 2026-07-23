namespace Timetracker.Core.Interfaces;
public interface ITimerService
{
    Task<RunningTimer?> GetRunningTimerAsync();
    Task StartAsync(string title);
    Task StopAsync();
}