using Microsoft.EntityFrameworkCore;
using Timetracker.Cli.Data;
using Timetracker.Cli.Interfaces;
using Timetracker.Cli.Models;

namespace Timetracker.Cli.Services;

public sealed class TimeSlotService : ITimeSlotService
{
    private readonly TimeTrackerDbContext _db;

    public TimeSlotService(TimeTrackerDbContext db) => _db = db;

    public Task<TimeSlot?> GetRunningTimeSlotAsync() =>
        _db.TimeSlots.FirstOrDefaultAsync(s => s.EndedAtUtc == null);

    public async Task StartAsync(Guid trackedTaskId)
    {
        // Only one timer runs at a time — close any open slot before opening a new one.
        await StopAsync();

        _db.TimeSlots.Add(new TimeSlot
        {
            TrackedTaskId = trackedTaskId,
            StartedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        });
        await _db.SaveChangesAsync();
    }

    public async Task StopAsync()
    {
        var running = await GetRunningTimeSlotAsync();
        if (running is null)
            return;

        running.EndedAtUtc = DateTime.UtcNow;
        running.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
