using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace TimeTracker.Cli.Views;

public static class Theme
{
    public static readonly Color Accent = new(ColorName16.BrightCyan);
    public static readonly Color Muted = new(ColorName16.DarkGray);
    public static readonly Color Active = new(ColorName16.BrightGreen);

    public static void StylePane(View pane, bool focused)
    {
        pane.BorderStyle = LineStyle.Rounded;
        pane.Border?.GetOrCreateView().SetScheme(new Scheme(Attr(pane, focused ? Accent : Muted)));
    }

    public static void StyleMuted(View view) => view.SetScheme(new Scheme(Attr(view, Muted)));

    public static void StyleActive(View view, bool bold = false)
    {
        view.SetScheme(new Scheme(Attr(view, Active, bold ? TextStyle.Bold : TextStyle.None)));
    }

    private static Attribute Attr(View view, Color foreground, TextStyle style = TextStyle.None)
    {
        return new Attribute(foreground, view.GetScheme().Normal.Background, style);
    }
}
