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
    /// 创建迁移：1.Add-Migration Init -Context DrypointIdentityServerDbContext
    ///         2.Add-Migration Init_ConfigurationDbContext -Context ConfigurationDbContext
    ///         3.Add-Migration Init_PersistedGrantDbContext -Context PersistedGrantDbContext
    /// 同步数据库：1.Update-Database -Context DrypointIdentityServerDbContext
    ///           2.Update-Database -Context ConfigurationDbContext
    ///           3.Update-Database -Context PersistedGrantDbContext
    /// </summary>
    public class DrypointIdentityServerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {

        public DrypointIdentityServerDbContext(DbContextOptions<DrypointIdentityServerDbContext> options)
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
