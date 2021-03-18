using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.OptionsConfigModels
{
    public class RedisConnection
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 默认DB的ID
        /// </summary>
        public string DatabaseId { get; set; }

        /// <summary>
        /// Key的前缀
        /// </summary>
        public string Prefix { get; set; }
    }
}
