using System;
using System.Diagnostics;

namespace TimeTracker.Services;

public interface ITimeTrackingService
{
    void Start();
    void Stop();
    double GetDecimalHours();
    TimeSpan GetTimeSpan();
    bool IsRunning();
    void Reset();
}

public class TimeTrackingService : ITimeTrackingService
{
    private readonly Stopwatch _stopwatch = new Stopwatch();
    
    public void Start()
    {
        _stopwatch.Start();
    }

    public void Stop()
    {
        _stopwatch.Stop();
    }
    public double GetDecimalHours()
    {
        return _stopwatch.Elapsed.TotalHours;
    }
    public TimeSpan GetTimeSpan()
    {
        return _stopwatch.Elapsed;
    }

    public bool IsRunning() => _stopwatch.IsRunning;
    public void Reset()
    {
        _stopwatch.Reset();
    }
}