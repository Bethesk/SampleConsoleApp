using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

class Program
{
    //static async Task Main(string[] args)
    //{
    //    // Create and start the host with Hangfire and Kestrel
    //    var host = CreateHostBuilder(args).Build();
    //    await host.RunAsync();
    //}

    //// Configure the host and services
    //public static IHostBuilder CreateHostBuilder(string[] args) =>
    //    Host.CreateDefaultBuilder(args)
    //        .UseWindowsService()
    //        .ConfigureServices((context, services) =>
    //        {
    //            // Add Hangfire services and use in-memory storage
    //            services.AddHangfire(config => config.UseMemoryStorage());
    //            services.AddHangfireServer();
    //        })
    //        .ConfigureLogging(logging =>
    //        {
    //            logging.ClearProviders();
    //            logging.AddConsole();
    //        })
    //        .ConfigureWebHostDefaults(webBuilder =>
    //        {
    //            // Configure the app to use Hangfire Dashboard
    //            webBuilder.Configure(app =>
    //            {
    //                app.UseHangfireDashboard("/hangfire");
    //            });
    //            // Configure Kestrel to listen on localhost at port 5000
    //            webBuilder.UseUrls("http://localhost:5000");
    //        });

    static async Task Main(string[] args)
    {
        // Create and start the host with Hangfire and Kestrel
        var host = CreateHostBuilder(args).Build();

        // Run the host
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService()  // Enables running as a Windows Service
            .ConfigureServices((context, services) =>
            {
                // Add Hangfire services and use in-memory storage
                services.AddHangfire(config => config.UseMemoryStorage());
                services.AddHangfireServer();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.Configure(app =>
                {
                    // Configure Hangfire Dashboard
                    app.UseHangfireDashboard("/hangfire");
                });
                webBuilder.UseUrls("http://localhost:5000");
            });

    // Background job to move a file
    public static async Task MoveFileAsync()
    {
        string sourceFilePath = @"C:\file.txt"; // Source file path
        string destinationFilePath = @"C:\New Folder\file.txt"; // Destination file path

        try
        {
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine($"Source file does not exist: {sourceFilePath}");
                return;
            }

            string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
                Console.WriteLine($"Created directory: {destinationDirectory}");
            }

            await Task.Run(() => File.Move(sourceFilePath, destinationFilePath));

            Console.WriteLine($"File moved from {sourceFilePath} to {destinationFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
