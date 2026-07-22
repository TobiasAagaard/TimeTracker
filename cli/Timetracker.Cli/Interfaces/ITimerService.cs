namespace Timetracker.Cli.Interfaces;
public interface ITimerService
{
    Task<RunningTimer?> GetRunningTimerAsync();
    Task StartAsync(string title);
    Task StopAsync();
}