using Microsoft.Extensions.DependencyInjection;
using TimeTracker.Cli.Views;


var services = new ServiceCollection();
services.AddTransient<TimerView>();

var provider = services.BuildServiceProvider();

await provider.GetRequiredService<TimerView>().RunAsync();

