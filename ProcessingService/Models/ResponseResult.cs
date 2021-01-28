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

    }
}
