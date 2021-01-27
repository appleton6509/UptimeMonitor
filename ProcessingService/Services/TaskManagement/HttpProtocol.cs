using Data.Models;


namespace ProcessingService.Services
{
    public class HttpProtocol : IProtocol
    {
        private readonly IHttpService _proc;
        private readonly EndPoint _ep;

        public HttpProtocol(IHttpService proc, EndPoint ep)
        {
            _proc = proc;
            _ep = ep;
        }

        public TaskResult Execute()
        {
            var res = _proc.CheckConnection(_ep).Result;
            TaskResult task = new TaskResult()
            {
                Response = res,
                EmailOnCompletion = false
            };

            return task;
        }
    }
}
