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
        private readonly IDatabaseService _db;
        private readonly IProcessor _proc;
        private readonly IMapService _map;
        /// <summary>
        /// interval (in milliseconds) between running of app
        /// </summary>
        private readonly int _intervalBetweenPing = 60000;
        /// <summary>
        /// Number of service workers that will run simultaneously
        /// </summary>
        private int NumberOfWorkers { get; set; } = 5000;

        public Worker(ILogger<Worker> logger,IProcessor processor, IDatabaseService db, IMapService map)
        {
            _logger = logger;
            _proc = processor;
            _db = db;
            _map = map;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await StartWork();
                _logger.LogInformation("Worker done at: {time}", DateTimeOffset.Now);
                var task = Task.Delay(_intervalBetweenPing);
                await CheckForNewEndPoints(task);
            }
        }


        private async Task StartWork()
        {
            List<Task> tasks = new List<Task>();
            CreateTasks(tasks);
            await MonitorTaskCompletion(tasks);
        }

        /// <summary>
        /// monitors task completions and returns once done.
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        private async Task MonitorTaskCompletion(List<Task> tasks)
        {
            bool notDone = true;
            while (notDone)
            {
                notDone = tasks.All(x => x.IsCompleted);
                _logger.LogInformation("Waiting for tasks to finish, number left: " + tasks.Count, DateTimeOffset.Now);

                await Task.Delay(1000);
            }
        }
        /// <summary>
        /// creates tasks on seperate threads as they become avaliable
        /// </summary>
        /// <param name="tasks"></param>
        private async void CreateTasks(List<Task> tasks)
        {
            List<EndPoint> data = _db.Get();
            _logger.LogInformation("Endpoint count: " + data.Count, DateTimeOffset.Now);
            foreach (var ep in data)
            {
                if (Object.Equals(ep, null) || Object.Equals(ep.Ip, null))
                    continue;

                int taskCount = tasks.Count;
                while (taskCount >= NumberOfWorkers)
                {
                    await Task.Delay(1000);
                    tasks.RemoveAll(x => x.IsCompleted);
                    taskCount = tasks.Count;
                }
                if (tasks.Count < NumberOfWorkers)
                        tasks.Add(Task.Run(() =>
                        {
                            CheckEndPointConnection(ep);
                        }));
            }
        }
        /// <summary>
        /// Checks for newly added endpoints, if found, makes a request and adds to database
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private async Task CheckForNewEndPoints(Task task)
        {
            while (!task.IsCompleted)
            {
                var eps = await _db.FindNewEndpoints();
                if (eps.Count() > 0)
                {
                    _logger.LogInformation("New EndPoint(s) added, checking connection", DateTimeOffset.Now);
                    foreach (EndPoint ep in eps)
                        CheckEndPointConnection(ep);
                    await Task.Delay(1000); 
                }

            }
        }
        /// <summary>
        /// contacts the endpoint and stores the response in the database
        /// </summary>
        /// <param name="ep"></param>
        private void CheckEndPointConnection(EndPoint ep)
        {
            ResponseResult res = _proc.CheckConnection(ep);
            HttpResult result = _map.Map(res);
            _db.Create(result);
        }
    }
}
