using Microsoft.EntityFrameworkCore;
using TimeTracker.Core.Models;
using TimeTracker.Core.Interfaces;

namespace TimeTracker.Cli.Data;

public sealed class TimeSlotsRepository : ITimeSlotsRepository
{
    private readonly LocalDbContext _dbContext;

    public TimeSlotsRepository(LocalDbContext dbContext) => _dbContext = dbContext;
    public async Task<TimeSlots?> GetRunningTimeSlotAsync()
    {
        return await _dbContext.TimeSlots.FirstOrDefaultAsync(s => s.EndedAt == null);
    }

    public async Task StartTimeSlotAsync(Guid trackedTaskId)
    {
        DateTime StartedAt = DateTime.UtcNow;
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
        TimeSlots? runningTimeSlot = await GetRunningTimeSlotAsync();

        if (runningTimeSlot is null)
        {
            throw new InvalidOperationException("No running time slot found.");
        }

        runningTimeSlot.EndedAt = DateTime.UtcNow;
        runningTimeSlot.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
    
}