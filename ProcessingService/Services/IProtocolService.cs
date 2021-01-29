using Microsoft.Extensions.Logging;
using ProcessingService.Models;


namespace ProcessingService.BusinessLogic
{
    public interface IProtocolService
    {
        public ResponseResult CheckConnection(EndPointExtended endpoint);
    }
}
