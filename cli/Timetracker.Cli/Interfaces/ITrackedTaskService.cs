using Timetracker.Cli.Models;

namespace Timetracker.Cli.Interfaces;

public interface ITrackedTaskService
{
    Task<TrackedTask?> GetByIdAsync(Guid id);
    Task<TrackedTask?> GetByTitleAsync(string title);
    Task CreateAsync(TrackedTask task);
}
