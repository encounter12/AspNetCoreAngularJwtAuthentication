using AspNetCoreJwtAuthentication.Models.DomainModels;

namespace AspNetCoreJwtAuthentication.Data.Repositories.Contracts
{
    public interface IReportRepository : IAuditableEntityRepository<Report>
    {
        Report GetReportById(string id);
    }
}
