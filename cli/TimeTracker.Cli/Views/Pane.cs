using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace TimeTracker.Cli.Views;

/// <summary>
/// A bordered region that highlights itself while focused and advertises the key that focuses it.
/// Panes are focus targets in their own right so the whole UI is reachable from the keyboard.
/// </summary>
public abstract class Pane : FrameView
{
    protected Pane(string title, Key focusKey)
    {
        Title = $" {focusKey} {title} ";
        CanFocus = true;

        Initialized += (_, _) => Restyle();
        HasFocusChanged += (_, _) => Restyle();
    }

    private void Restyle() => Theme.StylePane(this, HasFocus);
}
