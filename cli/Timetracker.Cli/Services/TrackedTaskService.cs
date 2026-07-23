using Microsoft.EntityFrameworkCore;
using Timetracker.Cli.Data;
using Timetracker.Core.Interfaces;
using Timetracker.Core.Models;

namespace Timetracker.Cli.Services;

public sealed class TrackedTaskService : ITrackedTaskService
{
    private readonly TimeTrackerLocalDbContext _db;

    public TrackedTaskService(TimeTrackerLocalDbContext db) => _db = db;

    public Task<TrackedTask?> GetByIdAsync(Guid id) =>
        _db.TrackedTasks.FirstOrDefaultAsync(t => t.Id == id);

    public Task<TrackedTask?> GetByTitleAsync(string title) =>
        _db.TrackedTasks.FirstOrDefaultAsync(t => t.Title == title);

    public async Task CreateAsync(TrackedTask task)
    {
        _db.TrackedTasks.Add(task);
        await _db.SaveChangesAsync();
    }
}
