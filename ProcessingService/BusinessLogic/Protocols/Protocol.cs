
using ProcessingService.DTO;
using ProcessingService.Models;

namespace ProcessingService.BusinessLogic.Protocols
{
    public class Protocol : IProtocol
    {
        private readonly IProtocolService _service;
        private readonly EndPointExtended _ep;

        public Protocol(IProtocolService service, EndPointExtended ep)
        {
            _service = service;
            _ep = ep;
        }

        public TaskResultDTO Execute()
        {
            var res = _service.CheckConnection(_ep);
            TaskResultDTO task = new TaskResultDTO()
            {
                Response = res,
                EndPoint = this._ep
            };

            return task;
        }
    }
}
