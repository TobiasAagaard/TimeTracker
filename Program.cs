using TimeTracker.UI;
using TimeTracker.Services;
using TimeTracker.Data;

namespace TimeTracker
{
    internal class Program
    {
        static void Main(string[] args) 
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimeTracker.db");
            try
            {
            _ = new Database(databasePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                return;
            }

            Menus.ShowBanner();

            var timeTrackingService = new TimeTrackingService();
            var timeTrackingMenu = new Menus.TimeTrackingMenu(timeTrackingService);
            timeTrackingMenu.Run();
        }
    }
}
 