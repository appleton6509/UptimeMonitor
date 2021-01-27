using Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public class ProtocolHandler
    {
        private  List<Task<TaskResult>> _tasksToDo;
        private  List<Task<TaskResult>> _tasksRunning;
        private  List<TaskResult> _tasksResults;
        private ILogger _logger;
        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; }
        }
        public int ResultCount { get
            {
                return this._tasksResults.Count;
            } }
        public ProtocolHandler(ILogger logger)            
        {
            _logger = logger;
            _tasksResults = new List<TaskResult>();
            _tasksRunning = new List<Task<TaskResult>>();
            _tasksToDo = new List<Task<TaskResult>>();
        }

        private int NumberOfWorkers { get; set; } = 5000;

        public void Stop()
        {
            _isRunning = false;
        }
        public void Start()
        {
            _isRunning = true;
            Task.Run(() =>
            {
                while (_isRunning)
                {
                    var task1 = Task.Run(async () => { await this.BeginTasks(); });
                    var task2 = Task.Run(async () => { await this.MoveCompletedTasks(); });
                    task1.Wait();
                    task2.Wait();
                }
            });
        }

        private  async Task BeginTasks( )
        {
            while (_isRunning)
            {
                if (_tasksRunning.Count < NumberOfWorkers && _tasksToDo.Count > 0)
                {
                    var task = _tasksToDo[0];
                    _tasksToDo.Remove(task);
                    _tasksRunning.Add(task);
                    task.Start();
                }
                else { await Task.Delay(500); }
            }
        }
        private async Task MoveCompletedTasks()
        {
            while (_isRunning)
            {
                if (_tasksRunning.Count > 0 || _tasksToDo.Count > 0)
                {
                    var completed = _tasksRunning.FindAll(x => x.IsCompleted);
                    if(completed.Count > 0)
                    {
                        _logger.LogInformation($"moving ({completed.Count}) tasks as completed");
                        foreach (var task in completed)
                        {
                            _tasksRunning.Remove(task);
                            _tasksResults.Add(task.Result);
                        }
                    }
                }
                await Task.Delay(1000);
            }
        }
  
        public List<TaskResult> GetAndClearResults()
        {
            List<TaskResult> results = new List<TaskResult>(_tasksResults);
            if (results.Count > 0)
                results.ForEach(x => _tasksResults.Remove(x));
            return results;
        }
        public void Add(IProtocol task)
        {
            _tasksToDo.Add(new Task<TaskResult>(() => task.Execute()));
        }
        public void AddRange(List<IProtocol> tasks)
        {
            foreach(var task in tasks)
                _tasksToDo.Add(new Task<TaskResult>(() => task.Execute()));
        }
    }
}
