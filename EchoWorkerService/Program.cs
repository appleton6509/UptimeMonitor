using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessingService.Services;

namespace ProcessingService
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

                    var hosting = services.BuildServiceProvider().GetService<IHostEnvironment>();
                    var options = new DbContextOptionsBuilder<UptimeContext>();

                    if (hosting.IsDevelopment())
                        options.UseSqlServer(hostContext.Configuration.GetSection("ConnectionStrings")["Development"]);
                    else
                        options.UseSqlServer(hostContext.Configuration.GetSection("ConnectionStrings")["Production"]);
                   
                    services.AddScoped(s => new UptimeContext(options.Options));
                    services.AddScoped(s => new HttpService());
                });

    }
}

