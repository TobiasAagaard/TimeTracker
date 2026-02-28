using System;
using System.IO;
using TimeTracker.Services;
using TimeTracker.UI;
using Xunit;

namespace TimeTracker.Tests;

public class MenusTests
{
    private sealed class FakeTrackingService : ITimeTrackingService
    {
        public int StartCalls { get; private set; }
        public TimeSpan TimeSpanToReturn { get; set; } = TimeSpan.FromHours(2.5);

        public void Start() => StartCalls++;
        public void Stop() { }
        public double GetDecimalHours() => 0;
        public TimeSpan GetTimeSpan() => TimeSpanToReturn;
        public bool IsRunning() => false;
        public void Reset() { }
    }

    [Fact]
    public void ShowBanner_WritesTrackerBanner()
    {
        var originalOut = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);

        Menus.ShowBanner();

        var output = sw.ToString();
        Console.SetOut(originalOut);

        Assert.NotEmpty(output);
        Assert.Contains("TRACKER", output);
    }

    [Fact]
    public void TimeTrackingMenu_Constructor_ThrowsArgumentNullException_WhenServiceIsNull()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Menus.TimeTrackingMenu(null!));
        Assert.Equal("service", exception.ParamName);
    }

    [Fact]
    public void TimeTrackingMenu_Constructor_AcceptsService()
    {
        var menu = new Menus.TimeTrackingMenu(new FakeTrackingService());
        Assert.NotNull(menu);
    }

    [Fact]
    public void TimeTrackingMenu_Run_StartsAndShowsLifecycleMessages()
    {
        var service = new FakeTrackingService();
        var menu = new Menus.TimeTrackingMenu(service);

        var originalIn = Console.In;
        var originalOut = Console.Out;
        using var inputReader = new StringReader("\n");
        Console.SetIn(inputReader);
        using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        menu.Run();

        var output = outputWriter.ToString();
        Console.SetIn(originalIn);
        Console.SetOut(originalOut);

        Assert.Equal(1, service.StartCalls);
        Assert.Contains("Press ENTER to start tracking", output);
        Assert.Contains("Tracking started", output);
        Assert.Contains("Tracking stopped", output);
    }

    [Fact]
    public void TimeTrackingMenu_Run_DisplaysWorkedHours()
    {
        var service = new FakeTrackingService { TimeSpanToReturn = TimeSpan.FromHours(2.5) };
        var menu = new Menus.TimeTrackingMenu(service);

        var originalIn = Console.In;
        var originalOut = Console.Out;
        using var inputReader = new StringReader("\n");
        Console.SetIn(inputReader);
        using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        menu.Run();

        var output = outputWriter.ToString();
        Console.SetIn(originalIn);
        Console.SetOut(originalOut);

        Assert.Contains("hours", output);
        Assert.Contains("2", output);
    }
}