using AutoMapper.Configuration;
using Drypoint.EntityFrameworkCore.EntityFrameworkCore;
using Drypoint.Unity;
using Drypoint.Unity.EnumCollection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Drypoint.Core.Configuration
{
    public static class DbContextConfigExtensions
    {
        public static void AddDbContextConfigurer(this IServiceCollection services, IConfiguration configuration, DBCategoryEnum dbCategory)
        {
            services.AddDbContext<DrypointDbContext>(o =>
            {
                if (dbCategory == DBCategoryEnum.PostgreSQL)
                {
                    o.UseNpgsql(configuration.GetConnectionString(DrypointConsts.ConnectionStringName_PostgreSQL));
                }
                else if (dbCategory == DBCategoryEnum.SQLServer)
                {
                    o.UseSqlServer(configuration.GetConnectionString(DrypointConsts.ConnectionStringName));
                }
            });

        }
    }
}
