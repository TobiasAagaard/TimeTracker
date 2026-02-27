using TimeTracker.UI;
using TimeTracker.Services;

namespace TimeTracker
{
    internal class Program
    {
        static void Main(string[] args) 
        {
            Menus.ShowBanner();

            var timeTrackingService = new TimeTrackingService();
            var timeTrackingMenu = new Menus.TimeTrackingMenu(timeTrackingService);
            timeTrackingMenu.Run();
        }
    }
}
 