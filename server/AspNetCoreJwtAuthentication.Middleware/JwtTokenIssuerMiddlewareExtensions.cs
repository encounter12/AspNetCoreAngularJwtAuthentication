using System;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public static class JwtTokenIssuerMiddlewareExtensions
    {
        public static void UseJwtTokenIssuer(
            this IApplicationBuilder app,
            Func<HttpContext, Task<GenericPrincipal>> principalResolver)
        {
            app.UseMiddleware<JwtTokenIssuerMiddleware>(principalResolver);
        }
    }
}
