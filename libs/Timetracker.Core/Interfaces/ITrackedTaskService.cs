using Timetracker.Core.Models;

namespace Timetracker.Core.Interfaces;

public interface ITrackedTaskService
{
    Task<TrackedTask?> GetByIdAsync(Guid id);
    Task<TrackedTask?> GetByTitleAsync(string title);
    Task CreateAsync(TrackedTask task);
}
