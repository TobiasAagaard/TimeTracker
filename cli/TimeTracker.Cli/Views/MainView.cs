using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;
using TimeTracker.Cli.Views.Components;
using TimeTracker.Core.Interfaces;

public class MainView
{
    private readonly ITimerService _timerService;

    private readonly HeaderComponent _header = new();

    public MainView(ITimerService timerService) => _timerService = timerService;

    public async Task Render()
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        AnsiConsole.Clear();
        var rendered = await Compose();
        AnsiConsole.Write(rendered);

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
        }   
        catch (TaskCanceledException)
        {
        }
    }

    // Compose the 4 components into one screen for a given step/body.
    private async Task<IRenderable> Compose() => new Rows(
        _header.Render(await TodayTotalAsync())
    );

    private async Task<TimeSpan> TodayTotalAsync()
    {
        var summaries = await _timerService.GetDailyTaskSummariesAsync();
        return summaries.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.TotalTimeSpent);
    }
}