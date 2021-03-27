using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Hosting.HostService
{
    public class InitDataService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public readonly ILogger<InitDataService> _logger;
        public readonly IHostEnvironment _hostEnvironment;

        public InitDataService(
            IServiceProvider serviceProvider,
            ILogger<InitDataService> logger,
            IHostEnvironment hostEnvironment)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    await Task.Factory.StartNew( () =>
                    {
                        var filePath = _hostEnvironment.ContentRootPath;
                        filePath = Path.Combine(filePath, "config");
                        if (Directory.Exists(filePath))
                        {
                            foreach (var dirPath in Directory.GetDirectories(filePath))
                            {

                            }
                        }
                    }, TaskCreationOptions.LongRunning);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"InitDataService Exception：{ex}");
                    throw;
                }
            }

            //定时执行
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("InitDataService 后台同步服务正在执行");
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        [DebuggerStepThrough]
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("InitDataService 后台同步服务启动");
            return base.StartAsync(cancellationToken);
        }

        [DebuggerStepThrough]
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("InitDataService 后台同步服务停止");
            return base.StopAsync(cancellationToken);
        }
    }
}
