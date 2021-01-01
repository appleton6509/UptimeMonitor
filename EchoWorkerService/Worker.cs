using Data;
using Data.Models;
using EchoWorkerService.Models;
using EchoWorkerService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private PingService _ping;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scope, PingService ping)
        {
            _logger = logger;
            _scopeFactory = scope;
            _ping = ping;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                PingHost();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private List<EndPoint> GetEndPoints()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UptimeContext>();
            return dbContext.EndPoint.ToList();
        }

        private void PingHost()
        {
            List<EndPoint> data = GetEndPoints();
            List<Task> tasks = new List<Task>();
            List<PingResult> results = new List<PingResult>();

            foreach (var ep in data)
            {
                if (tasks.Count < 6)
                    CreateTasks(ep,tasks,results);

                while(tasks.Count > 5)
                    RemoveCompletedTasks(tasks);
            }
        }

        private static void RemoveCompletedTasks(List<Task> tasks)
        {
            tasks.ForEach(x => { if (x.IsCompleted) tasks.Remove(x); });
        }

        private void CreateTasks(EndPoint ep,List<Task> tasks, List<PingResult> results)
        {
            tasks.Add(Task.Run(() =>
            {
                _ping.AddressOrIp = ep.Ip;
                PingResult result = _ping.StartPingAsync().Result;
                results.Add(result);
            }));
        }
    }
}
