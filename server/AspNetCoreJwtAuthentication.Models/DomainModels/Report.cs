using AspNetCoreJwtAuthentication.Models.SystemModels;

namespace AspNetCoreJwtAuthentication.Models.DomainModels
{
    public class Report : AuditableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
