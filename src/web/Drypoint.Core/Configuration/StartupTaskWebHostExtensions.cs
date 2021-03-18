using Drypoint.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drypoint.Core.Configuration
{
    public static class StartupTaskWebHostExtensions
    {
        public static async Task RunWithTasksAsync(this IHost host, CancellationToken cancellationToken = default)
        {
            using var scope = host.Services.CreateScope();

            var myDbContext = scope.ServiceProvider.GetRequiredService<DrypointDbContext>();

            await myDbContext.Database.MigrateAsync();

            await host.RunAsync(cancellationToken);
        }
    }
}
