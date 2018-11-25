using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Data.Repositories.Contracts;
using AspNetCoreJwtAuthentication.Models.DomainModels;

namespace AspNetCoreJwtAuthentication.Data.Repositories
{
    public class ReportRepository : AuditableEntityRepository<Report>, IReportRepository
    {
        public ReportRepository(IApplicationDbContext context) : base(context)
        {
        }

        public Report GetReportById(string id)
        {
            Report report = this.GetById(id);
            return report;
        }
    }
}
