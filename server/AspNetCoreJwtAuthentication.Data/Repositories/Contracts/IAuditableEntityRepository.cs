namespace AspNetCoreJwtAuthentication.Data.Repositories.Contracts
{
    using System.Linq;

    public interface IAuditableEntityRepository<T> : IGenericRepository<T> where T : class
    {
        IQueryable<T> AllWithDeleted();

        void HardDelete(T entity);
    }
}
