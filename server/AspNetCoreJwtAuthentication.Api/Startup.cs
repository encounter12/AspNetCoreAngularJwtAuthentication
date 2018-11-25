using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AspNetCoreJwtAuthentication.Models.InfrastructureModels;
using AspNetCoreJwtAuthentication.DI;
using AspNetCoreJwtAuthentication.DI.Enums;
using AspNetCoreJwtAuthentication.Data.Context;
using Microsoft.AspNetCore.Identity;
using AspNetCoreJwtAuthentication.Models.IdentityModels;

namespace AspNetCoreJwtAuthentication.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AppData appData = Configuration.GetSection("AppData").Get<AppData>();
            services.AddDependencyInjectionContainer(DiContainers.AspNetCoreDependencyInjector, appData);

            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 2;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredUniqueChars = 2;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
