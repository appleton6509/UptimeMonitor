using System;

namespace Data.Models
{
    public class Echo
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public int StatusCode { get; set; }
        public int Latency { get; set; }
        public string StatusMessage { get; set; }
        public Guid EndPointId { get; set; }
    }
}