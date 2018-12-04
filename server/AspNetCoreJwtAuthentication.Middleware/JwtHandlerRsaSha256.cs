using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class JwtHandlerRsaSha256 : IJwtHandler
    {
        public TokenValidationParameters Parameters { get; private set; }

        public string CreateToken(GenericPrincipal genericPrincipal)
        {
           return string.Empty;
        }
    }
}
