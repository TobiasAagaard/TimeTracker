using TimeTracker.Core.Models;

namespace TimeTracker.Core.Interfaces;

public interface ITimeSlotsRepository
{
    Task<TimeSlots?> GetRunningTimeSlotAsync();
    Task StartTimeSlotAsync(Guid trackedTaskId);
    Task StopTimeSlotAsync();
}