namespace TimeTracker.Cli.Configuration;

public sealed class KeyBindingsOptions
{
    public string NewTask { get; set; } = string.Empty;
    public string StartStopTimer { get; set; } = string.Empty;
    public string FocusTimer { get; set; } = string.Empty;
    public string FocusSummary { get; set; } = string.Empty;
    public string Refresh { get; set; } = string.Empty;
    public string Help { get; set; } = string.Empty;
    public string Quit { get; set; } = string.Empty;
    public string CancelInput { get; set; } = string.Empty;
}