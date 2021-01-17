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
            if (Object.Equals(result, null)) return null;

            HttpResult http = new HttpResult()
            {
                EndPointId = (Guid)result?.Id,
                Latency = result.Latency,
                IsReachable = result.IsReachable,
                StatusMessage = result.StatusMessage,
                TimeStamp = DateTime.UtcNow
            };
            return http;
        }
    }
}
