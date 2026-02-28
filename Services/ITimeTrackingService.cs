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
