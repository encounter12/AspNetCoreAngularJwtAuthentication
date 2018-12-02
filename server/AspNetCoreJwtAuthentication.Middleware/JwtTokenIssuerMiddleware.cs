using System;
using System.Collections.Generic;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using AspNetCoreJwtAuthentication.Models.IdentityModels;
using AspNetCoreJwtAuthentication.Models.InfrastructureModels;
using AspNetCoreJwtAuthentication.Middleware.Models;
using AspNetCoreJwtAuthentication.Data.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class JwtTokenIssuerMiddleware
    {
        private readonly RequestDelegate next;
        private JwtSettings jwtSettings;

        public JwtTokenIssuerMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
        {
            this.next = next;
            this.jwtSettings = jwtSettings.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext applicationDbContext)
        {
            string pathValue = httpContext.Request.Path.Value;

            if (pathValue.Contains($"/token"))
            {
                httpContext.Request.EnableRewind();
                string requestBodyStr = new StreamReader(httpContext.Request.Body).ReadToEnd();

                var loginModel = JsonConvert.DeserializeObject<LoginModel>(requestBodyStr);
                var signInManager = applicationDbContext.GetService<SignInManager<ApplicationUser>>();

                var result = await signInManager.PasswordSignInAsync(
                    loginModel.Username,
                    loginModel.Password,
                    loginModel.RememberLogin,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    httpContext.Response.ContentType = "application/json";

                    var userManager = applicationDbContext.GetService<UserManager<ApplicationUser>>();

                    ApplicationUser applicationUser = await userManager.FindByNameAsync(loginModel.Username);

                    var userModel = new UserModel()
                    {
                        Name = applicationUser.UserName,
                        Email = applicationUser.Email,
                        Birthdate = applicationUser.Birthdate
                    };

                    var userRoleClaims = await this.GetUserRoleClaims(applicationUser, userManager);

                    if (this.jwtSettings == null || this.jwtSettings.Key == null || this.jwtSettings.Issuer == null)
                    {
                        this.jwtSettings = new JwtSettings()
                        {
                            Key = "this is my custom Secret key for authnetication (using middleware)",
                            Issuer = this.GetApplicationUrl(httpContext)
                        };
                    }

                    var tokenResponse = new
                    {
                        token = this.BuildToken(userModel, userRoleClaims, this.jwtSettings)
                    };

                    string tokenResponseStr = JsonConvert.SerializeObject(tokenResponse);
                    await httpContext.Response.WriteAsync(tokenResponseStr, Encoding.UTF8);
                }
                else
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync("wrong username or password");
                }

                // to stop futher pipeline execution 
                return;
            }

            await this.next(httpContext);
        }

        private string BuildToken(
            UserModel user,
            List<Claim> userRoleClaims,
            JwtSettings jwtSettings)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(userRoleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                jwtSettings.Issuer,
                jwtSettings.Issuer,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<List<Claim>> GetUserRoleClaims(
            ApplicationUser applicationUser,
            UserManager<ApplicationUser> userManager)
        {
            IList<string> userRoles = await userManager.GetRolesAsync(applicationUser);

            var userRoleClaims = userRoles
                .Select(role => new Claim(ClaimTypes.Role, role))
                .ToList();

            return userRoleClaims;
        }

        private string GetApplicationUrl(HttpContext context)
        {
            string hostValue = context.Request.Host.Value.ToString();

            string requestScheme = context.Request.Scheme;

            string applicationUrl = string.Format("{0}://{1}", requestScheme, hostValue);

            return applicationUrl;
        }
    }
}
