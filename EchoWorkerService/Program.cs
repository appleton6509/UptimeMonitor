using Data;
using EchoWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EchoWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    var options = new DbContextOptionsBuilder<UptimeContext>();
                    options.UseSqlServer(hostContext.Configuration.GetSection("ConnectionStrings")["Development"]);
                    services.AddScoped(s => new UptimeContext(options.Options));
                    services.AddScoped(s => new PingService());
                });

    }
}

