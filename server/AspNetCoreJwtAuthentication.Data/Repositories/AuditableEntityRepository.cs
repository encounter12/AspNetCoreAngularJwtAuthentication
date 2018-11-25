using System;
using System.Linq;

using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Data.Repositories.Contracts;
using AspNetCoreJwtAuthentication.Models.SystemModels;

namespace AspNetCoreJwtAuthentication.Data.Repositories
{
    public class AuditableEntityRepository<T> : GenericRepository<T>, IAuditableEntityRepository<T> where T :
        class, IAuditableEntity
    {
        public AuditableEntityRepository(IApplicationDbContext context) : base(context)
        {
        }

        public override IQueryable<T> All()
        {
            return base.All().Where(x => !x.Deleted);
        }

        public IQueryable<T> AllWithDeleted()
        {
            return base.All();
        }

        public override void Add(T entity)
        {
            entity.CreatedOn = DateTime.Now;
            base.DbSet.Add(entity);
        }

        public override void Update(T entity)
        {
            entity.ModifiedOn = DateTime.Now;
            base.DbSet.Update(entity);
        }

        public override void Delete(T entity)
        {
            entity.Deleted = true;
            entity.DeletedOn = DateTime.Now;
            base.Update(entity);
        }

        public void HardDelete(T entity)
        {
            base.Delete(entity);
        }
    }
}
