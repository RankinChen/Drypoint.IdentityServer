using Drypoint.IdentityServer.Hosting.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Hosting.Data
{
    /// <summary>
    /// 获取所有可用DbContext：Get-DbContext
    /// 创建迁移：1.Add-Migration Init -Context DrypointDbContext -OutputDir Data\Migrations
    ///         2.Add-Migration Init_ConfigurationDbContext  -Context ConfigurationDbContext -OutputDir Data\Migrations\IdentityServer\ConfigurationDb
    ///         3.Add-Migration Init_PersistedGrantDbContext -Context PersistedGrantDbContext -OutputDir Data\Migrations\IdentityServer\PersistedGrantDb
    /// 同步数据库：1.Update-Database -Context DrypointDbContext
    ///           2.Update-Database -Context ConfigurationDbContext
    ///           3.Update-Database -Context PersistedGrantDbContext
    ///           
    ///可以使用dotnet ef 工具 参考：https://docs.microsoft.com/zh-cn/ef/core/cli/dotnet
    /// 1.全局安装 dotnet tool install --global dotnet-ef
    ///         （dotnet tool update --global dotnet-ef）
    /// </summary>
    public class DrypointDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {

        public DrypointDbContext(DbContextOptions<DrypointDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //指定表名
            //builder.Entity<ApplicationRole>().ToTable("DrypointRole");
        }
    }
}
