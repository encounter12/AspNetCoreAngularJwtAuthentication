using System;

namespace AspNetCoreJwtAuthentication.Models.SystemModels
{
    public class AuditableEntity: IAuditableEntity
    {
        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string DeletedBy { get; set; }
    }
}
