using Microsoft.EntityFrameworkCore;
using TimeTracker.Core.Models;
using TimeTracker.Core.Interfaces;

namespace TimeTracker.Cli.Data;

public sealed class TrackedTasksRepository : ITrackedTasksRepository
{
    private readonly LocalDbContext _dbContext;

    public TrackedTasksRepository(LocalDbContext dbContext) => _dbContext = dbContext;

    public async Task<TrackedTasks?> GetTaskByIdAsync(Guid taskId)
    {
        return await _dbContext.TrackedTasks.FirstOrDefaultAsync(t => t.Id == taskId);
    }
    public async Task<List<TrackedTasks>> GetAllTasksByTodayAsync()
    {
        var todayStart = DateTime.UtcNow.Date;
        var tomorrowStart = todayStart.AddDays(1);

        return await _dbContext.TrackedTasks
            .AsNoTracking()
            .Where(t => t.TimeSlots.Any(s => s.StartedAt >= todayStart && s.StartedAt < tomorrowStart))
            .Include(t => t.TimeSlots.Where(s => s.StartedAt >= todayStart && s.StartedAt < tomorrowStart))
            .ToListAsync();
    }

    public async Task<TrackedTasks?> GetTasksByTitleAsync(string title)
    {
        return await _dbContext.TrackedTasks.FirstOrDefaultAsync(t => t.Title == title);
    }

    public async Task CreateTaskAsync(TrackedTasks task)
    {
        _dbContext.TrackedTasks.Add(task);
        await _dbContext.SaveChangesAsync();
    }
}