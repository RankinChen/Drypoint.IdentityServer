using Drypoint.IdentityServer.Hosting.Models.Common;
using Drypoint.IdentityServer.Hosting.Models.Common.Auditing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Hosting.Models
{
    public class ApplicationUser : IdentityUser<int>,ISoftDelete,IPassivable,IFullAudited<int?>
    {
        public string LoginName { get; set; }

        public string RealName { get; set; }
        /// <summary>
        /// 0：女 1：男
        /// </summary>
        public int Sex { get; set; } = 0;

        public int Age { get; set; }

        public DateTime Birthday { get; set; } = DateTime.Now;

        public string Address { get; set; }

       /// <summary>
       /// 是否删除
       /// </summary>
        public bool IsDeleted { get; set; }
        public int? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public int? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
