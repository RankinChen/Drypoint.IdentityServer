using Drypoint.IdentityServer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "Drypoint.IdentityServer";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File(
                        "Logs/log.txt",
                        shared: true,
                        restrictedToMinimumLevel: LogEventLevel.Warning,
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 5242880,
                        rollOnFileSizeLimit: true,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();

            await host.RunWithTasksAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.AllowSynchronousIO = true;//????ͬ?? IO
                    })
                    .ConfigureLogging((hostingContext, builder) =>
                    {
                        builder.ClearProviders();
                        builder.SetMinimumLevel(LogLevel.Trace);
                        builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        builder.AddConsole();
                        builder.AddDebug();
                    }).ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        //var env = hostingContext.HostingEnvironment;
                        //???ݻ??????????ز?ͬ??JSON????
                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                                optional: true, reloadOnChange: true)
                            .AddUserSecrets<Program>();
                        //?ӻ???????????????
                        //config.AddEnvironmentVariables("DOTNET_");

                    })
                    .UseStartup<Startup>();
                });
    }
}
