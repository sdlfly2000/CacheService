using Common.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System.Collections.Generic;

namespace CacheService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddMemoryCache();
                    DIModule.RegisterDomain(services, new List<string>
                    {
                        "CacheService",
                        "Application.Cache.Service",
                        "Infrastructure.cache.memory"
                    });
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.AddConfiguration();
                })
                .UseSystemd();
    }
}
