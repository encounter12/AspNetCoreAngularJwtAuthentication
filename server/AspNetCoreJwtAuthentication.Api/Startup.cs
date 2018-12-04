using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using AspNetCoreJwtAuthentication.Api.ViewModels;
using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.DI;
using AspNetCoreJwtAuthentication.DI.Enums;
using AspNetCoreJwtAuthentication.Middleware;
using AspNetCoreJwtAuthentication.Models.IdentityModels;
using AspNetCoreJwtAuthentication.Models.InfrastructureModels;

namespace AspNetCoreJwtAuthentication.Api
{
    public class Startup
    {
        private readonly JwtSettings jwtSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this.jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AppData appData = Configuration.GetSection("AppData").Get<AppData>();
            services.AddDependencyInjectionContainer(
                DiContainers.AspNetCoreDependencyInjector,
                appData,
                this.jwtSettings);

            var serviceProvider = services.BuildServiceProvider();
            var jwtHandler = serviceProvider.GetService<IJwtHandler>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = jwtHandler.Parameters;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",  
                    new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .RequireRole("Administrator")
                        .Build());
            });

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

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
                .Build());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            app.UseJwtTokenIssuer(this.jwtSettings, PrincipalResolver);

            app.UseMvc();
        }

        private static async Task<GenericPrincipal> PrincipalResolver(HttpContext httpContext)
        {
            httpContext.Request.EnableRewind();
            string requestBodyStr = new StreamReader(httpContext.Request.Body).ReadToEnd();
            var loginModel = JsonConvert.DeserializeObject<LoginModel>(requestBodyStr);

            var signInManager = httpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();

            var result = await signInManager.PasswordSignInAsync(
                loginModel.Username,
                loginModel.Password,
                loginModel.RememberLogin,
                lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return null;
            }

            var userManager = httpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser appUser = await userManager.FindByNameAsync(loginModel.Username);

            var identity = new GenericIdentity(appUser.UserName, "Token");

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appUser.Id));
            identity.AddClaim(new Claim(ClaimTypes.Email, appUser.Email));
            identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, appUser.Birthdate.ToString("yyyy-MM-dd")));

            var roles = await userManager.GetRolesAsync(appUser);

            return new GenericPrincipal(identity, roles.ToArray());
        }
    }
}
