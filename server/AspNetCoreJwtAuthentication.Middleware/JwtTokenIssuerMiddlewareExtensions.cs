using System;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using AspNetCoreJwtAuthentication.Models.InfrastructureModels;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public static class JwtTokenIssuerMiddlewareExtensions
    {
        public static void UseJwtTokenIssuer(
            this IApplicationBuilder app,
            JwtSettings jwtSettings,
            Func<HttpContext, Task<GenericPrincipal>> principalResolver)
        {
            app.UseMiddleware<JwtTokenIssuerMiddleware>(
                new OptionsWrapper<JwtSettings>(jwtSettings),
                principalResolver);
        }
    }
}
