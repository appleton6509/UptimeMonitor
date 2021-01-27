using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Services.TaskManager
{
    public class ProtocolFactory
    {
        private IHttpService _http;
        public ProtocolFactory(IHttpService http)
        {
            _http = http;
        }
        public IProtocol GetTask(EndPoint ep)
        {
            return new HttpProtocol(_http, ep);
        }
        public List<IProtocol> GetTasks(List<EndPoint> eps)
        {
            List<IProtocol> tasks = new List<IProtocol>();
            foreach (var ep in eps)
                tasks.Add(new HttpProtocol(_http, ep));
            return tasks;
        }
    }
}
