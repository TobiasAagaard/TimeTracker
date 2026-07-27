using TimeTracker.Core.DTOs;
using TimeTracker.Core.Interfaces;
using TimeTracker.Core.Models;
namespace TimeTracker.Cli.Services;

public class TimerService : ITimerService
{

    private readonly ITrackedTasksRepository _trackedTasksRepository;
    private readonly ITimeSlotsRepository _timeSlotsRepository;

    public TimerService(ITrackedTasksRepository trackedTasksRepository, ITimeSlotsRepository timeSlotsRepository)
    {
        _trackedTasksRepository = trackedTasksRepository;
        _timeSlotsRepository = timeSlotsRepository;
    }
    
    public async Task<RunningTimer?> GetRunningTimerAsync()
    {
        var runningTimeSlot = await _timeSlotsRepository.GetRunningTimeSlotAsync();
        
        if (runningTimeSlot is null)
        {
            throw new InvalidOperationException("No running time slot found.");
        }

        var trackedTasks = await _trackedTasksRepository.GetTaskByIdAsync(runningTimeSlot.TrackedTaskId);

        if (trackedTasks is null)
        {
            throw new InvalidOperationException("Tracked task not found for the running time slot.");
        }

        return new RunningTimer
        {
            TaskTitle = trackedTasks.Title,
            StartedAt = runningTimeSlot.StartedAt
        };
    }

    public async Task StartTimerAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title cannot be null or empty.", nameof(title));
        }

        title = title.Trim();
        var trackedTask = await _trackedTasksRepository.GetTasksByTitleAsync(title);

        if (trackedTask is null)
        {
            trackedTask = new TrackedTasks {Title = title};
            await _trackedTasksRepository.CreateTaskAsync(trackedTask);
        }
        await _timeSlotsRepository.StartTimeSlotAsync(trackedTask.Id);
    }

    public async Task StopTimerAsync()
    {
        await _timeSlotsRepository.StopTimeSlotAsync();
    }
}