using Timetracker.Cli.Models;

namespace Timetracker.Cli.Interfaces;

public interface ITimeSlotService
{
    Task<TimeSlot?> GetRunningTimeSlotAsync();
    Task StartAsync(Guid trackedTaskId);
    Task StopAsync();
}
