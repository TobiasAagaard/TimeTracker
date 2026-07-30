using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace TimeTracker.Cli.Views;

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
