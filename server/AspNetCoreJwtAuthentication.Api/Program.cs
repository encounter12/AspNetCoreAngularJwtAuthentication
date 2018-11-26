using System;
using System.Diagnostics;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Data.Seed;

namespace AspNetCoreJwtAuthentication.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var applicationDbContext = services.GetService<ApplicationDbContext>();
                    DatabaseInitializer.SeedData(applicationDbContext);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("Configuration/appdata.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                    config.AddCommandLine(args);
                })
                .UseStartup<Startup>();
        }
    }
}
