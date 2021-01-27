
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessingService.Models;
using ProcessingService.Services;
using ProcessingService.Services.TaskManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDatabaseService _db;
        private readonly IHttpService _http;
        /// <summary>
        /// interval (in milliseconds) between running of app
        /// </summary>
        private readonly int _intervalBetweenPing = 60000;
        /// <summary>
        /// Number of service workers that will run simultaneously
        /// </summary>
        private int NumberOfWorkers { get; set; } = 5000;

        public Worker(ILogger<Worker> logger, IHttpService processor, IDatabaseService db)
        {
            _logger = logger;
            _http = processor;
            _db = db;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Worker running at: {time}", DateTimeOffset.Now);

                var data = _db.GetAll();
                ProtocolHandler handler = new ProtocolHandler(_logger);
                ProtocolFactory factory = new ProtocolFactory(_http);
                List<IProtocol> tasks = factory.GetTasks(data);
                handler.AddRange(tasks);
                handler.Start();
                Task delay = Task.Run(async () => await Task.Delay(_intervalBetweenPing));
                Task addNewEndPoints = Task.Run(() => FindNewEndPoints(handler, factory, delay));
                Task processResults = Task.Run(() => ProcessResults(handler, delay));
                delay.Wait();
                handler.Stop();
                addNewEndPoints.Wait();
                processResults.Wait();

            }
        }

        private async Task ProcessResults(ProtocolHandler handler, Task delay)
        {
            while (!delay.IsCompleted)
            {
                while (handler.ResultCount > 0)
                {
                    _logger.LogInformation($"adding {handler.ResultCount} records to db");
                    List<TaskResult> results = handler.GetAndClearResults();
                    results.ForEach(x =>
                    {
                        try
                        {
                            _db.Create(new HttpResult(x.Response));
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Error adding to DB: " + e.Message);
                        }
                    });
                    await Task.Delay(1000);
                }
            }
        }

        private async Task FindNewEndPoints(ProtocolHandler handler, ProtocolFactory factory, Task delay)
        {
            while (!delay.IsCompleted)
            {
                try
                {
                    List<IProtocol> newTasks = factory.GetTasks(_db.FindNewEndpoints());
                    if (newTasks.Count > 0)
                        handler.AddRange(newTasks);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error finding new endpoings in DB: " + e.Message);
                }
                await Task.Delay(2000);
            }
        }
    }
}
