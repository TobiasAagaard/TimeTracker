using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Cli.Views;
using TimeTracker.Cli.Data;


var services = new ServiceCollection();

services.AddDbContext<LocalDbContext>();

using var serviceProvider = services.BuildServiceProvider();
using (var scope = serviceProvider.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
	dbContext.Database.EnsureCreated();
}

await TimerView.RunAsync();

