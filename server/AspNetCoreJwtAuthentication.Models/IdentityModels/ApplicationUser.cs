using System;
using AspNetCoreJwtAuthentication.Models.SystemModels;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreJwtAuthentication.Models.IdentityModels
{
    public class ApplicationUser : IdentityUser, IAuditableEntity
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
