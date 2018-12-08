using System.IO;
using System.Reflection;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class CryptoRsaKeyProvider : ICryptoRsaKeyProvider
    {
        private readonly string cryptoKeysAbsoluteDirectory;

        public CryptoRsaKeyProvider()
        {
            this.cryptoKeysAbsoluteDirectory = this.GetCryptoKeysAbsoluteDirectory();
        }

        public string GetPrivateKey()
        {
            string privateKeyFileName = "privateKey.pem";
            string privateKeyFilePath = Path.Combine(this.cryptoKeysAbsoluteDirectory, privateKeyFileName);

            string privateKeyFileContent = File.ReadAllText(privateKeyFilePath);

            return privateKeyFileContent;
        }

        public string GetPublicKey()
        {
            string publicKeyFileName = "publicKey.pem";
            string publicKeyFilePath = Path.Combine(this.cryptoKeysAbsoluteDirectory, publicKeyFileName);

            string publicKeyFileContent = File.ReadAllText(publicKeyFilePath);

            return publicKeyFileContent;
        }

        private string GetCryptoKeysAbsoluteDirectory()
        {
            string cryptoKeysRelativeDirectory = @"Keys/RSA";

            var currentAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string cryptoKeysAbsoluteDirectory = Path.Combine(
                currentAssemblyDirectory,
                cryptoKeysRelativeDirectory);

            return cryptoKeysAbsoluteDirectory;
        }
    }
}
