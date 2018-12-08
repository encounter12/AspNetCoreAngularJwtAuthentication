using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Data.Repositories;
using AspNetCoreJwtAuthentication.Data.Repositories.Contracts;
using AspNetCoreJwtAuthentication.Data.UnitOfWork;
using AspNetCoreJwtAuthentication.DI.Enums;
using AspNetCoreJwtAuthentication.Middleware;
using AspNetCoreJwtAuthentication.Models.InfrastructureModels;
using AspNetCoreJwtAuthentication.Services.Configuration;

namespace AspNetCoreJwtAuthentication.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjectionContainer(
            this IServiceCollection services, 
            DiContainers selectedContainer, 
            AppData appData,
            JwtSettings jwtSettings)
        {
            if (selectedContainer == DiContainers.AspNetCoreDependencyInjector)
            {
                BindRepositories(services);
                BindServices(services);
                BindJwtTokenHandler(services, jwtSettings);
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

        public static void BindJwtTokenHandler(IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddSingleton<IConfigurationService>(s => new ConfigurationService(jwtSettings));
            services.AddSingleton<ICryptoRsaKeyProvider, CryptoRsaKeyProvider>();
            services.AddSingleton<ICryptoHmacShaKeyProvider, CryptoHmacShaKeyProvider>();

            if (jwtSettings.Alg == SecurityAlgorithms.HmacSha256)
            {
                services.AddScoped<IJwtHandler, JwtHandlerHmacSha256>();
            }
            else if (jwtSettings.Alg == SecurityAlgorithms.RsaSha256)
            {
                services.AddScoped<IJwtHandler, JwtHandlerRsaSha256>();
            }
            else
            {
                throw new ArgumentException($"There is no implementation of : {jwtSettings.Alg}");
            }
        }

        public static void BindDbContexts(IServiceCollection services, AppData appData)
        {
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(appData.DatabaseConnectionString));
        }
    }
}
