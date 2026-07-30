using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TimeTracker.Cli.utils;
using TimeTracker.Core.DTOs;

namespace TimeTracker.Cli.Views;

/// <summary>
/// The timer pane. Shows one of three states: nothing running, entering a task name, or counting up.
/// It renders and collects input only — starting and stopping is decided by <see cref="MainWindow"/>.
/// </summary>
public sealed class TimerPane : Pane
{
    private static readonly string[] SpinnerFrames = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];

    private readonly IApplication _app;
    private readonly Label _marker;
    private readonly Label _title;
    private readonly Label _detail;
    private readonly TextField _input;

    private RunningTimer? _timer;
    private object? _timeoutToken;
    private int _frame;

    /// <summary>Raised when the user confirms a task name with Enter.</summary>
    public event Action<string>? StartRequested;

    /// <summary>Raised when the user abandons the task-name prompt with Esc.</summary>
    public event Action? InputCancelled;

    public TimerPane(IApplication app) : base("timer", KeyMap.FocusTimer)
    {
        _app = app;

        _marker = new Label { X = 1, Y = 1 };
        _title = new Label { X = 4, Y = 1, Width = Dim.Fill(1) };
        _detail = new Label { X = 4, Y = 2, Width = Dim.Fill(1) };

        _input = new TextField
        {
            X = 4,
            Y = 1,
            Width = Dim.Fill(2),
            Visible = false,
            CanFocus = false,
        };
        _input.Accepting += (_, e) =>
        {
            e.Handled = true;
            var title = _input.Text.Trim();
            if (title.Length > 0)
            {
                StartRequested?.Invoke(title);
            }
        };

        Initialized += (_, _) =>
        {
            Theme.StyleMuted(_detail);
            Theme.StyleActive(_marker);
        };

        Add(_marker, _title, _detail, _input);
        ShowIdle();
    }

    public void ShowIdle()
    {
        StopTicking();
        HideInput();

        _marker.Text = "○";
        _title.Text = "no timer running";
        _detail.Text = $"press {KeyMap.NewTask} to start tracking a task";
        Theme.StyleMuted(_marker);
    }

    public void ShowRunning(RunningTimer timer)
    {
        HideInput();

        _timer = timer;
        _title.Text = timer.TaskTitle;
        Theme.StyleActive(_marker);
        RenderElapsed();

        _timeoutToken ??= _app.AddTimeout(TimeSpan.FromMilliseconds(100), OnTick);
    }

    /// <summary>Opens the task-name prompt and puts the keyboard into it.</summary>
    public void BeginInput()
    {
        StopTicking();

        _marker.Text = "▸";
        Theme.StyleActive(_marker);
        _title.Visible = false;
        _detail.Text = "enter to start · esc to cancel";

        _input.Text = string.Empty;
        _input.Visible = true;
        _input.CanFocus = true;
        _input.SetFocus();
    }

    public void CancelInput()
    {
        ShowIdle();
        InputCancelled?.Invoke();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopTicking();
        }
        base.Dispose(disposing);
    }

    private void HideInput()
    {
        _input.Visible = false;
        _input.CanFocus = false;
        _title.Visible = true;
    }

    private void StopTicking()
    {
        _timer = null;
        if (_timeoutToken is not null)
        {
            _app.RemoveTimeout(_timeoutToken);
            _timeoutToken = null;
        }
    }

    private bool OnTick()
    {
        if (_timer is null)
        {
            return false;
        }

        RenderElapsed();
        return true;
    }

    private void RenderElapsed()
    {
        if (_timer is null)
        {
            return;
        }

        _frame = (_frame + 1) % SpinnerFrames.Length;
        _marker.Text = SpinnerFrames[_frame];
        _detail.Text = TimeFormat.Hms(_timer.ElapsedTime);
        Theme.StyleActive(_detail, bold: true);
    }
}
