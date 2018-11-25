namespace AspNetCoreJwtAuthentication.Data.Repositories.Contracts
{
    using AspNetCoreJwtAuthentication.Data.Context;
    using AspNetCoreJwtAuthentication.Models.IdentityModels;

    public interface IApplicationUserRepository : IAuditableEntityRepository<ApplicationUser>
    {
        ApplicationDbContext ApplicationDbContext { get; }
    }
}
