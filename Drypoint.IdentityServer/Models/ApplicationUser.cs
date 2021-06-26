using Drypoint.IdentityServer.Models.Common;
using Drypoint.IdentityServer.Models.Common.Auditing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Models
{
    public class ApplicationUser : IdentityUser<long>,ISoftDelete,IPassivable,IFullAudited<long?>
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
        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? ModifierUserId { get; set; }
        public DateTime? ModificationTime { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
