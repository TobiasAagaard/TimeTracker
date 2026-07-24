using Microsoft.Extensions.DependencyInjection;
using TimeTracker.Cli.Views;


var services = new ServiceCollection();
var provider = services.BuildServiceProvider();

await TimerView.RunAsync();

