using System.Text;
using TimeTracker.Core.Interfaces;
namespace TimeTracker.Cli.Views;

public class TimerView
{
    private static readonly string[] spinnerFrames  = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
    private readonly ITimerService _timerService;
    private volatile bool _isRunning;


    public TimerView(ITimerService timerService)
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
                // TODO: Log the exception or handle it appropriately
                Console.WriteLine($"Failed to start timer for task '{title}': {ex.Message}");
                Console.WriteLine();
                continue;
            }

            await DisplayTimerAsync();
        }
        Console.WriteLine("Exiting TimeTracker.Cli");
    }

    private async Task DisplayTimerAsync()
    {
        throw new NotImplementedException("DisplayTimerAsync method is not implemented yet.");
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
}