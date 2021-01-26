using Data.Models;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Services
{
    public class MapService : IMapService
    {
        public HttpResult Map(ResponseResult result)
        {
            if(result is null)
                return null;

            HttpResult http = new HttpResult()
            {
                EndPointId = result.Id != null ? (Guid)result.Id : Guid.Empty,
                Latency = result.Latency,
                IsReachable = result.IsReachable,
                StatusMessage = result.StatusMessage,
                TimeStamp = DateTime.UtcNow
            };

            return http;
        }
    }
}
