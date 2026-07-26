using TimeTracker.Core.Models;

namespace TimeTracker.Core.Interfaces;

public interface ITrackedTasks
{
    Task<TrackedTasks?> GetTaskByIdAsync(Guid taskId);
    Task <TrackedTasks?> GetAllTasksAsync();
    Task CreateTaskAsync(TrackedTasks task);

}