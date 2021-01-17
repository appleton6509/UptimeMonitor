using Data.Models;
using System.Collections.Generic;

namespace ProcessingService.Services
{
    public interface IDatabaseService
    {
        void Create(HttpResult result);
        List<EndPoint> Get();
    }
}