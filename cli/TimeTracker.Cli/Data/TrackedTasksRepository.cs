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
        return await _dbContext.TrackedTasks.Where(t => t.CreatedAt.Date == DateTime.UtcNow.Date).ToListAsync();
    }

    public async Task CreateTaskAsync(TrackedTasks task)
    {
        _dbContext.TrackedTasks.Add(task);
        await _dbContext.SaveChangesAsync();
    }
}