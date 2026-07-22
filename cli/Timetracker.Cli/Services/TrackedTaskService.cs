using Microsoft.EntityFrameworkCore;
using Timetracker.Cli.Data;
using Timetracker.Cli.Interfaces;
using Timetracker.Cli.Models;

namespace Timetracker.Cli.Services;

public sealed class TrackedTaskService : ITrackedTaskService
{
    private readonly TimeTrackerDbContext _db;

    public TrackedTaskService(TimeTrackerDbContext db) => _db = db;

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
