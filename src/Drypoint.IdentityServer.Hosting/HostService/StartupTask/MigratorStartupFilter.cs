using Drypoint.IdentityServer.Hosting.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Hosting.HostService.StartupTask
{
    public class MigratorStartupFilter : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;
        public MigratorStartupFilter(IServiceProvider serviceProvider)
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
            using var scope = _serviceProvider.CreateScope();

            var drypointDbContext = scope.ServiceProvider.GetRequiredService<DrypointIdentityServerDbContext>();

            await drypointDbContext.Database.MigrateAsync();
        }
    }
}
