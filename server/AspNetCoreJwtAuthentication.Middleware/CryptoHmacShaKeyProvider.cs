using System.IO;
using System.Reflection;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class CryptoHmacShaKeyProvider : ICryptoHmacShaKeyProvider
    {
        private readonly string cryptoKeysAbsoluteDirectory;

        public CryptoHmacShaKeyProvider()
        {
            this.cryptoKeysAbsoluteDirectory = this.GetCryptoKeysAbsoluteDirectory();
        }

        public string GetSecretKey()
        {
            string secretKeyFileName = "secretKey.txt";
            string secretKeyFilePath = Path.Combine(this.cryptoKeysAbsoluteDirectory, secretKeyFileName);

            string secretKeyFileContent = File.ReadAllText(secretKeyFilePath);

            return secretKeyFileContent;
        }

        private string GetCryptoKeysAbsoluteDirectory()
        {
            string cryptoKeysRelativeDirectory = @"Keys/HS256";

            var currentAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string cryptoKeysAbsoluteDirectory = Path.Combine(
                currentAssemblyDirectory,
                cryptoKeysRelativeDirectory);

            return cryptoKeysAbsoluteDirectory;
        }
    }
}
