using Timetracker.Core.Models;

namespace Timetracker.Core.Interfaces;

public interface ITimeSlotService
{
    Task<TimeSlot?> GetRunningTimeSlotAsync();
    Task StartAsync(Guid trackedTaskId);
    Task StopAsync();
}
