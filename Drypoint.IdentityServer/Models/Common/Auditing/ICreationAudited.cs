using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.IdentityServer.Models.Common.Auditing
{
    /// <summary>
    /// 包含创建操作用户的Id 和创建时间
    /// </summary>
    public interface ICreationAudited<TPrimaryKey>
    {
        /// <summary>
        /// 创建用户Id
        /// </summary>
        TPrimaryKey CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreationTime { get; set; }
    }

}
