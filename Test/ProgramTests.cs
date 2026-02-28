using System;
using System.IO;
using TimeTracker.Data;
using TimeTracker.Services;
using Xunit;

namespace TimeTracker.Tests;

public class ProgramTests
{
    private sealed class FakeService : ITimeTrackingService
    {
        public void Start() { }
        public void Stop() { }
        public double GetDecimalHours() => 0;
        public TimeSpan GetTimeSpan() => TimeSpan.Zero;
        public bool IsRunning() => false;
        public void Reset() { }
    }

    [Fact]
    public void Run_ReturnsOne_AndStops_WhenDatabaseInitializationFails()
    {
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        var bannerCalled = false;
        var menuCalled = false;

        var exitCode = AppRunner.Run(
            databaseFactory: _ => throw new InvalidOperationException("db failed"),
            showBanner: () => bannerCalled = true,
            serviceFactory: () => new FakeService(),
            runMenu: _ => menuCalled = true);

        Console.SetOut(originalOut);

        Assert.Equal(1, exitCode);
        Assert.False(bannerCalled);
        Assert.False(menuCalled);
        Assert.Contains("Error initializing database:", output.ToString());
    }

    [Fact]
    public void Run_ReturnsZero_AndExecutesBannerThenMenu_WhenDatabaseInitializationSucceeds()
    {
        var callOrder = new List<string>();
        var pathPassedToFactory = string.Empty;
        var service = new FakeService();

        var exitCode = AppRunner.Run(
            databaseFactory: path =>
            {
                pathPassedToFactory = path;
                callOrder.Add("database");
                return new Database(":memory:");
            },
            showBanner: () => callOrder.Add("banner"),
            serviceFactory: () =>
            {
                callOrder.Add("service");
                return service;
            },
            runMenu: _ => callOrder.Add("menu"));

        Assert.Equal(0, exitCode);
        Assert.Equal(new[] { "database", "banner", "service", "menu" }, callOrder);
        Assert.EndsWith("TimeTracker.db", pathPassedToFactory);
    }
}