using System;
using System.Linq;

namespace AspNetCoreJwtAuthentication.Data.Repositories.Contracts
{
    public interface IGenericRepository<T>
    {
        IQueryable<T> All();

        T GetById(Guid id);

        T GetById(int id);

        T GetById(string id);

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        void SaveChanges();
    }
}
