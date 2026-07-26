using TimeTracker.Core.Models;

namespace TimeTracker.Core.Interfaces;

public interface ITimeSlotsServices
{
    Task<TimeSlots?> GetRunningTimeSlotAsync();
    Task StartTimeSlotAsync(Guid trackedTaskId);
    Task StopTimeSlotAsync();
}