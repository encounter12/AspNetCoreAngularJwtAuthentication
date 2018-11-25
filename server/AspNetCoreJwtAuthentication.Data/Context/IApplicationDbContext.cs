using AspNetCoreJwtAuthentication.Models.IdentityModels;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreJwtAuthentication.Data.Context
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> Users { get; set; }

        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
