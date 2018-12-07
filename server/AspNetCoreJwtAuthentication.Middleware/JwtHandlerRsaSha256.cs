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
using AspNetCoreJwtAuthentication.Models.InfrastructureModels;
using System.Reflection;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class JwtHandlerRsaSha256 : IJwtHandler
    {
        private readonly JwtSettings jwtSettings;

        private SecurityKey issuerSigningKey;

        public TokenValidationParameters Parameters { get; private set; }

        public JwtHandlerRsaSha256(JwtSettings jwtSettings)
        {
            this.jwtSettings = jwtSettings;
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

            string privateKeyRelativeDirectory = "Keys";
            string privateKeyFileName = "privateKey.pem";

            var currentAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string privateKeyFilePath = Path.Combine(
                currentAssemblyDirectory,
                privateKeyRelativeDirectory,
                privateKeyFileName);

            string privateKeyPemFileContent = File.ReadAllText(privateKeyFilePath);

            var token = this.CreateToken(existingClaims, utcNow, privateKeyPemFileContent);

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
                    issuer: jwtSettings.Issuer,
                    audience: jwtSettings.Issuer,
                    claims: claims,
                    notBefore: utcNow,
                    expires: utcNow.AddMinutes(this.jwtSettings.ExpiresIn),
                    signingCredentials: signingCredentials);

                string writtenToken = new JwtSecurityTokenHandler().WriteToken(token);

                return writtenToken;
            }
        }

        private void InitializeRsa256()
        {
            string publicKey = File.ReadAllText(@"E:\Projects\AspNetCoreAngularJwtAuthentication\server\AspNetCoreJwtAuthentication.Middleware\Keys\publicKey.pem");
            RSA publicRsa = this.PublicKeyFromPemFile(publicKey);
            this.issuerSigningKey = new RsaSecurityKey(publicRsa);
        }

        private RSACryptoServiceProvider PublicKeyFromPemFile(string publicKey)
        {
            using (var publicKeyTextReader = new StringReader(publicKey))
            {
                var pemReader = new PemReader(publicKeyTextReader);
                RsaKeyParameters publicKeyParameters = (RsaKeyParameters)pemReader.ReadObject();
                var parameters = new RSAParameters();

                parameters.Modulus = publicKeyParameters.Modulus.ToByteArrayUnsigned();
                parameters.Exponent = publicKeyParameters.Exponent.ToByteArrayUnsigned();

                RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();
                cryptoServiceProvider.ImportParameters(parameters);
                return cryptoServiceProvider;
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
                ValidIssuer = this.jwtSettings.Issuer,
                ValidAudience = this.jwtSettings.Issuer,
                IssuerSigningKey = this.issuerSigningKey
            };
        }
    }
}
