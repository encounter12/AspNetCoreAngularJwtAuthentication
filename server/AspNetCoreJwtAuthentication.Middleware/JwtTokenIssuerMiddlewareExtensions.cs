using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

using AspNetCoreJwtAuthentication.Models.InfrastructureModels;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public static class JwtTokenIssuerMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtTokenIssuer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtTokenIssuerMiddleware>();
        }

        public static IApplicationBuilder UseJwtTokenIssuer(
            this IApplicationBuilder builder, 
            JwtSettings jwtSettings)
        {
            return builder.UseMiddleware<JwtTokenIssuerMiddleware>(
                new OptionsWrapper<JwtSettings>(jwtSettings));
        }
    }
}
