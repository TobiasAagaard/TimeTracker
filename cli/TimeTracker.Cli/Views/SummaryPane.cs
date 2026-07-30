using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TimeTracker.Cli.utils;
using TimeTracker.Core.DTOs;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace TimeTracker.Cli.Views;

public sealed class SummaryPane : Pane
{
    private readonly TableView _table;
    private readonly Label _total;

    public SummaryPane() : base("today", KeyMap.FocusSummary)
    {
        _table = new TableView
        {
            X = 1,
            Y = 0,
            Width = Dim.Fill(1),
            Height = Dim.Fill(1),
            FullRowSelect = true,
        };

        _table.Style.ShowHorizontalHeaderOverline = false;
        _table.Style.ShowHorizontalHeaderUnderline = false;
        _table.Style.ShowHorizontalBottomLine = false;
        _table.Style.ShowVerticalCellLines = false;
        _table.Style.ShowVerticalHeaderLines = false;
        _table.Style.ExpandLastColumn = false;

        _total = new Label { X = 1, Y = Pos.AnchorEnd(), Width = Dim.Fill(1) };

        Initialized += (_, _) =>
        {
            _table.Style.HeaderScheme = new Scheme(new Attribute(Theme.Muted, GetScheme().Normal.Background));
            Theme.StyleMuted(_total);
        };

        Add(_table, _total);
        SetSummaries(Array.Empty<DailyTaskSummary>());
    }

    public void SetSummaries(IReadOnlyList<DailyTaskSummary> summaries)
    {
        var rows = summaries.OrderByDescending(s => s.TotalTimeSpent).ToList();

        _table.Table = new EnumerableTableSource<DailyTaskSummary>(rows, new Dictionary<string, Func<DailyTaskSummary, object>>
        {
            ["task"] = s => s.TaskTitle,
            ["elapsed"] = s => TimeFormat.Hms(s.TotalTimeSpent),
            ["hours"] = s => TimeFormat.DecimalHours(s.TotalTimeSpent),
        });

        var total = rows.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.TotalTimeSpent);
        var taskLabel = rows.Count == 1 ? "task" : "tasks";

        _total.Text = rows.Count == 0
            ? "nothing tracked yet today"
            : $"{rows.Count} {taskLabel} · {TimeFormat.Hms(total)} · {TimeFormat.DecimalHours(total)} h";
    }

}
