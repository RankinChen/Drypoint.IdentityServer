using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.IdentityServer.Hosting.Models.Common.Auditing
{ 
    /// <summary>
    /// 包含删除操作用户的Id 和删除时间
    /// </summary>
public interface IDeletionAudited<TPrimaryKey>
    {
        /// <summary>
        /// 删除用户Id
        /// </summary>
        TPrimaryKey DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? DeletionTime { get; set; }

    }

}
