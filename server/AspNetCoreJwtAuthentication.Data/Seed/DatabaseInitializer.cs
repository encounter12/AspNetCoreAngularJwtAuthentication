using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Models.IdentityModels;
using AspNetCoreJwtAuthentication.Models.DomainModels;

namespace AspNetCoreJwtAuthentication.Data.Seed
{
    public class DatabaseInitializer
    {
        public static void SeedData(ApplicationDbContext applicationDbContext)
        {
            //AddMigrations(applicationDbContext);

            //applicationDbContext.Database.EnsureCreated();
            //bool databaseExists = (applicationDbContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();

            applicationDbContext.Database.Migrate();
            bool allMigrationsApplied = AllMigrationsApplied(applicationDbContext);

            if (allMigrationsApplied)
            {
                var userManager = applicationDbContext.GetService<UserManager<ApplicationUser>>();
                SeedDatabase(applicationDbContext, userManager).Wait();
            }
        }

        private static bool AllMigrationsApplied(DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        private static async Task SeedDatabase(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            if (!context.Users.Any())
            {
                var usersWithPasswords = new List<UserWithPassword>
                {
                    new UserWithPassword
                    {
                        ApplicationUser = new ApplicationUser()
                        {
                            UserName = "alice",
                            Email = "alice@sample.com",
                            Birthdate = new DateTime(1985, 04, 23)
                        },
                        Password = "alice"
                    }
                    ,
                    new UserWithPassword
                    {
                        ApplicationUser = new ApplicationUser()
                        {
                            UserName = "bob",
                            Email = "bob@sample.com",
                            Birthdate = new DateTime(1990, 02, 15)
                        },
                        Password = "bob"
                    }
                };

                foreach (var userWithPassword in usersWithPasswords)
                {
                    IdentityResult result = await userManager.CreateAsync(
                        userWithPassword.ApplicationUser,
                        userWithPassword.Password);
                }

                context.SaveChanges();
            }

            if (!context.Reports.Any())
            {
                var reports = new List<Report>
                {
                    new Report() { Name = "First Report" },
                    new Report() { Name = "Second Report" }
                };

                context.Reports.AddRange(reports);
                context.SaveChanges();
            }
        }

        private static void AddMigrations(ApplicationDbContext context)
        {
            var reporter = new OperationReporter(
                new OperationReportHandler(
                    m => Console.WriteLine("  error: " + m),
                    m => Console.WriteLine("   warn: " + m),
                    m => Console.WriteLine("   info: " + m),
                    m => Console.WriteLine("verbose: " + m)));

            var designTimeServices = new ServiceCollection()
                .AddSingleton(context.GetService<IHistoryRepository>())
                .AddSingleton(context.GetService<IMigrationsIdGenerator>())
                .AddSingleton(context.GetService<IMigrationsModelDiffer>())
                .AddSingleton(context.GetService<IMigrationsAssembly>())
                .AddSingleton(context.Model)
                .AddSingleton(context.GetService<ICurrentDbContext>())
                .AddSingleton(context.GetService<IDatabaseProvider>())
                .AddSingleton<MigrationsCodeGeneratorDependencies>()
                .AddSingleton<ICSharpHelper, CSharpHelper>()
                .AddSingleton<CSharpMigrationOperationGeneratorDependencies>()
                .AddSingleton<ICSharpMigrationOperationGenerator, CSharpMigrationOperationGenerator>()
                .AddSingleton<CSharpSnapshotGeneratorDependencies>()
                .AddSingleton<ICSharpSnapshotGenerator, CSharpSnapshotGenerator>()
                .AddSingleton<CSharpMigrationsGeneratorDependencies>()
                .AddSingleton<IMigrationsCodeGenerator, CSharpMigrationsGenerator>()
                .AddSingleton<IOperationReporter>(reporter)
                .AddSingleton<MigrationsScaffolderDependencies>()
                .AddSingleton<MigrationsScaffolder>()
                .BuildServiceProvider();

            var scaffolder = designTimeServices.GetRequiredService<MigrationsScaffolder>();

            var migration = scaffolder.ScaffoldMigration(
                "MyMigration",
                "AspNetCoreJwtAuthentication.Data");

            File.WriteAllText(
                migration.MigrationId + migration.FileExtension,
                migration.MigrationCode);
            File.WriteAllText(
                migration.MigrationId + ".Designer" + migration.FileExtension,
                migration.MetadataCode);
            File.WriteAllText(migration.SnapshotName + migration.FileExtension,
                migration.SnapshotCode);
        }

        public class UserWithPassword
        {
            public ApplicationUser ApplicationUser { get; set; }

            public string Password { get; set; }
        }
    }
}
