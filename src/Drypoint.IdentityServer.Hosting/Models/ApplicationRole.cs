using Drypoint.IdentityServer.Hosting.Models.Common;
using Drypoint.IdentityServer.Hosting.Models.Common.Auditing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Hosting.Models
{
    public class ApplicationRole : IdentityRole<int>, ISoftDelete, IPassivable, IFullAudited<int?>
    {
        public string Description { get; set; }
        public int? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public int? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }

    }
}
