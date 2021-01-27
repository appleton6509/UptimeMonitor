using Data.Models;
using ProcessingService.Models;
using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public interface IHttpService
    {
        public Task<ResponseResult> CheckConnection(EndPoint endpoint);
    }
}
