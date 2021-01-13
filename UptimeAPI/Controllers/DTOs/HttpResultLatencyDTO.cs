using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.DTOs
{
    public class HttpResultLatencyDTO
    {
        public DateTime TimeStamp { get; set; }
        public bool IsReachable { get; set; }
        public int Latency { get; set; }
        public Guid EndPointId { get; set; }
    }
}
