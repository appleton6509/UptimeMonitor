using Data.Models;
using ProcessingService.Models;

namespace ProcessingService.Services
{
    public interface IMapService
    {
        HttpResult Map(ResponseResult result);
    }
}