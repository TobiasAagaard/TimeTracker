

using Microsoft.Playwright;

namespace TimeTracker
{
   

    internal class Program
    {
        static async Task Main()
        {
            using var app = await Playwright.CreateAsync();

            await using var browser = await app.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            var page = await browser.NewPageAsync();
            await page.GotoAsync("https://www.toggl.com");

            Console.WriteLine("Browser opened. Press any key to close...");
            Console.ReadKey();
        }
    }
}
 