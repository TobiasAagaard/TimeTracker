using Spectre.Console;
using Spectre.Console.Rendering;

namespace TimeTracker.Cli.Views.Components;

public class HeaderComponent
{
    public IRenderable Render(TimeSpan TodayTotalAsync)
    {
        var grid = new Grid { Expand = true };
        grid.AddColumn();
        grid.AddColumn();
        grid.Columns[1].RightAligned();

        grid.AddRow(
            new Markup("[bold green]⏱  TIME TRACKER[/]"),
            new Markup($"[bold yellow] Today:{TodayTotalAsync}[/]")
        );
        return grid;
    }
}