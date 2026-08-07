using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using TimeTracker.Core.Interfaces;
using TimeTracker.Core.Models;
using TimeTracker.Core.Services;


namespace TimeTracker.Core.Tests.Services;

public class TimerServiceTests
{
    private static readonly DateTimeOffset _fixedTime = new(2026, 3, 14, 10, 30, 0, TimeSpan.Zero);
    private static readonly DateTime _fixedTimeUtc = _fixedTime.UtcDateTime;

    private readonly ITrackedTasksRepository _trackedTasks = Substitute.For<ITrackedTasksRepository>();
    private readonly ITimeSlotsRepository _timeSlots = Substitute.For<ITimeSlotsRepository>();
    private readonly FakeTimeProvider _clock = new(_fixedTime);

    private TimerService CreateTimerService()
    {
        return new TimerService(_trackedTasks, _timeSlots, _clock);
    }

    private static TimeSlots CreateTimeSlot(DateTime startedAt, DateTime? endedAt = null, Guid? trackedTaskId = null)
    {
        return new TimeSlots
        {
            StartedAt = startedAt,
            EndedAt = endedAt,
            TrackedTaskId = trackedTaskId ?? Guid.NewGuid()
        };
    }

    private static TrackedTasks CreateTrackedTask(string title, params TimeSlots[] timeSlots)
    {
        return new TrackedTasks
        {
            Title = title,
            TimeSlots = timeSlots.ToList(),
            CreatedAt = _fixedTimeUtc.AddDays(-1)
        };
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task StartTimerAsync_ThrowsForBlankTitle(string? title)
    {
        // Arrange
        _trackedTasks.GetTasksByTitleAsync(Arg.Any<string>()).Returns(Task.FromResult<TrackedTasks?>(null));
        // Act
        TimerService timerService = CreateTimerService();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => timerService.StartTimerAsync(title!));
    }
}