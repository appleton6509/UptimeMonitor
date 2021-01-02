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

        /// <summary>
        /// Number of service workers that will run simultaneously
        /// </summary>
        private int NumberOfWorkers { get; set; } = 5;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scope)
        {
            _logger = logger;
            _scopeFactory = scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await StartWork();
                _logger.LogInformation("Worker done at: {time}", DateTimeOffset.Now);
                await Task.Delay(60000, stoppingToken);
            }
        }

        private async Task StartWork()
        {
            List<Task> tasks = new List<Task>();
            List<Echo> results = new List<Echo>();
            CreateTasks(tasks, results);
            await WaitForProcessResults(tasks,results);
        }

        private async Task WaitForProcessResults(List<Task> tasks, List<Echo> results)
        {
            int taskCount = tasks.Count;
            while (taskCount > 0)
            {
                tasks.RemoveAll(x => x.IsCompleted);
                _logger.LogInformation("Waiting for tasks to finish, number left: " + taskCount, DateTimeOffset.Now);
                await Task.Delay(1000);
                taskCount = tasks.Count;
            }
            int resultCount = results.Count;
            while (resultCount > 0)
            {
                _logger.LogInformation("Waiting for results to finish processing, number left: "+ resultCount, DateTimeOffset.Now);
                AddToDatabase(results);
                await Task.Delay(1000);
                resultCount = results.Count;
            }
        }

        private void AddToDatabase(List<Echo> results)
        {
            if (results.Count <= 0)
                return;

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UptimeContext>();
            Echo[] copy = results.ToArray();
            dbContext.AddRange(copy);
            dbContext.SaveChanges();
            results.RemoveAll(x => copy.Contains(x));
        }

        private List<EndPoint> GetEndPoints()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UptimeContext>();
            return dbContext.EndPoint.ToList();
        }

        private void CreateTasks(List<Task> tasks, List<Echo> results)
        {
            List<EndPoint> data = GetEndPoints();
            _logger.LogInformation("Endpoint count: " + data.Count, DateTimeOffset.Now);
            foreach (var ep in data)
            {
                if (Object.Equals(ep, null) || Object.Equals(ep.Ip, null))
                    continue;

                int taskCount = tasks.Count;
                while (taskCount >= NumberOfWorkers)
                {
                    tasks.RemoveAll(x => x.IsCompleted);
                    taskCount = tasks.Count;
                }
                if (tasks.Count < NumberOfWorkers)
                    tasks.Add(Task.Run(() =>
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var ping = scope.ServiceProvider.GetRequiredService<PingService>();
                        ping.AddressOrIp = ep.Ip;
                        results.Add(MapResult(ping.StartPingAsync().Result, ep));
                    }));
                AddToDatabase(results);
            }
        }

        private Echo MapResult(PingResult result, EndPoint ep)
        {
            if (Object.Equals(result, null)) return null;

            Echo echo = new Echo()
            {
                EndPointId = ep.Id,
                Latency = result.Latency,
                StatusCode = result.StatusCode,
                StatusMessage = (result.ErrorMessage),
                TimeStamp = DateTime.Now
            };
            return echo;
        }
    }
}
