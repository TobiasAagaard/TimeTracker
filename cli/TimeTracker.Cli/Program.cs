using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Core.Interfaces;
using TimeTracker.Cli.Data;
using TimeTracker.Cli.Views;
using TimeTracker.Cli.Services;


var services = new ServiceCollection();


services.AddDbContext<LocalDbContext>();
services.AddScoped<ITimerService, TimerService>();
services.AddScoped<ITrackedTasksRepository, TrackedTasksRepository>();
services.AddScoped<ITimeSlotsRepository, TimeSlotsRepository>();
services.AddScoped<TimerView>();


using var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
	await dbContext.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<TimerView>().RunAsync();
}


