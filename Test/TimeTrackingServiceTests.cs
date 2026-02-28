using System;
using System.Threading;
using TimeTracker.Services;
using Xunit;

namespace TimeTracker.Tests;

public class TimeTrackingServiceTests
{
    [Fact]
    public void Start_ThenStop_TracksPositiveElapsedTime()
    {
        var service = new TimeTrackingService();

        service.Start();
        Thread.Sleep(20);
        service.Stop();

        Assert.False(service.IsRunning());
        Assert.True(service.GetTimeSpan() > TimeSpan.Zero);
        Assert.True(service.GetDecimalHours() > 0);
    }

    [Fact]
    public void Reset_ClearsElapsedTime()
    {
        var service = new TimeTrackingService();

        service.Start();
        Thread.Sleep(20);
        service.Stop();
        service.Reset();

        Assert.Equal(TimeSpan.Zero, service.GetTimeSpan());
        Assert.Equal(0, service.GetDecimalHours());
        Assert.False(service.IsRunning());
    }
}
