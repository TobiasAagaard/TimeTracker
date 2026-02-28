using System;
using System.IO;
using Moq;
using TimeTracker.Services;
using TimeTracker.UI;
using Xunit;

namespace TimeTracker.Tests;

public class MenusTests
{
    [Fact]
    public void ShowBanner_ExecutesWithoutException()
    {
        // Arrange
        using var sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        Menus.ShowBanner();

        // Assert
        var output = sw.ToString();
        Assert.NotEmpty(output);
        Assert.Contains("TIME", output);
        Assert.Contains("TRACKER", output);
    }

    [Fact]
    public void ShowBanner_OutputsASCIIArt()
    {
        // Arrange
        using var sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        Menus.ShowBanner();

        // Assert
        var output = sw.ToString();
        Assert.Contains("████", output);
        Assert.Contains("╚", output);
    }

    [Fact]
    public void TimeTrackingMenu_Constructor_ThrowsArgumentNullException_WhenServiceIsNull()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Menus.TimeTrackingMenu(null!));
        Assert.Equal("service", exception.ParamName);
    }

    [Fact]
    public void TimeTrackingMenu_Constructor_InitializesSuccessfully_WithValidService()
    {
        // Arrange
        var service = new TimeTrackingService();

        // Act
        var menu = new Menus.TimeTrackingMenu(service);

        // Assert
        Assert.NotNull(menu);
    }

    [Fact]
    public void TimeTrackingMenu_Constructor_AcceptsMockedService()
    {
        // Arrange
        var mockService = new Mock<TimeTrackingService>();

        // Act
        var menu = new Menus.TimeTrackingMenu(mockService.Object);

        // Assert
        Assert.NotNull(menu);
    }

    [Fact]
    public void TimeTrackingMenu_Run_CallsServiceStart()
    {
        // Arrange
        var mockService = new Mock<TimeTrackingService>();
        mockService.Setup(s => s.IsRunning()).Returns(false); // Exit immediately
        var menu = new Menus.TimeTrackingMenu(mockService.Object);

        // Redirect console input to simulate pressing Enter then nothing
        using var inputReader = new StringReader("\n");
        Console.SetIn(inputReader);
        using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        // Act
        menu.Run();

        // Assert
        mockService.Verify(s => s.Start(), Times.Once);
    }

    [Fact]
    public void TimeTrackingMenu_Run_CallsIsRunning()
    {
        // Arrange
        var mockService = new Mock<TimeTrackingService>();
        mockService.Setup(s => s.IsRunning()).Returns(false); // Exit immediately
        var menu = new Menus.TimeTrackingMenu(mockService.Object);

        using var inputReader = new StringReader("\n");
        Console.SetIn(inputReader);
        using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        // Act
        menu.Run();

        // Assert
        mockService.Verify(s => s.IsRunning(), Times.AtLeastOnce);
    }

    [Fact]
    public void TimeTrackingMenu_Run_CallsGetTimeSpan_AfterStopping()
    {
        // Arrange
        var mockService = new Mock<TimeTrackingService>();
        mockService.Setup(s => s.IsRunning()).Returns(false);
        mockService.Setup(s => s.GetTimeSpan()).Returns(TimeSpan.FromHours(1.5));
        var menu = new Menus.TimeTrackingMenu(mockService.Object);

        using var inputReader = new StringReader("\n");
        Console.SetIn(inputReader);
        using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        // Act
        menu.Run();

        // Assert
        mockService.Verify(s => s.GetTimeSpan(), Times.Once);
    }

    [Fact]
    public void TimeTrackingMenu_Run_DisplaysTrackingStartedMessage()
    {
        // Arrange
        var mockService = new Mock<TimeTrackingService>();
        mockService.Setup(s => s.IsRunning()).Returns(false);
        var menu = new Menus.TimeTrackingMenu(mockService.Object);

        using var inputReader = new StringReader("\n");
        Console.SetIn(inputReader);
        using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        // Act
        menu.Run();

        // Assert
        var output = outputWriter.ToString();
        Assert.Contains("Tracking started", output);
    }

    [Fact]
    public void TimeTrackingMenu_Run_DisplaysTrackingStoppedMessage()
    {
        // Arrange
        var mockService = new Mock<TimeTrackingService>();
        mockService.Setup(s => s.IsRunning()).Returns(false);
        mockService.Setup(s => s.GetTimeSpan()).Returns(TimeSpan.FromHours(2.5));
        var menu = new Menus.TimeTrackingMenu(mockService.Object);

        using var inputReader = new StringReader("\n");
        Console.SetIn(inputReader);
        using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        // Act
        menu.Run();

        // Assert
        var output = outputWriter.ToString();
        Assert.Contains("Tracking stopped", output);
        Assert.Contains("2.50 hours", output);
    }

    [Fact]
    public void TimeTrackingMenu_Run_DisplaysPromptToStart()
    {
        // Arrange
        var mockService = new Mock<TimeTrackingService>();
        mockService.Setup(s => s.IsRunning()).Returns(false);
        var menu = new Menus.TimeTrackingMenu(mockService.Object);

        using var inputReader = new StringReader("\n");
        Console.SetIn(inputReader);
        using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        // Act
        menu.Run();

        // Assert
        var output = outputWriter.ToString();
        Assert.Contains("Press ENTER to start tracking", output);
    }
}