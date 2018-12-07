namespace AspNetCoreJwtAuthentication.Middleware
{
    public interface ICryptoKeysProvider
    {
        string GetPrivateKey();

        string GetPublicKey();
    }
}
