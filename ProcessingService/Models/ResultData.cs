using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Models
{
    public class ResultData : Data.Models.ResultData
    {
        public ResultData() { }
        public ResultData(ResponseResult result)
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
            Protocol = result.Protocol;
             
        }
    }
}
