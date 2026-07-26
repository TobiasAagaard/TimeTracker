using Microsoft.EntityFrameworkCore;
using TimeTracker.Core.Models;
using TimeTracker.Core.Interfaces;
using TimeTracker.Cli.Data;

namespace TimeTracker.Cli.Services;

public sealed class TimeSlotsService : ITimeSlotsServices
{
    private readonly LocalDbContext _dbContext;

    public TimeSlotsService(LocalDbContext dbContext) => _dbContext = dbContext;
    public async Task<TimeSlots?> GetRunningTimeSlotAsync()
    {
        return await _dbContext.TimeSlots.FirstOrDefaultAsync(s => s.EndedAt == null);
    }

    public async Task StartTimeSlotAsync(Guid trackedTaskId)
    {
        var StartedAt = DateTime.UtcNow;
        _dbContext.TimeSlots.Add(new TimeSlots
        {
            TrackedTaskId = trackedTaskId,
            StartedAt = StartedAt,
            UpdatedAt = StartedAt
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task StopTimeSlotAsync()
    {
        var runningTimeSlot = await GetRunningTimeSlotAsync();

        if (runningTimeSlot is null)
        {
            throw new InvalidOperationException("No running time slot found.");
        }

        runningTimeSlot.EndedAt = DateTime.UtcNow;
        runningTimeSlot.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
    
}