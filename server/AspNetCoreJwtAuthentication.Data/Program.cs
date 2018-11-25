using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Models.InfrastructureModels;
using AspNetCoreJwtAuthentication.Data.Seed;

namespace AspNetCoreJwtAuthentication.Data
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Configuration/appdata.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args)
                .Build();

            var services = new ServiceCollection();

            AppData appData = config.GetSection("AppData").Get<AppData>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(appData.DatabaseConnectionString));

            var serviceProvider = services.BuildServiceProvider();

            try
            {
                var applicationDbContext = serviceProvider.GetService<ApplicationDbContext>();
                DatabaseInitializer.SeedData(applicationDbContext);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            Environment.Exit(0);
        }
    }
}
