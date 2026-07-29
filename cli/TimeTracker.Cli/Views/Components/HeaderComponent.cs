using Spectre.Console;
using Spectre.Console.Rendering;

namespace TimeTracker.Cli.Views.Components;

public class HeaderComponent
{
    public IRenderable Render(TimeSpan TodayTotalAsync)
    {
        var grid = new Grid { Expand = false };
        grid.AddColumn();

        grid.AddRow(
            new Markup("[bold green]⏱  TIME TRACKER[/]")
        );
        return grid;
    }
}