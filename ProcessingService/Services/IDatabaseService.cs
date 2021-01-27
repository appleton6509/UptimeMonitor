using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public interface IDatabaseService
    {
        void Create(HttpResult result);
        List<EndPoint> GetAll();
        EndPoint Get(Guid id);
         List<EndPoint> FindNewEndpoints();
    }
}