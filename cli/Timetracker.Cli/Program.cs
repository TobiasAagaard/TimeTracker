using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timetracker.Cli.Data;
using Timetracker.Cli.Interfaces;
using Timetracker.Cli.Services;
using Timetracker.Cli.Views;

var dbPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "timetracker",
    "timetracker.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

var services = new ServiceCollection();
services.AddDbContext<TimeTrackerDbContext>(
    options => options.UseSqlite($"Data Source={dbPath}"),
    ServiceLifetime.Singleton);
services.AddSingleton<ITrackedTaskService, TrackedTaskService>();
services.AddSingleton<ITimeSlotService, TimeSlotService>();
services.AddSingleton<ITimerService, TimerService>();
services.AddSingleton<TimerView>();

var provider = services.BuildServiceProvider();

await provider.GetRequiredService<TimeTrackerDbContext>().Database.EnsureCreatedAsync();

await provider.GetRequiredService<TimerView>().RunAsync();
