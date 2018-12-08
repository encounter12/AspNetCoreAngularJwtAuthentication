using AspNetCoreJwtAuthentication.Models.InfrastructureModels;

namespace AspNetCoreJwtAuthentication.Services.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        public JwtSettings JwtSettings { get; private set; }

        public ConfigurationService(JwtSettings jwtSettings)
        {
            this.JwtSettings = jwtSettings;
        }
    }
}
