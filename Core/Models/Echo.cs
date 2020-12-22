using System;

namespace Core.Models
{
    public class Echo
    {
        public Guid ID { get; set; }
        public DateTime TimeStamp { get; set; }
        public int EndPointID { get; set; }
    }
}