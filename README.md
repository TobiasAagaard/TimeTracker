# TimeTracker

A simple command-line time tracking app built with .NET 10.

## Run

```bash
dotnet run --project cli/Timetracker.Cli
```

## Usage

The app runs as an interactive loop:

1. Type a task name and press **Enter** to start tracking — you'll see a live ticking clock.
2. Press **Enter** (or Esc / Q) to stop.
3. Leave the prompt empty and press **Enter** (or Ctrl+C) to quit.

Timers are saved to a local SQLite database, created automatically on first run.
