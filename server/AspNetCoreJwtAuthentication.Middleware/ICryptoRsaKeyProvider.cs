namespace AspNetCoreJwtAuthentication.Middleware
{
    public interface ICryptoRsaKeyProvider
    {
        string GetPrivateKey();

        string GetPublicKey();
    }
}
