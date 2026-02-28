using TimeTracker.UI;
using TimeTracker.Services;
using TimeTracker.Data;

namespace TimeTracker
{
    public static class AppRunner
    {
        public static int Run(
            Func<string, Database>? databaseFactory = null,
            Action? showBanner = null,
            Func<ITimeTrackingService>? serviceFactory = null,
            Action<ITimeTrackingService>? runMenu = null)
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimeTracker.db");

            try
            {
                if (databaseFactory is null)
                {
                    _ = new Database(databasePath);
                }
                else
                {
                    _ = databaseFactory(databasePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                return 1;
            }

            if (showBanner is null)
            {
                Menus.ShowBanner();
            }
            else
            {
                showBanner();
            }

            var timeTrackingService = serviceFactory is null
                ? new TimeTrackingService()
                : serviceFactory();

            if (runMenu is null)
            {
                var timeTrackingMenu = new Menus.TimeTrackingMenu(timeTrackingService);
                timeTrackingMenu.Run();
            }
            else
            {
                runMenu(timeTrackingService);
            }

            return 0;
        }
    }

    internal class Program
    {
        static void Main(string[] args) 
        {
            _ = AppRunner.Run();
        }
    }
}
 