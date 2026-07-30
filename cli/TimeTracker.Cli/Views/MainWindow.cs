using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TimeTracker.Core.DTOs;
using TimeTracker.Core.Interfaces;

namespace TimeTracker.Cli.Views;

public sealed class MainWindow : Window
{
    private enum UiState
    {
        Idle,
        Input,
        Running,
    }

    private readonly ITimerService _timerService;
    private readonly IApplication _app;
    private readonly TimerPane _timerPane;
    private readonly SummaryPane _summaryPane;
    private readonly Label _hints;

    private UiState _state = UiState.Idle;
    private bool _busy;

    public MainWindow(ITimerService timerService, IApplication app)
    {
        _timerService = timerService;
        _app = app;

        BorderStyle = LineStyle.None;

        var heading = new Label { X = 1, Y = 0, Text = "⏱  timetracker" };
        var today = new Label
        {
            X = Pos.AnchorEnd(),
            Y = 0,
            Text = $"{DateTime.Now:ddd d MMM}  ",
        };

        _timerPane = new TimerPane(app)
        {
            X = 1,
            Y = 2,
            Width = Dim.Fill(1),
            Height = 5,
        };

        _summaryPane = new SummaryPane
        {
            X = 1,
            Y = Pos.Bottom(_timerPane),
            Width = Dim.Fill(1),
            Height = Dim.Fill(2),
        };

        _hints = new Label { X = 1, Y = Pos.AnchorEnd(), Width = Dim.Fill(1) };

        _timerPane.StartRequested += OnStartRequested;
        _timerPane.InputCancelled += () =>
        {
            _state = UiState.Idle;
            _timerPane.SetFocus();
            RenderHints();
        };

        Initialized += (_, _) =>
        {
            Theme.StyleMuted(_hints);
            Theme.StyleMuted(today);
        };

        _app.Keyboard.KeyDown += OnKeyDown;

        Add(heading, today, _timerPane, _summaryPane, _hints);
        RenderHints();
    }

    public async Task LoadStateAsync()
    {
        try
        {
            _summaryPane.SetSummaries(await _timerService.GetDailyTaskSummariesAsync());

            var running = await _timerService.GetRunningTimerAsync();
            if (running is not null)
            {
                EnterRunning(running);
            }
            else
            {
                EnterIdle();
            }
        }
        catch (Exception ex)
        {
            EnterIdle();
            ShowError("Could not load the previous state.", ex);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _app.Keyboard.KeyDown -= OnKeyDown;
        }
        base.Dispose(disposing);
    }

    private async void OnKeyDown(object? sender, Key key)
    {
        if (!ReferenceEquals(_app.TopRunnable, this))
        {
            return;
        }

        if (_state == UiState.Input)
        {
            if (key == Key.Esc)
            {
                key.Handled = true;
                _timerPane.CancelInput();
            }
            return;
        }

        if (key == KeyMap.NewTask && _state == UiState.Idle)
        {
            key.Handled = true;
            EnterInput();
        }
        else if (key == KeyMap.StopTimer && _state == UiState.Running)
        {
            key.Handled = true;
            await StopTimer();
        }
        else if (key == KeyMap.FocusTimer)
        {
            key.Handled = true;
            _timerPane.SetFocus();
        }
        else if (key == KeyMap.FocusSummary)
        {
            key.Handled = true;
            _summaryPane.SetFocus();
        }
        else if (key == KeyMap.Refresh)
        {
            key.Handled = true;
            RefreshSummaries();
        }
        else if (key == KeyMap.Help)
        {
            key.Handled = true;
            ShowHelp();
        }
        else if (key == KeyMap.Quit || key == Key.Esc)
        {
            key.Handled = true;
            _app.RequestStop(this);
        }
    }

    private async void OnStartRequested(string title)
    {
        if (_busy)
        {
            return;
        }
        _busy = true;

        try
        {
            await _timerService.StartTimerAsync(title);
            var running = await _timerService.GetRunningTimerAsync()
                          ?? new RunningTimer { TaskTitle = title, StartedAt = DateTime.UtcNow };

            _app.Invoke(() => EnterRunning(running));
        }
        catch (Exception ex)
        {
            ShowError($"Could not start '{title}'.", ex);
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task StopTimer()
    {
        if (_busy)
        {
            return;
        }
        _busy = true;

        try
        {
            await _timerService.StopTimerAsync();
            var summaries = await _timerService.GetDailyTaskSummariesAsync();

            _app.Invoke(() =>
            {
                _summaryPane.SetSummaries(summaries);
                EnterIdle();
            });
        }
        catch (Exception ex)
        {
            ShowError("Could not stop the timer.", ex);
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task RefreshSummaries()
    {
        if (_busy)
        {
            return;
        }
        _busy = true;

        try
        {
            var summaries = await _timerService.GetDailyTaskSummariesAsync();
            _app.Invoke(() => _summaryPane.SetSummaries(summaries));
        }
        catch (Exception ex)
        {
            ShowError("Could not refresh today's totals.", ex);
        }
        finally
        {
            _busy = false;
        }
    }

    private void EnterIdle()
    {
        _state = UiState.Idle;
        _timerPane.ShowIdle();
        _timerPane.SetFocus();
        RenderHints();
    }

    private void EnterInput()
    {
        _state = UiState.Input;
        _timerPane.BeginInput();
        RenderHints();
    }

    private void EnterRunning(RunningTimer timer)
    {
        _state = UiState.Running;
        _timerPane.ShowRunning(timer);
        RenderHints();
    }

    private void RenderHints()
    {
        _hints.Text = _state switch
        {
            UiState.Input => "enter  start   ·   esc  cancel",
            UiState.Running => Join($"{KeyMap.StopTimer}  stop"),
            _ => Join($"{KeyMap.NewTask}  new"),
        };

        static string Join(string action) =>
            $"{action}   ·   {KeyMap.FocusTimer}/{KeyMap.FocusSummary}  panes   ·   tab  cycle   ·   " +
            $"{KeyMap.Refresh}  refresh   ·   {KeyMap.Help}  help   ·   {KeyMap.Quit}  quit";
    }

    private void ShowHelp()
    {
        MessageBox.Query(
            _app,
            " keys ",
            $"""
             {KeyMap.NewTask}          start a new task
             {KeyMap.StopTimer}          stop the running timer
             {KeyMap.FocusTimer} / {KeyMap.FocusSummary}      focus the timer / today pane
             tab        cycle panes
             {KeyMap.Refresh}          reload today's totals
             {KeyMap.Help}          this help
             {KeyMap.Quit} / esc    quit (a running timer keeps counting)
             """,
            "close");
    }

    private void ShowError(string message, Exception ex)
    {
        _app.Invoke(() => MessageBox.ErrorQuery(_app, " error ", $"{message}\n\n{ex.Message}", "close"));
    }
}
