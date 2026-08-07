using TimeTracker.Core.DTOs;
using TimeTracker.Core.Interfaces;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Services;

public class TimerService : ITimerService
{

    private readonly ITrackedTasksRepository _trackedTasksRepository;
    private readonly ITimeSlotsRepository _timeSlotsRepository;
    private readonly TimeProvider _timeProvider;

    public TimerService(ITrackedTasksRepository trackedTasksRepository, ITimeSlotsRepository timeSlotsRepository, TimeProvider timeProvider)
    {
        _trackedTasksRepository = trackedTasksRepository;
        _timeSlotsRepository = timeSlotsRepository;
        _timeProvider = timeProvider;
    }
    
    public async Task<RunningTimer?> GetRunningTimerAsync()
    {
        var runningTimeSlot = await _timeSlotsRepository.GetRunningTimeSlotAsync();
        
        if (runningTimeSlot is null)
        {
            return null;
        }

        var trackedTasks = await _trackedTasksRepository.GetTaskByIdAsync(runningTimeSlot.TrackedTaskId);

        if (trackedTasks is null)
        {
            return null;
        }

        return new RunningTimer
        {
            TaskTitle = trackedTasks.Title,
            StartedAt = runningTimeSlot.StartedAt,
            TimeProvider = _timeProvider
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

    public async Task<IReadOnlyList<DailyTaskSummary>> GetDailyTaskSummariesAsync()
    {
        var tasks = await _trackedTasksRepository.GetAllTasksByTodayAsync();

        return tasks.Select(t => new DailyTaskSummary
        {
            TaskTitle = t.Title,
            TotalTimeSpent = t.TimeSlots.Aggregate(TimeSpan.Zero, (acc, ts) => acc + ((ts.EndedAt ?? DateTime.UtcNow) - ts.StartedAt))
        }).ToList();
    }
}