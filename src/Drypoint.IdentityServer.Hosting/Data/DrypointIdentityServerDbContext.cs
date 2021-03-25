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
    /// 创建迁移：Add-Migration Init -Context DrypointIdentityServerDbContext
    /// 同步数据库：Update-Database -Context DrypointIdentityServerDbContext
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
