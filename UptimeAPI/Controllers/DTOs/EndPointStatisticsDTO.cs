using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.DTOs
{
    public class EndPointStatisticsDTO
    {
        public int AverageLatency { get; set; }
        public DateTime? LastDownTime { get; set; }
        public DateTime? LastSeen { get; set; }
        public string Ip { get; set; }
        public string Description { get; set; }
        public bool IsReachable { get; set; }

    }
}
