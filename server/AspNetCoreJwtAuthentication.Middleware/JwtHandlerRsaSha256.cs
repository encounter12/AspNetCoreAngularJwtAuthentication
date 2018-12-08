using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using AspNetCoreJwtAuthentication.Services.Configuration;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class JwtHandlerRsaSha256 : IJwtHandler
    {
        private readonly ICryptoRsaKeyProvider cryptoRsaKeyProvider;

        private readonly IConfigurationService configurationService;

        private SecurityKey issuerSigningKey;

        public TokenValidationParameters Parameters { get; private set; }

        public JwtHandlerRsaSha256(IConfigurationService configurationService, ICryptoRsaKeyProvider cryptoRsaKeyProvider)
        {
            this.configurationService = configurationService;
            this.cryptoRsaKeyProvider = cryptoRsaKeyProvider;
            InitializeRsa256();
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

            string privateKeyFileContent = this.cryptoRsaKeyProvider.GetPrivateKey();

            var token = this.CreateToken(existingClaims, utcNow, privateKeyFileContent);

            return token;
        }

        private string CreateToken(List<Claim> claims, DateTime utcNow, string privateRsaKey)
        {
            using (RSA privateRsa = RSA.Create())
            {
                RSAParameters rsaParameters;

                using (var tr = new StringReader(privateRsaKey))
                {
                    var pemReader = new PemReader(tr);
                    var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;

                    if (keyPair == null)
                    {
                        throw new Exception("Could not read RSA private key");
                    }

                    var privateKeyParameters = keyPair.Private as RsaPrivateCrtKeyParameters;
                    rsaParameters = DotNetUtilities.ToRSAParameters(privateKeyParameters);
                }

                privateRsa.ImportParameters(rsaParameters);

                SecurityKey securityKey = new RsaSecurityKey(privateRsa);

                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

                var token = new JwtSecurityToken(
                    issuer: this.configurationService.JwtSettings?.Issuer,
                    audience: this.configurationService.JwtSettings?.Issuer,
                    claims: claims,
                    notBefore: utcNow,
                    expires: utcNow.AddMinutes(this.configurationService.JwtSettings?.ExpiresIn ?? 30),
                    signingCredentials: signingCredentials);

                string writtenToken = new JwtSecurityTokenHandler().WriteToken(token);

                return writtenToken;
            }
        }

        private void InitializeRsa256()
        {
            string publicKeyFileContent = this.cryptoRsaKeyProvider.GetPublicKey();

            RSA publicRsa = this.BuildCryptoServiceProvider(publicKeyFileContent);
            this.issuerSigningKey = new RsaSecurityKey(publicRsa);
        }

        private RSACryptoServiceProvider BuildCryptoServiceProvider(string keyFileContent)
        {
            using (var keyTextReader = new StringReader(keyFileContent))
            {
                var pemReader = new PemReader(keyTextReader);
                RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)pemReader.ReadObject();

                var rsaParameters = new RSAParameters
                {
                    Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
                    Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned()
                };

                var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
                rsaCryptoServiceProvider.ImportParameters(rsaParameters);

                return rsaCryptoServiceProvider;
            }
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
