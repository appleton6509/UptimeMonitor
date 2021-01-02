using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Models
{
    public class ResponseResult
    {
        public DateTime TimeStamp { get; set; }
        public bool IsReachable { get; set; }
        public int Latency { get; set; }
        public string StatusMessage { get; set; }

        public override string ToString()
        {
            return $"{TimeStamp.Date} - Reachable? {IsReachable} - Latency: {Latency}ms - StatusMsg: {StatusMessage}";
        }
    }
}
