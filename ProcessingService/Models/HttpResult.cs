using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Models
{
    public class HttpResult : Data.Models.HttpResult, IMap
    {
        public HttpResult() { }
        public HttpResult(ResponseResult result)
        {
             this.Map(result);
        }
        public void Map(ResponseResult result)
        {
            if (result is null)
                return;

            EndPointId = result.Id != null ? (Guid)result.Id : Guid.Empty;
            Latency = result.Latency;
            IsReachable = result.IsReachable;
            StatusMessage = result.StatusMessage;
            TimeStamp = DateTime.UtcNow;
        }
    }
}
