using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.ToolKit
{
    public class DrypointConsts
    {
        #region 数据库连接字符串 Key
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public const string ConnectionStringName_Default = "Default";

        /// <summary>
        /// 数据库连接字符串（PostgreSQL）
        /// </summary>
        public const string ConnectionStringName_PostgreSQL = "PostgreSQL";
        #endregion

        #region 接口定义相关
        /// <summary>
        /// 接口前缀
        /// </summary>
        public const string ApiPrefix = "api/";

        /// <summary>
        /// admin 接口组名
        /// </summary>
        public const string AdminAPIGroupName = "admin";

        /// <summary>
        /// app 接口组名
        /// </summary>
        public const string AppAPIGroupName = "app";
        #endregion

        #region 自定义授权相关scope
        public const string RolesScope = "roles";
        public const string RolesNameScope = "rolename";
        #endregion

        /// <summary>
        /// 列表数据 分页 每页最多显示条数
        /// </summary>
        public const int MaxPageSize = 1000;

        /// <summary>
        /// 列表数据 每页条数
        /// </summary>
        public const int DefaultPageSize = 10;


        /// <summary>
        ///     Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public const string DefaultPassPhrase = "(*^▽^*)";

        public const string CacheKey_TokenValidityKey = "token_validity_key";

        //CORS策略名
        public const string LocalCorsPolicyName = "localhost";
    }
}
