namespace AspNetCoreJwtAuthentication.Models.InfrastructureModels
{
    public class JwtSettings
    {
        public string Path { get; set; }

        public string HmacSha256Key { get; set; }

        public string Issuer { get; set; }

        public int ExpiresIn { get; set; }

        public string Alg { get; set; }
    }
}
