using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.IdentityServer.Models.Common.Auditing
{    
    /// <summary>
     /// 包含更新操作用户的Id 和更新时间
     /// </summary>
    public interface IModificationAudited<TPrimaryKey>
    {
        /// <summary>
        /// 更新用户Id
        /// </summary>
        long? ModifierUserId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime? ModificationTime { get; set; }
    }
}
