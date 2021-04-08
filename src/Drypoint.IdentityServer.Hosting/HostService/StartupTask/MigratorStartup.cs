using Drypoint.IdentityServer.Hosting.Data;
using Drypoint.IdentityServer.Hosting.Data.InitData;
using Drypoint.IdentityServer.Hosting.Models;
using Drypoint.IdentityServer.Hosting.ToolKit;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Hosting.HostService.StartupTask
{
    public class MigratorStartup : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;
        public MigratorStartup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 执行Migrage
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope(); ;
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            if (configuration.GetValue<bool>("Seed"))
            {
                var drypointDbContext = scope.ServiceProvider.GetRequiredService<DrypointDbContext>();
                await drypointDbContext.Database.MigrateAsync();

                var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                await persistedGrantDbContext.Database.MigrateAsync();

                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                await configurationDbContext.Database.MigrateAsync();

                Console.WriteLine("Done seeding database.");
                Console.WriteLine();
                await EnsureSeedDataAsync();
            }
        }

        private async Task EnsureSeedDataAsync()
        {
            await EnsureSeedDataPersistedGrantAsync();
            await EnsureSeedDataUserAndRoleAsync();
        }

        private async Task EnsureSeedDataUserAndRoleAsync()
        {
            var scope = _serviceProvider.CreateScope();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            foreach (var role in IdentityServerInitData.GetTestRole())
            {
                var existRole = await roleMgr.FindByNameAsync(role.Name);
                if (existRole == null)
                {
                    var result = await roleMgr.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    else
                    {
                        Console.WriteLine($"{role.Name} created");
                    }
                }
                else
                {
                    Console.WriteLine($"{existRole.Name} role already exists");
                }
            }
            foreach (var user in IdentityServerInitData.GetTestUser())
            {
                if (!string.IsNullOrEmpty(user.LoginName))
                {
                    var existUser = await userMgr.FindByNameAsync(user.LoginName);

                    if (existUser == null)
                    {
                        user.UserRoles = IdentityServerInitData.GetTestUserRole().Where(x => x.UserId == user.Id).ToList();

                        var result = await userMgr.CreateAsync(user, user.PasswordHash);//正常PasswordHash不应该是明文
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        var claims = new List<Claim>{
                                    new Claim(JwtClaimTypes.Name, user.RealName),
                                    new Claim(JwtClaimTypes.Email, $"{user.LoginName}@email.com"),
                                };
                        var roleIds = IdentityServerInitData.GetTestRole().Where(s => user.UserRoles.Select(x => x.RoleId).Contains(s.Id));
                        claims.AddRange(user.UserRoles.Select(s => new Claim(JwtClaimTypes.Role, s.RoleId.ToString())));
                        claims.AddRange(roleIds.Select(s => new Claim(DrypointConsts.RolesNameScope, s.Name)));

                        result = await userMgr.AddClaimsAsync(user, claims);

                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        else
                        {
                            Console.WriteLine($"{user.LoginName} created");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{user.LoginName} user already exists");
                    }
                }
            }
        }

        private async Task EnsureSeedDataPersistedGrantAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            if (!context.Clients.Any())
            {
                Console.WriteLine("Clients being populated");
                foreach (var client in IdentityServerInitData.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("IdentityResources being populated");
                foreach (var resource in IdentityServerInitData.GetIdentityResources().ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("IdentityResources already populated");
            }

            if (!context.ApiResources.Any())
            {
                Console.WriteLine("ApiResources being populated");
                foreach (var resource in IdentityServerInitData.GetApiResources().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("ApiResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Console.WriteLine("ApiScopes being populated");
                foreach (var resource in IdentityServerInitData.GetApiScopes().ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("ApiScopes already populated");
            }
        }
    }
}
