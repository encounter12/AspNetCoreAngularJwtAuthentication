using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Data.Repositories;
using AspNetCoreJwtAuthentication.Data.Repositories.Contracts;

namespace AspNetCoreJwtAuthentication.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IApplicationUserRepository applicationUserRepository;

        private IReportRepository reportRepository;

        public UnitOfWork(IApplicationDbContext applicationDbContext)
        {
            this.ApplicationDbContext = applicationDbContext;
        }

        public IApplicationDbContext ApplicationDbContext { get; }

        public IApplicationUserRepository ApplicationUserRepository
        {
            get
            {
                if (this.applicationUserRepository == null)
                {
                    this.applicationUserRepository = new ApplicationUserRepository(this.ApplicationDbContext);
                }

                return this.applicationUserRepository;
            }
        }

        public IReportRepository ReportRepository
        {
            get
            {
                if (this.reportRepository == null)
                {
                    this.reportRepository = new ReportRepository(this.ApplicationDbContext);
                }

                return this.reportRepository;
            }
        }

        public void SaveChanges()
        {
            this.ApplicationDbContext.SaveChanges();
        }
    }
}
