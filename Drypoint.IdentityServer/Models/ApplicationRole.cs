using Drypoint.IdentityServer.Models.Common;
using Drypoint.IdentityServer.Models.Common.Auditing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Models
{
    public class ApplicationRole : IdentityRole<long>, ISoftDelete, IPassivable, IFullAudited<long>
    {
        public string Description { get; set; }
        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? ModifierUserId { get; set; }
        public DateTime? ModificationTime { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
