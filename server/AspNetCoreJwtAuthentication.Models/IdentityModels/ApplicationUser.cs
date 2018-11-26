using System;
using Microsoft.AspNetCore.Identity;
using AspNetCoreJwtAuthentication.Models.SystemModels;

namespace AspNetCoreJwtAuthentication.Models.IdentityModels
{
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {
        public DateTime Birthdate { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string DeletedBy { get; set; }
    }
}
