using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public interface IDatabaseService
    {
        void Create(HttpResult result);
        List<EndPoint> Get();
        EndPoint Get(Guid id);
         Task<List<EndPoint>> FindNewEndpoints();
    }
}