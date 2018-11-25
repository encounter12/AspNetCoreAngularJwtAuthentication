using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Data.Repositories;
using AspNetCoreJwtAuthentication.Data.Repositories.Contracts;
using AspNetCoreJwtAuthentication.Data.UnitOfWork;
using AspNetCoreJwtAuthentication.DI.Enums;
using AspNetCoreJwtAuthentication.Models.InfrastructureModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreJwtAuthentication.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjectionContainer(
            this IServiceCollection services, DiContainers selectedContainer, AppData appData)
        {
            if (selectedContainer == DiContainers.AspNetCoreDependencyInjector)
            {
                BindRepositories(services);
                BindServices(services);
                BindDbContexts(services, appData);
            }

            return services;
        }

        public static void BindRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IAuditableEntityRepository<>), typeof(AuditableEntityRepository<>));
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void BindServices(IServiceCollection services)
        {
        }

        public static void BindDbContexts(IServiceCollection services, AppData appData)
        {
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(appData.DatabaseConnectionString));
        }
    }
}
