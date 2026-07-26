using Microsoft.EntityFrameworkCore;
using TimeTracker.Core.Models;
using TimeTracker.Core.Interfaces;
using TimeTracker.Cli.Data;

namespace TimeTracker.Cli.Services;

public sealed class TrackedTasksService : ITrackedTasks
{
    private readonly LocalDbContext _dbContext;

    public TrackedTasksService(LocalDbContext dbContext) => _dbContext = dbContext;

    public async Task<TrackedTasks?> GetTaskByIdAsync(Guid taskId)
    {
        return await _dbContext.TrackedTasks.FirstOrDefaultAsync(t => t.Id == taskId);
    }
    public async Task<List<TrackedTasks>> GetAllTasksByTodayAsync()
    {
        return await _dbContext.TrackedTasks.Where(t => t.CreatedAt.Date == DateTime.UtcNow.Date).ToListAsync();
    }

    public async Task CreateTaskAsync(TrackedTasks tasks)
    {
        _dbContext.TrackedTasks.Add(tasks);
        await _dbContext.SaveChangesAsync();
    }
}