using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public interface IJwtHandler
    {
        TokenValidationParameters Parameters { get; }

        string CreateToken(GenericPrincipal genericPrincipal);
    }
}
