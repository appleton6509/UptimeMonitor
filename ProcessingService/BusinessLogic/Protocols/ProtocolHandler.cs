
using Microsoft.Extensions.Logging;
using ProcessingService.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessingService.BusinessLogic.Protocols
{
    public class ProtocolHandler : IObservable<TaskResultDTO>
    {
        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<TaskResultDTO>> observers;
            private readonly IObserver<TaskResultDTO> observer;
            public Unsubscriber(List<IObserver<TaskResultDTO>> observers, IObserver<TaskResultDTO> observer)
            {
                this.observer = observer;
                this.observers = observers;
            }
            public void Dispose()
            {
                if (observer != null)
                    observers.Remove(observer);
            }
        }

        private readonly Queue<Task<TaskResultDTO>> _queued;
        private readonly List<Task<TaskResultDTO>> _running;
        private readonly Queue<TaskResultDTO> _finished;
        private readonly ILogger<ProtocolHandler> _log;
        private readonly Task _moveCompleted;
        private readonly Task _beginTasks;
        private readonly int _threadWorkers;
        private readonly List<IObserver<TaskResultDTO>> _observers;
        private bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; }
        }

        public ProtocolHandler(ILogger<ProtocolHandler> logger) 
        {
            _log = logger;
            _finished = new Queue<TaskResultDTO>();
            _running = new List<Task<TaskResultDTO>>();
            _queued = new Queue<Task<TaskResultDTO>>();
            _moveCompleted = new Task( () => { this.MoveCompletedTasks(); }, TaskCreationOptions.LongRunning);
            _beginTasks = new Task(() => { this.BeginTasks(); }, TaskCreationOptions.LongRunning);
            _observers = new List<IObserver<TaskResultDTO>>();

            ThreadPool.GetMaxThreads(out int worker, out _);
            _threadWorkers = worker;
        }

        public IDisposable Subscribe(IObserver<TaskResultDTO> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                var numerate = _finished.GetEnumerator();
                while (numerate.MoveNext())
                    observer.OnNext(numerate.Current);
            }
            return new Unsubscriber(_observers, observer);
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Start()
        {
            _isRunning = true;
            _moveCompleted.Start();
            _beginTasks.Start();
        }

        public void ExecuteImmediately(IProtocol task)
        {
            Task<TaskResultDTO> t1 = Task.Run(() => task.Execute());
            t1.Wait();
            var result = t1.Result;
            _finished.Enqueue(result);
            _observers.ForEach(x => x.OnNext(result));
        }

        public void AddRange(List<IProtocol> tasks)
        {
            if (tasks is null || tasks.Count == 0)
                return;

            _log.LogInformation($"{DateTime.UtcNow} - adding ({tasks.Count}) tasks to queue");
            foreach (var task in tasks)
                _queued.Enqueue(new Task<TaskResultDTO>(() => task.Execute()));
        }

        private async Task BeginTasks()
        {
            while (_isRunning)
            {
                await AddTasksToRunning();
                await Task.Delay(1000);
            }
        }

        private async Task AddTasksToRunning()
        {
           await Task.Run(() =>
            {
                while (
                    _running.Count < _threadWorkers &&
                    _queued.Count > 0 &&
                    _isRunning)
                {
                    var task = _queued.Dequeue();
                    _running.Add(task);
                    task.Start();
                }
            });
        }

        private async Task MoveCompletedTasks()
        {
            while (_isRunning)
            {
                await MoveTasksWithResults();
                await RemoveTasksThatAreCompleted();
                _observers.ForEach(x => x.OnCompleted());
                await Task.Delay(1000);
            }
        }

        private async Task MoveTasksWithResults()
        {
            await Task.Run(() =>
            {
                foreach (var task in _running)
                {
                    if (task.IsCompleted == true)
                    {
                        _finished.Enqueue(task.Result);
                        _observers.ForEach(x => x.OnNext(task.Result));
                    }
                }
            });
        }

        private async Task RemoveTasksThatAreCompleted()
        {
           await Task.Run(() =>
            {
                int removeCount = _running.Count;
                if (removeCount > 0)
                {
                    _running.RemoveAll(x => x.IsCompleted);
                    _log.LogInformation($"{DateTime.UtcNow} - removed ({removeCount}) and moved into completed list");
                }
            });
        }
    }
}
