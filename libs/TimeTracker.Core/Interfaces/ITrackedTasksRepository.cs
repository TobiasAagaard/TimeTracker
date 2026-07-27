using TimeTracker.Core.Models;

namespace TimeTracker.Core.Interfaces;

public interface ITrackedTasksRepository
{
    Task<TrackedTasks?> GetTaskByIdAsync(Guid taskId);
    Task<List<TrackedTasks>> GetAllTasksByTodayAsync();
    Task<TrackedTasks?> GetTasksByTitleAsync(string title);
    Task CreateTaskAsync(TrackedTasks task);


}