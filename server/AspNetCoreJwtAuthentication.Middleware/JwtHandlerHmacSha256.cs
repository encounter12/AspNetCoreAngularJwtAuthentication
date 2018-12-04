using System;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using AspNetCoreJwtAuthentication.Models.InfrastructureModels;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class JwtHandlerHmacSha256 : IJwtHandler
    {
        private readonly JwtSettings jwtSettings;

        private SecurityKey issuerSigningKey;

        public TokenValidationParameters Parameters { get; private set; }

        public JwtHandlerHmacSha256(JwtSettings jwtSettings)
        {
            this.jwtSettings = jwtSettings;
            InitializeHmacSha256();
            InitializeJwtParameters();
        }

        public string CreateToken(GenericPrincipal genericPrincipal)
        {
            var utcNow = DateTime.UtcNow;

            long unixTimeSeconds = (long)Math.Round(
                (utcNow.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

            var signingCredentials = new SigningCredentials(this.issuerSigningKey, SecurityAlgorithms.HmacSha256);

            var existingClaims = genericPrincipal.Claims.ToList();

            var systemClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, genericPrincipal.Identity.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
            };

            existingClaims.AddRange(systemClaims);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Issuer,
                claims: existingClaims,
                notBefore: utcNow,
                expires: utcNow.AddMinutes(this.jwtSettings.ExpiresIn),
                signingCredentials: signingCredentials);

            string writtenToken = new JwtSecurityTokenHandler().WriteToken(token);
            return writtenToken;
        }

        private void InitializeHmacSha256()
        {
            this.issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.HmacSha256Key));
        }

        private void InitializeJwtParameters()
        {
            Parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = this.jwtSettings.Issuer,
                ValidAudience = this.jwtSettings.Issuer,
                IssuerSigningKey = this.issuerSigningKey
            };
        }
    }
}
