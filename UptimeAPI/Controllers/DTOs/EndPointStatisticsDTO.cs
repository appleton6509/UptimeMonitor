using Data.Models;
using System;

namespace UptimeAPI.Controllers.DTOs
{
    public class EndPointStatisticsDTO
    {
        public Guid Id { get; set; }
        public int AverageLatency { get; set; }
        public DateTime? LastDownTime { get; set; }
        public DateTime? LastSeen { get; set; }
        public string Ip { get; set; }
        public string Description { get; set; }
        public bool IsReachable { get; set; }
        public Protocol Protocol { get; set; }

    }
}
