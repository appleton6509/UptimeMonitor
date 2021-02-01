
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessingService.BusinessLogic;
using ProcessingService.BusinessLogic.Protocols;
using ProcessingService.DTO;
using ProcessingService.Models;
using ProcessingService.Services;
using ProcessingService.Services.Email;
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
        private readonly ProtocolFactory factory;
        private readonly ProtocolHandler handler;
        private readonly ResultProcessor processor;
        private readonly IDatabaseService db;
        /// <summary>
        /// interval (in milliseconds) between running of app
        /// </summary>
        private readonly int _intervalBetweenPing = 60000;



        public Worker(ILogger<Worker> logger, ProtocolFactory factory, ProtocolHandler handler,
            IDatabaseService db , ResultProcessor processor)
        {
            log = logger;
            this.factory = factory;
            this.handler = handler;
            this.db = db;
            this.processor = processor;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            log.LogTrace("Worker running at: {time}", DateTimeOffset.Now);
            Task findNewlyAdded = new Task( () =>  {   FindNewEndPoints(); }, TaskCreationOptions.LongRunning);
            Task processResults = new Task( () => {  ProcessEndPoints();  }, TaskCreationOptions.LongRunning);

            while (!stoppingToken.IsCancellationRequested)
            {
                List<EndPointExtended> data = db.GetAll();
                List<IProtocol> tasks = factory.MapToProtocols(data);
                handler.AddRange(tasks);
                if (!handler.IsRunning)
                {
                    handler.Start();
                    findNewlyAdded.Start();
                    processResults.Start();
                }

                await Task.Delay(_intervalBetweenPing, stoppingToken);
            }
        }

        private async Task ProcessEndPoints()
        {
            while (handler.IsRunning)
            {
                processor.Process(handler.RetrieveResults());
                await Task.Delay(1000);
            }
        }

        private async Task FindNewEndPoints()
        {
            while (handler.IsRunning)
            {
                try
                {
                    var endpoints = db.FindNewEndpoints();
                    var protocols = factory.MapToProtocols(endpoints);
                    Queue<TaskResultDTO> queue = new Queue<TaskResultDTO>();
                    foreach (var pro in protocols)
                    {
                        var result = handler.ExecuteImmediately(pro);
                        queue.Enqueue(result);
                    }
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
