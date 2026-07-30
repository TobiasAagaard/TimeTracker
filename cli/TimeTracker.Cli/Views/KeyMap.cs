using Terminal.Gui.Input;

namespace TimeTracker.Cli.Views;

public static class KeyMap
{
    public static readonly Key NewTask = Key.S;
    public static readonly Key StopTimer = Key.Enter;
    public static readonly Key FocusTimer = Key.D1;
    public static readonly Key FocusSummary = Key.D2;
    public static readonly Key Refresh = Key.R;
    public static readonly Key Help = (Key)'?';
    public static readonly Key Quit = Key.Q;
}
