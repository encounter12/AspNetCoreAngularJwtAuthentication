using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Data.Repositories.Contracts;
using AspNetCoreJwtAuthentication.Models.IdentityModels;

namespace AspNetCoreJwtAuthentication.Data.Repositories
{
    public class ApplicationUserRepository : AuditableEntityRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly IApplicationDbContext applicationDbContext;

        public ApplicationUserRepository(IApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public ApplicationDbContext ApplicationDbContext
        {
            get
            {
                return (ApplicationDbContext)this.applicationDbContext;
            }
        }
    }
}
