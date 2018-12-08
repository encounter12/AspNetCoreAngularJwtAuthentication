namespace AspNetCoreJwtAuthentication.Middleware
{
    public interface ICryptoHmacShaKeyProvider
    {
        string GetSecretKey();
    }
}
