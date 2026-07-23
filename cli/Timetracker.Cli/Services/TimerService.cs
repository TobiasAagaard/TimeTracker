using Timetracker.Core.Interfaces;
using Timetracker.Core.Models;

namespace Timetracker.Cli.Services;

public class TimerService : ITimerService 
{
    private readonly ITrackedTaskService _trackedTaskService;
    private readonly ITimeSlotService _timeSlotService;

    public TimerService(ITrackedTaskService trackedTaskService, ITimeSlotService timeSlotService)
    {
        _trackedTaskService = trackedTaskService;
        _timeSlotService = timeSlotService;
    }

    public async Task<RunningTimer?> GetRunningTimerAsync()
    {
        var runningTimeSlot = await _timeSlotService.GetRunningTimeSlotAsync();
        if (runningTimeSlot is null)
            return null;

        var task = await _trackedTaskService.GetByIdAsync(runningTimeSlot.TrackedTaskId);
        if (task is null)
            return null;

        return new RunningTimer
        {
            TaskTitle = task.Title,
            StartedAtUtc = runningTimeSlot.StartedAtUtc
        };
    }

    public async Task StartAsync(string title)
    {
        var task = await _trackedTaskService.GetByTitleAsync(title);
        if (task is null)
        {
            task = new TrackedTask { Title = title };
            await _trackedTaskService.CreateAsync(task);
        }

        await _timeSlotService.StartAsync(task.Id);
    }

    public async Task StopAsync()
    {
        await _timeSlotService.StopAsync();
    }
}