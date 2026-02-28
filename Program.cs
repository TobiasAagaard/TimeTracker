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
            var database = new Database(databasePath);

            Menus.ShowBanner();

            var timeTrackingService = new TimeTrackingService();
            var timeTrackingMenu = new Menus.TimeTrackingMenu(timeTrackingService);
            timeTrackingMenu.Run();
        }
    }
}
 