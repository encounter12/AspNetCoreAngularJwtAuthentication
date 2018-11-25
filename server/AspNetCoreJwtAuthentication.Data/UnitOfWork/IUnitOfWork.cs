using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Data.Repositories.Contracts;

namespace AspNetCoreJwtAuthentication.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IApplicationDbContext ApplicationDbContext { get; }

        IApplicationUserRepository ApplicationUserRepository { get; }

        IReportRepository ReportRepository { get; }
    }
}
