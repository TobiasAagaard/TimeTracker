using Timetracker.Core.Interfaces;

namespace Timetracker.Cli.Views;

/// <summary>
/// The interactive app view. Runs a loop: prompt for a task name, start the
/// timer, render an animated ticking clock until the user stops it, then return
/// to the prompt. Empty input (or Ctrl+C) exits.
/// </summary>
public sealed class TimerView
{
    private static readonly string[] Spinner =
        { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
    private static readonly TimeSpan FrameInterval = TimeSpan.FromMilliseconds(100);

    private readonly ITimerService _timerService;

    public TimerView(ITimerService timerService) => _timerService = timerService;

    public async Task RunAsync()
    {
        // Ctrl+C ends the session gracefully (stopping any running timer)
        // instead of hard-killing the process.
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        Console.WriteLine("⏱  TimeTracker");
        Console.WriteLine("Type a task name and press Enter to start tracking.");
        Console.WriteLine("Leave it empty and press Enter to quit.");
        Console.WriteLine();

        while (!cts.IsCancellationRequested)
        {
            Console.Write("task> ");
            var title = Console.ReadLine();

            // Empty input or end-of-stream (Ctrl+D / Ctrl+C) ends the session.
            if (string.IsNullOrWhiteSpace(title))
                break;

            await _timerService.StartAsync(title.Trim());
            await TrackAsync(title.Trim(), cts.Token);
        }

        Console.WriteLine("Goodbye.");
    }

    private async Task TrackAsync(string title, CancellationToken cancellationToken)
    {
        var running = await _timerService.GetRunningTimerAsync();
        var startedAtUtc = running?.StartedAtUtc ?? DateTime.UtcNow;

        Console.WriteLine($"Tracking \"{title}\" — press Enter to stop.");
        SetCursorVisible(false);

        var frame = 0;
        try
        {
            while (!cancellationToken.IsCancellationRequested && !StopRequested())
            {
                var elapsed = DateTime.UtcNow - startedAtUtc;
                Console.Write($"\r  {Spinner[frame]}  {Format(elapsed)}   ");
                frame = (frame + 1) % Spinner.Length;

                try
                {
                    await Task.Delay(FrameInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
        finally
        {
            SetCursorVisible(true);
        }

        await _timerService.StopAsync();
        var total = DateTime.UtcNow - startedAtUtc;
        Console.WriteLine($"\r  ✓  {Format(total)}   (stopped)   ");
        Console.WriteLine();
    }

    /// <summary>Returns true once the user presses Enter, Esc, or Q to stop.</summary>
    private static bool StopRequested()
    {
        if (Console.IsInputRedirected || !Console.KeyAvailable)
            return false;

        var key = Console.ReadKey(intercept: true).Key;
        return key is ConsoleKey.Enter or ConsoleKey.Escape or ConsoleKey.Q;
    }

    private static void SetCursorVisible(bool visible)
    {
        // Cursor control is unavailable when the console is redirected (e.g. tests).
        try { Console.CursorVisible = visible; }
        catch (IOException) { }
    }

    private static string Format(TimeSpan elapsed) =>
        $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
}
