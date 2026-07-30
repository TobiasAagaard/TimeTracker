using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.App;
using TimeTracker.Cli.Data;
using TimeTracker.Cli.Services;
using TimeTracker.Cli.Views;
using TimeTracker.Core.Interfaces;

using var app = Application.Create();

var services = new ServiceCollection();

services.AddDbContext<LocalDbContext>();
services.AddScoped<ITimerService, TimerService>();
services.AddScoped<ITrackedTasksRepository, TrackedTasksRepository>();
services.AddScoped<ITimeSlotsRepository, TimeSlotsRepository>();
services.AddSingleton(app);
services.AddScoped<MainWindow>();

using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
await dbContext.Database.EnsureCreatedAsync();

app.Init();

var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
await mainWindow.LoadStateAsync();

app.Run(mainWindow);
mainWindow.Dispose();
