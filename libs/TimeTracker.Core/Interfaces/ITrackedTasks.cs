using TimeTracker.Core.Models;

namespace TimeTracker.Core.Interfaces;

public interface ITrackedTasks
{
    Task<TrackedTasks?> GetTaskByIdAsync(Guid taskId);
    Task<List<TrackedTasks>> GetAllTasksByTodayAsync();
    Task CreateTaskAsync(TrackedTasks task);

}