using Data.Models;


namespace ProcessingService.Services
{
    public class HttpProtocol : IProtocol
    {
        private readonly IHttpService _proc;
        private readonly EndPointExtended _ep;

        public HttpProtocol(IHttpService proc, EndPointExtended ep)
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
                NotifyOnFailure = _ep.NotifyOnFailure,
                Email = _ep.Email
            };

            return task;
        }
    }
}
