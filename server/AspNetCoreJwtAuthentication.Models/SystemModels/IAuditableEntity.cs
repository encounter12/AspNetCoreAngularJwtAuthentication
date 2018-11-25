namespace AspNetCoreJwtAuthentication.Models.SystemModels
{
    using System;

    public interface IAuditableEntity
    {
        DateTime? CreatedOn { get; set; }

        string CreatedBy { get; set; }

        DateTime? ModifiedOn { get; set; }

        string ModifiedBy { get; set; }

        bool Deleted { get; set; }

        DateTime? DeletedOn { get; set; }

        string DeletedBy { get; set; }
    }
}
