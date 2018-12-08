using System;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AspNetCoreJwtAuthentication.Services.Configuration;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class JwtHandlerHmacSha256 : IJwtHandler
    {
        private readonly IConfigurationService configurationService;
        private readonly ICryptoHmacShaKeyProvider cryptoHmacShaKeyProvider;

        private SecurityKey issuerSigningKey;

        public TokenValidationParameters Parameters { get; private set; }

        public JwtHandlerHmacSha256(
            IConfigurationService configurationService,
            ICryptoHmacShaKeyProvider cryptoHmacShaKeyProvider)
        {
            this.configurationService = configurationService;
            this.cryptoHmacShaKeyProvider = cryptoHmacShaKeyProvider;
            InitializeHmacSha256();
            InitializeJwtParameters();
        }

        public string CreateToken(GenericPrincipal genericPrincipal)
        {
            var utcNow = DateTime.UtcNow;

            long unixTimeSeconds = (long)Math.Round(
                (utcNow.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

            var existingClaims = genericPrincipal.Claims.ToList();

            var systemClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, genericPrincipal.Identity.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
            };

            existingClaims.AddRange(systemClaims);

            var signingCredentials = new SigningCredentials(this.issuerSigningKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configurationService.JwtSettings?.Issuer,
                audience: configurationService.JwtSettings?.Issuer,
                claims: existingClaims,
                notBefore: utcNow,
                expires: utcNow.AddMinutes(this.configurationService.JwtSettings?.ExpiresIn ?? 30),
                signingCredentials: signingCredentials);

            string writtenToken = new JwtSecurityTokenHandler().WriteToken(token);
            return writtenToken;
        }

        private void InitializeHmacSha256()
        {
            string secretKey = this.cryptoHmacShaKeyProvider.GetSecretKey();
            this.issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        private void InitializeJwtParameters()
        {
            Parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = this.configurationService.JwtSettings?.Issuer,
                ValidAudience = this.configurationService.JwtSettings?.Issuer,
                IssuerSigningKey = this.issuerSigningKey
            };
        }
    }
}
