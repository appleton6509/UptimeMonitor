using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Models
{
    public class ResponseResult
    {
        public object Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsReachable { get; set; }
        public int Latency { get; set; }
        public string StatusMessage { get; set; }
        public Protocol Protocol { get; set; }

        public static ResponseResult Create(EndPointExtended endpoint)
        {
            ResponseResult result = new ResponseResult()
            {
                Id = endpoint.Id,
                Protocol = endpoint.Protocol,
                TimeStamp = DateTime.UtcNow,
                IsReachable = false,
            };
            return result;
        }
        public void UpdateAsFailed(string errorMessage)
        {
            this.IsReachable = false;
            this.StatusMessage = errorMessage;
        }
        public void UpdateAsSuccessful(int latency)
        {
            this.Latency = latency;
            this.IsReachable = true;
        }

    }
}
