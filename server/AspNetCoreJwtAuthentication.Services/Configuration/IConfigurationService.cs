using AspNetCoreJwtAuthentication.Models.InfrastructureModels;

namespace AspNetCoreJwtAuthentication.Services.Configuration
{
    public interface IConfigurationService
    {
        JwtSettings JwtSettings { get; }
    }
}
