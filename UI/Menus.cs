using System;
using System.Threading;
using TimeTracker.Services;

namespace TimeTracker.UI;

public class Menus
{
    public static void ShowBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
  ████████╗██╗███╗   ███╗███████╗
  ╚══██╔══╝██║████╗ ████║██╔════╝
     ██║   ██║██╔████╔██║█████╗
     ██║   ██║██║╚██╔╝██║██╔══╝
     ██║   ██║██║ ╚═╝ ██║███████╗
     ╚═╝   ╚═╝╚═╝     ╚═╝╚══════╝  TRACKER");
        Console.ResetColor();
    }

    public class TimeTrackingMenu
    {
        private readonly TimeTrackingService _service;
        public TimeTrackingMenu(TimeTrackingService service)
        {
            _service = service;
        }
        public void Run()
        {
            Console.WriteLine("Press ENTER to start tracking...");
            Console.ReadLine();

            _service.Start();

            Console.WriteLine("Tracking started. Press S to stop.");

            while (_service.IsRunning())
            {
                Console.CursorLeft = 0;
                Console.Write($"Hours worked: {_service.GetDecimalHours():F2}");
                Thread.Sleep(100);

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S)
                {
                    _service.Stop();
                    
                }
            }
            Console.WriteLine($"Tracking stopped. You worked for {_service.GetTimeSpan().TotalHours:F2} hours.");
        }
    }

}