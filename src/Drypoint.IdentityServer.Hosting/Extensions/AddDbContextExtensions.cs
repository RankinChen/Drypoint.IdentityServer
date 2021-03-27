using Drypoint.IdentityServer.Hosting.Data;
using Drypoint.IdentityServer.Hosting.Models;
using Drypoint.IdentityServer.Hosting.ToolKit;
using Drypoint.IdentityServer.Hosting.ToolKit.EnumCollection;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Drypoint.IdentityServer.Hosting.Extensions
{
    public static class AddDbContextExtensions
    {
        public static void AddDrypointIdentityServerDbContext(this IServiceCollection services, IConfiguration configuration, DBCategoryEnum dbCategory)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            if (dbCategory == DBCategoryEnum.PostgreSQL)
            {
                services.AddDbContext<DrypointIdentityServerDbContext>(o =>
                {
                    o.UseNpgsql(configuration.GetConnectionString(DrypointConst.ConnectionStringName_PostgreSQL), sql => sql.MigrationsAssembly(migrationsAssembly));
                });
                services.AddDbContext<ConfigurationDbContext>(o =>
                {
                    o.UseNpgsql(configuration.GetConnectionString(DrypointConst.ConnectionStringName_PostgreSQL), sql => sql.MigrationsAssembly(migrationsAssembly));
                });
                services.AddDbContext<PersistedGrantDbContext>(o =>
                {
                    o.UseNpgsql(configuration.GetConnectionString(DrypointConst.ConnectionStringName_PostgreSQL), sql => sql.MigrationsAssembly(migrationsAssembly));
                });
            }
            else if (dbCategory == DBCategoryEnum.SQLServer)
            {
                services.AddDbContext<DrypointIdentityServerDbContext>(o =>
                {
                    o.UseSqlServer(configuration.GetConnectionString(DrypointConst.ConnectionStringName_Default), sql => sql.MigrationsAssembly(migrationsAssembly));
                });
                services.AddDbContext<ConfigurationDbContext>(o =>
                {
                    o.UseSqlServer(configuration.GetConnectionString(DrypointConst.ConnectionStringName_Default), sql => sql.MigrationsAssembly(migrationsAssembly));
                });
                services.AddDbContext<PersistedGrantDbContext>(o =>
                {
                    o.UseSqlServer(configuration.GetConnectionString(DrypointConst.ConnectionStringName_Default), sql => sql.MigrationsAssembly(migrationsAssembly));
                });
            }
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User = new UserOptions
                {
                    RequireUniqueEmail = true, //要求Email唯一
                    AllowedUserNameCharacters = null //允许的用户名字符
                };
                options.Password = new PasswordOptions
                {
                    RequiredLength = 8, //要求密码最小长度，默认是 6 个字符
                    RequireDigit = true, //要求有数字
                    RequiredUniqueChars = 3, //要求至少要出现的字母数
                    RequireLowercase = true, //要求小写字母
                    RequireNonAlphanumeric = false, //要求特殊字符
                    RequireUppercase = false //要求大写字母
                };

                //options.Lockout = new LockoutOptions
                //{
                //    AllowedForNewUsers = true, // 新用户锁定账户
                //    DefaultLockoutTimeSpan = TimeSpan.FromHours(1), //锁定时长，默认是 5 分钟
                //    MaxFailedAccessAttempts = 3 //登录错误最大尝试次数，默认 5 次
                //};
                //options.SignIn = new SignInOptions
                //{
                //    RequireConfirmedEmail = true, //要求激活邮箱
                //    RequireConfirmedPhoneNumber = true //要求激活手机号
                //};
                //options.ClaimsIdentity = new ClaimsIdentityOptions
                //{
                //    // 这里都是修改相应的Cliams声明的
                //    RoleClaimType = "IdentityRole",
                //    UserIdClaimType = "IdentityId",
                //    SecurityStampClaimType = "SecurityStamp",
                //    UserNameClaimType = "IdentityName"
                //};
            })
            .AddEntityFrameworkStores<DrypointIdentityServerDbContext>()
            .AddDefaultTokenProviders();
        }
    }


}
