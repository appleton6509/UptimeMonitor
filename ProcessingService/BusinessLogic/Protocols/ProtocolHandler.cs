
using Microsoft.Extensions.Logging;
using ProcessingService.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessingService.BusinessLogic.Protocols
{
    public class ProtocolHandler
    {
        private Queue<Task<TaskResultDTO>> queued;
        private List<Task<TaskResultDTO>> running;
        private Queue<TaskResultDTO> finished;
        private ILogger<ProtocolHandler> log;
        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; }
        }
        private readonly Task moveCompletedTasks;
        private readonly Task beginTasks;
        private readonly int threadWorkers;

        public ProtocolHandler(ILogger<ProtocolHandler> logger)
        {
            log = logger;
            finished = new Queue<TaskResultDTO>();
            running = new List<Task<TaskResultDTO>>();
            queued = new Queue<Task<TaskResultDTO>>();
            moveCompletedTasks = new Task(() => { this.MoveCompletedTasks(); }, TaskCreationOptions.LongRunning);
            beginTasks = new Task(() => { this.BeginTasks(); }, TaskCreationOptions.LongRunning);

            ThreadPool.GetMaxThreads(out int worker, out _);
            threadWorkers = worker;
        }

        public void Stop()
        {
            _isRunning = false;
        }
        public void Start()
        {
            _isRunning = true;
            moveCompletedTasks.Start();
            beginTasks.Start();
        }

        private async Task BeginTasks()
        {
            while (_isRunning)
            {
                while (
                    running.Count < threadWorkers && 
                    queued.Count > 0 && 
                    _isRunning)
                {
                    var task = queued.Dequeue();
                    running.Add(task);
                    task.Start();
                }
                await Task.Delay(1000);
            }
        }
        private async Task MoveCompletedTasks()
        {
            while (_isRunning)
            {
                foreach(var task in running)
                {
                    if (task.IsCompleted == true)
                        finished.Enqueue(task.Result);
                }

                int  removed = running.RemoveAll(x => x.IsCompleted);
                if (removed > 0)
                    log.LogInformation($"{DateTime.UtcNow} - removed ({removed}) and moved into completed list");
                await Task.Delay(1000);
            }
        }

        public Queue<TaskResultDTO> GetAndClearResults()
        {
            Queue<TaskResultDTO> queue = new Queue<TaskResultDTO>(finished);
            finished.Clear();
            return queue;
        }
        public void AddRange(List<IProtocol> tasks)
        {
            if (tasks is null || tasks.Count == 0)
                return;

                log.LogInformation($"{DateTime.UtcNow} - adding ({tasks.Count}) tasks to queue");
                foreach (var task in tasks)
                    queued.Enqueue(new Task<TaskResultDTO>(() => task.Execute()));
        }
    }
}
