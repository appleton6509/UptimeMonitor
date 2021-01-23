using Data.Models;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;

namespace UptimeAPI.Controllers.Repositories
{
   public interface IEndPointRepository : IRepository<EndPoint>
    {
        public List<EndPointOfflineDTO> GetOfflineEndPoints();
        public List<EndPointOfflineOnlineDTO> GetEndPointsStatus();
        public EndPointStatisticsDTO GetEndPointStatistics(EndPoint endPoint);
        public List<EndPointStatisticsDTO> GetEndPointStatistics();
    } 
}
