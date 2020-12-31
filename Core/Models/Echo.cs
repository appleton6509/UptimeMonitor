using System;

namespace Data.Models
{
    public class Echo
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid EndPointId { get; set; }
    }
}