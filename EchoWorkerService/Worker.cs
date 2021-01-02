using Data;
using Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessingService.Models;
using ProcessingService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        /// <summary>
        /// interval (in milliseconds) between running of app
        /// </summary>
        private readonly int _intervalBetweenPing;
        /// <summary>
        /// Number of service workers that will run simultaneously
        /// </summary>
        private int NumberOfWorkers { get; set; } = 100;

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
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task StartWork()
        {
            List<Task> tasks = new List<Task>();
            CreateTasks(tasks);
            await WaitForProcessResults(tasks);
        }

        private async Task WaitForProcessResults(List<Task> tasks)
        {
            bool notDone = true;
            while (notDone)
            {
                notDone = tasks.All(x => x.IsCompleted);
                _logger.LogInformation("Waiting for tasks to finish, number left: " + tasks.Count, DateTimeOffset.Now);
                await Task.Delay(60000);
            }
        }

        private void AddToDatabase(HttpResult result)
        {
            if (Object.Equals(result,null))
                return;

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UptimeContext>();
            dbContext.HttpResult.Add(result);
            dbContext.SaveChanges();
        }

        private List<EndPoint> GetEndPoints()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UptimeContext>();
            return dbContext.EndPoint.ToList();
        }

        private void CreateTasks(List<Task> tasks)
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
                            var http = scope.ServiceProvider.GetRequiredService<HttpService>();
                            http.SetHostName(ep.Ip);
                            HttpResult result = MapResult(http.CheckConnection(), ep);
                            AddToDatabase(result);
                        }));
            }
        }

        private HttpResult MapResult(ResponseResult result, EndPoint ep)
        {
            if (Object.Equals(result, null)) return null;

            HttpResult http = new HttpResult()
            {
                EndPointId = ep.Id,
                Latency = result.Latency,
                IsReachable = result.IsReachable,
                StatusMessage = result.StatusMessage,
                TimeStamp = DateTime.Now
            };
            return http;
        }
    }
}
