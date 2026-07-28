using System.Text;
using TimeTracker.Core.Interfaces;
namespace TimeTracker.Cli.Views;

public class TimerView
{
    private static readonly string[] spinner  = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
    private readonly ITimerService _timerService;
    private volatile bool _isRunning;

    public enum TrackAction
    {
        None,
        Stop,
    }

    public TimerView(ITimerService timerService, ITrackedTasksRepository trackedTasksRepository)
    {
        _timerService = timerService;
    }
    public async Task RunAsync()
    {
        Console.Clear();
        Console.WriteLine("⏱  TimeTracker");
        Console.WriteLine("Type a task name and press Enter to start tracking");
        
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
           if (_isRunning)
            {
                return;
            }
            e.Cancel = true;
            cts.Cancel();
        };

        while (!_isRunning)
        {
            Console.Write("Task> ");
            var title = await ReadTitleAsync();

            if (title is null)
            {
                break;
            }

            try
            {
                await _timerService.StartTimerAsync(title);
                _isRunning = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start timer for task '{title}': {ex}");
                Console.WriteLine();
                continue;
            }

            await DisplayTimerAsync(title.Trim(), cts.Token);
        }
        Console.WriteLine("Exiting TimeTracker.Cli");
    }

    private async Task<TrackAction> DisplayTimerAsync(string title, CancellationToken cancellationToken)
    {
        var running = await _timerService.GetRunningTimerAsync();
        var startedAt = running?.StartedAt ?? DateTime.UtcNow;

        Console.WriteLine($"Started tracking task: {title} ");
        Console.WriteLine("Press Enter to stop tracking.");
        _isRunning = true;

        var action = TrackAction.None;
        var frame = 0;

        while (action == TrackAction.None && !cancellationToken.IsCancellationRequested)
        {
            var elapsed = DateTime.UtcNow - startedAt;
            Console.Write($"\r  {spinner[frame]}  {FormatTimeSpan(elapsed)}   ");
            frame = (frame + 1) % spinner.Length;

            action = ReadAction();

            try
            {
                await Task.Delay(200, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        if (action == TrackAction.Stop)
        {
            try
            {
                await _timerService.StopTimerAsync();
                Console.Clear();
               
                var summaries = await _timerService.GetDailyTaskSummariesAsync();

                Console.WriteLine("Daily Task Summaries:");
                Console.WriteLine();
                foreach (var summary in summaries)
                {
                    Console.WriteLine($"Task: {summary.TaskTitle}, Total Time Spent {FormatTimeSpan(summary.TotalTimeSpent)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to stop timer for task '{title}': {ex.Message}");
            }
            Console.WriteLine();
        }

        _isRunning = false;
        return action;
    }

    private static TrackAction ReadAction()
    {
        if (Console.IsInputRedirected || !Console.KeyAvailable)
        {
            return TrackAction.None;
        }

        var key = Console.ReadKey(intercept: true);
        return key.Key switch
        {
            ConsoleKey.Enter => TrackAction.Stop,
            _ => TrackAction.None,
        };
    }

    private static async Task<string?> ReadTitleAsync()
    {
        if (Console.IsInputRedirected)
        {
            return await Task.Run(() => Console.ReadLine());
        }
        var title = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    return null;

                case ConsoleKey.Enter:
                    Console.WriteLine();
                    return title.ToString();
                
                case ConsoleKey.Backspace:
                    if (title.Length > 0)
                    {
                        title.Length--;
                        Console.Write("\b \b");
                    }
                    break;
                
                default:
                    if (!char.IsControl(key.KeyChar))
                    {
                        title.Append(key.KeyChar);
                        Console.Write(key.KeyChar);
                    }
                    break;
            }
        }
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}