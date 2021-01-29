
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessingService.BusinessLogic.Protocols;
using ProcessingService.DTO;
using ProcessingService.Models;
using ProcessingService.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger log;
        private readonly IDatabaseService db;
        private readonly ProtocolFactory factory;
        private readonly ProtocolHandler handler;
        /// <summary>
        /// interval (in milliseconds) between running of app
        /// </summary>
        private readonly int _intervalBetweenPing = 60000;
        /// <summary>
        /// Number of service workers that will run simultaneously
        /// </summary>
        private int NumberOfWorkers { get; set; } = 5000;
        private static List<EndPointExtended> newEndPointsAdded;

        public Worker(ILogger<Worker> logger, ProtocolFactory factory, ProtocolHandler handler, IDatabaseService db)
        {
            log = logger;
            this.db = db;
            this.factory = factory;
            this.handler = handler;
            newEndPointsAdded = new List<EndPointExtended>();
            using System.Timers.Timer timer = new System.Timers.Timer(30000)
            {
                AutoReset = true
            };
            timer.Elapsed += Timer_Elapsed;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task find = new Task(() => { FindNewEndPoints(handler); }, TaskCreationOptions.PreferFairness);
            Task process = new Task(() => { ProcessResults(handler); }, TaskCreationOptions.PreferFairness);
            log.LogTrace("Worker running at: {time}", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                List<EndPointExtended> data = db.GetAll();
                List<IProtocol> tasks = factory.GetProtocols(data);
                handler.AddRange(tasks);
                if (!handler.IsRunning)
                {
                    handler.Start();
                    find.Start();
                    process.Start();
                }

                await Task.Delay(_intervalBetweenPing);
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            newEndPointsAdded.Clear();
        }

        private async Task ProcessResults(ProtocolHandler handler)
        {
            while (handler.IsRunning)
            {
                Queue<TaskResultDTO> results = handler.GetAndClearResults();
                if (results.Count > 0)
                {
                    log.LogInformation($"{DateTime.UtcNow} - adding {results.Count} records to db");
                    while(results.Count > 0)
                    {
                        try
                        {
                            db.Create(new ResultData(results.Dequeue().Response));
                        }
                        catch (Exception e)
                        {
                            log.LogError("Error adding to DB: " + e.Message);
                        }
                    }
                }
                await Task.Delay(1000);
            }
        }

        private async Task FindNewEndPoints(ProtocolHandler handler)
        {
            while (handler.IsRunning)
            {
                try
                {
                    var endpoints = db.FindNewEndpoints();
                    List<EndPointExtended> toBeAdded = new List<EndPointExtended>();
                    foreach (var ep in endpoints)
                    {
                        if (!newEndPointsAdded.Exists(x => x.Id.Equals(ep.Id)))
                        {
                            newEndPointsAdded.Add(ep);
                            toBeAdded.Add(ep);
                        }
                    }
                    List<IProtocol> newTasks = factory.GetProtocols(toBeAdded);
                    handler.AddRange(newTasks);
                }
                catch (Exception e)
                {
                    log.LogError("Error finding new endpoings in DB: " + e.Message);
                }
                await Task.Delay(4000);
            }
        }
    }
}
