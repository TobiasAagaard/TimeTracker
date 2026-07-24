
namespace TimeTracker.Cli.Views;

public sealed class TimerView
{
    private static readonly string[] spinnerFrames  = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
    

    public async Task RunAsync()
    {
        Console.Clear();
        Console.WriteLine("⏱  TimeTracker");
        Console.WriteLine("Type a task name and press Enter to start tracking");
    }
}